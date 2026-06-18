Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.IO
Imports System.Web.Script.Services

' この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class Phone
    Inherits System.Web.Services.WebService

    <WebMethod(Description:="保存手机上传的图片到D盘")>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Sub UploadPhoto()
        Dim context As HttpContext = HttpContext.Current
        Dim request As HttpRequest = context.Request
        Dim response As HttpResponse = context.Response

        ' 允许跨域（本地调试用）
        response.AppendHeader("Access-Control-Allow-Origin", "*")

        Try
            ' 1. 获取前端传过来的 id 和 sessionId
            Dim id As String = request.Form("id")
            Dim sessionId As String = request.Form("sessionId")

            ' 验证 ID 是否为空
            If String.IsNullOrEmpty(id) Then
                WriteJsonResponse(response, False, "错误：未收到关键ID")
                Return
            End If

            ' 2. 检查是否有文件
            If request.Files.Count = 0 Then
                WriteJsonResponse(response, False, "错误：未检测到图片文件")
                Return
            End If

            Dim photo As HttpPostedFile = request.Files(0)
            If photo Is Nothing OrElse photo.ContentLength = 0 Then
                WriteJsonResponse(response, False, "错误：图片内容为空")
                Return
            End If

            ' 3. 设定 D 盘保存路径
            Dim targetFolder As String = "D:\phone\"

            ' 如果 D 盘没有 phone 文件夹，则自动创建
            If Not Directory.Exists(targetFolder) Then
                Directory.CreateDirectory(targetFolder)
            End If

            ' 4. 获取文件的原始后缀名（例如 .jpg, .png）
            Dim extension As String = Path.GetExtension(photo.FileName)
            If String.IsNullOrEmpty(extension) Then
                extension = ".jpg" ' 如果拿不到后缀，默认给个 .jpg
            End If

            ' 5. 【核心】根据传过来的 ID 决定文件名
            ' 拼接出来的路径类似：D:\phone\12345.jpg
            Dim fullPath As String = Path.Combine(targetFolder, id & extension)

            ' 6. 将图片保存到 D 盘
            photo.SaveAs(fullPath)

            ' --- 如果需要转成 Base64 实时同步到电脑屏幕，保留这段，不需要可删掉 ---
            Dim base64Image As String = ""
            Using ms As New MemoryStream()
                photo.InputStream.CopyTo(ms)
                Dim fileBytes As Byte() = ms.ToArray()
                base64Image = Convert.ToBase64String(fileBytes)
            End Using
            Dim srcString As String = "data:" & photo.ContentType & ";base64," & base64Image
            ' TODO: 这里可以继续写你的 SignalR 推送代码，把 srcString 传给电脑
            ' -------------------------------------------------------------

            ' 返回成功信息
            WriteJsonResponse(response, True, "图片已成功保存到D盘，文件名：" & id & extension)

        Catch ex As Exception
            WriteJsonResponse(response, False, "服务器错误: " & ex.Message)
        End Try
    End Sub

    ' 辅助方法：返回 JSON
    Private Sub WriteJsonResponse(response As HttpResponse, success As Boolean, message As String)
        response.ContentType = "application/json"
        Dim json As String = "{""success"":" & success.ToString().ToLower() & ",""message"":""" & message & """}"
        response.Write(json)
    End Sub

End Class