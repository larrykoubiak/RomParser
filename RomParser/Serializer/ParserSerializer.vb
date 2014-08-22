Imports System.Data.SQLite
Imports System.ComponentModel
Public Class ParserSerializer
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
    Private db As SQLiteConnection
    Private ds As DataSet
    Private caches As Hashtable
    Public Property DataSet As DataSet
        Get
            Return ds
        End Get
        Set(value As DataSet)
            ds = value
        End Set
    End Property

    Public Sub New(path As String)
        db = New SQLiteConnection("Data Source=" + path)
        db.Open()
        CreateParserTables()
        caches = New Hashtable
        ds = New DataSet("Parser")
    End Sub
    Public Sub SerializeList(ByRef list As List(Of ParserSoftware))
        Dim rom As ParserSoftware
        Dim id, nb As Integer
        Dim trans As SQLiteTransaction
        nb = list.Count
        id = 0
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        For Each rom In list
            Serialize(rom, True)
            id += 1
            RaiseEvent ItemAdded(rom.Manufacturer + " - " + rom.Platform, id, nb)
        Next
        trans.Commit()
    End Sub
    Public Function Serialize(sfSoftware As ParserSoftware, Optional firstLoad As Boolean = False) As Integer
        Dim cmd As SQLiteCommand
        Dim str As String
        Dim intManufacturerId, intSystemId, intTypeId As Integer
        Dim intRomsetId, intSoftwareId, intFormatId As Integer
        Dim intFlagId, intFileId, intSoftwareFlagId, intFileFlagId As Integer
        Dim intArchiveFileId As Integer
        Dim parameters As Hashtable
        Dim flag As ParserFlag
        Dim file As ParserZipFile
        Dim archiveFile As ParserArchiveFile
        Dim strFile As String
        If IsNothing(db) Then Return -1
        '1. write Software data
        intManufacturerId = GetId("manufacturer", sfSoftware.Manufacturer, True)
        intSystemId = GetId("system", sfSoftware.Platform, True)
        intTypeId = GetId("type", sfSoftware.ROMType, True)
        parameters = New Hashtable
        parameters.Add("systemId", intSystemId)
        intSoftwareId = GetId("software", sfSoftware.SoftwareName, False, parameters)
        If intSoftwareId = 0 Then
            str = "INSERT INTO tblSoftwares (softwareName,manufacturerId,systemId,typeId) VALUES ("
            str += """" + sfSoftware.SoftwareName + """," + intManufacturerId.ToString + "," + intSystemId.ToString + "," + intTypeId.ToString + ")"
            cmd = New SQLiteCommand(str, db)
            cmd.ExecuteNonQuery()
            intSoftwareId = db.LastInsertRowId
        End If
        '2. write software flag data
        For Each flag In sfSoftware.Flags
            parameters = New Hashtable
            parameters.Add("flagType", flag.FlagType)
            intFlagId = GetId("flag", flag.Name, False, parameters)
            If intFlagId = 0 Then
                str = "INSERT INTO tblFlags (flagName, flagType) VALUES ("
                str += """" + flag.Name + """,""" + flag.FlagType + """)"
                cmd = New SQLiteCommand(str, db)
                cmd.ExecuteNonQuery()
                intFlagId = db.LastInsertRowId
            End If
            parameters = New Hashtable
            parameters.Add("softwareId", intSoftwareId)
            parameters.Add("flagId", intFlagId)
            intSoftwareFlagId = GetId("softwareFlag", flag.Value, False, parameters, "flagValue")
            If intSoftwareFlagId = 0 Then
                str = "INSERT INTO tblSoftwareFlags (softwareId, flagId,flagValue) VALUES ("
                str += intSoftwareId.ToString + "," + intFlagId.ToString + ",""" + flag.Value + """)"
                cmd = New SQLiteCommand(str, db)
                cmd.ExecuteNonQuery()
                intSoftwareFlagId = db.LastInsertRowId
            End If
        Next
        '3. write software file data
        For Each file In sfSoftware.Files
            intRomsetId = GetId("romset", file.ROMSet, True)
            intFormatId = GetId("format", file.Format, True)
            If Not firstLoad Then
                parameters = New Hashtable
                parameters.Add("softwareId", intSoftwareId)
                parameters.Add("formatId", intFormatId)
                parameters.Add("romsetId", intRomsetId)
                intFileId = GetId("file", file.FileName, False, parameters)
            Else
                intFileId = 0
            End If
            If intFileId = 0 Then
                str = "INSERT INTO tblFiles (fileName, softwareId, formatId, romsetId) VALUES ("
                str += """" + file.FileName + """," + intSoftwareId.ToString + "," + intFormatId.ToString + "," + intRomsetId.ToString + ")"
                cmd = New SQLiteCommand(str, db)
                cmd.ExecuteNonQuery()
                intFileId = db.LastInsertRowId
            End If
            '4. write software file flags data
            For Each flag In file.Flags
                If Not firstLoad Then
                    parameters = New Hashtable
                    parameters.Add("fileId", intFileId)
                    parameters.Add("flagId", intFlagId)
                    intFileFlagId = GetId("fileFlag", flag.Value, False, parameters, "flagValue")
                Else
                    intFileFlagId = 0
                End If
                If intFileFlagId = 0 Then
                    str = "INSERT INTO tblFileFlags (fileId, flagId,flagValue) VALUES ("
                    str += intSoftwareId.ToString + "," + intFlagId.ToString + ",""" + flag.Value + """)"
                    cmd = New SQLiteCommand(str, db)
                    cmd.ExecuteNonQuery()
                    intFileFlagId = db.LastInsertRowId
                End If
            Next
            '5. write software archive file data
            For Each archiveFile In file.ArchiveFiles
                strFile = archiveFile.Name
                parameters = New Hashtable
                parameters.Add("fileid", intFileId)
                parameters.Add("archiveFileExtension", archiveFile.Extension)
                intArchiveFileId = GetId("archiveFile", strFile, False, parameters)
                If intArchiveFileId = 0 Then
                    str = "INSERT INTO tblArchiveFiles (fileId, archiveFileName,archiveFileExtension) VALUES ("
                    str += intFileId.ToString + ",""" + strFile + """,""" + archiveFile.Extension + """)"
                    cmd = New SQLiteCommand(str, db)
                    cmd.ExecuteNonQuery()
                    intArchiveFileId = db.LastInsertRowId
                End If
            Next
        Next
        Return intFileId
    End Function
    Public Sub Deserialize(ByRef lst As List(Of ParserSoftware))
        Dim soft As ParserSoftware
        Dim file As ParserZipFile
        Dim flag As ParserFlag
        Dim archiveFile As ParserArchiveFile
        Dim strSQL As String
        Dim softwareDA As SQLiteDataAdapter
        Dim filesDA As SQLiteDataAdapter
        Dim archiveFilesDA As SQLiteDataAdapter
        Dim softwareFlagsDA As SQLiteDataAdapter
        Dim fileFlagsDA As SQLiteDataAdapter
        Dim softwareDR, fileDR, softwareFlagDr, fileFlagDR, fileArchiveDR As DataRow
        Dim trans As SQLiteTransaction
        If IsNothing(db) Then Exit Sub
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        '1. Setup data adapters
        strSQL = "SELECT s.softwareId, s.softwareName, m.manufacturerName, p.systemName, t.typeName"
        strSQL += " FROM tblSoftwares s INNER JOIN tblManufacturers m on m.manufacturerId = s.manufacturerId"
        strSQL += " INNER JOIN tblSystems p on p.systemId = s.systemId "
        strSQL += " INNER JOIN tblTypes t on t.typeId = s.typeId"
        softwareDA = New SQLiteDataAdapter(strSQL, db)
        softwareDA.Fill(ds, "Softwares")
        strSQL = "select s.softwareFlagId, s.softwareId, f.flagName, s.flagValue, f.flagType"
        strSQL += " FROM tblSoftwareFlags s INNER JOIN tblFlags f on f.flagId = s.FlagId"
        softwareFlagsDA = New SQLiteDataAdapter(strSQL, db)
        softwareFlagsDA.Fill(ds, "SoftwareFlags")
        strSQL = "SELECT fileId, f.softwareId, fileName, formatName, romsetName "
        strSQL += " FROM tblFiles f INNER JOIN tblFormats o on o.formatId = f.formatId"
        strSQL += " INNER JOIN tblRomSets r on r.romsetId = f.romsetId"
        filesDA = New SQLiteDataAdapter(strSQL, db)
        filesDA.Fill(ds, "Files")
        strSQL = "select s.fileFlagId, s.FileId, f.flagName, s.flagValue, f.flagType"
        strSQL += " FROM tblFileFlags s INNER JOIN tblFlags f on f.flagId = s.FlagId"
        fileFlagsDA = New SQLiteDataAdapter(strSQL, db)
        fileFlagsDA.Fill(ds, "FileFlags")
        strSQL = "SELECT archiveFileId, fileId, archiveFileName, archiveFileExtension"
        strSQL += " FROM tblArchiveFiles"
        archiveFilesDA = New SQLiteDataAdapter(strSQL, db)
        archiveFilesDA.Fill(ds, "ArchiveFiles")
        '2. add data relations
        ds.Relations.Add("softwareFile", ds.Tables("Softwares").Columns("softwareId"), ds.Tables("Files").Columns("softwareId"))
        ds.Relations.Add("softwareFlag", ds.Tables("Softwares").Columns("softwareId"), ds.Tables("SoftwareFlags").Columns("softwareId"))
        ds.Relations.Add("fileFlag", ds.Tables("Files").Columns("fileId"), ds.Tables("FileFlags").Columns("fileId"))
        ds.Relations.Add("fileArchive", ds.Tables("Files").Columns("fileId"), ds.Tables("ArchiveFiles").Columns("fileId"))
        '3. read data from dataset
        For Each softwareDR In ds.Tables("Softwares").Rows
            soft = New ParserSoftware
            soft.SoftwareId = softwareDR("softwareId")
            soft.SoftwareName = softwareDR("softwareName")
            soft.Manufacturer = softwareDR("manufacturerName")
            soft.Platform = softwareDR("systemName")
            soft.ROMType = softwareDR("typeName")
            soft.Flags = New List(Of ParserFlag)
            For Each softwareFlagDr In softwareDR.GetChildRows("softwareFlag")
                flag = New ParserFlag()
                flag.Name = softwareFlagDr("flagName")
                flag.Value = softwareFlagDr("flagValue")
                flag.FlagType = softwareFlagDr("flagType")
                soft.Flags.Add(flag)
            Next
            soft.Files = New List(Of ParserZipFile)
            For Each fileDR In softwareDR.GetChildRows("softwareFile")
                file = New ParserZipFile
                file.FileName = fileDR("fileName")
                file.Format = fileDR("formatName")
                file.ROMSet = fileDR("romsetName")
                file.Flags = New List(Of ParserFlag)
                For Each fileFlagDR In fileDR.GetChildRows("fileFlag")
                    flag = New ParserFlag()
                    flag.Name = fileFlagDR("flagName")
                    flag.Value = fileFlagDR("flagValue")
                    flag.FlagType = fileFlagDR("flagType")
                    file.Flags.Add(flag)
                Next
                file.ArchiveFiles = New List(Of ParserArchiveFile)
                For Each fileArchiveDR In fileDR.GetChildRows("fileArchive")
                    archiveFile = New ParserArchiveFile
                    archiveFile.Name = fileArchiveDR("archiveFileName")
                    archiveFile.Extension = fileArchiveDR("archiveFileExtension")
                    file.ArchiveFiles.Add(archiveFile)
                Next
                soft.Files.Add(file)
            Next
            RaiseEvent ItemAdded(soft.Manufacturer + " - " + soft.Platform, softwareDR("softwareId"), ds.Tables("Softwares").Rows.Count)
            lst.Add(soft)
        Next
        trans.Commit()
    End Sub
    Private Function GetId(name As String, value As String, insert As Boolean, Optional parameters As Hashtable = Nothing, _
                           Optional valueField As String = "") As Integer
        Dim cmd As SQLiteCommand
        Dim id As Integer
        Dim strSQL As String
        Dim cache As Hashtable
        'init caches if needed
        If Not caches.Contains(name) Then
            cache = New Hashtable
            caches.Add(name, cache)
        Else
            cache = caches(name)
        End If
        If cache.Contains(value) Then
            Return cache(value)
        Else
            If valueField = "" Then
                valueField = name + "Name"
            End If
            strSQL = "SELECT " + name + "Id FROM tbl" + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name) + "s WHERE " + valueField + " = """ + value + """"
            If Not IsNothing(parameters) Then
                Dim param As DictionaryEntry
                Dim i As Integer
                For Each param In parameters
                    strSQL += " AND " + param.Key + " = """ + param.Value.ToString() + """"
                    i += 1
                Next
            End If
            cmd = New SQLiteCommand(strSQL, db)
            id = cmd.ExecuteScalar
            If id = 0 And insert Then
                strSQL = "INSERT INTO tbl" + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name) + "s (" + name + "Name) VALUES (""" + value + """)"
                cmd = New SQLiteCommand(strSQL, db)
                cmd.ExecuteNonQuery()
                id = db.LastInsertRowId()
            End If
            If id <> 0 Then
                cache.Add(value, id)
            End If
            cmd.Dispose()
            Return id
        End If
    End Function
    Private Sub CreateParserTables()
        Dim cmd As SQLiteCommand
        If Not CheckTableExists("tblRomsets") Then
            cmd = New SQLiteCommand("CREATE TABLE tblRomsets (romsetId INTEGER PRIMARY KEY, romsetName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblManufacturers") Then
            cmd = New SQLiteCommand("CREATE TABLE tblManufacturers (manufacturerId INTEGER PRIMARY KEY, manufacturerName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblSystems") Then
            cmd = New SQLiteCommand("CREATE TABLE tblSystems (systemId INTEGER PRIMARY KEY, systemName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblTypes") Then
            cmd = New SQLiteCommand("CREATE TABLE tblTypes (typeId INTEGER PRIMARY KEY, typeName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblFormats") Then
            cmd = New SQLiteCommand("CREATE TABLE tblFormats (formatId INTEGER PRIMARY KEY, formatName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblFlags") Then
            cmd = New SQLiteCommand("CREATE TABLE tblFlags (flagId INTEGER PRIMARY KEY, flagName TEXT, flagType TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblSoftwares") Then
            cmd = New SQLiteCommand("CREATE TABLE tblSoftwares (softwareId INTEGER PRIMARY KEY, softwareName TEXT, manufacturerId INTEGER, systemId INTEGER, typeId INTEGER)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxManufacturer ON tblSoftwares(manufacturerId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxSystem ON tblSoftwares(systemId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxType ON tblSoftwares(typeId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblSoftwareFlags") Then
            cmd = New SQLiteCommand("CREATE TABLE tblSoftwareFlags (softwareFlagId INTEGER PRIMARY KEY, softwareId INTEGER, flagId INTEGER, flagValue TEXT)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxFlagSoftware ON tblSoftwareFlags(softwareId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxSoftwareFlag ON tblSoftwareFlags(flagId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblFiles") Then
            cmd = New SQLiteCommand("CREATE TABLE tblFiles (fileId INTEGER PRIMARY KEY, fileName TEXT, softwareId INTEGER, formatId INTEGER, romsetId INTEGER)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxFileSoftware ON tblFiles(softwareId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxFormat ON tblFiles(formatId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxRomset ON tblFiles(romsetId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblFileFlags") Then
            cmd = New SQLiteCommand("CREATE TABLE tblFileFlags (fileFlagId INTEGER PRIMARY KEY, fileId INTEGER, flagId INTEGER, flagValue TEXT)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxFlagFile ON tblFileFlags(fileId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxFileFlag ON tblFileFlags(flagId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblArchiveFiles") Then
            cmd = New SQLiteCommand("CREATE TABLE tblArchiveFiles (archiveFileId INTEGER PRIMARY KEY, fileId INTEGER, archiveFileName TEXT, archiveFileExtension TEXT)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxArchiveFile ON tblArchiveFiles(fileId)", db)
            cmd.ExecuteNonQuery()
        End If
    End Sub
    Private Function CheckTableExists(tblName As String) As Boolean
        Dim cmd As SQLiteCommand
        Dim str As String
        If IsNothing(db) Then Return False
        'check if tblFile exists
        cmd = New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' and name='" + tblName + "'", db)
        str = cmd.ExecuteScalar()
        If IsNothing(str) Then
            Return False
        Else
            Return True
        End If
    End Function
End Class
