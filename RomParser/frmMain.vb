Imports System.IO
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports System.Xml
Imports System.Drawing
Public Class frmMain
#Region "Variables"
    Dim WithEvents scraper As GameFAQScraper
    Dim WithEvents parser As Generic
    Dim datreader As New DatReader("F:\\DATRoot\\No-Intro\\Commodore - Amiga.dat")
    Dim previousId As Integer
#End Region
#Region "Events"
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        parser = New Generic("parser.db", Parser1)
        scraper = New GameFAQScraper("scraper.db", Scraper1)
        ' Add any initialization after the InitializeComponent() call.
        refreshTvSoftwares()
        refreshTvGames()
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
            parser.Serialize(Parser1)
            refreshTvSoftwares()
            Me.Cursor = Cursors.Default
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Cursor = Cursors.WaitCursor
        parser.Serialize(Parser1)
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim sys As ScraperSystem
        For Each sys In scraper.Systems
            scraper.GetGameList(sys, Scraper1)
            scraper.Serialize(Scraper1)
            refreshTvGames()
        Next
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.Cursor = Cursors.WaitCursor
        scraper.Serialize(Me.Scraper1)
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub tvSoftwares_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvSoftwares.AfterSelect
        Dim soft As Parser.SoftwaresRow
        Dim file As Parser.FilesRow
        Dim flag As Parser.FlagsRow
        Dim fileflag As Parser.FileFlagsRow
        Dim itm As ListViewItem
        Dim subitm As ListViewItem.ListViewSubItem
        Dim g As System.Drawing.Graphics = Me.CreateGraphics()
        soft = e.Node.Tag
        lvFiles.Clear()
        If Not IsNothing(soft) Then
            Me.txtSoftwareTitle.Text = soft.softwareName
            Me.txtSoftwareManufacturer.Text = soft.SystemsRow.ManufacturersRow.manufacturerName
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
    Private Sub tvGames_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles tvGames.AfterSelect
        Dim game As Scraper.GamesRow
        Dim genre As Scraper.GameGenreFlagsRow
        Dim release As Scraper.ReleasesRow
        Dim strGenre As String
        Dim itm As ListViewItem
        game = e.Node.Tag
        lvReleases.Clear()
        If Not IsNothing(game) Then
            Me.txtTitle.Text = game.gameName
            Me.txtPlatform.Text = game.PlatformsRow.platformName
            Me.txtURL.Text = game.gameURL
            Me.txtPlot.Text = game.gamePlot
            strGenre = ""
            For Each genre In game.GetChildRows("gameGameGenreFlag")
                strGenre += genre.GenreFlagsRow.genreFlagName + " - "
            Next
            If strGenre.Length > 0 Then strGenre = strGenre.Substring(0, strGenre.Length - 3)
            Me.txtGenre.Text = strGenre
            Me.txtDeveloper.Text = game.DevelopersRow.developerName
            lvReleases.Columns.Add("releaseName", "File Name", 200)
            lvReleases.Columns.Add("regionName", "Region", 60)
            lvReleases.Columns.Add("publisherName", "Publisher", 60)
            lvReleases.Columns.Add("productId", "Product Nr", 60)
            lvReleases.Columns.Add("distBarcode", "Dist/Barcode", 60)
            lvReleases.Columns.Add("releaseDate", "Release Date", 60)
            lvReleases.Columns.Add("rating", "Rating", 60)
            For Each release In game.GetChildRows("gameRelease")
                itm = lvReleases.Items.Add(release.releaseName)
                itm.Tag = release
                itm.SubItems.Add(release.RegionsRow.regionName)
                itm.SubItems.Add(release.PublishersRow.publisherName)
                itm.SubItems.Add(release.releaseProductNr)
                itm.SubItems.Add(release.releaseDistBarcode)
                itm.SubItems.Add(release.releaseDate)
                itm.SubItems.Add(release.releaseRating)
            Next
            lvReleases.Items(0).Selected = True
        Else
            Me.txtSoftwareTitle.Text = ""
            Me.txtSoftwareManufacturer.Text = ""
            Me.txtSoftwarePlatform.Text = ""
            Me.txtSoftwareType.Text = ""
        End If
    End Sub
    Private Sub lvReleases_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvReleases.SelectedIndexChanged
        Dim release As Scraper.ReleasesRow
        Dim image As Scraper.ImagesRow
        Dim picturebox As PictureBox
        Dim itm As ListViewItem
        Dim ratio As Double
        If lvReleases.SelectedItems.Count > 0 Then
            Me.FlowLayoutPanel1.Controls.Clear()
            For Each itm In lvReleases.SelectedItems
                release = itm.Tag
                If release IsNot Nothing Then
                    For Each image In release.GetChildRows("releaseImage")
                        picturebox = New PictureBox()
                        picturebox.SizeMode = PictureBoxSizeMode.StretchImage
                        picturebox.Image = System.Drawing.Image.FromStream(New MemoryStream(image.imageData))
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

#End Region
#Region "Handlers"
    Private Sub itemAdded(strLabel As String, intId As Integer, intCount As Integer) Handles parser.ItemAdded, scraper.ItemAdded
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
            If tvSoftwares.Nodes.ContainsKey("mf" + soft.SystemsRow.ManufacturersRow.manufacturerName) Then
                manufacturerNode = tvSoftwares.Nodes("mf" + soft.SystemsRow.ManufacturersRow.manufacturerName)
            Else
                manufacturerNode = tvSoftwares.Nodes.Add("mf" + soft.SystemsRow.ManufacturersRow.manufacturerName, soft.SystemsRow.ManufacturersRow.manufacturerName)
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
    Private Sub refreshTvGames()
        Dim rootnode, node As TreeNode
        For Each game In Scraper1.Games
            If tvGames.Nodes.ContainsKey(game.PlatformsRow.platformAcronym) Then
                rootnode = tvGames.Nodes(game.PlatformsRow.platformAcronym)
            Else
                rootnode = tvGames.Nodes.Add(game.PlatformsRow.platformAcronym, game.PlatformsRow.platformName)
            End If
            node = New TreeNode(game.gameName)
            node.Tag = game
            rootnode.Nodes.Add(node)
        Next
    End Sub
End Class
