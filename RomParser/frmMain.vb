Imports System.IO
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Drawing
Public Class frmMain
#Region "Variables"
    Dim WithEvents gf As New GameFAQScraper
    Dim WithEvents parser As New Generic
    Dim softwares As New List(Of ParserSoftware)
    Dim games As New List(Of ScraperGame)
    Dim WithEvents parserserial As New ParserSerializer("parser.db")
    Dim WithEvents scraperserial As New ScraperSerializer("scraper.db")
    Dim previousId As Integer
#End Region
#Region "Events"
    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Dim res As DialogResult
        res = FolderBrowserDialog1.ShowDialog()
        If res = Windows.Forms.DialogResult.OK Then
            txtPath.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim manufacturerNode, systemNode, softwareNode As TreeNode
        Dim directory As DirectoryInfo
        If txtPath.Text.Length > 0 Then
            Me.Cursor = Cursors.WaitCursor
            directory = New DirectoryInfo(txtPath.Text)
            parser.ParsePath(directory, softwares)
            For Each soft In softwares
                If tvSoftwares.Nodes.ContainsKey("mf" + soft.Manufacturer) Then
                    manufacturerNode = tvSoftwares.Nodes("mf" + soft.Manufacturer)
                Else
                    manufacturerNode = tvSoftwares.Nodes.Add("mf" + soft.Manufacturer, soft.Manufacturer)
                End If
                If manufacturerNode.Nodes.ContainsKey("sys" + soft.Platform) Then
                    systemNode = manufacturerNode.Nodes("sys" + soft.Platform)
                Else
                    systemNode = manufacturerNode.Nodes.Add("sys" + soft.Platform, soft.Platform)
                End If
                softwareNode = New TreeNode(soft.SoftwareName)
                softwareNode.Tag = soft.SoftwareId
                systemNode.Nodes.Add(softwareNode)
            Next
            Me.Cursor = Cursors.Default
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Cursor = Cursors.WaitCursor
        parserserial.SerializeList(softwares)
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim sys As ScraperSystem
        Dim rootnode, node As TreeNode
        Dim game As ScraperGame
        tvGames.Nodes.Clear()
        sys = gf.Systems.Find(Function(x) x.Acronym = "WSC")
        gf.GetGameList(sys, games)
        rootnode = tvGames.Nodes.Add(sys.Acronym, sys.Name)
        For Each game In games
            node = New TreeNode(game.Title)
            node.Tag = game.ID
            rootnode.Nodes.Add(node)
        Next
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Cursor = Cursors.WaitCursor
        scraperserial.SerializeList(games)
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim rootnode, node As TreeNode
        Me.Cursor = Cursors.WaitCursor
        scraperserial.Deserialize(games)
        For Each game In games
            If tvGames.Nodes.ContainsKey(game.Platform.Acronym) Then
                rootnode = tvGames.Nodes(game.Platform.Acronym)
            Else
                rootnode = tvGames.Nodes.Add(game.Platform.Acronym, game.Platform.Name)
            End If
            node = New TreeNode(game.Title)
            node.Tag = game.ID
            rootnode.Nodes.Add(node)
        Next
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim manufacturerNode, systemNode, softwareNode As TreeNode
        Dim soft As ParserSoftware
        Me.Cursor = Cursors.WaitCursor
        parserserial.Deserialize(softwares)
        For Each soft In softwares
            If tvSoftwares.Nodes.ContainsKey("mf" + soft.Manufacturer) Then
                manufacturerNode = tvSoftwares.Nodes("mf" + soft.Manufacturer)
            Else
                manufacturerNode = tvSoftwares.Nodes.Add("mf" + soft.Manufacturer, soft.Manufacturer)
            End If
            If manufacturerNode.Nodes.ContainsKey("sys" + soft.Platform) Then
                systemNode = manufacturerNode.Nodes("sys" + soft.Platform)
            Else
                systemNode = manufacturerNode.Nodes.Add("sys" + soft.Platform, soft.Platform)
            End If
            softwareNode = New TreeNode(soft.SoftwareName)
            softwareNode.Tag = soft.SoftwareId
            systemNode.Nodes.Add(softwareNode)
        Next
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub dgvReleases_SelectionChanged(sender As Object, e As EventArgs) Handles dgvReleases.SelectionChanged
        Dim release As ScraperRelease
        Dim image As ScraperImage
        Dim picturebox As PictureBox
        Dim row As DataGridViewRow
        Dim ratio As Double
        If dgvReleases.SelectedRows.Count > 0 Then
            Me.FlowLayoutPanel1.Controls.Clear()
            For Each row In dgvReleases.SelectedRows
                release = TryCast(row.DataBoundItem, ScraperRelease)
                If release IsNot Nothing Then
                    For Each image In release.Images
                        picturebox = New PictureBox()
                        picturebox.SizeMode = PictureBoxSizeMode.StretchImage
                        picturebox.Image = System.Drawing.Image.FromStream(New MemoryStream(image.ImageData))
                        ratio = picturebox.Image.Width / picturebox.Image.Height
                        Select Case ratio
                            Case Is < 1
                                picturebox.Width = Int(200 * ratio)
                                picturebox.Height = 200
                            Case Is > 1
                                picturebox.Width = 200
                                picturebox.Height = Int(200 / ratio)
                            Case 1
                                picturebox.Width = picturebox.Height = 200
                        End Select
                        FlowLayoutPanel1.Controls.Add(picturebox)
                    Next
                End If
            Next
        End If
    End Sub
    Private Sub tvGames_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tvGames.NodeMouseClick
        Dim game As ScraperGame
        Dim genre As ScraperGenre
        Dim strGenre As String
        game = games.Find(Function(x) x.ID = e.Node.Tag)
        If Not IsNothing(game) Then
            Me.txtTitle.Text = game.Title
            Me.txtPlatform.Text = game.Platform.Name
            Me.txtURL.Text = game.URL
            Me.txtPlot.Text = game.Plot
            strGenre = ""
            For Each genre In game.Genres
                strGenre += genre.Name + " - "
            Next
            strGenre = strGenre.Substring(0, strGenre.Length - 3)
            Me.txtGenre.Text = strGenre
            Me.txtDeveloper.Text = game.Developer
            Me.dgvReleases.DataSource = game.Releases
            Me.dgvReleases.Rows(0).Selected = True
        End If
    End Sub
    Private Sub tvSoftwares_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tvSoftwares.NodeMouseClick
        Dim soft As ParserSoftware
        soft = softwares.Find(Function(x) x.SoftwareId = e.Node.Tag)
        If Not IsNothing(soft) Then
            Me.txtSoftwareTitle.Text = soft.SoftwareName
            Me.txtSoftwareManufacturer.Text = soft.Manufacturer
            Me.txtSoftwarePlatform.Text = soft.Platform
            Me.txtSoftwareType.Text = soft.ROMType
            Me.dgvFiles.DataSource = soft.Files
        End If
    End Sub
#End Region
#Region "Handlers"
    Private Sub itemAdded(strLabel As String, intId As Integer, intCount As Integer) Handles parserserial.ItemAdded, parser.ItemAdded, gf.ItemAdded, scraperserial.ItemAdded
        ToolStripStatusLabel1.Text = intId.ToString + " of " + intCount.ToString
        ToolStripStatusLabel2.Text = strLabel
        If ToolStripProgressBar1.Maximum <> intCount Then
            ToolStripProgressBar1.Maximum = intCount
        End If
        ToolStripProgressBar1.Value = intId
        Application.DoEvents()
    End Sub
#End Region


End Class
