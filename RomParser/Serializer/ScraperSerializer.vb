Imports System.Data.SQLite
Imports System.ComponentModel
Public Class ScraperSerializer
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
    Private db As SQLiteConnection
    Private WithEvents gameDA As SQLiteDataAdapter
    Private WithEvents scraperDA As SQLiteDataAdapter
    Private WithEvents platformDA As SQLiteDataAdapter
    Private WithEvents developerDA As SQLiteDataAdapter
    Private WithEvents gameGenreFlagDA As SQLiteDataAdapter
    Private WithEvents genreFlagDA As SQLiteDataAdapter
    Private WithEvents genreFlagTypeDA As SQLiteDataAdapter
    Private WithEvents releaseDA As SQLiteDataAdapter
    Private WithEvents regionDA As SQLiteDataAdapter
    Private WithEvents publisherDA As SQLiteDataAdapter
    Private WithEvents imageDA As SQLiteDataAdapter
    Private WithEvents imageTypeDA As SQLiteDataAdapter
    Dim nbGames, currentId As Integer
#Region "Public Subs"
    Public Sub New(path As String, ds As Scraper)
        db = New SQLiteConnection("Data Source=" + path)
        CreateParserTables()
        Deserialize(ds)
    End Sub
    Public Sub Serialize(ByRef ds As Scraper)
        Dim trans As SQLiteTransaction
        db.Open()
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        developerDA.Update(ds.Developers)
        platformDA.Update(ds.Platforms)
        scraperDA.Update(ds.Scrapers)
        nbGames = ds.Games.GetChanges.Rows.Count
        currentId = 0
        gameDA.Update(ds.Games)

        genreFlagTypeDA.Update(ds.GenreFlagTypes)
        genreFlagDA.Update(ds.GenreFlags)
        gameGenreFlagDA.Update(ds.GameGenreFlags)

        regionDA.Update(ds.Regions)
        publisherDA.Update(ds.Publishers)
        releaseDA.Update(ds.Releases)

        imageTypeDA.Update(ds.ImageTypes)
        imageDA.Update(ds.Images)
        trans.Commit()
        db.Close()
    End Sub
    Public Sub Deserialize(ByRef ds As Scraper)
        Dim trans As SQLiteTransaction
        db.Open()
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        '1. scraper data adapter
        scraperDA = New SQLiteDataAdapter
        scraperDA.SelectCommand = New SQLiteCommand("SELECT scraperId, scraperName, scraperURL FROM tblScrapers", db)
        scraperDA.InsertCommand = New SQLiteCommand("INSERT INTO tblScrapers (scraperName, scraperURL) VALUES (@scraperName, @scraperURL)", db)
        scraperDA.InsertCommand.Parameters.Add("@scraperName", DbType.String, 255, "scraperName")
        scraperDA.InsertCommand.Parameters.Add("@scraperURL", DbType.String, 255, "scraperURL")
        scraperDA.UpdateCommand = New SQLiteCommand("UPDATE tblScrapers set scraperName=@scraperName,scraperURL=@scraperURL WHERE scraperId=@scraperId", db)
        scraperDA.UpdateCommand.Parameters.Add("@scraperName", DbType.String, 255, "scraperName")
        scraperDA.UpdateCommand.Parameters.Add("@scraperURL", DbType.String, 255, "scraperURL")
        scraperDA.UpdateCommand.Parameters.Add("@scraperId", DbType.Int64, 0, "scraperId")
        scraperDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblScrapers WHERE scraperId=@scraperId", db)
        scraperDA.DeleteCommand.Parameters.Add("@scraperId", DbType.Int64, 0, "scraperId")
        scraperDA.Fill(ds, "Scrapers")
        '2. platform data adapter
        platformDA = New SQLiteDataAdapter
        platformDA.SelectCommand = New SQLiteCommand("SELECT platformId, platformName, platformURL, platformAcronym FROM tblPlatforms", db)
        platformDA.InsertCommand = New SQLiteCommand("INSERT INTO tblPlatforms (platformName, platformURL, platformAcronym) VALUES " _
                                                 + "(@platformName, @platformURL, @platformAcronym)", db)
        platformDA.InsertCommand.Parameters.Add("@platformName", DbType.String, 255, "platformName")
        platformDA.InsertCommand.Parameters.Add("@platformURL", DbType.String, 255, "platformURL")
        platformDA.InsertCommand.Parameters.Add("@platformAcronym", DbType.String, 255, "platformAcronym")
        platformDA.UpdateCommand = New SQLiteCommand("UPDATE tblPlatforms set platformName=@platformName,platformURL=@platformURL, platformAcronym=@platformAcronym WHERE platformId=@platformId", db)
        platformDA.UpdateCommand.Parameters.Add("@platformName", DbType.String, 255, "platformName")
        platformDA.UpdateCommand.Parameters.Add("@platformURL", DbType.String, 255, "platformURL")
        platformDA.UpdateCommand.Parameters.Add("@platformAcronym", DbType.String, 255, "platformAcronym")
        platformDA.UpdateCommand.Parameters.Add("@platformId", DbType.Int64, 0, "platformId")
        platformDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblPlatforms WHERE platformId=@platformId", db)
        platformDA.DeleteCommand.Parameters.Add("@platformId", DbType.Int64, 0, "platformId")
        platformDA.Fill(ds, "Platforms")
        '3. developer data adapter
        developerDA = New SQLiteDataAdapter
        developerDA.SelectCommand = New SQLiteCommand("SELECT developerId, developerName FROM tblDevelopers", db)
        developerDA.InsertCommand = New SQLiteCommand("INSERT INTO tblDevelopers (developerName) VALUES (@developerName)", db)
        developerDA.InsertCommand.Parameters.Add("@developerName", DbType.String, 255, "developerName")
        developerDA.UpdateCommand = New SQLiteCommand("UPDATE tblDevelopers set developerName=@developerName WHERE developerId=@developerId", db)
        developerDA.UpdateCommand.Parameters.Add("@developerName", DbType.String, 255, "developerName")
        developerDA.UpdateCommand.Parameters.Add("@developerId", DbType.Int64, 0, "developerId")
        developerDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblDevelopers WHERE developerId=@developerId", db)
        developerDA.DeleteCommand.Parameters.Add("@developerId", DbType.Int64, 0, "developerId")
        developerDA.Fill(ds, "Developers")
        '4. game data adapter
        gameDA = New SQLiteDataAdapter
        gameDA.SelectCommand = New SQLiteCommand("SELECT gameId, gameName, gameURL, gamePlot, developerId, platformId, scraperId FROM tblGames", db)
        gameDA.InsertCommand = New SQLiteCommand("INSERT INTO tblGames (gameName, gameURL, gamePlot, developerId, platformId, scraperId) VALUES " _
                                                 + "(@gameName, @gameURL, @gamePlot, @developerId, @platformId, @scraperId)", db)
        gameDA.InsertCommand.Parameters.Add("@gameName", DbType.String, 255, "gameName")
        gameDA.InsertCommand.Parameters.Add("@gameURL", DbType.String, 255, "gameURL")
        gameDA.InsertCommand.Parameters.Add("@gamePlot", DbType.String, 255, "gamePlot")
        gameDA.InsertCommand.Parameters.Add("@developerId", DbType.Int64, 0, "developerId")
        gameDA.InsertCommand.Parameters.Add("@platformId", DbType.Int64, 0, "platformId")
        gameDA.InsertCommand.Parameters.Add("@scraperId", DbType.Int64, 0, "scraperId")
        gameDA.UpdateCommand = New SQLiteCommand("UPDATE tblGames set gameName=@gameName,gameURL=@gameURL,gamePlot=@gamePlot, developerId=@developerId,platformId=@platformId, scraperId=@scraperId " _
                                                 + "WHERE gameId=@gameID", db)
        gameDA.UpdateCommand.Parameters.Add("@gameName", DbType.String, 255, "gameName")
        gameDA.UpdateCommand.Parameters.Add("@gameURL", DbType.String, 255, "gameURL")
        gameDA.UpdateCommand.Parameters.Add("@gamePlot", DbType.String, 255, "gamePlot")
        gameDA.UpdateCommand.Parameters.Add("@developerId", DbType.Int64, 0, "developerId")
        gameDA.UpdateCommand.Parameters.Add("@platformId", DbType.Int64, 0, "platformId")
        gameDA.UpdateCommand.Parameters.Add("@scraperId", DbType.Int64, 0, "scraperId")
        gameDA.UpdateCommand.Parameters.Add("@gameId", DbType.Int64, 0, "gameId")
        gameDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblGames WHERE gameId=@gameID", db)
        gameDA.DeleteCommand.Parameters.Add("@gameId", DbType.Int64, 0, "gameId")
        gameDA.Fill(ds, "Games")
        '5. genre flag type data adapter
        genreFlagTypeDA = New SQLiteDataAdapter
        genreFlagTypeDA.SelectCommand = New SQLiteCommand("SELECT genreFlagTypeId, genreFlagTypeName FROM tblGenreFlagTypes", db)
        genreFlagTypeDA.InsertCommand = New SQLiteCommand("INSERT INTO tblGenreFlagTypes (genreFlagTypeName) VALUES (@genreFlagTypeName)", db)
        genreFlagTypeDA.InsertCommand.Parameters.Add("@genreFlagTypeName", DbType.String, 255, "genreFlagTypeName")
        genreFlagTypeDA.UpdateCommand = New SQLiteCommand("UPDATE tblGenreFlagTypes set genreFlagTypeName=@genreFlagTypeName WHERE genreFlagTypeId=@genreFlagTypeId", db)
        genreFlagTypeDA.UpdateCommand.Parameters.Add("@genreFlagTypeName", DbType.String, 255, "genreFlagTypeName")
        genreFlagTypeDA.UpdateCommand.Parameters.Add("@genreFlagTypeId", DbType.Int64, 0, "genreFlagTypeId")
        genreFlagTypeDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblGenreFlagTypes WHERE genreFlagTypeId=@genreFlagTypeId", db)
        genreFlagTypeDA.DeleteCommand.Parameters.Add("@genreFlagTypeId", DbType.Int64, 0, "genreFlagTypeId")
        genreFlagTypeDA.Fill(ds, "GenreFlagTypes")
        '6. genre flag data adapter
        genreFlagDA = New SQLiteDataAdapter
        genreFlagDA.SelectCommand = New SQLiteCommand("SELECT genreFlagId, genreFlagName, genreFlagTypeId FROM tblGenreFlags", db)
        genreFlagDA.InsertCommand = New SQLiteCommand("INSERT INTO tblGenreFlags (genreFlagName, genreFlagTypeId) VALUES (@genreFlagName, @genreFlagTypeId)", db)
        genreFlagDA.InsertCommand.Parameters.Add("@genreFlagName", DbType.String, 255, "genreFlagName")
        genreFlagDA.InsertCommand.Parameters.Add("@genreFlagTypeId", DbType.Int64, 0, "genreFlagTypeId")
        genreFlagDA.UpdateCommand = New SQLiteCommand("UPDATE tblGenreFlags set genreFlagName=@genreFlagName,genreFlagTypeId=@genreFlagTypeId WHERE genreFlagId=@genreFlagId", db)
        genreFlagDA.UpdateCommand.Parameters.Add("@genreFlagName", DbType.String, 255, "genreFlagName")
        genreFlagDA.UpdateCommand.Parameters.Add("@genreFlagTypeId", DbType.Int64, 0, "genreFlagTypeId")
        genreFlagDA.UpdateCommand.Parameters.Add("@genreFlagId", DbType.Int64, 0, "genreFlagId")
        genreFlagDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblGenreFlags WHERE genreFlagId=@genreFlagId", db)
        genreFlagDA.DeleteCommand.Parameters.Add("@genreFlagId", DbType.Int64, 0, "genreFlagId")
        genreFlagDA.Fill(ds, "GenreFlags")
        '7. game genre flag data adapter
        gameGenreFlagDA = New SQLiteDataAdapter
        gameGenreFlagDA.SelectCommand = New SQLiteCommand("SELECT gameGenreFlagId, gameId, genreFlagId FROM tblGameGenreFlags", db)
        gameGenreFlagDA.InsertCommand = New SQLiteCommand("INSERT INTO tblGameGenreFlags (gameId, genreFlagId) VALUES (@gameId, @genreFlagId)", db)
        gameGenreFlagDA.InsertCommand.Parameters.Add("@gameId", DbType.Int64, 0, "gameId")
        gameGenreFlagDA.InsertCommand.Parameters.Add("@genreFlagId", DbType.Int64, 0, "genreFlagId")
        gameGenreFlagDA.UpdateCommand = New SQLiteCommand("UPDATE tblGameGenreFlags set gameId=@gameId,genreFlagId=@genreFlagId WHERE gameGenreFlagId=@gameGenreFlagId", db)
        gameGenreFlagDA.UpdateCommand.Parameters.Add("@gameId", DbType.Int64, 0, "gameId")
        gameGenreFlagDA.UpdateCommand.Parameters.Add("@genreFlagId", DbType.Int64, 0, "genreFlagId")
        gameGenreFlagDA.UpdateCommand.Parameters.Add("@gameGenreFlagId", DbType.Int64, 0, "gameGenreFlagId")
        gameGenreFlagDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblGameGenreFlags WHERE gameGenreFlagId=@gameGenreFlagId", db)
        gameGenreFlagDA.DeleteCommand.Parameters.Add("@gameGenreFlagId", DbType.Int64, 0, "gameGenreFlagId")
        gameGenreFlagDA.Fill(ds, "GameGenreFlags")
        '8. region data adapter
        regionDA = New SQLiteDataAdapter
        regionDA.SelectCommand = New SQLiteCommand("SELECT regionId, regionName FROM tblRegions", db)
        regionDA.InsertCommand = New SQLiteCommand("INSERT INTO tblRegions (regionName) VALUES (@regionName)", db)
        regionDA.InsertCommand.Parameters.Add("@regionName", DbType.String, 255, "regionName")
        regionDA.UpdateCommand = New SQLiteCommand("UPDATE tblRegions set regionName=@regionName WHERE regionId=@regionId", db)
        regionDA.UpdateCommand.Parameters.Add("@regionName", DbType.String, 255, "regionName")
        regionDA.UpdateCommand.Parameters.Add("@regionId", DbType.Int64, 0, "regionId")
        regionDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblRegions WHERE regionId=@regionId", db)
        regionDA.DeleteCommand.Parameters.Add("@regionId", DbType.Int64, 0, "regionId")
        regionDA.Fill(ds, "Regions")
        '9. publisher data adapter
        publisherDA = New SQLiteDataAdapter
        publisherDA.SelectCommand = New SQLiteCommand("SELECT publisherId, publisherName FROM tblPublishers", db)
        publisherDA.InsertCommand = New SQLiteCommand("INSERT INTO tblPublishers (publisherName) VALUES (@publisherName)", db)
        publisherDA.InsertCommand.Parameters.Add("@publisherName", DbType.String, 255, "publisherName")
        publisherDA.UpdateCommand = New SQLiteCommand("UPDATE tblPublishers set publisherName=@publisherName WHERE publisherId=@publisherId", db)
        publisherDA.UpdateCommand.Parameters.Add("@publisherName", DbType.String, 255, "publisherName")
        publisherDA.UpdateCommand.Parameters.Add("@publisherId", DbType.Int64, 0, "publisherId")
        publisherDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblPublishers WHERE publisherId=@publisherId", db)
        publisherDA.DeleteCommand.Parameters.Add("@publisherId", DbType.Int64, 0, "publisherId")
        publisherDA.Fill(ds, "Publishers")
        '10. release data adapter
        releaseDA = New SQLiteDataAdapter
        releaseDA.SelectCommand = New SQLiteCommand("SELECT releaseId, releaseName, releaseProductNr, releaseDistBarcode,releaseDate, releaseRating, gameId, regionId, publisherId FROM tblReleases", db)
        releaseDA.InsertCommand = New SQLiteCommand("INSERT INTO tblReleases (releaseName, releaseProductNr, releaseDistBarcode,releaseDate, releaseRating, gameId, regionId, publisherId) VALUES " _
                                                 + "(@releaseName, @releaseProductNr, @releaseDistBarcode, @releaseDate, @releaseRating, @gameId, @regionId, @publisherId)", db)
        releaseDA.InsertCommand.Parameters.Add("@releaseName", DbType.String, 255, "releaseName")
        releaseDA.InsertCommand.Parameters.Add("@releaseProductNr", DbType.String, 255, "releaseProductNr")
        releaseDA.InsertCommand.Parameters.Add("@releaseDistBarcode", DbType.String, 255, "releaseDistBarcode")
        releaseDA.InsertCommand.Parameters.Add("@releaseDate", DbType.String, 255, "releaseDate")
        releaseDA.InsertCommand.Parameters.Add("@releaseRating", DbType.String, 255, "releaseRating")
        releaseDA.InsertCommand.Parameters.Add("@gameId", DbType.Int64, 0, "gameId")
        releaseDA.InsertCommand.Parameters.Add("@regionId", DbType.Int64, 0, "regionId")
        releaseDA.InsertCommand.Parameters.Add("@publisherId", DbType.Int64, 0, "publisherId")
        releaseDA.UpdateCommand = New SQLiteCommand("UPDATE tblReleases set releaseName=@releaseName,releaseProductNr=@releaseProductNr,releaseDistBarcode=@releaseDistBarcode, " _
                                                    + "releaseDate = @releaseDate, releaseRating =@releaseRating,gameId=@gameId,regionId=@regionId, publisherId=@publisherId " _
                                                    + "WHERE releaseId=@releaseID", db)
        releaseDA.UpdateCommand.Parameters.Add("@releaseName", DbType.String, 255, "releaseName")
        releaseDA.UpdateCommand.Parameters.Add("@releaseProductNr", DbType.String, 255, "releaseProductNr")
        releaseDA.UpdateCommand.Parameters.Add("@releaseDistBarcode", DbType.String, 255, "releaseDistBarcode")
        releaseDA.UpdateCommand.Parameters.Add("@releaseDate", DbType.String, 255, "releaseDate")
        releaseDA.UpdateCommand.Parameters.Add("@releaseRating", DbType.String, 255, "releaseRating")
        releaseDA.UpdateCommand.Parameters.Add("@gameId", DbType.Int64, 0, "gameId")
        releaseDA.UpdateCommand.Parameters.Add("@regionId", DbType.Int64, 0, "regionId")
        releaseDA.UpdateCommand.Parameters.Add("@publisherId", DbType.Int64, 0, "publisherId")
        releaseDA.UpdateCommand.Parameters.Add("@releaseId", DbType.Int64, 0, "releaseId")
        releaseDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblReleases WHERE releaseId=@releaseID", db)
        releaseDA.DeleteCommand.Parameters.Add("@releaseId", DbType.Int64, 0, "releaseId")
        releaseDA.Fill(ds, "Releases")
        '11. imageType data adapter
        imageTypeDA = New SQLiteDataAdapter
        imageTypeDA.SelectCommand = New SQLiteCommand("SELECT imageTypeId, imageTypeName FROM tblImageTypes", db)
        imageTypeDA.InsertCommand = New SQLiteCommand("INSERT INTO tblImageTypes (imageTypeName) VALUES (@imageTypeName)", db)
        imageTypeDA.InsertCommand.Parameters.Add("@imageTypeName", DbType.String, 255, "imageTypeName")
        imageTypeDA.UpdateCommand = New SQLiteCommand("UPDATE tblImageTypes set imageTypeName=@imageTypeName WHERE imageTypeId=@imageTypeId", db)
        imageTypeDA.UpdateCommand.Parameters.Add("@imageTypeName", DbType.String, 255, "imageTypeName")
        imageTypeDA.UpdateCommand.Parameters.Add("@imageTypeId", DbType.Int64, 0, "imageTypeId")
        imageTypeDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblImageTypes WHERE imageTypeId=@imageTypeId", db)
        imageTypeDA.DeleteCommand.Parameters.Add("@imageTypeId", DbType.Int64, 0, "imageTypeId")
        imageTypeDA.Fill(ds, "ImageTypes")
        '12. image data adapter
        imageDA = New SQLiteDataAdapter
        imageDA.SelectCommand = New SQLiteCommand("SELECT imageId, imageSize, imageData, releaseId, imageTypeId FROM tblImages", db)
        imageDA.InsertCommand = New SQLiteCommand("INSERT INTO tblImages (imageSize, imageData, releaseId, imageTypeId) VALUES " _
                                                 + "(@imageSize, @imageData, @releaseId, @imageTypeId)", db)
        imageDA.InsertCommand.Parameters.Add("@imageSize", DbType.String, 255, "imageSize")
        imageDA.InsertCommand.Parameters.Add("@imageData", DbType.Object, 0, "imageData")
        imageDA.InsertCommand.Parameters.Add("@releaseId", DbType.Int64, 0, "releaseId")
        imageDA.InsertCommand.Parameters.Add("@imageTypeId", DbType.Int64, 0, "imageTypeId")
        imageDA.UpdateCommand = New SQLiteCommand("UPDATE tblImages set imageSize=@imageSize,imageData=@imageData, releaseId=@releaseId,imagetypeId=@imageTypeId WHERE imageId=@imageId", db)
        imageDA.UpdateCommand.Parameters.Add("@imageSize", DbType.String, 255, "imageSize")
        imageDA.UpdateCommand.Parameters.Add("@imageData", DbType.Object, 0, "imageData")
        imageDA.UpdateCommand.Parameters.Add("@releaseId", DbType.Int64, 0, "releaseId")
        imageDA.UpdateCommand.Parameters.Add("@imageTypeId", DbType.Int64, 0, "imageTypeId")
        imageDA.UpdateCommand.Parameters.Add("@imageId", DbType.Int64, 0, "imageId")
        imageDA.DeleteCommand = New SQLiteCommand("DELETE FROM tblImages WHERE imageId=@imageId", db)
        imageDA.DeleteCommand.Parameters.Add("@imageId", DbType.Int64, 0, "imageId")
        imageDA.Fill(ds, "Images")
        trans.Commit()
        db.Close()
    End Sub
