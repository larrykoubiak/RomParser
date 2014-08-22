Imports HtmlAgilityPack
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.Xml.Serialization
Public Class GameFAQScraper
    Inherits Scraper

    Public Overrides Sub GetAllGames(ByRef list As List(Of ScraperGame))
        Dim sys As ScraperSystem
        For Each sys In lstSystems
            GetGameList(sys, list)
        Next
    End Sub

    Public Overrides Function GetGameInfo(gameURL As String) As ScraperGame
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
        Dim game As New ScraperGame
        Dim genres As String()
        Dim release As ScraperRelease
        Dim image As ScraperImage
        Dim i As Integer
        game.ID = regex.Match(gameURL).Groups(1).Value
        game.URL = regex.Match(gameURL).Groups(2).Value
        'get main data
        html = GetHtml(gameURL)
        doc.LoadHtml(html)
        contentnode = doc.GetElementbyId("content")
        node = contentnode.SelectSingleNode("//h1[@class='page-title']/a")
        game.Title = node.InnerText
        node = contentnode.SelectSingleNode("//div[@class='body game_desc']/div[@class='desc']")
        game.Plot = node.InnerText
        'get release data
        html = GetHtml(gameURL + "/data")
        doc.LoadHtml(html)
        contentnode = doc.GetElementbyId("content")
        node = contentnode.SelectSingleNode("//div[@class='pod pod_titledata']/div/dl/dd")
        genres = WebUtility.HtmlDecode(node.InnerText).Split(">")
        game.Genres = New List(Of ScraperGenre)
        For i = 0 To genres.Length - 1
            Dim tmp As ScraperGenre
            tmp = lstGenres.Find(Function(x) x.Name = genres(i).Trim)
            If Not IsNothing(tmp) Then
                game.Genres.Add(tmp)
            End If
        Next
        node = contentnode.SelectSingleNode("//div[@class='pod pod_titledata']/div/dl/dd[2]")
        If IsNothing(node) Then
            game.Developer = ""
        Else
            game.Developer = node.InnerText
        End If
        tablenode = contentnode.SelectSingleNode("//div[@class='main_content row']/div/div[@class='pod']/div/table[@class='contrib']")
        rownodes = tablenode.SelectNodes("tbody/tr")
        i = 0
        While i < rownodes.Count
            release = New ScraperRelease
            node = rownodes.Item(i).SelectSingleNode("td[@class='cbox']/a")
            If IsNothing(node) Then
                strUrl = ""
            Else
                strUrl = "http://www.gamefaqs.com" + node.Attributes("href").Value
            End If
            node = rownodes.Item(i).SelectSingleNode("td[@class='ctitle']/b")
            release.Title = WebUtility.HtmlDecode(node.InnerText)
            i += 1
            node = rownodes.Item(i).SelectSingleNode("td[@class='cregion']")
            release.Region = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datacompany']/a")
            release.Publisher = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datapid'][1]")
            release.ProductID = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datapid'][2]")
            release.DistBarcode = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='cdate']")
            release.ReleaseDate = WebUtility.HtmlDecode(node.InnerText)
            node = rownodes.Item(i).SelectSingleNode("td[@class='datarating']")
            release.Rating = WebUtility.HtmlDecode(node.InnerText).Trim
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
                    image = New ScraperImage
                    image.ImageType = strURL2.Substring(strURL2.IndexOf("_") + 1, strURL2.IndexOf(".", strURL2.IndexOf("_")) - strURL2.IndexOf("_") - 1)
                    image.ImageData = DownloadData(strURL2)
                    release.Images.Add(image)
                Next
            End If
            game.Releases.Add(release)
        End While
        game.Scraper = Me
        Return game
    End Function

    Public Overrides Sub GetGameList(system As ScraperSystem, ByRef list As List(Of ScraperGame))
        Dim html As String
        Dim doc As New HtmlDocument
        Dim contentnode As HtmlNode
        Dim node As HtmlNode
        Dim nodes As HtmlNodeCollection
        Dim game As ScraperGame
        Dim strPages As String
        Dim intId As Integer
        Dim intCount As Integer
        Dim strURL As String
        Dim urls As New List(Of String)
        'prepare datatable
        Cursor.Current = Cursors.WaitCursor
        html = GetHtml("http://www.gamefaqs.com/" + system.URL + "/list-999")
        doc.LoadHtml(html)
        contentnode = doc.GetElementById("content")
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
            game = New ScraperGame()
            game = GetGameInfo(entry)
            game.Platform = system
            list.Add(game)
            intId += 1
            OnItemAdded("Scrapped " + game.Title, intId, intCount)
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
End Class
