﻿Imports System.IO
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
    Dim WithEvents parserserial As ParserSerializer
    Dim WithEvents scraperserial As New ScraperSerializer("scraper.db")
    Dim previousId As Integer
#End Region
#Region "Events"
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        Parser1 = New Parser
        parserserial = New ParserSerializer("parser.db", Parser1)
        ' Add any initialization after the InitializeComponent() call.
        refreshTvSoftwares()
    End Sub
    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Dim res As DialogResult
        res = FolderBrowserDialog1.ShowDialog()
        If res = Windows.Forms.DialogResult.OK Then
            txtPath.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim directory As DirectoryInfo
        If txtPath.Text.Length > 0 Then
            Me.Cursor = Cursors.WaitCursor
            directory = New DirectoryInfo(txtPath.Text)
            parser.ParsePath(directory, Parser1)
            refreshTvSoftwares()
            Me.Cursor = Cursors.Default
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Cursor = Cursors.WaitCursor
        parserserial.SerializeList(Parser1)
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
        Me.Cursor = Cursors.WaitCursor
        parserserial.Deserialize(Parser1)
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
    Private Sub refreshTvSoftwares()
        Dim manufacturerNode, systemNode, softwareNode As TreeNode
        Dim soft As Parser.SoftwaresRow
        tvSoftwares.Nodes.Clear()
        For Each soft In Parser1.Softwares
            If tvSoftwares.Nodes.ContainsKey("mf" + soft.ManufacturersRow.manufacturerName) Then
                manufacturerNode = tvSoftwares.Nodes("mf" + soft.ManufacturersRow.manufacturerName)
            Else
                manufacturerNode = tvSoftwares.Nodes.Add("mf" + soft.ManufacturersRow.manufacturerName, soft.ManufacturersRow.manufacturerName)
            End If
            If manufacturerNode.Nodes.ContainsKey("sys" + soft.SystemsRow.systemName) Then
                systemNode = manufacturerNode.Nodes("sys" + soft.SystemsRow.systemName)
            Else
                systemNode = manufacturerNode.Nodes.Add("sys" + soft.SystemsRow.systemName, soft.SystemsRow.systemName)
            End If
            softwareNode = New TreeNode(soft.softwareName)
            softwareNode.Tag = soft
            systemNode.Nodes.Add(softwareNode)
        Next
    End Sub

    Private Sub tvSoftwares_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvSoftwares.AfterSelect
        Dim soft As Parser.SoftwaresRow
        Dim file As Parser.FilesRow
        Dim flag As Parser.FlagsRow
        Dim fileflag As Parser.FileFlagsRow
        Dim itm As ListViewItem
        Dim subitm As ListViewItem.ListViewSubItem
        Dim list As New Hashtable
        Dim g As System.Drawing.Graphics = Me.CreateGraphics()
        soft = e.Node.Tag
        lvFiles.Clear()
        If Not IsNothing(soft) Then
            Me.txtSoftwareTitle.Text = soft.softwareName
            Me.txtSoftwareManufacturer.Text = soft.ManufacturersRow.manufacturerName
            Me.txtSoftwarePlatform.Text = soft.SystemsRow.systemName
            Me.txtSoftwareType.Text = soft.TypesRow.typeName
            lvFiles.Columns.Add("fileName", "File Name", 200)
            lvFiles.Columns.Add("formatName", "Format", 60)
            lvFiles.Columns.Add("romsetName", "Romset", 60)
            For Each flag In Parser1.Flags.Select("flagType='File'", "flagId ASC")
                lvFiles.Columns.Add(flag.flagId.ToString, flag.flagName, Convert.ToInt32(g.MeasureString(flag.flagName, lvFiles.Font).Width) + 20)
            Next
            For Each file In soft.GetChildRows("softwareFile")
                itm = lvFiles.Items.Add(file.fileName)
                itm.SubItems.Add(file.FormatsRow.formatName)
                itm.SubItems.Add(file.RomsetsRow.romsetName)
                For Each flag In Parser1.Flags.Select("flagType='File'", "flagId ASC")
                    subitm = itm.SubItems.Add("")
                    For Each fileflag In file.GetChildRows("fileFlag")
                        If fileflag.FlagsRow.Equals(flag) Then
                            subitm.Text += fileflag.flagValue + ","
                        End If
                    Next
                    If subitm.Text.Length > 0 Then subitm.Text = subitm.Text.Substring(0, subitm.Text.Length - 1)
                Next
            Next
        Else
            Me.txtSoftwareTitle.Text = ""
            Me.txtSoftwareManufacturer.Text = ""
            Me.txtSoftwarePlatform.Text = ""
            Me.txtSoftwareType.Text = ""
        End If
    End Sub
End Class
