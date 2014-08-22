Imports System.IO
Imports System.Xml
Imports System.Xml.Serialization
<Serializable()> Public Class game
    Private strTitle As String
    Private strPlatform As String
    Private intYear As Integer
    Private strPublisher As String
    Private strGenre As String
    Private strPlot As String
    <XmlElement(Order:=6)>
    Public Property plot() As String
        Get
            Return strPlot
        End Get
        Set(ByVal value As String)
            strPlot = value
        End Set
    End Property
    <XmlElement(Order:=5)>
    Public Property genre() As String
        Get
            Return strGenre
        End Get
        Set(ByVal value As String)
            strGenre = value
        End Set
    End Property
    <XmlElement(Order:=4)>
    Public Property publisher() As String
        Get
            Return strPublisher
        End Get
        Set(ByVal value As String)
            strPublisher = value
        End Set
    End Property
    <XmlElement(Order:=3)>
    Public Property year() As Integer
        Get
            Return intYear
        End Get
        Set(ByVal value As Integer)
            intYear = value
        End Set
    End Property
    <XmlElement(Order:=2)>
    Public Property platform() As String
        Get
            Return strPlatform
        End Get
        Set(ByVal value As String)
            strPlatform = value
        End Set
    End Property
    <XmlElement(Order:=1)>
    Public Property title() As String
        Get
            Return strTitle
        End Get
        Set(ByVal value As String)
            strTitle = value
        End Set
    End Property
    Public Sub Serialize()
        Dim settings As New System.Xml.XmlWriterSettings
        Dim namespaces As New XmlSerializerNamespaces
        Dim serializer As New XmlSerializer(GetType(game))
        Dim writer As XmlWriter
        settings.OmitXmlDeclaration = True
        settings.Indent = True
        namespaces.Add(String.Empty, String.Empty)
        writer = XmlWriter.Create("C:\Temp\" + Me.title + ".xml", settings)
        serializer.Serialize(writer, Me, namespaces)
        writer.Close()
        writer = Nothing
        serializer = Nothing
        namespaces = Nothing
        settings = Nothing
    End Sub
End Class
