Imports System.IO
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.IO.Compression
Public Class NoIntro
    Inherits Parser
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
    Dim gamefinder As Predicate(Of ParserSoftware)
    Public Overrides Sub ParsePath(dir As DirectoryInfo, ByRef list As List(Of ParserSoftware))
        Dim systemdir As DirectoryInfo
        Dim files As FileInfo()
        Dim file As FileInfo
        Dim strManufacturer As String
        Dim strSystem As String
        Dim pfFile As ParserZipFile
        Dim sfSoftware As ParserSoftware
        Dim strSoftwareName, strType As String
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
            strManufacturer = rgxManufacturer.Match(systemdir.Name).Groups(1).Value.Trim
            strSystem = rgxSystem.Match(systemdir.Name).Groups(1).Value.Trim
            files = systemdir.GetFiles("*.zip")
            count = files.Length
            fileid = 0
            gameid = 0
            For Each file In files
                '1. parse sotfware info
                If rgxName.IsMatch(file.Name) Then
                    strSoftwareName = rgxName.Match(file.Name).Groups(1).Value.Trim
                Else
                    strSoftwareName = "UNK"
                End If
                If rgxBIOS.IsMatch(file.Name) Then
                    strType = "BIOS"
                ElseIf rgxType.IsMatch(file.Name) Then
                    strType = rgxType.Match(file.Name).Groups(1).Value
                Else
                    strType = "Game"
                End If
                sfSoftware = list.Find(Function(x) x.SoftwareName.Equals(strSoftwareName) And x.ROMType.Equals(strType) And x.Platform.Equals(strSystem) And x.Manufacturer.Equals(strManufacturer))
                If IsNothing(sfSoftware) Then
                    gameid += 1
                    sfSoftware = New ParserSoftware()
                    sfSoftware.SoftwareId = gameid
                    sfSoftware.SoftwareName = strSoftwareName
                    sfSoftware.ROMType = strType
                    sfSoftware.Manufacturer = strManufacturer
                    sfSoftware.Platform = strSystem
                    list.Add(sfSoftware)
                End If
                '2. parse software file info
                pfFile = New ParserZipFile
                pfFile.ROMSet = "No-Intro"
                pfFile.FileName = file.Name
                archive = ZipFile.Open(file.FullName, ZipArchiveMode.Read)
                If archive.Entries.Count > 1 Then
                    pfFile.Format = "Archive"
                    For Each entry In archive.Entries
                        pfFile.ArchiveFiles.Add(New ParserArchiveFile(Path.GetFileNameWithoutExtension(entry.FullName), _
                                               Path.GetExtension(entry.FullName)))
                    Next
                Else
                    entry = archive.Entries(0)
                    pfFile.Format = Path.GetExtension(entry.FullName).Substring(1).ToUpper
                End If
                If rgxCompilation.IsMatch(file.Name) Then
                    pfFile.Flags.Add(New ParserFlag("Compilation", "True", "File"))
                End If
                If rgxDemo.IsMatch(file.Name.Replace(",", ")(")) Then
                    For Each match In rgxDemo.Matches(file.Name.Replace(",", ")("))
                        pfFile.Flags.Add(New ParserFlag("Demo", match.Groups(2).Value, "File"))
                    Next
                End If
                If rgxRegion.IsMatch(file.Name.Replace(",", ")(")) Then
                    For Each match In rgxRegion.Matches(file.Name.Replace(",", ")("))
                        pfFile.Flags.Add(New ParserFlag("Region", match.Groups(1).Value, "File"))
                    Next
                End If
                If rgxLanguage.IsMatch(file.Name.Replace(",", ")(")) Then
                    For Each match In rgxLanguage.Matches(file.Name.Replace(",", ")("))
                        pfFile.Flags.Add(New ParserFlag("Language", match.Groups(1).Value.ToLower, "File"))
                    Next
                End If
                If rgxVersion.IsMatch(file.Name) Then
                    pfFile.Flags.Add(New ParserFlag("Version", rgxVersion.Match(file.Name).Groups(1).Value, "File"))
                End If
                If rgxRevision.IsMatch(file.Name) Then
                    pfFile.Flags.Add(New ParserFlag("Version", rgxRevision.Match(file.Name).Groups(1).Value, "File"))
                End If
                If rgxDevStatus.IsMatch(file.Name) Then
                    pfFile.Flags.Add(New ParserFlag("DevStatus", rgxDevStatus.Match(file.Name).Groups(1).Value, "File"))
                End If
                If rgxLicense.IsMatch(file.Name) Then
                    pfFile.Flags.Add(New ParserFlag("License", rgxLicense.Match(file.Name).Groups(1).Value, "File"))
                End If
                If rgxBadDump.IsMatch(file.Name) Then
                    pfFile.Flags.Add(New ParserFlag("BadDump", "True", "File"))
                End If
                sfSoftware.Files.Add(pfFile)
                fileid += 1
                OnItemAdded(systemdir.Name, fileid, count)
            Next
        Next
    End Sub
End Class