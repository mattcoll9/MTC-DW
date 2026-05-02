Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports MTCDW.Models

Namespace Services

    Public Class RevSportSyncService

        Private _db As DatabaseService
        Private _api As RevSportApiService
        Private _logger As Action(Of String)

        Public Sub New(db As DatabaseService, api As RevSportApiService, Optional logger As Action(Of String) = Nothing)
            _db = db
            _api = api
            _logger = logger
        End Sub

        Private Sub Log(msg As String)
            AppState.Activity?.Invoke(msg)
            _logger?.Invoke(msg)
        End Sub

        ' ── Alias tables: normalized CSV header → DataTable column name ────────

        Private Shared ReadOnly MemberAliases As New Dictionary(Of String, String)(StringComparer.Ordinal) From {
            {"parentbodyid", "ParentBodyId"}, {"parentbody", "ParentBodyId"},
            {"australiansailingnumber", "ParentBodyId"}, {"sailingnumber", "ParentBodyId"}, {"membernumber", "ParentBodyId"},
            {"name", "FullName"}, {"fullname", "FullName"},
            {"dateofbirth", "DateOfBirth"}, {"dob", "DateOfBirth"},
            {"genderword", "Gender"}, {"gender", "Gender"}, {"genderidentity", "Gender"},
            {"creationtime", "CreationTime"}, {"created", "CreationTime"}, {"registrationdate", "CreationTime"},
            {"registeredon", "CreationTime"}, {"registrationtime", "CreationTime"},
            {"address", "Address"},
            {"phonehome", "PhoneHome"}, {"homephone", "PhoneHome"},
            {"phonemob", "PhoneMobile"}, {"mobile", "PhoneMobile"}, {"phonemobile", "PhoneMobile"},
            {"mobilenumber", "PhoneMobile"}, {"mobilephone", "PhoneMobile"},
            {"email", "Email"}, {"emailaddress", "Email"},
            {"paymentstatus", "PaymentStatus"},
            {"paymentdate", "PaymentDate"},
            {"paymentmethod", "PaymentMethod"},
            {"paymentreceipt", "PaymentReceipt"}, {"receiptnumber", "PaymentReceipt"},
            {"paymentwho", "PaymentWho"}, {"collectedby", "PaymentWho"},
            {"deceased", "Deceased"},
            {"lastupdated", "LastUpdated"}, {"lastmodified", "LastUpdated"}, {"lastupdatetime", "LastUpdated"}
        }

        Private Shared ReadOnly EventAliases As New Dictionary(Of String, String)(StringComparer.Ordinal) From {
            {"eventname", "EventName"}, {"name", "EventName"}, {"course", "EventName"}, {"title", "EventName"}, {"coursename", "EventName"},
            {"eventdate", "EventDate"}, {"date", "EventDate"}, {"startdate", "EventDate"}, {"coursedate", "EventDate"},
            {"category", "Category"}, {"eventcategory", "Category"}, {"coursecategory", "Category"},
            {"starttime", "StartTime"}, {"start", "StartTime"},
            {"endtime", "EndTime"}, {"end", "EndTime"},
            {"registered", "Registered"}, {"registrants", "Registered"}, {"totalregistered", "Registered"},
            {"attended", "Attended"}, {"attendees", "Attended"}, {"totalattended", "Attended"},
            {"revenue", "Revenue"}, {"amount", "Revenue"}, {"totalrevenue", "Revenue"}, {"total", "Revenue"}
        }

        Private Shared ReadOnly AttendeeAliases As New Dictionary(Of String, String)(StringComparer.Ordinal) From {
            {"memberid", "MemberId"}, {"parentbodyid", "MemberId"}, {"id", "MemberId"},
            {"membername", "MemberName"}, {"name", "MemberName"}, {"fullname", "MemberName"},
            {"email", "Email"}, {"emailaddress", "Email"},
            {"eventname", "EventName"}, {"course", "EventName"}, {"title", "EventName"}, {"coursename", "EventName"},
            {"eventdate", "EventDate"}, {"date", "EventDate"}, {"startdate", "EventDate"},
            {"category", "Category"}, {"eventcategory", "Category"},
            {"attendancestatus", "AttendanceStatus"}, {"status", "AttendanceStatus"}, {"attendance", "AttendanceStatus"},
            {"amountpaid", "AmountPaid"}, {"paid", "AmountPaid"}, {"amount", "AmountPaid"}, {"revenue", "AmountPaid"}
        }

        ' ── Public sync methods ───────────────────────────────────────────────

        Public Async Function SyncMembersAsync(job As JobDefinition, ct As Threading.CancellationToken) As Task(Of Integer)
            Return Await SyncMembersForSeasonAsync(RevSportApiService.KnownSeasons.Keys.Max(), ct)
        End Function

        Public Async Function SyncAllMembersAsync(job As JobDefinition, ct As Threading.CancellationToken) As Task(Of Integer)
            ' Seed all known seasons into the lookup table up front
            For Each kv In RevSportApiService.KnownSeasons
                _db.UpsertRevSportSeason(kv.Key, kv.Value)
            Next
            Dim total = 0
            For Each kv In RevSportApiService.KnownSeasons
                ct.ThrowIfCancellationRequested()
                total += Await SyncMembersForSeasonAsync(kv.Key, ct)
            Next
            Return total
        End Function

        Private Async Function SyncMembersForSeasonAsync(seasonId As Integer, ct As Threading.CancellationToken) As Task(Of Integer)
            Log($"RevSport: downloading members CSV for season {seasonId}…")
            Dim csv = Await _api.DownloadCsvAsync(BuildMembersUrl(seasonId), "https://portal.revolutionise.com.au/bsyc/members/reports")
            Log($"RevSport: CSV received ({csv.Length} chars), parsing…")
            Dim tmpFile = IO.Path.Combine(IO.Path.GetTempPath(), $"revsport-members-{seasonId}.csv")
            IO.File.WriteAllText(tmpFile, csv, System.Text.Encoding.UTF8)
            Log($"RevSport: CSV saved to {tmpFile}")
            Dim firstLine = If(csv.IndexOf(vbLf) > 0, csv.Substring(0, csv.IndexOf(vbLf)).Trim(), csv.Substring(0, Math.Min(500, csv.Length)))
            Log($"RevSport: CSV header row: {firstLine}")
            Dim dt = ParseMembersCsv(csv, seasonId)
            Log($"RevSport: parsed {dt.Rows.Count} member rows")
            If dt.Rows.Count = 0 Then
                Log($"RevSport: season {seasonId} returned 0 members — preserving existing data (archived or not yet populated)")
                Return 0
            End If
            Using conn = _db.GetConnection()
                Using cmd As New SqlCommand("DELETE FROM revsport.Members WHERE SeasonId=@s", conn)
                    cmd.Parameters.AddWithValue("@s", seasonId)
                    cmd.ExecuteNonQuery()
                End Using
                Using bcp As New SqlBulkCopy(conn)
                    bcp.DestinationTableName = "revsport.Members"
                    bcp.BulkCopyTimeout = 120
                    MapColumns(bcp, dt)
                    Await bcp.WriteToServerAsync(dt)
                End Using
            End Using
            ' Record this season in the lookup table (handles seasons not in KnownSeasons)
            Dim label As String = Nothing
            If Not RevSportApiService.KnownSeasons.TryGetValue(seasonId, label) Then
                label = $"Season {seasonId}"
            End If
            _db.UpsertRevSportSeason(seasonId, label)
            Return dt.Rows.Count
        End Function

        Public Async Function SyncEventsAsync(job As JobDefinition, ct As Threading.CancellationToken) As Task(Of Integer)
            Dim dateFrom As Date, dateTo As Date
            GetDateRange(job, dateFrom, dateTo)

            Log($"RevSport: downloading events {dateFrom:dd/MM/yyyy}–{dateTo:dd/MM/yyyy}…")
            Dim csv = Await _api.DownloadCsvAsync(BuildEventsUrl(dateFrom, dateTo, "per-event"), "https://portal.revolutionise.com.au/bsyc/events/reports")
            Log($"RevSport: CSV received ({csv.Length} chars), parsing…")
            Dim tmpFile = IO.Path.Combine(IO.Path.GetTempPath(), $"revsport-events-{dateFrom:yyyyMMdd}-{dateTo:yyyyMMdd}.csv")
            IO.File.WriteAllText(tmpFile, csv, System.Text.Encoding.UTF8)
            Log($"RevSport: CSV saved to {tmpFile}")
            Dim firstLine = If(csv.IndexOf(vbLf) > 0, csv.Substring(0, csv.IndexOf(vbLf)).Trim(), csv.Substring(0, Math.Min(500, csv.Length)))
            Log($"RevSport: CSV header row: {firstLine}")
            Dim dt = ParseEventsCsv(csv, dateFrom, dateTo)
            Log($"RevSport: importing {dt.Rows.Count} event rows…")
            Using conn = _db.GetConnection()
                Using cmd As New SqlCommand("DELETE FROM revsport.Events WHERE DateStart=@s AND DateEnd=@e", conn)
                    cmd.Parameters.AddWithValue("@s", dateFrom)
                    cmd.Parameters.AddWithValue("@e", dateTo)
                    cmd.ExecuteNonQuery()
                End Using
                Using bcp As New SqlBulkCopy(conn)
                    bcp.DestinationTableName = "revsport.Events"
                    bcp.BulkCopyTimeout = 120
                    MapColumns(bcp, dt)
                    Await bcp.WriteToServerAsync(dt)
                End Using
            End Using
            Return dt.Rows.Count
        End Function

        Public Async Function SyncEventAttendeesAsync(job As JobDefinition, ct As Threading.CancellationToken) As Task(Of Integer)
            Dim dateFrom As Date, dateTo As Date
            GetDateRange(job, dateFrom, dateTo)

            Log($"RevSport: downloading event attendees {dateFrom:dd/MM/yyyy}–{dateTo:dd/MM/yyyy}…")
            Dim csv = Await _api.DownloadCsvAsync(BuildEventsUrl(dateFrom, dateTo, "per-member"), "https://portal.revolutionise.com.au/bsyc/events/reports")
            Log($"RevSport: CSV received ({csv.Length} chars), parsing…")
            Dim tmpFile = IO.Path.Combine(IO.Path.GetTempPath(), $"revsport-attendees-{dateFrom:yyyyMMdd}-{dateTo:yyyyMMdd}.csv")
            IO.File.WriteAllText(tmpFile, csv, System.Text.Encoding.UTF8)
            Log($"RevSport: CSV saved to {tmpFile}")
            Dim firstLine = If(csv.IndexOf(vbLf) > 0, csv.Substring(0, csv.IndexOf(vbLf)).Trim(), csv.Substring(0, Math.Min(500, csv.Length)))
            Log($"RevSport: CSV header row: {firstLine}")
            Dim dt = ParseAttendeesCsv(csv, dateFrom, dateTo)
            Log($"RevSport: importing {dt.Rows.Count} attendee rows…")
            Using conn = _db.GetConnection()
                Using cmd As New SqlCommand("DELETE FROM revsport.EventAttendees WHERE DateStart=@s AND DateEnd=@e", conn)
                    cmd.Parameters.AddWithValue("@s", dateFrom)
                    cmd.Parameters.AddWithValue("@e", dateTo)
                    cmd.ExecuteNonQuery()
                End Using
                Using bcp As New SqlBulkCopy(conn)
                    bcp.DestinationTableName = "revsport.EventAttendees"
                    bcp.BulkCopyTimeout = 120
                    MapColumns(bcp, dt)
                    Await bcp.WriteToServerAsync(dt)
                End Using
            End Using
            Return dt.Rows.Count
        End Function

        ' ── Date range helper ─────────────────────────────────────────────────

        Private Sub GetDateRange(job As JobDefinition, ByRef dateFrom As Date, ByRef dateTo As Date)
            If job.SyncFromDate.HasValue AndAlso job.SyncToDate.HasValue Then
                dateFrom = job.SyncFromDate.Value
                dateTo = job.SyncToDate.Value
            Else
                Dim daysBack As Integer = 90
                Integer.TryParse(_db.GetConfigValue("RevSport.EventsDaysBack"), daysBack)
                If daysBack <= 0 Then daysBack = 90
                dateFrom = DateTime.Today.AddDays(-daysBack)
                dateTo = DateTime.Today
            End If
        End Sub

        ' ── URL builders ─────────────────────────────────────────────────────

        Private Shared Function BuildMembersUrl(seasonId As Integer) As String
            Return "https://portal.revolutionise.com.au/bsyc/members/reports/download" &
                   "?template=&date_type=creation_time" &
                   "&template_creation_time[start]=&template_creation_time[end]=" &
                   "&template_payment_date[start]=&template_payment_date[end]=" &
                   "&fields[]=parentBodyID&fields[]=name&fields[]=dateOfBirth&fields[]=gender_word" &
                   "&fields[]=creation_time&fields[]=address&fields[]=phoneHome&fields[]=phoneMob" &
                   "&fields[]=email&fields[]=payment_status&fields[]=payment_date" &
                   "&fields[]=payment_method&fields[]=payment_receipt&fields[]=payment_who" &
                   "&fields[]=deceased&fields[]=last_updated" &
                   "&paid=&payment_method=&payment_date[start]=&payment_date[end]=" &
                   "&dob_filter=&gender=&deceased=&receiveUpdates=&receive_sms=&email_valid=" &
                   "&creation_time[start]=&creation_time[end]=" &
                   "&temp_date[start]=&temp_date[end]=" &
                   "&last_updated[start]=&last_updated[end]=" &
                   "&season_id=" & seasonId &
                   "&order=surname&direction=asc&name_format=last_first&address_format=merged&report-format=csv&template_name="
        End Function

        Private Shared Function BuildEventsUrl(dateFrom As Date, dateTo As Date, reportType As String) As String
            Dim ds = Uri.EscapeDataString(dateFrom.ToString("dd/MM/yyyy"))
            Dim de = Uri.EscapeDataString(dateTo.ToString("dd/MM/yyyy"))
            Return "https://portal.revolutionise.com.au/bsyc/events/reports/attendance" & _
                   "?report_type=" & reportType & _
                   "&dateStart=" & ds & "&dateEnd=" & de & _
                   "&course_id=&category_id=" & _
                   "&include_finances=on&include_category=on&include_att_status=on" & _
                   "&report-format=csv"
        End Function

        ' ── CSV parse methods ─────────────────────────────────────────────────

        Private Shared Function ParseMembersCsv(csv As String, seasonId As Integer) As DataTable
            Dim dt As New DataTable()
            dt.Columns.Add("SeasonId", GetType(Integer))
            dt.Columns.Add("ParentBodyId", GetType(String))
            dt.Columns.Add("FullName", GetType(String))
            dt.Columns.Add("DateOfBirth", GetType(Object))
            dt.Columns.Add("Gender", GetType(String))
            dt.Columns.Add("CreationTime", GetType(Object))
            dt.Columns.Add("Address", GetType(String))
            dt.Columns.Add("PhoneHome", GetType(String))
            dt.Columns.Add("PhoneMobile", GetType(String))
            dt.Columns.Add("Email", GetType(String))
            dt.Columns.Add("PaymentStatus", GetType(String))
            dt.Columns.Add("PaymentDate", GetType(Object))
            dt.Columns.Add("PaymentMethod", GetType(String))
            dt.Columns.Add("PaymentReceipt", GetType(String))
            dt.Columns.Add("PaymentWho", GetType(String))
            dt.Columns.Add("Deceased", GetType(Object))
            dt.Columns.Add("LastUpdated", GetType(Object))

            Using sr As New StringReader(csv)
                Using tfp As New TextFieldParser(sr)
                    tfp.TextFieldType = FieldType.Delimited
                    tfp.SetDelimiters(",")
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.TrimWhiteSpace = True
                    If tfp.EndOfData Then Return dt
                    Dim colMap = BuildColumnMap(tfp.ReadFields(), MemberAliases)
                    Dim idx As Integer
                    While Not tfp.EndOfData
                        Dim f = tfp.ReadFields()
                        If f Is Nothing Then Continue While
                        Dim row = dt.NewRow()
                        row("SeasonId") = seasonId
                        Dim pid = If(colMap.TryGetValue("ParentBodyId", idx), GetStr(f, idx), DBNull.Value)
                        If pid Is DBNull.Value Then Continue While
                        row("ParentBodyId") = pid
                        row("FullName") = If(colMap.TryGetValue("FullName", idx), GetStr(f, idx), DBNull.Value)
                        row("DateOfBirth") = If(colMap.TryGetValue("DateOfBirth", idx), GetDate(f, idx), DBNull.Value)
                        row("Gender") = If(colMap.TryGetValue("Gender", idx), GetStr(f, idx), DBNull.Value)
                        row("CreationTime") = If(colMap.TryGetValue("CreationTime", idx), GetDate(f, idx), DBNull.Value)
                        row("Address") = If(colMap.TryGetValue("Address", idx), GetStr(f, idx), DBNull.Value)
                        row("PhoneHome") = If(colMap.TryGetValue("PhoneHome", idx), GetStr(f, idx), DBNull.Value)
                        row("PhoneMobile") = If(colMap.TryGetValue("PhoneMobile", idx), GetStr(f, idx), DBNull.Value)
                        row("Email") = If(colMap.TryGetValue("Email", idx), GetStr(f, idx), DBNull.Value)
                        row("PaymentStatus") = If(colMap.TryGetValue("PaymentStatus", idx), GetStr(f, idx), DBNull.Value)
                        row("PaymentDate") = If(colMap.TryGetValue("PaymentDate", idx), GetDate(f, idx), DBNull.Value)
                        row("PaymentMethod") = If(colMap.TryGetValue("PaymentMethod", idx), GetStr(f, idx), DBNull.Value)
                        row("PaymentReceipt") = If(colMap.TryGetValue("PaymentReceipt", idx), GetStr(f, idx), DBNull.Value)
                        row("PaymentWho") = If(colMap.TryGetValue("PaymentWho", idx), GetStr(f, idx), DBNull.Value)
                        row("Deceased") = If(colMap.TryGetValue("Deceased", idx), GetBool(f, idx), DBNull.Value)
                        row("LastUpdated") = If(colMap.TryGetValue("LastUpdated", idx), GetDate(f, idx), DBNull.Value)
                        dt.Rows.Add(row)
                    End While
                End Using
            End Using
            Return dt
        End Function

        Private Shared Function ParseEventsCsv(csv As String, dateFrom As Date, dateTo As Date) As DataTable
            Dim dt As New DataTable()
            dt.Columns.Add("DateStart", GetType(Date))
            dt.Columns.Add("DateEnd", GetType(Date))
            dt.Columns.Add("EventName", GetType(String))
            dt.Columns.Add("EventDate", GetType(Object))
            dt.Columns.Add("Category", GetType(String))
            dt.Columns.Add("StartTime", GetType(String))
            dt.Columns.Add("EndTime", GetType(String))
            dt.Columns.Add("Registered", GetType(Object))
            dt.Columns.Add("Attended", GetType(Object))
            dt.Columns.Add("Revenue", GetType(Object))

            Using sr As New StringReader(csv)
                Using tfp As New TextFieldParser(sr)
                    tfp.TextFieldType = FieldType.Delimited
                    tfp.SetDelimiters(",")
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.TrimWhiteSpace = True
                    If tfp.EndOfData Then Return dt
                    Dim colMap = BuildColumnMap(tfp.ReadFields(), EventAliases)
                    Dim idx As Integer
                    While Not tfp.EndOfData
                        Dim f = tfp.ReadFields()
                        If f Is Nothing Then Continue While
                        Dim row = dt.NewRow()
                        row("DateStart") = dateFrom
                        row("DateEnd") = dateTo
                        row("EventName") = If(colMap.TryGetValue("EventName", idx), GetStr(f, idx), DBNull.Value)
                        row("EventDate") = If(colMap.TryGetValue("EventDate", idx), GetDate(f, idx), DBNull.Value)
                        row("Category") = If(colMap.TryGetValue("Category", idx), GetStr(f, idx), DBNull.Value)
                        row("StartTime") = If(colMap.TryGetValue("StartTime", idx), GetStr(f, idx), DBNull.Value)
                        row("EndTime") = If(colMap.TryGetValue("EndTime", idx), GetStr(f, idx), DBNull.Value)
                        row("Registered") = If(colMap.TryGetValue("Registered", idx), GetInt(f, idx), DBNull.Value)
                        row("Attended") = If(colMap.TryGetValue("Attended", idx), GetInt(f, idx), DBNull.Value)
                        row("Revenue") = If(colMap.TryGetValue("Revenue", idx), GetDecimal(f, idx), DBNull.Value)
                        dt.Rows.Add(row)
                    End While
                End Using
            End Using
            Return dt
        End Function

        Private Shared Function ParseAttendeesCsv(csv As String, dateFrom As Date, dateTo As Date) As DataTable
            Dim dt As New DataTable()
            dt.Columns.Add("DateStart", GetType(Date))
            dt.Columns.Add("DateEnd", GetType(Date))
            dt.Columns.Add("MemberId", GetType(String))
            dt.Columns.Add("MemberName", GetType(String))
            dt.Columns.Add("Email", GetType(String))
            dt.Columns.Add("EventName", GetType(String))
            dt.Columns.Add("EventDate", GetType(Object))
            dt.Columns.Add("Category", GetType(String))
            dt.Columns.Add("AttendanceStatus", GetType(String))
            dt.Columns.Add("AmountPaid", GetType(Object))

            Using sr As New StringReader(csv)
                Using tfp As New TextFieldParser(sr)
                    tfp.TextFieldType = FieldType.Delimited
                    tfp.SetDelimiters(",")
                    tfp.HasFieldsEnclosedInQuotes = True
                    tfp.TrimWhiteSpace = True
                    If tfp.EndOfData Then Return dt
                    Dim colMap = BuildColumnMap(tfp.ReadFields(), AttendeeAliases)
                    Dim idx As Integer
                    While Not tfp.EndOfData
                        Dim f = tfp.ReadFields()
                        If f Is Nothing Then Continue While
                        Dim row = dt.NewRow()
                        row("DateStart") = dateFrom
                        row("DateEnd") = dateTo
                        row("MemberId") = If(colMap.TryGetValue("MemberId", idx), GetStr(f, idx), DBNull.Value)
                        row("MemberName") = If(colMap.TryGetValue("MemberName", idx), GetStr(f, idx), DBNull.Value)
                        row("Email") = If(colMap.TryGetValue("Email", idx), GetStr(f, idx), DBNull.Value)
                        row("EventName") = If(colMap.TryGetValue("EventName", idx), GetStr(f, idx), DBNull.Value)
                        row("EventDate") = If(colMap.TryGetValue("EventDate", idx), GetDate(f, idx), DBNull.Value)
                        row("Category") = If(colMap.TryGetValue("Category", idx), GetStr(f, idx), DBNull.Value)
                        row("AttendanceStatus") = If(colMap.TryGetValue("AttendanceStatus", idx), GetStr(f, idx), DBNull.Value)
                        row("AmountPaid") = If(colMap.TryGetValue("AmountPaid", idx), GetDecimal(f, idx), DBNull.Value)
                        dt.Rows.Add(row)
                    End While
                End Using
            End Using
            Return dt
        End Function

        ' ── Column mapping helpers ────────────────────────────────────────────

        ' Returns: DataTable column name → CSV column index
        Private Shared Function BuildColumnMap(headers As String(), aliases As Dictionary(Of String, String)) As Dictionary(Of String, Integer)
            Dim result As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
            For i = 0 To headers.Length - 1
                Dim normalized = NormalizeHeader(headers(i))
                Dim schemaCol As String = Nothing
                If aliases.TryGetValue(normalized, schemaCol) AndAlso Not result.ContainsKey(schemaCol) Then
                    result(schemaCol) = i
                End If
            Next
            Return result
        End Function

        Private Shared Function NormalizeHeader(s As String) As String
            Return s.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "").Trim()
        End Function

        Private Shared Sub MapColumns(bcp As SqlBulkCopy, dt As DataTable)
            For Each col As DataColumn In dt.Columns
                bcp.ColumnMappings.Add(col.ColumnName, col.ColumnName)
            Next
        End Sub

        ' ── Typed field extractors ────────────────────────────────────────────

        Private Shared Function GetStr(f As String(), idx As Integer) As Object
            If idx >= f.Length Then Return DBNull.Value
            Dim s = f(idx).Trim()
            Return If(String.IsNullOrEmpty(s), CObj(DBNull.Value), s)
        End Function

        Private Shared Function GetDate(f As String(), idx As Integer) As Object
            If idx >= f.Length Then Return DBNull.Value
            Dim s = f(idx).Trim()
            If String.IsNullOrEmpty(s) Then Return DBNull.Value
            Dim d As Date
            If Date.TryParse(s, d) Then Return CObj(d)
            If Date.TryParseExact(s, New String() {"d/MM/yyyy", "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd"}, _
                                  Nothing, Globalization.DateTimeStyles.None, d) Then Return CObj(d)
            Return DBNull.Value
        End Function

        Private Shared Function GetDecimal(f As String(), idx As Integer) As Object
            If idx >= f.Length Then Return DBNull.Value
            Dim s = f(idx).Trim().Replace("$", "").Replace(",", "")
            If String.IsNullOrEmpty(s) Then Return DBNull.Value
            Dim d As Decimal
            Return If(Decimal.TryParse(s, d), CObj(d), DBNull.Value)
        End Function

        Private Shared Function GetInt(f As String(), idx As Integer) As Object
            If idx >= f.Length Then Return DBNull.Value
            Dim s = f(idx).Trim()
            If String.IsNullOrEmpty(s) Then Return DBNull.Value
            Dim n As Integer
            Return If(Integer.TryParse(s, n), CObj(n), DBNull.Value)
        End Function

        Private Shared Function GetBool(f As String(), idx As Integer) As Object
            If idx >= f.Length Then Return DBNull.Value
            Dim s = f(idx).Trim().ToLower()
            If String.IsNullOrEmpty(s) Then Return DBNull.Value
            Select Case s
                Case "yes", "true", "1", "y" : Return CObj(True)
                Case "no", "false", "0", "n" : Return CObj(False)
                Case Else : Return DBNull.Value
            End Select
        End Function

    End Class

End Namespace
