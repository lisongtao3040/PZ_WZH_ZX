Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Text
Imports System.Data
Imports System.Collections.Generic
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports System.Net
Imports System.Xml
Imports SqlHelper.SqlHelper
Imports SqlHelper

<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class FullTrayListApi
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' 取得满托盘一览数据
    ''' </summary>
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetData() As String
        Dim jss As New JavaScriptSerializer()
        Try
            Dim BC As New t_FullTrayListBC()
            Dim dt As DataTable = BC.GetFullTrayListWithResult()

            Dim list As New List(Of Dictionary(Of String, Object))
            For Each row As DataRow In dt.Rows
                Dim item As New Dictionary(Of String, Object)
                item("innerCodeDes") = row("innerCodeDes").ToString()
                item("stationNo") = row("stationNo").ToString()
                item("stationUse") = row("stationUse").ToString()
                item("trayNo") = row("trayNo").ToString()
                item("Ttxt") = row("Ttxt").ToString()
                item("OrderNo") = row("OrderNo").ToString()
                item("sapCode") = row("sapCode").ToString()
                item("packageAmount") = row("packageAmount").ToString()
                item("destination") = row("destination").ToString()
                item("jizhong") = row("jizhong").ToString()
                item("lineCodeShort") = row("lineCodeShort").ToString()
                item("line_name") = row("line_name").ToString()
                item("result") = row("result").ToString()
                list.Add(item)
            Next

            Return jss.Serialize(New With {.success = True, .data = list})
        Catch ex As Exception
            Return jss.Serialize(New With {.success = False, .message = ex.Message, .data = New List(Of Object)()})
        End Try
    End Function

    ''' <summary>
    ''' 呼叫 AGV 新任务（SOAP 方式）
    ''' </summary>
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function CallAgv(ByVal callIEQ As String, ByVal trolleyNo As String, ByVal stationNo1 As String,
                            ByVal stationNo2 As String, ByVal opUserID As String, ByVal opUserName As String) As String
        Dim jss As New JavaScriptSerializer()
        Try
            Dim soap As String =
                "<?xml version=""1.0"" encoding=""utf-8""?>" &
                "<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">" &
                "<soap:Body><taskCall_New xmlns=""http://tempuri.org/"">" &
                "<callIEQ>" & XmlEsc(callIEQ) & "</callIEQ>" &
                "<callTaskTypeID>98</callTaskTypeID>" &
                "<trolleyNo>" & XmlEsc(trolleyNo) & "</trolleyNo>" &
                "<stationNo1>" & XmlEsc(stationNo1) & "</stationNo1>" &
                "<stationNo2>" & XmlEsc(stationNo2) & "</stationNo2>" &
                "<dueTime></dueTime>" &
                "<opUserID>" & XmlEsc(opUserID) & "</opUserID>" &
                "<opUserName>" & XmlEsc(opUserName) & "</opUserName>" &
                "<TrolleyType></TrolleyType>" &
                "</taskCall_New></soap:Body></soap:Envelope>"

            Dim req As HttpWebRequest = CType(WebRequest.Create("http://10.160.192.20:8083/webSerAgvCall.asmx"), HttpWebRequest)
            req.Method = "POST"
            req.ContentType = "text/xml; charset=utf-8"
            req.Headers("SOAPAction") = """http://tempuri.org/taskCall_New"""

            Dim bytes As Byte() = Encoding.UTF8.GetBytes(soap)
            req.ContentLength = bytes.Length
            Using stream = req.GetRequestStream()
                stream.Write(bytes, 0, bytes.Length)
            End Using

            Using resp As HttpWebResponse = CType(req.GetResponse(), HttpWebResponse)
                Using reader As New IO.StreamReader(resp.GetResponseStream(), Encoding.UTF8)
                    Dim xml As String = reader.ReadToEnd()
                    Dim doc As New XmlDocument()
                    doc.LoadXml(xml)
                    Dim outMess As String = If(doc.GetElementsByTagName("outMess").Count > 0, doc.GetElementsByTagName("outMess")(0).InnerText, "")
                    Dim outBl As String = If(doc.GetElementsByTagName("outBl").Count > 0, doc.GetElementsByTagName("outBl")(0).InnerText, "")
                    Return jss.Serialize(New With {.success = True, .outBl = outBl, .outMess = outMess})
                End Using
            End Using
        Catch ex As Exception
            Return jss.Serialize(New With {.success = False, .message = ex.Message})
        End Try
    End Function

    Private Function XmlEsc(val As String) As String
        Return System.Security.SecurityElement.Escape(If(val, ""))
    End Function

End Class
