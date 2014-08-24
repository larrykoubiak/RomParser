Imports System.IO
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.IO.Compression

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
    Dim gamefinder As Predicate(Of ParserSoftware)
    Public Overrides Sub ParsePath(dir As DirectoryInfo, ByRef ds As Parser)
        Dim systemdir As DirectoryInfo
        Dim files As FileInfo()
        Dim file As FileInfo
        Dim strManufacturer, strSystem, strType, strSoftwareName As String
        Dim strFormat, strFileName, strFlagValue As String
        Dim strArchiveFileName, strArchiveFileExtension As String
        Dim manufacturer As Parser.ManufacturersRow
        Dim system As Parser.SystemsRow
        Dim type As Parser.TypesRow
        Dim software As Parser.SoftwaresRow
        Dim format As Parser.FormatsRow
        Dim romset As Parser.RomsetsRow
        Dim filerow As Parser.FilesRow
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
            strManufacturer = rgxManufacturer.Match(systemdir.Name).Groups(1).Value.Trim
            manufacturer = GetManufacturer(strManufacturer, ds)
            strSystem = rgxSystem.Match(systemdir.Name).Groups(1).Value.Trim
            system = GetSystem(strSystem, ds)
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
                type = GetSoftwareType(strType, ds)
                software = GetSoftware(manufacturer, system, type, strSoftwareName, ds)
                gameid += 1
                '2. parse software file info
                romset = GetRomSet("No-Intro", ds)
                archive = ZipFile.Open(file.FullName, ZipArchiveMode.Read)
                If archive.Entries.Count > 1 Then
                    strFormat = "Archive"
                Else
                    entry = archive.Entries(0)
                    strFormat = Path.GetExtension(entry.FullName).Substring(1).ToUpper
                End If
                format = GetFormat(strFormat, ds)
                strFileName = file.Name
                filerow = getFile(strFileName, software, format, romset, ds)
                If strFormat = "Archive" Then
                    For Each entry In archive.Entries
                        strArchiveFileName = Path.GetFileNameWithoutExtension(entry.FullName)
                        strArchiveFileExtension = Path.GetExtension(entry.FullName)
                        archiverow = GetArchiveFile(filerow, strArchiveFileName, strArchiveFileExtension, ds)
                    Next
                End If
                If rgxCompilation.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("Compilation", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, "True", ds)
                End If
                If rgxDemo.IsMatch(file.Name.Replace(",", ")(")) Then
                    Dim flag As Parser.FlagsRow = GetFlag("Demo", "File", ds)
                    For Each match In rgxDemo.Matches(file.Name.Replace(",", ")("))
                        fileFlag = GetFileFlag(filerow, flag, match.Groups(2).Value, ds)
                    Next
                End If
                If rgxRegion.IsMatch(file.Name.Replace(",", ")(")) Then
                    Dim flag As Parser.FlagsRow = GetFlag("Region", "File", ds)
                    For Each match In rgxRegion.Matches(file.Name.Replace(",", ")("))
                        fileFlag = GetFileFlag(filerow, flag, match.Groups(1).Value, ds)
                    Next
                End If
                If rgxLanguage.IsMatch(file.Name.Replace(",", ")(")) Then
                    Dim flag As Parser.FlagsRow = GetFlag("Language", "File", ds)
                    For Each match In rgxLanguage.Matches(file.Name.Replace(",", ")("))
                        fileFlag = GetFileFlag(filerow, flag, match.Groups(1).Value.ToLower, ds)
                    Next
                End If
                If rgxVersion.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("Version", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxVersion.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxRevision.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("Version", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxRevision.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxDevStatus.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("DevStatus", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxDevStatus.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxLicense.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("License", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxLicense.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxBadDump.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("BadDump", "File", ds)
                    strFlagValue = "True"
                    fileFlag = GetFileFlag(filerow, flag, "True", ds)
                End If
                fileid += 1
                OnItemAdded(systemdir.Name, fileid, count)
            Next
        Next
    End Sub
    Private Function GetManufacturer(strManufacturerName As String, ByRef ds As Parser) As Parser.ManufacturersRow
        Dim manufacturer As Parser.ManufacturersRow()
        manufacturer = ds.Manufacturers.Select("manufacturerName = '" + strManufacturerName.Replace("'", "''") + "'")
        If manufacturer.Length = 0 Then
            Return ds.Manufacturers.AddManufacturersRow(strManufacturerName)
        Else
            Return manufacturer(0)
        End If
    End Function
    Private Function GetSystem(strSystemName As String, ByRef ds As Parser) As Parser.SystemsRow
        Dim system As Parser.SystemsRow()
        system = ds.Systems.Select("systemName = '" + strSystemName.Replace("'", "''") + "'")
        If system.Length = 0 Then
            Return ds.Systems.AddSystemsRow(strSystemName)
        Else
            Return system(0)
        End If
    End Function

    Private Function GetSoftwareType(strTypeName As String, ByRef ds As Parser) As Parser.TypesRow
        Dim type As Parser.TypesRow()
        type = ds.Types.Select("typeName = '" + strTypeName.Replace("'", "''") + "'")
        If type.Length = 0 Then
            Return ds.Types.AddTypesRow(strTypeName)
        End If
        Return type(0)
    End Function
    Private Function GetSoftware(manufacturer As Parser.ManufacturersRow, system As Parser.SystemsRow, type As Parser.TypesRow, strSoftwareName As String, ByRef ds As Parser) As Parser.SoftwaresRow
        Dim soft As Parser.SoftwaresRow()
        soft = ds.Softwares.Select("manufacturerId = " + manufacturer.manufacturerId.ToString + " And systemId = " + system.systemId.ToString _
                                   + " and typeId = " + type.typeId.ToString + " and softwareName = '" + strSoftwareName.Replace("'", "''") + "'")
        If soft.Length = 0 Then
            Return ds.Softwares.AddSoftwaresRow(strSoftwareName, manufacturer, system, type)
        Else
            Return soft(0)
        End If
    End Function
    Private Function GetFlag(strFlagName As String, strFlagType As String, ByRef ds As Parser) As Parser.FlagsRow
        Dim flag As Parser.FlagsRow()
        flag = ds.Flags.Select("flagName = '" + strFlagName.Replace("'", "''") + "' and flagType = '" + strFlagType.Replace("'", "''") + "'")
        If flag.Length = 0 Then
            Return ds.Flags.AddFlagsRow(strFlagName, strFlagType)
        Else
            Return flag(0)
        End If
    End Function
    Private Function GetRomSet(strRomsetName As String, ByRef ds As Parser) As Parser.RomsetsRow
        Dim romset As Parser.RomsetsRow()
        romset = ds.Romsets.Select("romsetName = '" + strRomsetName.Replace("'", "''") + "'")
        If romset.Length = 0 Then
            Return ds.Romsets.AddRomsetsRow(strRomsetName)
        Else
            Return romset(0)
        End If
    End Function
    Private Function GetFormat(strFormatName As String, ByRef ds As Parser) As Parser.FormatsRow
        Dim format As Parser.FormatsRow()
        format = ds.Formats.Select("formatName = '" + strFormatName.Replace("'", "''") + "'")
        If format.Length = 0 Then
            Return ds.Formats.AddFormatsRow(strFormatName)
        Else
            Return format(0)
        End If
    End Function
    Private Function GetFile(strFileName As String, software As Parser.SoftwaresRow, format As Parser.FormatsRow, romset As Parser.RomsetsRow, ByRef ds As Parser) As Parser.FilesRow
        Dim filerow As Parser.FilesRow()
        filerow = ds.Files.Select("softwareId = " + software.softwareId.ToString + " and formatId = " + format.formatId.ToString _
                                  + " and romsetId = " + romset.romsetId.ToString + " and fileName = '" + strFileName.Replace("'", "''") + "'")
        If filerow.Length = 0 Then
            Return ds.Files.AddFilesRow(strFileName, software, format, romset)
        Else
            Return filerow(0)
        End If
    End Function
    Private Function GetFileFlag(filerow As Parser.FilesRow, flagrow As Parser.FlagsRow, strFlagValue As String, ByRef ds As Parser)
        Dim fileflag As Parser.FileFlagsRow()
        fileflag = ds.FileFlags.Select("fileId = " + filerow.fileId.ToString + " and flagId = " + flagrow.flagId.ToString _
                                       + " and flagValue = '" + strFlagValue.Replace("'", "''") + "'")
        If fileflag.Length = 0 Then
            Return ds.FileFlags.AddFileFlagsRow(filerow, flagrow, strFlagValue)
        Else
            Return fileflag(0)
        End If
    End Function
    Private Function GetArchiveFile(filerow As Parser.FilesRow, strArchiveFileName As String, strArchiveFileExtension As String, ByRef ds As Parser)
        Dim archiveFile As Parser.ArchiveFilesRow()
        archiveFile = ds.ArchiveFiles.Select("fileId = " + filerow.fileId.ToString + " and archiveFileName = '" + strArchiveFileName.Replace("'", "''") + "'" _
                                             + " and archiveFileExtension = '" + strArchiveFileExtension.Replace("'", "''") + "'")
        If archiveFile.Length = 0 Then
            Return ds.ArchiveFiles.AddArchiveFilesRow(filerow, strArchiveFileName, strArchiveFileExtension)
        Else
            Return archiveFile(0)
        End If

    End Function
End Class