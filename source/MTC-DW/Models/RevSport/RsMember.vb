Namespace Models.RevSport

    Public Class RsMember
        Public Property SeasonId As Integer
        Public Property ParentBodyId As String
        Public Property FullName As String
        Public Property DateOfBirth As Date?
        Public Property Gender As String
        Public Property CreationTime As DateTime?
        Public Property Address As String
        Public Property PhoneHome As String
        Public Property PhoneMobile As String
        Public Property Email As String
        Public Property PaymentStatus As String
        Public Property PaymentDate As Date?
        Public Property PaymentMethod As String
        Public Property PaymentReceipt As String
        Public Property PaymentWho As String
        Public Property Deceased As Boolean
        Public Property LastUpdated As DateTime?
    End Class

End Namespace
