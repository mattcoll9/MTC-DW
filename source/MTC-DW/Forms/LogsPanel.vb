Public Class LogsPanel

    Public Sub New()
        InitializeComponent()
        cboStatus.Items.AddRange({"All", "Success", "Failed", "Running"})
        cboStatus.SelectedIndex = 0
        dtpFrom.Value = DateTime.Today.AddDays(-7)
        dtpTo.Value = DateTime.Today.AddDays(1)
    End Sub

    Public Sub RefreshLogs()
        If AppState.Db Is Nothing Then Return
        Try
            Dim hist = AppState.Db.GetJobHistory(maxRows:=500)
            Dim status = CStr(cboStatus.SelectedItem)

            Dim filtered = hist.Where(Function(h)
                If h.StartedAt.Date < dtpFrom.Value.Date OrElse h.StartedAt.Date > dtpTo.Value.Date Then Return False
                If status <> "All" AndAlso h.Status <> status Then Return False
                Return True
            End Function).ToList()

            dgLogs.DataSource = filtered.Select(Function(h) New With {
                .Job = h.JobName,
                .Started = h.StartedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                .Completed = If(h.CompletedAt.HasValue, h.CompletedAt.Value.ToString("HH:mm:ss"), "—"),
                .Duration = h.Duration,
                .Status = h.Status,
                .Records = If(h.RecordsAffected.HasValue, h.RecordsAffected.Value.ToString("N0"), "—"),
                .Error = If(String.IsNullOrEmpty(h.ErrorMessage), "", h.ErrorMessage.Substring(0, Math.Min(200, h.ErrorMessage.Length)))
            }).ToList()

            lblCount.Text = $"{filtered.Count} entries"
            ColourRows()
        Catch ex As Exception
            MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ColourRows()
        For Each row As DataGridViewRow In dgLogs.Rows
            Dim status = If(row.Cells("Status")?.Value?.ToString(), "")
            row.DefaultCellStyle.BackColor = Select Case status
                Case "Success" : Drawing.Color.FromArgb(220, 255, 220)
                Case "Failed" : Drawing.Color.FromArgb(255, 220, 220)
                Case "Running" : Drawing.Color.FromArgb(255, 255, 200)
                Case Else : Drawing.Color.White
            End Select
        Next
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshLogs()
    End Sub

    Private Sub cboStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboStatus.SelectedIndexChanged
        RefreshLogs()
    End Sub

End Class
