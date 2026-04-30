Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
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

        Public Async Function SyncOperationalUnits(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim items = Await _api.GetAll("resource/OperationalUnit", ct)
            Dim count As Integer = 0
            Using conn = _db.GetConnection()
                Using txn = conn.BeginTransaction()
                    For Each obj As JObject In items
                        UpsertOperationalUnit(obj, conn, txn)
                        count += 1
                    Next
                    txn.Commit()
                End Using
            End Using
            Return count
        End Function

        Public Async Function SyncDepartments(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim items = Await _api.GetAll("resource/Department", ct)
            Dim count As Integer = 0
            Using conn = _db.GetConnection()
                Using txn = conn.BeginTransaction()
                    For Each obj As JObject In items
                        UpsertDepartment(obj, conn, txn)
                        count += 1
                    Next
                    txn.Commit()
                End Using
            End Using
            Return count
        End Function

        Public Async Function SyncEmployees(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim searchJson = "{""search"":{}}"
            Dim total As Integer = 0
            Await _api.ForEachPage("resource/Employee/QUERY", searchJson, "Syncing Employees",
                Sub(page)
                    Using conn = _db.GetConnection()
                        Using txn = conn.BeginTransaction()
                            For Each obj As JObject In page
                                UpsertEmployee(obj, conn, txn)
                                total += 1
                            Next
                            txn.Commit()
                        End Using
                    End Using
                End Sub, ct)
            Return total
        End Function

        Public Async Function SyncTimesheets(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim daysBackStr = _db.GetConfigValue("Deputy.SyncDaysBack")
            Dim daysBack As Integer = 90
            Integer.TryParse(daysBackStr, daysBack)
            Return Await SyncTimesheetRange(DateTime.Today.AddDays(-daysBack), DateTime.Today, ct)
        End Function

        Public Async Function SyncTimesheetRange(fromDate As Date, toDate As Date, Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim searchJson = $"{{""search"":{{""mdFrom"":{{""field"":""Date"",""type"":""ge"",""data"":""{fromDate:yyyy-MM-dd}""}},""mdTo"":{{""field"":""Date"",""type"":""le"",""data"":""{toDate:yyyy-MM-dd}""}}}}}}"
            Dim label = $"Syncing Timesheets {fromDate:dd/MM/yy}–{toDate:dd/MM/yy}"
            Dim total As Integer = 0

            Await _api.ForEachPage("resource/Timesheet/QUERY", searchJson, label,
                Sub(page)
                    Dim batch = page.Where(Function(obj)
                        Dim d = ParseJDate(obj, "Date")
                        Return Not d.HasValue OrElse (d.Value >= fromDate AndAlso d.Value <= toDate)
                    End Function).ToList()
                    If batch.Count = 0 Then Return
                    Using conn = _db.GetConnection()
                        Using txn = conn.BeginTransaction()
                            BulkUpsertTimesheets(batch, conn, txn)
                            txn.Commit()
                        End Using
                    End Using
                    total += batch.Count
                End Sub, ct)

            Return total
        End Function

        Public Async Function SyncCompany(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim items = Await _api.GetAll("resource/Company", ct)
            Dim count As Integer = 0
            Using conn = _db.GetConnection()
                Using txn = conn.BeginTransaction()
                    For Each obj As JObject In items
                        UpsertCompany(obj, conn, txn)
                        count += 1
                    Next
                    txn.Commit()
                End Using
            End Using
            Return count
        End Function

        Public Async Function SyncRosters(Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim daysBackStr = _db.GetConfigValue("Deputy.SyncDaysBack")
            Dim daysBack As Integer = 90
            Integer.TryParse(daysBackStr, daysBack)
            Return Await SyncRosterRange(DateTime.Today.AddDays(-daysBack), DateTime.Today, ct)
        End Function

        Public Async Function SyncRosterRange(fromDate As Date, toDate As Date, Optional ct As CancellationToken = Nothing) As Task(Of Integer)
            Dim searchJson = $"{{""search"":{{""mdFrom"":{{""field"":""Date"",""type"":""ge"",""data"":""{fromDate:yyyy-MM-dd}""}},""mdTo"":{{""field"":""Date"",""type"":""le"",""data"":""{toDate:yyyy-MM-dd}""}}}}}}"
            Dim label = $"Syncing Rosters {fromDate:dd/MM/yy}–{toDate:dd/MM/yy}"
            Dim total As Integer = 0
            Await _api.ForEachPage("resource/Roster/QUERY", searchJson, label,
                Sub(page)
                    Dim batch = page.Where(Function(obj)
                        Dim d = ParseJDate(obj, "Date")
                        Return Not d.HasValue OrElse (d.Value >= fromDate AndAlso d.Value <= toDate)
                    End Function).ToList()
                    If batch.Count = 0 Then Return
                    Using conn = _db.GetConnection()
                        Using txn = conn.BeginTransaction()
                            BulkUpsertRosters(batch, conn, txn)
                            txn.Commit()
                        End Using
                    End Using
                    total += batch.Count
                End Sub, ct)
            Return total
        End Function

        ' ── Bulk upsert helpers (temp table + single MERGE per page) ─────────

        Private Shared Sub UpsertOperationalUnit(obj As JObject, conn As SqlConnection, txn As SqlTransaction)
            Dim id = SafeLong(obj, "Id")
            Dim name = obj.Value(Of String)("OperationalUnitName")
            Dim active As Boolean = SafeBool(obj, "Active")
            Dim companyId As Long? = Nothing
            Dim cToken = obj("Company")
            If cToken IsNot Nothing AndAlso cToken.Type = JTokenType.Integer Then companyId = cToken.Value(Of Long)()
            Dim payrollExportName = obj.Value(Of String)("PayrollExportName")
            Dim colour = obj.Value(Of String)("Colour")
            Dim companyCode = obj.Value(Of String)("CompanyCode")
            Dim companyName = obj.Value(Of String)("CompanyName")
            Dim rosterSortOrder As Integer? = Nothing
            Dim rsoToken = obj("RosterSortOrder")
            If rsoToken IsNot Nothing AndAlso rsoToken.Type = JTokenType.Integer Then rosterSortOrder = rsoToken.Value(Of Integer)()
            Dim showOnRoster = SafeBool(obj, "ShowOnRoster")
            Dim parentOU As Long? = Nothing
            Dim pToken = obj("ParentOperationalUnit")
            If pToken IsNot Nothing AndAlso pToken.Type = JTokenType.Integer Then parentOU = pToken.Value(Of Long)()
            Using cmd As New SqlCommand(
                "MERGE deputy.OperationalUnits AS t " &
                "USING (SELECT @id,@name,@active,@co,@pex,@col,@cc,@cn,@rso,@sor,@pou) AS s(Id,UnitName,IsActive,CompanyId,PayrollExportName,Colour,CompanyCode,CompanyName,RosterSortOrder,ShowOnRoster,ParentOperationalUnit) ON t.Id=s.Id " &
                "WHEN MATCHED THEN UPDATE SET UnitName=s.UnitName,IsActive=s.IsActive,CompanyId=s.CompanyId,PayrollExportName=s.PayrollExportName,Colour=s.Colour,CompanyCode=s.CompanyCode,CompanyName=s.CompanyName,RosterSortOrder=s.RosterSortOrder,ShowOnRoster=s.ShowOnRoster,ParentOperationalUnit=s.ParentOperationalUnit,SyncedAt=GETDATE() " &
                "WHEN NOT MATCHED THEN INSERT (Id,UnitName,IsActive,CompanyId,PayrollExportName,Colour,CompanyCode,CompanyName,RosterSortOrder,ShowOnRoster,ParentOperationalUnit) VALUES(s.Id,s.UnitName,s.IsActive,s.CompanyId,s.PayrollExportName,s.Colour,s.CompanyCode,s.CompanyName,s.RosterSortOrder,s.ShowOnRoster,s.ParentOperationalUnit);", conn, txn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@name", NullOr(name))
                cmd.Parameters.AddWithValue("@active", active)
                cmd.Parameters.AddWithValue("@co", If(companyId.HasValue, CObj(companyId.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@pex", NullOr(payrollExportName))
                cmd.Parameters.AddWithValue("@col", NullOr(colour))
                cmd.Parameters.AddWithValue("@cc", NullOr(companyCode))
                cmd.Parameters.AddWithValue("@cn", NullOr(companyName))
                cmd.Parameters.AddWithValue("@rso", If(rosterSortOrder.HasValue, CObj(rosterSortOrder.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@sor", showOnRoster)
                cmd.Parameters.AddWithValue("@pou", If(parentOU.HasValue, CObj(parentOU.Value), DBNull.Value))
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub UpsertDepartment(obj As JObject, conn As SqlConnection, txn As SqlTransaction)
            Dim id = SafeLong(obj, "Id")
            Dim name = If(obj.Value(Of String)("DepartmentName"), obj.Value(Of String)("CompanyName"))
            Dim active As Boolean = SafeBool(obj, "Active")
            Using cmd As New SqlCommand(
                "MERGE deputy.Departments AS t " &
                "USING (SELECT @id,@name,@active) AS s(Id,DepartmentName,IsActive) ON t.Id=s.Id " &
                "WHEN MATCHED THEN UPDATE SET DepartmentName=s.DepartmentName,IsActive=s.IsActive,SyncedAt=GETDATE() " &
                "WHEN NOT MATCHED THEN INSERT (Id,DepartmentName,IsActive) VALUES(s.Id,s.DepartmentName,s.IsActive);", conn, txn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@name", NullOr(name))
                cmd.Parameters.AddWithValue("@active", active)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub UpsertCompany(obj As JObject, conn As SqlConnection, txn As SqlTransaction)
            Dim id = SafeLong(obj, "Id")
            Dim code = obj.Value(Of String)("Code")
            Dim name = obj.Value(Of String)("CompanyName")
            Dim tradingName = obj.Value(Of String)("TradingName")
            Dim isWorkplace = SafeBool(obj, "IsWorkplace")
            Dim isPayrollEntity = SafeBool(obj, "IsPayrollEntity")
            Dim active As Boolean = SafeBool(obj, "Active")
            Using cmd As New SqlCommand(
                "MERGE deputy.Company AS t " &
                "USING (SELECT @id,@code,@name,@trading,@wp,@pe,@active) AS s(Id,CompanyCode,CompanyName,TradingName,IsWorkplace,IsPayrollEntity,IsActive) ON t.Id=s.Id " &
                "WHEN MATCHED THEN UPDATE SET CompanyCode=s.CompanyCode,CompanyName=s.CompanyName,TradingName=s.TradingName,IsWorkplace=s.IsWorkplace,IsPayrollEntity=s.IsPayrollEntity,IsActive=s.IsActive,SyncedAt=GETDATE() " &
                "WHEN NOT MATCHED THEN INSERT (Id,CompanyCode,CompanyName,TradingName,IsWorkplace,IsPayrollEntity,IsActive) VALUES(s.Id,s.CompanyCode,s.CompanyName,s.TradingName,s.IsWorkplace,s.IsPayrollEntity,s.IsActive);", conn, txn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@code", NullOr(code))
                cmd.Parameters.AddWithValue("@name", NullOr(name))
                cmd.Parameters.AddWithValue("@trading", NullOr(tradingName))
                cmd.Parameters.AddWithValue("@wp", isWorkplace)
                cmd.Parameters.AddWithValue("@pe", isPayrollEntity)
                cmd.Parameters.AddWithValue("@active", active)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub BulkUpsertRosters(rows As List(Of JObject), conn As SqlConnection, txn As SqlTransaction)
            Dim dt As New DataTable()
            dt.Columns.Add("Id", GetType(Long))
            dt.Columns.Add("EmployeeId", GetType(Long))
            dt.Columns.Add("OperationalUnitId", GetType(Long))
            dt.Columns.Add("TimesheetId", GetType(Long))
            dt.Columns.Add("RosterDate", GetType(DateTime))
            dt.Columns.Add("StartTime", GetType(DateTime))
            dt.Columns.Add("EndTime", GetType(DateTime))
            dt.Columns.Add("MealbreakMinutes", GetType(Decimal))
            dt.Columns.Add("TotalHours", GetType(Decimal))
            dt.Columns.Add("Cost", GetType(Decimal))
            dt.Columns.Add("OnCost", GetType(Decimal))
            dt.Columns.Add("Published", GetType(Boolean))
            dt.Columns.Add("IsOpen", GetType(Boolean))
            dt.Columns.Add("Modified", GetType(DateTime))

            For Each obj As JObject In rows
                Dim empId = SafeLong(obj, "Employee")
                Dim ouId As Long? = Nothing
                Dim ouToken = obj("OperationalUnit")
                If ouToken IsNot Nothing AndAlso ouToken.Type = JTokenType.Integer Then ouId = ouToken.Value(Of Long)()
                Dim tsId As Long? = Nothing
                Dim tsToken = obj("MatchedByTimesheet")
                If tsToken IsNot Nothing AndAlso tsToken.Type = JTokenType.Integer Then tsId = tsToken.Value(Of Long)()
                Dim rDate = ParseJDate(obj, "Date")
                Dim startTime = ParseJDateTime(obj, "StartTimeLocalized")
                Dim endTime = ParseJDateTime(obj, "EndTimeLocalized")
                Dim mealMins = ParseMealbreakMins(obj, "Mealbreak")
                Dim modified = ParseJDateTime(obj, "Modified")
                Dim row = dt.NewRow()
                row("Id") = SafeLong(obj, "Id")
                row("EmployeeId") = If(empId <> 0, CObj(empId), DBNull.Value)
                row("OperationalUnitId") = If(ouId.HasValue, CObj(ouId.Value), DBNull.Value)
                row("TimesheetId") = If(tsId.HasValue, CObj(tsId.Value), DBNull.Value)
                row("RosterDate") = If(rDate.HasValue, CObj(rDate.Value), DBNull.Value)
                row("StartTime") = If(startTime.HasValue, CObj(startTime.Value), DBNull.Value)
                row("EndTime") = If(endTime.HasValue, CObj(endTime.Value), DBNull.Value)
                row("MealbreakMinutes") = If(mealMins.HasValue, CObj(mealMins.Value), DBNull.Value)
                row("TotalHours") = SafeDecimal(obj, "TotalTime")
                row("Cost") = SafeDecimal(obj, "Cost")
                row("OnCost") = SafeDecimal(obj, "OnCost")
                row("Published") = SafeBool(obj, "Published")
                row("IsOpen") = SafeBool(obj, "Open")
                row("Modified") = If(modified.HasValue, CObj(modified.Value), DBNull.Value)
                dt.Rows.Add(row)
            Next

            Using cmd As New SqlCommand(
                "CREATE TABLE #rs (Id BIGINT NOT NULL, EmployeeId BIGINT NULL, OperationalUnitId BIGINT NULL, TimesheetId BIGINT NULL, " &
                "RosterDate DATE NULL, StartTime DATETIME2 NULL, EndTime DATETIME2 NULL, MealbreakMinutes DECIMAL(5,2) NULL, " &
                "TotalHours DECIMAL(6,2) NULL, Cost DECIMAL(18,4) NULL, OnCost DECIMAL(18,4) NULL, " &
                "Published BIT NOT NULL, IsOpen BIT NOT NULL, Modified DATETIME2 NULL)", conn, txn)
                cmd.ExecuteNonQuery()
            End Using

            Using bcp As New SqlBulkCopy(conn, SqlBulkCopyOptions.Default, txn)
                bcp.DestinationTableName = "#rs"
                bcp.WriteToServer(dt)
            End Using

            Using cmd As New SqlCommand(
                "MERGE deputy.Rosters AS t USING #rs AS s ON t.Id = s.Id " &
                "WHEN MATCHED THEN UPDATE SET EmployeeId=s.EmployeeId,OperationalUnitId=s.OperationalUnitId,TimesheetId=s.TimesheetId,RosterDate=s.RosterDate,StartTime=s.StartTime,EndTime=s.EndTime,MealbreakMinutes=s.MealbreakMinutes,TotalHours=s.TotalHours,Cost=s.Cost,OnCost=s.OnCost,Published=s.Published,IsOpen=s.IsOpen,Modified=s.Modified,SyncedAt=GETDATE() " &
                "WHEN NOT MATCHED THEN INSERT (Id,EmployeeId,OperationalUnitId,TimesheetId,RosterDate,StartTime,EndTime,MealbreakMinutes,TotalHours,Cost,OnCost,Published,IsOpen,Modified) " &
                "VALUES(s.Id,s.EmployeeId,s.OperationalUnitId,s.TimesheetId,s.RosterDate,s.StartTime,s.EndTime,s.MealbreakMinutes,s.TotalHours,s.Cost,s.OnCost,s.Published,s.IsOpen,s.Modified);", conn, txn)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub UpsertEmployee(obj As JObject, conn As SqlConnection, txn As SqlTransaction)
            Dim id = SafeLong(obj, "Id")
            Dim firstName = obj.Value(Of String)("FirstName")
            Dim lastName = obj.Value(Of String)("LastName")
            Dim displayName = obj.Value(Of String)("DisplayName")
            Dim roleTitle As String = Nothing
            Dim posObj = obj("Position")
            If posObj IsNot Nothing AndAlso posObj.Type = JTokenType.Object Then
                roleTitle = posObj.Value(Of String)("Title")
            End If
            Dim companyId As Long? = Nothing
            Dim cToken = obj("Company")
            If cToken IsNot Nothing AndAlso cToken.Type = JTokenType.Integer Then companyId = cToken.Value(Of Long)()
            Dim yearOfBirth As Short? = ExtractYear(obj.Value(Of String)("DateOfBirth"))
            Dim startDate As Date? = ParseJDate(obj, "StartDate")
            Dim active As Boolean = SafeBool(obj, "Active")
            Using cmd As New SqlCommand(
                "MERGE deputy.Employees AS t " &
                "USING (SELECT @id,@fn,@ln,@name,@role,@co,@yob,@sd,@active) AS s(Id,FirstName,LastName,DisplayName,RoleTitle,CompanyId,YearOfBirth,StartDate,IsActive) ON t.Id=s.Id " &
                "WHEN MATCHED THEN UPDATE SET FirstName=s.FirstName,LastName=s.LastName,DisplayName=s.DisplayName,RoleTitle=s.RoleTitle,CompanyId=s.CompanyId,YearOfBirth=s.YearOfBirth,StartDate=s.StartDate,IsActive=s.IsActive,SyncedAt=GETDATE() " &
                "WHEN NOT MATCHED THEN INSERT (Id,FirstName,LastName,DisplayName,RoleTitle,CompanyId,YearOfBirth,StartDate,IsActive) " &
                "VALUES(s.Id,s.FirstName,s.LastName,s.DisplayName,s.RoleTitle,s.CompanyId,s.YearOfBirth,s.StartDate,s.IsActive);", conn, txn)
                cmd.Parameters.AddWithValue("@id", id)
                cmd.Parameters.AddWithValue("@fn", NullOr(firstName))
                cmd.Parameters.AddWithValue("@ln", NullOr(lastName))
                cmd.Parameters.AddWithValue("@name", NullOr(displayName))
                cmd.Parameters.AddWithValue("@role", NullOr(roleTitle))
                cmd.Parameters.AddWithValue("@co", If(companyId.HasValue, CObj(companyId.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@yob", If(yearOfBirth.HasValue, CObj(yearOfBirth.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@sd", If(startDate.HasValue, CObj(startDate.Value), DBNull.Value))
                cmd.Parameters.AddWithValue("@active", active)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Private Shared Sub BulkUpsertTimesheets(rows As List(Of JObject), conn As SqlConnection, txn As SqlTransaction)
            Dim dt As New DataTable()
            dt.Columns.Add("Id", GetType(Long))
            dt.Columns.Add("EmployeeId", GetType(Long))
            dt.Columns.Add("OperationalUnitId", GetType(Long))
            dt.Columns.Add("RosterId", GetType(Long))
            dt.Columns.Add("TimesheetDate", GetType(DateTime))
            dt.Columns.Add("StartTime", GetType(DateTime))
            dt.Columns.Add("EndTime", GetType(DateTime))
            dt.Columns.Add("MealbreakMinutes", GetType(Decimal))
            dt.Columns.Add("TotalHours", GetType(Decimal))
            dt.Columns.Add("TotalHoursInv", GetType(Decimal))
            dt.Columns.Add("Cost", GetType(Decimal))
            dt.Columns.Add("OnCost", GetType(Decimal))
            dt.Columns.Add("IsApproved", GetType(Boolean))
            dt.Columns.Add("IsPayRuleApproved", GetType(Boolean))
            dt.Columns.Add("IsLeave", GetType(Boolean))
            dt.Columns.Add("IsInProgress", GetType(Boolean))
            dt.Columns.Add("Discarded", GetType(Boolean))
            dt.Columns.Add("ReviewState", GetType(Integer))
            dt.Columns.Add("Modified", GetType(DateTime))

            For Each obj As JObject In rows
                Dim empId = SafeLong(obj, "Employee")
                Dim ouId As Long? = Nothing
                Dim ouToken = obj("OperationalUnit")
                If ouToken IsNot Nothing AndAlso ouToken.Type = JTokenType.Integer Then ouId = ouToken.Value(Of Long)()
                Dim rosterId As Long? = Nothing
                Dim rToken = obj("Roster")
                If rToken IsNot Nothing AndAlso rToken.Type = JTokenType.Integer Then rosterId = rToken.Value(Of Long)()
                Dim reviewState As Integer? = Nothing
                Dim rvToken = obj("ReviewState")
                If rvToken IsNot Nothing AndAlso rvToken.Type = JTokenType.Integer Then reviewState = rvToken.Value(Of Integer)()
                Dim tsDate = ParseJDate(obj, "Date")
                Dim startTime = ParseJDateTime(obj, "StartTimeLocalized")
                Dim endTime = ParseJDateTime(obj, "EndTimeLocalized")
                Dim mealMins = ParseMealbreakMins(obj, "Mealbreak")
                Dim modified = ParseJDateTime(obj, "Modified")
                Dim row = dt.NewRow()
                row("Id") = SafeLong(obj, "Id")
                row("EmployeeId") = If(empId <> 0, CObj(empId), DBNull.Value)
                row("OperationalUnitId") = If(ouId.HasValue, CObj(ouId.Value), DBNull.Value)
                row("RosterId") = If(rosterId.HasValue, CObj(rosterId.Value), DBNull.Value)
                row("TimesheetDate") = If(tsDate.HasValue, CObj(tsDate.Value), DBNull.Value)
                row("StartTime") = If(startTime.HasValue, CObj(startTime.Value), DBNull.Value)
                row("EndTime") = If(endTime.HasValue, CObj(endTime.Value), DBNull.Value)
                row("MealbreakMinutes") = If(mealMins.HasValue, CObj(mealMins.Value), DBNull.Value)
                row("TotalHours") = SafeDecimal(obj, "TotalTime")
                row("TotalHoursInv") = SafeDecimal(obj, "TotalTimeInv")
                row("Cost") = SafeDecimal(obj, "Cost")
                row("OnCost") = SafeDecimal(obj, "OnCost")
                row("IsApproved") = SafeBool(obj, "TimeApproved")
                row("IsPayRuleApproved") = SafeBool(obj, "PayRuleApproved")
                row("IsLeave") = SafeBool(obj, "IsLeave")
                row("IsInProgress") = SafeBool(obj, "IsInProgress")
                row("Discarded") = SafeBool(obj, "Discarded")
                row("ReviewState") = If(reviewState.HasValue, CObj(reviewState.Value), DBNull.Value)
                row("Modified") = If(modified.HasValue, CObj(modified.Value), DBNull.Value)
                dt.Rows.Add(row)
            Next

            Using cmd As New SqlCommand(
                "CREATE TABLE #ts (Id BIGINT NOT NULL, EmployeeId BIGINT NULL, OperationalUnitId BIGINT NULL, RosterId BIGINT NULL, " &
                "TimesheetDate DATE NULL, StartTime DATETIME2 NULL, EndTime DATETIME2 NULL, MealbreakMinutes DECIMAL(5,2) NULL, " &
                "TotalHours DECIMAL(6,2) NULL, TotalHoursInv DECIMAL(6,2) NULL, Cost DECIMAL(18,4) NULL, OnCost DECIMAL(18,4) NULL, " &
                "IsApproved BIT NOT NULL, IsPayRuleApproved BIT NOT NULL, IsLeave BIT NOT NULL, IsInProgress BIT NOT NULL, " &
                "Discarded BIT NOT NULL, ReviewState INT NULL, Modified DATETIME2 NULL)", conn, txn)
                cmd.ExecuteNonQuery()
            End Using

            Using bcp As New SqlBulkCopy(conn, SqlBulkCopyOptions.Default, txn)
                bcp.DestinationTableName = "#ts"
                bcp.WriteToServer(dt)
            End Using

            Using cmd As New SqlCommand(
                "MERGE deputy.Timesheets AS t USING #ts AS s ON t.Id = s.Id " &
                "WHEN MATCHED THEN UPDATE SET EmployeeId=s.EmployeeId,OperationalUnitId=s.OperationalUnitId,RosterId=s.RosterId,TimesheetDate=s.TimesheetDate,StartTime=s.StartTime,EndTime=s.EndTime,MealbreakMinutes=s.MealbreakMinutes,TotalHours=s.TotalHours,TotalHoursInv=s.TotalHoursInv,Cost=s.Cost,OnCost=s.OnCost,IsApproved=s.IsApproved,IsPayRuleApproved=s.IsPayRuleApproved,IsLeave=s.IsLeave,IsInProgress=s.IsInProgress,Discarded=s.Discarded,ReviewState=s.ReviewState,Modified=s.Modified,SyncedAt=GETDATE() " &
                "WHEN NOT MATCHED THEN INSERT (Id,EmployeeId,OperationalUnitId,RosterId,TimesheetDate,StartTime,EndTime,MealbreakMinutes,TotalHours,TotalHoursInv,Cost,OnCost,IsApproved,IsPayRuleApproved,IsLeave,IsInProgress,Discarded,ReviewState,Modified) " &
                "VALUES(s.Id,s.EmployeeId,s.OperationalUnitId,s.RosterId,s.TimesheetDate,s.StartTime,s.EndTime,s.MealbreakMinutes,s.TotalHours,s.TotalHoursInv,s.Cost,s.OnCost,s.IsApproved,s.IsPayRuleApproved,s.IsLeave,s.IsInProgress,s.Discarded,s.ReviewState,s.Modified);", conn, txn)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        ' ── Shared helpers ────────────────────────────────────────────────────

        Private Shared Function NullOr(s As String) As Object
            Return If(String.IsNullOrEmpty(s), CObj(DBNull.Value), s)
        End Function

        Private Shared Function SafeBool(obj As JObject, key As String) As Boolean
            Dim t = obj(key)
            If t Is Nothing OrElse t.Type = JTokenType.Null Then Return False
            Return t.Value(Of Boolean)()
        End Function

        Private Shared Function SafeLong(obj As JObject, key As String) As Long
            Dim t = obj(key)
            If t Is Nothing OrElse t.Type = JTokenType.Null Then Return 0
            Return t.Value(Of Long)()
        End Function

        Private Shared Function SafeDecimal(obj As JObject, key As String) As Decimal
            Dim t = obj(key)
            If t Is Nothing OrElse t.Type = JTokenType.Null Then Return 0D
            Return t.Value(Of Decimal)()
        End Function

        ' Handles fields Newtonsoft auto-parses as JTokenType.Date and plain ISO strings.
        Private Shared Function ParseJDateTime(obj As JObject, key As String) As DateTime?
            Dim t = obj(key)
            If t Is Nothing OrElse t.Type = JTokenType.Null Then Return Nothing
            If t.Type = JTokenType.Date Then
                Try : Return CDate(CType(t, JValue).Value) : Catch : End Try
            End If
            If t.Type = JTokenType.String Then
                Dim d As DateTime
                If DateTime.TryParse(t.Value(Of String)(), d) Then Return d
            End If
            Return Nothing
        End Function

        Private Shared Function ParseJDate(obj As JObject, key As String) As Date?
            Dim dt = ParseJDateTime(obj, key)
            Return If(dt.HasValue, CType(dt.Value.Date, Date?), Nothing)
        End Function

        ' Mealbreak is stored as a datetime where the time portion = break duration.
        Private Shared Function ParseMealbreakMins(obj As JObject, key As String) As Decimal?
            Dim dt = ParseJDateTime(obj, key)
            If Not dt.HasValue Then Return Nothing
            Dim mins = CDec(dt.Value.TimeOfDay.TotalMinutes)
            Return If(mins = 0D, CType(Nothing, Decimal?), mins)
        End Function

        Private Shared Function ExtractYear(dateStr As String) As Short?
            If String.IsNullOrEmpty(dateStr) Then Return Nothing
            Dim d As DateTime
            If DateTime.TryParse(dateStr, d) Then Return CShort(d.Year)
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
