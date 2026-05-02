Public Class DeputyPanel

    Public Sub New()
        InitializeComponent()
        dtpFrom.Value = DateTime.Today.AddDays(-30)
        dtpTo.Value = DateTime.Today
        For Each dgv As DataGridView In {dgTimesheets, dgEmployees, dgOpUnits, dgWorkTypes}
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText
            dgv.AllowUserToOrderColumns = True
            dgv.MultiSelect = True
        Next
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
                Case 1 : LoadTable(dgEmployees, "deputy.Employees", "DeputyEmployees")
                Case 2 : LoadTable(dgOpUnits, "deputy.OperationalUnits", "DeputyOpUnits")
                Case 3 : LoadTable(dgWorkTypes, "deputy.Departments", "DeputyWorkTypes")
            End Select
        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadTimesheets()
        Dim dt = AppState.Db.GetDeputyTimesheets(dtpFrom.Value.Date, dtpTo.Value.Date)
        dgTimesheets.DataSource = dt
        GridColumnStore.Restore("DeputyTimesheets", dgTimesheets)
        lblTimesheetCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

    Private Sub LoadTable(dgv As DataGridView, tableName As String, storeKey As String)
        Dim dt = AppState.Db.GetDeputyTable(tableName)
        dgv.DataSource = dt
        GridColumnStore.Restore(storeKey, dgv)
    End Sub

    Private Sub btnConfigure_Click(sender As Object, e As EventArgs) Handles btnConfigure.Click
        Using frm As New DeputyConfigForm()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub dgTimesheets_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgTimesheets.ColumnWidthChanged
        GridColumnStore.Save("DeputyTimesheets", dgTimesheets)
    End Sub

    Private Sub dgEmployees_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgEmployees.ColumnWidthChanged
        GridColumnStore.Save("DeputyEmployees", dgEmployees)
    End Sub

    Private Sub dgOpUnits_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgOpUnits.ColumnWidthChanged
        GridColumnStore.Save("DeputyOpUnits", dgOpUnits)
    End Sub

    Private Sub dgWorkTypes_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgWorkTypes.ColumnWidthChanged
        GridColumnStore.Save("DeputyWorkTypes", dgWorkTypes)
    End Sub

End Class
