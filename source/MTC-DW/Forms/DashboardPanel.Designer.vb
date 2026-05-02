<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DashboardPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.grpTableStats = New System.Windows.Forms.GroupBox()
        Me.dgTableStats = New System.Windows.Forms.DataGridView()
        Me.grpHistory = New System.Windows.Forms.GroupBox()
        Me.dgHistory = New System.Windows.Forms.DataGridView()
        Me.grpTableStats.SuspendLayout()
        CType(Me.dgTableStats, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpHistory.SuspendLayout()
        CType(Me.dgHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' grpTableStats
        '
        Me.grpTableStats.Controls.Add(Me.dgTableStats)
        Me.grpTableStats.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpTableStats.Name = "grpTableStats"
        Me.grpTableStats.Size = New System.Drawing.Size(900, 240)
        Me.grpTableStats.Text = "Table Row Counts"
        Me.grpTableStats.Padding = New System.Windows.Forms.Padding(4)
        '
        ' dgTableStats
        '
        Me.dgTableStats.AllowUserToAddRows = False
        Me.dgTableStats.AllowUserToDeleteRows = False
        Me.dgTableStats.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgTableStats.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgTableStats.Name = "dgTableStats"
        Me.dgTableStats.ReadOnly = True
        Me.dgTableStats.RowHeadersVisible = False
        Me.dgTableStats.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgTableStats.TabIndex = 0
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
        Me.Controls.Add(Me.grpTableStats)
        Me.Name = "DashboardPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.grpTableStats.ResumeLayout(False)
        CType(Me.dgTableStats, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpHistory.ResumeLayout(False)
        CType(Me.dgHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents grpTableStats As System.Windows.Forms.GroupBox
    Friend WithEvents dgTableStats As System.Windows.Forms.DataGridView
    Friend WithEvents grpHistory As System.Windows.Forms.GroupBox
    Friend WithEvents dgHistory As System.Windows.Forms.DataGridView
End Class