#End Region

#Region "Private Subs"
    Private Sub OnDeveloperInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles developerDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("developerId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnPlatformInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles platformDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("platformId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnScraperInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles scraperDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("scraperId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnGameInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles gameDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("gameId") = db.LastInsertRowId
            e.Row.AcceptChanges()
            currentId += 1
            RaiseEvent ItemAdded(CType(e.Row, Scraper.GamesRow).gameName, currentId, nbGames)
        End If
    End Sub
    Private Sub OnGenreFlagTypeInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles genreFlagTypeDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("genreFlagTypeId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnGenreFlagInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles genreFlagDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("genreFlagId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnGameGenreFlagInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles gameGenreFlagDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("gameGenreFlagId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnRegionInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles regionDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("regionId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnPublisherInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles publisherDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("publisherId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnReleaseInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles releaseDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("releaseId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnImageTypeInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles imageTypeDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("imageTypeId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Private Sub OnImageInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles imageDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("imageId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub

    Private Sub CreateParserTables()
        db.Open()
        Dim cmd As SQLiteCommand
        If Not CheckTableExists("tblScrapers") Then
            cmd = New SQLiteCommand("CREATE TABLE tblScrapers (scraperId INTEGER PRIMARY KEY, scraperName TEXT, scraperURL TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblPlatforms") Then
            cmd = New SQLiteCommand("CREATE TABLE tblPlatforms (platformId INTEGER PRIMARY KEY, platformName TEXT, platformAcronym TEXT, platformURL TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblGenreFlags") Then
            cmd = New SQLiteCommand("CREATE TABLE tblGenreFlags (genreFlagId INTEGER PRIMARY KEY, genreFlagName TEXT, genreFlagTypeId INTEGER)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblGenreFlagTypes") Then
            cmd = New SQLiteCommand("CREATE TABLE tblGenreFlagTypes (genreFlagTypeId INTEGER PRIMARY KEY, genreFlagTypeName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblRegions") Then
            cmd = New SQLiteCommand("CREATE TABLE tblRegions (regionId INTEGER PRIMARY KEY, regionName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblDevelopers") Then
            cmd = New SQLiteCommand("CREATE TABLE tblDevelopers (developerId INTEGER PRIMARY KEY, developerName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblPublishers") Then
            cmd = New SQLiteCommand("CREATE TABLE tblPublishers (publisherId INTEGER PRIMARY KEY, publisherName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblImageTypes") Then
            cmd = New SQLiteCommand("CREATE TABLE tblImageTypes (imageTypeId INTEGER PRIMARY KEY, imageTypeName TEXT)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblGames") Then
            cmd = New SQLiteCommand("CREATE TABLE tblGames (gameId INTEGER PRIMARY KEY, gameName TEXT, gameURL TEXT, gamePlot TEXT, developerId INTEGER, platformId INTEGER, scraperId INTEGER)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxDeveloper ON tblGames(developerId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxPlatform ON tblGames(platformId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxGameScraper ON tblGames(scraperId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblGameGenreFlags") Then
            cmd = New SQLiteCommand("CREATE TABLE tblGameGenreFlags (gameGenreFlagId INTEGER PRIMARY KEY, gameId INTEGER, genreFlagId INTEGER)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxFlagGame ON tblGameGenreFlags(gameId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxGenreFlag ON tblGameGenreFlags(genreFlagId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblReleases") Then
            cmd = New SQLiteCommand("CREATE TABLE tblReleases (releaseId INTEGER PRIMARY KEY, releaseName TEXT, releaseProductNr TEXT, releaseDistBarcode TEXT, releaseDate TEXT, releaseRating TEXT," _
                                    + "gameId INTEGER, regionId INTEGER, publisherId INTEGER )", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxReleaseGame ON tblReleases(gameId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxRegion ON tblReleases(regionId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxPublisher ON tblReleases(publisherId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblImages") Then
            cmd = New SQLiteCommand("CREATE TABLE tblImages (imageId INTEGER PRIMARY KEY, imageSize INTEGER, imageData BLOB, releaseId INTEGER, imageTypeId INTEGER)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxImageRelease ON tblImages(releaseId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxImageType ON tblImages(imageTypeId)", db)
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
#End Region
End Class
