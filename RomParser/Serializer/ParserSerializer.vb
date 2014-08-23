Imports System.Data.SQLite
Imports System.ComponentModel
Public Class ParserSerializer
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
    Private db As SQLiteConnection
    Private softwareDA As SQLiteDataAdapter
    Private manufacturersDA As SQLiteDataAdapter
    Private systemsDA As SQLiteDataAdapter
    Private typesDA As SQLiteDataAdapter
    Private flagsDA As SQLiteDataAdapter
    Private softwareFlagsDA As SQLiteDataAdapter
    Private filesDA As SQLiteDataAdapter
    Private formatsDA As SQLiteDataAdapter
    Private romsetsDA As SQLiteDataAdapter
    Private fileFlagsDA As SQLiteDataAdapter
    Private archiveFilesDA As SQLiteDataAdapter
    Public Sub New(path As String, ByRef ds As Parser)
        db = New SQLiteConnection("Data Source=" + path)
        CreateParserTables()
        Deserialize(ds)
    End Sub
    Public Sub SerializeList(ByRef ds As Parser)
        Dim dt As DataTable
        db.Open()
        dt = ds.Manufacturers.GetChanges()
        If Not IsNothing(dt) Then manufacturersDA.Update(dt)
        dt = ds.Systems.GetChanges
        If Not IsNothing(dt) Then systemsDA.Update(dt)
        dt = ds.Types.GetChanges()
        If Not IsNothing(dt) Then typesDA.Update(dt)
        dt = ds.Softwares.GetChanges()
        If Not IsNothing(dt) Then softwareDA.Update(dt)
        dt = ds.Flags.GetChanges
        If Not IsNothing(dt) Then flagsDA.Update(dt)
        dt = ds.SoftwareFlags.GetChanges
        If Not IsNothing(dt) Then softwareFlagsDA.Update(dt)
        dt = ds.Formats.GetChanges()
        If Not IsNothing(dt) Then formatsDA.Update(dt)
        dt = ds.Romsets.GetChanges
        If Not IsNothing(dt) Then romsetsDA.Update(dt)
        dt = ds.Files.GetChanges
        If Not IsNothing(dt) Then filesDA.Update(dt)
        dt = ds.FileFlags.GetChanges
        If Not IsNothing(dt) Then fileFlagsDA.Update(dt)
        dt = ds.ArchiveFiles.GetChanges
        If Not IsNothing(dt) Then archiveFilesDA.Update(dt)
        db.Close()
    End Sub
    Public Sub Deserialize(ByRef ds As Parser)
        Dim trans As SQLiteTransaction
        db.Open()
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        '1. Setup data adapters
        softwareDA = New SQLiteDataAdapter
        softwareDA.SelectCommand = New SQLiteCommand("SELECT s.softwareId, s.softwareName, s.manufacturerId, s.systemId, s.typeId FROM tblSoftwares s", db)
        softwareDA.InsertCommand = New SQLiteCommand("INSERT INTO tblSoftwares (softwareName,manufacturerId, systemId, typeId) VALUES (@softwareName,@manufacturerId,@systemId,@typeId)", db)
        softwareDA.InsertCommand.Parameters.Add("@softwareName", DbType.String, 255, "softwareName")
        softwareDA.InsertCommand.Parameters.Add("@manufacturerId", DbType.String, 255, "manufacturerId")
        softwareDA.InsertCommand.Parameters.Add("@systemId", DbType.String, 255, "systemId")
        softwareDA.InsertCommand.Parameters.Add("@typeId", DbType.String, 255, "typeId")
        softwareDA.UpdateCommand = New SQLiteCommand("UPDATE tblSoftwares set softwareName=@softwareName,manufacturerId=@manufacturerId, systemId=@systemId, typeId=@typeId) WHERE softwareId=@softwareId", db)
        softwareDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblSoftwares WHERE softwareId=@softwareId", db)
        softwareDA.Fill(ds, "Softwares")
        manufacturersDA = New SQLiteDataAdapter
        manufacturersDA.SelectCommand = New SQLiteCommand("SELECT m.manufacturerId, m.manufacturerName FROM tblManufacturers m", db)
        manufacturersDA.InsertCommand = New SQLiteCommand("INSERT INTO tblManufacturers (manufacturerName) VALUES (@manufacturerName)", db)
        manufacturersDA.InsertCommand.Parameters.Add("@manufacturerName", DbType.String, 255, "manufacturerName")
        manufacturersDA.UpdateCommand = New SQLiteCommand("UPDATE tblManufacturers SET manufacturerName=@manufacturerName WHERE manufacturerId=@manufacturerId", db)
        manufacturersDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblManufacturers WHERE manufacturerId=@manufacturerId", db)
        manufacturersDA.Fill(ds, "Manufacturers")
        systemsDA = New SQLiteDataAdapter
        systemsDA.SelectCommand = New SQLiteCommand("SELECT s.systemId, s.systemName FROM tblSystems s", db)
        systemsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblSystems (systemName) VALUES (@systemName)", db)
        systemsDA.InsertCommand.Parameters.Add("@systemName", DbType.String, 255, "systemName")
        systemsDA.UpdateCommand = New SQLiteCommand("UPDATE tblSystems SET systemName=@systemName WHERE systemId=@systemId", db)
        systemsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblSystems WHERE systemId=@systemId", db)
        systemsDA.Fill(ds, "Systems")
        typesDA = New SQLiteDataAdapter
        typesDA.SelectCommand = New SQLiteCommand("SELECT t.typeId, t.typeName FROM tblTypes t", db)
        typesDA.InsertCommand = New SQLiteCommand("INSERT INTO tblTypes (typeName) VALUES (@typeName)", db)
        typesDA.InsertCommand.Parameters.Add("@typeName", DbType.String, 255, "typeName")
        typesDA.UpdateCommand = New SQLiteCommand("UPDATE tblTypes SET typeName=@typeName WHERE typeId=@typeId", db)
        typesDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblTypes WHERE typeId=@typeId", db)
        typesDA.Fill(ds, "Types")
        flagsDA = New SQLiteDataAdapter
        flagsDA.SelectCommand = New SQLiteCommand("SELECT f.flagId, f.flagName, f.flagType FROM tblFlags f", db)
        flagsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFlags (flagName,flagType) VALUES (@flagName,@flagType)", db)
        flagsDA.UpdateCommand = New SQLiteCommand("UPDATE tblFlags SET flagName=@flagName, flagType=@flagType WHERE flagId=@flagId", db)
        flagsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFlags WHERE flagId=@flagId", db)
        flagsDA.Fill(ds, "Flags")
        softwareFlagsDA = New SQLiteDataAdapter
        softwareFlagsDA.SelectCommand = New SQLiteCommand("SELECT sf.softwareFlagId, sf.softwareId, sf.flagId, sf.flagValue FROM tblSoftwareFlags sf", db)
        softwareFlagsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblSoftwareFlags (softwareId,flagId,flagValue) VALUES (@softwareId,@flagId,@flagValue)", db)
        softwareFlagsDA.UpdateCommand = New SQLiteCommand("UPDATE tblSoftwareFlags SET softwareId=@softwareId, flagId=@flagId, flagValue=@flagValue WHERE softwareFlagId=@softwareFlagId", db)
        softwareFlagsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblSoftwareFlags WHERE softwareFlagId=@softwareFlagId", db)
        softwareFlagsDA.Fill(ds, "SoftwareFlags")
        filesDA = New SQLiteDataAdapter
        filesDA.SelectCommand = New SQLiteCommand("SELECT f.fileId, f.fileName, f.softwareId, f.formatId, f.romsetId FROM tblFiles f", db)
        filesDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFiles (fileName,softwareId, formatId, romsetId) VALUES (@fileName,@softwareId,@formatId,@romsetId", db)
        filesDA.UpdateCommand = New SQLiteCommand("UPDATE tblFiles set fileName=@fileName,softwareId=@softwareId, formatId=@formatId, romsetId=@romsetId) WHERE fileId=@fileId", db)
        filesDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFiles WHERE fileId=@fileId", db)
        filesDA.Fill(ds, "Files")
        formatsDA = New SQLiteDataAdapter
        formatsDA.SelectCommand = New SQLiteCommand("SELECT f.formatId, f.formatName FROM tblFormats f", db)
        formatsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFormats (formatName) VALUES (@formatName)", db)
        formatsDA.UpdateCommand = New SQLiteCommand("UPDATE tblFormats SET formatName=@formatName WHERE formatId=@formatId", db)
        formatsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFormats WHERE formatId=@formatId", db)
        formatsDA.Fill(ds, "Formats")
        romsetsDA = New SQLiteDataAdapter
        romsetsDA.SelectCommand = New SQLiteCommand("SELECT r.romsetId, r.romsetName FROM tblRomsets r", db)
        romsetsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblRomsets (romsetName) VALUES (@romsetName)", db)
        romsetsDA.UpdateCommand = New SQLiteCommand("UPDATE tblRomsets SET romsetName=@romsetName WHERE romsetId=@romsetId", db)
        romsetsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblRomsets WHERE romsetId=@romsetId", db)
        romsetsDA.Fill(ds, "Romsets")
        fileFlagsDA = New SQLiteDataAdapter
        fileFlagsDA.SelectCommand = New SQLiteCommand("SELECT f.fileFlagId, f.fileId, f.flagId, f.flagValue FROM tblFileFlags f", db)
        fileFlagsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFileFlags (fileId,flagId,flagValue) VALUES (@fileId,@flagId,@flagValue)", db)
        fileFlagsDA.UpdateCommand = New SQLiteCommand("UPDATE tblFileFlags SET fileId=@fileId, flagId=@flagId, flagValue=@flagValue WHERE fileFlagId=@fileFlagId", db)
        fileFlagsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFileFlags WHERE fileFlagId=@fileFlagId", db)
        fileFlagsDA.Fill(ds, "FileFlags")
        archiveFilesDA = New SQLiteDataAdapter
        archiveFilesDA.SelectCommand = New SQLiteCommand("SELECT a.archiveFileId, a.fileId, a.archiveFileName, a.archiveFileExtension FROM tblArchiveFiles a", db)
        archiveFilesDA.InsertCommand = New SQLiteCommand("INSERT INTO tblArchiveFiles (fileId, archiveFileName, archiveFileExtension) VALUES (@fileId, @archiveFileName, @archiveFileExtension)", db)
        archiveFilesDA.UpdateCommand = New SQLiteCommand("UPDATE tblArchiveFiles SET fileId, archiveFileName, archiveFileExtension WHERE archiveFileId=@archiveFileId", db)
        archiveFilesDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblArchiveFiles WHERE archiveFileId=@archiveFileId", db)
        archiveFilesDA.Fill(ds, "ArchiveFiles")
        '2. add data relations
        'ds.Relations.Add("manufacturerSoftware", ds.Tables("Manufacturers").Columns("manufacturerId"), ds.Tables("Softwares").Columns("manufacturerId"))
        'ds.Relations.Add("systemSoftware", ds.Tables("Systems").Columns("systemId"), ds.Tables("Softwares").Columns("systemId"))
        'ds.Relations.Add("typeSoftware", ds.Tables("Types").Columns("typeId"), ds.Tables("Softwares").Columns("typeId"))
        'ds.Relations.Add("softwareFlag", ds.Tables("Softwares").Columns("softwareId"), ds.Tables("SoftwareFlags").Columns("softwareId"))
        'ds.Relations.Add("flagSoftwareFlag", ds.Tables("Flags").Columns("flagId"), ds.Tables("SoftwareFlags").Columns("flagId"))
        'ds.Relations.Add("softwareFile", ds.Tables("Softwares").Columns("softwareId"), ds.Tables("Files").Columns("softwareId"))
        'ds.Relations.Add("formatFile", ds.Tables("Formats").Columns("formatId"), ds.Tables("Files").Columns("formatId"))
        'ds.Relations.Add("romsetFile", ds.Tables("Romsets").Columns("romsetId"), ds.Tables("Files").Columns("romsetId"))
        'ds.Relations.Add("fileFlag", ds.Tables("Files").Columns("fileId"), ds.Tables("FileFlags").Columns("fileId"))
        'ds.Relations.Add("flagFileFlag", ds.Tables("Flags").Columns("flagId"), ds.Tables("FileFlags").Columns("flagId"))
        'ds.Relations.Add("fileArchive", ds.Tables("Files").Columns("fileId"), ds.Tables("ArchiveFiles").Columns("fileId"))
        trans.Commit()
        db.Close()
        ds.WriteXmlSchema("C:\\Temp\\parser.xsd")
    End Sub

    Private Sub CreateParserTables()
        Dim cmd As SQLiteCommand
        db.Open()
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
        db.Close()
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
