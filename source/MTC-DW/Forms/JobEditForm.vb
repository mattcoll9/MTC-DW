Imports MTCDW.Models

Public Class JobEditForm

    Public Property Result As JobDefinition

    Public Sub New(existingJob As JobDefinition)
        InitializeComponent()

        cboSource.Items.AddRange({"Deputy"})
        cboEntity.Items.AddRange({"Timesheets", "Employees", "OperationalUnits", "WorkTypes"})
        cboInterval.Items.AddRange({"2", "5", "15", "60", "240", "720", "1440"})

        If existingJob IsNot Nothing Then
            txtName.Text = existingJob.JobName
            cboSource.SelectedItem = existingJob.SourceType
            cboEntity.SelectedItem = existingJob.EntityType
            chkEnabled.Checked = existingJob.IsEnabled

            Select Case existingJob.ScheduleType
                Case "Recurring"
                    rbRecurring.Checked = True
                    cboInterval.Text = existingJob.IntervalMinutes?.ToString() ?? "60"
                Case "Backfill"
                    rbBackfill.Checked = True
                    cboInterval.Text = existingJob.IntervalMinutes?.ToString() ?? "2"
                    If existingJob.SyncFromDate.HasValue Then dtpBackfillFrom.Value = existingJob.SyncFromDate.Value
                    If existingJob.SyncToDate.HasValue Then dtpBackfillTo.Value = existingJob.SyncToDate.Value
                    nudChunkDays.Value = If(existingJob.ChunkDays.HasValue, existingJob.ChunkDays.Value, 30)
                Case Else
                    rbOnce.Checked = True
            End Select

            If existingJob.NextRunTime.HasValue Then dtpNextRun.Value = existingJob.NextRunTime.Value
            Result = existingJob
        Else
            rbRecurring.Checked = True
            cboSource.SelectedIndex = 0
            cboEntity.SelectedIndex = 0
            cboInterval.SelectedItem = "60"
            chkEnabled.Checked = True
            dtpNextRun.Value = DateTime.Now.AddMinutes(5)
            dtpBackfillFrom.Value = DateTime.Today.AddYears(-2)
            dtpBackfillTo.Value = DateTime.Today
            nudChunkDays.Value = 30
            Result = New JobDefinition()
        End If

        UpdateVisibility()
    End Sub

    Private Sub AnyScheduleChanged(sender As Object, e As EventArgs) _
        Handles rbOnce.CheckedChanged, rbRecurring.CheckedChanged, rbBackfill.CheckedChanged
        UpdateVisibility()
    End Sub

    Private Sub cboEntity_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboEntity.SelectedIndexChanged
        UpdateVisibility()
    End Sub

    Private Sub UpdateVisibility()
        Dim isRecurring = rbRecurring.Checked
        Dim isBackfill = rbBackfill.Checked
        Dim isTimesheets = CStr(cboEntity.SelectedItem) = "Timesheets"

        lblInterval.Visible = isRecurring OrElse isBackfill
        cboInterval.Visible = isRecurring OrElse isBackfill
        lblIntervalMin.Visible = isRecurring OrElse isBackfill
        lblIntervalHint.Visible = isBackfill

        ' Backfill date range only makes sense for Timesheets
        pnlBackfill.Visible = isBackfill AndAlso isTimesheets
        If isBackfill AndAlso Not isTimesheets Then
            lblBackfillNote.Visible = True
            lblBackfillNote.Text = "Note: Backfill date range applies to Timesheets only."
        Else
            lblBackfillNote.Visible = False
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If String.IsNullOrWhiteSpace(txtName.Text) Then
            MessageBox.Show("Job name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If cboSource.SelectedItem Is Nothing OrElse cboEntity.SelectedItem Is Nothing Then
            MessageBox.Show("Select both a source and an entity.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Result.JobName = txtName.Text.Trim()
        Result.SourceType = CStr(cboSource.SelectedItem)
        Result.EntityType = CStr(cboEntity.SelectedItem)
        Result.IsEnabled = chkEnabled.Checked
        Result.NextRunTime = dtpNextRun.Value

        If rbRecurring.Checked Then
            Result.ScheduleType = "Recurring"
            Dim intv As Integer
            If Not Integer.TryParse(cboInterval.Text, intv) OrElse intv <= 0 Then
                MessageBox.Show("Enter a valid interval in minutes.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Result.IntervalMinutes = intv
            Result.SyncFromDate = Nothing
            Result.SyncToDate = Nothing
            Result.ChunkDays = Nothing

        ElseIf rbBackfill.Checked Then
            If dtpBackfillFrom.Value.Date >= dtpBackfillTo.Value.Date Then
                MessageBox.Show("Backfill 'From' date must be before 'To' date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Result.ScheduleType = "Backfill"
            Dim intv As Integer
            If Not Integer.TryParse(cboInterval.Text, intv) OrElse intv <= 0 Then intv = 2
            Result.IntervalMinutes = intv
            Result.SyncFromDate = dtpBackfillFrom.Value.Date
            Result.SyncToDate = dtpBackfillTo.Value.Date
            Result.ChunkDays = CInt(nudChunkDays.Value)
            ' Reset cursor so backfill restarts from the beginning if re-saved
            If Result.Id = 0 Then Result.SyncCursor = Nothing

        Else
            Result.ScheduleType = "Once"
            Result.IntervalMinutes = Nothing
            Result.SyncFromDate = Nothing
            Result.SyncToDate = Nothing
            Result.ChunkDays = Nothing
        End If

        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

End Class
