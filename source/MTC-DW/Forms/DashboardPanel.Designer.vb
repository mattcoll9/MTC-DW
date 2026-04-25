<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DashboardPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.tlpStats = New System.Windows.Forms.TableLayoutPanel()
        Me.grpTimesheets = New System.Windows.Forms.GroupBox()
        Me.lblTimesheets = New System.Windows.Forms.Label()
        Me.grpEmployees = New System.Windows.Forms.GroupBox()
        Me.lblEmployees = New System.Windows.Forms.Label()
        Me.grpOpUnits = New System.Windows.Forms.GroupBox()
        Me.lblOpUnits = New System.Windows.Forms.Label()
        Me.grpWorkTypes = New System.Windows.Forms.GroupBox()
        Me.lblWorkTypes = New System.Windows.Forms.Label()
        Me.grpHistory = New System.Windows.Forms.GroupBox()
        Me.dgHistory = New System.Windows.Forms.DataGridView()
        Me.tlpStats.SuspendLayout()
        Me.grpTimesheets.SuspendLayout()
        Me.grpEmployees.SuspendLayout()
        Me.grpOpUnits.SuspendLayout()
        Me.grpWorkTypes.SuspendLayout()
        Me.grpHistory.SuspendLayout()
        CType(Me.dgHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' tlpStats
        '
        Me.tlpStats.ColumnCount = 4
        Me.tlpStats.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tlpStats.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tlpStats.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tlpStats.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tlpStats.Controls.Add(Me.grpTimesheets, 0, 0)
        Me.tlpStats.Controls.Add(Me.grpEmployees, 1, 0)
        Me.tlpStats.Controls.Add(Me.grpOpUnits, 2, 0)
        Me.tlpStats.Controls.Add(Me.grpWorkTypes, 3, 0)
        Me.tlpStats.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlpStats.Name = "tlpStats"
        Me.tlpStats.RowCount = 1
        Me.tlpStats.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90.0!))
        Me.tlpStats.Size = New System.Drawing.Size(900, 90)
        Me.tlpStats.TabIndex = 0
        '
        ' grpTimesheets
        '
        Me.grpTimesheets.Controls.Add(Me.lblTimesheets)
        Me.grpTimesheets.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpTimesheets.Name = "grpTimesheets"
        Me.grpTimesheets.Text = "Timesheets"
        Me.grpTimesheets.Padding = New System.Windows.Forms.Padding(4)
        '
        ' lblTimesheets
        '
        Me.lblTimesheets.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTimesheets.Font = New System.Drawing.Font("Segoe UI", 22, System.Drawing.FontStyle.Bold)
        Me.lblTimesheets.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204)
        Me.lblTimesheets.Name = "lblTimesheets"
        Me.lblTimesheets.Text = "—"
        Me.lblTimesheets.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        ' grpEmployees
        '
        Me.grpEmployees.Controls.Add(Me.lblEmployees)
        Me.grpEmployees.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpEmployees.Name = "grpEmployees"
        Me.grpEmployees.Text = "Employees"
        '
        ' lblEmployees
        '
        Me.lblEmployees.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblEmployees.Font = New System.Drawing.Font("Segoe UI", 22, System.Drawing.FontStyle.Bold)
        Me.lblEmployees.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204)
        Me.lblEmployees.Name = "lblEmployees"
        Me.lblEmployees.Text = "—"
        Me.lblEmployees.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        ' grpOpUnits
        '
        Me.grpOpUnits.Controls.Add(Me.lblOpUnits)
        Me.grpOpUnits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpOpUnits.Name = "grpOpUnits"
        Me.grpOpUnits.Text = "Operational Units"
        '
        ' lblOpUnits
        '
        Me.lblOpUnits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblOpUnits.Font = New System.Drawing.Font("Segoe UI", 22, System.Drawing.FontStyle.Bold)
        Me.lblOpUnits.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204)
        Me.lblOpUnits.Name = "lblOpUnits"
        Me.lblOpUnits.Text = "—"
        Me.lblOpUnits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        ' grpWorkTypes
        '
        Me.grpWorkTypes.Controls.Add(Me.lblWorkTypes)
        Me.grpWorkTypes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpWorkTypes.Name = "grpWorkTypes"
        Me.grpWorkTypes.Text = "Rosters"
        '
        ' lblWorkTypes
        '
        Me.lblWorkTypes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblWorkTypes.Font = New System.Drawing.Font("Segoe UI", 22, System.Drawing.FontStyle.Bold)
        Me.lblWorkTypes.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204)
        Me.lblWorkTypes.Name = "lblWorkTypes"
        Me.lblWorkTypes.Text = "—"
        Me.lblWorkTypes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        ' grpHistory
        '
        Me.grpHistory.Controls.Add(Me.dgHistory)
        Me.grpHistory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpHistory.Name = "grpHistory"
        Me.grpHistory.Text = "Recent Job Runs"
        Me.grpHistory.Padding = New System.Windows.Forms.Padding(4)
        '
        ' dgHistory
        '
        Me.dgHistory.AllowUserToAddRows = False
        Me.dgHistory.AllowUserToDeleteRows = False
        Me.dgHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgHistory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgHistory.Name = "dgHistory"
        Me.dgHistory.ReadOnly = True
        Me.dgHistory.RowHeadersVisible = False
        Me.dgHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgHistory.TabIndex = 0
        '
        ' DashboardPanel
        '
        Me.Controls.Add(Me.grpHistory)
        Me.Controls.Add(Me.tlpStats)
        Me.Name = "DashboardPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.tlpStats.ResumeLayout(False)
        Me.grpTimesheets.ResumeLayout(False)
        Me.grpEmployees.ResumeLayout(False)
        Me.grpOpUnits.ResumeLayout(False)
        Me.grpWorkTypes.ResumeLayout(False)
        Me.grpHistory.ResumeLayout(False)
        CType(Me.dgHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlpStats As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents grpTimesheets As System.Windows.Forms.GroupBox
    Friend WithEvents lblTimesheets As System.Windows.Forms.Label
    Friend WithEvents grpEmployees As System.Windows.Forms.GroupBox
    Friend WithEvents lblEmployees As System.Windows.Forms.Label
    Friend WithEvents grpOpUnits As System.Windows.Forms.GroupBox
    Friend WithEvents lblOpUnits As System.Windows.Forms.Label
    Friend WithEvents grpWorkTypes As System.Windows.Forms.GroupBox
    Friend WithEvents lblWorkTypes As System.Windows.Forms.Label
    Friend WithEvents grpHistory As System.Windows.Forms.GroupBox
    Friend WithEvents dgHistory As System.Windows.Forms.DataGridView
End Class
