Imports MTCDW.Services

' Application-wide singleton references for single-instance desktop use.
Module AppState
    Public Db As DatabaseService
    Public Scheduler As SchedulerService
    ' Set by MainForm to a thread-safe UI update. Call from anywhere to show activity in the status bar.
    Public Activity As Action(Of String)
End Module
