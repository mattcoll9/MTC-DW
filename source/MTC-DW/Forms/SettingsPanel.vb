Imports System.Configuration

Public Class SettingsPanel

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub SettingsPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Pre-fill connection string from current app.config
        Dim cs = ConfigurationManager.ConnectionStrings("MTCDW")
        txtConnStr.Text = If(cs IsNot Nothing, cs.ConnectionString, "")
        LoadApiConfig()
    End Sub

    Private Sub LoadApiConfig()
        If AppState.Db Is Nothing Then Return
        Try
            Dim items = AppState.Db.GetAllConfig()
            dgConfig.DataSource = items.Select(Function(c) New With {
                .Key = c.ConfigKey,
                .Value = c.ConfigValue,
                .Updated = c.UpdatedAt.ToString("dd/MM/yyyy HH:mm")
            }).ToList()
        Catch
        End Try
    End Sub

    Private Sub btnTestConn_Click(sender As Object, e As EventArgs) Handles btnTestConn.Click
        Dim testDb As New Services.DatabaseService(txtConnStr.Text.Trim())
        If testDb.TestConnection() Then
            MessageBox.Show("Connection successful.", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Connection failed. Check server name, database, and permissions.", "Test",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnSaveConn_Click(sender As Object, e As EventArgs) Handles btnSaveConn.Click
        Try
            Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            config.ConnectionStrings.ConnectionStrings("MTCDW").ConnectionString = txtConnStr.Text.Trim()
            config.Save(ConfigurationSaveMode.Modified)
            ConfigurationManager.RefreshSection("connectionStrings")

            AppState.Db?.UpdateConnectionString(txtConnStr.Text.Trim())
            MessageBox.Show("Connection string saved. Restart the app if this is a new database.", "Saved",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Could not save: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveConfig_Click(sender As Object, e As EventArgs) Handles btnSaveConfig.Click
        If AppState.Db Is Nothing Then
            MessageBox.Show("Not connected to database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        ' Save any edits in txtNewKey / txtNewValue
        If Not String.IsNullOrWhiteSpace(txtNewKey.Text) Then
            AppState.Db.SetConfigValue(txtNewKey.Text.Trim(), txtNewValue.Text.Trim())
            txtNewKey.Text = ""
            txtNewValue.Text = ""
        End If
        ' Seed defaults if missing
        SeedDefaults()
        LoadApiConfig()
        MessageBox.Show("Settings saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub SeedDefaults()
        Dim defaults As New Dictionary(Of String, String) From {
            {"Deputy.BaseUrl", "https://YOURINSTANCE.au.deputy.com/api/v1/"},
            {"Deputy.OAuthToken", ""},
            {"Deputy.SyncDaysBack", "90"}
        }
        For Each kv In defaults
            If String.IsNullOrEmpty(AppState.Db.GetConfigValue(kv.Key)) Then
                AppState.Db.SetConfigValue(kv.Key, kv.Value)
            End If
        Next
    End Sub

    Private Sub btnDeleteConfig_Click(sender As Object, e As EventArgs) Handles btnDeleteConfig.Click
        ' Editing is done via txtNewKey/txtNewValue — this button is for future use
        MessageBox.Show("To change a value, type the key in 'Key' and the new value in 'Value', then Save.", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class
