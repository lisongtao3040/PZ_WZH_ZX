Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.ComponentModel
Imports System.Data
Imports System.Net

Public Class TPVB
    Public client As HttpClient = Nothing
    Public url As String = "http://10.160.192.116:8001/"

    Public Sub New()
        'client = New HttpClient With {
        '    .BaseAddress = New Uri(url)
        '}

        'Dim client As New HttpClient()
        client = New HttpClient()
        client.BaseAddress = New Uri(url)
        'client.BaseAddress = New Uri("")
    End Sub

    Public Function InsertTrayDataTest() As String
        Dim n As DateTime = DateTime.Now
        Dim model As Root = New Root With {
            .fid = "dd",
            .formid = "sdfdf",
            .ttxt = "sdd",
            .no1 = "ddd",
            .no2 = "222",
            .cuserid = "dd",
            .cusername = "ddd",
            .[cdate] = "2023-05-15 15:47:02.243"
        }
        Dim Data = Post(client.BaseAddress.ToString & "Tray/InsertTrayData", model)

        If String.IsNullOrEmpty(Data) Then
            Return ""
        Else
            Return Data.ToString()
        End If
    End Function

    Public Function GetListByTrayTest2() As String
        Dim n As DateTime = DateTime.Now
        Dim model As RootTrayNo = New RootTrayNo With {
            .tray = "300099"
        }
        Dim Data = PostGetListByTray(client.BaseAddress.ToString & "Tray/GetListByTray", model)

        If String.IsNullOrEmpty(Data) Then
            Return ""
        Else
            Return Data.ToString()
        End If
    End Function


    Public Function GetListByTray(ByVal tray As String) As String
        Dim n As DateTime = DateTime.Now
        Dim model As RootTrayNo = New RootTrayNo With {
            .tray = tray
        }
        Dim Data = PostGetListByTray(client.BaseAddress.ToString & "Tray/GetListByTray", model)

        If String.IsNullOrEmpty(Data) Then
            Return ""
        Else
            Return Data.ToString()
        End If
    End Function

    Public Function PostGetListByTray(ByVal url As String, ByVal data As RootTrayNo) As String
        Try
            Dim jsonData = JsonConvert.SerializeObject(data)
            Dim content As HttpContent = New StringContent(jsonData)
            content.Headers.ContentType = New System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            Dim res As HttpResponseMessage = client.PostAsync(url, content).Result

            If res.StatusCode = System.Net.HttpStatusCode.OK Then
                Dim resMsgStr As String = res.Content.ReadAsStringAsync().Result
                Return resMsgStr
            Else
                Return Nothing
            End If

        Catch ex As Exception
            Return Nothing
        End Try
    End Function



    Public Function InsertTrayData(ByVal no As String, ByVal formid As String, ByVal qr As String, ByVal no1 As String, ByVal user_id As String, ByVal user_name As String) As String
        Dim n As DateTime = DateTime.Now
        'Dim model As Root = New Root With {
        '    .fid = "dd",
        '    .formid = "sdfdf",
        '    .ttxt = "sdd",
        '    .no1 = "ddd",
        '    .no2 = "222",
        '    .cuserid = "dd",
        '    .cusername = "ddd",
        '    .[cdate] = "2023-05-15 15:47:02.243"
        '}
        Dim model As Root = New Root With {
            .fid = no & n.ToString("yyMMddHHmmssfff"),
            .formid = formid,
            .ttxt = qr,
            .no1 = no1,
            .no2 = “”,
            .cuserid = user_id,
            .cusername = user_name,
            .[cdate] = n.ToString("yyyy-MM-dd HH:mm:ss.fff")
        }

        Dim Data = Post(client.BaseAddress.ToString & "Tray/InsertTrayData", model)

        If String.IsNullOrEmpty(Data) Then
            Return ""
        Else
            Return Data.ToString()
        End If
    End Function

    Public Function UpdateTrayData(ByVal no As String, ByVal formid As String, ByVal qr As String, ByVal no1 As String, ByVal no2 As String, ByVal user_id As String, ByVal user_name As String) As String
        Dim n As DateTime = DateTime.Now
        Dim model As Root = New Root With {
            .fid = no & n.ToString("yyMMddHHmmssfff"),
            .formid = formid,
            .ttxt = qr,
            .no1 = no1,
            .no2 = no2,
            .cuserid = user_id,
            .cusername = user_name,
            .[cdate] = n.ToString("yyyy-MM-dd HH:mm:ss.fff")
        }
        Dim Data = Post(client.BaseAddress.ToString & "Tray/UpdateTrayData", model)

        If String.IsNullOrEmpty(Data) Then
            Return ""
        Else
            Return Data.ToString()
        End If
    End Function

    Public Function Post(ByVal url As String, ByVal data As Root) As String
        Try
            Dim jsonData = JsonConvert.SerializeObject(data)
            Dim content As HttpContent = New StringContent(jsonData)
            content.Headers.ContentType = New System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            Dim res As HttpResponseMessage = client.PostAsync(url, content).Result

            If res.StatusCode = System.Net.HttpStatusCode.OK Then
                Dim resMsgStr As String = res.Content.ReadAsStringAsync().Result
                Return resMsgStr
            Else
                Return Nothing
            End If

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class

Public Class Root
    Public Property fid As String
    Public Property formid As String
    Public Property ttxt As String
    Public Property no1 As String
    Public Property no2 As String
    Public Property cuserid As String
    Public Property cusername As String
    Public Property [cdate] As String
End Class


Public Class RootTrayNo
    Public Property tray As String
End Class
