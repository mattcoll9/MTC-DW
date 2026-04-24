<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.splitMain = New System.Windows.Forms.SplitContainer()
        Me.pnlNav = New System.Windows.Forms.Panel()
        Me.tvNav = New System.Windows.Forms.TreeView()
        Me.lblNav = New System.Windows.Forms.Label()
        Me.pnlRight = New System.Windows.Forms.Panel()
        Me.pnlToolbar = New System.Windows.Forms.Panel()
        Me.btnSettings = New System.Windows.Forms.Button()
        Me.btnLogs = New System.Windows.Forms.Button()
        Me.btnDeputy = New System.Windows.Forms.Button()
        Me.btnJobs = New System.Windows.Forms.Button()
        Me.btnDashboard = New System.Windows.Forms.Button()
        Me.lblCurrentPanel = New System.Windows.Forms.Label()
        Me.pnlContent = New System.Windows.Forms.Panel()
        Me.statusStrip = New System.Windows.Forms.StatusStrip()
        Me.lblScheduler = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblLastRun = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitMain.Panel1.SuspendLayout()
        Me.splitMain.Panel2.SuspendLayout()
        Me.splitMain.SuspendLayout()
        Me.pnlNav.SuspendLayout()
        Me.pnlRight.SuspendLayout()
        Me.pnlToolbar.SuspendLayout()
        Me.statusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        ' splitMain
        '
        Me.splitMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitMain.Location = New System.Drawing.Point(0, 0)
        Me.splitMain.Name = "splitMain"
        Me.splitMain.Panel1.Controls.Add(Me.pnlNav)
        Me.splitMain.Panel2.Controls.Add(Me.pnlRight)
        Me.splitMain.Size = New System.Drawing.Size(1100, 680)
        Me.splitMain.SplitterDistance = 190
        Me.splitMain.TabIndex = 0
        '
        ' pnlNav
        '
        Me.pnlNav.BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
        Me.pnlNav.Controls.Add(Me.tvNav)
        Me.pnlNav.Controls.Add(Me.lblNav)
        Me.pnlNav.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNav.Name = "pnlNav"
        '
        ' lblNav
        '
        Me.lblNav.BackColor = System.Drawing.Color.FromArgb(0, 122, 204)
        Me.lblNav.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblNav.Font = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
        Me.lblNav.ForeColor = System.Drawing.Color.White
        Me.lblNav.Name = "lblNav"
        Me.lblNav.Padding = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.lblNav.Size = New System.Drawing.Size(190, 36)
        Me.lblNav.Text = "MTC Data Warehouse"
        Me.lblNav.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        ' tvNav
        '
        Me.tvNav.BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
        Me.tvNav.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.tvNav.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvNav.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.tvNav.ForeColor = System.Drawing.Color.WhiteSmoke
        Me.tvNav.ItemHeight = 24
        Me.tvNav.Name = "tvNav"
        Me.tvNav.TabIndex = 1
        '
        ' pnlRight
        '
        Me.pnlRight.Controls.Add(Me.pnlContent)
        Me.pnlRight.Controls.Add(Me.pnlToolbar)
        Me.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlRight.Name = "pnlRight"
        '
        ' pnlToolbar
        '
        Me.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(240, 240, 240)
        Me.pnlToolbar.Controls.Add(Me.lblCurrentPanel)
        Me.pnlToolbar.Controls.Add(Me.btnSettings)
        Me.pnlToolbar.Controls.Add(Me.btnLogs)
        Me.pnlToolbar.Controls.Add(Me.btnDeputy)
        Me.pnlToolbar.Controls.Add(Me.btnJobs)
        Me.pnlToolbar.Controls.Add(Me.btnDashboard)
        Me.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlToolbar.Name = "pnlToolbar"
        Me.pnlToolbar.Size = New System.Drawing.Size(906, 40)
        '
        ' btnDashboard
        '
        Me.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDashboard.Location = New System.Drawing.Point(4, 6)
        Me.btnDashboard.Name = "btnDashboard"
        Me.btnDashboard.Size = New System.Drawing.Size(90, 28)
        Me.btnDashboard.Text = "Dashboard"
        '
        ' btnJobs
        '
        Me.btnJobs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnJobs.Location = New System.Drawing.Point(98, 6)
        Me.btnJobs.Name = "btnJobs"
        Me.btnJobs.Size = New System.Drawing.Size(70, 28)
        Me.btnJobs.Text = "Jobs"
        '
        ' btnDeputy
        '
        Me.btnDeputy.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDeputy.Location = New System.Drawing.Point(172, 6)
        Me.btnDeputy.Name = "btnDeputy"
        Me.btnDeputy.Size = New System.Drawing.Size(70, 28)
        Me.btnDeputy.Text = "Deputy"
        '
        ' btnLogs
        '
        Me.btnLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLogs.Location = New System.Drawing.Point(246, 6)
        Me.btnLogs.Name = "btnLogs"
        Me.btnLogs.Size = New System.Drawing.Size(60, 28)
        Me.btnLogs.Text = "Logs"
        '
        ' btnSettings
        '
        Me.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSettings.Location = New System.Drawing.Point(310, 6)
        Me.btnSettings.Name = "btnSettings"
        Me.btnSettings.Size = New System.Drawing.Size(70, 28)
        Me.btnSettings.Text = "Settings"
        '
        ' lblCurrentPanel
        '
        Me.lblCurrentPanel.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.lblCurrentPanel.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        Me.lblCurrentPanel.ForeColor = System.Drawing.Color.FromArgb(0, 122, 204)
        Me.lblCurrentPanel.Location = New System.Drawing.Point(700, 10)
        Me.lblCurrentPanel.Name = "lblCurrentPanel"
        Me.lblCurrentPanel.Size = New System.Drawing.Size(200, 20)
        Me.lblCurrentPanel.Text = ""
        Me.lblCurrentPanel.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' pnlContent
        '
        Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Padding = New System.Windows.Forms.Padding(6)
        '
        ' statusStrip
        '
        Me.statusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblScheduler, Me.lblLastRun})
        Me.statusStrip.Name = "statusStrip"
        Me.statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom
        '
        ' lblScheduler
        '
        Me.lblScheduler.Name = "lblScheduler"
        Me.lblScheduler.Text = "Scheduler: —"
        Me.lblScheduler.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        '
        ' lblLastRun
        '
        Me.lblLastRun.Name = "lblLastRun"
        Me.lblLastRun.Text = "Ready"
        Me.lblLastRun.Spring = True
        Me.lblLastRun.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        ' MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1100, 700)
        Me.Controls.Add(Me.splitMain)
        Me.Controls.Add(Me.statusStrip)
        Me.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.MinimumSize = New System.Drawing.Size(900, 600)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MTC Data Warehouse"
        Me.splitMain.Panel1.ResumeLayout(False)
        Me.splitMain.Panel2.ResumeLayout(False)
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitMain.ResumeLayout(False)
        Me.pnlNav.ResumeLayout(False)
        Me.pnlRight.ResumeLayout(False)
        Me.pnlToolbar.ResumeLayout(False)
        Me.statusStrip.ResumeLayout(False)
        Me.statusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents splitMain As System.Windows.Forms.SplitContainer
    Friend WithEvents pnlNav As System.Windows.Forms.Panel
    Friend WithEvents lblNav As System.Windows.Forms.Label
    Friend WithEvents tvNav As System.Windows.Forms.TreeView
    Friend WithEvents pnlRight As System.Windows.Forms.Panel
    Friend WithEvents pnlToolbar As System.Windows.Forms.Panel
    Friend WithEvents btnDashboard As System.Windows.Forms.Button
    Friend WithEvents btnJobs As System.Windows.Forms.Button
    Friend WithEvents btnDeputy As System.Windows.Forms.Button
    Friend WithEvents btnLogs As System.Windows.Forms.Button
    Friend WithEvents btnSettings As System.Windows.Forms.Button
    Friend WithEvents lblCurrentPanel As System.Windows.Forms.Label
    Friend WithEvents pnlContent As System.Windows.Forms.Panel
    Friend WithEvents statusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents lblScheduler As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lblLastRun As System.Windows.Forms.ToolStripStatusLabel
End Class
