Imports System.IO
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.IO.Compression
Imports System.Data.SQLite

Public Class NoIntro
    Inherits FileParser
    Const patternManufacturer As String = "^(.+?) - "
    Const patternSystem As String = "^(?:.+?) - (.+?)(?:\(.+?\))?$"
    Const patternName As String = "(?:\[.*?\])?(.*?)\("
    Const patternBIOS As String = "\[BIOS\]"
    Const patternType As String = "\((Addon|Coverdisk|Diskmag|Program)(?: - )?([^\)]*?)?\)"
    Const patternCompilation As String = "\(([^\)]*?)?\s?(Compilation)(?: - )?([^\)]*?)?\)"
    Const patternDemo As String = "\(([^\)]*?)?\s?(Budget|Demo|Promo)(?: - )?([^\)]*?)?\)"
    Const patternRegion As String = "\((Australia|Brazil|Canada|China|France|Germany|Hong Kong|Italy|Japan|Korea|Netherlands|Spain|Sweden|USA|Asia|Europe|World)\)"
    Const patternLanguage As String = "\((En|Ja|Fr|De|Es|It|Nl|Pt|Sv|No|Da|Fi|Zh|Ko|Pl)\)"
    Const patternVersion As String = "\(.*?(v[\d|\.]+\w?)\s?(?:[^\)]*)\)"
    Const patternRevision As String = "\(.*?(Rev [\d|\w|\.]+)(?:[^\)]*)?\)"
    Const patternDevStatus As String = "\((Beta|Proto|Sample)\d?\)"
    Const patternLicense As String = "\((Unl)\)"
    Const patternBadDump As String = "\[b\]"
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
        Dim rgxName As New Regex(patternName)
        Dim rgxBIOS As New Regex(patternBIOS)
        Dim rgxType As New Regex(patternType)
        Dim rgxCompilation As New Regex(patternCompilation)
        Dim rgxDemo As New Regex(patternDemo)
        Dim rgxRegion As New Regex(patternRegion)
        Dim rgxLanguage As New Regex(patternLanguage)
        Dim rgxVersion As New Regex(patternVersion)
        Dim rgxRevision As New Regex(patternRevision)
        Dim rgxDevStatus As New Regex(patternDevStatus)
        Dim rgxLicense As New Regex(patternLicense)
        Dim rgxBadDump As New Regex(patternBadDump)
        Dim archive As ZipArchive
        Dim entry As ZipArchiveEntry
        Dim match As Match
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
                '1. get software name
                If rgxName.IsMatch(file.Name) Then
                    strSoftwareName = rgxName.Match(file.Name).Groups(1).Value.Trim
                Else
                    strSoftwareName = "UNK"
                End If
                '2. get software type
                If rgxBIOS.IsMatch(file.Name) Then
                    strType = "BIOS"
                ElseIf rgxType.IsMatch(file.Name) Then
                    strType = rgxType.Match(file.Name).Groups(1).Value
                Else
                    strType = "Game"
                End If
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
                If rgxCompilation.IsMatch(file.Name) Then
                    flag = GetFlag("Compilation", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, "True", ds)
                End If
                If rgxDemo.IsMatch(file.Name.Replace(",", ")(")) Then
                    flag = GetFlag("Demo", "File", ds)
                    For Each match In rgxDemo.Matches(file.Name.Replace(",", ")("))
                        fileFlag = GetFileFlag(filerow, flag, match.Groups(2).Value, ds)
                    Next
                End If
                If rgxRegion.IsMatch(file.Name.Replace(",", ")(")) Then
                    flag = GetFlag("Region", "File", ds)
                    For Each match In rgxRegion.Matches(file.Name.Replace(",", ")("))
                        fileFlag = GetFileFlag(filerow, flag, match.Groups(1).Value, ds)
                    Next
                End If
                If rgxLanguage.IsMatch(file.Name.Replace(",", ")(")) Then
                    flag = GetFlag("Language", "File", ds)
                    For Each match In rgxLanguage.Matches(file.Name.Replace(",", ")("))
                        fileFlag = GetFileFlag(filerow, flag, match.Groups(1).Value.ToLower, ds)
                    Next
                End If
                If rgxVersion.IsMatch(file.Name) Then
                    flag = GetFlag("Version", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxVersion.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxRevision.IsMatch(file.Name) Then
                    flag = GetFlag("Version", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxRevision.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxDevStatus.IsMatch(file.Name) Then
                    flag = GetFlag("DevStatus", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxDevStatus.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxLicense.IsMatch(file.Name) Then
                    flag = GetFlag("License", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxLicense.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxBadDump.IsMatch(file.Name) Then
                    flag = GetFlag("BadDump", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, "True", ds)
                End If
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