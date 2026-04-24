Namespace Models
    Public Class JobHistory
        Public Property Id As Integer
        Public Property JobId As Integer
        Public Property JobName As String
        Public Property StartedAt As DateTime
        Public Property CompletedAt As DateTime?
        Public Property Status As String   ' Running | Success | Failed
        Public Property RecordsAffected As Integer?
        Public Property ErrorMessage As String

        Public ReadOnly Property Duration As String
            Get
                If Not CompletedAt.HasValue Then Return "Running..."
                Dim secs = (CompletedAt.Value - StartedAt).TotalSeconds
                Return If(secs < 60, $"{secs:F0}s", $"{secs / 60:F1}m")
            End Get
        End Property
    End Class
End Namespace
