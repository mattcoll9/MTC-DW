Public Class RevSportPanel

    Public Sub New()
        InitializeComponent()
        For Each kv In Services.RevSportApiService.KnownSeasons
            cboSeason.Items.Add(New SeasonItem(kv.Key, kv.Value))
        Next
        If cboSeason.Items.Count > 0 Then cboSeason.SelectedIndex = 0
        dtpFrom.Value = DateTime.Today.AddDays(-90)
        dtpTo.Value = DateTime.Today
        For Each dgv As DataGridView In {dgMembers, dgEvents, dgAttendees}
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText
            dgv.AllowUserToOrderColumns = True
            dgv.MultiSelect = True
        Next
    End Sub

    Private Class SeasonItem
        Public ReadOnly Id As Integer
        Public ReadOnly Label As String
        Public Sub New(id As Integer, label As String)
            Me.Id = id
            Me.Label = label
        End Sub
        Public Overrides Function ToString() As String
            Return Label
        End Function
    End Class

    Private Sub tabRevSport_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabRevSport.SelectedIndexChanged
        LoadCurrentTab()
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        LoadCurrentTab()
    End Sub

    Private Sub LoadCurrentTab()
        If AppState.Db Is Nothing Then Return
        Try
            Select Case tabRevSport.SelectedIndex
                Case 0 : LoadMembers()
                Case 1 : LoadEvents()
                Case 2 : LoadAttendees()
            End Select
        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadMembers()
        Dim seasonId As Integer = 43649
        If cboSeason.SelectedItem IsNot Nothing Then
            seasonId = CType(cboSeason.SelectedItem, SeasonItem).Id
        End If
        Dim dt = AppState.Db.GetRevSportMembers(seasonId)
        dgMembers.DataSource = dt
        GridColumnStore.Restore("RevSportMembers", dgMembers)
        lblCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

    Private Sub LoadEvents()
        Dim dt = AppState.Db.GetRevSportEvents(dtpFrom.Value.Date, dtpTo.Value.Date)
        dgEvents.DataSource = dt
        GridColumnStore.Restore("RevSportEvents", dgEvents)
        lblCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

    Private Sub LoadAttendees()
        Dim dt = AppState.Db.GetRevSportEventAttendees(dtpFrom.Value.Date, dtpTo.Value.Date)
        dgAttendees.DataSource = dt
        GridColumnStore.Restore("RevSportAttendees", dgAttendees)
        lblCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

    Private Sub btnConfigure_Click(sender As Object, e As EventArgs) Handles btnConfigure.Click
        Using frm As New RevSportConfigForm()
            frm.ShowDialog(Me)
        End Using
    End Sub

    Private Sub dgMembers_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgMembers.ColumnWidthChanged
        GridColumnStore.Save("RevSportMembers", dgMembers)
    End Sub

    Private Sub dgEvents_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgEvents.ColumnWidthChanged
        GridColumnStore.Save("RevSportEvents", dgEvents)
    End Sub

    Private Sub dgAttendees_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgAttendees.ColumnWidthChanged
        GridColumnStore.Save("RevSportAttendees", dgAttendees)
    End Sub

End Class
