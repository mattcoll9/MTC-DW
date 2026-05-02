<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SettingsPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.grpDB = New System.Windows.Forms.GroupBox()
        Me.btnImportConn = New System.Windows.Forms.Button()
        Me.btnExportConn = New System.Windows.Forms.Button()
        Me.btnSaveConn = New System.Windows.Forms.Button()
        Me.btnTestConn = New System.Windows.Forms.Button()
        Me.txtConnStr = New System.Windows.Forms.TextBox()
        Me.lblConnStr = New System.Windows.Forms.Label()
        Me.lblConfigHint = New System.Windows.Forms.Label()
        Me.grpDB.SuspendLayout()
        Me.SuspendLayout()
        '
        ' grpDB
        '
        Me.grpDB.Controls.Add(Me.btnImportConn)
        Me.grpDB.Controls.Add(Me.btnExportConn)
        Me.grpDB.Controls.Add(Me.btnSaveConn)
        Me.grpDB.Controls.Add(Me.btnTestConn)
        Me.grpDB.Controls.Add(Me.txtConnStr)
        Me.grpDB.Controls.Add(Me.lblConnStr)
        Me.grpDB.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpDB.Name = "grpDB"
        Me.grpDB.Size = New System.Drawing.Size(900, 112)
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
        ' btnExportConn
        '
        Me.btnExportConn.Location = New System.Drawing.Point(122, 62)
        Me.btnExportConn.Name = "btnExportConn"
        Me.btnExportConn.Size = New System.Drawing.Size(90, 26)
        Me.btnExportConn.Text = "Export…"
        '
        ' btnImportConn
        '
        Me.btnImportConn.Location = New System.Drawing.Point(218, 62)
        Me.btnImportConn.Name = "btnImportConn"
        Me.btnImportConn.Size = New System.Drawing.Size(90, 26)
        Me.btnImportConn.Text = "Import…"
        '
        ' lblConfigHint
        '
        Me.lblConfigHint.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblConfigHint.ForeColor = System.Drawing.Color.Gray
        Me.lblConfigHint.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic)
        Me.lblConfigHint.Name = "lblConfigHint"
        Me.lblConfigHint.Size = New System.Drawing.Size(900, 40)
        Me.lblConfigHint.Text = "Source-specific settings are configured via the Configure button on each source panel."
        Me.lblConfigHint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.lblConfigHint.Padding = New System.Windows.Forms.Padding(8, 0, 0, 0)
        '
        ' SettingsPanel
        '
        Me.Controls.Add(Me.lblConfigHint)
        Me.Controls.Add(Me.grpDB)
        Me.Name = "SettingsPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.grpDB.ResumeLayout(False)
        Me.grpDB.PerformLayout()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents grpDB As System.Windows.Forms.GroupBox
    Friend WithEvents lblConnStr As System.Windows.Forms.Label
    Friend WithEvents txtConnStr As System.Windows.Forms.TextBox
    Friend WithEvents btnTestConn As System.Windows.Forms.Button
    Friend WithEvents btnSaveConn As System.Windows.Forms.Button
    Friend WithEvents btnExportConn As System.Windows.Forms.Button
    Friend WithEvents btnImportConn As System.Windows.Forms.Button
    Friend WithEvents lblConfigHint As System.Windows.Forms.Label
End Class
