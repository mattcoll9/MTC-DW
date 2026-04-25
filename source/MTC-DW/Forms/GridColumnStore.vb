Imports Newtonsoft.Json
Imports System.Collections.Generic
Imports System.IO

' Persists DataGridView column widths to a JSON file in the user's app-data folder.
' Call Restore after setting DataSource, Save from ColumnWidthChanged.
Module GridColumnStore

    Private _path As String = Path.Combine(Application.UserAppDataPath, "grid_columns.json")
    Private _data As Dictionary(Of String, Dictionary(Of String, Integer))

    Private Sub EnsureLoaded()
        If _data IsNot Nothing Then Return
        _data = New Dictionary(Of String, Dictionary(Of String, Integer))
        Try
            If File.Exists(_path) Then
                Dim loaded = JsonConvert.DeserializeObject(Of Dictionary(Of String, Dictionary(Of String, Integer)))(File.ReadAllText(_path))
                If loaded IsNot Nothing Then _data = loaded
            End If
        Catch
        End Try
    End Sub

    Public Sub Save(gridKey As String, dgv As DataGridView)
        EnsureLoaded()
        Dim widths As New Dictionary(Of String, Integer)
        For Each col As DataGridViewColumn In dgv.Columns
            widths(col.Name) = col.Width
        Next
        _data(gridKey) = widths
        Try
            File.WriteAllText(_path, JsonConvert.SerializeObject(_data))
        Catch
        End Try
    End Sub

    Public Sub Restore(gridKey As String, dgv As DataGridView)
        EnsureLoaded()
        Dim widths As Dictionary(Of String, Integer) = Nothing
        If Not _data.TryGetValue(gridKey, widths) Then Return
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None
        For Each col As DataGridViewColumn In dgv.Columns
            Dim w As Integer
            If widths.TryGetValue(col.Name, w) AndAlso w > 10 Then col.Width = w
        Next
    End Sub

End Module
