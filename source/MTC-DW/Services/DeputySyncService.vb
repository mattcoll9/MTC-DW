Imports System.Data.SqlClient
Imports System.Threading.Tasks
Imports Newtonsoft.Json.Linq

Namespace Services

    Public Class DeputySyncService

        Private _db As DatabaseService
        Private _api As DeputyApiService

        Public Sub New(db As DatabaseService, api As DeputyApiService)
            _db = db
            _api = api
        End Sub

        Public Async Function SyncOperationalUnits() As Task(Of Integer)
            Dim items = Await _api.GetAllPaged("resource/OperationalUnit")
            Dim count As Integer = 0
            Using conn = _db.GetConnection()
                For Each obj In items
                    Dim id = obj.Value(Of Long)("Id")
                    Dim code = obj.Value(Of String)("Code")
                    Dim name = If(obj.Value(Of String)("OperationalUnitName"), obj.Value(Of String)("CompanyName"))
                    Dim active = obj.Value(Of Boolean)("Active")

                    Using cmd As New SqlCommand(
                        "MERGE deputy.OperationalUnits AS t " &
                        "USING (SELECT @id,@code,@name,@active) AS s(Id,Code,UnitName,IsActive) ON t.Id=s.Id " &
                        "WHEN MATCHED THEN UPDATE SET Code=s.Code,UnitName=s.UnitName,IsActive=s.IsActive,SyncedAt=GETDATE() " &
                        "WHEN NOT MATCHED THEN INSERT (Id,Code,UnitName,IsActive) VALUES(s.Id,s.Code,s.UnitName,s.IsActive);", conn)
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.Parameters.AddWithValue("@code", NullOr(code))
                        cmd.Parameters.AddWithValue("@name", NullOr(name))
                        cmd.Parameters.AddWithValue("@active", active)
                        cmd.ExecuteNonQuery()
                        count += 1
                    End Using
                Next
            End Using
            Return count
        End Function

        Public Async Function SyncWorkTypes() As Task(Of Integer)
            Dim items = Await _api.GetAllPaged("resource/WorkType")
            Dim count As Integer = 0
            Using conn = _db.GetConnection()
                For Each obj In items
                    Dim id = obj.Value(Of Long)("Id")
                    Dim code = obj.Value(Of String)("Code")
                    Dim name = obj.Value(Of String)("WorkTypeName")
                    Dim active = obj.Value(Of Boolean)("Active")

                    Using cmd As New SqlCommand(
                        "MERGE deputy.WorkTypes AS t " &
                        "USING (SELECT @id,@code,@name,@active) AS s(Id,Code,WorkTypeName,IsActive) ON t.Id=s.Id " &
                        "WHEN MATCHED THEN UPDATE SET Code=s.Code,WorkTypeName=s.WorkTypeName,IsActive=s.IsActive,SyncedAt=GETDATE() " &
                        "WHEN NOT MATCHED THEN INSERT (Id,Code,WorkTypeName,IsActive) VALUES(s.Id,s.Code,s.WorkTypeName,s.IsActive);", conn)
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.Parameters.AddWithValue("@code", NullOr(code))
                        cmd.Parameters.AddWithValue("@name", NullOr(name))
                        cmd.Parameters.AddWithValue("@active", active)
                        cmd.ExecuteNonQuery()
                        count += 1
                    End Using
                Next
            End Using
            Return count
        End Function

        Public Async Function SyncEmployees() As Task(Of Integer)
            Dim items = Await _api.GetAllPaged("resource/Employee")
            Dim count As Integer = 0
            Using conn = _db.GetConnection()
                For Each obj In items
                    Dim id = obj.Value(Of Long)("Id")
                    Dim displayName = obj.Value(Of String)("DisplayName")

                    Dim roleTitle As String = Nothing
                    Dim posObj = obj("Position")
                    If posObj IsNot Nothing AndAlso posObj.Type <> JTokenType.Null Then
                        roleTitle = posObj.Value(Of String)("Title")
                    End If

                    Dim yearOfBirth As Short? = ExtractYear(obj.Value(Of String)("DateOfBirth"))
                    Dim startYear As Short? = ExtractYear(obj.Value(Of String)("EmploymentStartDate"))
                    Dim active = obj.Value(Of Boolean)("Active")

                    Using cmd As New SqlCommand(
                        "MERGE deputy.Employees AS t " &
                        "USING (SELECT @id,@name,@role,@yob,@sy,@active) AS s(Id,DisplayName,RoleTitle,YearOfBirth,StartYear,IsActive) ON t.Id=s.Id " &
                        "WHEN MATCHED THEN UPDATE SET DisplayName=s.DisplayName,RoleTitle=s.RoleTitle," &
                        "YearOfBirth=s.YearOfBirth,StartYear=s.StartYear,IsActive=s.IsActive,SyncedAt=GETDATE() " &
                        "WHEN NOT MATCHED THEN INSERT (Id,DisplayName,RoleTitle,YearOfBirth,StartYear,IsActive) " &
                        "VALUES(s.Id,s.DisplayName,s.RoleTitle,s.YearOfBirth,s.StartYear,s.IsActive);", conn)
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.Parameters.AddWithValue("@name", NullOr(displayName))
                        cmd.Parameters.AddWithValue("@role", NullOr(roleTitle))
                        cmd.Parameters.AddWithValue("@yob", If(yearOfBirth.HasValue, CObj(yearOfBirth.Value), DBNull.Value))
                        cmd.Parameters.AddWithValue("@sy", If(startYear.HasValue, CObj(startYear.Value), DBNull.Value))
                        cmd.Parameters.AddWithValue("@active", active)
                        cmd.ExecuteNonQuery()
                        count += 1
                    End Using
                Next
            End Using
            Return count
        End Function

        ' Standard rolling-window sync using the Deputy.SyncDaysBack config value.
        Public Async Function SyncTimesheets() As Task(Of Integer)
            Dim daysBackStr = _db.GetConfigValue("Deputy.SyncDaysBack")
            Dim daysBack As Integer = If(Integer.TryParse(daysBackStr, daysBack), daysBack, 90)
            Return Await SyncTimesheetRange(DateTime.Today.AddDays(-daysBack), DateTime.Today)
        End Function

        ' Sync a specific date range in one API call — used by both standard sync and backfill chunks.
        ' Server-side date filter is attempted via Deputy QUERY search body; MERGE ensures idempotency.
        Public Async Function SyncTimesheetRange(fromDate As Date, toDate As Date) As Task(Of Integer)
            ' Try server-side date filter first (Deputy API v1 QUERY format).
            ' If the Deputy instance does not support this filter the API returns all records
            ' and client-side filtering below keeps only the relevant range.
            Dim body = $"{{""search"":{{""f"":""Date"",""op"":""between"",""val"":""{fromDate:yyyy-MM-dd}"",""val2"":""{toDate:yyyy-MM-dd}""}}}}"

            Dim items = Await _api.PostQuery("resource/Timesheet/QUERY", body)
            Dim count As Integer = 0
            Using conn = _db.GetConnection()
                For Each obj In items
                    Dim tsDate As Date? = ParseDate(obj.Value(Of String)("Date"))
                    ' Client-side guard: skip if outside range (handles API versions ignoring the filter)
                    If tsDate.HasValue AndAlso (tsDate.Value < fromDate OrElse tsDate.Value > toDate) Then Continue For

                    Dim id = obj.Value(Of Long)("Id")
                    Dim startTime As DateTime? = ParseDateTime(obj.Value(Of String)("StartTime"))
                    Dim endTime As DateTime? = ParseDateTime(obj.Value(Of String)("EndTime"))
                    Dim totalMins = obj.Value(Of Decimal)("TotalTime") * 60D
                    Dim cost = obj.Value(Of Decimal)("Cost")
                    Dim approved = obj.Value(Of Boolean)("TimeApproved")
                    Dim empId = obj.Value(Of Long)("Employee")

                    Dim opUnitId As Long? = Nothing
                    Dim opInfo = obj("OperationalUnitInfo")
                    If opInfo IsNot Nothing AndAlso opInfo.Type <> JTokenType.Null Then
                        opUnitId = opInfo.Value(Of Long)("Id")
                    End If

                    Dim workTypeId As Long? = Nothing
                    Dim wtToken = obj("WorkType")
                    If wtToken IsNot Nothing AndAlso wtToken.Type <> JTokenType.Null Then
                        If wtToken.Type = JTokenType.Integer Then
                            workTypeId = wtToken.Value(Of Long)()
                        ElseIf wtToken("Id") IsNot Nothing Then
                            workTypeId = wtToken.Value(Of Long)("Id")
                        End If
                    End If

                    Using cmd As New SqlCommand(
                        "MERGE deputy.Timesheets AS t " &
                        "USING (SELECT @id,@date,@start,@end,@mins,@cost,@approved,@emp,@ou,@wt) AS " &
                        "s(Id,TimesheetDate,StartTime,EndTime,TotalMinutes,Cost,IsApproved,EmployeeId,OperationalUnitId,WorkTypeId) " &
                        "ON t.Id=s.Id " &
                        "WHEN MATCHED THEN UPDATE SET TimesheetDate=s.TimesheetDate,StartTime=s.StartTime," &
                        "EndTime=s.EndTime,TotalMinutes=s.TotalMinutes,Cost=s.Cost,IsApproved=s.IsApproved," &
                        "EmployeeId=s.EmployeeId,OperationalUnitId=s.OperationalUnitId,WorkTypeId=s.WorkTypeId,SyncedAt=GETDATE() " &
                        "WHEN NOT MATCHED THEN INSERT (Id,TimesheetDate,StartTime,EndTime,TotalMinutes,Cost,IsApproved,EmployeeId,OperationalUnitId,WorkTypeId) " &
                        "VALUES(s.Id,s.TimesheetDate,s.StartTime,s.EndTime,s.TotalMinutes,s.Cost,s.IsApproved,s.EmployeeId,s.OperationalUnitId,s.WorkTypeId);", conn)
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.Parameters.AddWithValue("@date", If(tsDate.HasValue, CObj(tsDate.Value), DBNull.Value))
                        cmd.Parameters.AddWithValue("@start", If(startTime.HasValue, CObj(startTime.Value), DBNull.Value))
                        cmd.Parameters.AddWithValue("@end", If(endTime.HasValue, CObj(endTime.Value), DBNull.Value))
                        cmd.Parameters.AddWithValue("@mins", totalMins)
                        cmd.Parameters.AddWithValue("@cost", cost)
                        cmd.Parameters.AddWithValue("@approved", approved)
                        cmd.Parameters.AddWithValue("@emp", If(empId <> 0, CObj(empId), DBNull.Value))
                        cmd.Parameters.AddWithValue("@ou", If(opUnitId.HasValue, CObj(opUnitId.Value), DBNull.Value))
                        cmd.Parameters.AddWithValue("@wt", If(workTypeId.HasValue, CObj(workTypeId.Value), DBNull.Value))
                        cmd.ExecuteNonQuery()
                        count += 1
                    End Using
                Next
            End Using
            Return count
        End Function

        ' ── Helpers ──────────────────────────────────────────────────────────

        Private Shared Function NullOr(s As String) As Object
            Return If(String.IsNullOrEmpty(s), CObj(DBNull.Value), s)
        End Function

        Private Shared Function ExtractYear(dateStr As String) As Short?
            If String.IsNullOrEmpty(dateStr) Then Return Nothing
            Dim d As DateTime
            If DateTime.TryParse(dateStr, d) Then Return CShort(d.Year)
            Return Nothing
        End Function

        Private Shared Function ParseDate(s As String) As Date?
            If String.IsNullOrEmpty(s) Then Return Nothing
            Dim d As DateTime
            If DateTime.TryParse(s, d) Then Return d.Date
            Return Nothing
        End Function

        Private Shared Function ParseDateTime(s As String) As DateTime?
            If String.IsNullOrEmpty(s) Then Return Nothing
            Dim d As DateTime
            If DateTime.TryParse(s, d) Then Return d
            Return Nothing
        End Function

    End Class

End Namespace
