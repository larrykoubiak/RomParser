<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.txtPath = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripProgressBar1 = New System.Windows.Forms.ToolStripProgressBar()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.tcTabs = New System.Windows.Forms.TabControl()
        Me.tbParser = New System.Windows.Forms.TabPage()
        Me.lvFiles = New System.Windows.Forms.ListView()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.txtSoftwareType = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtSoftwarePlatform = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtSoftwareManufacturer = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtSoftwareTitle = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tvSoftwares = New System.Windows.Forms.TreeView()
        Me.tbScraper = New System.Windows.Forms.TabPage()
        Me.lvReleases = New System.Windows.Forms.ListView()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtGenre = New System.Windows.Forms.TextBox()
        Me.lbl8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtDeveloper = New System.Windows.Forms.TextBox()
        Me.txtPlot = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtURL = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtPlatform = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtTitle = New System.Windows.Forms.TextBox()
        Me.tvGames = New System.Windows.Forms.TreeView()
        Me.Parser1 = New RomParser.Parser()
        Me.Scraper1 = New RomParser.Scraper()
        Me.StatusStrip1.SuspendLayout()
        Me.tcTabs.SuspendLayout()
        Me.tbParser.SuspendLayout()
        Me.tbScraper.SuspendLayout()
        CType(Me.Parser1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Scraper1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtPath
        '
        Me.txtPath.Location = New System.Drawing.Point(102, 14)
        Me.txtPath.Name = "txtPath"
        Me.txtPath.Size = New System.Drawing.Size(294, 20)
        Me.txtPath.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(6, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(90, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "ROM Root folder:"
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(402, 12)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowse.TabIndex = 3
        Me.btnBrowse.Text = "Browse"
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(489, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(68, 22)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Parse Roms"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel2, Me.ToolStripStatusLabel1, Me.ToolStripProgressBar1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 640)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1226, 22)
        Me.StatusStrip1.TabIndex = 7
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.AutoSize = False
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(350, 17)
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.AutoSize = False
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(150, 17)
        Me.ToolStripStatusLabel1.Text = " "
        '
        'ToolStripProgressBar1
        '
        Me.ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        Me.ToolStripProgressBar1.Size = New System.Drawing.Size(200, 16)
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(563, 12)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(149, 22)
        Me.Button2.TabIndex = 8
        Me.Button2.Text = "Export Parsed Data To DB"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(718, 12)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 22)
        Me.Button3.TabIndex = 9
        Me.Button3.Text = "Scrape"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(607, 103)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(25, 13)
        Me.Label6.TabIndex = 17
        Me.Label6.Text = "Plot"
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(799, 12)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(153, 22)
        Me.Button4.TabIndex = 27
        Me.Button4.Text = "Export Scraped Data To DB"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'tcTabs
        '
        Me.tcTabs.Controls.Add(Me.tbParser)
        Me.tcTabs.Controls.Add(Me.tbScraper)
        Me.tcTabs.Location = New System.Drawing.Point(0, 53)
        Me.tcTabs.Name = "tcTabs"
        Me.tcTabs.SelectedIndex = 0
        Me.tcTabs.Size = New System.Drawing.Size(1226, 584)
        Me.tcTabs.TabIndex = 30
        '
        'tbParser
        '
        Me.tbParser.AllowDrop = True
        Me.tbParser.Controls.Add(Me.lvFiles)
        Me.tbParser.Controls.Add(Me.Label13)
        Me.tbParser.Controls.Add(Me.txtSoftwareType)
        Me.tbParser.Controls.Add(Me.Label12)
        Me.tbParser.Controls.Add(Me.txtSoftwarePlatform)
        Me.tbParser.Controls.Add(Me.Label11)
        Me.tbParser.Controls.Add(Me.txtSoftwareManufacturer)
        Me.tbParser.Controls.Add(Me.Label10)
        Me.tbParser.Controls.Add(Me.txtSoftwareTitle)
        Me.tbParser.Controls.Add(Me.Label1)
        Me.tbParser.Controls.Add(Me.tvSoftwares)
        Me.tbParser.Location = New System.Drawing.Point(4, 22)
        Me.tbParser.Name = "tbParser"
        Me.tbParser.Padding = New System.Windows.Forms.Padding(3)
        Me.tbParser.Size = New System.Drawing.Size(1218, 558)
        Me.tbParser.TabIndex = 1
        Me.tbParser.Text = "Parser"
        Me.tbParser.UseVisualStyleBackColor = True
        '
        'lvFiles
        '
        Me.lvFiles.Location = New System.Drawing.Point(287, 145)
        Me.lvFiles.Name = "lvFiles"
        Me.lvFiles.Size = New System.Drawing.Size(911, 97)
        Me.lvFiles.TabIndex = 11
        Me.lvFiles.UseCompatibleStateImageBehavior = False
        Me.lvFiles.View = System.Windows.Forms.View.Details
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(211, 145)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(28, 13)
        Me.Label13.TabIndex = 10
        Me.Label13.Text = "Files"
        '
        'txtSoftwareType
        '
        Me.txtSoftwareType.Location = New System.Drawing.Point(287, 81)
        Me.txtSoftwareType.Name = "txtSoftwareType"
        Me.txtSoftwareType.Size = New System.Drawing.Size(186, 20)
        Me.txtSoftwareType.TabIndex = 8
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(211, 84)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(31, 13)
        Me.Label12.TabIndex = 7
        Me.Label12.Text = "Type"
        '
        'txtSoftwarePlatform
        '
        Me.txtSoftwarePlatform.Location = New System.Drawing.Point(287, 55)
        Me.txtSoftwarePlatform.Name = "txtSoftwarePlatform"
        Me.txtSoftwarePlatform.Size = New System.Drawing.Size(186, 20)
        Me.txtSoftwarePlatform.TabIndex = 6
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(211, 58)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(45, 13)
        Me.Label11.TabIndex = 5
        Me.Label11.Text = "Platform"
        '
        'txtSoftwareManufacturer
        '
        Me.txtSoftwareManufacturer.Location = New System.Drawing.Point(287, 29)
        Me.txtSoftwareManufacturer.Name = "txtSoftwareManufacturer"
        Me.txtSoftwareManufacturer.Size = New System.Drawing.Size(186, 20)
        Me.txtSoftwareManufacturer.TabIndex = 4
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(211, 32)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(70, 13)
        Me.Label10.TabIndex = 3
        Me.Label10.Text = "Manufacturer"
        '
        'txtSoftwareTitle
        '
        Me.txtSoftwareTitle.Location = New System.Drawing.Point(287, 3)
        Me.txtSoftwareTitle.Name = "txtSoftwareTitle"
        Me.txtSoftwareTitle.Size = New System.Drawing.Size(186, 20)
        Me.txtSoftwareTitle.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(211, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(27, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Title"
        '
        'tvSoftwares
        '
        Me.tvSoftwares.Location = New System.Drawing.Point(6, 6)
        Me.tvSoftwares.Name = "tvSoftwares"
        Me.tvSoftwares.Size = New System.Drawing.Size(199, 536)
        Me.tvSoftwares.TabIndex = 0
        '
        'tbScraper
        '
        Me.tbScraper.Controls.Add(Me.lvReleases)
        Me.tbScraper.Controls.Add(Me.Label9)
        Me.tbScraper.Controls.Add(Me.FlowLayoutPanel1)
        Me.tbScraper.Controls.Add(Me.Label8)
        Me.tbScraper.Controls.Add(Me.txtGenre)
        Me.tbScraper.Controls.Add(Me.lbl8)
        Me.tbScraper.Controls.Add(Me.Label7)
        Me.tbScraper.Controls.Add(Me.txtDeveloper)
        Me.tbScraper.Controls.Add(Me.txtPlot)
        Me.tbScraper.Controls.Add(Me.Label5)
        Me.tbScraper.Controls.Add(Me.txtURL)
        Me.tbScraper.Controls.Add(Me.Label4)
        Me.tbScraper.Controls.Add(Me.txtPlatform)
        Me.tbScraper.Controls.Add(Me.Label3)
        Me.tbScraper.Controls.Add(Me.txtTitle)
        Me.tbScraper.Controls.Add(Me.tvGames)
        Me.tbScraper.Location = New System.Drawing.Point(4, 22)
        Me.tbScraper.Name = "tbScraper"
        Me.tbScraper.Padding = New System.Windows.Forms.Padding(3)
        Me.tbScraper.Size = New System.Drawing.Size(1218, 558)
        Me.tbScraper.TabIndex = 0
        Me.tbScraper.Text = "Scraper"
        Me.tbScraper.UseVisualStyleBackColor = True
        '
        'lvReleases
        '
        Me.lvReleases.Location = New System.Drawing.Point(295, 142)
        Me.lvReleases.Name = "lvReleases"
        Me.lvReleases.Size = New System.Drawing.Size(903, 97)
        Me.lvReleases.TabIndex = 43
        Me.lvReleases.UseCompatibleStateImageBehavior = False
        Me.lvReleases.View = System.Windows.Forms.View.Details
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(250, 302)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(41, 13)
        Me.Label9.TabIndex = 42
        Me.Label9.Text = "Images"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoScroll = True
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(295, 302)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(903, 203)
        Me.FlowLayoutPanel1.TabIndex = 41
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(253, 119)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(36, 13)
        Me.Label8.TabIndex = 40
        Me.Label8.Text = "Genre"
        '
        'txtGenre
        '
        Me.txtGenre.Location = New System.Drawing.Point(295, 116)
        Me.txtGenre.Name = "txtGenre"
        Me.txtGenre.Size = New System.Drawing.Size(190, 20)
        Me.txtGenre.TabIndex = 39
        '
        'lbl8
        '
        Me.lbl8.AutoSize = True
        Me.lbl8.Location = New System.Drawing.Point(238, 142)
        Me.lbl8.Name = "lbl8"
        Me.lbl8.Size = New System.Drawing.Size(51, 13)
        Me.lbl8.TabIndex = 38
        Me.lbl8.Text = "Releases"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(233, 93)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(56, 13)
        Me.Label7.TabIndex = 36
        Me.Label7.Text = "Developer"
        '
        'txtDeveloper
        '
        Me.txtDeveloper.Location = New System.Drawing.Point(295, 90)
        Me.txtDeveloper.Name = "txtDeveloper"
        Me.txtDeveloper.Size = New System.Drawing.Size(190, 20)
        Me.txtDeveloper.TabIndex = 35
        '
        'txtPlot
        '
        Me.txtPlot.Location = New System.Drawing.Point(622, 8)
        Me.txtPlot.Multiline = True
        Me.txtPlot.Name = "txtPlot"
        Me.txtPlot.Size = New System.Drawing.Size(576, 128)
        Me.txtPlot.TabIndex = 34
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(260, 67)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(29, 13)
        Me.Label5.TabIndex = 33
        Me.Label5.Text = "URL"
        '
        'txtURL
        '
        Me.txtURL.Location = New System.Drawing.Point(295, 64)
        Me.txtURL.Name = "txtURL"
        Me.txtURL.Size = New System.Drawing.Size(321, 20)
        Me.txtURL.TabIndex = 32
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(244, 41)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(45, 13)
        Me.Label4.TabIndex = 31
        Me.Label4.Text = "Platform"
        '
        'txtPlatform
        '
        Me.txtPlatform.Location = New System.Drawing.Point(295, 38)
        Me.txtPlatform.Name = "txtPlatform"
        Me.txtPlatform.Size = New System.Drawing.Size(190, 20)
        Me.txtPlatform.TabIndex = 30
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(262, 11)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(27, 13)
        Me.Label3.TabIndex = 29
        Me.Label3.Text = "Title"
        '
        'txtTitle
        '
        Me.txtTitle.Location = New System.Drawing.Point(295, 8)
        Me.txtTitle.Name = "txtTitle"
        Me.txtTitle.Size = New System.Drawing.Size(190, 20)
        Me.txtTitle.TabIndex = 28
        '
        'tvGames
        '
        Me.tvGames.Location = New System.Drawing.Point(6, 8)
        Me.tvGames.Name = "tvGames"
        Me.tvGames.Size = New System.Drawing.Size(222, 492)
        Me.tvGames.TabIndex = 27
        '
        'Parser1
        '
        Me.Parser1.DataSetName = "Parser"
        Me.Parser1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'Scraper1
        '
        Me.Scraper1.DataSetName = "Scraper"
        Me.Scraper1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1226, 662)
        Me.Controls.Add(Me.tcTabs)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtPath)
        Me.Name = "frmMain"
        Me.Text = "ROM Parser"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.tcTabs.ResumeLayout(False)
        Me.tbParser.ResumeLayout(False)
        Me.tbParser.PerformLayout()
        Me.tbScraper.ResumeLayout(False)
        Me.tbScraper.PerformLayout()
        CType(Me.Parser1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Scraper1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents txtPath As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripProgressBar1 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents tcTabs As System.Windows.Forms.TabControl
    Friend WithEvents tbScraper As System.Windows.Forms.TabPage
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtGenre As System.Windows.Forms.TextBox
    Friend WithEvents lbl8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtDeveloper As System.Windows.Forms.TextBox
    Friend WithEvents txtPlot As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtURL As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents txtPlatform As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtTitle As System.Windows.Forms.TextBox
    Friend WithEvents tvGames As System.Windows.Forms.TreeView
    Friend WithEvents tvSoftwares As System.Windows.Forms.TreeView
    Friend WithEvents txtSoftwarePlatform As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents txtSoftwareManufacturer As System.Windows.Forms.TextBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents txtSoftwareTitle As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtSoftwareType As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Parser1 As RomParser.Parser
    Friend WithEvents tbParser As System.Windows.Forms.TabPage
    Friend WithEvents lvFiles As System.Windows.Forms.ListView
    Friend WithEvents Scraper1 As RomParser.Scraper
    Friend WithEvents lvReleases As System.Windows.Forms.ListView

End Class
