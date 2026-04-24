<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class JobsPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.splitJobs = New System.Windows.Forms.SplitContainer()
        Me.pnlJobsTop = New System.Windows.Forms.Panel()
        Me.dgJobs = New System.Windows.Forms.DataGridView()
        Me.tsJobs = New System.Windows.Forms.ToolStrip()
        Me.btnAdd = New System.Windows.Forms.ToolStripButton()
        Me.btnEdit = New System.Windows.Forms.ToolStripButton()
        Me.btnDelete = New System.Windows.Forms.ToolStripButton()
        Me.tsSep1 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnRunNow = New System.Windows.Forms.ToolStripButton()
        Me.tsSep2 = New System.Windows.Forms.ToolStripSeparator()
        Me.btnRefresh = New System.Windows.Forms.ToolStripButton()
        Me.pnlHistoryBot = New System.Windows.Forms.Panel()
        Me.dgHistory = New System.Windows.Forms.DataGridView()
        Me.lblHistTitle = New System.Windows.Forms.Label()
        CType(Me.splitJobs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitJobs.Panel1.SuspendLayout()
        Me.splitJobs.Panel2.SuspendLayout()
        Me.splitJobs.SuspendLayout()
        Me.pnlJobsTop.SuspendLayout()
        CType(Me.dgJobs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tsJobs.SuspendLayout()
        Me.pnlHistoryBot.SuspendLayout()
        CType(Me.dgHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' splitJobs
        '
        Me.splitJobs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitJobs.Name = "splitJobs"
        Me.splitJobs.Orientation = System.Windows.Forms.Orientation.Horizontal
        Me.splitJobs.Panel1.Controls.Add(Me.pnlJobsTop)
        Me.splitJobs.Panel2.Controls.Add(Me.pnlHistoryBot)
        Me.splitJobs.SplitterDistance = 280
        Me.splitJobs.TabIndex = 0
        '
        ' pnlJobsTop
        '
        Me.pnlJobsTop.Controls.Add(Me.dgJobs)
        Me.pnlJobsTop.Controls.Add(Me.tsJobs)
        Me.pnlJobsTop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlJobsTop.Name = "pnlJobsTop"
        '
        ' tsJobs
        '
        Me.tsJobs.Items.AddRange(New System.Windows.Forms.ToolStripItem() {
            Me.btnAdd, Me.btnEdit, Me.btnDelete, Me.tsSep1, Me.btnRunNow, Me.tsSep2, Me.btnRefresh})
        Me.tsJobs.Name = "tsJobs"
        Me.tsJobs.Dock = System.Windows.Forms.DockStyle.Top
        '
        ' btnAdd
        '
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Text = "Add Job"
        Me.btnAdd.Image = Nothing
        '
        ' btnEdit
        '
        Me.btnEdit.Name = "btnEdit"
        Me.btnEdit.Text = "Edit"
        '
        ' btnDelete
        '
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Text = "Delete"
        '
        ' tsSep1
        '
        Me.tsSep1.Name = "tsSep1"
        '
        ' btnRunNow
        '
        Me.btnRunNow.Name = "btnRunNow"
        Me.btnRunNow.Text = "Run Now"
        Me.btnRunNow.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        '
        ' tsSep2
        '
        Me.tsSep2.Name = "tsSep2"
        '
        ' btnRefresh
        '
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Text = "Refresh"
        '
        ' dgJobs
        '
        Me.dgJobs.AllowUserToAddRows = False
        Me.dgJobs.AllowUserToDeleteRows = False
        Me.dgJobs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgJobs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgJobs.Name = "dgJobs"
        Me.dgJobs.ReadOnly = True
        Me.dgJobs.RowHeadersVisible = False
        Me.dgJobs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgJobs.MultiSelect = False
        Me.dgJobs.TabIndex = 1
        '
        ' pnlHistoryBot
        '
        Me.pnlHistoryBot.Controls.Add(Me.dgHistory)
        Me.pnlHistoryBot.Controls.Add(Me.lblHistTitle)
        Me.pnlHistoryBot.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlHistoryBot.Name = "pnlHistoryBot"
        '
        ' lblHistTitle
        '
        Me.lblHistTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblHistTitle.Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
        Me.lblHistTitle.Name = "lblHistTitle"
        Me.lblHistTitle.Size = New System.Drawing.Size(200, 20)
        Me.lblHistTitle.Text = "Run History for Selected Job"
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
        Me.dgHistory.TabIndex = 1
        '
        ' JobsPanel
        '
        Me.Controls.Add(Me.splitJobs)
        Me.Name = "JobsPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.splitJobs.Panel1.ResumeLayout(False)
        Me.splitJobs.Panel2.ResumeLayout(False)
        CType(Me.splitJobs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitJobs.ResumeLayout(False)
        Me.pnlJobsTop.ResumeLayout(False)
        Me.pnlJobsTop.PerformLayout()
        CType(Me.dgJobs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tsJobs.ResumeLayout(False)
        Me.tsJobs.PerformLayout()
        Me.pnlHistoryBot.ResumeLayout(False)
        CType(Me.dgHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents splitJobs As System.Windows.Forms.SplitContainer
    Friend WithEvents pnlJobsTop As System.Windows.Forms.Panel
    Friend WithEvents tsJobs As System.Windows.Forms.ToolStrip
    Friend WithEvents btnAdd As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnEdit As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnDelete As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsSep1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnRunNow As System.Windows.Forms.ToolStripButton
    Friend WithEvents tsSep2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnRefresh As System.Windows.Forms.ToolStripButton
    Friend WithEvents dgJobs As System.Windows.Forms.DataGridView
    Friend WithEvents pnlHistoryBot As System.Windows.Forms.Panel
    Friend WithEvents lblHistTitle As System.Windows.Forms.Label
    Friend WithEvents dgHistory As System.Windows.Forms.DataGridView
End Class
