Imports System.IO
Imports System.Data.SQLite
Imports System.ComponentModel
Imports System.Xml.Serialization
Public MustInherit Class FileParser
    Protected db As SQLiteConnection
    Protected WithEvents softwareDA As SQLiteDataAdapter
    Protected WithEvents manufacturersDA As SQLiteDataAdapter
    Protected WithEvents systemsDA As SQLiteDataAdapter
    Protected WithEvents typesDA As SQLiteDataAdapter
    Protected WithEvents flagsDA As SQLiteDataAdapter
    Protected WithEvents softwareFlagsDA As SQLiteDataAdapter
    Protected WithEvents filesDA As SQLiteDataAdapter
    Protected WithEvents formatsDA As SQLiteDataAdapter
    Protected WithEvents romsetsDA As SQLiteDataAdapter
    Protected WithEvents fileFlagsDA As SQLiteDataAdapter
    Protected WithEvents archiveFilesDA As SQLiteDataAdapter
    Protected systemSoftwaresHT As Hashtable
    Protected softwareFlagsHT As Hashtable
    Protected softwareFilesHT As Hashtable
    Protected fileFlagsHT As Hashtable
    Protected fileArchiveHT As Hashtable
    Protected nbSoftwares, currentId As Integer
    Public MustOverride Sub ParsePath(dir As DirectoryInfo, ByRef ds As Parser)
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
    Protected Sub OnItemAdded(strLabel As String, intId As Integer, intCount As Integer)
        RaiseEvent ItemAdded(strLabel, intId, intCount)
    End Sub
    Public Sub Serialize(ByRef ds As Parser)
        Dim trans As SQLiteTransaction
        db.Open()
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        manufacturersDA.Update(ds.Manufacturers)
        systemsDA.Update(ds.Systems)
        typesDA.Update(ds.Types)
        flagsDA.Update(ds.Flags)
        If IsNothing(ds.Softwares.GetChanges) Then
            nbSoftwares = 0
        Else
            nbSoftwares = ds.Softwares.GetChanges.Rows.Count()
        End If
        currentId = 0
        softwareDA.Update(ds.Softwares)
        softwareFlagsDA.Update(ds.SoftwareFlags)

        formatsDA.Update(ds.Formats)
        romsetsDA.Update(ds.Romsets)

        filesDA.Update(ds.Files)
        fileFlagsDA.Update(ds.FileFlags)
        archiveFilesDA.Update(ds.ArchiveFiles)
        trans.Commit()
        db.Close()
    End Sub
    Public Sub Deserialize(ByRef ds As Parser)
        Dim trans As SQLiteTransaction
        db.Open()
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        '1. manufacturers data adapter
        manufacturersDA.Fill(ds, "Manufacturers")
        '2. systems data adapter
        systemsDA.Fill(ds, "Systems")
        '3. types data adapter
        typesDA.Fill(ds, "Types")
        '4. softwares data adapter
        softwareDA.Fill(ds, "Softwares")
        '5. flags data adapter
        flagsDA.Fill(ds, "Flags")
        '6. softwareFlags data adapter
        softwareFlagsDA.Fill(ds, "SoftwareFlags")
        '7. formats data adapter
        formatsDA.Fill(ds, "Formats")
        '8. romsets data adapter
        romsetsDA.Fill(ds, "Romsets")
        '9. files data adapter
        filesDA.Fill(ds, "Files")
        '10. fileFlags data adapter
        fileFlagsDA.Fill(ds, "FileFlags")
        '11. archiveFiles data adapter
        archiveFilesDA.Fill(ds, "ArchiveFiles")
        trans.Commit()
        db.Close()
    End Sub
    Protected Sub SetupDataAdapters()
        manufacturersDA = New SQLiteDataAdapter
        manufacturersDA.SelectCommand = New SQLiteCommand("SELECT m.manufacturerId, m.manufacturerName FROM tblManufacturers m", db)
        manufacturersDA.InsertCommand = New SQLiteCommand("INSERT INTO tblManufacturers (manufacturerName) VALUES (@manufacturerName)", db)
        manufacturersDA.InsertCommand.Parameters.Add("@manufacturerName", DbType.String, 255, "manufacturerName")
        manufacturersDA.UpdateCommand = New SQLiteCommand("UPDATE tblManufacturers SET manufacturerName=@manufacturerName WHERE manufacturerId=@manufacturerId", db)
        manufacturersDA.UpdateCommand.Parameters.Add("@manufacturerName", DbType.String, 255, "manufacturerName")
        manufacturersDA.UpdateCommand.Parameters.Add("@manufacturerId", DbType.Int64, 0, "manufacturerId")
        manufacturersDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblManufacturers WHERE manufacturerId=@manufacturerId", db)
        manufacturersDA.DeleteCommand.Parameters.Add("@manufacturerId", DbType.Int64, 0, "manufacturerId")
        '2. systems data adapter
        systemsDA = New SQLiteDataAdapter
        systemsDA.SelectCommand = New SQLiteCommand("SELECT s.systemId, s.systemName, s.manufacturerId  FROM tblSystems s", db)
        systemsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblSystems (systemName, manufacturerId) VALUES (@systemName,@manufacturerId)", db)
        systemsDA.InsertCommand.Parameters.Add("@systemName", DbType.String, 255, "systemName")
        systemsDA.InsertCommand.Parameters.Add("@manufacturerId", DbType.Int64, 0, "manufacturerId")
        systemsDA.UpdateCommand = New SQLiteCommand("UPDATE tblSystems SET systemName=@systemName, manufacturerId=@manufacturerId WHERE systemId=@systemId", db)
        systemsDA.UpdateCommand.Parameters.Add("@systemName", DbType.String, 255, "systemName")
        systemsDA.UpdateCommand.Parameters.Add("@manufacturerId", DbType.Int64, 0, "manufacturerId")
        systemsDA.UpdateCommand.Parameters.Add("@systemId", DbType.Int64, 0, "systemId")
        systemsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblSystems WHERE systemId=@systemId", db)
        systemsDA.DeleteCommand.Parameters.Add("@systemId", DbType.Int64, 0, "systemId")
        '3. types data adapter
        typesDA = New SQLiteDataAdapter
        typesDA.SelectCommand = New SQLiteCommand("SELECT t.typeId, t.typeName FROM tblTypes t", db)
        typesDA.InsertCommand = New SQLiteCommand("INSERT INTO tblTypes (typeName) VALUES (@typeName)", db)
        typesDA.InsertCommand.Parameters.Add("@typeName", DbType.String, 255, "typeName")
        typesDA.UpdateCommand = New SQLiteCommand("UPDATE tblTypes SET typeName=@typeName WHERE typeId=@typeId", db)
        typesDA.UpdateCommand.Parameters.Add("@typeName", DbType.String, 255, "typeName")
        typesDA.UpdateCommand.Parameters.Add("@typeId", DbType.Int64, 0, "typeId")
        typesDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblTypes WHERE typeId=@typeId", db)
        typesDA.DeleteCommand.Parameters.Add("@typeId", DbType.Int64, 0, "typeId")
        '4. softwares data adapter
        softwareDA = New SQLiteDataAdapter
        softwareDA.SelectCommand = New SQLiteCommand("SELECT s.softwareId, s.softwareName, s.systemId, s.typeId FROM tblSoftwares s", db)
        softwareDA.InsertCommand = New SQLiteCommand("INSERT INTO tblSoftwares (softwareName,systemId, typeId) VALUES (@softwareName,@systemId,@typeId)", db)
        softwareDA.InsertCommand.Parameters.Add("@softwareName", DbType.String, 255, "softwareName")
        softwareDA.InsertCommand.Parameters.Add("@systemId", DbType.Int64, 0, "systemId")
        softwareDA.InsertCommand.Parameters.Add("@typeId", DbType.Int64, 0, "typeId")
        softwareDA.UpdateCommand = New SQLiteCommand("UPDATE tblSoftwares set softwareName=@softwareName,systemId=@systemId, typeId=@typeId WHERE softwareId=@softwareId", db)
        softwareDA.InsertCommand.Parameters.Add("@softwareName", DbType.String, 255, "softwareName")
        softwareDA.InsertCommand.Parameters.Add("@systemId", DbType.Int64, 0, "systemId")
        softwareDA.InsertCommand.Parameters.Add("@typeId", DbType.Int64, 0, "typeId")
        softwareDA.InsertCommand.Parameters.Add("@softwareId", DbType.Int64, 0, "softwareId")
        softwareDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblSoftwares WHERE softwareId=@softwareId", db)
        softwareDA.DeleteCommand.Parameters.Add("@softwareId", DbType.Int64, 0, "softwareId")
        '5. flags data adapter
        flagsDA = New SQLiteDataAdapter
        flagsDA.SelectCommand = New SQLiteCommand("SELECT f.flagId, f.flagName, f.flagType FROM tblFlags f", db)
        flagsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFlags (flagName,flagType) VALUES (@flagName,@flagType)", db)
        flagsDA.InsertCommand.Parameters.Add("@flagName", DbType.String, 255, "flagName")
        flagsDA.InsertCommand.Parameters.Add("@flagType", DbType.String, 255, "flagType")
        flagsDA.UpdateCommand = New SQLiteCommand("UPDATE tblFlags SET flagName=@flagName, flagType=@flagType WHERE flagId=@flagId", db)
        flagsDA.UpdateCommand.Parameters.Add("@flagName", DbType.String, 255, "flagName")
        flagsDA.UpdateCommand.Parameters.Add("@flagType", DbType.String, 255, "flagType")
        flagsDA.UpdateCommand.Parameters.Add("@flagId", DbType.Int64, 0, "flagId")
        flagsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFlags WHERE flagId=@flagId", db)
        flagsDA.DeleteCommand.Parameters.Add("@flagId", DbType.Int64, 0, "flagId")
        '6. softwareFlags data adapter
        softwareFlagsDA = New SQLiteDataAdapter
        softwareFlagsDA.SelectCommand = New SQLiteCommand("SELECT sf.softwareFlagId, sf.softwareId, sf.flagId, sf.flagValue FROM tblSoftwareFlags sf", db)
        softwareFlagsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblSoftwareFlags (softwareId,flagId,flagValue) VALUES (@softwareId,@flagId,@flagValue)", db)
        softwareFlagsDA.InsertCommand.Parameters.Add("@softwareId", DbType.Int64, 0, "softwareId")
        softwareFlagsDA.InsertCommand.Parameters.Add("@flagId", DbType.Int64, 0, "flagId")
        softwareFlagsDA.InsertCommand.Parameters.Add("@flagValue", DbType.String, 255, "flagValue")
        softwareFlagsDA.UpdateCommand = New SQLiteCommand("UPDATE tblSoftwareFlags SET softwareId=@softwareId, flagId=@flagId, flagValue=@flagValue WHERE softwareFlagId=@softwareFlagId", db)
        softwareFlagsDA.UpdateCommand.Parameters.Add("@softwareId", DbType.Int64, 0, "softwareId")
        softwareFlagsDA.UpdateCommand.Parameters.Add("@flagId", DbType.Int64, 0, "flagId")
        softwareFlagsDA.UpdateCommand.Parameters.Add("@flagValue", DbType.String, 255, "flagValue")
        softwareFlagsDA.UpdateCommand.Parameters.Add("@softwareFlagId", DbType.Int64, 0, "softwareFlagId")
        softwareFlagsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblSoftwareFlags WHERE softwareFlagId=@softwareFlagId", db)
        softwareFlagsDA.DeleteCommand.Parameters.Add("@softwareFlagId", DbType.Int64, 0, "softwareFlagId")
        '7. formats data adapter
        formatsDA = New SQLiteDataAdapter
        formatsDA.SelectCommand = New SQLiteCommand("SELECT f.formatId, f.formatName FROM tblFormats f", db)
        formatsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFormats (formatName) VALUES (@formatName)", db)
        formatsDA.InsertCommand.Parameters.Add("@formatName", DbType.String, 255, "formatName")
        formatsDA.UpdateCommand = New SQLiteCommand("UPDATE tblFormats SET formatName=@formatName WHERE formatId=@formatId", db)
        formatsDA.UpdateCommand.Parameters.Add("@formatName", DbType.String, 255, "formatName")
        formatsDA.UpdateCommand.Parameters.Add("@formatId", DbType.Int64, 0, "formatId")
        formatsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFormats WHERE formatId=@formatId", db)
        formatsDA.DeleteCommand.Parameters.Add("@formatId", DbType.Int64, 0, "formatId")
        '8. romsets data adapter
        romsetsDA = New SQLiteDataAdapter
        romsetsDA.SelectCommand = New SQLiteCommand("SELECT r.romsetId, r.romsetName FROM tblRomsets r", db)
        romsetsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblRomsets (romsetName) VALUES (@romsetName)", db)
        romsetsDA.InsertCommand.Parameters.Add("@romsetName", DbType.String, 255, "romsetName")
        romsetsDA.UpdateCommand = New SQLiteCommand("UPDATE tblRomsets SET romsetName=@romsetName WHERE romsetId=@romsetId", db)
        romsetsDA.UpdateCommand.Parameters.Add("@romsetName", DbType.String, 255, "romsetName")
        romsetsDA.UpdateCommand.Parameters.Add("@romsetId", DbType.Int64, 0, "romsetId")
        romsetsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblRomsets WHERE romsetId=@romsetId", db)
        romsetsDA.DeleteCommand.Parameters.Add("@romsetId", DbType.Int64, 0, "romsetId")
        '9. files data adapter
        filesDA = New SQLiteDataAdapter
        filesDA.SelectCommand = New SQLiteCommand("SELECT f.fileId, f.fileName, f.softwareId, f.formatId, f.romsetId FROM tblFiles f", db)
        filesDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFiles (fileName,softwareId, formatId, romsetId) VALUES (@fileName,@softwareId,@formatId,@romsetId)", db)
        filesDA.InsertCommand.Parameters.Add("@fileName", DbType.String, 255, "fileName")
        filesDA.InsertCommand.Parameters.Add("@softwareId", DbType.Int64, 0, "softwareId")
        filesDA.InsertCommand.Parameters.Add("@formatId", DbType.Int64, 0, "formatId")
        filesDA.InsertCommand.Parameters.Add("@romsetId", DbType.Int64, 0, "romsetId")
        filesDA.UpdateCommand = New SQLiteCommand("UPDATE tblFiles set fileName=@fileName,softwareId=@softwareId, formatId=@formatId, romsetId=@romsetId) WHERE fileId=@fileId", db)
        filesDA.UpdateCommand.Parameters.Add("@fileName", DbType.String, 255, "fileName")
        filesDA.UpdateCommand.Parameters.Add("@softwareId", DbType.Int64, 0, "softwareId")
        filesDA.UpdateCommand.Parameters.Add("@formatId", DbType.Int64, 0, "formatId")
        filesDA.UpdateCommand.Parameters.Add("@romsetId", DbType.Int64, 0, "romsetId")
        filesDA.UpdateCommand.Parameters.Add("@fileId", DbType.Int64, 0, "fileId")
        filesDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFiles WHERE fileId=@fileId", db)
        filesDA.DeleteCommand.Parameters.Add("@fileId", DbType.Int64, 0, "fileId")
        '10. fileFlags data adapter
        fileFlagsDA = New SQLiteDataAdapter
        fileFlagsDA.SelectCommand = New SQLiteCommand("SELECT f.fileFlagId, f.fileId, f.flagId, f.flagValue FROM tblFileFlags f", db)
        fileFlagsDA.InsertCommand = New SQLiteCommand("INSERT INTO tblFileFlags (fileId,flagId,flagValue) VALUES (@fileId,@flagId,@flagValue)", db)
        fileFlagsDA.InsertCommand.Parameters.Add("@fileId", DbType.Int64, 0, "fileId")
        fileFlagsDA.InsertCommand.Parameters.Add("@flagId", DbType.Int64, 0, "flagId")
        fileFlagsDA.InsertCommand.Parameters.Add("@flagValue", DbType.String, 255, "flagValue")
        fileFlagsDA.UpdateCommand = New SQLiteCommand("UPDATE tblFileFlags SET fileId=@fileId, flagId=@flagId, flagValue=@flagValue WHERE fileFlagId=@fileFlagId", db)
        fileFlagsDA.UpdateCommand.Parameters.Add("@fileId", DbType.Int64, 0, "fileId")
        fileFlagsDA.UpdateCommand.Parameters.Add("@flagId", DbType.Int64, 0, "flagId")
        fileFlagsDA.UpdateCommand.Parameters.Add("@flagValue", DbType.String, 255, "flagValue")
        fileFlagsDA.UpdateCommand.Parameters.Add("@fileFlagId", DbType.Int64, 0, "fileFlagId")
        fileFlagsDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblFileFlags WHERE fileFlagId=@fileFlagId", db)
        fileFlagsDA.DeleteCommand.Parameters.Add("@fileFlagId", DbType.Int64, 0, "fileFlagId")
        '11. archiveFiles data adapter
        archiveFilesDA = New SQLiteDataAdapter
        archiveFilesDA.SelectCommand = New SQLiteCommand("SELECT a.archiveFileId, a.fileId, a.archiveFileName, a.archiveFileExtension FROM tblArchiveFiles a", db)
        archiveFilesDA.InsertCommand = New SQLiteCommand("INSERT INTO tblArchiveFiles (fileId, archiveFileName, archiveFileExtension) VALUES (@fileId, @archiveFileName, @archiveFileExtension)", db)
        archiveFilesDA.InsertCommand.Parameters.Add("@fileId", DbType.Int64, 0, "fileId")
        archiveFilesDA.InsertCommand.Parameters.Add("@archiveFileName", DbType.String, 255, "archiveFileName")
        archiveFilesDA.InsertCommand.Parameters.Add("@archiveFileExtension", DbType.String, 255, "archiveFileExtension")
        archiveFilesDA.UpdateCommand = New SQLiteCommand("UPDATE tblArchiveFiles SET fileId, archiveFileName, archiveFileExtension WHERE archiveFileId=@archiveFileId", db)
        archiveFilesDA.UpdateCommand.Parameters.Add("@fileId", DbType.Int64, 0, "fileId")
        archiveFilesDA.UpdateCommand.Parameters.Add("@archiveFileName", DbType.String, 255, "archiveFileName")
        archiveFilesDA.UpdateCommand.Parameters.Add("@archiveFileExtension", DbType.String, 255, "archiveFileExtension")
        archiveFilesDA.UpdateCommand.Parameters.Add("@archiveFileId", DbType.Int64, 0, "archiveFileId")
        archiveFilesDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblArchiveFiles WHERE archiveFileId=@archiveFileId", db)
        archiveFilesDA.DeleteCommand.Parameters.Add("@archiveFileId", DbType.Int64, 0, "archiveFileId")
    End Sub
    Protected Sub OnSoftwareInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles softwareDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("softwareId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnManufacturerInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles manufacturersDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("manufacturerId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnSystemInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles systemsDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("systemId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnTypeInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles typesDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("typeId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnFlagInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles flagsDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("flagId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnSoftwareFlagInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles softwareFlagsDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("softwareFlagId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnFileInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles filesDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("fileId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnFormatInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles formatsDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("formatId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnRomsetInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles romsetsDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("romsetId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnFileFlagInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles fileFlagsDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("fileFlagId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnArchiveFileInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles archiveFilesDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("archiveFileId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub CreateParserTables()
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
            cmd = New SQLiteCommand("CREATE TABLE tblSystems (systemId INTEGER PRIMARY KEY, systemName TEXT, manufacturerId INTEGER)", db)
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
            cmd = New SQLiteCommand("CREATE TABLE tblSoftwares (softwareId INTEGER PRIMARY KEY, softwareName TEXT, systemId INTEGER, typeId INTEGER)", db)
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
    Protected Function CheckTableExists(tblName As String) As Boolean
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
    Protected Function GetManufacturer(strManufacturerName As String, ByRef ds As Parser) As Parser.ManufacturersRow
        Dim manufacturer As Parser.ManufacturersRow()
        manufacturer = ds.Manufacturers.Select("manufacturerName = '" + strManufacturerName.Replace("'", "''") + "'")
        If manufacturer.Length = 0 Then
            Return ds.Manufacturers.AddManufacturersRow(strManufacturerName)
        Else
            Return manufacturer(0)
        End If
    End Function
    Protected Function GetSystem(strSystemName As String, manufacturer As Parser.ManufacturersRow, ByRef ds As Parser) As Parser.SystemsRow
        Dim system As Parser.SystemsRow()
        system = ds.Systems.Select("systemName = '" + strSystemName.Replace("'", "''") + "' and manufacturerId=" + manufacturer.manufacturerId.ToString)
        If system.Length = 0 Then
            Return ds.Systems.AddSystemsRow(strSystemName, manufacturer)
        Else
            Return system(0)
        End If
    End Function
    Protected Function GetSoftwareType(strTypeName As String, ByRef ds As Parser) As Parser.TypesRow
        Dim type As Parser.TypesRow()
        type = ds.Types.Select("typeName = '" + strTypeName.Replace("'", "''") + "'")
        If type.Length = 0 Then
            Return ds.Types.AddTypesRow(strTypeName)
        End If
        Return type(0)
    End Function
    Protected Function GetSoftware(system As Parser.SystemsRow, type As Parser.TypesRow, strSoftwareName As String, ByRef ds As Parser) As Parser.SoftwaresRow
        Dim soft As Parser.SoftwaresRow
        If systemSoftwaresHT.Contains(strSoftwareName) Then
            Return systemSoftwaresHT(strSoftwareName)
            'soft = ds.Softwares.Select("systemId= " + system.systemId.ToString + " and typeId = " + type.typeId.ToString + " and softwareName = '" + strSoftwareName.Replace("'", "''") + "'")
            'If soft.Length = 0 Then
            'Return ds.Softwares.AddSoftwaresRow(strSoftwareName, system, type)
        Else
            soft = ds.Softwares.AddSoftwaresRow(strSoftwareName, system, type)
            systemSoftwaresHT.Add(strSoftwareName, soft)
            Return soft
        End If
    End Function
    Protected Function GetFlag(strFlagName As String, strFlagType As String, ByRef ds As Parser) As Parser.FlagsRow
        Dim flag As Parser.FlagsRow()
        flag = ds.Flags.Select("flagName = '" + strFlagName.Replace("'", "''") + "' and flagType = '" + strFlagType.Replace("'", "''") + "'")
        If flag.Length = 0 Then
            Return ds.Flags.AddFlagsRow(strFlagName, strFlagType)
        Else
            Return flag(0)
        End If
    End Function
    Protected Function GetSoftwareFlag(softwareRow As Parser.SoftwaresRow, flagRow As Parser.FlagsRow, strFlagValue As String, ByRef ds As Parser) As Parser.SoftwareFlagsRow
        Dim softwareFlag As Parser.SoftwareFlagsRow
        If softwareFlagsHT.Contains(flagRow.flagName + "=" + strFlagValue) Then
            Return softwareFlagsHT(flagRow.flagName + "=" + strFlagValue)
            'softwareFlag = ds.SoftwareFlags.Select("softwareId = " + softwareRow.softwareId.ToString + " and flagId = " + flagRow.flagId.ToString _
            '                              + " and flagValue = '" + strFlagValue.Replace("'", "''") + "'")
            'If softwareFlag.Length = 0 Then
        Else
            softwareFlag = ds.SoftwareFlags.AddSoftwareFlagsRow(softwareRow, flagRow, strFlagValue)
            softwareFlagsHT.Add(flagRow.flagName + "=" + strFlagValue, softwareFlag)
            Return softwareFlag
            'Else
            '    Return softwareFlag(0)
        End If
    End Function
    Protected Function GetRomSet(strRomsetName As String, ByRef ds As Parser) As Parser.RomsetsRow
        Dim romset As Parser.RomsetsRow()
        romset = ds.Romsets.Select("romsetName = '" + strRomsetName.Replace("'", "''") + "'")
        If romset.Length = 0 Then
            Return ds.Romsets.AddRomsetsRow(strRomsetName)
        Else
            Return romset(0)
        End If
    End Function
    Protected Function GetFormat(strFormatName As String, ByRef ds As Parser) As Parser.FormatsRow
        Dim format As Parser.FormatsRow()
        format = ds.Formats.Select("formatName = '" + strFormatName.Replace("'", "''") + "'")
        If format.Length = 0 Then
            Return ds.Formats.AddFormatsRow(strFormatName)
        Else
            Return format(0)
        End If
    End Function
    Protected Function GetFile(strFileName As String, software As Parser.SoftwaresRow, format As Parser.FormatsRow, romset As Parser.RomsetsRow, ByRef ds As Parser) As Parser.FilesRow
        Dim filerow As Parser.FilesRow
        'filerow = ds.Files.Select("softwareId = " + software.softwareId.ToString + " and formatId = " + format.formatId.ToString _
        '                          + " and romsetId = " + romset.romsetId.ToString + " and fileName = '" + strFileName.Replace("'", "''") + "'")
        'If filerow.Length = 0 Then
        If softwareFilesHT.Contains(strFileName) Then
            Return softwareFilesHT(strFileName)
        Else
            filerow = ds.Files.AddFilesRow(strFileName, software, format, romset)
            softwareFilesHT.Add(strFileName, filerow)
            Return filerow
            'Else
            'Return filerow(0)
        End If
    End Function
    Protected Function GetFileFlag(filerow As Parser.FilesRow, flagrow As Parser.FlagsRow, strFlagValue As String, ByRef ds As Parser) As Parser.FileFlagsRow
        Dim fileflag As Parser.FileFlagsRow
        'fileflag = ds.FileFlags.Select("fileId = " + filerow.fileId.ToString + " and flagId = " + flagrow.flagId.ToString _
        '                               + " and flagValue = '" + strFlagValue.Replace("'", "''") + "'")
        'If fileflag.Length = 0 Then
        If fileFlagsHT.Contains(flagrow.flagName + "=" + strFlagValue) Then
            Return fileFlagsHT(flagrow.flagName + "=" + strFlagValue)
        Else
            fileflag = ds.FileFlags.AddFileFlagsRow(filerow, flagrow, strFlagValue)
            fileFlagsHT.Add(flagrow.flagName + "=" + strFlagValue, fileflag)
            Return fileflag
            'Else
            'Return fileflag(0)
        End If
    End Function
    Protected Function GetArchiveFile(filerow As Parser.FilesRow, strArchiveFileName As String, strArchiveFileExtension As String, ByRef ds As Parser) As Parser.ArchiveFilesRow
        Dim archiveFile As Parser.ArchiveFilesRow
        'archiveFile = ds.ArchiveFiles.Select("fileId = " + filerow.fileId.ToString + " and archiveFileName = '" + strArchiveFileName.Replace("'", "''") + "'" _
        '                                     + " and archiveFileExtension = '" + strArchiveFileExtension.Replace("'", "''") + "'")
        'If archiveFile.Length = 0 Then
        If fileArchiveHT.Contains(strArchiveFileName) Then
            Return fileArchiveHT(strArchiveFileName)
        Else
            archiveFile = ds.ArchiveFiles.AddArchiveFilesRow(filerow, strArchiveFileName, strArchiveFileExtension)
            fileArchiveHT.Add(strArchiveFileName, archiveFile)
            Return archiveFile
            'Else
            'Return archiveFile(0)
        End If
    End Function
    Public Sub New()

    End Sub
    Public Sub New(path As String, ByRef ds As Parser)
    End Sub
End Class

