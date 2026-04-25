Public Class DashboardPanel

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Overrides Sub Refresh()
        MyBase.Refresh()
        LoadStats()
        LoadRecentHistory()
    End Sub

    Private Sub LoadStats()
        If AppState.Db Is Nothing Then Return
        Try
            Dim stats = AppState.Db.GetDashboardStats()
            lblTimesheets.Text = DictGet(stats, "Timesheets").ToString("N0")
            lblEmployees.Text = DictGet(stats, "Employees").ToString("N0")
            lblOpUnits.Text = DictGet(stats, "OperationalUnits").ToString("N0")
            lblWorkTypes.Text = DictGet(stats, "Rosters").ToString("N0")
        Catch ex As Exception
            lblTimesheets.Text = "—"
        End Try
    End Sub

    Private Sub LoadRecentHistory()
        If AppState.Db Is Nothing Then Return
        Try
            Dim hist = AppState.Db.GetJobHistory(maxRows:=20)
            dgHistory.DataSource = hist.Select(Function(h) New With {
                .Job = h.JobName,
                .Started = h.StartedAt.ToString("dd/MM HH:mm"),
                .Duration = h.Duration,
                .Status = h.Status,
                .Records = If(h.RecordsAffected.HasValue, h.RecordsAffected.Value.ToString("N0"), "—"),
                .Error = If(String.IsNullOrEmpty(h.ErrorMessage), "", h.ErrorMessage.Substring(0, Math.Min(80, h.ErrorMessage.Length)))
            }).ToList()
            GridColumnStore.Restore("DashboardHistory", dgHistory)
            ColourStatusRows()
        Catch ex As Exception
            ' Leave grid empty on error
        End Try
    End Sub

    Private Sub ColourStatusRows()
        For Each row As DataGridViewRow In dgHistory.Rows
            Dim status = If(row.Cells("Status").Value?.ToString(), "")
            Select Case status
                Case "Success" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(220, 255, 220)
                Case "Failed"  : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 220, 220)
                Case "Running" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 255, 200)
                Case Else      : row.DefaultCellStyle.BackColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Private Sub dgHistory_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgHistory.ColumnWidthChanged
        GridColumnStore.Save("DashboardHistory", dgHistory)
    End Sub

    Private Shared Function DictGet(d As Dictionary(Of String, Integer), key As String) As Integer
        Dim v As Integer
        Return If(d.TryGetValue(key, v), v, 0)
    End Function

End Class
