Imports System.IO
Imports System.ComponentModel
Imports System.Data.SQLite
Public Class Generic
    Inherits Parser
    Dim WithEvents realParser As Parser
    Public Overrides Sub ParsePath(dir As DirectoryInfo, ByRef list As List(Of ParserSoftware))
        Dim setdir As DirectoryInfo
        For Each setdir In dir.GetDirectories()
            Select Case setdir.Name
                Case "No-Intro"
                    realParser = New NoIntro
                    'realParser = Nothing
                Case "TOSEC"
                    'realParser = New TOSEC
                    realParser = Nothing
                Case Else
                    realParser = Nothing
            End Select
            If Not IsNothing(realParser) Then
                realParser.ParsePath(setdir, list)
                Exit For
            End If
        Next
    End Sub
    Public Sub OnReadItemAdded(strLabel As String, intId As Integer, intCount As Integer) Handles realParser.ItemAdded
        OnItemAdded(strLabel, intId, intCount)
    End Sub
End Class
