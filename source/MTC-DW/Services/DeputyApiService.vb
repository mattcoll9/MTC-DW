Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace Services

    Public Class DeputyApiService

        Private Shared ReadOnly _client As New HttpClient()
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

        ''' Paginate a GET resource endpoint, returning all records.
        Public Async Function GetAllPaged(endpoint As String) As Task(Of List(Of JObject))
            Dim all As New List(Of JObject)
            Dim pageSize As Integer = 100
            Dim start As Integer = 0

            Do
                Dim req = BuildRequest(HttpMethod.Get, $"{endpoint}?max={pageSize}&start={start}")
                Dim resp = Await _client.SendAsync(req)
                resp.EnsureSuccessStatusCode()

                Dim json = Await resp.Content.ReadAsStringAsync()
                Dim page = JsonConvert.DeserializeObject(Of JArray)(json)
                If page Is Nothing OrElse page.Count = 0 Then Exit Do

                all.AddRange(page.Cast(Of JObject)())
                If page.Count < pageSize Then Exit Do
                start += pageSize
            Loop

            Return all
        End Function

        ''' Paginate a POST QUERY endpoint (Deputy-style), returning all records.
        Public Async Function PostQuery(endpoint As String, bodyTemplate As String) As Task(Of List(Of JObject))
            Dim all As New List(Of JObject)
            Dim pageSize As Integer = 100
            Dim start As Integer = 0

            Do
                Dim pageBody = InjectPagination(bodyTemplate, start, pageSize)
                Dim req = BuildRequest(HttpMethod.Post, endpoint, pageBody)
                Dim resp = Await _client.SendAsync(req)
                resp.EnsureSuccessStatusCode()

                Dim json = Await resp.Content.ReadAsStringAsync()
                Dim parsed = JToken.Parse(json)

                Dim items As JArray
                If parsed.Type = JTokenType.Array Then
                    items = CType(parsed, JArray)
                ElseIf parsed("results") IsNot Nothing AndAlso parsed("results").Type = JTokenType.Array Then
                    items = CType(parsed("results"), JArray)
                Else
                    Exit Do
                End If

                If items.Count = 0 Then Exit Do
                all.AddRange(items.Cast(Of JObject)())
                If items.Count < pageSize Then Exit Do
                start += pageSize
            Loop

            Return all
        End Function

        Private Function InjectPagination(body As String, start As Integer, max As Integer) As String
            Dim obj As JObject = If(String.IsNullOrWhiteSpace(body), New JObject(), JObject.Parse(body))
            obj("start") = start
            obj("max") = max
            Return obj.ToString(Formatting.None)
        End Function

    End Class

End Namespace
