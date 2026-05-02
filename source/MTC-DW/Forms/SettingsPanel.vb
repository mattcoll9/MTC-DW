Imports System.Configuration

Public Class SettingsPanel

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub SettingsPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim cs = ConfigurationManager.ConnectionStrings("MTCDW")
        txtConnStr.Text = If(cs IsNot Nothing, cs.ConnectionString, "")
        If AppState.Db IsNot Nothing Then
            Try
                SeedDefaults()
            Catch
            End Try
        End If
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

    Private Sub btnExportConn_Click(sender As Object, e As EventArgs) Handles btnExportConn.Click
        Using sfd As New SaveFileDialog()
            sfd.Filter = "Config files (*.config)|*.config|Text files (*.txt)|*.txt"
            sfd.FileName = "mtcdw-connection"
            If sfd.ShowDialog() = DialogResult.OK Then
                Try
                    System.IO.File.WriteAllText(sfd.FileName, txtConnStr.Text.Trim())
                    MessageBox.Show("Exported.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show($"Export failed: {ex.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    Private Sub btnImportConn_Click(sender As Object, e As EventArgs) Handles btnImportConn.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Config files (*.config)|*.config|Text files (*.txt)|*.txt|All files (*.*)|*.*"
            If ofd.ShowDialog() = DialogResult.OK Then
                Try
                    txtConnStr.Text = System.IO.File.ReadAllText(ofd.FileName).Trim()
                    MessageBox.Show("Imported. Click Save to apply.", "Imported",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show($"Import failed: {ex.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

End Class
