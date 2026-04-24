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
            lblTimesheets.Text = stats.GetValueOrDefault("Timesheets", 0).ToString("N0")
            lblEmployees.Text = stats.GetValueOrDefault("Employees", 0).ToString("N0")
            lblOpUnits.Text = stats.GetValueOrDefault("OperationalUnits", 0).ToString("N0")
            lblWorkTypes.Text = stats.GetValueOrDefault("WorkTypes", 0).ToString("N0")
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
            ColourStatusRows()
        Catch ex As Exception
            ' Leave grid empty on error
        End Try
    End Sub

    Private Sub ColourStatusRows()
        For Each row As DataGridViewRow In dgHistory.Rows
            Dim status = If(row.Cells("Status").Value?.ToString(), "")
            row.DefaultCellStyle.BackColor = Select Case status
                Case "Success" : Drawing.Color.FromArgb(220, 255, 220)
                Case "Failed" : Drawing.Color.FromArgb(255, 220, 220)
                Case "Running" : Drawing.Color.FromArgb(255, 255, 200)
                Case Else : Drawing.Color.White
            End Select
        Next
    End Sub

End Class
