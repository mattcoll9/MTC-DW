Imports System.Net
Imports System.Net.Http
Imports System.Security.Cryptography

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

        Public Async Function DownloadCsvAsync(url As String) As Task(Of String)
            If Not _loggedIn Then Await LoginAsync()
            Dim resp = Await _client.GetAsync(url)
            ' If we got HTML back instead of CSV, session expired — re-auth and retry once
            If resp.Content.Headers.ContentType?.MediaType?.ToLower().Contains("html") Then
                _loggedIn = False
                Await LoginAsync()
                resp = Await _client.GetAsync(url)
            End If
            If resp.Content.Headers.ContentType?.MediaType?.ToLower().Contains("html") Then
                Throw New InvalidOperationException("RevSport returned an HTML page instead of CSV after re-authentication. Check credentials.")
            End If
            If Not resp.IsSuccessStatusCode Then
                Throw New HttpRequestException($"RevSport download failed ({resp.StatusCode})")
            End If
            Return Await resp.Content.ReadAsStringAsync()
        End Function

        Public Async Function LoginAsync() As Task(Of Boolean)
            ' Step 1: GET login page — server sets XSRF-TOKEN cookie
            Await _client.GetAsync(BASE_URL & LOGIN_PATH)

            ' Step 2: POST credentials with CSRF token
            Dim xsrf = GetXsrfToken()
            Dim loginReq As New HttpRequestMessage(HttpMethod.Post, BASE_URL & LOGIN_PATH)
            loginReq.Headers.Add("X-XSRF-TOKEN", xsrf)
            loginReq.Content = New FormUrlEncodedContent(New List(Of KeyValuePair(Of String, String)) From {
                New KeyValuePair(Of String, String)("_token", xsrf),
                New KeyValuePair(Of String, String)("username", _username),
                New KeyValuePair(Of String, String)("password", _password)
            })
            Dim loginResp = Await _client.SendAsync(loginReq)
            Dim loginHtml = Await loginResp.Content.ReadAsStringAsync()

            ' Step 3: Handle 2FA modal — detected by form id "otpModal" in response HTML
            If loginHtml.Contains("otpModal") Then
                xsrf = GetXsrfToken()
                Dim code = GenerateTotp(_totpSeed)
                ' Portal uses six individual digit inputs: otp[] x 6
                Dim tfaPairs As New List(Of KeyValuePair(Of String, String)) From {
                    New KeyValuePair(Of String, String)("_token", xsrf)
                }
                For Each digit As Char In code
                    tfaPairs.Add(New KeyValuePair(Of String, String)("otp[]", digit.ToString()))
                Next
                Dim tfaReq As New HttpRequestMessage(HttpMethod.Post, BASE_URL & TFA_PATH)
                tfaReq.Headers.Add("X-XSRF-TOKEN", xsrf)
                tfaReq.Headers.Add("Referer", BASE_URL & LOGIN_PATH)
                tfaReq.Content = New FormUrlEncodedContent(tfaPairs)
                Dim tfaResp = Await _client.SendAsync(tfaReq)
                Dim tfaHtml = Await tfaResp.Content.ReadAsStringAsync()
                _loggedIn = Not (tfaHtml.Contains("otpModal") OrElse tfaHtml.Contains("name=""username"""))
            Else
                _loggedIn = Not loginHtml.Contains("name=""username""")
            End If

            If Not _loggedIn Then
                Throw New InvalidOperationException("RevSport login failed. Verify RevSport.Email, RevSport.Password, and RevSport.TotpSeed in Settings.")
            End If
            Return _loggedIn
        End Function

        Private Function GetXsrfToken() As String
            Dim cookies = _cookieContainer.GetCookies(New Uri(BASE_URL))
            Dim xsrf = cookies("XSRF-TOKEN")
            If xsrf Is Nothing Then Return String.Empty
            Return Uri.UnescapeDataString(xsrf.Value)
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
