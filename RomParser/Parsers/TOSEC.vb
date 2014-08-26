Imports System.IO
Imports System.IO.Compression
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Public Class TOSEC
    Inherits FileParser
    Const patternName As String = "^(.*?) (?:v[\d|\.]+|[rR]ev [\w|\d|\.]+|r\d[\d\.]+|\()"
    Const patternVersion As String = "(v[\d|\.]+|[rR]ev [\w|\d|\.]+|r\d[\d\.]+)+"
    Const patternDemo As String = "\((demo|demo-kiosk|demo-playable|demo-rolling|demo-slideshow)\)\s"
    Const patternDate As String = "(?:[^\(]*?)\((\d{4}|\d{2}xx|\d{3}x|\d{4}-\d{2}|\d{4}-\d{2}-\d{2})\)"
    Const patternPublisher As String = "(?:[^\(]*?)\((?:\d{4}|\d{2}xx|\d{3}x|\d{4}-\d{2}|\d{4}-\d{2}-\d{2})\)\((.*?)\)"
    Const patternSystem As String = "\((\+2|\+2a|\+3|130XE|A1000|A1200|A1200-A4000|A2000|A2000-A3000|A2024|A2500-A3000UX|A3000|A4000|A4000T|A500|A500\+|A500-A1000-A2000|" +
                                    "A500-A1000-A2000-CDTV|A500-A1200|A500-A1200-A2000-A4000|A500-A2000|A500-A600-A2000|A570|A600|A600HD|AGA|AGA-CD32|" +
                                    "Aladdin Deck Enhancer|CD32|CDTV|Computrainer|Doctor PC Jr\.|ECS|ECS-AGA|Executive|Mega ST|Mega-STE|OCS|OCS-AGA|ORCH80|Osbourne 1|" +
                                    "PIANO90|PlayChoice-10|Plus4|Primo-A|Primo-A64|Primo-B|Primo-B64|Pro-Primo|ST|STE|STE-Falcon|TT|TURBO-R GT|TURBO-R ST|VS DualSystem|" +
                                    "VS UniSystem)\)"
    Const patternCountry As String = "\((AE|AL|AS|AT|AU|BA|BE|BG|BR|CA|CH|CL|CN|CS|CY|CZ|DE|DK|EE|EG|ES|EU|FI|FR|GB|GR|HK|HR|HU|ID|IE|IL|IN|IR|IS|IT|JO|JP|KR|LT|LU|LV" +
                                    "|MN|MX|MY|NL|NO|NP|NZ|OM|PE|PH|PL|PT|QA|RO|RU|SE|SG|SI|SK|TH|TR|TW|US|VN|YU|ZA)\)"
    Const patternLanguage As String = "\((ar|bg|bs|cs|cy|da|de|el|en|eo|es|et|fa|fi|fr|ga|gu|he|hi|hr|hu|is|it|ja|ko|lt|lv|ms|nl|no|pl|pt|ro|ru|sk|sl|sq|sr|sv|th|tr|" +
                                      "ur|vi|yi|zh|M\d)\)"
    Const patternLicense As String = "\((CW|CW-R|FW|GW|GW-R|LW|PD|SW|SW-R)\)"
    Const patternDevStatus As String = "\((alpha|beta|preview|pre-release|proto)\)"
    Public Overrides Sub ParsePath(dir As DirectoryInfo, ByRef ds As Parser)
        Dim manufacturerdir As DirectoryInfo
        Dim systemdir As DirectoryInfo
        Dim typedir As DirectoryInfo
        Dim formatdir As DirectoryInfo
        Dim file As FileInfo
        Dim manufacturer As Parser.ManufacturersRow
        Dim system As Parser.SystemsRow
        Dim type As Parser.TypesRow
        Dim software As Parser.SoftwaresRow
        Dim strManufacturer As String
        Dim strSystem As String
        Dim strType As String
        Dim strSoftwareName As String
        Dim rgxName As New Regex(patternName)
        For Each manufacturerdir In dir.GetDirectories()
            strManufacturer = manufacturerdir.Name
            manufacturer = GetManufacturer(strManufacturer, ds)
            For Each systemdir In manufacturerdir.GetDirectories()
                strSystem = systemdir.Name
                system = GetSystem(strSystem, manufacturer, ds)
                For Each typedir In systemdir.GetDirectories
                    strType = typedir.Name
                    type = GetSoftwareType(strType, ds)
                    If typedir.GetDirectories().Length > 0 Then
                        For Each formatdir In typedir.GetDirectories()
                            For Each file In formatdir.GetFiles("*.zip")
                                If rgxName.IsMatch(file.Name) Then
                                    strSoftwareName = rgxName.Match(file.Name).Groups(1).Value.Trim
                                Else
                                    strSoftwareName = "UNK"
                                End If
                                software = GetSoftware(system, type, strSoftwareName, ds)
                                ParseFilename(file, software, ds)
                            Next
                        Next
                    Else
                        For Each file In typedir.GetFiles("*.zip")
                            If rgxName.IsMatch(file.Name) Then
                                strSoftwareName = rgxName.Match(file.Name).Groups(1).Value.Trim
                            Else
                                strSoftwareName = "UNK"
                            End If
                            software = GetSoftware(system, type, strSoftwareName, ds)
                            ParseFilename(file, software, ds)
                        Next
                    End If
                Next
            Next
            Exit For
        Next
    End Sub
    Private Sub ParseFilename(file As FileInfo, ByRef software As Parser.SoftwaresRow, ByRef ds As Parser)
        Dim archive As ZipArchive
        Dim entry As ZipArchiveEntry
        Dim rgxVersion As New Regex(patternVersion)
        Dim rgxDemo As New Regex(patternDemo)
        Dim rgxDate As New Regex(patternDate)
        Dim rgxPublisher As New Regex(patternPublisher)
        Dim rgxSystem As New Regex(patternSystem)
        Dim rgxCountry As New Regex(patternCountry)
        Dim rgxLanguage As New Regex(patternLanguage)
        Dim rgxLicense As New Regex(patternLicense)
        Dim rgxDevStatus As New Regex(patternDevStatus)
        Dim romset As Parser.RomsetsRow
        Dim format As Parser.FormatsRow
        Dim filerow As Parser.FilesRow
        Dim flag As Parser.FlagsRow
        Dim fileFlag As Parser.FileFlagsRow
        Dim softwareFlag As Parser.SoftwareFlagsRow
        Dim archiverow As Parser.ArchiveFilesRow
        Dim strFormat, strFileName
        Dim strArchiveFileName, strArchiveFileExtension As String
        Dim match As Match
        archive = ZipFile.Open(file.FullName, ZipArchiveMode.Read)
        If archive.Entries.Count > 1 Then
            strFormat = "Archive"
        Else
            entry = archive.Entries(0)
            strFormat = Path.GetExtension(entry.FullName).Substring(1).ToUpper
        End If
        format = GetFormat(strFormat, ds)
        romset = GetRomSet("TOSEC", ds)
        strFileName = file.Name
        filerow = GetFile(strFileName, software, format, romset, ds)
        If strFormat = "Archive" Then
            For Each entry In archive.Entries
                strArchiveFileName = Path.GetFileNameWithoutExtension(entry.FullName)
                strArchiveFileExtension = Path.GetExtension(entry.FullName)
                archiverow = GetArchiveFile(filerow, strArchiveFileName, strArchiveFileExtension, ds)
            Next
        End If
        If rgxVersion.IsMatch(file.Name) Then
            flag = GetFlag("Version", "File", ds)
            For Each match In rgxVersion.Matches(file.Name)
                fileFlag = GetFileFlag(filerow, flag, match.Groups(1).Value, ds)
            Next
        End If
        If rgxDemo.IsMatch(file.Name) Then
            flag = GetFlag("Demo", "File", ds)
            fileFlag = GetFileFlag(filerow, flag, rgxDemo.Match(file.Name).Groups(1).Value, ds)
        End If
        If rgxDate.IsMatch(file.Name) Then
            flag = GetFlag("Date", "File", ds)
            fileFlag = GetFileFlag(filerow, flag, rgxDate.Match(file.Name).Groups(1).Value, ds)
        End If
        If rgxPublisher.IsMatch(file.Name) Then
            flag = GetFlag("Publisher", "Game", ds)
            softwareFlag = GetSoftwareFlag(software, flag, rgxPublisher.Match(file.Name).Groups(1).Value, ds)
        End If
        If rgxSystem.IsMatch(file.Name) Then
            flag = GetFlag("System", "File", ds)
            fileFlag = GetFileFlag(filerow, flag, rgxSystem.Match(file.Name).Groups(1).Value, ds)
        End If
        If rgxCountry.IsMatch(file.Name.Replace("-", ")(")) Then
            flag = GetFlag("Region", "File", ds)
            For Each match In rgxCountry.Matches(file.Name.Replace("-", ")("))
                fileFlag = GetFileFlag(filerow, flag, match.Groups(1).Value, ds)
            Next
        End If
        If rgxLanguage.IsMatch(file.Name.Replace("-", ")(")) Then
            flag = GetFlag("Language", "File", ds)
            For Each match In rgxLanguage.Matches(file.Name.Replace("-", ")("))
                fileFlag = GetFileFlag(filerow, flag, match.Groups(1).Value, ds)
            Next
        End If
        If rgxLicense.IsMatch(file.Name) Then
            flag = GetFlag("License", "File", ds)
            fileFlag = GetFileFlag(filerow, flag, rgxLicense.Match(file.Name).Groups(1).Value, ds)
        End If
        If rgxDevStatus.IsMatch(file.Name) Then
            flag = GetFlag("DevStatus", "File", ds)
            fileFlag = GetFileFlag(filerow, flag, rgxDevStatus.Match(file.Name).Groups(1).Value, ds)
        End If
    End Sub
End Class
