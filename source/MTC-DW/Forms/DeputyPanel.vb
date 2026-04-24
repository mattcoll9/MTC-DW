Public Class DeputyPanel

    Public Sub New()
        InitializeComponent()
        dtpFrom.Value = DateTime.Today.AddDays(-30)
        dtpTo.Value = DateTime.Today
    End Sub

    Private Sub tabDeputy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabDeputy.SelectedIndexChanged
        LoadCurrentTab()
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        LoadCurrentTab()
    End Sub

    Private Sub LoadCurrentTab()
        If AppState.Db Is Nothing Then Return
        Try
            Select Case tabDeputy.SelectedIndex
                Case 0 : LoadTimesheets()
                Case 1 : LoadTable(dgEmployees, "deputy.Employees")
                Case 2 : LoadTable(dgOpUnits, "deputy.OperationalUnits")
                Case 3 : LoadTable(dgWorkTypes, "deputy.WorkTypes")
            End Select
        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadTimesheets()
        Dim dt = AppState.Db.GetDeputyTimesheets(dtpFrom.Value.Date, dtpTo.Value.Date)
        dgTimesheets.DataSource = dt
        lblTimesheetCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

    Private Sub LoadTable(dgv As DataGridView, tableName As String)
        Dim dt = AppState.Db.GetDeputyTable(tableName)
        dgv.DataSource = dt
    End Sub

End Class
