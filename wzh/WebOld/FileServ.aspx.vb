Imports System.IO
Imports System.Configuration
Imports System.Web.Configuration
Imports Ionic.Zip

Partial Class FileServ
    Inherits System.Web.UI.Page

    Private ReadOnly Property FileServerPath() As String
        Get
            Return ConfigurationManager.AppSettings("FileServerPath")
        End Get
    End Property

    Private Property CurrentPath() As String
        Get
            If ViewState("CurrentPath") Is Nothing Then
                ViewState("CurrentPath") = ""
            End If
            Return ViewState("CurrentPath").ToString()
        End Get
        Set(ByVal value As String)
            ViewState("CurrentPath") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' 获取当前路径参数
            If Not String.IsNullOrEmpty(Request.QueryString("path")) Then
                CurrentPath = Request.QueryString("path")
            End If

            ' 检查根文件夹是否存在
            If Not Directory.Exists(FileServerPath) Then
                Try
                    Directory.CreateDirectory(FileServerPath)
                Catch ex As Exception
                    ShowMessage("无法创建根文件夹: " & ex.Message, False)
                End Try
            End If

            LoadFileList()
            UpdatePathNavigation()
        End If

        ' 注册客户端脚本，在页面加载完成后隐藏遮罩
        If Not ClientScript.IsStartupScriptRegistered("HideWaitOverlay") Then
            ClientScript.RegisterStartupScript(Me.GetType(), "HideWaitOverlay",
                "try{hideWaitOverlay();}catch(e){}", True)
        End If
    End Sub

    Private Sub UpdatePathNavigation()
        ' 更新路径导航
        Dim fullPath As String = Path.Combine(FileServerPath, CurrentPath)
        Dim relativePath As String = ""
        Dim pathParts As String() = CurrentPath.Split("\"c)

        lblPath.Text = ""
        For i As Integer = 0 To pathParts.Length - 1
            If i > 0 Then
                relativePath += "\"
            End If
            relativePath += pathParts(i)

            If i < pathParts.Length - 1 Then
                lblPath.Text += " \ " & pathParts(i)
            Else
                lblPath.Text += " \ <strong>" & pathParts(i) & "</strong>"
            End If
        Next
    End Sub

    Private Sub LoadFileList()
        Try
            Dim fullPath As String = Path.Combine(FileServerPath, CurrentPath)
            Dim directoryInfo As New DirectoryInfo(fullPath)

            Dim dt As New Data.DataTable()
            dt.Columns.Add("Name", GetType(String))
            dt.Columns.Add("FullPath", GetType(String))
            dt.Columns.Add("IsFolder", GetType(Boolean))
            dt.Columns.Add("Size", GetType(String))
            dt.Columns.Add("ModifiedDate", GetType(String))

            ' 添加子文件夹
            For Each dir As DirectoryInfo In directoryInfo.GetDirectories()
                Dim row As Data.DataRow = dt.NewRow()
                row("Name") = "[文件夹] " & dir.Name
                row("FullPath") = Path.Combine(CurrentPath, dir.Name)
                row("IsFolder") = True
                row("Size") = "文件夹"
                row("ModifiedDate") = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                dt.Rows.Add(row)
            Next

            ' 添加文件
            For Each file As FileInfo In directoryInfo.GetFiles()
                Dim row As Data.DataRow = dt.NewRow()
                row("Name") = file.Name
                row("FullPath") = Path.Combine(CurrentPath, file.Name)
                row("IsFolder") = False
                row("Size") = FormatFileSize(file.Length)
                row("ModifiedDate") = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                dt.Rows.Add(row)
            Next

            ' 排序：文件夹在前，文件在后
            Dim dv As New Data.DataView(dt)
            dv.Sort = "IsFolder DESC, Name ASC"

            gvFiles.DataSource = dv
            gvFiles.DataBind()

        Catch ex As Exception
            ShowMessage("加载文件列表失败: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub gvFiles_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim isFolder As Boolean = CBool(DataBinder.Eval(e.Row.DataItem, "IsFolder"))
            If isFolder Then
                e.Row.CssClass = "folder-row"

                ' 为文件夹添加双击进入功能
                e.Row.Attributes("ondblclick") = "showWaitOverlay(); window.location.href='?path=" &
                    HttpUtility.UrlEncode(DataBinder.Eval(e.Row.DataItem, "FullPath").ToString()) & "';"
                e.Row.Attributes("style") = "cursor: pointer;"
            End If
        End If
    End Sub

    Private Function FormatFileSize(ByVal bytes As Long) As String
        Dim sizes As String() = {"B", "KB", "MB", "GB"}
        Dim i As Integer = 0
        Dim dblBytes As Double = bytes

        While dblBytes >= 1024 And i < sizes.Length - 1
            dblBytes = dblBytes / 1024
            i += 1
        End While

        Return String.Format("{0:0.##} {1}", dblBytes, sizes(i))
    End Function

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpload.Click
        If fileUpload.HasFile Then
            Try
                Dim fileName As String = Path.GetFileName(fileUpload.FileName)
                Dim savePath As String = Path.Combine(FileServerPath, CurrentPath, fileName)

                ' 检查文件是否已存在
                If File.Exists(savePath) Then
                    ShowMessage("文件已存在，请先删除原文件", False)
                Else
                    fileUpload.SaveAs(savePath)
                    ShowMessage("文件上传成功: " & fileName, True)
                    LoadFileList()
                End If

            Catch ex As Exception
                ShowMessage("文件上传失败: " & ex.Message, False)
            End Try
        Else
            ShowMessage("请选择要上传的文件", False)
        End If
    End Sub

    Protected Sub btnCreateFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateFolder.Click
        Dim folderName As String = txtFolderName.Text.Trim()

        If String.IsNullOrEmpty(folderName) Then
            ShowMessage("请输入文件夹名称", False)
            Return
        End If

        ' 移除非法字符
        Dim invalidChars As Char() = Path.GetInvalidFileNameChars()
        For Each c As Char In invalidChars
            folderName = folderName.Replace(c.ToString(), "")
        Next

        If String.IsNullOrEmpty(folderName) Then
            ShowMessage("文件夹名称包含非法字符", False)
            Return
        End If

        Try
            Dim folderPath As String = Path.Combine(FileServerPath, CurrentPath, folderName)

            If Directory.Exists(folderPath) Then
                ShowMessage("文件夹已存在", False)
            Else
                Directory.CreateDirectory(folderPath)
                ShowMessage("文件夹创建成功: " & folderName, True)
                txtFolderName.Text = ""
                LoadFileList()
            End If

        Catch ex As Exception
            ShowMessage("创建文件夹失败: " & ex.Message, False)
        End Try
    End Sub

    Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefresh.Click
        LoadFileList()
        ShowMessage("文件列表已刷新", True)
    End Sub

    ' 处理 RowDeleting 事件，防止错误
    Protected Sub gvFiles_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        ' 什么都不做，只是防止错误
        ' 实际的删除操作在 RowCommand 中处理
        e.Cancel = True
    End Sub

    Protected Sub gvFiles_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Dim fullPath As String = e.CommandArgument.ToString()
        Dim physicalPath As String = Path.Combine(FileServerPath, fullPath)
        Dim isFolder As Boolean = Directory.Exists(physicalPath)

        Select Case e.CommandName
            Case "Download"
                Try
                    If isFolder Then
                        ' 如果是文件夹，使用Ionic.Zip压缩后下载
                        DownloadFolderAsZip(physicalPath, fullPath)
                    Else
                        ' 如果是文件，直接下载
                        If File.Exists(physicalPath) Then
                            DownloadFile(physicalPath)
                        Else
                            ShowMessage("文件不存在", False)
                        End If
                    End If
                Catch ex As Exception
                    ShowMessage("下载失败: " & ex.Message, False)
                End Try

            Case "Delete"
                Try
                    If isFolder Then
                        ' 删除文件夹及其内容
                        If Directory.Exists(physicalPath) Then
                            Directory.Delete(physicalPath, True)
                            ShowMessage("文件夹删除成功", True)
                        End If
                    Else
                        ' 删除文件
                        If File.Exists(physicalPath) Then
                            File.Delete(physicalPath)
                            ShowMessage("文件删除成功", True)
                        End If
                    End If
                    LoadFileList()

                Catch ex As Exception
                    ShowMessage("删除失败: " & ex.Message, False)
                End Try
        End Select
    End Sub

    Private Sub DownloadFile(ByVal filePath As String)
        Dim fileInfo As New FileInfo(filePath)
        Response.Clear()
        Response.ClearHeaders()
        Response.Buffer = False
        Response.ContentType = "application/octet-stream"
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & HttpUtility.UrlEncode(fileInfo.Name, System.Text.Encoding.UTF8))
        Response.AppendHeader("Content-Length", fileInfo.Length.ToString())
        Response.WriteFile(filePath)
        Response.Flush()
        Response.End()
    End Sub

    Private Sub DownloadFolderAsZip(ByVal folderPath As String, ByVal relativePath As String)
        Dim tempZipPath As String = Path.GetTempFileName() & ".zip"
        Dim folderName As String = New DirectoryInfo(folderPath).Name
        Dim zipFileName As String = folderName & ".zip"

        Try
            ' 使用Ionic.Zip创建ZIP文件
            Using zip As New ZipFile()
                ' 设置编码以支持中文文件名
                zip.AlternateEncodingUsage = ZipOption.Always
                zip.AlternateEncoding = System.Text.Encoding.UTF8

                ' 添加文件夹所有内容到ZIP
                zip.AddDirectory(folderPath, folderName)

                ' 保存ZIP文件
                zip.Save(tempZipPath)
            End Using

            ' 下载ZIP文件
            Dim fileInfo As New FileInfo(tempZipPath)
            Response.Clear()
            Response.ClearHeaders()
            Response.Buffer = False
            Response.ContentType = "application/zip"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & HttpUtility.UrlEncode(zipFileName, System.Text.Encoding.UTF8))
            Response.AppendHeader("Content-Length", fileInfo.Length.ToString())
            Response.WriteFile(tempZipPath)
            Response.Flush()
            Response.End()

        Catch ex As Exception
            ShowMessage("压缩文件夹失败: " & ex.Message, False)
        Finally
            ' 删除临时ZIP文件
            If File.Exists(tempZipPath) Then
                Try
                    File.Delete(tempZipPath)
                Catch
                    ' 忽略删除临时文件的错误
                End Try
            End If
        End Try
    End Sub

    Private Sub ShowMessage(ByVal message As String, ByVal isSuccess As Boolean)
        lblMessage.Text = message
        If isSuccess Then
            lblMessage.CssClass = "message success"
        Else
            lblMessage.CssClass = "message error"
        End If
        lblMessage.Visible = True
    End Sub
End Class