Imports System.Configuration
Imports System.Windows.Forms
Imports MTCDW.Services

Public Class MainForm

    Private _dashboard As DashboardPanel
    Private _jobs As JobsPanel
    Private _deputy As DeputyPanel
    Private _revSport As RevSportPanel
    Private _settings As SettingsPanel
    Private _logs As LogsPanel

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Text = "MTC Data Warehouse"
        Dim lblVersion As New ToolStripStatusLabel() With {
            .Text = $"v{Application.ProductVersion}",
            .Alignment = ToolStripItemAlignment.Right,
            .ForeColor = Drawing.Color.Gray
        }
        statusStrip.Items.Add(lblVersion)
        InitNavTree()

        Dim connStr = ConfigurationManager.ConnectionStrings("MTCDW")?.ConnectionString
        If String.IsNullOrEmpty(connStr) Then
            MessageBox.Show("No connection string found in app.config. Opening Settings.", "Setup Required",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            ShowPanel("Settings")
            Return
        End If

        AppState.Db = New DatabaseService(connStr)

        If Not AppState.Db.TestConnection() Then
            MessageBox.Show("Cannot connect to SQL Server using the connection string in app.config." & vbCrLf &
                            "Open Settings to update the connection string.",
                            "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ShowPanel("Settings")
            Return
        End If

        Try
            AppState.Db.EnsureSchema()
        Catch ex As Exception
            MessageBox.Show($"Schema initialisation failed: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        AppState.Activity = Sub(msg)
                                If InvokeRequired Then
                                    BeginInvoke(Sub() lblActivity.Text = msg)
                                Else
                                    lblActivity.Text = msg
                                End If
                            End Sub

        AppState.Scheduler = New SchedulerService(AppState.Db)
        AddHandler AppState.Scheduler.JobStarted, AddressOf OnJobStarted
        AddHandler AppState.Scheduler.JobCompleted, AddressOf OnJobCompleted
        AddHandler AppState.Scheduler.JobFailed, AddressOf OnJobFailed
        AppState.Scheduler.Start()

        lblScheduler.Text = "Scheduler: ON"
        ShowPanel("Dashboard")
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        AppState.Scheduler?.Stop()
    End Sub

    ' ── Navigation ───────────────────────────────────────────────────────────

    Private Sub InitNavTree()
        tvNav.Nodes.Clear()
        tvNav.Nodes.Add("Dashboard").Tag = "Dashboard"
        Dim nSources = tvNav.Nodes.Add("Sources")
        nSources.Nodes.Add("Deputy").Tag = "Deputy"
        nSources.Nodes.Add("RevSport").Tag = "RevSport"
        Dim nJobs = tvNav.Nodes.Add("Jobs")
        nJobs.Nodes.Add("Schedule").Tag = "Jobs"
        nJobs.Nodes.Add("History").Tag = "Logs"
        tvNav.Nodes.Add("Settings").Tag = "Settings"
        tvNav.ExpandAll()
    End Sub

    Private Sub tvNav_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tvNav.NodeMouseClick
        If e.Node.Tag IsNot Nothing Then ShowPanel(CStr(e.Node.Tag))
    End Sub

    Private Sub ShowPanel(name As String)
        pnlContent.Controls.Clear()

        Dim ctrl As Control
        Select Case name
            Case "Dashboard"
                If _dashboard Is Nothing Then
                    _dashboard = New DashboardPanel()
                End If
                _dashboard.Refresh()
                ctrl = _dashboard

            Case "Jobs"
                If _jobs Is Nothing Then _jobs = New JobsPanel()
                _jobs.RefreshJobs()
                ctrl = _jobs

            Case "Deputy"
                If _deputy Is Nothing Then _deputy = New DeputyPanel()
                ctrl = _deputy

            Case "RevSport"
                If _revSport Is Nothing Then _revSport = New RevSportPanel()
                ctrl = _revSport

            Case "Settings"
                If _settings Is Nothing Then _settings = New SettingsPanel()
                ctrl = _settings

            Case "Logs"
                If _logs Is Nothing Then _logs = New LogsPanel()
                _logs.RefreshLogs()
                ctrl = _logs

            Case Else
                Return
        End Select

        ctrl.Dock = DockStyle.Fill
        pnlContent.Controls.Add(ctrl)
    End Sub

    ' ── Scheduler event handlers (called from background thread) ─────────────

    Private Sub OnJobStarted(sender As Object, e As SchedulerService.JobEventArgs)
        If InvokeRequired Then
            Invoke(Sub() OnJobStarted(sender, e))
            Return
        End If
        lblLastRun.Text = $"Running: {e.JobName}"
        lblActivity.Text = ""
    End Sub

    Private Sub OnJobCompleted(sender As Object, e As SchedulerService.JobEventArgs)
        If InvokeRequired Then
            Invoke(Sub() OnJobCompleted(sender, e))
            Return
        End If
        lblLastRun.Text = $"Last: {e.JobName} — {e.RecordsAffected} rows at {DateTime.Now:HH:mm}"
        lblActivity.Text = ""
        If _dashboard IsNot Nothing AndAlso _dashboard.Visible Then _dashboard.Refresh()
        If _logs IsNot Nothing AndAlso _logs.Visible Then _logs.RefreshLogs()
        If _jobs IsNot Nothing AndAlso _jobs.Visible Then _jobs.RefreshJobs()
    End Sub

    Private Sub OnJobFailed(sender As Object, e As SchedulerService.JobEventArgs)
        If InvokeRequired Then
            Invoke(Sub() OnJobFailed(sender, e))
            Return
        End If
        lblLastRun.Text = $"Last: {e.JobName} FAILED at {DateTime.Now:HH:mm}"
        lblActivity.Text = ""
        If _logs IsNot Nothing AndAlso _logs.Visible Then _logs.RefreshLogs()
        If _jobs IsNot Nothing AndAlso _jobs.Visible Then _jobs.RefreshJobs()
    End Sub

End Class
