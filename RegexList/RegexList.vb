Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports System.IO
Imports System.Collections
Public Class RegexList
    Private lstPatterns As List(Of RegexEntry)
    Public Sub New(strPath As String)
        If File.Exists(strPath) Then
            Dim serial As New XmlSerializer(GetType(List(Of RegexEntry)))
            Dim reader As New StreamReader(strPath)
            lstPatterns = serial.Deserialize(reader)
            reader.Close()
            serial = Nothing
        End If
    End Sub
    Public Function ParseString(str As String) As List(Of RegexResult)
        Dim lst As New List(Of RegexResult)
        Dim re As RegexEntry
        For Each re In lstPatterns
            re.ParseValue(str, lst)
        Next
        Return lst
    End Function
End Class
Public Class RegexEntry
    Public Enum EntryType
        Simple
        Multiple
        List
    End Enum
    Private rgx As Regex
    Private strName As String
    Private strPattern As String
    Private intGroup As Integer
    Private enType As EntryType
    Private strSeparator As String
    Private lstChild As List(Of RegexEntry)
    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(ByVal value As String)
            strName = value
        End Set
    End Property
    Public Property Pattern() As String
        Get
            Return strPattern
        End Get
        Set(ByVal value As String)
            strPattern = value
            rgx = New Regex(strPattern)
        End Set
    End Property
    Public Property Group() As Integer
        Get
            Return intGroup
        End Get
        Set(ByVal value As Integer)
            intGroup = value
        End Set
    End Property
    Public ReadOnly Property Regex() As Regex
        Get
            Return rgx
        End Get
    End Property
    Public Property Type() As EntryType
        Get
            Return enType
        End Get
        Set(ByVal value As EntryType)
            enType = value
        End Set
    End Property
    Public Property Separator() As String
        Get
            Return strSeparator
        End Get
        Set(ByVal value As String)
            strSeparator = value
        End Set
    End Property
    Public Property Items() As List(Of RegexEntry)
        Get
            Return lstChild
        End Get
        Set(ByVal value As List(Of RegexEntry))
            lstChild = value
        End Set
    End Property

    Public Sub New()
        rgx = Nothing
        strName = ""
        strPattern = ""
        intGroup = 0
        enType = EntryType.Simple
        strSeparator = ""
    End Sub
    Public Sub New(name As String, pattern As String, groupId As Integer, type As EntryType, Optional separator As String = "")
        strName = name
        strPattern = pattern
        intGroup = groupId
        enType = type
        strSeparator = separator
        rgx = New Regex(strPattern)
    End Sub

    Public Sub ParseValue(str As String, ByRef lst As List(Of RegexResult))
        Select Case enType
            Case RegexEntry.EntryType.Simple
                Dim match As Match
                match = rgx.Match(str)
                If match.Groups(intGroup).Success Then
                    lst.Add(New RegexResult(strName, match.Groups(intGroup).Value.Trim))
                End If
            Case RegexEntry.EntryType.Multiple
                Dim result As New RegexResult(strName, "")
                result.Items = New List(Of RegexResult)
                For Each entry As RegexEntry In lstChild
                    entry.ParseValue(str, result.Items)
                Next
                lst.Add(result)
            Case RegexEntry.EntryType.List
                Dim match As Match
                Dim strings() As String
                match = rgx.Match(str)
                If match.Groups(intGroup).Success Then
                    strings = match.Groups(intGroup).Value.Split(strSeparator)
                    For i = 0 To strings.Length - 1
                        lst.Add(New RegexResult(strName, strings(i).Trim))
                    Next
                End If
        End Select
    End Sub
End Class

Public Class RegexResult
    Private strName As String
    Private strValue As String
    Private lstChild As List(Of RegexResult)
    Public Property Items() As List(Of RegexResult)
        Get
            Return lstChild
        End Get
        Set(ByVal value As List(Of RegexResult))
            lstChild = value
        End Set
    End Property

    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(ByVal value As String)
            strName = value
        End Set
    End Property
    Public Property Value() As String
        Get
            Return strValue
        End Get
        Set(ByVal value As String)
            strValue = value
        End Set
    End Property
    Public Sub New()
        strName = ""
        strValue = ""
    End Sub
    Public Sub New(name As String, value As String)
        strName = name
        strValue = value
    End Sub
End Class