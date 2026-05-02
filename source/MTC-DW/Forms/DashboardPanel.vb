Public Class DashboardPanel

    Public Sub New()
        InitializeComponent()
        For Each dgv As DataGridView In {dgHistory, dgTableStats}
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText
            dgv.AllowUserToOrderColumns = True
            dgv.MultiSelect = True
        Next
    End Sub

    Public Overrides Sub Refresh()
        MyBase.Refresh()
        LoadTableStats()
        LoadRecentHistory()
    End Sub

    Private Sub LoadTableStats()
        If AppState.Db Is Nothing Then Return
        Try
            dgTableStats.DataSource = AppState.Db.GetTableStats()
            GridColumnStore.Restore("DashboardTableStats", dgTableStats)
            ColourSourceRows()
        Catch
        End Try
    End Sub

    Private Sub ColourSourceRows()
        For Each row As DataGridViewRow In dgTableStats.Rows
            Dim src = If(row.Cells("Source").Value?.ToString(), "")
            Select Case src
                Case "Deputy"   : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(240, 248, 255)
                Case "RevSport" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 248, 240)
                Case Else       : row.DefaultCellStyle.BackColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Private Sub dgTableStats_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgTableStats.ColumnWidthChanged
        GridColumnStore.Save("DashboardTableStats", dgTableStats)
    End Sub

    Private Sub LoadRecentHistory()
        If AppState.Db Is Nothing Then Return
        Try
            Dim hist = AppState.Db.GetJobHistory(maxRows:=20)
            Dim dt As New DataTable()
            dt.Columns.Add("Job", GetType(String))
            dt.Columns.Add("Started", GetType(DateTime))
            dt.Columns.Add("Duration", GetType(String))
            dt.Columns.Add("Status", GetType(String))
            dt.Columns.Add("Records", GetType(String))
            dt.Columns.Add("Error", GetType(String))
            For Each h In hist
                dt.Rows.Add(h.JobName, h.StartedAt, h.Duration, h.Status,
                            If(h.RecordsAffected.HasValue, h.RecordsAffected.Value.ToString("N0"), "—"),
                            If(String.IsNullOrEmpty(h.ErrorMessage), "", h.ErrorMessage.Substring(0, Math.Min(80, h.ErrorMessage.Length))))
            Next
            dgHistory.DataSource = dt
            GridColumnStore.Restore("DashboardHistory", dgHistory)
            If dgHistory.Columns.Contains("Started") Then
                dgHistory.Columns("Started").DefaultCellStyle.Format = "dd/MM HH:mm"
                dgHistory.Sort(dgHistory.Columns("Started"), System.ComponentModel.ListSortDirection.Descending)
            End If
            ColourStatusRows()
        Catch
        End Try
    End Sub

    Private Sub ColourStatusRows()
        For Each row As DataGridViewRow In dgHistory.Rows
            Dim status = If(row.Cells("Status").Value?.ToString(), "")
            Select Case status
                Case "Success" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(220, 255, 220)
                Case "Failed"  : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 220, 220)
                Case "Running" : row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 255, 200)
                Case Else      : row.DefaultCellStyle.BackColor = Drawing.Color.White
            End Select
        Next
    End Sub

    Private Sub dgHistory_ColumnWidthChanged(sender As Object, e As DataGridViewColumnEventArgs) Handles dgHistory.ColumnWidthChanged
        GridColumnStore.Save("DashboardHistory", dgHistory)
    End Sub

End Class
