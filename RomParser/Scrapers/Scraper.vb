Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports HtmlAgilityPack
Imports System.Data.SQLite
Public MustInherit Class GameScraper
    Protected lstSystems As List(Of ScraperSystem)
    Protected lstGenres As List(Of ScraperGenre)
    Protected strName As String
    Protected strURL As String
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
    Protected db As SQLiteConnection
    Protected WithEvents gameDA As SQLiteDataAdapter
    Protected WithEvents scraperDA As SQLiteDataAdapter
    Protected WithEvents platformDA As SQLiteDataAdapter
    Protected WithEvents developerDA As SQLiteDataAdapter
    Protected WithEvents gameGenreFlagDA As SQLiteDataAdapter
    Protected WithEvents genreFlagDA As SQLiteDataAdapter
    Protected WithEvents genreFlagTypeDA As SQLiteDataAdapter
    Protected WithEvents releaseDA As SQLiteDataAdapter
    Protected WithEvents regionDA As SQLiteDataAdapter
    Protected WithEvents publisherDA As SQLiteDataAdapter
    Protected WithEvents imageDA As SQLiteDataAdapter
    Protected WithEvents imageTypeDA As SQLiteDataAdapter
    Protected platformGamesHT As Hashtable
    Protected gameGenreFlagsHT As Hashtable
    Protected gameReleasesHT As Hashtable
    Protected releaseImagesHT As Hashtable
    Dim nbGames, currentId As Integer
    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(value As String)
            strName = value
        End Set
    End Property
    Public Property URL() As String
        Get
            Return strURL
        End Get
        Set(value As String)
            strURL = value
        End Set
    End Property
    Public Property Systems() As List(Of ScraperSystem)
        Get
            Return lstSystems
        End Get
        Set(ByVal value As List(Of ScraperSystem))
            lstSystems = value
        End Set
    End Property
    Public Property Genres() As List(Of ScraperGenre)
        Get
            Return lstGenres
        End Get
        Set(value As List(Of ScraperGenre))
            lstGenres = value
        End Set
    End Property

    Public MustOverride Sub GetGameInfo(gameURL As String, platform As Scraper.PlatformsRow, ByRef ds As Scraper)
    Public MustOverride Sub GetGameList(ByVal system As ScraperSystem, ByRef ds As Scraper)
    Public MustOverride Sub GetAllGames(ByRef ds As Scraper)
    Protected Function GetHtml(url As String) As String
        Dim client As WebClient
        Dim data As Stream
        Dim reader As StreamReader
        Dim html As String
        client = New WebClient
        data = client.OpenRead(New Uri(url))
        reader = New StreamReader(data)
        html = reader.ReadToEnd()
        reader.Close()
        data.Close()
        Return html
    End Function
    Protected Function DownloadData(url As String) As Byte()
        Dim client As WebClient
        Dim data() As Byte
        client = New WebClient
        data = client.DownloadData(url)
        client.Dispose()
        Return data
    End Function
    Protected Sub OnItemAdded(strLabel As String, intId As Integer, intCount As Integer)
        RaiseEvent ItemAdded(strLabel, intId, intCount)
    End Sub
    Protected Function GetScraper(strScraperName As String, strScraperURL As String, ByRef ds As Scraper) As Scraper.ScrapersRow
        Dim scraper As Scraper.ScrapersRow()
        scraper = ds.Scrapers.Select("scraperName = '" + strScraperName.Replace("'", "''") + "' And scraperURL='" + strScraperURL.Replace("'", "''") + "'")
        If scraper.Length = 0 Then
            Return ds.Scrapers.AddScrapersRow(strScraperName, strScraperURL)
        Else
            Return scraper(0)
        End If
    End Function
    Protected Function GetDeveloper(strdeveloperName As String, ByRef ds As Scraper) As Scraper.DevelopersRow
        Dim developer As Scraper.DevelopersRow()
        developer = ds.Developers.Select("developerName = '" + strdeveloperName.Replace("'", "''") + "'")
        If developer.Length = 0 Then
            Return ds.Developers.AddDevelopersRow(strdeveloperName)
        Else
            Return developer(0)
        End If
    End Function
    Protected Function GetPlatform(strPlatformName As String, strPlatformURL As String, strPlatformAcronym As String, ByRef ds As Scraper) As Scraper.PlatformsRow
        Dim platform As Scraper.PlatformsRow()
        platform = ds.Platforms.Select("platformName = '" + strPlatformName.Replace("'", "''") + "' And platformURL = '" + strPlatformURL.Replace("'", "''") + _
                                       "' and platformAcronym = '" + strPlatformAcronym.Replace("'", "''") + "'")
        If platform.Length = 0 Then
            Return ds.Platforms.AddPlatformsRow(strPlatformName, strPlatformURL, strPlatformAcronym)
        Else
            Return platform(0)
        End If
    End Function
    Protected Function GetGame(strGameName As String, strGameURL As String, strGamePlot As String, developer As Scraper.DevelopersRow, platform As Scraper.PlatformsRow, _
                             scraper As Scraper.ScrapersRow, ByRef ds As Scraper) As Scraper.GamesRow
        Dim game As Scraper.GamesRow
        If platformGamesHT.Contains(strGameName) Then
            Return platformGamesHT(strGameName)
        Else
            game = ds.Games.AddGamesRow(strGameName, strGameURL, strGamePlot, developer, platform, scraper)
            platformGamesHT.Add(strGameName, game)
            Return game
        End If
        'game = ds.Games.Select("gameName = '" + strGameName.Replace("'", "''") + "' And gameURL = '" + strGameURL.Replace("'", "''") + _
        '                               "' and gamePlot = '" + strGamePlot.Replace("'", "''") + "' and developerId=" + developer.developerId.ToString + _
        '                               " and platformId=" + platform.platformId.ToString + " and scraperId=" + scraper.scraperId.ToString)
        'If game.Length = 0 Then
        '    Return ds.Games.AddGamesRow(strGameName, strGameURL, strGamePlot, developer, platform, scraper)
        'Else
        '    Return game(0)
        'End If
    End Function
    Protected Function GetGenreFlagType(strGenreFlagTypeName As String, ByRef ds As Scraper) As Scraper.GenreFlagTypesRow
        Dim genreFlagType As Scraper.GenreFlagTypesRow()
        genreFlagType = ds.GenreFlagTypes.Select("genreFlagTypeName = '" + strGenreFlagTypeName.Replace("'", "''") + "'")
        If genreFlagType.Length = 0 Then
            Return ds.GenreFlagTypes.AddGenreFlagTypesRow(strGenreFlagTypeName)
        Else
            Return genreFlagType(0)
        End If
    End Function
    Protected Function GetGenreFlag(strGenreFlagName As String, genreFlagType As Scraper.GenreFlagTypesRow, ByRef ds As Scraper) As Scraper.GenreFlagsRow
        Dim genreFlag As Scraper.GenreFlagsRow()
        genreFlag = ds.GenreFlags.Select("genreFlagName = '" + strGenreFlagName.Replace("'", "''") + "' and genreFlagTypeId = " + genreFlagType.genreFlagTypeId.ToString)
        If genreFlag.Length = 0 Then
            Return ds.GenreFlags.AddGenreFlagsRow(strGenreFlagName, genreFlagType)
        Else
            Return genreFlag(0)
        End If
    End Function
    Protected Function GetGameGenreFlag(game As Scraper.GamesRow, genreFlag As Scraper.GenreFlagsRow, ByRef ds As Scraper) As Scraper.GameGenreFlagsRow
        Dim gameGenreFlag As Scraper.GameGenreFlagsRow
        If gameGenreFlagsHT.Contains(genreFlag.genreFlagId) Then
            Return gameGenreFlagsHT(genreFlag.genreFlagId)
        Else
            gameGenreFlag = ds.GameGenreFlags.AddGameGenreFlagsRow(game, genreFlag)
            gameGenreFlagsHT.Add(genreFlag.genreFlagId, gameGenreFlag)
            Return gameGenreFlag
        End If
        'gameGenreFlag = ds.GameGenreFlags.Select("gameId = " + game.gameId.ToString + " and genreFlagId = " + genreFlag.genreFlagId.ToString)
        'If gameGenreFlag.Length = 0 Then
        '    Return ds.GameGenreFlags.AddGameGenreFlagsRow(game, genreFlag)
        'Else
        '    Return gameGenreFlag(0)
        'End If
    End Function
    Protected Function GetRegion(strRegionName As String, ByRef ds As Scraper) As Scraper.RegionsRow
        Dim region As Scraper.RegionsRow()
        region = ds.Regions.Select("regionName = '" + strRegionName.Replace("'", "''") + "'")
        If region.Length = 0 Then
            Return ds.Regions.AddRegionsRow(strRegionName)
        Else
            Return region(0)
        End If
    End Function
    Protected Function GetPublisher(strPublisherName As String, ByRef ds As Scraper) As Scraper.PublishersRow
        Dim publisher As Scraper.PublishersRow()
        publisher = ds.Publishers.Select("publisherName = '" + strPublisherName.Replace("'", "''") + "'")
        If publisher.Length = 0 Then
            Return ds.Publishers.AddPublishersRow(strPublisherName)
        Else
            Return publisher(0)
        End If
    End Function
    Protected Function GetRelease(strReleaseName As String, strReleaseProductId As String, strReleaseDistBarcode As String, strReleaseDate As String, strReleaseRating As String, _
                                game As Scraper.GamesRow, region As Scraper.RegionsRow, publisher As Scraper.PublishersRow, ByRef ds As Scraper) As Scraper.ReleasesRow
        Dim release As Scraper.ReleasesRow
        If gameReleasesHT.Contains(strReleaseName + "(" + strReleaseProductId + ")") Then
            Return gameReleasesHT(strReleaseName + "(" + strReleaseProductId + ")")
        Else
            release = ds.Releases.AddReleasesRow(strReleaseName, strReleaseProductId, strReleaseDistBarcode, strReleaseDate, strReleaseRating, game, region, publisher)
            gameReleasesHT.Add(strReleaseName + "(" + strReleaseProductId + ")", release)
            Return release
        End If
        'release = ds.Releases.Select("releaseName = '" + strReleaseName.Replace("'", "''") + "' And releaseProductNr = '" + strReleaseProductId.Replace("'", "''") + _
        '                               "' and releaseDistBarcode = '" + strReleaseDistBarcode.Replace("'", "''") + "' and releaseDate = '" + strReleaseDate.Replace("'", "''") + _
        '                               "' and releaseRating = '" + strReleaseRating.Replace("'", "''") + "' and gameId=" + game.gameId.ToString + " and regionId=" + region.regionId.ToString + _
        '                               " and publisherId=" + publisher.publisherId.ToString)
        'If release.Length = 0 Then
        '    Return ds.Releases.AddReleasesRow(strReleaseName, strReleaseProductId, strReleaseDistBarcode, strReleaseDate, strReleaseRating, game, region, publisher)
        'Else
        '    Return release(0)
        'End If
    End Function
    Protected Function GetImageType(strImageTypeName As String, ByRef ds As Scraper) As Scraper.ImageTypesRow
        Dim imageType As Scraper.ImageTypesRow()
        imageType = ds.ImageTypes.Select("imageTypeName = '" + strImageTypeName.Replace("'", "''") + "'")
        If imageType.Length = 0 Then
            Return ds.ImageTypes.AddImageTypesRow(strImageTypeName)
        Else
            Return imageType(0)
        End If
    End Function
    Protected Function GetImage(lngImageSize As Long, bytesImageData As Byte(), release As Scraper.ReleasesRow, imageType As Scraper.ImageTypesRow, ByRef ds As Scraper) As Scraper.ImagesRow
        Dim image As Scraper.ImagesRow
        If releaseImagesHT.Contains(imageType.imageTypeName + "(" + lngImageSize.ToString + ")") Then
            Return releaseImagesHT(imageType.imageTypeName + "(" + lngImageSize.ToString + ")")
        Else
            image = ds.Images.AddImagesRow(lngImageSize, bytesImageData, release, imageType)
            releaseImagesHT.Add(imageType.imageTypeName + "(" + lngImageSize.ToString + ")", image)
            Return image
        End If
        '        image = ds.Images.Select("imageSize = " + lngImageSize.ToString + " and releaseId = " + release.releaseId.ToString + " and imageTypeId = " + imageType.imageTypeId.ToString)
        '        If image.Length = 0 Then
        ' Return ds.Images.AddImagesRow(lngImageSize, bytesImageData, release, imageType)
        'Else
        'Return image(0)
        'End If
    End Function
    Protected Sub OnDeveloperInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles developerDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("developerId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnPlatformInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles platformDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("platformId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnScraperInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles scraperDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("scraperId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnGameInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles gameDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("gameId") = db.LastInsertRowId
            e.Row.AcceptChanges()
            currentId += 1
            RaiseEvent ItemAdded(CType(e.Row, Scraper.GamesRow).gameName, currentId, nbGames)
        End If
    End Sub
    Protected Sub OnGenreFlagTypeInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles genreFlagTypeDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("genreFlagTypeId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnGenreFlagInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles genreFlagDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("genreFlagId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnGameGenreFlagInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles gameGenreFlagDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("gameGenreFlagId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnRegionInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles regionDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("regionId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnPublisherInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles publisherDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("publisherId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnReleaseInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles releaseDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("releaseId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnImageTypeInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles imageTypeDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("imageTypeId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub
    Protected Sub OnImageInsert(sender As Object, e As Common.RowUpdatedEventArgs) Handles imageDA.RowUpdated
        If e.Status = UpdateStatus.Continue And e.StatementType = StatementType.Insert Then
            e.Row("imageId") = db.LastInsertRowId
            e.Row.AcceptChanges()
        End If
    End Sub

    Protected Sub CreateParserTables()
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
    Public Sub New(path As String, ByRef ds As Scraper)
        db = New SQLiteConnection("Data Source=" + path)
        CreateParserTables()
        Deserialize(ds)
    End Sub
End Class
<Serializable()> _
Public Class ScraperGame
    Protected intID As Integer
    Protected strTitle As String
    Protected sysPlatform As ScraperSystem
    Protected strURL As String
    Protected strPlot As String
    Protected lstGenres As List(Of ScraperGenre)
    Protected strDeveloper As String
    Protected lstReleases As List(Of ScraperRelease)
    Protected spScraper As GameScraper

    Public Property ID() As Integer
        Get
            Return intID
        End Get
        Set(ByVal value As Integer)
            intID = value
        End Set
    End Property
    Public Property Title() As String
        Get
            Return strTitle
        End Get
        Set(ByVal value As String)
            strTitle = value
        End Set
    End Property
    Public Property URL() As String
        Get
            Return strURL
        End Get
        Set(ByVal value As String)
            strURL = value
        End Set
    End Property
    Public Property Platform() As ScraperSystem
        Get
            Return sysPlatform
        End Get
        Set(ByVal value As ScraperSystem)
            sysPlatform = value
        End Set
    End Property
    Public Property Plot() As String
        Get
            Return strPlot
        End Get
        Set(ByVal value As String)
            strPlot = value
        End Set
    End Property
    Public Property Genres() As List(Of ScraperGenre)
        Get
            Return lstGenres
        End Get
        Set(ByVal value As List(Of ScraperGenre))
            lstGenres = value
        End Set
    End Property
    Public Property Developer() As String
        Get
            Return strDeveloper
        End Get
        Set(ByVal value As String)
            strDeveloper = value
        End Set
    End Property
    Public Property Releases() As List(Of ScraperRelease)
        Get
            Return lstReleases
        End Get
        Set(ByVal value As List(Of ScraperRelease))
            lstReleases = value
        End Set
    End Property
    Public Property Scraper() As GameScraper
        Get
            Return spScraper
        End Get
        Set(ByVal value As GameScraper)
            spScraper = value
        End Set
    End Property
    Public Sub New()
        lstReleases = New List(Of ScraperRelease)
    End Sub
End Class
<Serializable()> _
Public Class ScraperSystem
    Protected strName As String
    Protected intID As Integer
    Protected strAcronym As String
    Protected strURL As String
    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(ByVal value As String)
            strName = value
        End Set
    End Property
    Public Property ID() As Integer
        Get
            Return intID
        End Get
        Set(ByVal value As Integer)
            intID = value
        End Set
    End Property
    Public Property Acronym() As String
        Get
            Return strAcronym
        End Get
        Set(ByVal value As String)
            strAcronym = value
        End Set
    End Property
    Public Property URL() As String
        Get
            Return strURL
        End Get
        Set(ByVal value As String)
            strURL = value
        End Set
    End Property
End Class
<Serializable()> _
<XmlType(TypeName:="Tag")> _
Public Class ScraperGenre
    Public Enum GenreType
        Genre
        Gameplay
        Hardware
        Perspective
        Sport
        Theme
        Vehicle
    End Enum
    Protected strName As String
    Protected gtType As GenreType
    <XmlAttribute(AttributeName:="name")> _
    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(ByVal value As String)
            strName = value
        End Set
    End Property
    <XmlAttribute(AttributeName:="genre")> _
    Public Property Type() As String
        Get
            Return gtType.ToString
        End Get
        Set(ByVal value As String)
            gtType = [Enum].Parse(GetType(GenreType), value)
        End Set
    End Property
End Class
Public Class ScraperRelease
    Protected strTitle As String
    Protected strRegion As String
    Protected strPublisher As String
    Protected strProductID As String
    Protected strDistBarcode As String
    Protected strReleaseDate As String
    Protected strRating As String
    Protected lstImages As List(Of ScraperImage)
    Public Property Title() As String
        Get
            Return strTitle
        End Get
        Set(ByVal value As String)
            strTitle = value
        End Set
    End Property
    Public Property Region() As String
        Get
            Return strRegion
        End Get
        Set(ByVal value As String)
            strRegion = value
        End Set
    End Property
    Public Property Publisher() As String
        Get
            Return strPublisher
        End Get
        Set(ByVal value As String)
            strPublisher = value
        End Set
    End Property
    Public Property ProductID() As String
        Get
            Return strProductID
        End Get
        Set(ByVal value As String)
            strProductID = value
        End Set
    End Property
    Public Property DistBarcode() As String
        Get
            Return strDistBarcode
        End Get
        Set(ByVal value As String)
            strDistBarcode = value
        End Set
    End Property
    Public Property ReleaseDate() As String
        Get
            Return strReleaseDate
        End Get
        Set(ByVal value As String)
            strReleaseDate = value
        End Set
    End Property
    Public Property Rating() As String
        Get
            Return strRating
        End Get
        Set(ByVal value As String)
            strRating = value
        End Set
    End Property
    Public Property Images() As List(Of ScraperImage)
        Get
            Return lstImages
        End Get
        Set(ByVal value As List(Of ScraperImage))
            lstImages = value
        End Set
    End Property
    Public Sub New()
        lstImages = New List(Of ScraperImage)
    End Sub
End Class
Public Class ScraperImage
    Public Enum ScraperImageType
        front
        back
        side
        top
    End Enum
    Protected strURL As String
    Protected siType As ScraperImageType
    Protected baImageData As Byte()
    Public Property URL() As String
        Get
            Return strURL
        End Get
        Set(ByVal value As String)
            strURL = value
        End Set
    End Property
    Public Property ImageType() As String
        Get
            Return siType.ToString
        End Get
        Set(ByVal value As String)
            siType = [Enum].Parse(GetType(ScraperImageType), value)
        End Set
    End Property
    Public Property ImageData() As Byte()
        Get
            Return baImageData
        End Get
        Set(ByVal value As Byte())
            baImageData = value
        End Set
    End Property

End Class