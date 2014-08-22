Imports System.IO
Imports System.IO.Compression
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Public Class TOSEC
    Inherits Parser
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
    Public Overrides Sub ParsePath(dir As DirectoryInfo, ByRef list As List(Of ParserSoftware))
        Dim manufacturerdir As DirectoryInfo
        Dim systemdir As DirectoryInfo
        Dim typedir As DirectoryInfo
        Dim formatdir As DirectoryInfo
        Dim file As FileInfo
        Dim strManufacturer As String
        Dim strSystem As String
        Dim strType As String
        Dim strFormat As String
        Dim strSoftwareName As String
        Dim sfSoftware As ParserSoftware
        Dim rgxName As New Regex(patternName)
        For Each manufacturerdir In dir.GetDirectories()
            strManufacturer = manufacturerdir.Name
            For Each systemdir In manufacturerdir.GetDirectories()
                strSystem = systemdir.Name
                For Each typedir In systemdir.GetDirectories
                    strType = typedir.Name
                    If typedir.GetDirectories().Length > 0 Then
                        For Each formatdir In typedir.GetDirectories()
                            strFormat = formatdir.Name
                            For Each file In formatdir.GetFiles("*.zip")
                                If rgxName.IsMatch(file.Name) Then
                                    strSoftwareName = rgxName.Match(file.Name).Groups(1).Value.Trim
                                Else
                                    strSoftwareName = "UNK"
                                End If
                                sfSoftware = list.Find(Function(x) x.SoftwareName.Equals(strSoftwareName) And x.ROMType.Equals(strType) And x.Platform.Equals(strSystem) And x.Manufacturer.Equals(strManufacturer))
                                If IsNothing(sfSoftware) Then
                                    sfSoftware = New ParserSoftware()
                                    sfSoftware.SoftwareName = strSoftwareName
                                    sfSoftware.ROMType = strType
                                    sfSoftware.Manufacturer = strManufacturer
                                    sfSoftware.Platform = strSystem
                                    list.Add(sfSoftware)
                                End If
                                sfSoftware.Manufacturer = strManufacturer
                                sfSoftware.Platform = strSystem
                                sfSoftware.ROMType = strType
                                ParseFilename(file, sfSoftware, strFormat)
                            Next
                        Next
                    Else
                        For Each file In typedir.GetFiles("*.zip")
                            If rgxName.IsMatch(file.Name) Then
                                strSoftwareName = rgxName.Match(file.Name).Groups(1).Value.Trim
                            Else
                                strSoftwareName = "UNK"
                            End If
                            sfSoftware = list.Find(Function(x) x.SoftwareName.Equals(strSoftwareName) And x.ROMType.Equals(strType) And x.Platform.Equals(strSystem) And x.Manufacturer.Equals(strManufacturer))
                            If IsNothing(sfSoftware) Then
                                sfSoftware = New ParserSoftware()
                                sfSoftware.SoftwareName = strSoftwareName
                                sfSoftware.ROMType = strType
                                sfSoftware.Manufacturer = strManufacturer
                                sfSoftware.Platform = strSystem
                                list.Add(sfSoftware)
                            End If
                            ParseFilename(file, sfSoftware, "")
                        Next
                    End If
                Next
            Next
            Exit For
        Next
    End Sub
    Private Sub ParseFilename(file As FileInfo, ByRef sfSoftware As ParserSoftware, ByVal format As String)
        Dim zfFile As New ParserZipFile
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
        Dim match As Match
        zfFile.ROMSet = "TOSEC"
        If format = "" Then
            archive = ZipFile.Open(file.FullName, ZipArchiveMode.Read)
            If archive.Entries.Count > 1 Then
                zfFile.Format = "Archive"
                entry = archive.Entries(0)
                For Each entry In archive.Entries
                    zfFile.ArchiveFiles.Add(New ParserArchiveFile(Path.GetFileNameWithoutExtension(entry.FullName), _
                                           Path.GetExtension(entry.FullName)))
                Next

            Else
                entry = archive.Entries(0)
                zfFile.Format = Path.GetExtension(entry.FullName).Substring(1).ToUpper
            End If
        Else
            zfFile.Format = format
        End If
        zfFile.FileName = file.Name
        If rgxVersion.IsMatch(file.Name) Then
            For Each match In rgxVersion.Matches(file.Name)
                zfFile.Flags.Add(New ParserFlag("Version", match.Groups(1).Value, "File"))
            Next
        End If
        If rgxDemo.IsMatch(file.Name) Then
            zfFile.Flags.Add(New ParserFlag("Demo", rgxDemo.Match(file.Name).Groups(1).Value, "File"))
        End If
        If rgxDate.IsMatch(file.Name) Then
            zfFile.Flags.Add(New ParserFlag("Date", rgxDate.Match(file.Name).Groups(1).Value, "File"))
        End If
        If rgxPublisher.IsMatch(file.Name) Then
            sfSoftware.Flags.Add(New ParserFlag("Publisher", rgxPublisher.Match(file.Name).Groups(1).Value, "Game"))
        End If
        If rgxSystem.IsMatch(file.Name) Then
            zfFile.Flags.Add(New ParserFlag("System", rgxSystem.Match(file.Name).Groups(1).Value, "File"))
        End If
        If rgxCountry.IsMatch(file.Name.Replace("-", ")(")) Then
            For Each match In rgxCountry.Matches(file.Name.Replace("-", ")("))
                zfFile.Flags.Add(New ParserFlag("Region", match.Groups(1).Value, "File"))
            Next
        End If
        If rgxLanguage.IsMatch(file.Name.Replace("-", ")(")) Then
            For Each match In rgxLanguage.Matches(file.Name.Replace("-", ")("))
                zfFile.Flags.Add(New ParserFlag("Language", match.Groups(1).Value, "File"))
            Next
        End If
        If rgxLicense.IsMatch(file.Name) Then
            zfFile.Flags.Add(New ParserFlag("License", rgxLicense.Match(file.Name).Groups(1).Value, "File"))
        End If
        If rgxDevStatus.IsMatch(file.Name) Then
            zfFile.Flags.Add(New ParserFlag("DevStatus", rgxDevStatus.Match(file.Name).Groups(1).Value, "File"))
        End If
        sfSoftware.Files.Add(zfFile)
    End Sub
End Class
