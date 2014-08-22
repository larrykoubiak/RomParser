Imports HtmlAgilityPack
Imports System.Data
Public Class Table
    Private dt As DataTable
    Public Property Data() As DataTable
        Get
            Return dt
        End Get
        Set(ByVal value As DataTable)
            dt = value
        End Set
    End Property

    Public Sub New()
        dt = New DataTable()
    End Sub
    Public Sub New(tableName As String, tableNode As HtmlNode)
        dt = New DataTable(tableName)
    End Sub
    Public Function ParseTable(tableNode As HtmlNode) As DataTable
        Dim headNodes As HtmlNodeCollection
        Dim rownodes As HtmlNodeCollection
        Dim rownode As HtmlNode
        Dim columnnodes As HtmlNodeCollection
        Dim node As HtmlNode
        Dim datarow As DataRow
        Dim i As Integer
        Dim colname As String
        Dim colFlags As New Hashtable
        headNodes = tableNode.SelectNodes("thead/tr/th")
        rownodes = tableNode.SelectNodes("tr")
        dt.Rows.Clear()
        dt.Columns.Clear()
        columnnodes = rownodes(0).SelectNodes("td")
        For i = 0 To headNodes.Count - 1
            colname = headNodes(i).InnerText.Trim
            If Not columnnodes(i).HasChildNodes Then
                dt.Columns.Add(colname)
                colFlags.Add(i, "Text")
            Else
                For Each node In columnnodes(i).ChildNodes
                    Select node.Name
                        Case "a"
                            colFlags.Add(i, "Link")
                            dt.Columns.Add(colname)
                            dt.Columns.Add(colname + "_URL")
                        Case "img"
                            colFlags.Add(i, "Image")
                            dt.Columns.Add(colname)
                        Case Else
                    End Select
                Next
            End If

        Next
        rownodes = tableNode.SelectNodes("tr")
        'check if there are links

        For Each rownode In rownodes
            datarow = dt.NewRow()
            columnnodes = rownode.SelectNodes("td")
            For i = 0 To columnnodes.Count - 1
                Select Case colFlags(i)
                    Case "Text"
                        datarow(i) = columnnodes(i).InnerText.Trim
                    Case "Link"
                        datarow(i) = columnnodes(i).SelectSingleNode("a").Attributes("href").Value
                    Case Else
                End Select
            Next
        Next
        Return dt
    End Function
End Class
