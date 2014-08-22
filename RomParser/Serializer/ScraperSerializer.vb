Imports System.Data.SQLite
Imports System.ComponentModel
Public Class ScraperSerializer
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
    Private db As SQLiteConnection
    Private caches As Hashtable
#Region "Public Subs"
    Public Sub New(path As String)
        db = New SQLiteConnection("Data Source=" + path)
        db.Open()
        CreateParserTables()
        caches = New Hashtable
    End Sub
    Public Sub SerializeList(ByRef list As List(Of ScraperGame))
        Dim game As ScraperGame
        Dim id, nb As Integer
        Dim trans As SQLiteTransaction
        nb = list.Count
        id = 0
        trans = db.BeginTransaction(IsolationLevel.ReadCommitted)
        For Each game In list
            Serialize(game)
            id += 1
            RaiseEvent ItemAdded(game.Platform.Name, id, nb)
        Next
        trans.Commit()
    End Sub
    Public Sub Serialize(game As ScraperGame)
        Dim cmd As SQLiteCommand
        Dim str As String
        Dim intScraperId, intPlatformId, intGenreFlagId, intGenreFlagTypeId As Integer
        Dim intRegionId, intDeveloperId, intPublisherId, intImageTypeId As Integer
        Dim intGameId, intReleaseId, intGameGenreFlagId As Integer
        Dim parameters As Hashtable
        Dim genre As ScraperGenre
        Dim release As ScraperRelease
        Dim image As ScraperImage
        If IsNothing(db) Then Exit Sub
        '1. Serialize Game Lookup Values
        intScraperId = GetId("scraper", game.Scraper.Name, False)
        If intScraperId = 0 Then
            str = "INSERT INTO tblScrapers (scraperName, scraperURL) VALUES ("
            str += """" + game.Scraper.Name + """,""" + game.Scraper.URL + """)"
            cmd = New SQLiteCommand(str, db)
            cmd.ExecuteNonQuery()
            intScraperId = db.LastInsertRowId
        End If
        intPlatformId = GetId("platform", game.Platform.Name, False)
        If intPlatformId = 0 Then
            str = "INSERT INTO tblPlatforms (platformName, platformAcronym, platformURL) VALUES ("
            str += """" + game.Platform.Name + """,""" + game.Platform.Acronym + """,""" + game.Platform.URL + """)"
            cmd = New SQLiteCommand(str, db)
            cmd.ExecuteNonQuery()
            intPlatformId = db.LastInsertRowId
        End If
        intDeveloperId = GetId("developer", game.Developer, True)
        '2. Serialize Game
        parameters = New Hashtable
        parameters.Add("platformId", intPlatformId)
        parameters.Add("developerId", intDeveloperId)
        parameters.Add("scraperId", intScraperId)
        intGameId = GetId("game", game.Title, False, parameters)
        If intGameId = 0 Then
            str = "INSERT INTO tblGames (gameName,gameURL,gamePlot, developerId,platformId, scraperId) VALUES ("
            str += """" + game.Title + """,""" + game.URL + """,""" + game.Plot + """," + intDeveloperId.ToString + "," + intPlatformId.ToString + "," + intScraperId.ToString + ")"
            cmd = New SQLiteCommand(str, db)
            cmd.ExecuteNonQuery()
            intGameId = db.LastInsertRowId
        End If
        '3. Serialize Genres
        For Each genre In game.Genres
            intGenreFlagTypeId = GetId("genreFlagType", genre.Type, True)
            intGenreFlagId = GetId("genreFlag", genre.Name, False)
            If intGenreFlagId = 0 Then
                str = "INSERT INTO tblGenreFlags (genreFlagName, genreFlagTypeId) VALUES ("
                str += """" + genre.Name + """," + intGenreFlagTypeId.ToString + ")"
                cmd = New SQLiteCommand(str, db)
                cmd.ExecuteNonQuery()
                intGenreFlagId = db.LastInsertRowId
            End If
            str = "INSERT INTO tblGameGenreFlags (gameId, genreFlagID) VALUES ("
            str += intGameId.ToString + "," + intGenreFlagId.ToString + ")"
            cmd = New SQLiteCommand(str, db)
            cmd.ExecuteNonQuery()
            intGameGenreFlagId = db.LastInsertRowId
        Next
        '4. serialize Releases
        For Each release In game.Releases
            intRegionId = GetId("region", release.Region, True)
            intPublisherId = GetId("publisher", release.Publisher, True)
            parameters = New Hashtable
            parameters.Add("gameId", intGameId)
            parameters.Add("regionId", intRegionId)
            parameters.Add("publisherId", intPublisherId)
            intReleaseId = GetId("release", release.Title, False, parameters)
            If intReleaseId = 0 Then
                str = "INSERT INTO tblReleases (releaseName,gameId,regionId,publisherId,releaseProductNr, releaseDistBarcode,releaseDate,releaseRating ) VALUES ("
                str += """" + release.Title + """," + intGameId.ToString + "," + intRegionId.ToString + "," + intPublisherId.ToString + ","
                str += """" + release.ProductID + """,""" + release.DistBarcode + """,""" + release.ReleaseDate + """,""" + release.Rating + """)"
                cmd = New SQLiteCommand(str, db)
                cmd.ExecuteNonQuery()
                intReleaseId = db.LastInsertRowId
            End If
            For Each image In release.Images
                intImageTypeId = GetId("imageType", image.ImageType, True)
                str = "INSERT INTO tblImages (releaseId,imageTypeId,imageSize,imageData) VALUES (@release,@imagetype,@imageSize, @imagedata)"
                cmd = New SQLiteCommand(str, db)
                cmd.Parameters.Add("@release", DbType.String, 255).Value = intReleaseId
                cmd.Parameters.Add("@imagetype", DbType.Int32).Value = intImageTypeId
                cmd.Parameters.Add("@imagesize", DbType.Int32).Value = image.ImageData.Length
                cmd.Parameters.Add("@imagedata", DbType.Object).Value = image.ImageData
                cmd.ExecuteNonQuery()
            Next
        Next
    End Sub
    Public Sub Deserialize(ByRef lst As List(Of ScraperGame))
        Dim game As ScraperGame
        Dim genre As ScraperGenre
        Dim release As ScraperRelease
        Dim image As ScraperImage
        Dim strSQL As String
        Dim maincmd As SQLiteCommand
        Dim mainreader As SQLiteDataReader
        Dim secondCmd As SQLiteCommand
        Dim secondReader As SQLiteDataReader
        Dim thirdCmd As SQLiteCommand
        Dim thirdreader As SQLiteDataReader
        Dim intGameId, intReleaseId As Integer
        If IsNothing(db) Then Exit Sub
        strSQL = "SELECT g.gameId, g.gameName,g.gameURL, g.gamePlot, d.developerName, p.platformName, p.platformAcronym, p.platformURL, s.scraperName, s.scraperURL"
        strSQL += " FROM tblGames g INNER JOIN tblDevelopers d ON d.developerId = g.developerId"
        strSQL += " INNER JOIN tblPlatforms p on p.platformId = g.platformId"
        strSQL += " INNER JOIN tblScrapers s on s.scraperId = g.scraperId"
        maincmd = New SQLiteCommand(strSQL, db)
        mainreader = maincmd.ExecuteReader
        While mainreader.Read()
            '1. read game data
            intGameId = mainreader.GetInt32(0)
            game = New ScraperGame
            game.ID = intGameId
            game.Title = mainreader.GetString(1)
            game.URL = mainreader.GetString(2)
            game.Plot = mainreader.GetString(3)
            game.Developer = mainreader.GetString(4)
            game.Platform = New ScraperSystem()
            game.Platform.Name = mainreader.GetString(5)
            game.Platform.Acronym = mainreader.GetString(6)
            game.Platform.URL = mainreader.GetString(7)
            game.Scraper = New GameFAQScraper()
            game.Scraper.Name = mainreader.GetString(8)
            game.Scraper.URL = mainreader.GetString(9)
            '2. read game genre data
            game.Genres = New List(Of ScraperGenre)
            strSQL = "SELECT g.genreFlagName, f.genreFlagTypeName FROM"
            strSQL += " tblGameGenreFlags t INNER JOIN tblGenreFlags g on g.genreFlagId = t.genreFlagId"
            strSQL += " INNER JOIN tblGenreFlagTypes f on f.genreFlagTypeId = g.genreFlagTypeId"
            strSQL += " WHERE t.gameId = @gameId"
            secondCmd = New SQLiteCommand(strSQL, db)
            secondCmd.Parameters.Add("@gameId", DbType.Int32).Value = intGameId
            secondReader = secondCmd.ExecuteReader
            While secondReader.Read
                genre = New ScraperGenre
                genre.Name = secondReader.GetString(0)
                genre.Type = secondReader.GetString(1)
                game.Genres.Add(genre)
            End While
            secondReader.Close()
            secondCmd = Nothing
            '3. read game release data
            game.Releases = New List(Of ScraperRelease)
            strSQL = "SELECT releaseId, releaseName, regionName, publisherName, releaseProductNr, releaseDistBarcode, releaseDate, releaseRating"
            strSQL += " FROM tblReleases r INNER JOIN tblRegions i on i.regionId = r.regionId"
            strSQL += " INNER JOIN tblPublishers p on p.publisherId = r.publisherId"
            strSQL += " WHERE r.gameId = @gameId"
            secondCmd = New SQLiteCommand(strSQL, db)
            secondCmd.Parameters.Add("@gameId", DbType.Int32).Value = intGameId
            secondReader = secondCmd.ExecuteReader
            While secondReader.Read
                release = New ScraperRelease
                intReleaseId = secondReader.GetInt32(0)
                release.Title = secondReader.GetString(1)
                release.Region = secondReader.GetString(2)
                release.Publisher = secondReader.GetString(3)
                release.ProductID = secondReader.GetString(4)
                release.DistBarcode = secondReader.GetString(5)
                release.ReleaseDate = secondReader.GetString(6)
                release.Rating = secondReader.GetString(7)
                '4. read release image data
                release.Images = New List(Of ScraperImage)
                strSQL = "SELECT t.imageTypeName, i.imageSize, i.imageData FROM tblImages i INNER JOIN tblImageTypes t on t.imageTypeId = i.imageTypeId WHERE i.releaseId = @release"
                thirdCmd = New SQLiteCommand(strSQL, db)
                thirdCmd.Parameters.Add("@release", DbType.Int32).Value = intReleaseId
                thirdreader = thirdCmd.ExecuteReader
                While thirdreader.Read
                    Dim intsize As Int32
                    image = New ScraperImage
                    image.ImageType = thirdreader.GetString(0)
                    intsize = thirdreader.GetInt32(1)
                    ReDim image.ImageData(intsize)
                    thirdreader.GetBytes(2, 0, image.ImageData, 0, intsize)
                    release.Images.Add(image)
                End While
                thirdreader.Close()
                thirdCmd = Nothing
                game.Releases.Add(release)
            End While
            secondReader.Close()
            secondCmd = Nothing
            lst.Add(game)
        End While
        mainreader.Close()
        maincmd = Nothing
    End Sub
#End Region

#Region "Private Subs"
    Private Sub CreateParserTables()
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
            cmd = New SQLiteCommand("CREATE TABLE tblReleases (releaseId INTEGER PRIMARY KEY, releaseName TEXT, gameId INTEGER, regionId INTEGER, publisherId INTEGER," _
                                    + " releaseProductNr TEXT, releaseDistBarcode TEXT, releaseDate TEXT, releaseRating TEXT)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxReleaseGame ON tblReleases(gameId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxRegion ON tblReleases(regionId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxPublisher ON tblReleases(publisherId)", db)
            cmd.ExecuteNonQuery()
        End If
        If Not CheckTableExists("tblImages") Then
            cmd = New SQLiteCommand("CREATE TABLE tblImages (imageId INTEGER PRIMARY KEY, releaseId INTEGER, imageTypeId INTEGER, imageSize INTEGER, imageData BLOB)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxImageRelease ON tblImages(releaseId)", db)
            cmd.ExecuteNonQuery()
            cmd = New SQLiteCommand("CREATE INDEX idxImageType ON tblImages(imageTypeId)", db)
            cmd.ExecuteNonQuery()
        End If
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
