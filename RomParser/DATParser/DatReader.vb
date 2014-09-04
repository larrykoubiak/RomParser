Imports System.Xml
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Public Class DatReader
    Private strFileName As String
    Private dat As datafile
    Public Sub New(fileName As String)
        Dim serial As XmlSerializer
        Dim stream As StreamReader
        Dim str As String
        strFileName = fileName
        serial = New XmlSerializer(GetType(datafile))
        stream = New StreamReader(fileName, System.Text.Encoding.UTF8)
        str = stream.ReadLine
        If str = "<?xml version=""1.0"" encoding=""utf-8""?>" Then
            dat = serial.Deserialize(stream)
        Else
            stream.Close()
            stream = New StreamReader(fileName, System.Text.Encoding.UTF8)
            ParseOldDat(stream)
        End If
        stream.Close()
    End Sub
    Private Sub ParseOldDat(ByRef stream As StreamReader)
        Dim str As String
        Dim header As datafileHeader
        Dim game As datafileGame
        Dim lstGames As List(Of datafileGame)
        Dim counter As Integer
        dat = New datafile
        str = stream.ReadLine
        If str = "clrmamepro (" Then
            header = parseHeader(stream)
            lstGames = New List(Of datafileGame)
            While Not (stream.EndOfStream)
                game = parseGame(stream)
                If Not IsNothing(game) Then lstGames.Add(game)
            End While
            ReDim dat.Items(lstGames.Count)
            dat.Items(0) = header
            counter = 1
            For Each game In lstGames
                dat.Items(counter) = game
                counter += 1
            Next
        Else
            dat = Nothing
        End If
    End Sub
    Private Function parseHeader(ByRef stream As StreamReader) As datafileHeader
        Dim regexentry As New Regex("^\t(.*?)\s(.*)$")
        Dim match As Match
        Dim header As datafileHeader
        Dim str As String
        header = New datafileHeader()
        str = stream.ReadLine
        Do Until str = ")"
            match = regexentry.Match(str)
            Select Case match.Groups(1).Value
                Case "name"
                    header.name = match.Groups(2).Value.Replace("""", "")
                Case "description"
                    header.description = match.Groups(2).Value.Replace("""", "")
                Case "category"
                    header.category = match.Groups(2).Value.Replace("""", "")
                Case "version"
                    header.version = match.Groups(2).Value.Replace("""", "")
                Case "author"
                    header.author = match.Groups(2).Value.Replace("""", "")
                Case "comment"
                    header.comment = match.Groups(2).Value.Replace("""", "")
                Case Else
            End Select
            str = stream.ReadLine
        Loop
        'skip blank line
        str = stream.ReadLine
        Return header
    End Function
    Private Function parseGame(stream As StreamReader) As datafileGame
        Dim regexgameentry As New Regex("^\t(.*?)\s(.*)$")
        Dim regexgamerom As New Regex("(name|size|crc|md5|sha1|merge|status|bios) (?<quote>\"")?(?(quote)(?<text>[^\""]*)|(?<value>[^\s]*))")
        Dim gamematch, gamerommatch As Match
        Dim gamerommatches As MatchCollection
        Dim game As New datafileGame
        Dim gamerom As datafileGameRom
        Dim lstRom As List(Of datafileGameRom)
        Dim str As String
        Dim counter As Integer
        str = stream.ReadLine
        If str = "game (" Then
            lstRom = New List(Of datafileGameRom)
            str = stream.ReadLine
            Do Until str = ")"
                gamematch = regexgameentry.Match(str)
                Select Case gamematch.Groups(1).Value
                    Case "name"
                        game.name = gamematch.Groups(2).Value.Replace("""", "")
                    Case "description"
                        game.description = gamematch.Groups(2).Value.Replace("""", "")
                    Case "year"
                        game.year = gamematch.Groups(2).Value.Replace("""", "")
                    Case "manufacturer"
                        game.manufacturer = gamematch.Groups(2).Value.Replace("""", "")
                    Case "cloneof"
                        game.cloneof = gamematch.Groups(2).Value.Replace("""", "")
                    Case "romof"
                        game.romof = gamematch.Groups(2).Value.Replace("""", "")
                    Case "isbios"
                        game.isbios = gamematch.Groups(2).Value.Replace("""", "")
                    Case "rom"
                        gamerom = New datafileGameRom
                        gamerommatches = regexgamerom.Matches(gamematch.Groups(2).Value)
                        For Each gamerommatch In gamerommatches
                            Select Case gamerommatch.Groups(1).Value
                                Case "name"
                                    gamerom.name = gamerommatch.Groups("text").Value.Replace("""", "")
                                Case "size"
                                    gamerom.size = gamerommatch.Groups("value").Value.Replace("""", "")
                                Case "crc"
                                    gamerom.crc = gamerommatch.Groups("value").Value.Replace("""", "")
                                Case "md5"
                                    gamerom.md5 = gamerommatch.Groups("value").Value.Replace("""", "")
                                Case "sha1"
                                    gamerom.sha1 = gamerommatch.Groups("value").Value.Replace("""", "")
                                Case "merge"
                                    gamerom.merge = gamerommatch.Groups("value").Value.Replace("""", "")
                                Case "status"
                                    gamerom.status = gamerommatch.Groups("value").Value.Replace("""", "")
                                Case "bios"
                                    gamerom.bios = gamerommatch.Groups("value").Value.Replace("""", "")
                            End Select
                        Next

                        lstRom.Add(gamerom)
                    Case Else

                End Select
                str = stream.ReadLine
            Loop
            ReDim game.rom(lstRom.Count - 1)
            counter = 0
            For Each gamerom In lstRom
                game.rom(counter) = gamerom
                counter += 1
            Next
        Else
            game = Nothing
        End If
        'skip blank line
        str = stream.ReadLine
        Return game
    End Function
End Class
