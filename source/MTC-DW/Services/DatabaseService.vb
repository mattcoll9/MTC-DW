Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports MTCDW.Models

Namespace Services

    Public Class DatabaseService

        Private _connStr As String

        Public Sub New(connStr As String)
            _connStr = connStr
        End Sub

        Public Sub UpdateConnectionString(newConnStr As String)
            _connStr = newConnStr
        End Sub

        Public Function GetConnection() As SqlConnection
            Dim conn As New SqlConnection(_connStr)
            conn.Open()
            Return conn
        End Function

        Public Function TestConnection() As Boolean
            Try
                Using conn = GetConnection()
                    Return True
                End Using
            Catch
                Return False
            End Try
        End Function

        ' ── Schema bootstrap ─────────────────────────────────────────────────

        Public Sub EnsureSchema()
            Dim statements As String() = {
                "IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'deputy') EXEC('CREATE SCHEMA deputy')",

                "IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Config') AND type = 'U')
                 CREATE TABLE dbo.Config (
                     ConfigKey   VARCHAR(100)  NOT NULL PRIMARY KEY,
                     ConfigValue NVARCHAR(MAX) NULL,
                     UpdatedAt   DATETIME2     NOT NULL DEFAULT GETDATE()
                 )",

                "IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Jobs') AND type = 'U')
                 CREATE TABLE dbo.Jobs (
                     Id              INT          NOT NULL IDENTITY(1,1) PRIMARY KEY,
                     JobName         VARCHAR(100) NOT NULL,
                     SourceType      VARCHAR(50)  NOT NULL,
                     EntityType      VARCHAR(50)  NOT NULL,
                     ScheduleType    VARCHAR(20)  NOT NULL,
                     IntervalMinutes INT          NULL,
                     NextRunTime     DATETIME2    NULL,
                     LastRunTime     DATETIME2    NULL,
                     IsEnabled       BIT          NOT NULL DEFAULT 1,
                     SyncFromDate    DATE         NULL,
                     SyncToDate      DATE         NULL,
                     ChunkDays       INT          NULL,
                     SyncCursor      DATE         NULL
                 )",

                "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.Jobs') AND name='SyncFromDate')
                 ALTER TABLE dbo.Jobs ADD SyncFromDate DATE NULL",

                "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.Jobs') AND name='SyncToDate')
                 ALTER TABLE dbo.Jobs ADD SyncToDate DATE NULL",

                "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.Jobs') AND name='ChunkDays')
                 ALTER TABLE dbo.Jobs ADD ChunkDays INT NULL",

                "IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID(N'dbo.Jobs') AND name='SyncCursor')
                 ALTER TABLE dbo.Jobs ADD SyncCursor DATE NULL",

                "IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.JobHistory') AND type = 'U')
                 CREATE TABLE dbo.JobHistory (
                     Id              INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
                     JobId           INT           NOT NULL REFERENCES dbo.Jobs(Id),
                     StartedAt       DATETIME2     NOT NULL,
                     CompletedAt     DATETIME2     NULL,
                     Status          VARCHAR(20)   NOT NULL,
                     RecordsAffected INT           NULL,
                     ErrorMessage    NVARCHAR(MAX) NULL
                 )",

                "IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'deputy.OperationalUnits') AND type = 'U')
                 CREATE TABLE deputy.OperationalUnits (
                     Id       BIGINT        NOT NULL PRIMARY KEY,
                     Code     VARCHAR(50)   NULL,
                     UnitName NVARCHAR(200) NULL,
                     IsActive BIT           NOT NULL DEFAULT 1,
                     SyncedAt DATETIME2     NOT NULL DEFAULT GETDATE()
                 )",

                "IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'deputy.WorkTypes') AND type = 'U')
                 CREATE TABLE deputy.WorkTypes (
                     Id           BIGINT        NOT NULL PRIMARY KEY,
                     Code         VARCHAR(50)   NULL,
                     WorkTypeName NVARCHAR(200) NULL,
                     IsActive     BIT           NOT NULL DEFAULT 1,
                     SyncedAt     DATETIME2     NOT NULL DEFAULT GETDATE()
                 )",

                "IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'deputy.Employees') AND type = 'U')
                 CREATE TABLE deputy.Employees (
                     Id          BIGINT        NOT NULL PRIMARY KEY,
                     DisplayName NVARCHAR(200) NULL,
                     RoleTitle   NVARCHAR(200) NULL,
                     YearOfBirth SMALLINT      NULL,
                     StartYear   SMALLINT      NULL,
                     IsActive    BIT           NOT NULL DEFAULT 1,
                     SyncedAt    DATETIME2     NOT NULL DEFAULT GETDATE()
                 )",

                "IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'deputy.Timesheets') AND type = 'U')
                 CREATE TABLE deputy.Timesheets (
                     Id                BIGINT        NOT NULL PRIMARY KEY,
                     TimesheetDate     DATE          NULL,
                     StartTime         DATETIME2     NULL,
                     EndTime           DATETIME2     NULL,
                     TotalMinutes      DECIMAL(10,2) NULL,
                     Cost              DECIMAL(18,2) NULL,
                     IsApproved        BIT           NOT NULL DEFAULT 0,
                     EmployeeId        BIGINT        NULL,
                     OperationalUnitId BIGINT        NULL,
                     WorkTypeId        BIGINT        NULL,
                     SyncedAt          DATETIME2     NOT NULL DEFAULT GETDATE()
                 )"
            }

            Using conn = GetConnection()
                For Each sql In statements
                    Using cmd As New SqlCommand(sql, conn)
                        cmd.CommandTimeout = 30
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Using
        End Sub

        ' ── Config ───────────────────────────────────────────────────────────

        Public Function GetConfigValue(key As String) As String
            Using conn = GetConnection()
                Using cmd As New SqlCommand("SELECT ConfigValue FROM dbo.Config WHERE ConfigKey = @k", conn)
                    cmd.Parameters.AddWithValue("@k", key)
                    Dim result = cmd.ExecuteScalar()
                    Return If(result Is Nothing OrElse IsDBNull(result), Nothing, CStr(result))
                End Using
            End Using
        End Function

        Public Sub SetConfigValue(key As String, value As String)
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "MERGE dbo.Config AS t " &
                    "USING (SELECT @k AS ConfigKey, @v AS ConfigValue) AS s ON t.ConfigKey = s.ConfigKey " &
                    "WHEN MATCHED THEN UPDATE SET ConfigValue = s.ConfigValue, UpdatedAt = GETDATE() " &
                    "WHEN NOT MATCHED THEN INSERT (ConfigKey, ConfigValue) VALUES (s.ConfigKey, s.ConfigValue);", conn)
                    cmd.Parameters.AddWithValue("@k", key)
                    cmd.Parameters.AddWithValue("@v", If(value Is Nothing, CObj(DBNull.Value), value))
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function GetAllConfig() As List(Of AppConfig)
            Dim result As New List(Of AppConfig)
            Using conn = GetConnection()
                Using cmd As New SqlCommand("SELECT ConfigKey, ConfigValue, UpdatedAt FROM dbo.Config ORDER BY ConfigKey", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            result.Add(New AppConfig With {
                                .ConfigKey = rdr.GetString(0),
                                .ConfigValue = If(rdr.IsDBNull(1), Nothing, rdr.GetString(1)),
                                .UpdatedAt = rdr.GetDateTime(2)
                            })
                        End While
                    End Using
                End Using
            End Using
            Return result
        End Function

        ' ── Jobs ─────────────────────────────────────────────────────────────

        Public Function GetJobs() As List(Of JobDefinition)
            Dim result As New List(Of JobDefinition)
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "SELECT Id,JobName,SourceType,EntityType,ScheduleType,IntervalMinutes," &
                    "NextRunTime,LastRunTime,IsEnabled,SyncFromDate,SyncToDate,ChunkDays,SyncCursor " &
                    "FROM dbo.Jobs ORDER BY JobName", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            result.Add(ReadJob(rdr))
                        End While
                    End Using
                End Using
            End Using
            Return result
        End Function

        Public Function GetDueJobs() As List(Of JobDefinition)
            Dim result As New List(Of JobDefinition)
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "SELECT Id,JobName,SourceType,EntityType,ScheduleType,IntervalMinutes," &
                    "NextRunTime,LastRunTime,IsEnabled,SyncFromDate,SyncToDate,ChunkDays,SyncCursor " &
                    "FROM dbo.Jobs " &
                    "WHERE IsEnabled = 1 AND NextRunTime IS NOT NULL AND NextRunTime <= GETDATE()", conn)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            result.Add(ReadJob(rdr))
                        End While
                    End Using
                End Using
            End Using
            Return result
        End Function

        Private Function ReadJob(rdr As SqlDataReader) As JobDefinition
            Return New JobDefinition With {
                .Id = rdr.GetInt32(0),
                .JobName = rdr.GetString(1),
                .SourceType = rdr.GetString(2),
                .EntityType = rdr.GetString(3),
                .ScheduleType = rdr.GetString(4),
                .IntervalMinutes = If(rdr.IsDBNull(5), CType(Nothing, Integer?), rdr.GetInt32(5)),
                .NextRunTime = If(rdr.IsDBNull(6), CType(Nothing, DateTime?), rdr.GetDateTime(6)),
                .LastRunTime = If(rdr.IsDBNull(7), CType(Nothing, DateTime?), rdr.GetDateTime(7)),
                .IsEnabled = rdr.GetBoolean(8),
                .SyncFromDate = If(rdr.IsDBNull(9), CType(Nothing, Date?), rdr.GetDateTime(9).Date),
                .SyncToDate = If(rdr.IsDBNull(10), CType(Nothing, Date?), rdr.GetDateTime(10).Date),
                .ChunkDays = If(rdr.IsDBNull(11), CType(Nothing, Integer?), rdr.GetInt32(11)),
                .SyncCursor = If(rdr.IsDBNull(12), CType(Nothing, Date?), rdr.GetDateTime(12).Date)
            }
        End Function

        Public Function SaveJob(job As JobDefinition) As Integer
            Using conn = GetConnection()
                If job.Id = 0 Then
                    Using cmd As New SqlCommand(
                        "INSERT INTO dbo.Jobs (JobName,SourceType,EntityType,ScheduleType,IntervalMinutes," &
                        "NextRunTime,IsEnabled,SyncFromDate,SyncToDate,ChunkDays,SyncCursor) " &
                        "OUTPUT INSERTED.Id VALUES (@n,@src,@ent,@sched,@intv,@next,@ena,@sfd,@std,@cd,@sc)", conn)
                        SetJobParams(cmd, job)
                        Return CInt(cmd.ExecuteScalar())
                    End Using
                Else
                    Using cmd As New SqlCommand(
                        "UPDATE dbo.Jobs SET JobName=@n,SourceType=@src,EntityType=@ent,ScheduleType=@sched," &
                        "IntervalMinutes=@intv,NextRunTime=@next,IsEnabled=@ena," &
                        "SyncFromDate=@sfd,SyncToDate=@std,ChunkDays=@cd,SyncCursor=@sc WHERE Id=@id", conn)
                        SetJobParams(cmd, job)
                        cmd.Parameters.AddWithValue("@id", job.Id)
                        cmd.ExecuteNonQuery()
                        Return job.Id
                    End Using
                End If
            End Using
        End Function

        Private Sub SetJobParams(cmd As SqlCommand, job As JobDefinition)
            cmd.Parameters.AddWithValue("@n", job.JobName)
            cmd.Parameters.AddWithValue("@src", job.SourceType)
            cmd.Parameters.AddWithValue("@ent", job.EntityType)
            cmd.Parameters.AddWithValue("@sched", job.ScheduleType)
            cmd.Parameters.AddWithValue("@intv", If(job.IntervalMinutes.HasValue, CObj(job.IntervalMinutes.Value), DBNull.Value))
            cmd.Parameters.AddWithValue("@next", If(job.NextRunTime.HasValue, CObj(job.NextRunTime.Value), DBNull.Value))
            cmd.Parameters.AddWithValue("@ena", job.IsEnabled)
            cmd.Parameters.AddWithValue("@sfd", If(job.SyncFromDate.HasValue, CObj(job.SyncFromDate.Value), DBNull.Value))
            cmd.Parameters.AddWithValue("@std", If(job.SyncToDate.HasValue, CObj(job.SyncToDate.Value), DBNull.Value))
            cmd.Parameters.AddWithValue("@cd", If(job.ChunkDays.HasValue, CObj(job.ChunkDays.Value), DBNull.Value))
            cmd.Parameters.AddWithValue("@sc", If(job.SyncCursor.HasValue, CObj(job.SyncCursor.Value), DBNull.Value))
        End Sub

        ' Advance the backfill cursor; disable job when cursor has passed SyncToDate.
        Public Sub UpdateBackfillCursor(jobId As Integer, nextCursor As Date, syncToDate As Date)
            Dim complete = nextCursor > syncToDate
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "UPDATE dbo.Jobs SET SyncCursor=@cur, IsEnabled=@ena, " &
                    "NextRunTime=CASE WHEN @complete=1 THEN NULL ELSE NextRunTime END WHERE Id=@id", conn)
                    cmd.Parameters.AddWithValue("@cur", nextCursor)
                    cmd.Parameters.AddWithValue("@ena", Not complete)
                    cmd.Parameters.AddWithValue("@complete", complete)
                    cmd.Parameters.AddWithValue("@id", jobId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function GetBackfillCursor(jobId As Integer) As Date?
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "SELECT SyncCursor FROM dbo.Jobs WHERE Id=@id", conn)
                    cmd.Parameters.AddWithValue("@id", jobId)
                    Dim result = cmd.ExecuteScalar()
                    Return If(result Is Nothing OrElse IsDBNull(result), CType(Nothing, Date?), CDate(result))
                End Using
            End Using
        End Function

        Public Sub DeleteJob(jobId As Integer)
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "DELETE FROM dbo.JobHistory WHERE JobId=@id; DELETE FROM dbo.Jobs WHERE Id=@id", conn)
                    cmd.Parameters.AddWithValue("@id", jobId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Sub UpdateJobAfterRun(jobId As Integer, lastRun As DateTime, nextRun As DateTime?)
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "UPDATE dbo.Jobs SET LastRunTime=@last, NextRunTime=@next WHERE Id=@id", conn)
                    cmd.Parameters.AddWithValue("@last", lastRun)
                    cmd.Parameters.AddWithValue("@next", If(nextRun.HasValue, CObj(nextRun.Value), DBNull.Value))
                    cmd.Parameters.AddWithValue("@id", jobId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ' ── Job History ───────────────────────────────────────────────────────

        Public Function StartJobHistory(jobId As Integer) As Integer
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "INSERT INTO dbo.JobHistory (JobId,StartedAt,Status) OUTPUT INSERTED.Id " &
                    "VALUES (@jid, GETDATE(), 'Running')", conn)
                    cmd.Parameters.AddWithValue("@jid", jobId)
                    Return CInt(cmd.ExecuteScalar())
                End Using
            End Using
        End Function

        Public Sub CompleteJobHistory(historyId As Integer, status As String, records As Integer, errMsg As String)
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "UPDATE dbo.JobHistory SET CompletedAt=GETDATE(),Status=@s,RecordsAffected=@r,ErrorMessage=@e WHERE Id=@id", conn)
                    cmd.Parameters.AddWithValue("@s", status)
                    cmd.Parameters.AddWithValue("@r", If(records >= 0, CObj(records), DBNull.Value))
                    cmd.Parameters.AddWithValue("@e", If(String.IsNullOrEmpty(errMsg), CObj(DBNull.Value), errMsg))
                    cmd.Parameters.AddWithValue("@id", historyId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        Public Function GetJobHistory(Optional jobId As Integer = 0, Optional maxRows As Integer = 200) As List(Of JobHistory)
            Dim result As New List(Of JobHistory)
            Dim where_ = If(jobId > 0, "AND h.JobId = @jid ", "")
            Using conn = GetConnection()
                Using cmd As New SqlCommand(
                    "SELECT TOP (@max) h.Id,h.JobId,j.JobName,h.StartedAt,h.CompletedAt," &
                    "h.Status,h.RecordsAffected,h.ErrorMessage " &
                    "FROM dbo.JobHistory h JOIN dbo.Jobs j ON h.JobId=j.Id " &
                    "WHERE 1=1 " & where_ & "ORDER BY h.StartedAt DESC", conn)
                    cmd.Parameters.AddWithValue("@max", maxRows)
                    If jobId > 0 Then cmd.Parameters.AddWithValue("@jid", jobId)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            result.Add(New JobHistory With {
                                .Id = rdr.GetInt32(0),
                                .JobId = rdr.GetInt32(1),
                                .JobName = rdr.GetString(2),
                                .StartedAt = rdr.GetDateTime(3),
                                .CompletedAt = If(rdr.IsDBNull(4), CType(Nothing, DateTime?), rdr.GetDateTime(4)),
                                .Status = rdr.GetString(5),
                                .RecordsAffected = If(rdr.IsDBNull(6), CType(Nothing, Integer?), rdr.GetInt32(6)),
                                .ErrorMessage = If(rdr.IsDBNull(7), Nothing, rdr.GetString(7))
                            })
                        End While
                    End Using
                End Using
            End Using
            Return result
        End Function

        ' ── Deputy data queries ───────────────────────────────────────────────

        Public Function GetDeputyTimesheets(fromDate As Date, toDate As Date) As DataTable
            Using conn = GetConnection()
                Using da As New SqlDataAdapter(
                    "SELECT t.Id, t.TimesheetDate, t.StartTime, t.EndTime, t.TotalMinutes, t.Cost, " &
                    "t.IsApproved, e.DisplayName AS Employee, " &
                    "u.UnitName AS OperationalUnit, w.WorkTypeName AS WorkType " &
                    "FROM deputy.Timesheets t " &
                    "LEFT JOIN deputy.Employees e ON t.EmployeeId = e.Id " &
                    "LEFT JOIN deputy.OperationalUnits u ON t.OperationalUnitId = u.Id " &
                    "LEFT JOIN deputy.WorkTypes w ON t.WorkTypeId = w.Id " &
                    "WHERE t.TimesheetDate BETWEEN @from AND @to " &
                    "ORDER BY t.TimesheetDate DESC, t.StartTime", conn)
                    da.SelectCommand.Parameters.AddWithValue("@from", fromDate)
                    da.SelectCommand.Parameters.AddWithValue("@to", toDate)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Function

        Public Function GetDeputyTable(tableName As String) As DataTable
            Dim allowed As New HashSet(Of String)({"deputy.Employees", "deputy.OperationalUnits", "deputy.WorkTypes"})
            If Not allowed.Contains(tableName) Then Throw New ArgumentException("Invalid table name")
            Using conn = GetConnection()
                Using da As New SqlDataAdapter($"SELECT * FROM {tableName} ORDER BY Id", conn)
                    Dim dt As New DataTable()
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        End Function

        Public Function GetDashboardStats() As Dictionary(Of String, Integer)
            Dim stats As New Dictionary(Of String, Integer)
            Dim queries As New Dictionary(Of String, String) From {
                {"Timesheets", "SELECT COUNT(*) FROM deputy.Timesheets"},
                {"Employees", "SELECT COUNT(*) FROM deputy.Employees"},
                {"OperationalUnits", "SELECT COUNT(*) FROM deputy.OperationalUnits"},
                {"WorkTypes", "SELECT COUNT(*) FROM deputy.WorkTypes"}
            }
            Using conn = GetConnection()
                For Each kv In queries
                    Try
                        Using cmd As New SqlCommand(kv.Value, conn)
                            stats(kv.Key) = CInt(cmd.ExecuteScalar())
                        End Using
                    Catch
                        stats(kv.Key) = 0
                    End Try
                Next
            End Using
            Return stats
        End Function

    End Class

End Namespace
