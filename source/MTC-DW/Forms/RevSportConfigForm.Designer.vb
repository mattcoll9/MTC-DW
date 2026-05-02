<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RevSportConfigForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.tlp = New System.Windows.Forms.TableLayoutPanel()
        Me.lblEmail = New System.Windows.Forms.Label()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.lblTotpSeed = New System.Windows.Forms.Label()
        Me.txtTotpSeed = New System.Windows.Forms.TextBox()
        Me.lblSeason = New System.Windows.Forms.Label()
        Me.cboSeasonId = New System.Windows.Forms.ComboBox()
        Me.lblEventsDays = New System.Windows.Forms.Label()
        Me.nudEventsDaysBack = New System.Windows.Forms.NumericUpDown()
        Me.pnlTest = New System.Windows.Forms.Panel()
        Me.btnShowSecrets = New System.Windows.Forms.Button()
        Me.btnTestRevSport = New System.Windows.Forms.Button()
        Me.btnShowTotpCode = New System.Windows.Forms.Button()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tlp.SuspendLayout()
        CType(Me.nudEventsDaysBack, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlTest.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        ' tlp
        '
        Me.tlp.ColumnCount = 2
        Me.tlp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130.0!))
        Me.tlp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlp.RowCount = 4
        Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.tlp.Controls.Add(Me.lblEmail, 0, 0)
        Me.tlp.Controls.Add(Me.txtEmail, 1, 0)
        Me.tlp.Controls.Add(Me.lblPassword, 0, 1)
        Me.tlp.Controls.Add(Me.txtPassword, 1, 1)
        Me.tlp.Controls.Add(Me.lblTotpSeed, 0, 2)
        Me.tlp.Controls.Add(Me.txtTotpSeed, 1, 2)
        Me.tlp.Controls.Add(Me.lblEventsDays, 0, 3)
        Me.tlp.Controls.Add(Me.nudEventsDaysBack, 1, 3)
        Me.tlp.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlp.Name = "tlp"
        Me.tlp.Padding = New System.Windows.Forms.Padding(8)
        Me.tlp.Size = New System.Drawing.Size(480, 144)
        '
        ' Labels
        '
        Me.lblEmail.Text = "Username:"
        Me.lblEmail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblEmail.Name = "lblEmail"
        Me.lblPassword.Text = "Password:"
        Me.lblPassword.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblPassword.Name = "lblPassword"
        Me.lblTotpSeed.Text = "TOTP Seed:"
        Me.lblTotpSeed.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotpSeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblTotpSeed.Name = "lblTotpSeed"
        Me.lblEventsDays.Text = "Events Days Back:"
        Me.lblEventsDays.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblEventsDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblEventsDays.Name = "lblEventsDays"
        '
        ' txtEmail
        '
        Me.txtEmail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtEmail.Name = "txtEmail"
        '
        ' txtPassword
        '
        Me.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = "*"c
        '
        ' txtTotpSeed
        '
        Me.txtTotpSeed.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtTotpSeed.Name = "txtTotpSeed"
        Me.txtTotpSeed.PasswordChar = "*"c
        '
        ' nudEventsDaysBack
        '
        Me.nudEventsDaysBack.Minimum = 1
        Me.nudEventsDaysBack.Maximum = 3650
        Me.nudEventsDaysBack.Value = 90
        Me.nudEventsDaysBack.Name = "nudEventsDaysBack"
        Me.nudEventsDaysBack.Size = New System.Drawing.Size(80, 23)
        '
        ' pnlTest
        '
        Me.pnlTest.Controls.Add(Me.btnShowSecrets)
        Me.pnlTest.Controls.Add(Me.btnTestRevSport)
        Me.pnlTest.Controls.Add(Me.btnShowTotpCode)
        Me.pnlTest.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTest.Name = "pnlTest"
        Me.pnlTest.Size = New System.Drawing.Size(480, 38)
        Me.pnlTest.Padding = New System.Windows.Forms.Padding(8, 0, 8, 0)
        '
        ' btnShowSecrets
        '
        Me.btnShowSecrets.Location = New System.Drawing.Point(8, 6)
        Me.btnShowSecrets.Name = "btnShowSecrets"
        Me.btnShowSecrets.Size = New System.Drawing.Size(60, 26)
        Me.btnShowSecrets.Text = "Show"
        '
        ' btnTestRevSport
        '
        Me.btnTestRevSport.Location = New System.Drawing.Point(76, 6)
        Me.btnTestRevSport.Name = "btnTestRevSport"
        Me.btnTestRevSport.Size = New System.Drawing.Size(160, 26)
        Me.btnTestRevSport.Text = "Test RevSport Login"
        '
        ' btnShowTotpCode
        '
        Me.btnShowTotpCode.Location = New System.Drawing.Point(244, 6)
        Me.btnShowTotpCode.Name = "btnShowTotpCode"
        Me.btnShowTotpCode.Size = New System.Drawing.Size(100, 26)
        Me.btnShowTotpCode.Text = "Show TOTP Code"
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
        '
        ' btnCancel
        '
        Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.btnCancel.Location = New System.Drawing.Point(384, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 28)
        Me.btnCancel.Text = "Cancel"
        '
        ' RevSportConfigForm
        '
        Me.AcceptButton = Me.btnOK
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(480, 234)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.pnlTest)
        Me.Controls.Add(Me.tlp)
        Me.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "RevSportConfigForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Configure RevSport"
        Me.tlp.ResumeLayout(False)
        Me.tlp.PerformLayout()
        CType(Me.nudEventsDaysBack, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlTest.ResumeLayout(False)
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlp As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblEmail As System.Windows.Forms.Label
    Friend WithEvents txtEmail As System.Windows.Forms.TextBox
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblTotpSeed As System.Windows.Forms.Label
    Friend WithEvents txtTotpSeed As System.Windows.Forms.TextBox
    Friend WithEvents lblEventsDays As System.Windows.Forms.Label
    Friend WithEvents nudEventsDaysBack As System.Windows.Forms.NumericUpDown
    Friend WithEvents pnlTest As System.Windows.Forms.Panel
    Friend WithEvents btnShowSecrets As System.Windows.Forms.Button
    Friend WithEvents btnTestRevSport As System.Windows.Forms.Button
    Friend WithEvents btnShowTotpCode As System.Windows.Forms.Button
    Friend WithEvents pnlButtons As System.Windows.Forms.Panel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
