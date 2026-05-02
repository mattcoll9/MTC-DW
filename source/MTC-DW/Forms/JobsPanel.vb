Imports System.Threading
Imports MTCDW.Models

Public Class JobsPanel

    Private _jobs As List(Of JobDefinition)
    Private _runCts As CancellationTokenSource
    Private _btnStop As ToolStripButton
    Private _cmsEdit As ToolStripItem
    Private _cmsDelete As ToolStripItem
    Private _cmsRun As ToolStripItem

    Public Sub New()
        InitializeComponent()

        For Each dgv As DataGridView In {dgJobs, dgHistory}
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText
            dgv.AllowUserToOrderColumns = True
            dgv.MultiSelect = True
        Next

        _btnStop = New ToolStripButton("Stop") With {
            .Name = "btnStop",
            .Enabled = False,
            .ForeColor = Drawing.Color.DarkRed,
            .Font = New Drawing.Font("Segoe UI", 9, Drawing.FontStyle.Bold)
        }
        Dim sepStop As New ToolStripSeparator()
        tsJobs.Items.Add(sepStop)
        tsJobs.Items.Add(_btnStop)
        AddHandler _btnStop.Click, AddressOf btnStop_Click

        Dim cms As New ContextMenuStrip()
        Dim cmsAdd As ToolStripItem = cms.Items.Add("Add Job")
        _cmsEdit = cms.Items.Add("Edit Job")
        _cmsDelete = cms.Items.Add("Delete Job")
        cms.Items.Add(New ToolStripSeparator())
        _cmsRun = cms.Items.Add("Run Now")
        AddHandler cmsAdd.Click, AddressOf btnAdd_Click
        AddHandler _cmsEdit.Click, AddressOf btnEdit_Click
        AddHandler _cmsDelete.Click, AddressOf btnDelete_Click
        AddHandler _cmsRun.Click, AddressOf btnRunNow_Click
        AddHandler cms.Opening, AddressOf JobsContextMenu_Opening
        dgJobs.ContextMenuStrip = cms
    End Sub

    Public Sub RefreshJobs()
        If AppState.Db Is Nothing Then Return
        Try
            _jobs = AppState.Db.GetJobs()
            dgJobs.DataSource = _jobs.Select(Function(j) New With {
                .Id = j.Id,
                .Name = j.JobName,
                .Source = j.SourceType,
                .Entity = j.EntityType,
                .Schedule = j.IntervalDisplay,
                .NextRun = If(j.NextRunTime.HasValue, j.NextRunTime.Value.ToString("dd/MM HH:mm"), "—"),
                .LastRun = If(j.LastRunTime.HasValue, j.LastRunTime.Value.ToString("dd/MM HH:mm"), "Never"),
                .Enabled = If(j.IsEnabled, "Yes", "No")
            }).ToList()
            GridColumnStore.Restore("JobsJobs", dgJobs)
            RefreshHistory()
        Catch ex As Exception
            MessageBox.Show($"Error loading jobs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RefreshHistory()
        If AppState.Db Is Nothing Then Return
        Try
            Dim selectedId = SelectedJobId()
            Dim hist = AppState.Db.GetJobHistory(selectedId, 50)
            Dim dt As New DataTable()
            dt.Columns.Add("Started", GetType(DateTime))
            dt.Columns.Add("Duration", GetType(String))
            dt.Columns.Add("Status", GetType(String))
            dt.Columns.Add("Records", GetType(String))
            dt.Columns.Add("Error", GetType(String))
            For Each h In hist
                dt.Rows.Add(h.StartedAt, h.Duration, h.Status,
                            If(h.RecordsAffected.HasValue, h.RecordsAffected.Value.ToString("N0"), "—"),
                            If(String.IsNullOrEmpty(h.ErrorMessage), "", h.ErrorMessage.Substring(0, Math.Min(120, h.ErrorMessage.Length))))
            Next
            dgHistory.DataSource = dt
            GridColumnStore.Restore("JobsHistory", dgHistory)
            If dgHistory.Columns.Contains("Started") Then
                dgHistory.Columns("Started").DefaultCellStyle.Format = "dd/MM HH:mm:ss"
                dgHistory.Sort(dgHistory.Columns("Started"), System.ComponentModel.ListSortDirection.Descending)
            End If
            ColourRows(dgHistory)
        Catch
        End Try
    End Sub

    Private Function SelectedJobId() As Integer
        If dgJobs.CurrentRow Is Nothing OrElse _jobs Is Nothing Then Return 0
        Dim idx = dgJobs.CurrentRow.Index
        If idx < 0 OrElse idx >= _jobs.Count Then Return 0
        Return _jobs(idx).Id
    End Function

    Private Function SelectedJob() As JobDefinition
        Dim idx = If(dgJobs.CurrentRow Is Nothing, -1, dgJobs.CurrentRow.Index)
        If idx < 0 OrElse _jobs Is Nothing OrElse idx >= _jobs.Count Then Return Nothing
        Return _jobs(idx)
    End Function

    Private Sub ColourRows(dgv As DataGridView)
        For Each row As DataGridViewRow In dgv.Rows
            Dim status = If(row.Cells("Status")?.Value?.ToString(), "")
            Select Case status
                Case "Success" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(220, 255, 220)
                Case "Failed"  : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 220, 220)
                Case "Running" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 255, 200)
                Case Else      : row.DefaultCellStyle.BackColor = Drawing.Color.White
            End Select
        Next
    End Sub

    ' ── Toolbar button handlers ───────────────────────────────────────────────

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using frm As New JobEditForm(Nothing)
            If frm.ShowDialog() = DialogResult.OK Then
                AppState.Db.SaveJob(frm.Result)
                RefreshJobs()
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Dim job = SelectedJob()
        If job Is Nothing Then Return
        Using frm As New JobEditForm(job)
            If frm.ShowDialog() = DialogResult.OK Then
                AppState.Db.SaveJob(frm.Result)
                RefreshJobs()
            End If
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim job = SelectedJob()
        If job Is Nothing Then Return
        If MessageBox.Show($"Delete job '{job.JobName}' and its history?", "Confirm",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            AppState.Db.DeleteJob(job.Id)
            RefreshJobs()
        End If
    End Sub

    Private Async Sub btnRunNow_Click(sender As Object, e As EventArgs) Handles btnRunNow.Click
        Dim job = SelectedJob()
        If job Is Nothing Then Return
        _runCts = New CancellationTokenSource()
        btnRunNow.Enabled = False
        _btnStop.Enabled = True
        Try
            Await AppState.Scheduler.RunJobNow(job, _runCts.Token)
            RefreshJobs()
        Catch ex As Exception
            MessageBox.Show($"Job failed: {ex.Message}", "Run Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnRunNow.Enabled = True
            _btnStop.Enabled = False
            _runCts?.Dispose()
            _runCts = Nothing
        End Try
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs)
        _runCts?.Cancel()
        _btnStop.Enabled = False
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshJobs()
    End Sub

    Private Sub dgJobs_SelectionChanged(sender As Object, e As EventArgs) Handles dgJobs.SelectionChanged
        RefreshHistory()
    End Sub

    Private Sub JobsContextMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Dim hasSelection = SelectedJob() IsNot Nothing
        _cmsEdit.Enabled = hasSelection
        _cmsDelete.Enabled = hasSelection
        _cmsRun.Enabled = hasSelection
    End Sub

    Private Sub dgJobs_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgJobs.ColumnWidthChanged
        GridColumnStore.Save("JobsJobs", dgJobs)
    End Sub

    Private Sub dgHistory_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgHistory.ColumnWidthChanged
        GridColumnStore.Save("JobsHistory", dgHistory)
    End Sub

End Class
