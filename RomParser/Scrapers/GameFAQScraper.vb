Imports HtmlAgilityPack
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Xml.Serialization
Public Class GameFAQScraper
    Inherits GameScraper
    Dim intId As Integer
    Dim intCount As Integer

    Public Overrides Sub GetAllGames(ByRef ds As Scraper)
        Dim sys As ScraperSystem
        For Each sys In lstSystems
            GetGameList(sys, ds)
        Next
    End Sub

    Public Overrides Sub GetGameInfo(gameURL As String, platform As Scraper.PlatformsRow, ByRef ds As Scraper)
        Dim html As String
        Dim strUrl As String
        Dim regex As New Regex("\/(?:.*?)\/(\d*?)-(.*)$")
        Dim doc As New HtmlDocument
        Dim imagedoc As New HtmlDocument
        Dim tablenode As HtmlNode
        Dim rownodes As HtmlNodeCollection
        Dim imagenodes As HtmlNodeCollection
        Dim node As HtmlNode
        Dim contentnode As HtmlNode
        Dim imagecontentnode As HtmlNode
        Dim developer As Scraper.DevelopersRow
        Dim scraper As Scraper.ScrapersRow
        Dim game As Scraper.GamesRow
        Dim genres As String()
        Dim genreFlagType As Scraper.GenreFlagTypesRow
        Dim genreFlag As Scraper.GenreFlagsRow
        Dim gameGenreFlag As Scraper.GameGenreFlagsRow
        Dim release As Scraper.ReleasesRow
        Dim region As Scraper.RegionsRow
        Dim publisher As Scraper.PublishersRow
        Dim imageType As Scraper.ImageTypesRow
        Dim image As Scraper.ImagesRow
        Dim strGameId, strGameName, strGameURL, strGamePlot, strDeveloperName
        Dim strRegionName, strPublisherName, strReleaseName, strReleaseProductId, strReleaseDistBarcode, strReleaseDate, strReleaseRating As String
        Dim strImageTypeName As String
        Dim lngImageSize As Long
        Dim bytesImageData() As Byte
        Dim i As Integer
        strGameId = regex.Match(gameURL).Groups(1).Value
        strGameURL = regex.Match(gameURL).Groups(2).Value
        'get main data
        html = GetHtml(gameURL)
        doc.LoadHtml(html)
        contentnode = doc.GetElementbyId("content")
        node = contentnode.SelectSingleNode("//h1[@class='page-title']/a")
        strGameName = node.InnerText
        node = contentnode.SelectSingleNode("//div[@class='body game_desc']/div[@class='desc']")
        strGamePlot = node.InnerText
        'get release data
        html = GetHtml(gameURL + "/data")
        doc.LoadHtml(html)
        contentnode = doc.GetElementbyId("content")
        'get developer
        node = contentnode.SelectSingleNode("//div[@class='pod pod_titledata']/div/dl/dd[2]")
        If IsNothing(node) Then
            strDeveloperName = ""
        Else
            strDeveloperName = node.InnerText
        End If
        developer = GetDeveloper(strDeveloperName, ds)
        scraper = GetScraper("GameFAQS", "http://www.gamefaqs/com", ds)
        game = GetGame(strGameName, strGameURL, strGamePlot, developer, platform, scraper, ds)
        'get genres
        node = contentnode.SelectSingleNode("//div[@class='pod pod_titledata']/div/dl/dd")
        genres = WebUtility.HtmlDecode(node.InnerText).Split(">")
        For i = 0 To genres.Length - 1
            Dim tmp As ScraperGenre
            tmp = lstGenres.Find(Function(x) x.Name = genres(i).Trim)
            If Not IsNothing(tmp) Then
                genreFlagType = GetGenreFlagType(tmp.Type, ds)
                genreFlag = GetGenreFlag(tmp.Name, genreFlagType, ds)
                gameGenreFlag = GetGameGenreFlag(game, genreFlag, ds)
            End If
        Next
        'get releases
        tablenode = contentnode.SelectSingleNode("//div[@class='main_content row']/div/div[@class='pod']/div/table[@class='contrib']")
        rownodes = tablenode.SelectNodes("tbody/tr")
        i = 0
        While i < rownodes.Count
            node = rownodes.Item(i).SelectSingleNode("td[@class='cbox']/a")
            If IsNothing(node) Then
                strUrl = ""
            Else
                strUrl = "http://www.gamefaqs.com" + node.Attributes("href").Value
            End If
            node = rownodes.Item(i).SelectSingleNode("td[@class='ctitle']/b")
            strReleaseName = WebUtility.HtmlDecode(node.InnerText)
            i += 1
            node = rownodes.Item(i).SelectSingleNode("td[@class='cregion']")
            strRegionName = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datacompany']/a")
            strPublisherName = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datapid'][1]")
            strReleaseProductId = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datapid'][2]")
            strReleaseDistBarcode = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='cdate']")
            strReleaseDate = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datarating']")
            strReleaseRating = WebUtility.HtmlDecode(node.InnerText).Trim
            region = GetRegion(strRegionName, ds)
            publisher = GetPublisher(strPublisherName, ds)
            release = GetRelease(strReleaseName, strReleaseProductId, strReleaseDistBarcode, strReleaseDate, strReleaseRating, game, region, publisher, ds)
            i += 1
            'get release images
            If strUrl.Length > 0 Then
                html = GetHtml(strUrl)
                imagedoc.LoadHtml(html)
                imagecontentnode = imagedoc.GetElementbyId("content")
                imagenodes = imagecontentnode.SelectNodes("//div[@class='pod game_imgs']/div/div[@class='img']/a")
                For Each node In imagenodes
                    Dim strURL2 As String
                    strURL2 = node.Attributes("href").Value
                    strImageTypeName = strURL2.Substring(strURL2.IndexOf("_") + 1, strURL2.IndexOf(".", strURL2.IndexOf("_")) - strURL2.IndexOf("_") - 1)
                    bytesImageData = DownloadData(strURL2)
                    lngImageSize = bytesImageData.Length
                    imageType = GetImageType(strImageTypeName, ds)
                    image = GetImage(lngImageSize, bytesImageData, release, imageType, ds)
                Next
            End If
        End While
        intId += 1
        OnItemAdded("Scrapped " + game.gameName, intId, intCount)
    End Sub

    Public Overrides Sub GetGameList(system As ScraperSystem, ByRef ds As Scraper)
        Dim html As String
        Dim doc As New HtmlDocument
        Dim contentnode As HtmlNode
        Dim node As HtmlNode
        Dim nodes As HtmlNodeCollection
        Dim platform As Scraper.PlatformsRow
        Dim strPages As String
        Dim strURL As String
        Dim urls As New List(Of String)
        'prepare datatable
        Cursor.Current = Cursors.WaitCursor
        html = GetHtml("http://www.gamefaqs.com/" + system.URL + "/list-999")
        doc.LoadHtml(html)
        contentnode = doc.GetElementbyId("content")
        node = contentnode.SelectSingleNode("//ul[@class='paginate']/li")
        strPages = node.InnerText.Trim
        intCount = Integer.Parse(strPages.Substring(strPages.IndexOf("of") + 3))
        intId = 0
        While intId < intCount
            node = contentnode.SelectSingleNode("//table[@class='results']")
            nodes = node.SelectNodes("tr")
            For Each node In nodes
                strURL = "http://www.gamefaqs.com/" + node.SelectSingleNode("td[@class='rtitle']/a").Attributes("href").Value
                urls.Add(strURL)
            Next
            intId += 1
            OnItemAdded("Retrieving list: " + system.Name, intId, intCount)
            If intId < intCount Then
                html = GetHtml("http://www.gamefaqs.com/" + system.URL + "/list-999?page=" + intId.ToString)
                doc.LoadHtml(html)
            End If
        End While
        intCount = urls.Count
        intId = 0
        For Each entry In urls
            platform = GetPlatform(system.Name, system.URL, system.Acronym, ds)
            GetGameInfo(entry, platform, ds)
        Next
        Cursor.Current = Cursors.Default
    End Sub

    Public Sub New()
        Dim serializer As New XmlSerializer(GetType(List(Of ScraperSystem)))
        Dim reader As XmlReader
        strName = "GameFAQs"
        strURL = "http://www.gamefaqs.com/"
        reader = XmlReader.Create("Scrapers\\GameFAQSystems.xml")
        lstSystems = serializer.Deserialize(reader)
        reader.Close()
        reader = XmlReader.Create("Scrapers\\GameFAQGenres.xml")
        serializer = New XmlSerializer(GetType(List(Of ScraperGenre)))
        lstGenres = serializer.Deserialize(reader)
        reader.Close()
        reader = Nothing
        serializer = Nothing
    End Sub

    Private Function GetScraper(strScraperName As String, strScraperURL As String, ByRef ds As Scraper) As Scraper.ScrapersRow
        Dim scraper As Scraper.ScrapersRow()
        scraper = ds.Scrapers.Select("scraperName = '" + strScraperName.Replace("'", "''") + "' And scraperURL='" + strScraperURL.Replace("'", "''") + "'")
        If scraper.Length = 0 Then
            Return ds.Scrapers.AddScrapersRow(strScraperName, strScraperURL)
        Else
            Return scraper(0)
        End If
    End Function
    Private Function GetDeveloper(strdeveloperName As String, ByRef ds As Scraper) As Scraper.DevelopersRow
        Dim developer As Scraper.DevelopersRow()
        developer = ds.Developers.Select("developerName = '" + strdeveloperName.Replace("'", "''") + "'")
        If developer.Length = 0 Then
            Return ds.Developers.AddDevelopersRow(strdeveloperName)
        Else
            Return developer(0)
        End If
    End Function
    Private Function GetPlatform(strPlatformName As String, strPlatformURL As String, strPlatformAcronym As String, ByRef ds As Scraper) As Scraper.PlatformsRow
        Dim platform As Scraper.PlatformsRow()
        platform = ds.Platforms.Select("platformName = '" + strPlatformName.Replace("'", "''") + "' And platformURL = '" + strPlatformURL.Replace("'", "''") + _
                                       "' and platformAcronym = '" + strPlatformAcronym.Replace("'", "''") + "'")
        If platform.Length = 0 Then
            Return ds.Platforms.AddPlatformsRow(strPlatformName, strPlatformURL, strPlatformAcronym)
        Else
            Return platform(0)
        End If
    End Function
    Private Function GetGame(strGameName As String, strGameURL As String, strGamePlot As String, developer As Scraper.DevelopersRow, platform As Scraper.PlatformsRow, _
                             scraper As Scraper.ScrapersRow, ByRef ds As Scraper) As Scraper.GamesRow
        Dim game As Scraper.GamesRow()
        game = ds.Games.Select("gameName = '" + strGameName.Replace("'", "''") + "' And gameURL = '" + strGameURL.Replace("'", "''") + _
                                       "' and gamePlot = '" + strGamePlot.Replace("'", "''") + "' and developerId=" + developer.developerId.ToString + _
                                       " and platformId=" + platform.platformId.ToString + " and scraperId=" + scraper.scraperId.ToString)
        If game.Length = 0 Then
            Return ds.Games.AddGamesRow(strGameName, strGameURL, strGamePlot, developer, platform, scraper)
        Else
            Return game(0)
        End If
    End Function
    Private Function GetGenreFlagType(strGenreFlagTypeName As String, ByRef ds As Scraper) As Scraper.GenreFlagTypesRow
        Dim genreFlagType As Scraper.GenreFlagTypesRow()
        genreFlagType = ds.GenreFlagTypes.Select("genreFlagTypeName = '" + strGenreFlagTypeName.Replace("'", "''") + "'")
        If genreFlagType.Length = 0 Then
            Return ds.GenreFlagTypes.AddGenreFlagTypesRow(strGenreFlagTypeName)
        Else
            Return genreFlagType(0)
        End If
    End Function
    Private Function GetGenreFlag(strGenreFlagName As String, genreFlagType As Scraper.GenreFlagTypesRow, ByRef ds As Scraper) As Scraper.GenreFlagsRow
        Dim genreFlag As Scraper.GenreFlagsRow()
        genreFlag = ds.GenreFlags.Select("genreFlagName = '" + strGenreFlagName.Replace("'", "''") + "' and genreFlagTypeId = " + genreFlagType.genreFlagTypeId.ToString)
        If genreFlag.Length = 0 Then
            Return ds.GenreFlags.AddGenreFlagsRow(strGenreFlagName, genreFlagType)
        Else
            Return genreFlag(0)
        End If
    End Function
    Private Function GetGameGenreFlag(game As Scraper.GamesRow, genreFlag As Scraper.GenreFlagsRow, ByRef ds As Scraper) As Scraper.GameGenreFlagsRow
        Dim gameGenreFlag As Scraper.GameGenreFlagsRow()
        gameGenreFlag = ds.GameGenreFlags.Select("gameId = " + game.gameId.ToString + " and genreFlagId = " + genreFlag.genreFlagId.ToString)
        If gameGenreFlag.Length = 0 Then
            Return ds.GameGenreFlags.AddGameGenreFlagsRow(game, genreFlag)
        Else
            Return gameGenreFlag(0)
        End If
    End Function
    Private Function GetRegion(strRegionName As String, ByRef ds As Scraper) As Scraper.RegionsRow
        Dim region As Scraper.RegionsRow()
        region = ds.Regions.Select("regionName = '" + strRegionName.Replace("'", "''") + "'")
        If region.Length = 0 Then
            Return ds.Regions.AddRegionsRow(strRegionName)
        Else
            Return region(0)
        End If
    End Function
    Private Function GetPublisher(strPublisherName As String, ByRef ds As Scraper) As Scraper.PublishersRow
        Dim publisher As Scraper.PublishersRow()
        publisher = ds.Publishers.Select("publisherName = '" + strPublisherName.Replace("'", "''") + "'")
        If publisher.Length = 0 Then
            Return ds.Publishers.AddPublishersRow(strPublisherName)
        Else
            Return publisher(0)
        End If
    End Function
    Private Function GetRelease(strReleaseName As String, strReleaseProductId As String, strReleaseDistBarcode As String, strReleaseDate As String, strReleaseRating As String, _
                                game As Scraper.GamesRow, region As Scraper.RegionsRow, publisher As Scraper.PublishersRow, ByRef ds As Scraper) As Scraper.ReleasesRow
        Dim release As Scraper.ReleasesRow()
        release = ds.Releases.Select("releaseName = '" + strReleaseName.Replace("'", "''") + "' And releaseProductNr = '" + strReleaseProductId.Replace("'", "''") + _
                                       "' and releaseDistBarcode = '" + strReleaseDistBarcode.Replace("'", "''") + "' and releaseDate = '" + strReleaseDate.Replace("'", "''") + _
                                       "' and releaseRating = '" + strReleaseRating.Replace("'", "''") + "' and gameId=" + game.gameId.ToString + " and regionId=" + region.regionId.ToString + _
                                       " and publisherId=" + publisher.publisherId.ToString)
        If release.Length = 0 Then
            Return ds.Releases.AddReleasesRow(strReleaseName, strReleaseProductId, strReleaseDistBarcode, strReleaseDate, strReleaseRating, game, region, publisher)
        Else
            Return release(0)
        End If
    End Function
    Private Function GetImageType(strImageTypeName As String, ByRef ds As Scraper) As Scraper.ImageTypesRow
        Dim imageType As Scraper.ImageTypesRow()
        imageType = ds.ImageTypes.Select("imageTypeName = '" + strImageTypeName.Replace("'", "''") + "'")
        If imageType.Length = 0 Then
            Return ds.ImageTypes.AddImageTypesRow(strImageTypeName)
        Else
            Return imageType(0)
        End If
    End Function
    Private Function GetImage(lngImageSize As Long, bytesImageData As Byte(), release As Scraper.ReleasesRow, imageType As Scraper.ImageTypesRow, ByRef ds As Scraper) As Scraper.ImagesRow
        Dim image As Scraper.ImagesRow()
        image = ds.Images.Select("imageSize = " + lngImageSize.ToString + " and releaseId = " + release.releaseId.ToString + " and imageTypeId = " + imageType.imageTypeId.ToString)
        If image.Length = 0 Then
            Return ds.Images.AddImagesRow(lngImageSize, bytesImageData, release, imageType)
        Else
            Return image(0)
        End If
    End Function
End Class
