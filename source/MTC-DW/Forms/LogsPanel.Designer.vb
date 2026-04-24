<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LogsPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlFilter = New System.Windows.Forms.Panel()
        Me.lblCount = New System.Windows.Forms.Label()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.cboStatus = New System.Windows.Forms.ComboBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.dtpTo = New System.Windows.Forms.DateTimePicker()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.dgLogs = New System.Windows.Forms.DataGridView()
        Me.pnlFilter.SuspendLayout()
        CType(Me.dgLogs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' pnlFilter
        '
        Me.pnlFilter.Controls.Add(Me.lblCount)
        Me.pnlFilter.Controls.Add(Me.btnRefresh)
        Me.pnlFilter.Controls.Add(Me.cboStatus)
        Me.pnlFilter.Controls.Add(Me.lblStatus)
        Me.pnlFilter.Controls.Add(Me.dtpTo)
        Me.pnlFilter.Controls.Add(Me.lblTo)
        Me.pnlFilter.Controls.Add(Me.dtpFrom)
        Me.pnlFilter.Controls.Add(Me.lblFrom)
        Me.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFilter.Name = "pnlFilter"
        Me.pnlFilter.Size = New System.Drawing.Size(900, 40)
        '
        ' lblFrom
        '
        Me.lblFrom.Location = New System.Drawing.Point(4, 10)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(44, 20)
        Me.lblFrom.Text = "From:"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' dtpFrom
        '
        Me.dtpFrom.CustomFormat = "dd/MM/yyyy"
        Me.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpFrom.Location = New System.Drawing.Point(52, 8)
        Me.dtpFrom.Name = "dtpFrom"
        Me.dtpFrom.Size = New System.Drawing.Size(110, 23)
        '
        ' lblTo
        '
        Me.lblTo.Location = New System.Drawing.Point(168, 10)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(28, 20)
        Me.lblTo.Text = "To:"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' dtpTo
        '
        Me.dtpTo.CustomFormat = "dd/MM/yyyy"
        Me.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpTo.Location = New System.Drawing.Point(200, 8)
        Me.dtpTo.Name = "dtpTo"
        Me.dtpTo.Size = New System.Drawing.Size(110, 23)
        '
        ' lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(318, 10)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(48, 20)
        Me.lblStatus.Text = "Status:"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' cboStatus
        '
        Me.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStatus.Location = New System.Drawing.Point(370, 8)
        Me.cboStatus.Name = "cboStatus"
        Me.cboStatus.Size = New System.Drawing.Size(100, 23)
        '
        ' btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(480, 7)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(70, 26)
        Me.btnRefresh.Text = "Refresh"
        '
        ' lblCount
        '
        Me.lblCount.Location = New System.Drawing.Point(560, 10)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Size = New System.Drawing.Size(160, 20)
        Me.lblCount.ForeColor = System.Drawing.Color.Gray
        '
        ' dgLogs
        '
        Me.dgLogs.AllowUserToAddRows = False
        Me.dgLogs.AllowUserToDeleteRows = False
        Me.dgLogs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgLogs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgLogs.Name = "dgLogs"
        Me.dgLogs.ReadOnly = True
        Me.dgLogs.RowHeadersVisible = False
        Me.dgLogs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgLogs.TabIndex = 1
        '
        ' LogsPanel
        '
        Me.Controls.Add(Me.dgLogs)
        Me.Controls.Add(Me.pnlFilter)
        Me.Name = "LogsPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.pnlFilter.ResumeLayout(False)
        CType(Me.dgLogs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlFilter As System.Windows.Forms.Panel
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents dtpFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents dtpTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents cboStatus As System.Windows.Forms.ComboBox
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents lblCount As System.Windows.Forms.Label
    Friend WithEvents dgLogs As System.Windows.Forms.DataGridView
End Class
