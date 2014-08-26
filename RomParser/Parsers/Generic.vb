Imports System.IO
Imports System.ComponentModel
Imports System.Data.SQLite
Public Class Generic
    Inherits FileParser
    Dim dbpath As String
    Dim parserds As Parser
    Dim WithEvents realparser As FileParser
    Public Overrides Sub ParsePath(dir As DirectoryInfo, ByRef ds As Parser)
        Dim setdir As DirectoryInfo
        For Each setdir In dir.GetDirectories()
            Select Case setdir.Name
                Case "No-Intro"
                    realparser = New NoIntro(dbpath)
                    'realParser = Nothing
                Case "TOSEC"
                    'realParser = New TOSEC
                    realParser = Nothing
                Case Else
                    realParser = Nothing
            End Select
            If Not IsNothing(realParser) Then
                realParser.ParsePath(setdir, ds)
                Exit For
            End If
        Next
    End Sub
    Public Sub New(path As String, ByRef ds As Parser)
        dbpath = path
        db = New SQLiteConnection("Data Source=" + path)
        CreateParserTables()
        SetupDataAdapters()
        Deserialize(ds)
    End Sub
    Public Sub OnReadItemAdded(strLabel As String, intId As Integer, intCount As Integer) Handles realParser.ItemAdded
        OnItemAdded(strLabel, intId, intCount)
    End Sub
End Class
