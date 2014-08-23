Imports System.IO
Imports System.ComponentModel
Imports System.Xml.Serialization

Public MustInherit Class FileParser
    Public Event ItemAdded(strLabel As String, intId As Integer, intCount As Integer)
    Sub New()

    End Sub
    Public MustOverride Sub ParsePath(dir As DirectoryInfo, ByRef ds As Parser)
    Protected Sub OnItemAdded(strLabel As String, intId As Integer, intCount As Integer)
        RaiseEvent ItemAdded(strLabel, intId, intCount)
    End Sub
End Class

<Serializable()> _
Public Class ParserSoftware
    Private intSoftwareId As Int32
    Private strSoftwareName As String
    Private strManufacturer As String
    Private strPlatform As String
    Private strType As String
    Private lstFlags As List(Of ParserFlag)
    Private lstFiles As List(Of ParserZipFile)
    Public Property SoftwareId() As Int32
        Get
            Return intSoftwareId
        End Get
        Set(ByVal value As Int32)
            intSoftwareId = value
        End Set
    End Property
    Public Property SoftwareName() As String
        Get
            Return strSoftwareName
        End Get
        Set(ByVal value As String)
            strSoftwareName = value
        End Set
    End Property

    Public Property Platform() As String
        Get
            Return strPlatform
        End Get
        Set(ByVal value As String)
            strPlatform = value
        End Set
    End Property

    Public Property Manufacturer() As String
        Get
            Return strManufacturer
        End Get
        Set(ByVal value As String)
            strManufacturer = value
        End Set
    End Property
    Public Property ROMType() As String
        Get
            Return strType
        End Get
        Set(ByVal value As String)
            strType = value
        End Set
    End Property


    Public Property Flags() As List(Of ParserFlag)
        Get
            Return lstFlags
        End Get
        Set(ByVal value As List(Of ParserFlag))
            lstFlags = value
        End Set
    End Property

    Public Property Files() As List(Of ParserZipFile)
        Get
            Return lstFiles
        End Get
        Set(ByVal value As List(Of ParserZipFile))
            lstFiles = value
        End Set
    End Property
#Region "Read Only Properties"

    <XmlIgnore()>
    Public ReadOnly Property BIOS() As Boolean
        Get
            Dim flg As ParserFlag
            flg = lstFlags.Find(Function(x) x.Name = "BIOS")
            If IsNothing(flg) Then
                Return False
            Else
                Return Boolean.Parse(flg.Value)
            End If
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property Regions() As String
        Get
            Dim str As String
            Dim flags As List(Of ParserFlag)
            flags = lstFlags.FindAll(Function(x) x.Name = "Region")
            str = ""
            For Each flag As ParserFlag In flags
                str += flag.Value + ","
            Next
            If str.Length > 0 Then
                str = str.Substring(0, str.Length - 1)
            End If
            Return str
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property Languages() As String
        Get
            Dim str As String
            Dim flags As List(Of ParserFlag)
            flags = lstFlags.FindAll(Function(x) x.Name = "Language")
            str = ""
            For Each flag As ParserFlag In flags
                str += flag.Value + ","
            Next
            If str.Length > 0 Then
                str = str.Substring(0, str.Length - 1)
            End If
            Return str
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property Version() As String
        Get
            Dim flg As ParserFlag
            flg = lstFlags.Find(Function(x) x.Name = "Version")
            If IsNothing(flg) Then
                Return ""
            Else
                Return flg.Value
            End If
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property DevStatus() As String
        Get
            Dim flg As ParserFlag
            flg = lstFlags.Find(Function(x) x.Name = "DevStatus")
            If IsNothing(flg) Then
                Return ""
            Else
                Return flg.Value
            End If
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property License() As String
        Get
            Dim flg As ParserFlag
            flg = lstFlags.Find(Function(x) x.Name = "License")
            If IsNothing(flg) Then
                Return ""
            Else
                Return flg.Value
            End If
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property Year() As String
        Get
            Dim flg As ParserFlag
            flg = lstFlags.Find(Function(x) x.Name = "Date")
            If IsNothing(flg) Then
                Return ""
            Else
                Return flg.Value
            End If
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property Publisher() As String
        Get
            Dim flg As ParserFlag
            flg = lstFlags.Find(Function(x) x.Name = "Publisher")
            If IsNothing(flg) Then
                Return ""
            Else
                Return flg.Value
            End If
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property Compilation() As Boolean
        Get
            Dim flg As ParserFlag
            flg = lstFlags.Find(Function(x) x.Name = "Compilation")
            If IsNothing(flg) Then
                Return False
            Else
                Return Boolean.Parse(flg.Value)
            End If
        End Get
    End Property
    <XmlIgnore()>
    Public ReadOnly Property Demo() As Boolean
        Get
            Dim str As String
            Dim flags As List(Of ParserFlag)
            flags = lstFlags.FindAll(Function(x) x.Name = "Demo")
            str = ""
            For Each flag As ParserFlag In flags
                str += flag.Value + ","
            Next
            If str.Length > 0 Then
                str = str.Substring(0, str.Length - 1)
            End If
            Return str
        End Get
    End Property
