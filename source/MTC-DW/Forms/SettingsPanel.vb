Imports System.Configuration

Public Class SettingsPanel

    Private _btnTestApi As Button

    Public Sub New()
        InitializeComponent()

        _btnTestApi = New Button() With {
            .Name = "btnTestApi",
            .Text = "Test Deputy API",
            .Size = New Drawing.Size(120, 26),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right
        }
        AddHandler _btnTestApi.Click, AddressOf btnTestApi_Click

        Dim pnlApiTest As New Panel() With {
            .Dock = DockStyle.Top,
            .Height = 36,
            .Padding = New Padding(4)
        }
        pnlApiTest.Controls.Add(_btnTestApi)
        _btnTestApi.Location = New Drawing.Point(pnlApiTest.Width - 128, 5)
        _btnTestApi.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        grpAPI.Controls.Add(pnlApiTest)
        grpAPI.Controls.SetChildIndex(pnlApiTest, 0)
    End Sub

    Private Sub SettingsPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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
            GridColumnStore.Restore("SettingsConfig", dgConfig)
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

    Private Async Sub btnTestApi_Click(sender As Object, e As EventArgs)
        If AppState.Db Is Nothing Then
            MessageBox.Show("Not connected to database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim baseUrl = AppState.Db.GetConfigValue("Deputy.BaseUrl")
        Dim token = AppState.Db.GetConfigValue("Deputy.OAuthToken")
        If String.IsNullOrEmpty(baseUrl) OrElse String.IsNullOrEmpty(token) Then
            MessageBox.Show("Deputy.BaseUrl and Deputy.OAuthToken must be set in config before testing.", "Missing Config",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        _btnTestApi.Enabled = False
        _btnTestApi.Text = "Testing…"
        Try
            Dim api As New Services.DeputyApiService(baseUrl, token)
            Dim items = Await api.GetAll("resource/OperationalUnit")
            MessageBox.Show($"Deputy API connected successfully.{vbCrLf}{items.Count} operational unit(s) returned.",
                            "Test Passed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Deputy API test failed:{vbCrLf}{ex.Message}", "Test Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _btnTestApi.Enabled = True
            _btnTestApi.Text = "Test Deputy API"
        End Try
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
        If Not String.IsNullOrWhiteSpace(txtNewKey.Text) Then
            AppState.Db.SetConfigValue(txtNewKey.Text.Trim(), txtNewValue.Text.Trim())
            txtNewKey.Text = ""
            txtNewValue.Text = ""
        End If
        SeedDefaults()
        LoadApiConfig()
        MessageBox.Show("Settings saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub SeedDefaults()
        Dim defaults As New Dictionary(Of String, String) From {
            {"Deputy.BaseUrl", "https://YOURINSTANCE.au.deputy.com/api/v1/"},
            {"Deputy.OAuthToken", ""},
            {"Deputy.SyncDaysBack", "90"},
            {"RevSport.Email", ""},
            {"RevSport.Password", ""},
            {"RevSport.TotpSeed", ""},
            {"RevSport.SeasonId", "43649"},
            {"RevSport.EventsDaysBack", "90"}
        }
        For Each kv In defaults
            If String.IsNullOrEmpty(AppState.Db.GetConfigValue(kv.Key)) Then
                AppState.Db.SetConfigValue(kv.Key, kv.Value)
            End If
        Next
    End Sub

    Private Sub btnDeleteConfig_Click(sender As Object, e As EventArgs) Handles btnDeleteConfig.Click
        If AppState.Db Is Nothing Then Return
        If dgConfig.CurrentRow Is Nothing Then
            MessageBox.Show("Select a row to delete.", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim key = dgConfig.CurrentRow.Cells("Key").Value?.ToString()
        If String.IsNullOrEmpty(key) Then Return
        If MessageBox.Show($"Delete config key '{key}'?", "Confirm Delete",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            AppState.Db.DeleteConfigValue(key)
            LoadApiConfig()
        End If
    End Sub

    Private Sub dgConfig_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgConfig.ColumnWidthChanged
        GridColumnStore.Save("SettingsConfig", dgConfig)
    End Sub

End Class
