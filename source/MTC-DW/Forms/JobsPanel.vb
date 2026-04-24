Imports MTCDW.Models

Public Class JobsPanel

    Private _jobs As List(Of JobDefinition)

    Public Sub New()
        InitializeComponent()
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
            dgHistory.DataSource = hist.Select(Function(h) New With {
                .Started = h.StartedAt.ToString("dd/MM HH:mm:ss"),
                .Duration = h.Duration,
                .Status = h.Status,
                .Records = If(h.RecordsAffected.HasValue, h.RecordsAffected.Value.ToString("N0"), "—"),
                .Error = If(String.IsNullOrEmpty(h.ErrorMessage), "", h.ErrorMessage.Substring(0, Math.Min(120, h.ErrorMessage.Length)))
            }).ToList()
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
            row.DefaultCellStyle.BackColor = Select Case status
                Case "Success" : Drawing.Color.FromArgb(220, 255, 220)
                Case "Failed" : Drawing.Color.FromArgb(255, 220, 220)
                Case "Running" : Drawing.Color.FromArgb(255, 255, 200)
                Case Else : Drawing.Color.White
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
        btnRunNow.Enabled = False
        Try
            Await AppState.Scheduler.RunJobNow(job)
            RefreshJobs()
        Catch ex As Exception
            MessageBox.Show($"Job failed: {ex.Message}", "Run Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnRunNow.Enabled = True
        End Try
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        RefreshJobs()
    End Sub

    Private Sub dgJobs_SelectionChanged(sender As Object, e As EventArgs) Handles dgJobs.SelectionChanged
        RefreshHistory()
    End Sub

End Class
