Imports MTCDW.Services

Public Class RevSportConfigForm

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub RevSportConfigForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If AppState.Db Is Nothing Then Return
        txtEmail.Text = If(AppState.Db.GetConfigValue("RevSport.Email"), "")
        txtPassword.Text = If(AppState.Db.GetConfigValue("RevSport.Password"), "")
        txtTotpSeed.Text = If(AppState.Db.GetConfigValue("RevSport.TotpSeed"), "")

        Dim days As Integer = 90
        Integer.TryParse(AppState.Db.GetConfigValue("RevSport.EventsDaysBack"), days)
        nudEventsDaysBack.Value = Math.Max(1, Math.Min(3650, days))
    End Sub

    Private Sub btnShowSecrets_Click(sender As Object, e As EventArgs) Handles btnShowSecrets.Click
        Dim hiding = txtPassword.PasswordChar <> Nothing
        txtPassword.PasswordChar = If(hiding, Nothing, "*"c)
        txtTotpSeed.PasswordChar = If(hiding, Nothing, "*"c)
        btnShowSecrets.Text = If(hiding, "Hide", "Show")
    End Sub

    Private Async Sub btnTestRevSport_Click(sender As Object, e As EventArgs) Handles btnTestRevSport.Click
        btnTestRevSport.Enabled = False
        btnTestRevSport.Text = "Testing…"
        Try
            Dim api As New RevSportApiService(txtEmail.Text.Trim(),
                                              txtPassword.Text.Trim(),
                                              txtTotpSeed.Text.Trim())
            Await api.LoginAsync()
            MessageBox.Show("RevSport login successful.", "Test Passed",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Test failed: {ex.Message}", "Test Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnTestRevSport.Enabled = True
            btnTestRevSport.Text = "Test RevSport Login"
        End Try
    End Sub

    Private Sub btnShowTotpCode_Click(sender As Object, e As EventArgs) Handles btnShowTotpCode.Click
        Dim seed = txtTotpSeed.Text.Trim()
        If String.IsNullOrEmpty(seed) Then
            MessageBox.Show("No TOTP seed entered.", "TOTP", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Try
            Dim code = RevSportApiService.GenerateTotpCode(seed)
            MessageBox.Show($"Current TOTP code: {code}" & vbCrLf & "Compare with your authenticator app. Codes refresh every 30 seconds.", "TOTP Code", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Failed to generate code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If AppState.Db Is Nothing Then
            MessageBox.Show("Not connected to database.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        AppState.Db.SetConfigValue("RevSport.Email", txtEmail.Text.Trim())
        AppState.Db.SetConfigValue("RevSport.Password", txtPassword.Text.Trim())
        AppState.Db.SetConfigValue("RevSport.TotpSeed", txtTotpSeed.Text.Trim())
        AppState.Db.SetConfigValue("RevSport.EventsDaysBack", CInt(nudEventsDaysBack.Value).ToString())
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

End Class
