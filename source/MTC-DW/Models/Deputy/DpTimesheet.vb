Namespace Models.Deputy
    Public Class DpTimesheet
        Public Property Id As Long
        Public Property TimesheetDate As Date?
        Public Property StartTime As DateTime?
        Public Property EndTime As DateTime?
        Public Property TotalMinutes As Decimal
        Public Property Cost As Decimal
        Public Property IsApproved As Boolean
        Public Property EmployeeId As Long
        Public Property OperationalUnitId As Long?
        Public Property WorkTypeId As Long?
    End Class
End Namespace
