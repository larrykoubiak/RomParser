Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
Imports HtmlAgilityPack
Public MustInherit Class GameScraper
    Protected lstSystems As List(Of ScraperSystem)
    Protected lstGenres As List(Of ScraperGenre)
    Protected strName As String
    Protected strURL As String
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
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

End Class
<Serializable()> _
Public Class ScraperGame
    Private intID As Integer
    Private strTitle As String
    Private sysPlatform As ScraperSystem
    Private strURL As String
    Private strPlot As String
    Private lstGenres As List(Of ScraperGenre)
    Private strDeveloper As String
    Private lstReleases As List(Of ScraperRelease)
    Private spScraper As GameScraper

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
    Private strName As String
    Private intID As Integer
    Private strAcronym As String
    Private strURL As String
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
    Private strName As String
    Private gtType As GenreType
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
    Private strTitle As String
    Private strRegion As String
    Private strPublisher As String
    Private strProductID As String
    Private strDistBarcode As String
    Private strReleaseDate As String
    Private strRating As String
    Private lstImages As List(Of ScraperImage)
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
    Private strURL As String
    Private siType As ScraperImageType
    Private baImageData As Byte()
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