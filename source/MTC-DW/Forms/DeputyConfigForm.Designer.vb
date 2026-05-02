<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DeputyConfigForm
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.tlp = New System.Windows.Forms.TableLayoutPanel()
        Me.lblBaseUrl = New System.Windows.Forms.Label()
        Me.txtBaseUrl = New System.Windows.Forms.TextBox()
        Me.lblToken = New System.Windows.Forms.Label()
        Me.txtOAuthToken = New System.Windows.Forms.TextBox()
        Me.lblSyncDays = New System.Windows.Forms.Label()
        Me.nudSyncDaysBack = New System.Windows.Forms.NumericUpDown()
        Me.pnlTest = New System.Windows.Forms.Panel()
        Me.btnShowToken = New System.Windows.Forms.Button()
        Me.btnTestDeputy = New System.Windows.Forms.Button()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.tlp.SuspendLayout()
        CType(Me.nudSyncDaysBack, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlTest.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        ' tlp
        '
        Me.tlp.ColumnCount = 2
        Me.tlp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130.0!))
        Me.tlp.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlp.RowCount = 3
        Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.tlp.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34.0!))
        Me.tlp.Controls.Add(Me.lblBaseUrl, 0, 0)
        Me.tlp.Controls.Add(Me.txtBaseUrl, 1, 0)
        Me.tlp.Controls.Add(Me.lblToken, 0, 1)
        Me.tlp.Controls.Add(Me.txtOAuthToken, 1, 1)
        Me.tlp.Controls.Add(Me.lblSyncDays, 0, 2)
        Me.tlp.Controls.Add(Me.nudSyncDaysBack, 1, 2)
        Me.tlp.Dock = System.Windows.Forms.DockStyle.Top
        Me.tlp.Name = "tlp"
        Me.tlp.Padding = New System.Windows.Forms.Padding(8)
        Me.tlp.Size = New System.Drawing.Size(480, 110)
        '
        ' Labels
        '
        Me.lblBaseUrl.Text = "Base URL:"
        Me.lblBaseUrl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblBaseUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblBaseUrl.Name = "lblBaseUrl"
        Me.lblToken.Text = "OAuth Token:"
        Me.lblToken.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblToken.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblToken.Name = "lblToken"
        Me.lblSyncDays.Text = "Sync Days Back:"
        Me.lblSyncDays.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblSyncDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.lblSyncDays.Name = "lblSyncDays"
        '
        ' txtBaseUrl
        '
        Me.txtBaseUrl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtBaseUrl.Name = "txtBaseUrl"
        '
        ' txtOAuthToken
        '
        Me.txtOAuthToken.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtOAuthToken.Name = "txtOAuthToken"
        Me.txtOAuthToken.PasswordChar = "*"c
        '
        ' nudSyncDaysBack
        '
        Me.nudSyncDaysBack.Minimum = 1
        Me.nudSyncDaysBack.Maximum = 3650
        Me.nudSyncDaysBack.Value = 90
        Me.nudSyncDaysBack.Name = "nudSyncDaysBack"
        Me.nudSyncDaysBack.Size = New System.Drawing.Size(80, 23)
        '
        ' pnlTest
        '
        Me.pnlTest.Controls.Add(Me.btnShowToken)
        Me.pnlTest.Controls.Add(Me.btnTestDeputy)
        Me.pnlTest.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTest.Name = "pnlTest"
        Me.pnlTest.Size = New System.Drawing.Size(480, 38)
        Me.pnlTest.Padding = New System.Windows.Forms.Padding(8, 0, 8, 0)
        '
        ' btnShowToken
        '
        Me.btnShowToken.Location = New System.Drawing.Point(8, 6)
        Me.btnShowToken.Name = "btnShowToken"
        Me.btnShowToken.Size = New System.Drawing.Size(60, 26)
        Me.btnShowToken.Text = "Show"
        '
        ' btnTestDeputy
        '
        Me.btnTestDeputy.Location = New System.Drawing.Point(76, 6)
        Me.btnTestDeputy.Name = "btnTestDeputy"
        Me.btnTestDeputy.Size = New System.Drawing.Size(150, 26)
        Me.btnTestDeputy.Text = "Test Deputy API"
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
        ' DeputyConfigForm
        '
        Me.AcceptButton = Me.btnOK
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(480, 200)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.pnlTest)
        Me.Controls.Add(Me.tlp)
        Me.Font = New System.Drawing.Font("Segoe UI", 9)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DeputyConfigForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Configure Deputy"
        Me.tlp.ResumeLayout(False)
        Me.tlp.PerformLayout()
        CType(Me.nudSyncDaysBack, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlTest.ResumeLayout(False)
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents tlp As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblBaseUrl As System.Windows.Forms.Label
    Friend WithEvents txtBaseUrl As System.Windows.Forms.TextBox
    Friend WithEvents lblToken As System.Windows.Forms.Label
    Friend WithEvents txtOAuthToken As System.Windows.Forms.TextBox
    Friend WithEvents lblSyncDays As System.Windows.Forms.Label
    Friend WithEvents nudSyncDaysBack As System.Windows.Forms.NumericUpDown
    Friend WithEvents pnlTest As System.Windows.Forms.Panel
    Friend WithEvents btnShowToken As System.Windows.Forms.Button
    Friend WithEvents btnTestDeputy As System.Windows.Forms.Button
    Friend WithEvents pnlButtons As System.Windows.Forms.Panel
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
