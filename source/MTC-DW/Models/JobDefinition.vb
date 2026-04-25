Namespace Models
    Public Class JobDefinition
        Public Property Id As Integer
        Public Property JobName As String
        Public Property SourceType As String
        Public Property EntityType As String
        Public Property ScheduleType As String   ' Once | Recurring
        Public Property IntervalMinutes As Integer?
        Public Property NextRunTime As DateTime?
        Public Property LastRunTime As DateTime?
        Public Property IsEnabled As Boolean

        ' Backfill-only fields — NULL for Once/Recurring jobs.
        Public Property SyncFromDate As Date?
        Public Property SyncToDate As Date?
        Public Property ChunkDays As Integer?     ' days per pull (default 30)
        Public Property SyncCursor As Date?       ' auto-advanced after each chunk

        Public ReadOnly Property IntervalDisplay As String
            Get
                Select Case ScheduleType
                    Case "Backfill"
                        If Not SyncFromDate.HasValue OrElse Not SyncToDate.HasValue Then Return "Backfill"
                        Dim cursor As Date = If(SyncCursor.HasValue, SyncCursor.Value, SyncFromDate.Value)
                        Dim pct As Double = (cursor - SyncFromDate.Value).TotalDays / Math.Max(1, (SyncToDate.Value - SyncFromDate.Value).TotalDays) * 100
                        Return $"Backfill {pct:F0}%"
                    Case "Once" : Return "Once"
                    Case Else
                        If Not IntervalMinutes.HasValue Then Return "—"
                        Select Case IntervalMinutes.Value
                            Case 15 : Return "Every 15 min"
                            Case 60 : Return "Every 1 hour"
                            Case 240 : Return "Every 4 hours"
                            Case 720 : Return "Every 12 hours"
                            Case 1440 : Return "Every 24 hours"
                            Case Else : Return $"Every {IntervalMinutes} min"
                        End Select
                End Select
            End Get
        End Property

        Public ReadOnly Property IsBackfillComplete As Boolean
            Get
                If ScheduleType <> "Backfill" OrElse Not SyncToDate.HasValue Then Return False
                Return SyncCursor.HasValue AndAlso SyncCursor.Value > SyncToDate.Value
            End Get
        End Property
    End Class
End Namespace
