Imports MTCDW.Services

' Application-wide singleton references for single-instance desktop use.
Module AppState
    Public Db As DatabaseService
    Public Scheduler As SchedulerService
End Module
