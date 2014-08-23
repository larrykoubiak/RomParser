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
                    fileFlag = GetFileFlag(filerow, flag, rgxVersion.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxDevStatus.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("DevStatus", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxVersion.Match(file.Name).Groups(1).Value, ds)
                End If
                If rgxLicense.IsMatch(file.Name) Then
                    Dim flag As Parser.FlagsRow = GetFlag("License", "File", ds)
                    fileFlag = GetFileFlag(filerow, flag, rgxVersion.Match(file.Name).Groups(1).Value, ds)
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
        Dim temp, manufacturer As Parser.ManufacturersRow
        manufacturer = Nothing
        For Each temp In ds.Manufacturers
            If temp.manufacturerName = strManufacturerName Then
                manufacturer = temp
                Exit For
            End If
        Next
        If IsNothing(manufacturer) Then
            manufacturer = ds.Manufacturers.AddManufacturersRow(strManufacturerName)
        End If
        Return manufacturer
    End Function
    Private Function GetSystem(strSystemName As String, ByRef ds As Parser) As Parser.SystemsRow
        Dim temp, system As Parser.SystemsRow
        system = Nothing
        For Each temp In ds.Systems
            If temp.systemName = strSystemName Then
                system = temp
                Exit For
            End If
        Next
        If IsNothing(system) Then
            system = ds.Systems.AddSystemsRow(strSystemName)
        End If
        Return system
    End Function

    Private Function GetSoftwareType(strTypeName As String, ByRef ds As Parser) As Parser.TypesRow
        Dim temp, type As Parser.TypesRow
        type = Nothing
        For Each temp In ds.Types
            If temp.typeName = strTypeName Then
                type = temp
                Exit For
            End If
        Next
        If IsNothing(type) Then
            type = ds.Types.AddTypesRow(strTypeName)
        End If
        Return type
    End Function
    Private Function GetSoftware(manufacturer As Parser.ManufacturersRow, system As Parser.SystemsRow, type As Parser.TypesRow, strSoftwareName As String, ByRef ds As Parser) As Parser.SoftwaresRow
        Dim temp, soft As Parser.SoftwaresRow
        soft = Nothing
        For Each temp In ds.Softwares
            If temp.ManufacturersRow.Equals(manufacturer) And temp.SystemsRow.Equals(system) And temp.TypesRow.Equals(type) And temp.softwareName = strSoftwareName Then
                soft = temp
                Exit For
            End If
        Next
        If IsNothing(soft) Then
            soft = ds.Softwares.AddSoftwaresRow(strSoftwareName, manufacturer, system, type)
        End If
        Return soft
    End Function
    Private Function GetFlag(strFlagName As String, strFlagType As String, ByRef ds As Parser) As Parser.FlagsRow
        Dim temp, flag As Parser.FlagsRow
        flag = Nothing
        For Each temp In ds.Flags
            If temp.flagName = strFlagName And temp.flagType = strFlagType Then
                flag = temp
                Exit For
            End If
        Next
        If IsNothing(flag) Then
            flag = ds.Flags.AddFlagsRow(strFlagName, strFlagType)
        End If
        Return flag
    End Function
    Private Function GetRomSet(strRomsetName As String, ByRef ds As Parser) As Parser.RomsetsRow
        Dim temp, romset As Parser.RomsetsRow
        romset = Nothing
        For Each temp In ds.Romsets
            If temp.romsetName = strRomsetName Then
                romset = temp
                Exit For
            End If
        Next
        If IsNothing(romset) Then
            romset = ds.Romsets.AddRomsetsRow(strRomsetName)
        End If
        Return romset
    End Function
    Private Function GetFormat(strFormatName As String, ByRef ds As Parser) As Parser.FormatsRow
        Dim temp, format As Parser.FormatsRow
        format = Nothing
        For Each temp In ds.Formats
            If temp.formatName = strFormatName Then
                format = temp
                Exit For
            End If
        Next
        If IsNothing(format) Then
            format = ds.Formats.AddFormatsRow(strFormatName)
        End If
        Return format
    End Function
    Private Function getFile(strFileName As String, software As Parser.SoftwaresRow, format As Parser.FormatsRow, romset As Parser.RomsetsRow, ByRef ds As Parser) As Parser.FilesRow
        Dim temp, filerow As Parser.FilesRow
        filerow = Nothing
        For Each temp In ds.Files
            If temp.fileName = strFileName And temp.SoftwaresRow.Equals(software) And temp.FormatsRow.Equals(format) And temp.RomsetsRow.Equals(romset) Then
                filerow = temp
                Exit For
            End If
        Next
        If IsNothing(filerow) Then
            filerow = ds.Files.AddFilesRow(strFileName, software, format, romset)
        End If
        Return filerow

    End Function
    Private Function GetFileFlag(filerow As Parser.FilesRow, flagrow As Parser.FlagsRow, strFlagValue As String, ByRef ds As Parser)
        Dim temp, fileflag As Parser.FileFlagsRow
        fileflag = Nothing
        For Each temp In ds.FileFlags
            If temp.FilesRow.Equals(filerow) And temp.FlagsRow.Equals(flagrow) And temp.flagValue = strFlagValue Then
                fileflag = temp
                Exit For
            End If
        Next
        If IsNothing(fileflag) Then
            fileflag = ds.FileFlags.AddFileFlagsRow(filerow, flagrow, strFlagValue)
        End If
        Return fileflag
    End Function
    Private Function GetArchiveFile(filerow As Parser.FilesRow, strArchiveFileName As String, strArchiveFileExtension As String, ByRef ds As Parser)
        Dim temp, archiveFile As Parser.ArchiveFilesRow
        archiveFile = Nothing
        For Each temp In ds.ArchiveFiles
            If temp.FilesRow.Equals(filerow) And temp.archiveFileName = strArchiveFileName And temp.archiveFileExtension = strArchiveFileExtension Then
                archiveFile = temp
                Exit For
            End If
        Next
        If IsNothing(archiveFile) Then
            archiveFile = ds.ArchiveFiles.AddArchiveFilesRow(filerow, strArchiveFileName, strArchiveFileExtension)
        End If
        Return archiveFile
    End Function
End Class