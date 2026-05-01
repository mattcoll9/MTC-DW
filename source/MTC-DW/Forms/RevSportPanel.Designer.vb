<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RevSportPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlFilter = New System.Windows.Forms.Panel()
        Me.lblCount = New System.Windows.Forms.Label()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.dtpTo = New System.Windows.Forms.DateTimePicker()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.cboSeason = New System.Windows.Forms.ComboBox()
        Me.lblSeason = New System.Windows.Forms.Label()
        Me.tabRevSport = New System.Windows.Forms.TabControl()
        Me.tabMembers = New System.Windows.Forms.TabPage()
        Me.dgMembers = New System.Windows.Forms.DataGridView()
        Me.tabEvents = New System.Windows.Forms.TabPage()
        Me.dgEvents = New System.Windows.Forms.DataGridView()
        Me.tabAttendees = New System.Windows.Forms.TabPage()
        Me.dgAttendees = New System.Windows.Forms.DataGridView()
        Me.pnlFilter.SuspendLayout()
        Me.tabRevSport.SuspendLayout()
        Me.tabMembers.SuspendLayout()
        CType(Me.dgMembers, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabEvents.SuspendLayout()
        CType(Me.dgEvents, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabAttendees.SuspendLayout()
        CType(Me.dgAttendees, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' pnlFilter
        '
        Me.pnlFilter.Controls.Add(Me.lblCount)
        Me.pnlFilter.Controls.Add(Me.btnLoad)
        Me.pnlFilter.Controls.Add(Me.dtpTo)
        Me.pnlFilter.Controls.Add(Me.lblTo)
        Me.pnlFilter.Controls.Add(Me.dtpFrom)
        Me.pnlFilter.Controls.Add(Me.lblFrom)
        Me.pnlFilter.Controls.Add(Me.cboSeason)
        Me.pnlFilter.Controls.Add(Me.lblSeason)
        Me.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFilter.Name = "pnlFilter"
        Me.pnlFilter.Size = New System.Drawing.Size(900, 40)
        '
        ' lblSeason
        '
        Me.lblSeason.Location = New System.Drawing.Point(4, 10)
        Me.lblSeason.Name = "lblSeason"
        Me.lblSeason.Size = New System.Drawing.Size(54, 20)
        Me.lblSeason.Text = "Season:"
        Me.lblSeason.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' cboSeason
        '
        Me.cboSeason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSeason.Location = New System.Drawing.Point(62, 8)
        Me.cboSeason.Name = "cboSeason"
        Me.cboSeason.Size = New System.Drawing.Size(140, 23)
        '
        ' lblFrom
        '
        Me.lblFrom.Location = New System.Drawing.Point(212, 10)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(36, 20)
        Me.lblFrom.Text = "From:"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' dtpFrom
        '
        Me.dtpFrom.CustomFormat = "dd/MM/yyyy"
        Me.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpFrom.Location = New System.Drawing.Point(252, 8)
        Me.dtpFrom.Name = "dtpFrom"
        Me.dtpFrom.Size = New System.Drawing.Size(110, 23)
        '
        ' lblTo
        '
        Me.lblTo.Location = New System.Drawing.Point(368, 10)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(26, 20)
        Me.lblTo.Text = "To:"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' dtpTo
        '
        Me.dtpTo.CustomFormat = "dd/MM/yyyy"
        Me.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpTo.Location = New System.Drawing.Point(398, 8)
        Me.dtpTo.Name = "dtpTo"
        Me.dtpTo.Size = New System.Drawing.Size(110, 23)
        '
        ' btnLoad
        '
        Me.btnLoad.Location = New System.Drawing.Point(518, 7)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(70, 26)
        Me.btnLoad.Text = "Load"
        '
        ' lblCount
        '
        Me.lblCount.Location = New System.Drawing.Point(598, 10)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Size = New System.Drawing.Size(160, 20)
        Me.lblCount.ForeColor = System.Drawing.Color.Gray
        '
        ' tabRevSport
        '
        Me.tabRevSport.Controls.Add(Me.tabMembers)
        Me.tabRevSport.Controls.Add(Me.tabEvents)
        Me.tabRevSport.Controls.Add(Me.tabAttendees)
        Me.tabRevSport.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabRevSport.Name = "tabRevSport"
        Me.tabRevSport.TabIndex = 1
        '
        ' tabMembers
        '
        Me.tabMembers.Controls.Add(Me.dgMembers)
        Me.tabMembers.Name = "tabMembers"
        Me.tabMembers.Text = "Members"
        '
        ' dgMembers
        '
        Me.dgMembers.AllowUserToAddRows = False
        Me.dgMembers.AllowUserToDeleteRows = False
        Me.dgMembers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgMembers.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgMembers.Name = "dgMembers"
        Me.dgMembers.ReadOnly = True
        Me.dgMembers.RowHeadersVisible = False
        Me.dgMembers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' tabEvents
        '
        Me.tabEvents.Controls.Add(Me.dgEvents)
        Me.tabEvents.Name = "tabEvents"
        Me.tabEvents.Text = "Events"
        '
        ' dgEvents
        '
        Me.dgEvents.AllowUserToAddRows = False
        Me.dgEvents.AllowUserToDeleteRows = False
        Me.dgEvents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgEvents.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgEvents.Name = "dgEvents"
        Me.dgEvents.ReadOnly = True
        Me.dgEvents.RowHeadersVisible = False
        Me.dgEvents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' tabAttendees
        '
        Me.tabAttendees.Controls.Add(Me.dgAttendees)
        Me.tabAttendees.Name = "tabAttendees"
        Me.tabAttendees.Text = "Event Attendees"
        '
        ' dgAttendees
        '
        Me.dgAttendees.AllowUserToAddRows = False
        Me.dgAttendees.AllowUserToDeleteRows = False
        Me.dgAttendees.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgAttendees.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgAttendees.Name = "dgAttendees"
        Me.dgAttendees.ReadOnly = True
        Me.dgAttendees.RowHeadersVisible = False
        Me.dgAttendees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' RevSportPanel
        '
        Me.Controls.Add(Me.tabRevSport)
        Me.Controls.Add(Me.pnlFilter)
        Me.Name = "RevSportPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.pnlFilter.ResumeLayout(False)
        Me.tabRevSport.ResumeLayout(False)
        Me.tabMembers.ResumeLayout(False)
        CType(Me.dgMembers, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabEvents.ResumeLayout(False)
        CType(Me.dgEvents, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabAttendees.ResumeLayout(False)
        CType(Me.dgAttendees, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlFilter As System.Windows.Forms.Panel
    Friend WithEvents lblSeason As System.Windows.Forms.Label
    Friend WithEvents cboSeason As System.Windows.Forms.ComboBox
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents dtpFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents dtpTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents btnLoad As System.Windows.Forms.Button
    Friend WithEvents lblCount As System.Windows.Forms.Label
    Friend WithEvents tabRevSport As System.Windows.Forms.TabControl
    Friend WithEvents tabMembers As System.Windows.Forms.TabPage
    Friend WithEvents dgMembers As System.Windows.Forms.DataGridView
    Friend WithEvents tabEvents As System.Windows.Forms.TabPage
    Friend WithEvents dgEvents As System.Windows.Forms.DataGridView
    Friend WithEvents tabAttendees As System.Windows.Forms.TabPage
    Friend WithEvents dgAttendees As System.Windows.Forms.DataGridView
End Class
