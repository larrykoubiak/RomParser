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
        gameGenreFlagsHT = New Hashtable
        For Each gameGenreFlag In game.GetGameGenreFlagsRows()
            gameGenreFlagsHT.Add(gameGenreFlag.genreFlagId, gameGenreFlag)
        Next
        gameReleasesHT = New Hashtable
        For Each release In game.GetReleasesRows()
            gameReleasesHT.Add(release.releaseName + "(" + release.releaseProductNr + ")", release)
        Next
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
            releaseImagesHT = New Hashtable
            For Each image In release.GetImagesRows()
                releaseImagesHT.Add(image.ImageTypesRow.imageTypeName + "(" + image.imageSize.ToString + ")", image)
            Next
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
        Dim game As Scraper.GamesRow
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
        platform = GetPlatform(system.Name, system.URL, system.Acronym, ds)
        platformGamesHT = New Hashtable
        For Each game In platform.GetGamesRows()
            platformGamesHT.Add(game.gameName, game)
        Next
        For Each entry In urls
            GetGameInfo(entry, platform, ds)
        Next
        Cursor.Current = Cursors.Default
    End Sub

    Public Sub New(path As String, ByRef ds As Scraper)
        MyBase.New(path, ds)
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
End Class
