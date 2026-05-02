Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text.RegularExpressions

Namespace Services

    Public Class RevSportApiService

        Private Const BASE_URL As String = "https://portal.revolutionise.com.au"
        Private Const LOGIN_PATH As String = "/bsyc/login"
        Private Const TFA_PATH As String = "/bsyc/tfa"

        Private _client As HttpClient
        Private _cookieContainer As CookieContainer
        Private _username As String
        Private _password As String
        Private _totpSeed As String
        Private _loggedIn As Boolean
        Private _lastDiag As String = ""

        Public Sub New(username As String, password As String, totpSeed As String)
            _username = username
            _password = password
            _totpSeed = totpSeed
            _loggedIn = False
            _cookieContainer = New CookieContainer()
            Dim handler As New HttpClientHandler() With {
                .CookieContainer = _cookieContainer,
                .AllowAutoRedirect = True
            }
            _client = New HttpClient(handler)
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36")
            _client.Timeout = TimeSpan.FromMinutes(5)
        End Sub

        Public Async Function DownloadCsvAsync(url As String, Optional refererUrl As String = Nothing) As Task(Of String)
            If Not _loggedIn Then Await LoginAsync()
            Dim resp = Await FetchCsv(url, refererUrl)
            ' If we got HTML back instead of CSV, session expired — re-auth and retry once
            If resp.Content.Headers.ContentType?.MediaType?.ToLower().Contains("html") Then
                _loggedIn = False
                Await LoginAsync()
                resp = Await FetchCsv(url, refererUrl)
            End If
            If resp.Content.Headers.ContentType?.MediaType?.ToLower().Contains("html") Then
                Dim html = Await resp.Content.ReadAsStringAsync()
                Dim titleMatch = Regex.Match(html, "<title[^>]*>([^<]*)</title>", RegexOptions.IgnoreCase)
                Dim pageTitle = If(titleMatch.Success, titleMatch.Groups(1).Value.Trim(), "unknown")
                Dim dumpFile = IO.Path.Combine(IO.Path.GetTempPath(), "revsport-error.html")
                Try : IO.File.WriteAllText(dumpFile, html, System.Text.Encoding.UTF8) : Catch : End Try
                Throw New InvalidOperationException($"RevSport: download returned HTML (status={resp.StatusCode}, page=""{pageTitle}"") — see {dumpFile}")
            End If
            If Not resp.IsSuccessStatusCode Then
                Throw New HttpRequestException($"RevSport download failed ({resp.StatusCode})")
            End If
            Return Await resp.Content.ReadAsStringAsync()
        End Function

        Private Async Function FetchCsv(url As String, Optional refererUrl As String = Nothing) As Task(Of HttpResponseMessage)
            ' Visit the source page first to warm session, then download
            Dim ref = If(refererUrl, BASE_URL & "/bsyc/members/reports")
            Await _client.GetAsync(ref)
            Dim req As New HttpRequestMessage(HttpMethod.Get, url)
            req.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
            req.Headers.TryAddWithoutValidation("Referer", ref)
            Return Await _client.SendAsync(req)
        End Function

        Public Async Function DownloadCsvPostAsync(postUrl As String, refererUrl As String, formFields As List(Of KeyValuePair(Of String, String))) As Task(Of String)
            If Not _loggedIn Then Await LoginAsync()
            Dim resp = Await PostCsv(postUrl, refererUrl, formFields)
            If resp.Content.Headers.ContentType?.MediaType?.ToLower().Contains("html") Then
                _loggedIn = False
                Await LoginAsync()
                resp = Await PostCsv(postUrl, refererUrl, formFields)
            End If
            If resp.Content.Headers.ContentType?.MediaType?.ToLower().Contains("html") Then
                Dim html = Await resp.Content.ReadAsStringAsync()
                Dim snippet = html.Substring(0, Math.Min(200, html.Length)).Replace(vbCrLf, " ").Replace(vbLf, " ")
                Throw New InvalidOperationException($"RevSport returned HTML after re-auth ({resp.StatusCode}). Diag: {_lastDiag}")
            End If
            If Not resp.IsSuccessStatusCode Then
                Throw New HttpRequestException($"RevSport download failed ({resp.StatusCode}). Diag: {_lastDiag}")
            End If
            Return Await resp.Content.ReadAsStringAsync()
        End Function

        Private Async Function PostCsv(postUrl As String, refererUrl As String, formFields As List(Of KeyValuePair(Of String, String))) As Task(Of HttpResponseMessage)
            ' GET the report page to ensure XSRF token is fresh, and capture form diagnostics
            Dim pageResp = Await _client.GetAsync(refererUrl)
            Dim pageHtml = Await pageResp.Content.ReadAsStringAsync()
            Dim token = GetXsrfToken()
            ' Collect all <form ...> opening tags so we can see action/method in the error
            Dim sb As New System.Text.StringBuilder()
            For Each m As Match In Regex.Matches(pageHtml, "<form\b[^>]*>", RegexOptions.IgnoreCase)
                sb.Append(m.Value.Substring(0, Math.Min(160, m.Value.Length)))
                sb.Append(" || ")
            Next
            _lastDiag = $"pageStatus={pageResp.StatusCode}, tokenLen={token.Length}, forms=[{sb}]"
            Dim pairs As New List(Of KeyValuePair(Of String, String))
            pairs.Add(New KeyValuePair(Of String, String)("_token", token))
            pairs.AddRange(formFields)
            Dim req As New HttpRequestMessage(HttpMethod.Post, postUrl)
            req.Content = New FormUrlEncodedContent(pairs)
            req.Headers.TryAddWithoutValidation("Referer", refererUrl)
            Return Await _client.SendAsync(req)
        End Function

        Public Async Function LoginAsync() As Task(Of Boolean)
            ' Step 1: GET login page — sets XSRF-TOKEN cookie and renders _token hidden field
            Dim loginPageResp = Await _client.GetAsync(BASE_URL & LOGIN_PATH)
            Dim loginPageHtml = Await loginPageResp.Content.ReadAsStringAsync()

            ' Extract _token from the HTML form — the form field and cookie are different encrypted values
            Dim formTokenMatch = Regex.Match(loginPageHtml, "<input[^>]+name=""_token""[^>]+value=""([^""]+)""", RegexOptions.IgnoreCase)
            If Not formTokenMatch.Success Then
                formTokenMatch = Regex.Match(loginPageHtml, "<input[^>]+value=""([^""]+)""[^>]+name=""_token""", RegexOptions.IgnoreCase)
            End If
            Dim xsrf = GetXsrfToken()
            If String.IsNullOrEmpty(xsrf) Then
                Throw New InvalidOperationException("RevSport: no CSRF token received from login page. Check network connectivity to portal.revolutionise.com.au.")
            End If
            Dim formToken = If(formTokenMatch.Success, formTokenMatch.Groups(1).Value, xsrf)

            ' Detect whether the login form uses "email" or "username" as the credential field name
            Dim credFieldName = "email"
            Dim allInputNames = Regex.Matches(loginPageHtml, "<input[^>]+name=""([^""]+)""", RegexOptions.IgnoreCase)
            Dim fieldsDiag As New System.Text.StringBuilder()
            For Each m As Match In allInputNames
                Dim n = m.Groups(1).Value.ToLower()
                fieldsDiag.Append(n & " ")
                If n = "username" Then credFieldName = "username"
            Next

            ' Step 2: POST credentials — _token from HTML form, X-XSRF-TOKEN from cookie
            Dim loginReq As New HttpRequestMessage(HttpMethod.Post, BASE_URL & LOGIN_PATH)
            loginReq.Headers.TryAddWithoutValidation("X-XSRF-TOKEN", xsrf)
            loginReq.Headers.TryAddWithoutValidation("Referer", BASE_URL & LOGIN_PATH)
            loginReq.Content = New FormUrlEncodedContent(New List(Of KeyValuePair(Of String, String)) From {
                New KeyValuePair(Of String, String)("_token", formToken),
                New KeyValuePair(Of String, String)(credFieldName, _username),
                New KeyValuePair(Of String, String)("password", _password)
            })
            Dim loginResp = Await _client.SendAsync(loginReq)
            Dim loginHtml = Await loginResp.Content.ReadAsStringAsync()
            Dim loginTitleM = Regex.Match(loginHtml, "<title[^>]*>([^<]*)</title>", RegexOptions.IgnoreCase)
            Dim loginPageTitle = If(loginTitleM.Success, loginTitleM.Groups(1).Value.Trim(), "unknown")

            ' Step 3: Handle 2FA — detected by otpModal id, /bsyc/tfa form action, or otp[] input
            Dim needs2FA = loginHtml.Contains("otpModal") OrElse loginHtml.Contains(TFA_PATH) OrElse loginHtml.Contains("name=""otp[")
            If needs2FA Then
                ' Explicitly GET the TFA page — ensures the session records we are at the 2FA step
                ' (AllowAutoRedirect followed the login POST redirect internally, but some middleware
                '  requires an explicit navigation to /bsyc/tfa before it will accept the code POST)
                Dim tfaPageResp = Await _client.GetAsync(BASE_URL & TFA_PATH)
                Dim tfaPageHtml = Await tfaPageResp.Content.ReadAsStringAsync()
                Dim tfaTokenMatch = Regex.Match(tfaPageHtml, "<input[^>]+name=""_token""[^>]+value=""([^""]+)""", RegexOptions.IgnoreCase)
                If Not tfaTokenMatch.Success Then
                    tfaTokenMatch = Regex.Match(tfaPageHtml, "<input[^>]+value=""([^""]+)""[^>]+name=""_token""", RegexOptions.IgnoreCase)
                End If
                xsrf = GetXsrfToken()
                Dim tfaFormToken = If(tfaTokenMatch.Success, tfaTokenMatch.Groups(1).Value, xsrf)
                Dim code = GenerateTotp(_totpSeed)
                ' Portal uses six individual digit inputs: otp[] x 6
                Dim tfaPairs As New List(Of KeyValuePair(Of String, String)) From {
                    New KeyValuePair(Of String, String)("_token", tfaFormToken)
                }
                For Each digit As Char In code
                    tfaPairs.Add(New KeyValuePair(Of String, String)("authentication-code[]", digit.ToString()))
                Next
                Dim tfaReq As New HttpRequestMessage(HttpMethod.Post, BASE_URL & TFA_PATH)
                tfaReq.Headers.TryAddWithoutValidation("X-XSRF-TOKEN", xsrf)
                tfaReq.Headers.TryAddWithoutValidation("Referer", BASE_URL & TFA_PATH)
                tfaReq.Content = New FormUrlEncodedContent(tfaPairs)
                Dim tfaResp = Await _client.SendAsync(tfaReq)
                Dim tfaHtml = Await tfaResp.Content.ReadAsStringAsync()
                Dim tfaTitleM = Regex.Match(tfaHtml, "<title[^>]*>([^<]*)</title>", RegexOptions.IgnoreCase)
                Dim tfaPageTitle = If(tfaTitleM.Success, tfaTitleM.Groups(1).Value.Trim(), "unknown")
                _loggedIn = Not (tfaHtml.Contains("otpModal") OrElse tfaHtml.Contains(TFA_PATH) OrElse tfaHtml.Contains("name=""email""") OrElse tfaHtml.Contains("name=""username"""))
                If Not _loggedIn Then
                    Throw New InvalidOperationException($"RevSport: 2FA step rejected (tfaStatus={tfaResp.StatusCode}, tfaPage=""{tfaPageTitle}""). TOTP code was: {code}")
                End If
            Else
                _loggedIn = Not (loginHtml.Contains("name=""email""") OrElse loginHtml.Contains("name=""username"""))
                If Not _loggedIn Then
                    Throw New InvalidOperationException($"RevSport login failed (loginPage=""{loginPageTitle}"", credField={credFieldName}, formFields=[{fieldsDiag}]). Verify credentials.")
                End If
            End If

            ' Verify session is actually established by GETting a protected page
            Dim verifyHtml = Await (Await _client.GetAsync(BASE_URL & "/bsyc/")).Content.ReadAsStringAsync()
            If verifyHtml.Contains("name=""email""") OrElse verifyHtml.Contains("name=""username""") Then
                _loggedIn = False
                Dim cookieSb As New System.Text.StringBuilder()
                For Each c As System.Net.Cookie In _cookieContainer.GetCookies(New Uri(BASE_URL))
                    cookieSb.Append($"{c.Name}(len={c.Value.Length}) ")
                Next
                Dim tokenDiag = $"formToken={If(formTokenMatch.Success, "found", "missing-used-cookie")}"
                Throw New InvalidOperationException($"RevSport: session not established (loginPage=""{loginPageTitle}"", 2fa={needs2FA}, {tokenDiag}). Cookies: [{cookieSb}]")
            End If

            Return _loggedIn
        End Function

        Private Function GetXsrfToken() As String
            Dim cookies = _cookieContainer.GetCookies(New Uri(BASE_URL))
            Dim xsrf = cookies("XSRF-TOKEN")
            If xsrf Is Nothing Then Return String.Empty
            Return Uri.UnescapeDataString(xsrf.Value)
        End Function

        Public Shared Function GenerateTotpCode(seed As String) As String
            Return GenerateTotp(seed)
        End Function

        Private Shared Function GenerateTotp(base32Secret As String) As String
            Dim keyBytes = Base32Decode(base32Secret.ToUpper().Replace(" ", ""))
            Dim counter = CLng(DateTimeOffset.UtcNow.ToUnixTimeSeconds() \ 30)
            Dim counterBytes = BitConverter.GetBytes(counter)
            If BitConverter.IsLittleEndian Then Array.Reverse(counterBytes)
            Using hmac As New HMACSHA1(keyBytes)
                Dim hash = hmac.ComputeHash(counterBytes)
                Dim offset = hash(hash.Length - 1) And &HF
                Dim code = ((CInt(hash(offset)) And &H7F) << 24) Or _
                           ((CInt(hash(offset + 1)) And &HFF) << 16) Or _
                           ((CInt(hash(offset + 2)) And &HFF) << 8) Or _
                           (CInt(hash(offset + 3)) And &HFF)
                Return (code Mod 1000000).ToString("D6")
            End Using
        End Function

        Private Shared Function Base32Decode(s As String) As Byte()
            Const alphabet As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"
            Dim bits As Long = 0
            Dim bitsCount As Integer = 0
            Dim result As New List(Of Byte)()
            For Each c As Char In s.TrimEnd("="c)
                Dim idx = alphabet.IndexOf(c)
                If idx < 0 Then Continue For
                bits = (bits << 5) Or idx
                bitsCount += 5
                If bitsCount >= 8 Then
                    bitsCount -= 8
                    result.Add(CByte((bits >> bitsCount) And &HFF))
                End If
            Next
            Return result.ToArray()
        End Function

        Public Shared ReadOnly Property KnownSeasons As Dictionary(Of Integer, String)
            Get
                Return New Dictionary(Of Integer, String) From {
                    {43649, "2025-26 (Current)"},
                    {36538, "2024-25"},
                    {28884, "2023-24"},
                    {22247, "2022-23"},
                    {16773, "2021-22"},
                    {11900, "2020-21"},
                    {7842, "2019/20"},
                    {5741, "2018/19"},
                    {4325, "2018"}
                }
            End Get
        End Property

    End Class

End Namespace