#End Region

    Public Sub New()
        lstFlags = New List(Of ParserFlag)
        lstFiles = New List(Of ParserZipFile)
    End Sub
End Class
<Serializable()> _
Public Class ParserZipFile
    Private strFileName As String
    Private strROMSet As String
    Private strFormat As String
    Private lstFlags As List(Of ParserFlag)
    Private lstFiles As List(Of ParserArchiveFile)
    Public Property FileName() As String
        Get
            Return strFileName
        End Get
        Set(ByVal value As String)
            strFileName = value
        End Set
    End Property
    Public Property ROMSet() As String
        Get
            Return strROMSet
        End Get
        Set(ByVal value As String)
            strROMSet = value
        End Set
    End Property
    Public Property Format() As String
        Get
            Return strFormat
        End Get
        Set(value As String)
            strFormat = value
        End Set
    End Property
    Public Property Flags() As List(Of ParserFlag)
        Get
            Return lstFlags
        End Get
        Set(ByVal value As List(Of ParserFlag))
            lstFlags = value
        End Set
    End Property
    Public Property ArchiveFiles() As List(Of ParserArchiveFile)
        Get
            Return lstFiles
        End Get
        Set(value As List(Of ParserArchiveFile))
            lstFiles = value
        End Set
    End Property
    Public Sub New()
        lstFlags = New List(Of ParserFlag)
        lstFiles = New List(Of ParserArchiveFile)
    End Sub
End Class

<Serializable()> _
Public Class ParserArchiveFile
    Private strName As String
    Private strExtension As String
    <XmlAttribute("name")> _
    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(ByVal value As String)
            strName = value
        End Set
    End Property
    <XmlAttribute("extension")> _
    Public Property Extension() As String
        Get
            Return strExtension
        End Get
        Set(ByVal value As String)
            strExtension = value
        End Set
    End Property
    Public Sub New()
        strName = ""
        strExtension = ""
    End Sub
    Public Sub New(name As String, extension As String)
        strName = name
        strExtension = extension
    End Sub
End Class
<Serializable()> _
Public Class ParserFlag
    Private strName As String
    Private strValue As String
    Private strType As String
    <XmlAttribute("name")> _
    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(ByVal value As String)
            strName = value
        End Set
    End Property
    <XmlAttribute("value")> _
    Public Property Value() As String
        Get
            Return strValue
        End Get
        Set(ByVal value As String)
            strValue = value
        End Set
    End Property
    <XmlAttribute("type")> _
    Public Property FlagType() As String
        Get
            Return strType
        End Get
        Set(ByVal value As String)
            strType = value
        End Set
    End Property
    Public Sub New(name As String, value As String, type As String)
        strName = name
        strValue = value
        strType = type
    End Sub
    Public Sub New()
        strName = ""
        strValue = ""
        strType = ""
    End Sub
End Class