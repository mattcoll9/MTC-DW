Public Class RevSportPanel

    Public Sub New()
        InitializeComponent()
        ' Populate season dropdown from KnownSeasons
        For Each kv In Services.RevSportApiService.KnownSeasons
            cboSeason.Items.Add(New SeasonItem(kv.Key, kv.Value))
        Next
        If cboSeason.Items.Count > 0 Then cboSeason.SelectedIndex = 0
        dtpFrom.Value = DateTime.Today.AddDays(-90)
        dtpTo.Value = DateTime.Today
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
        lblCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

    Private Sub LoadEvents()
        Dim dt = AppState.Db.GetRevSportEvents(dtpFrom.Value.Date, dtpTo.Value.Date)
        dgEvents.DataSource = dt
        lblCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

    Private Sub LoadAttendees()
        Dim dt = AppState.Db.GetRevSportEventAttendees(dtpFrom.Value.Date, dtpTo.Value.Date)
        dgAttendees.DataSource = dt
        lblCount.Text = $"{dt.Rows.Count:N0} rows"
    End Sub

End Class
