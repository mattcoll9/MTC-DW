Imports System.Threading
Imports System.Threading.Tasks
Imports MTCDW.Models

Namespace Services

    Public Class SchedulerService

        Private _timer As Timer
        Private _db As DatabaseService
        Private _lock As New Object()
        Private _running As Boolean

        Public Event JobStarted As EventHandler(Of JobEventArgs)
        Public Event JobCompleted As EventHandler(Of JobEventArgs)
        Public Event JobFailed As EventHandler(Of JobEventArgs)

        Public Class JobEventArgs
            Inherits EventArgs
            Public Property JobId As Integer
            Public Property JobName As String
            Public Property RecordsAffected As Integer
            Public Property ErrorMessage As String
        End Class

        Public Sub New(db As DatabaseService)
            _db = db
        End Sub

        Public Sub Start()
            _running = True
            ' Check every 60 seconds; fire immediately on start.
            _timer = New Timer(AddressOf TimerCallback, Nothing, 0, 60000)
        End Sub

        Public Sub [Stop]()
            _running = False
            _timer?.Dispose()
            _timer = Nothing
        End Sub

        Private Sub TimerCallback(state As Object)
            If Not _running Then Return
            If Not Monitor.TryEnter(_lock) Then Return  ' Skip tick if previous still running
            Try
                Dim due = _db.GetDueJobs()
                For Each job In due
                    RunJobAsync(job).Wait()
                Next
            Catch
                ' Keep scheduler alive through any error
            Finally
                Monitor.Exit(_lock)
            End Try
        End Sub

        Public Async Function RunJobNow(job As JobDefinition) As Task
            Await RunJobAsync(job)
        End Function

        Private Async Function RunJobAsync(job As JobDefinition) As Task
            Dim histId = _db.StartJobHistory(job.Id)
            RaiseEvent JobStarted(Me, New JobEventArgs With {.JobId = job.Id, .JobName = job.JobName})

            Dim records As Integer = -1
            Try
                records = Await DispatchJob(job)

                Dim nextRun As DateTime? = Nothing
                Select Case job.ScheduleType
                    Case "Recurring"
                        If job.IntervalMinutes.HasValue Then
                            nextRun = DateTime.Now.AddMinutes(job.IntervalMinutes.Value)
                        End If

                    Case "Backfill"
                        ' Cursor was advanced by DispatchDeputy. Re-read to decide whether to reschedule.
                        Dim updatedCursor = _db.GetBackfillCursor(job.Id)
                        If updatedCursor.HasValue AndAlso job.SyncToDate.HasValue AndAlso
                           updatedCursor.Value <= job.SyncToDate.Value Then
                            ' More chunks remain — schedule next chunk soon.
                            nextRun = DateTime.Now.AddMinutes(If(job.IntervalMinutes.HasValue, job.IntervalMinutes.Value, 2))
                        End If
                        ' If cursor > SyncToDate, UpdateBackfillCursor already disabled the job;
                        ' nextRun stays Nothing so UpdateJobAfterRun clears NextRunTime.
                End Select

                _db.UpdateJobAfterRun(job.Id, DateTime.Now, nextRun)
                _db.CompleteJobHistory(histId, "Success", records, Nothing)
                RaiseEvent JobCompleted(Me, New JobEventArgs With {.JobId = job.Id, .JobName = job.JobName, .RecordsAffected = records})

            Catch ex As Exception
                _db.CompleteJobHistory(histId, "Failed", -1, ex.Message)
                RaiseEvent JobFailed(Me, New JobEventArgs With {.JobId = job.Id, .JobName = job.JobName, .ErrorMessage = ex.Message})
            End Try
        End Function

        Private Async Function DispatchJob(job As JobDefinition) As Task(Of Integer)
            Select Case job.SourceType
                Case "Deputy"
                    Return Await DispatchDeputy(job)
                Case Else
                    Throw New NotSupportedException($"Unknown source type: {job.SourceType}")
            End Select
        End Function

        Private Async Function DispatchDeputy(job As JobDefinition) As Task(Of Integer)
            Dim baseUrl = _db.GetConfigValue("Deputy.BaseUrl")
            Dim token = _db.GetConfigValue("Deputy.OAuthToken")
            If String.IsNullOrEmpty(baseUrl) OrElse String.IsNullOrEmpty(token) Then
                Throw New InvalidOperationException("Deputy credentials not configured. Open Settings and set Deputy.BaseUrl and Deputy.OAuthToken.")
            End If

            Dim api As New DeputyApiService(baseUrl, token)
            Dim sync As New DeputySyncService(_db, api)

            Select Case job.EntityType
                Case "Timesheets"
                    If job.ScheduleType = "Backfill" AndAlso
                       job.SyncFromDate.HasValue AndAlso job.SyncToDate.HasValue Then
                        Return Await RunBackfillChunk(job, sync)
                    End If
                    Return Await sync.SyncTimesheets()

                Case "Employees" : Return Await sync.SyncEmployees()
                Case "OperationalUnits" : Return Await sync.SyncOperationalUnits()
                Case "WorkTypes" : Return Await sync.SyncWorkTypes()
                Case Else : Throw New NotSupportedException($"Unknown entity type: {job.EntityType}")
            End Select
        End Function

        Private Async Function RunBackfillChunk(job As JobDefinition, sync As DeputySyncService) As Task(Of Integer)
            Dim chunkDays = If(job.ChunkDays.HasValue, job.ChunkDays.Value, 30)
            ' SyncCursor starts at SyncFromDate on the first run, then advances each chunk.
            Dim cursor = If(job.SyncCursor.HasValue, job.SyncCursor.Value, job.SyncFromDate.Value)
            Dim chunkEnd = cursor.AddDays(chunkDays - 1)
            If chunkEnd > job.SyncToDate.Value Then chunkEnd = job.SyncToDate.Value

            Dim count = Await sync.SyncTimesheetRange(cursor, chunkEnd)

            ' Persist the next cursor; UpdateBackfillCursor auto-disables when past SyncToDate.
            _db.UpdateBackfillCursor(job.Id, chunkEnd.AddDays(1), job.SyncToDate.Value)
            Return count
        End Function

    End Class

End Namespace
