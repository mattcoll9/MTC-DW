Namespace Models.RevSport

    Public Class RsEvent
        Public Property DateStart As Date
        Public Property DateEnd As Date
        Public Property EventName As String
        Public Property EventDate As Date?
        Public Property Category As String
        Public Property StartTime As String
        Public Property EndTime As String
        Public Property Registered As Integer?
        Public Property Attended As Integer?
        Public Property Revenue As Decimal?
    End Class

    Public Class RsEventAttendee
        Public Property DateStart As Date
        Public Property DateEnd As Date
        Public Property MemberId As String
        Public Property MemberName As String
        Public Property Email As String
        Public Property EventName As String
        Public Property EventDate As Date?
        Public Property Category As String
        Public Property AttendanceStatus As String
        Public Property AmountPaid As Decimal?
    End Class

End Namespace
