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
            Dim status = CStr(cboStatus.SelectedItem)
            Dim statusFilter = If(status = "All", Nothing, status)
            Dim hist = AppState.Db.GetJobHistory(maxRows:=500,
                                                  fromDate:=dtpFrom.Value.Date,
                                                  toDate:=dtpTo.Value.Date,
                                                  statusFilter:=statusFilter)

            dgLogs.DataSource = hist.Select(Function(h) New With {
                .Job = h.JobName,
                .Started = h.StartedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                .Completed = If(h.CompletedAt.HasValue, h.CompletedAt.Value.ToString("HH:mm:ss"), "—"),
                .Duration = h.Duration,
                .Status = h.Status,
                .Records = If(h.RecordsAffected.HasValue, h.RecordsAffected.Value.ToString("N0"), "—"),
                .Error = If(String.IsNullOrEmpty(h.ErrorMessage), "", h.ErrorMessage.Substring(0, Math.Min(200, h.ErrorMessage.Length)))
            }).ToList()

            lblCount.Text = $"{hist.Count} entries"
            GridColumnStore.Restore("LogsGrid", dgLogs)
            ColourRows()
        Catch ex As Exception
            MessageBox.Show($"Error loading logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ColourRows()
        For Each row As DataGridViewRow In dgLogs.Rows
            Dim status = If(row.Cells("Status")?.Value?.ToString(), "")
            Select Case status
                Case "Success" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(220, 255, 220)
                Case "Failed"  : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 220, 220)
                Case "Running" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 255, 200)
                Case Else      : row.DefaultCellStyle.BackColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Private Sub dgLogs_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgLogs.ColumnWidthChanged
        GridColumnStore.Save("LogsGrid", dgLogs)
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshLogs()
    End Sub

    Private Sub cboStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboStatus.SelectedIndexChanged
        RefreshLogs()
    End Sub

End Class
