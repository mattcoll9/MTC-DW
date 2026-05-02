Public Class LogsPanel

    Public Sub New()
        InitializeComponent()
        cboStatus.Items.AddRange({"All", "Success", "Failed", "Running"})
        cboStatus.SelectedIndex = 0
        dtpFrom.Value = DateTime.Today.AddDays(-7)
        dtpTo.Value = DateTime.Today.AddDays(1)
        dgLogs.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText
        dgLogs.AllowUserToOrderColumns = True
        dgLogs.MultiSelect = True
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

            Dim dt As New DataTable()
            dt.Columns.Add("HistId", GetType(Integer))
            dt.Columns.Add("Job", GetType(String))
            dt.Columns.Add("Started", GetType(DateTime))
            dt.Columns.Add("Completed", GetType(String))
            dt.Columns.Add("Duration", GetType(String))
            dt.Columns.Add("Status", GetType(String))
            dt.Columns.Add("Records", GetType(String))
            dt.Columns.Add("Error", GetType(String))
            For Each h In hist
                dt.Rows.Add(h.Id, h.JobName, h.StartedAt,
                            If(h.CompletedAt.HasValue, h.CompletedAt.Value.ToString("HH:mm:ss"), "—"),
                            h.Duration, h.Status,
                            If(h.RecordsAffected.HasValue, h.RecordsAffected.Value.ToString("N0"), "—"),
                            If(h.ErrorMessage, ""))
            Next
            dgLogs.DataSource = dt

            lblCount.Text = $"{hist.Count} entries"
            GridColumnStore.Restore("LogsGrid", dgLogs)
            If dgLogs.Columns.Contains("HistId") Then dgLogs.Columns("HistId").Visible = False
            If dgLogs.Columns.Contains("Started") Then
                dgLogs.Columns("Started").DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss"
                dgLogs.Sort(dgLogs.Columns("Started"), System.ComponentModel.ListSortDirection.Descending)
            End If
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

    Private Sub dgLogs_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgLogs.CellDoubleClick
        If e.RowIndex < 0 OrElse dgLogs.Columns("Error") Is Nothing Then Return
        Dim msg = If(dgLogs.Rows(e.RowIndex).Cells("Error").Value?.ToString(), "")
        If String.IsNullOrEmpty(msg) Then Return
        Dim frm As New Form() With {
            .Text = "Error Detail",
            .Size = New Drawing.Size(700, 300),
            .StartPosition = FormStartPosition.CenterParent,
            .MinimizeBox = False
        }
        Dim txt As New TextBox() With {
            .Dock = DockStyle.Fill,
            .Multiline = True,
            .ReadOnly = True,
            .ScrollBars = ScrollBars.Both,
            .WordWrap = False,
            .Font = New Drawing.Font("Consolas", 9),
            .Text = msg
        }
        frm.Controls.Add(txt)
        frm.ShowDialog(Me)
    End Sub

    Private Sub btnViewLog_Click(sender As Object, e As EventArgs) Handles btnViewLog.Click
        If AppState.Db Is Nothing OrElse dgLogs.CurrentRow Is Nothing Then Return
        Dim histId As Object = dgLogs.CurrentRow.Cells("HistId").Value
        If histId Is Nothing OrElse IsDBNull(histId) Then Return
        Dim dt = AppState.Db.GetVerboseLog(CInt(histId))
        If dt.Rows.Count = 0 Then
            MessageBox.Show("No verbose log for this run. Enable 'Verbose Log' on the job and re-run.", "No Log", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim frm As New Form() With {
            .Text = "Verbose Log",
            .Size = New Drawing.Size(800, 500),
            .StartPosition = FormStartPosition.CenterParent,
            .MinimizeBox = False
        }
        Dim dgv As New DataGridView() With {
            .Dock = DockStyle.Fill,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .RowHeadersVisible = False,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }
        frm.Controls.Add(dgv)
        dgv.DataSource = dt
        frm.ShowDialog(Me)
    End Sub

End Class
