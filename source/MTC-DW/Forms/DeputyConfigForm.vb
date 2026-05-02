Imports MTCDW.Services

Public Class DeputyConfigForm

    Private Sub DeputyConfigForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If AppState.Db Is Nothing Then Return
        txtBaseUrl.Text = If(AppState.Db.GetConfigValue("Deputy.BaseUrl"), "")
        txtOAuthToken.Text = If(AppState.Db.GetConfigValue("Deputy.OAuthToken"), "")
        Dim days As Integer = 90
        Integer.TryParse(AppState.Db.GetConfigValue("Deputy.SyncDaysBack"), days)
        nudSyncDaysBack.Value = Math.Max(1, Math.Min(3650, days))
    End Sub

    Private Sub btnShowToken_Click(sender As Object, e As EventArgs) Handles btnShowToken.Click
        If txtOAuthToken.PasswordChar = Nothing Then
            txtOAuthToken.PasswordChar = "*"c
            btnShowToken.Text = "Show"
        Else
            txtOAuthToken.PasswordChar = Nothing
            btnShowToken.Text = "Hide"
        End If
    End Sub

    Private Async Sub btnTestDeputy_Click(sender As Object, e As EventArgs) Handles btnTestDeputy.Click
        Dim url = txtBaseUrl.Text.Trim()
        Dim token = txtOAuthToken.Text.Trim()
        If String.IsNullOrEmpty(url) OrElse String.IsNullOrEmpty(token) Then
            MessageBox.Show("Enter Base URL and OAuth Token before testing.", "Missing Values",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        btnTestDeputy.Enabled = False
        btnTestDeputy.Text = "Testing…"
        Try
            Dim api As New DeputyApiService(url, token)
            Dim items = Await api.GetAll("resource/OperationalUnit")
            MessageBox.Show($"Connected. {items.Count} operational unit(s) returned.",
                            "Test Passed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Test failed: {ex.Message}", "Test Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnTestDeputy.Enabled = True
            btnTestDeputy.Text = "Test Deputy API"
        End Try
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If AppState.Db Is Nothing Then
            MessageBox.Show("Not connected to database.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        AppState.Db.SetConfigValue("Deputy.BaseUrl", txtBaseUrl.Text.Trim())
        AppState.Db.SetConfigValue("Deputy.OAuthToken", txtOAuthToken.Text.Trim())
        AppState.Db.SetConfigValue("Deputy.SyncDaysBack", CInt(nudSyncDaysBack.Value).ToString())
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

End Class
