<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DeputyPanel
    Inherits System.Windows.Forms.UserControl

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlFilter = New System.Windows.Forms.Panel()
        Me.lblTimesheetCount = New System.Windows.Forms.Label()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.dtpTo = New System.Windows.Forms.DateTimePicker()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.tabDeputy = New System.Windows.Forms.TabControl()
        Me.tabTimesheets = New System.Windows.Forms.TabPage()
        Me.dgTimesheets = New System.Windows.Forms.DataGridView()
        Me.tabEmployees = New System.Windows.Forms.TabPage()
        Me.dgEmployees = New System.Windows.Forms.DataGridView()
        Me.tabOpUnits = New System.Windows.Forms.TabPage()
        Me.dgOpUnits = New System.Windows.Forms.DataGridView()
        Me.tabWorkTypes = New System.Windows.Forms.TabPage()
        Me.dgWorkTypes = New System.Windows.Forms.DataGridView()
        Me.pnlFilter.SuspendLayout()
        Me.tabDeputy.SuspendLayout()
        Me.tabTimesheets.SuspendLayout()
        CType(Me.dgTimesheets, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabEmployees.SuspendLayout()
        CType(Me.dgEmployees, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabOpUnits.SuspendLayout()
        CType(Me.dgOpUnits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabWorkTypes.SuspendLayout()
        CType(Me.dgWorkTypes, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        ' pnlFilter
        '
        Me.pnlFilter.Controls.Add(Me.lblTimesheetCount)
        Me.pnlFilter.Controls.Add(Me.btnLoad)
        Me.pnlFilter.Controls.Add(Me.dtpTo)
        Me.pnlFilter.Controls.Add(Me.lblTo)
        Me.pnlFilter.Controls.Add(Me.dtpFrom)
        Me.pnlFilter.Controls.Add(Me.lblFrom)
        Me.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFilter.Name = "pnlFilter"
        Me.pnlFilter.Size = New System.Drawing.Size(900, 40)
        '
        ' lblFrom
        '
        Me.lblFrom.Location = New System.Drawing.Point(4, 10)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(70, 20)
        Me.lblFrom.Text = "From:"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' dtpFrom
        '
        Me.dtpFrom.CustomFormat = "dd/MM/yyyy"
        Me.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpFrom.Location = New System.Drawing.Point(78, 8)
        Me.dtpFrom.Name = "dtpFrom"
        Me.dtpFrom.Size = New System.Drawing.Size(110, 23)
        '
        ' lblTo
        '
        Me.lblTo.Location = New System.Drawing.Point(194, 10)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(28, 20)
        Me.lblTo.Text = "To:"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        ' dtpTo
        '
        Me.dtpTo.CustomFormat = "dd/MM/yyyy"
        Me.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpTo.Location = New System.Drawing.Point(226, 8)
        Me.dtpTo.Name = "dtpTo"
        Me.dtpTo.Size = New System.Drawing.Size(110, 23)
        '
        ' btnLoad
        '
        Me.btnLoad.Location = New System.Drawing.Point(344, 7)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(70, 26)
        Me.btnLoad.Text = "Load"
        '
        ' lblTimesheetCount
        '
        Me.lblTimesheetCount.Location = New System.Drawing.Point(424, 10)
        Me.lblTimesheetCount.Name = "lblTimesheetCount"
        Me.lblTimesheetCount.Size = New System.Drawing.Size(160, 20)
        Me.lblTimesheetCount.ForeColor = System.Drawing.Color.Gray
        '
        ' tabDeputy
        '
        Me.tabDeputy.Controls.Add(Me.tabTimesheets)
        Me.tabDeputy.Controls.Add(Me.tabEmployees)
        Me.tabDeputy.Controls.Add(Me.tabOpUnits)
        Me.tabDeputy.Controls.Add(Me.tabWorkTypes)
        Me.tabDeputy.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabDeputy.Name = "tabDeputy"
        Me.tabDeputy.TabIndex = 1
        '
        ' tabTimesheets
        '
        Me.tabTimesheets.Controls.Add(Me.dgTimesheets)
        Me.tabTimesheets.Name = "tabTimesheets"
        Me.tabTimesheets.Text = "Timesheets"
        '
        ' dgTimesheets
        '
        Me.dgTimesheets.AllowUserToAddRows = False
        Me.dgTimesheets.AllowUserToDeleteRows = False
        Me.dgTimesheets.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgTimesheets.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgTimesheets.Name = "dgTimesheets"
        Me.dgTimesheets.ReadOnly = True
        Me.dgTimesheets.RowHeadersVisible = False
        Me.dgTimesheets.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' tabEmployees
        '
        Me.tabEmployees.Controls.Add(Me.dgEmployees)
        Me.tabEmployees.Name = "tabEmployees"
        Me.tabEmployees.Text = "Employees"
        '
        ' dgEmployees
        '
        Me.dgEmployees.AllowUserToAddRows = False
        Me.dgEmployees.AllowUserToDeleteRows = False
        Me.dgEmployees.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgEmployees.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgEmployees.Name = "dgEmployees"
        Me.dgEmployees.ReadOnly = True
        Me.dgEmployees.RowHeadersVisible = False
        Me.dgEmployees.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' tabOpUnits
        '
        Me.tabOpUnits.Controls.Add(Me.dgOpUnits)
        Me.tabOpUnits.Name = "tabOpUnits"
        Me.tabOpUnits.Text = "Operational Units"
        '
        ' dgOpUnits
        '
        Me.dgOpUnits.AllowUserToAddRows = False
        Me.dgOpUnits.AllowUserToDeleteRows = False
        Me.dgOpUnits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgOpUnits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgOpUnits.Name = "dgOpUnits"
        Me.dgOpUnits.ReadOnly = True
        Me.dgOpUnits.RowHeadersVisible = False
        Me.dgOpUnits.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' tabWorkTypes
        '
        Me.tabWorkTypes.Controls.Add(Me.dgWorkTypes)
        Me.tabWorkTypes.Name = "tabWorkTypes"
        Me.tabWorkTypes.Text = "Departments"
        '
        ' dgWorkTypes
        '
        Me.dgWorkTypes.AllowUserToAddRows = False
        Me.dgWorkTypes.AllowUserToDeleteRows = False
        Me.dgWorkTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgWorkTypes.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgWorkTypes.Name = "dgWorkTypes"
        Me.dgWorkTypes.ReadOnly = True
        Me.dgWorkTypes.RowHeadersVisible = False
        Me.dgWorkTypes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        '
        ' DeputyPanel
        '
        Me.Controls.Add(Me.tabDeputy)
        Me.Controls.Add(Me.pnlFilter)
        Me.Name = "DeputyPanel"
        Me.Size = New System.Drawing.Size(900, 600)
        Me.pnlFilter.ResumeLayout(False)
        Me.tabDeputy.ResumeLayout(False)
        Me.tabTimesheets.ResumeLayout(False)
        CType(Me.dgTimesheets, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabEmployees.ResumeLayout(False)
        CType(Me.dgEmployees, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabOpUnits.ResumeLayout(False)
        CType(Me.dgOpUnits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabWorkTypes.ResumeLayout(False)
        CType(Me.dgWorkTypes, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub

    Friend WithEvents pnlFilter As System.Windows.Forms.Panel
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents dtpFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents dtpTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents btnLoad As System.Windows.Forms.Button
    Friend WithEvents lblTimesheetCount As System.Windows.Forms.Label
    Friend WithEvents tabDeputy As System.Windows.Forms.TabControl
    Friend WithEvents tabTimesheets As System.Windows.Forms.TabPage
    Friend WithEvents dgTimesheets As System.Windows.Forms.DataGridView
    Friend WithEvents tabEmployees As System.Windows.Forms.TabPage
    Friend WithEvents dgEmployees As System.Windows.Forms.DataGridView
    Friend WithEvents tabOpUnits As System.Windows.Forms.TabPage
    Friend WithEvents dgOpUnits As System.Windows.Forms.DataGridView
    Friend WithEvents tabWorkTypes As System.Windows.Forms.TabPage
    Friend WithEvents dgWorkTypes As System.Windows.Forms.DataGridView
End Class
