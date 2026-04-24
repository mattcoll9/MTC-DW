Namespace Models.Deputy
    ' DisplayName included at operator request for internal use only.
    ' No contact info, no full DOB — YearOfBirth for age demographic only.
    Public Class DpEmployee
        Public Property Id As Long
        Public Property DisplayName As String
        Public Property RoleTitle As String
        Public Property YearOfBirth As Short?
        Public Property StartYear As Short?
        Public Property IsActive As Boolean
    End Class
End Namespace
