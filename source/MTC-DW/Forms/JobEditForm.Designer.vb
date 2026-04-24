<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class JobEditForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.tlp = New System.Windows.Forms.TableLayoutPanel()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.lblSource = New System.Windows.Forms.Label()
        Me.cboSource = New System.Windows.Forms.ComboBox()
        Me.lblEntity = New System.Windows.Forms.Label()
        Me.cboEntity = New System.Windows.Forms.ComboBox()
        Me.lblSchedule = New System.Windows.Forms.Label()
        Me.pnlRadio = New System.Windows.Forms.Panel()
        Me.rbOnce = New System.Windows.Forms.RadioButton()
        Me.rbRecurring = New System.Windows.Forms.RadioButton()
        Me.rbBackfill = New System.Windows.Forms.RadioButton()
        Me.lblInterval = New System.Windows.Forms.Label()
        Me.cboInterval = New System.Windows.Forms.ComboBox()
        Me.lblIntervalMin = New System.Windows.Forms.Label()
        Me.lblIntervalHint = New System.Windows.Forms.Label()
        Me.lblNextRun = New System.Windows.Forms.Label()
        Me.dtpNextRun = New System.Windows.Forms.DateTimePicker()
        Me.lblEnabled = New System.Windows.Forms.Label()
        Me.chkEnabled = New System.Windows.Forms.CheckBox()
        Me.pnlBackfill = New System.Windows.Forms.Panel()
        Me.nudChunkDays = New System.Windows.Forms.NumericUpDown()
        Me.dtpBackfillTo = New System.Windows.Forms.DateTimePicker()
        Me.dtpBackfillFrom = New System.Windows.Forms.DateTimePicker()
        Me.lblChunkDays = New System.Windows.Forms.Label()
        Me.lblBackfillTo = New System.Windows.Forms.Label()
        Me.lblBackfillFrom = New System.Windows.Forms.Label()
        Me.lblBackfillNote = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.tlp.SuspendLayout()
        Me.pnlRadio.SuspendLayout()
        CType(Me.nudChunkDays, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlBackfill.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        ' tlp — main layout: label column + control column
        '
        Me.tlp.ColumnCount = 2
        Me.tlp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120.0!))
        Me.tlp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlp.RowCount = 9
        For i = 0 To 8
            Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Next
        Me.tlp.Controls.Add(Me.lblName, 0, 0)
        Me.tlp.Controls.Add(Me.txtName, 1, 0)
        Me.tlp.Controls.Add(Me.lblSource, 0, 1)
        Me.tlp.Controls.Add(Me.cboSource, 1, 1)
        Me.tlp.Controls.Add(Me.lblEntity, 0, 2)
        Me.tlp.Controls.Add(Me.cboEntity, 1, 2)
        Me.tlp.Controls.Add(Me.lblSchedule, 0, 3)
        Me.tlp.Controls.Add(Me.pnlRadio, 1, 3)
        Me.tlp.Controls.Add(Me.lblInterval, 0, 4)
        Me.tlp.Controls.Add(Me.cboInterval, 1, 4)
        Me.tlp.Controls.Add(Me.lblNextRun, 0, 5)
        Me.tlp.Controls.Add(Me.dtpNextRun, 1, 5)
        Me.tlp.Controls.Add(Me.lblEnabled, 0, 6)
        Me.tlp.Controls.Add(Me.chkEnabled, 1, 6)
        Me.tlp.Controls.Add(Me.pnlBackfill, 0, 7)
        Me.tlp.SetColumnSpan(Me.pnlBackfill, 2)
        Me.tlp.Controls.Add(Me.lblBackfillNote, 0, 8)
        Me.tlp.SetColumnSpan(Me.lblBackfillNote, 2)
        Me.tlp.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlp.Name = "tlp"
        Me.tlp.Padding = New System.Windows.Forms.Padding(8)
        Me.tlp.Size = New System.Drawing.Size(480, 310)
        Me.tlp.TabIndex = 0
        '
        ' Standard labels
        '
        For Each pair In {
            New With {.Lbl = Me.lblName, .T = "Job Name:"},
            New With {.Lbl = Me.lblSource, .T = "Source:"},
            New With {.Lbl = Me.lblEntity, .T = "Entity:"},
            New With {.Lbl = Me.lblSchedule, .T = "Schedule:"},
            New With {.Lbl = Me.lblInterval, .T = "Interval (min):"},
            New With {.Lbl = Me.lblNextRun, .T = "First Run:"},
            New With {.Lbl = Me.lblEnabled, .T = "Enabled:"}
        }
            pair.Lbl.Text = pair.T
            pair.Lbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            pair.Lbl.Dock = System.Windows.Forms.DockStyle.Fill
        Next
        '
        ' txtName
        '
        Me.txtName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtName.Name = "txtName"
        '
        ' cboSource / cboEntity / cboInterval
        '
        For Each cbo In {Me.cboSource, Me.cboEntity, Me.cboInterval}
            cbo.Dock = System.Windows.Forms.DockStyle.Fill
            cbo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown
        Next
        Me.cboSource.Name = "cboSource"
        Me.cboEntity.Name = "cboEntity"
        Me.cboInterval.Name = "cboInterval"
        '
        ' pnlRadio — Once / Recurring / Backfill radio buttons
        '
        Me.pnlRadio.Controls.Add(Me.rbOnce)
        Me.pnlRadio.Controls.Add(Me.rbRecurring)
        Me.pnlRadio.Controls.Add(Me.rbBackfill)
        Me.pnlRadio.Controls.Add(Me.lblIntervalHint)
        Me.pnlRadio.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlRadio.Name = "pnlRadio"
        Me.rbOnce.Text = "Once"
        Me.rbOnce.Name = "rbOnce"
        Me.rbOnce.Location = New System.Drawing.Point(0, 6)
        Me.rbOnce.Size = New System.Drawing.Size(60, 20)
        Me.rbRecurring.Text = "Recurring"
        Me.rbRecurring.Name = "rbRecurring"
        Me.rbRecurring.Location = New System.Drawing.Point(64, 6)
        Me.rbRecurring.Size = New System.Drawing.Size(80, 20)
        Me.rbBackfill.Text = "Backfill (historical)"
        Me.rbBackfill.Name = "rbBackfill"
        Me.rbBackfill.Location = New System.Drawing.Point(148, 6)
        Me.rbBackfill.Size = New System.Drawing.Size(140, 20)
        Me.rbBackfill.ForeColor = System.Drawing.Color.DarkBlue
        '
        ' lblIntervalHint — shown when Backfill selected
        '
        Me.lblIntervalHint.Text = "(gap between chunks)"
        Me.lblIntervalHint.Name = "lblIntervalHint"
        Me.lblIntervalHint.ForeColor = System.Drawing.Color.Gray
        Me.lblIntervalHint.Font = New System.Drawing.Font("Segoe UI", 8)
        Me.lblIntervalHint.Location = New System.Drawing.Point(0, 28)  ' won't be visible in normal rows
        Me.lblIntervalHint.Size = New System.Drawing.Size(160, 18)
        '
        ' dtpNextRun
        '
        Me.dtpNextRun.CustomFormat = "dd/MM/yyyy HH:mm"
        Me.dtpNextRun.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dtpNextRun.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpNextRun.Name = "dtpNextRun"
        '
        ' chkEnabled
        '
        Me.chkEnabled.Checked = True
        Me.chkEnabled.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkEnabled.Dock = System.Windows.Forms.DockStyle.Fill
        Me.chkEnabled.Name = "chkEnabled"
        Me.chkEnabled.Text = ""
        '
        ' pnlBackfill — date range + chunk size controls
        '
        Me.pnlBackfill.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pnlBackfill.BackColor = System.Drawing.Color.FromArgb(240, 248, 255)
        Me.pnlBackfill.Controls.Add(Me.lblBackfillFrom)
        Me.pnlBackfill.Controls.Add(Me.dtpBackfillFrom)
        Me.pnlBackfill.Controls.Add(Me.lblBackfillTo)
        Me.pnlBackfill.Controls.Add(Me.dtpBackfillTo)
        Me.pnlBackfill.Controls.Add(Me.lblChunkDays)
        Me.pnlBackfill.Controls.Add(Me.nudChunkDays)
        Me.pnlBackfill.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlBackfill.Name = "pnlBackfill"
        Me.pnlBackfill.Padding = New System.Windows.Forms.Padding(6)
        '
        ' Backfill inner controls
        '
        Me.lblBackfillFrom.Text = "Pull From:"
        Me.lblBackfillFrom.Location = New System.Drawing.Point(6, 8)
        Me.lblBackfillFrom.Size = New System.Drawing.Size(68, 20)
        Me.lblBackfillFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblBackfillFrom.Name = "lblBackfillFrom"

        Me.dtpBackfillFrom.CustomFormat = "dd/MM/yyyy"
        Me.dtpBackfillFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpBackfillFrom.Location = New System.Drawing.Point(78, 6)
        Me.dtpBackfillFrom.Size = New System.Drawing.Size(110, 23)
        Me.dtpBackfillFrom.Name = "dtpBackfillFrom"

        Me.lblBackfillTo.Text = "To:"
        Me.lblBackfillTo.Location = New System.Drawing.Point(196, 8)
        Me.lblBackfillTo.Size = New System.Drawing.Size(28, 20)
        Me.lblBackfillTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblBackfillTo.Name = "lblBackfillTo"

        Me.dtpBackfillTo.CustomFormat = "dd/MM/yyyy"
        Me.dtpBackfillTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpBackfillTo.Location = New System.Drawing.Point(228, 6)
        Me.dtpBackfillTo.Size = New System.Drawing.Size(110, 23)
        Me.dtpBackfillTo.Name = "dtpBackfillTo"

        Me.lblChunkDays.Text = "Chunk (days):"
        Me.lblChunkDays.Location = New System.Drawing.Point(346, 8)
        Me.lblChunkDays.Size = New System.Drawing.Size(88, 20)
        Me.lblChunkDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblChunkDays.Name = "lblChunkDays"

        Me.nudChunkDays.Location = New System.Drawing.Point(438, 6)
        Me.nudChunkDays.Minimum = 1
        Me.nudChunkDays.Maximum = 365
        Me.nudChunkDays.Value = 30
        Me.nudChunkDays.Size = New System.Drawing.Size(54, 23)
        Me.nudChunkDays.Name = "nudChunkDays"
        '
        ' lblBackfillNote
        '
        Me.lblBackfillNote.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblBackfillNote.ForeColor = System.Drawing.Color.DarkOrange
        Me.lblBackfillNote.Name = "lblBackfillNote"
        Me.lblBackfillNote.Text = ""
        Me.lblBackfillNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        ' pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnOK)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(480, 44)
        '
        ' btnOK
        '
        Me.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.btnOK.Location = New System.Drawing.Point(296, 8)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(80, 28)
        Me.btnOK.Text = "OK"
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.None
        '
        ' btnCancel
        '
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.btnCancel.Location = New System.Drawing.Point(384, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 28)
        Me.btnCancel.Text = "Cancel"
        '
        ' JobEditForm
        '
        Me.AcceptButton = Me.btnOK
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(480, 400)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.tlp)
        Me.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "JobEditForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Job Definition"
        Me.tlp.ResumeLayout(False)
        Me.tlp.PerformLayout()
        Me.pnlRadio.ResumeLayout(False)
        CType(Me.nudChunkDays, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlBackfill.ResumeLayout(False)
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlp As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents lblSource As System.Windows.Forms.Label
    Friend WithEvents cboSource As System.Windows.Forms.ComboBox
    Friend WithEvents lblEntity As System.Windows.Forms.Label
    Friend WithEvents cboEntity As System.Windows.Forms.ComboBox
    Friend WithEvents lblSchedule As System.Windows.Forms.Label
    Friend WithEvents pnlRadio As System.Windows.Forms.Panel
    Friend WithEvents rbOnce As System.Windows.Forms.RadioButton
    Friend WithEvents rbRecurring As System.Windows.Forms.RadioButton
    Friend WithEvents rbBackfill As System.Windows.Forms.RadioButton
    Friend WithEvents lblIntervalHint As System.Windows.Forms.Label
    Friend WithEvents lblInterval As System.Windows.Forms.Label
    Friend WithEvents cboInterval As System.Windows.Forms.ComboBox
    Friend WithEvents lblIntervalMin As System.Windows.Forms.Label
    Friend WithEvents lblNextRun As System.Windows.Forms.Label
    Friend WithEvents dtpNextRun As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblEnabled As System.Windows.Forms.Label
    Friend WithEvents chkEnabled As System.Windows.Forms.CheckBox
    Friend WithEvents pnlBackfill As System.Windows.Forms.Panel
    Friend WithEvents lblBackfillFrom As System.Windows.Forms.Label
    Friend WithEvents dtpBackfillFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblBackfillTo As System.Windows.Forms.Label
    Friend WithEvents dtpBackfillTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblChunkDays As System.Windows.Forms.Label
    Friend WithEvents nudChunkDays As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblBackfillNote As System.Windows.Forms.Label
    Friend WithEvents pnlButtons As System.Windows.Forms.Panel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
