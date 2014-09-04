Imports System.IO
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.IO.Compression
Imports System.Data.SQLite
Imports RegexParser
Public Class NoIntro
    Inherits FileParser
    Const patternManufacturer As String = "^(.+?) - "
    Const patternSystem As String = "^(?:.+?) - (.+?)(?:\(.+?\))?$"
    Dim rgxlist As New RegexList("Parsers\\NoIntro.xml")
    Public Overrides Sub ParsePath(dir As DirectoryInfo, ByRef ds As Parser)
        Dim systemdir As DirectoryInfo
        Dim files As FileInfo()
        Dim file As FileInfo
        Dim strManufacturer, strSystem, strType, strSoftwareName As String
        Dim strFormat, strFileName
        Dim strArchiveFileName, strArchiveFileExtension As String
        Dim manufacturer As Parser.ManufacturersRow
        Dim system As Parser.SystemsRow
        Dim type As Parser.TypesRow
        Dim software As Parser.SoftwaresRow
        Dim format As Parser.FormatsRow
        Dim romset As Parser.RomsetsRow
        Dim filerow As Parser.FilesRow
        Dim flag As Parser.FlagsRow
        Dim fileFlag As Parser.FileFlagsRow
        Dim archiverow As Parser.ArchiveFilesRow
        Dim rgxManufacturer As New Regex(patternManufacturer)
        Dim rgxSystem As New Regex(patternSystem)
        Dim archive As ZipArchive
        Dim entry As ZipArchiveEntry
        Dim fileid, gameid, count As Integer
        For Each systemdir In dir.GetDirectories()
            'get manufacturer
            strManufacturer = rgxManufacturer.Match(systemdir.Name).Groups(1).Value.Trim
            manufacturer = GetManufacturer(strManufacturer, ds)
            'get system
            strSystem = rgxSystem.Match(systemdir.Name).Groups(1).Value.Trim
            system = GetSystem(strSystem, manufacturer, ds)
            systemSoftwaresHT = New Hashtable
            For Each software In system.GetChildRows("systemSoftware")
                systemSoftwaresHT.Add(software.softwareName, software)
            Next
            'get softwares
            files = systemdir.GetFiles("*.zip")
            count = files.Length
            fileid = 0
            gameid = 0
            For Each file In files
                Dim rgxresults As New List(Of RegexResult)
                Dim fileflagsresults As List(Of RegexResult)
                rgxresults = rgxlist.ParseString(file.Name)
                strType = ""
                strSoftwareName = ""
                fileflagsresults = Nothing
                For Each result As RegexResult In rgxresults
                    Select Case result.Name
                        Case "Name"
                            strSoftwareName = result.Value
                        Case "Type"
                            strType = result.Value
                        Case "FileFlags"
                            fileflagsresults = result.Items
                    End Select
                Next
                If strType = "" Then strType = "Game"
                type = GetSoftwareType(strType, ds)
                '3. get software
                software = GetSoftware(system, type, strSoftwareName, ds)
                softwareFilesHT = New Hashtable
                For Each filerow In software.GetChildRows("softwareFile")
                    softwareFilesHT.Add(filerow.fileName, filerow)
                Next
                gameid += 1
                '5. get romset
                romset = GetRomSet("No-Intro", ds)
                '6. get format
                archive = ZipFile.Open(file.FullName, ZipArchiveMode.Read)
                If archive.Entries.Count > 1 Then
                    strFormat = "Archive"
                Else
                    entry = archive.Entries(0)
                    strFormat = Path.GetExtension(entry.FullName).Substring(1).ToUpper
                End If
                format = GetFormat(strFormat, ds)
                formatsDA.Update(ds.Formats)
                '7. get file name
                strFileName = file.Name
                '8. get file
                filerow = GetFile(strFileName, software, format, romset, ds)
                fileFlagsHT = New Hashtable
                For Each fileFlag In filerow.GetChildRows("fileFlag")
                    fileFlagsHT.Add(fileFlag.FlagsRow.flagName + "=" + fileFlag.flagValue, fileFlag)
                Next
                fileArchiveHT = New Hashtable
                For Each archiverow In filerow.GetChildRows("fileArchive")
                    fileArchiveHT.Add(archiverow.archiveFileName, archiverow)
                Next
                '9. get file flags
                For Each result As RegexResult In fileflagsresults
                    flag = GetFlag(result.Name, "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, result.Value, ds)
                Next
                '10. get file archives
                If strFormat = "Archive" Then
                    For Each entry In archive.Entries
                        strArchiveFileName = Path.GetFileNameWithoutExtension(entry.FullName)
                        strArchiveFileExtension = Path.GetExtension(entry.FullName)
                        archiverow = GetArchiveFile(filerow, strArchiveFileName, strArchiveFileExtension, ds)
                    Next
                End If
                fileid += 1
                OnItemAdded(systemdir.Name, fileid, count)
            Next
        Next
    End Sub
    Public Sub New(path As String)
        db = New SQLiteConnection("Data Source=" + path)
        SetupDataAdapters()
    End Sub
End Class