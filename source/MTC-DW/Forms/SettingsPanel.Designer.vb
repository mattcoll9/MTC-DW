<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SettingsPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.grpDB = New System.Windows.Forms.GroupBox()
        Me.btnSaveConn = New System.Windows.Forms.Button()
        Me.btnTestConn = New System.Windows.Forms.Button()
        Me.txtConnStr = New System.Windows.Forms.TextBox()
        Me.lblConnStr = New System.Windows.Forms.Label()
        Me.grpAPI = New System.Windows.Forms.GroupBox()
        Me.btnSaveConfig = New System.Windows.Forms.Button()
        Me.btnDeleteConfig = New System.Windows.Forms.Button()
        Me.dgConfig = New System.Windows.Forms.DataGridView()
        Me.pnlNewEntry = New System.Windows.Forms.Panel()
        Me.txtNewValue = New System.Windows.Forms.TextBox()
        Me.txtNewKey = New System.Windows.Forms.TextBox()
        Me.lblNewValue = New System.Windows.Forms.Label()
        Me.lblNewKey = New System.Windows.Forms.Label()
        Me.grpDB.SuspendLayout()
        Me.grpAPI.SuspendLayout()
        CType(Me.dgConfig, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlNewEntry.SuspendLayout()
        Me.SuspendLayout()
        '
        ' grpDB
        '
        Me.grpDB.Controls.Add(Me.btnSaveConn)
        Me.grpDB.Controls.Add(Me.btnTestConn)
        Me.grpDB.Controls.Add(Me.txtConnStr)
        Me.grpDB.Controls.Add(Me.lblConnStr)
        Me.grpDB.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpDB.Name = "grpDB"
        Me.grpDB.Size = New System.Drawing.Size(900, 80)
        Me.grpDB.Text = "SQL Server Connection"
        Me.grpDB.Padding = New System.Windows.Forms.Padding(8)
        '
        ' lblConnStr
        '
        Me.lblConnStr.Location = New System.Drawing.Point(8, 30)
        Me.lblConnStr.Name = "lblConnStr"
        Me.lblConnStr.Size = New System.Drawing.Size(110, 23)
        Me.lblConnStr.Text = "Connection String:"
        Me.lblConnStr.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' txtConnStr
        '
        Me.txtConnStr.Anchor = CType(System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
        Me.txtConnStr.Location = New System.Drawing.Point(122, 30)
        Me.txtConnStr.Name = "txtConnStr"
        Me.txtConnStr.Size = New System.Drawing.Size(560, 23)
        '
        ' btnTestConn
        '
        Me.btnTestConn.Location = New System.Drawing.Point(690, 29)
        Me.btnTestConn.Name = "btnTestConn"
        Me.btnTestConn.Size = New System.Drawing.Size(70, 26)
        Me.btnTestConn.Text = "Test"
        '
        ' btnSaveConn
        '
        Me.btnSaveConn.Location = New System.Drawing.Point(766, 29)
        Me.btnSaveConn.Name = "btnSaveConn"
        Me.btnSaveConn.Size = New System.Drawing.Size(70, 26)
        Me.btnSaveConn.Text = "Save"
        '
        ' grpAPI
        '
        Me.grpAPI.Controls.Add(Me.dgConfig)
        Me.grpAPI.Controls.Add(Me.pnlNewEntry)
        Me.grpAPI.Controls.Add(Me.btnDeleteConfig)
        Me.grpAPI.Controls.Add(Me.btnSaveConfig)
        Me.grpAPI.Dock = System.Windows.Forms.DockStyle.Fill
        Me.grpAPI.Name = "grpAPI"
        Me.grpAPI.Text = "API Configuration (stored in database)"
        Me.grpAPI.Padding = New System.Windows.Forms.Padding(8)
        '
        ' dgConfig
        '
        Me.dgConfig.AllowUserToAddRows = False
        Me.dgConfig.AllowUserToDeleteRows = False
        Me.dgConfig.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgConfig.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgConfig.Name = "dgConfig"
        Me.dgConfig.ReadOnly = True
        Me.dgConfig.RowHeadersVisible = False
        Me.dgConfig.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' pnlNewEntry
        '
        Me.pnlNewEntry.Controls.Add(Me.txtNewValue)
        Me.pnlNewEntry.Controls.Add(Me.txtNewKey)
        Me.pnlNewEntry.Controls.Add(Me.lblNewValue)
        Me.pnlNewEntry.Controls.Add(Me.lblNewKey)
        Me.pnlNewEntry.Controls.Add(Me.btnSaveConfig)
        Me.pnlNewEntry.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlNewEntry.Name = "pnlNewEntry"
        Me.pnlNewEntry.Size = New System.Drawing.Size(900, 44)
        '
        ' lblNewKey
        '
        Me.lblNewKey.Location = New System.Drawing.Point(4, 12)
        Me.lblNewKey.Name = "lblNewKey"
        Me.lblNewKey.Size = New System.Drawing.Size(38, 20)
        Me.lblNewKey.Text = "Key:"
        Me.lblNewKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' txtNewKey
        '
        Me.txtNewKey.Location = New System.Drawing.Point(46, 10)
        Me.txtNewKey.Name = "txtNewKey"
        Me.txtNewKey.Size = New System.Drawing.Size(200, 23)
        '
        ' lblNewValue
        '
        Me.lblNewValue.Location = New System.Drawing.Point(252, 12)
        Me.lblNewValue.Name = "lblNewValue"
        Me.lblNewValue.Size = New System.Drawing.Size(44, 20)
        Me.lblNewValue.Text = "Value:"
        Me.lblNewValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' txtNewValue
        '
        Me.txtNewValue.Location = New System.Drawing.Point(300, 10)
        Me.txtNewValue.Name = "txtNewValue"
        Me.txtNewValue.Size = New System.Drawing.Size(400, 23)
        '
        ' btnSaveConfig
        '
        Me.btnSaveConfig.Location = New System.Drawing.Point(708, 9)
        Me.btnSaveConfig.Name = "btnSaveConfig"
        Me.btnSaveConfig.Size = New System.Drawing.Size(80, 26)
        Me.btnSaveConfig.Text = "Save"
        '
        ' btnDeleteConfig
        '
        Me.btnDeleteConfig.Location = New System.Drawing.Point(794, 9)
        Me.btnDeleteConfig.Name = "btnDeleteConfig"
        Me.btnDeleteConfig.Size = New System.Drawing.Size(80, 26)
        Me.btnDeleteConfig.Text = "Help"
        '
        ' SettingsPanel
        '
        Me.Controls.Add(Me.grpAPI)
        Me.Controls.Add(Me.grpDB)
        Me.Name = "SettingsPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.grpDB.ResumeLayout(False)
        Me.grpDB.PerformLayout()
        Me.grpAPI.ResumeLayout(False)
        CType(Me.dgConfig, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlNewEntry.ResumeLayout(False)
        Me.pnlNewEntry.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents grpDB As System.Windows.Forms.GroupBox
    Friend WithEvents lblConnStr As System.Windows.Forms.Label
    Friend WithEvents txtConnStr As System.Windows.Forms.TextBox
    Friend WithEvents btnTestConn As System.Windows.Forms.Button
    Friend WithEvents btnSaveConn As System.Windows.Forms.Button
    Friend WithEvents grpAPI As System.Windows.Forms.GroupBox
    Friend WithEvents dgConfig As System.Windows.Forms.DataGridView
    Friend WithEvents pnlNewEntry As System.Windows.Forms.Panel
    Friend WithEvents lblNewKey As System.Windows.Forms.Label
    Friend WithEvents txtNewKey As System.Windows.Forms.TextBox
    Friend WithEvents lblNewValue As System.Windows.Forms.Label
    Friend WithEvents txtNewValue As System.Windows.Forms.TextBox
    Friend WithEvents btnSaveConfig As System.Windows.Forms.Button
    Friend WithEvents btnDeleteConfig As System.Windows.Forms.Button
End Class
