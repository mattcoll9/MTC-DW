Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Services

    Public Class DeputyApiService

        Private Shared ReadOnly _client As HttpClient = CreateClient()

        Private Shared Function CreateClient() As HttpClient
            Dim c As New HttpClient()
            c.Timeout = TimeSpan.FromMinutes(5)
            Return c
        End Function

        Private _baseUrl As String
        Private _token As String

        Public Sub New(baseUrl As String, token As String)
            _baseUrl = baseUrl.TrimEnd("/"c) & "/"
            _token = token
        End Sub

        Private Function BuildRequest(method As HttpMethod, endpoint As String, Optional body As String = Nothing) As HttpRequestMessage
            Dim req As New HttpRequestMessage(method, _baseUrl & endpoint.TrimStart("/"c))
            req.Headers.Add("Authorization", $"OAuth {_token}")
            req.Headers.Add("Accept", "application/json")
            If body IsNot Nothing Then
                req.Content = New StringContent(body, Encoding.UTF8, "application/json")
            End If
            Return req
        End Function

        ''' Core streaming method — POSTs a search body page by page and calls processPage for each batch.
        ''' searchBodyTemplate is a JSON object containing a "search" key (and optionally "join" etc.)
        ''' but WITHOUT "start" or "max" — those are injected per page.
        Public Async Function ForEachPage(endpoint As String, searchBodyTemplate As String,
                                          label As String,
                                          processPage As Action(Of List(Of JObject)),
                                          Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim total As Integer = 0
            Dim pageSize As Integer = 200
            Dim start As Integer = 0

            Do
                ct.ThrowIfCancellationRequested()
                AppState.Activity?.Invoke($"{label} — fetching records {start + 1}…")

                Dim bodyObj = JObject.Parse(searchBodyTemplate)
                bodyObj("start") = start
                bodyObj("max") = pageSize

                Dim req = BuildRequest(HttpMethod.Post, endpoint, bodyObj.ToString(Formatting.None))
                Dim resp = Await _client.SendAsync(req, ct)
                If Not resp.IsSuccessStatusCode Then
                    Dim errBody = Await resp.Content.ReadAsStringAsync()
                    Throw New HttpRequestException($"Deputy API error {CInt(resp.StatusCode)} on {req.RequestUri}: {errBody}")
                End If

                Dim json = Await resp.Content.ReadAsStringAsync()
                Dim page = JsonConvert.DeserializeObject(Of JArray)(json)
                If page Is Nothing OrElse page.Count = 0 Then Exit Do

                processPage(page.Cast(Of JObject)().ToList())
                total += page.Count
                AppState.Activity?.Invoke($"{label} — {total} records saved…")

                If page.Count < pageSize Then Exit Do
                start += pageSize
            Loop

            Return total
        End Function

        ''' Collect all pages into a list (only use for small reference datasets).
        Public Async Function GetAll(endpoint As String, Optional ct As CancellationToken = Nothing) As Task(Of List(Of JObject))
            Dim all As New List(Of JObject)
            Await ForEachPage(endpoint, "{""search"":{}}", endpoint, Sub(page) all.AddRange(page), ct)
            Return all
        End Function

    End Class

End Namespace
