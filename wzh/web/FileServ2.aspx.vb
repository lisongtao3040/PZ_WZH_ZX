Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Configuration
Imports Ionic.Zip
Imports System.Web.Script.Serialization
Imports System.Drawing
Imports System.Drawing.Imaging
Imports SqlHelper


Partial Class FileServ2
    Inherits System.Web.UI.Page

    ' 当前显示的相对路径
    Private Property RelPath As String
        Get
            Return If(ViewState("RelPath"), "")
        End Get
        Set(value As String)
            ViewState("RelPath") = value
        End Set
    End Property

    ' 搜索关键词
    Private Property SearchKeyword As String
        Get
            Return If(ViewState("SearchKeyword"), "")
        End Get
        Set(value As String)
            ViewState("SearchKeyword") = value
        End Set
    End Property

    ' 搜索模式：File=共享文件夹, DB=数据库
    Private Property SearchMode As String
        Get
            Return If(ViewState("SearchMode"), "File")
        End Get
        Set(value As String)
            ViewState("SearchMode") = value
        End Set
    End Property

    ' 当前激活的搜索源：None=无搜索, Folder=文件夹搜索, Database=数据库搜索
    Private Property ActiveSearchSource As String
        Get
            Return If(ViewState("ActiveSearchSource"), "None")
        End Get
        Set(value As String)
            ViewState("ActiveSearchSource") = value
        End Set
    End Property

    ' 数据库搜索的总记录数
    Private Property DBTotalCount As Integer
        Get
            Return If(ViewState("DBTotalCount"), 0)
        End Get
        Set(value As Integer)
            ViewState("DBTotalCount") = value
        End Set
    End Property

    ' 文件夹搜索的总记录数
    Private Property FolderTotalCount As Integer
        Get
            Return If(ViewState("FolderTotalCount"), 0)
        End Get
        Set(value As Integer)
            ViewState("FolderTotalCount") = value
        End Set
    End Property

    ' 文件夹分页的当前页索引（0-based）
    Private Property FolderPageIndex As Integer
        Get
            Return If(ViewState("FolderPageIndex"), 0)
        End Get
        Set(value As Integer)
            ViewState("FolderPageIndex") = value
        End Set
    End Property

    ''' <summary>
    ''' 数据库来源的页码（完全独立）
    ''' </summary>
    Private Property DBPageIndex As Integer
        Get
            Return If(ViewState("DBPageIndex"), 0)
        End Get
        Set(value As Integer)
            ViewState("DBPageIndex") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            ' 处理缩略图请求（共享文件夹）
            Dim thumbnailReq As String = Request.QueryString("thumbnail")
            If Not String.IsNullOrEmpty(thumbnailReq) Then
                ServeThumbnail(thumbnailReq)
                Return
            End If

            ' 处理数据库图片缩略图请求
            Dim dbThumbReq As String = Request.QueryString("dbthumb")
            If Not String.IsNullOrEmpty(dbThumbReq) Then
                ServeDatabasePicture(dbThumbReq)
                Return
            End If

            ' 处理原图请求（共享文件夹）
            Dim imgReq As String = Request.QueryString("img")
            If Not String.IsNullOrEmpty(imgReq) Then
                ServeOriginalImage(imgReq)
                Return
            End If

            ' 处理数据库原图请求
            Dim dbImgReq As String = Request.QueryString("dbimg")
            If Not String.IsNullOrEmpty(dbImgReq) Then
                ServeDatabaseOriginalImage(dbImgReq)
                Return
            End If

            If Not IsPostBack Then
                SearchKeyword = ""
                SearchMode = "File"
                FolderPageIndex = 0
                BindGrid()
            End If

            ' 处理双击或 JS 触发的事件
            Dim target As String = Request("__EVENTTARGET")
            Dim arg As String = Request("__EVENTARGUMENT")
            If target = "btnEnterFolder" Then
                EnterDirectory(arg)
            End If

            ' 处理文件夹分页事件（完全独立）
            If target = "FolderPaging" Then
                Dim newPage As Integer = 0
                If Integer.TryParse(arg, newPage) Then
                    FolderPageIndex = newPage
                End If
                If String.IsNullOrEmpty(SearchKeyword) Then
                    BindGridFromFileSystem()
                Else
                    BindGridFromFileSystemSearch()
                End If
            End If

            ' ✅ 处理数据库分页事件（完全独立）
            If target = "DBPaging" Then
                Dim newPage As Integer = 0
                If Integer.TryParse(arg, newPage) Then
                    DBPageIndex = newPage
                End If
                ' 重新绑定数据库数据
                Dim dbKeyword As String = SearchKeyword
                If Not String.IsNullOrEmpty(dbKeyword) Then
                    Try
                        Dim dbList As New List(Of Object)()
                        Dim dbTotalCount As Integer = 0

                        ' 使用分页查询，只获取当前页需要的数据（不包含图片数据）
                        Dim dbPagedData As Data.DataTable = GetDatabasePicturesPaged(dbKeyword, DBPageIndex, gvDBFiles.PageSize, dbTotalCount)

                        ' ✅ 保存总记录数到 ViewState（使用大写 DBTotalCount）
                        dbTotalCount = dbTotalCount

                        If dbPagedData IsNot Nothing AndAlso dbPagedData.Rows.Count > 0 Then
                            For Each row As Data.DataRow In dbPagedData.Rows
                                ' 安全检查：确保字段存在且不为空
                                If row.Table.Columns.Contains("pic_name") AndAlso row("pic_name") IsNot DBNull.Value Then
                                    Dim picName As String = row("pic_name").ToString()
                                    Dim picUpdTime As String = ""

                                    ' 安全获取更新时间
                                    If row.Table.Columns.Contains("pic_upd_time") AndAlso row("pic_upd_time") IsNot DBNull.Value Then
                                        picUpdTime = row("pic_upd_time").ToString()
                                    End If

                                    Dim thumbnailUrl As String = "FileServ2.aspx?dbthumb=" & HttpUtility.UrlEncode(picName & "|" & picUpdTime)

                                    dbList.Add(New With {
                                        .Name = picName & " (" & picUpdTime & ")",
                                        .FullName = picName & "|" & picUpdTime,
                                        .IsDirectory = False,
                                        .LastWriteTime = CDate(ParseDatabaseDateTime(picUpdTime)),
                                        .ThumbnailUrl = CStr(thumbnailUrl),
                                        .IsImage = True,
                                        .SourceType = CStr("DB"),
                                        .PicData = Nothing
                                    })
                                End If
                            Next
                        End If

                        lblDBCount.Text = "共 " & dbTotalCount & " 个项目，当前第 " & (DBPageIndex + 1) & "/" & Math.Ceiling(dbTotalCount / gvDBFiles.PageSize) & " 页"

                        gvDBFiles.DataSource = dbList
                        gvDBFiles.DataBind()

                        ' ✅ 更新数据库分页控件
                        UpdateDBPager()
                    Catch ex As Exception
                        ShowError("数据库搜索失败: " & ex.Message)
                    End Try
                End If
            End If

            ' 确保分页控件始终更新
            If Not IsPostBack Then
                UpdateFolderPager()
                UpdateDBPager()
            End If
        Catch ex As Exception
            ShowError(ex.Message)
        End Try
    End Sub

    Private Sub BindGrid()
        ' 只从文件系统搜索（数据库功能已禁用）
        BindGridFromFileSystem()
    End Sub

    ''' <summary>
    ''' 从文件系统绑定网格（完全独立的手动分页）
    ''' </summary>
    Private Sub BindGridFromFileSystem()
        Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
        Dim fullPath As String = Path.Combine(root, RelPath)

        If Not Directory.Exists(fullPath) Then
            ShowError("路径不存在或已移动")
            Return
        End If

        litPath.Text = If(RelPath = "", "\ (根目录)", RelPath)
        lblFolderCount.Text = ""
        lblDBCount.Text = ""

        Dim di As New DirectoryInfo(fullPath)
        Dim allItems As New List(Of Object)

        ' 获取文件夹
        For Each d In di.GetDirectories()
            allItems.Add(New With {
                .Name = d.Name,
                .FullName = d.FullName,
                .IsDirectory = True,
                .LastWriteTime = CDate(d.LastWriteTime),
                .ThumbnailUrl = CStr(""),
                .IsImage = False,
                .SourceType = CStr("File")
            })
        Next
        ' 获取文件
        For Each f In di.GetFiles()
            Dim isImage As Boolean = IsImageFile(f.Name)
            Dim thumbnailUrl As String = ""

            ' 如果是图片文件，生成缩略图URL
            If isImage Then
                thumbnailUrl = "FileServ2.aspx?thumbnail=" & HttpUtility.UrlEncode(Path.Combine(RelPath, f.Name))
            End If

            allItems.Add(New With {
                .Name = f.Name,
                .FullName = f.FullName,
                .IsDirectory = False,
                .LastWriteTime = CDate(f.LastWriteTime),
                .ThumbnailUrl = CStr(thumbnailUrl),
                .IsImage = isImage,
                .SourceType = CStr("File")
            })
        Next

        FolderTotalCount = allItems.Count

        ' 手動で現在のページのデータを切り出す
        Dim pageSize As Integer = gvFiles.PageSize
        Dim startIndex As Integer = FolderPageIndex * pageSize
        Dim pageData As New List(Of Object)
        For i As Integer = startIndex To Math.Min(startIndex + pageSize - 1, allItems.Count - 1)
            pageData.Add(allItems(i))
        Next

        lblFolderCount.Text = "共 " & FolderTotalCount & " 个项目，当前第 " & (FolderPageIndex + 1) & "/" & Math.Ceiling(FolderTotalCount / pageSize) & " 页"

        gvFiles.DataSource = pageData
        gvFiles.DataBind()

        ' 更新分页控件
        UpdateFolderPager()
    End Sub

    ''' <summary>
    ''' 从数据库绑定网格（已禁用）
    ''' </summary>
    ' Private Sub BindGridFromDatabase()
    '     Try
    '         Dim cm As New ComDDL
    '         Dim dtImg As Data.DataTable = cm.SelChkPictureByName(SearchKeyword)
    '         
    '         litPath.Text = "数据库搜索结果"
    '         lblSearchMode.Text = "当前模式：数据库（只读）"
    '
    '         Dim list As New List(Of Object)
    '
    '         If dtImg IsNot Nothing AndAlso dtImg.Rows.Count > 0 Then
    '             System.Diagnostics.Debug.WriteLine("数据库返回 " & dtImg.Rows.Count & " 行数据")
    '             For Each row As Data.DataRow In dtImg.Rows
    '                 ' 安全检查：确保字段存在且不为空
    '                 If row.Table.Columns.Contains("pic_name") AndAlso row("pic_name") IsNot DBNull.Value Then
    '                     Dim picName As String = row("pic_name").ToString()
    '                     Dim picUpdTime As String = ""
    '                     Dim picConn As Byte() = Nothing
    '                     
    '                     ' 安全获取更新时间
    '                     If row.Table.Columns.Contains("pic_upd_time") AndAlso row("pic_upd_time") IsNot DBNull.Value Then
    '                         picUpdTime = row("pic_upd_time").ToString()
    '                     End If
    '                     
    '                     ' 安全获取图片数据
    '                     If row.Table.Columns.Contains("pic_conn") AndAlso row("pic_conn") IsNot DBNull.Value Then
    '                         picConn = CType(row("pic_conn"), Byte())
    '                     End If
    '                     
    '                     ' 如果图片数据为空，跳过这条记录
    '                     If picConn Is Nothing OrElse picConn.Length = 0 Then
    '                         System.Diagnostics.Debug.WriteLine("跳过空图片: " & picName)
    '                         Continue For
    '                     End If
    '                     
    '                     ' 生成缩略图 URL（通过查询字符串传递标识）
    '                     Dim thumbnailUrl As String = "FileServ2.aspx?dbthumb=" & HttpUtility.UrlEncode(picName & "|" & picUpdTime)
    '                     
    '                     System.Diagnostics.Debug.WriteLine("添加数据库图片: " & picName & ", SourceType=DB")
    '                     
    '                     ' 用管道符分隔，用于后续识别
    '                     list.Add(New With {
    '                         .Name = picName & " (" & picUpdTime & ")",
    '                         .FullName = picName & "|" & picUpdTime,
    '                         .IsDirectory = False,
    '                         .SizeDisplay = FormatFileSize(picConn.Length),
    '                         .LastWriteTime = ParseDatabaseDateTime(picUpdTime),
    '                         .ThumbnailUrl = thumbnailUrl,
    '                         .IsImage = True,
    '                         .SourceType = "DB",
    '                         .PicData = picConn
    '                     })
    '                 End If
    '             Next
    '         Else
    '             System.Diagnostics.Debug.WriteLine("数据库返回空结果")
    '         End If
    '
    '         gvFiles.DataSource = list
    '         gvFiles.DataBind()
    '
    '     Catch ex As Exception
    '         ShowError("数据库搜索失败: " & ex.Message)
    '     End Try
    ' End Sub

    ''' <summary>
    ''' 合并搜索：同时从文件夹和数据库检索，分开显示
    ''' </summary>
    Private Sub BindGridMerged()
        Try
            Dim folderList As New List(Of Object)()
            Dim dbList As New List(Of Object)()
            Dim folderCount As Integer = 0
            Dim dbCount As Integer = 0

            ' 1. 从文件系统搜索
            Try
                Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
                Dim fullPath As String = Path.Combine(root, RelPath)

                If Directory.Exists(fullPath) Then
                    Dim di As New DirectoryInfo(fullPath)

                    ' 获取文件夹
                    For Each d In di.GetDirectories()
                        If String.IsNullOrEmpty(SearchKeyword) OrElse d.Name.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                            folderList.Add(New With {
                                .Name = d.Name,
                                .FullName = d.FullName,
                                .IsDirectory = True,
                                .LastWriteTime = CDate(d.LastWriteTime),
                                .ThumbnailUrl = CStr(""),
                                .IsImage = False,
                                .SourceType = CStr("File")
                            })
                            folderCount += 1
                        End If
                    Next

                    ' 获取文件
                    For Each f In di.GetFiles()
                        If String.IsNullOrEmpty(SearchKeyword) OrElse f.Name.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                            Dim isImage As Boolean = IsImageFile(f.Name)
                            Dim thumbnailUrl As String = ""

                            If isImage Then
                                thumbnailUrl = "FileServ2.aspx?thumbnail=" & HttpUtility.UrlEncode(Path.Combine(RelPath, f.Name))
                            End If

                            folderList.Add(New With {
                                .Name = f.Name,
                                .FullName = f.FullName,
                                .IsDirectory = False,
                                .LastWriteTime = CDate(f.LastWriteTime),
                                .ThumbnailUrl = CStr(thumbnailUrl),
                                .IsImage = isImage,
                                .SourceType = CStr("File")
                            })
                            folderCount += 1
                        End If
                    Next
                End If
            Catch ex As Exception
                ShowError("文件夹搜索失败: " & ex.Message)
            End Try

            ' 2. 从数据库搜索（使用分页）
            Try
                Dim dbTotalCount As Integer = 0
                Dim dbPagedData As Data.DataTable = GetDatabasePicturesPaged(SearchKeyword, gvDBFiles.PageIndex, gvDBFiles.PageSize, dbTotalCount)

                If dbPagedData IsNot Nothing AndAlso dbPagedData.Rows.Count > 0 Then
                    For Each row As Data.DataRow In dbPagedData.Rows
                        ' 安全检查：确保字段存在且不为空
                        If row.Table.Columns.Contains("pic_name") AndAlso row("pic_name") IsNot DBNull.Value Then
                            Dim picName As String = row("pic_name").ToString()
                            Dim picUpdTime As String = ""

                            ' 安全获取更新时间
                            If row.Table.Columns.Contains("pic_upd_time") AndAlso row("pic_upd_time") IsNot DBNull.Value Then
                                picUpdTime = row("pic_upd_time").ToString()
                            End If

                            Dim thumbnailUrl As String = "FileServ2.aspx?dbthumb=" & HttpUtility.UrlEncode(picName & "|" & picUpdTime)

                            dbList.Add(New With {
                                .Name = picName & " (" & picUpdTime & ")",
                                .FullName = picName & "|" & picUpdTime,
                                .IsDirectory = False,
                                .LastWriteTime = CDate(ParseDatabaseDateTime(picUpdTime)),
                                .ThumbnailUrl = CStr(thumbnailUrl),
                                .IsImage = True,
                                .SourceType = CStr("DB"),
                                .PicData = Nothing
                            })
                            dbCount += 1
                        End If
                    Next
                End If

                ' 更新数据库 GridView 的虚拟ItemCount（如果需要）
                ' 注意：ASP.NET GridView 不支持虚拟分页，这里只是显示总数
            Catch ex As Exception
                ShowError("数据库搜索失败: " & ex.Message)
            End Try

            ' 显示结果
            litPath.Text = "搜索结果"
            lblFolderCount.Text = "文件夹: " & folderCount & " 个项目"
            lblDBCount.Text = "数据库: " & dbCount & " 个项目"

            ' 分别绑定到两个 GridView
            gvFiles.DataSource = folderList
            gvFiles.DataBind()

            gvDBFiles.DataSource = dbList
            gvDBFiles.DataBind()

            ' 更新分页控件
            UpdateFolderPager()
            UpdateDBPager()

        Catch ex As Exception
            ShowError("搜索失败: " & ex.Message)
        End Try
    End Sub

    ' 处理名称点击（进入目录）
    Protected Sub lnkName_Click(sender As Object, e As EventArgs)
        Dim name As String = CType(sender, IButtonControl).CommandArgument
        EnterDirectory(name)
    End Sub

    Private Sub EnterDirectory(name As String)
        Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
        Dim testPath = Path.Combine(root, RelPath, name)
        If Directory.Exists(testPath) Then
            RelPath = Path.Combine(RelPath, name)
            BindGrid()
        End If
    End Sub

    ' 返回上级
    Protected Sub btnBack_Click(sender As Object, e As EventArgs)
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        If Not String.IsNullOrEmpty(RelPath) Then
            Dim parent = Path.GetDirectoryName(RelPath)
            RelPath = If(parent Is Nothing, "", parent)
            BindGrid()
        End If
    End Sub

    ' 核心上传逻辑（支持 WebkitDirectory 保持结构）
    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        Try
            ' 读取新的配置路径：FileServerPath2
            Dim targetBase As String = ConfigurationManager.AppSettings("FileServerPath2")

            ' 确保物理路径存在
            If Not Directory.Exists(targetBase) Then
                Directory.CreateDirectory(targetBase)
            End If

            Dim files = Request.Files
            Dim count As Integer = 0

            For i As Integer = 0 To files.Count - 1
                Dim file As HttpPostedFile = files(i)
                If file.ContentLength > 0 Then
                    ' 【关键修改】：只取文件名，忽略 webkitdirectory 带来的相对路径
                    Dim fileNameOnly As String = Path.GetFileName(file.FileName)
                    Dim fullSavePath As String = Path.Combine(targetBase, fileNameOnly)

                    ' 冲突自动重命名（调用之前定义的 GetUniquePath）
                    'Dim finalPath = GetUniquePath(fullSavePath)
                    If System.IO.File.Exists(fullSavePath) Then
                        System.IO.File.Delete(fullSavePath)
                    End If

                    file.SaveAs(fullSavePath)
                    count += 1
                End If
            Next

            ShowSuccess("成功上传 " & count & " 个文件到目标目录。")
            BindGrid()
        Catch ex As Exception
            ShowError("上传失败: " & ex.Message)
        End Try
    End Sub


    ' 统一处理 GridView 中的删除和下载动作
    Protected Sub gvFiles_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvFiles.RowCommand
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        ' 获取点击行传入的文件/文件夹名
        Dim itemName As String = e.CommandArgument.ToString()
        Dim sourceType = ""

        ' 尝试获取 SourceType
        Try
            ' 找到对应的行
            For Each row As GridViewRow In gvFiles.Rows
                Dim lnkName As LinkButton = CType(row.FindControl("lnkName"), LinkButton)
                If lnkName IsNot Nothing AndAlso lnkName.CommandArgument = itemName Then
                    Dim stValue = gvFiles.DataKeys(row.RowIndex).Values("SourceType")
                    If stValue IsNot Nothing Then
                        sourceType = stValue.ToString()
                    Else
                        sourceType = "File"
                    End If
                    Exit For
                End If
            Next
        Catch ex As Exception
            sourceType = "File"
        End Try

        Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
        Dim fullPath As String = Path.Combine(root, RelPath, itemName)

        ' --- 处理下载逻辑 ---
        If e.CommandName = "DownloadItem" Then
            Try
                ' 如果是数据库中的图片，从数据库下载（已禁用）
                ' If sourceType = "DB" Then
                '     DownloadDatabasePicture(itemName)
                '     Return
                ' End If

                Response.Clear()
                ' 设置响应头以处理中文文件名
                Dim encodedName As String = HttpUtility.UrlEncode(itemName)

                If Directory.Exists(fullPath) Then
                    ' 如果是文件夹：调用 Ionic.Zip 压缩后下载
                    Response.ContentType = "application/zip"
                    Response.AddHeader("Content-Disposition", "attachment; filename=" & encodedName & ".zip")

                    Using zip As New ZipFile()
                        ' 关键：设置编码以支持中文文件名
                        zip.AlternateEncodingUsage = ZipOption.AsNecessary
                        zip.AlternateEncoding = System.Text.Encoding.UTF8

                        zip.AddDirectory(fullPath)
                        zip.Save(Response.OutputStream)
                    End Using
                Else
                    ' 如果是文件：直接传送
                    Response.ContentType = "application/octet-stream"
                    Response.AddHeader("Content-Disposition", "attachment; filename=" & encodedName)
                    Response.TransmitFile(fullPath)
                End If

                ' 强制结束响应，防止将 ASPX 的 HTML 内容附加到文件末尾
                Response.Flush()
                Response.SuppressContent = True
                HttpContext.Current.ApplicationInstance.CompleteRequest()

            Catch ex As Exception
                ShowError("下载失败: " & ex.Message)
            End Try

            ' --- 处理删除逻辑 ---
        ElseIf e.CommandName = "DelItem" Then
            ' 数据库中的图片不能删除
            If sourceType = "DB" Then
                ShowError("数据库中的图片不能删除")
                Return
            End If

            Try
                If Directory.Exists(fullPath) Then
                    Directory.Delete(fullPath, True)
                Else
                    File.Delete(fullPath)
                End If
                ShowSuccess("项目已成功删除")
                BindGrid()
            Catch ex As Exception
                ShowError("删除失败，可能该项目正被占用或权限不足。")
            End Try
        End If
    End Sub

    ' 下载与压缩逻辑
    Protected Sub btnDl_Click(sender As Object, e As EventArgs)
        Dim name As String = CType(sender, Button).CommandArgument
        Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
        Dim fullPath As String = Path.Combine(root, RelPath, name)

        Try
            Response.Clear()
            If Directory.Exists(fullPath) Then
                ' 文件夹打包下载
                Dim zipName As String = HttpUtility.UrlEncode(name) & ".zip"
                Response.ContentType = "application/zip"
                Response.AddHeader("content-disposition", "attachment; filename=" & zipName)
                Using zip As New ZipFile()
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary ' 支持中文
                    zip.AddDirectory(fullPath)
                    zip.Save(Response.OutputStream)
                End Using
            Else
                ' 单文件下载
                Response.ContentType = "application/octet-stream"
                Response.AddHeader("Content-Disposition", "attachment; filename=" & HttpUtility.UrlEncode(name))
                Response.TransmitFile(fullPath)
            End If
            Response.End()
        Catch ex As Exception
            ShowError("下载出错")
        End Try
    End Sub

    ' 批量删除逻辑
    Protected Sub btnBatchDel_Click(sender As Object, e As EventArgs) Handles btnBatchDel.Click
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        Dim deletedCount As Integer = 0
        Dim skippedDbCount As Integer = 0
        Try
            For Each row As GridViewRow In gvFiles.Rows
                Dim chk = CType(row.FindControl("chkSelect"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then
                    ' 安全地获取 SourceType
                    Dim sourceType As String = "File"
                    Try
                        Dim stValue = gvFiles.DataKeys(row.RowIndex).Values("SourceType")
                        If stValue IsNot Nothing Then
                            sourceType = stValue.ToString()
                        End If
                    Catch ex As Exception
                        sourceType = "File"
                    End Try

                    ' 跳过数据库中的图片
                    If sourceType = "DB" Then
                        skippedDbCount += 1
                        Continue For
                    End If

                    ' 安全地获取其他字段
                    Dim pathStr As String = ""
                    Dim isDir As Boolean = False

                    Try
                        pathStr = gvFiles.DataKeys(row.RowIndex).Values("FullName").ToString()
                        isDir = CBool(gvFiles.DataKeys(row.RowIndex).Values("IsDirectory"))
                    Catch ex As Exception
                        ' 如果获取失败，跳过这一行
                        Continue For
                    End Try

                    If isDir Then Directory.Delete(pathStr, True) Else File.Delete(pathStr)
                    deletedCount += 1
                End If
            Next

            Dim msg As String = "已删除 " & deletedCount & " 个项目"
            If skippedDbCount > 0 Then
                msg &= "（跳过 " & skippedDbCount & " 个数据库图片）"
            End If
            ShowSuccess(msg)
            BindGrid()
        Catch ex As Exception
            ShowError("删除时部分失败: " & ex.Message)
        End Try
    End Sub

    ' 辅助：获取唯一文件名
    Private Function GetUniquePath(pathStr As String) As String
        Dim dir = Path.GetDirectoryName(pathStr)
        Dim name = Path.GetFileNameWithoutExtension(pathStr)
        Dim ext = Path.GetExtension(pathStr)
        Dim count As Integer = 1
        Dim finalPath As String = pathStr
        While File.Exists(finalPath) Or Directory.Exists(finalPath)
            finalPath = Path.Combine(dir, name & "_" & count & ext)
            count += 1
        End While
        Return finalPath
    End Function

    ' 辅助：格式化大小
    Private Function FormatFileSize(bytes As Long) As String
        Dim Suffix() As String = {"B", "KB", "MB", "GB"}
        Dim i As Integer = 0
        Dim dblSzie As Double = bytes
        Do While dblSzie >= 1024 And i < Suffix.Length - 1
            i += 1
            dblSzie = dblSzie / 1024
        Loop
        Return String.Format("{0:0.##} {1}", dblSzie, Suffix(i))
    End Function

    Private Sub ShowError(msg As String)
        pnlMsg.Visible = True
        pnlMsg.Text = "❌ " & msg
        pnlMsg.BackColor = Drawing.Color.MistyRose
        ' 标记当前显示的是错误信息
        ViewState("LastMsgType") = "Error"
    End Sub

    Private Sub ShowSuccess(msg As String)
        ' 如果当前显示的是错误信息，不覆盖
        If ViewState("LastMsgType") IsNot Nothing AndAlso ViewState("LastMsgType").ToString() = "Error" Then
            Return
        End If

        pnlMsg.Visible = True
        pnlMsg.Text = "✅ " & msg
        pnlMsg.BackColor = Drawing.Color.LightCyan
        ViewState("LastMsgType") = "Success"
    End Sub

    Protected Sub gvFiles_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' 安全地获取 DataKeys 值，防止空引用
            Dim isDir As Boolean = False
            Dim name As String = ""
            Dim sourceType As String = "File" ' 默认为 File

            Try
                isDir = CBool(gvFiles.DataKeys(e.Row.RowIndex).Values("IsDirectory"))
                name = gvFiles.DataKeys(e.Row.RowIndex).Values("Name").ToString()

                Dim stValue = gvFiles.DataKeys(e.Row.RowIndex).Values("SourceType")
                If stValue IsNot Nothing Then
                    sourceType = stValue.ToString()
                End If
            Catch ex As Exception
                ' 如果获取失败，使用默认值
                sourceType = "File"
            End Try

            ' 获取数据源中的额外字段
            Dim dataItem = CType(e.Row.DataItem, Object)
            Dim thumbnailUrl = ""
            Dim isImage = False

            ' 使用反射获取动态对象的属性
            If dataItem IsNot Nothing Then
                Dim propThumbnail = dataItem.GetType().GetProperty("ThumbnailUrl")
                Dim propIsImage = dataItem.GetType().GetProperty("IsImage")
                Dim propSourceType = dataItem.GetType().GetProperty("SourceType")

                If propThumbnail IsNot Nothing Then
                    Dim thumbValue = propThumbnail.GetValue(dataItem, Nothing)
                    If thumbValue IsNot Nothing Then
                        thumbnailUrl = thumbValue.ToString()
                    Else
                        thumbnailUrl = ""
                    End If
                End If
                If propIsImage IsNot Nothing Then
                    Dim imgValue = propIsImage.GetValue(dataItem, Nothing)
                    If imgValue IsNot Nothing Then
                        isImage = CBool(imgValue)
                    End If
                End If
            End If

            ' 显示数据库标签
            Dim lblDbTag As Label = CType(e.Row.FindControl("lblDbTag"), Label)
            If lblDbTag IsNot Nothing Then
                If sourceType = "DB" Then
                    lblDbTag.Visible = True
                Else
                    lblDbTag.Visible = False
                End If
            End If

            ' 设置来源列
            Dim lblSource As Label = CType(e.Row.FindControl("lblSource"), Label)
            If lblSource IsNot Nothing Then
                If sourceType = "DB" Then
                    lblSource.Text = "DB"
                    lblSource.ForeColor = Drawing.Color.OrangeRed
                Else
                    lblSource.Text = "文件夹"
                    lblSource.ForeColor = Drawing.Color.Green
                End If
            End If

            ' 控制删除按钮的可见性：数据库中的图片不能删除
            Dim btnDel As Button = CType(e.Row.FindControl("btnDel"), Button)
            If btnDel IsNot Nothing Then
                System.Diagnostics.Debug.WriteLine("Row " & e.Row.RowIndex & " - SourceType: " & sourceType & ", IsDir: " & isDir.ToString())
                If sourceType = "DB" Then
                    btnDel.Visible = False
                    System.Diagnostics.Debug.WriteLine("  -> Hiding delete button (DB source)")
                Else
                    btnDel.Visible = True
                    System.Diagnostics.Debug.WriteLine("  -> Showing delete button (File source)")
                End If
            Else
                System.Diagnostics.Debug.WriteLine("Row " & e.Row.RowIndex & " - btnDel not found!")
            End If

            ' 设置行样式：数据库来源的行显示灰色，文件夹显示蓝色背景
            If sourceType = "DB" Then
                e.Row.BackColor = Drawing.Color.LightGray
                e.Row.ForeColor = Drawing.Color.DarkGray
            Else
                ' 文件夹来源：如果是目录则显示蓝色背景
                If isDir Then
                    e.Row.CssClass = "folder-row"
                End If
            End If

            If isDir Then
                ' 绑定双击事件
                e.Row.Attributes.Add("ondblclick", "onFolderDblClick('" & name.Replace("'", "\'") & "')")

                ' 隐藏缩略图列，显示文件夹图标
                Dim litIcon As Literal = CType(e.Row.FindControl("litIcon"), Literal)
                If litIcon IsNot Nothing Then
                    litIcon.Visible = True
                    litIcon.Text = "<span style='font-size: 40px;'>📁</span>"
                End If
            Else
                ' 文件行
                Dim imgThumbnail As System.Web.UI.WebControls.Image = CType(e.Row.FindControl("imgThumbnail"), System.Web.UI.WebControls.Image)
                Dim litIcon As Literal = CType(e.Row.FindControl("litIcon"), Literal)

                If isImage AndAlso Not String.IsNullOrEmpty(thumbnailUrl) Then
                    ' 显示图片缩略图
                    If imgThumbnail IsNot Nothing Then
                        imgThumbnail.Visible = True
                        imgThumbnail.ImageUrl = thumbnailUrl
                        imgThumbnail.AlternateText = name

                        ' 生成原图URL（用于点击预览）
                        Dim originalImageUrl As String = ""

                        If sourceType = "DB" Then
                            ' 数据库来源：使用 dbimg 参数获取原图
                            Dim fullName As String = ""
                            If dataItem IsNot Nothing Then
                                Dim propFullName = dataItem.GetType().GetProperty("FullName")
                                If propFullName IsNot Nothing Then
                                    Dim fullNameValue = propFullName.GetValue(dataItem, Nothing)
                                    If fullNameValue IsNot Nothing Then
                                        fullName = fullNameValue.ToString()
                                    End If
                                End If
                            End If
                            originalImageUrl = "FileServ2.aspx?dbimg=" & HttpUtility.UrlEncode(fullName)
                        Else
                            ' 文件夹来源：使用 img 参数获取原图
                            Dim relativePath As String = Path.Combine(RelPath, name)
                            originalImageUrl = "FileServ2.aspx?img=" & HttpUtility.UrlEncode(relativePath)
                        End If

                        ' 添加点击预览事件，使用原图URL
                        imgThumbnail.Attributes.Add("onclick", "previewImage('" & originalImageUrl & "', '" & name.Replace("'", "\\'") & "'); event.stopPropagation();")
                    End If
                Else
                    ' 非图片文件，显示文件图标
                    If litIcon IsNot Nothing Then
                        litIcon.Visible = True
                        litIcon.Text = "<span style='font-size: 40px;'>📄</span>"
                    End If
                End If
            End If

            ' 为复选框添加客户端事件
            Dim chkSelect As CheckBox = CType(e.Row.FindControl("chkSelect"), CheckBox)
            If chkSelect IsNot Nothing Then
                chkSelect.Attributes.Add("onclick", "onCheckboxChange()")
            End If
        End If
    End Sub

    Protected Sub btnDel_Click(sender As Object, e As EventArgs)
        ' 单个删除逻辑同批量，调用 CommandArgument 即可
    End Sub

    ''' <summary>
    ''' 搜索按钮点击事件（只搜索文件夹来源）
    ''' </summary>
    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        SearchKeyword = txtSearch.Text.Trim()
        RelPath = "" ' 重置路径到根目录
        ActiveSearchSource = "Folder" ' 设置激活的搜索源为文件夹

        ' ✅ 搜索时重置 PageIndex 为 0
        FolderPageIndex = 0

        ' 只搜索文件夹，不影响数据库 GridView
        BindGridFromFileSystemSearch()

        If String.IsNullOrEmpty(SearchKeyword) Then
            ShowSuccess("已清除搜索条件")
        Else
            ShowSuccess("文件夹搜索结果：找到包含 '" & SearchKeyword & "' 的项目")
        End If
    End Sub

    ''' <summary>
    ''' 清除搜索按钮点击事件
    ''' </summary>
    Protected Sub btnClearSearch_Click(sender As Object, e As EventArgs)
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        txtSearch.Text = ""
        SearchKeyword = ""
        SearchMode = "File"
        ActiveSearchSource = "None" ' 重置激活的搜索源
        FolderTotalCount = 0 ' 重置文件夹总记录数
        RelPath = ""
        BindGridFromFileSystem() ' 返回文件夹浏览模式
        ShowSuccess("已清除搜索条件，返回共享文件夹根目录")
    End Sub

    ''' <summary>
    ''' 数据库搜索按钮点击事件（只搜索数据库来源）
    ''' </summary>
    Protected Sub btnDBSearch_Click(sender As Object, e As EventArgs)
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        Dim dbKeyword As String = txtDBSearch.Text.Trim()

        If String.IsNullOrEmpty(dbKeyword) Then
            ShowError("请输入搜索关键词")
            Return
        End If

        ' 只搜索数据库，清空文件夹 GridView
        Try
            Dim dbList As New List(Of Object)()
            Dim dbTotalCount As Integer = 0

            ActiveSearchSource = "Database" ' 设置激活的搜索源为数据库
            SearchKeyword = dbKeyword ' 保存搜索关键词用于分页

            ' ✅ 搜索时重置 DBPageIndex 为 0
            DBPageIndex = 0

            ' 使用分页查询，只获取第一页的数据（不包含图片数据）
            Dim dbPagedData As Data.DataTable = GetDatabasePicturesPaged(dbKeyword, DBPageIndex, gvDBFiles.PageSize, dbTotalCount)

            ' 保存总记录数到 ViewState
            Me.DBTotalCount = dbTotalCount

            If dbPagedData IsNot Nothing AndAlso dbPagedData.Rows.Count > 0 Then
                For Each row As Data.DataRow In dbPagedData.Rows
                    ' 安全检查：确保字段存在且不为空
                    If row.Table.Columns.Contains("pic_name") AndAlso row("pic_name") IsNot DBNull.Value Then
                        Dim picName As String = row("pic_name").ToString()
                        Dim picUpdTime As String = ""

                        ' 安全获取更新时间
                        If row.Table.Columns.Contains("pic_upd_time") AndAlso row("pic_upd_time") IsNot DBNull.Value Then
                            picUpdTime = row("pic_upd_time").ToString()
                        End If

                        Dim thumbnailUrl As String = "FileServ2.aspx?dbthumb=" & HttpUtility.UrlEncode(picName & "|" & picUpdTime)

                        dbList.Add(New With {
                            .Name = picName & " (" & picUpdTime & ")",
                            .FullName = picName & "|" & picUpdTime,
                            .IsDirectory = False,
                            .LastWriteTime = CDate(ParseDatabaseDateTime(picUpdTime)),
                            .ThumbnailUrl = CStr(thumbnailUrl),
                            .IsImage = True,
                            .SourceType = CStr("DB"),
                            .PicData = Nothing
                        })
                    End If
                Next
            End If

            litPath.Text = "数据库搜索结果"
            lblFolderCount.Text = ""
            lblDBCount.Text = "共 " & dbTotalCount & " 个项目，当前第 1/" & Math.Ceiling(dbTotalCount / gvDBFiles.PageSize) & " 页"

            ' 只绑定数据库 GridView，不影响文件夹 GridView
            gvDBFiles.DataSource = dbList
            gvDBFiles.DataBind()

            UpdateDBPager()

            ShowSuccess("数据库搜索完成：找到 " & dbTotalCount & " 个项目")

        Catch ex As Exception
            ShowError("数据库搜索失败: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 清除数据库搜索按钮点击事件
    ''' </summary>
    Protected Sub btnClearDBSearch_Click(sender As Object, e As EventArgs)
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        txtDBSearch.Text = ""
        SearchKeyword = ""
        ActiveSearchSource = "None" ' 重置激活的搜索源
        DBTotalCount = 0 ' 重置数据库总记录数

        ' 清空数据库 GridView
        gvDBFiles.DataSource = New List(Of Object)()
        gvDBFiles.DataBind()
        UpdateDBPager()

        ' 恢复文件夹浏览
        RelPath = ""
        BindGridFromFileSystem()

        litPath.Text = "\ (根目录)"
        lblFolderCount.Text = ""
        lblDBCount.Text = ""

        ShowSuccess("已清除数据库搜索条件")
    End Sub

    ''' <summary>
    ''' 数据库批量下载选中文件
    ''' </summary>
    Protected Sub btnDBBatchDownload_Click(sender As Object, e As EventArgs)
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        Try
            System.Diagnostics.Debug.WriteLine("=== DB Batch Download Started ===")

            Dim selectedPics As New List(Of Object)()
            Dim picCount As Integer = 0

            ' 遍历数据库 GridView，获取选中的项目
            For Each row As GridViewRow In gvDBFiles.Rows
                Dim chk = CType(row.FindControl("chkSelect"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then
                    ' 获取图片信息
                    Dim fullName As String = ""

                    Try
                        fullName = gvDBFiles.DataKeys(row.RowIndex).Values("FullName").ToString()
                        System.Diagnostics.Debug.WriteLine("Selected: " & fullName)
                    Catch ex As Exception
                        Continue For
                    End Try

                    If Not String.IsNullOrEmpty(fullName) Then
                        ' FullName 格式: "picName|picUpdTime"
                        Dim parts As String() = fullName.Split("|"c)
                        If parts.Length >= 2 Then
                            Dim picName As String = parts(0)
                            Dim picUpdTime As String = parts(1)

                            ' ✅ 从数据库查询图片数据
                            Dim picData As Byte() = GetPictureDataFromDB(picName, picUpdTime)

                            If picData IsNot Nothing AndAlso picData.Length > 0 Then
                                ' ✅ 生成唯一文件名：如果文件名重复，添加更新时间后缀
                                Dim uniqueFileName As String = picName
                                
                                ' 检查是否已有相同文件名
                                Dim existingNames = selectedPics.Select(Function(p) p.Name).ToList()
                                If existingNames.Contains(uniqueFileName) Then
                                    ' 添加更新时间作为后缀，避免重复
                                    Dim ext As String = Path.GetExtension(picName)
                                    Dim nameWithoutExt As String = Path.GetFileNameWithoutExtension(picName)
                                    uniqueFileName = nameWithoutExt & "_" & picUpdTime & ext
                                End If
                                
                                selectedPics.Add(New With {
                                    .Name = uniqueFileName,
                                    .Data = picData
                                })
                                picCount += 1
                            End If
                        End If
                    End If
                End If
            Next

            If picCount = 0 Then
                ShowError("请先选择要下载的图片")
                Return
            End If

            ' 如果只有一张图片，直接下载
            If picCount = 1 Then
                Dim pic = selectedPics(0)
                Response.Clear()
                Response.ContentType = "application/octet-stream"
                Dim downloadName As String = HttpUtility.UrlEncode(pic.Name)
                Response.AddHeader("Content-Disposition", "attachment; filename=" & downloadName)
                Response.BinaryWrite(pic.Data)
                Response.Flush()
                Response.SuppressContent = True
                HttpContext.Current.ApplicationInstance.CompleteRequest()
                Return
            End If

            ' 多张图片，打包成 ZIP
            Response.Clear()
            Response.ContentType = "application/zip"
            Response.AddHeader("Content-Disposition", "attachment; filename=数据库图片_" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".zip")

            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary
                zip.AlternateEncoding = System.Text.Encoding.UTF8

                For Each pic In selectedPics
                    Dim fileName As String = pic.Name
                    Dim picData As Byte() = CType(pic.Data, Byte())

                    ' ✅ 清理文件名，移除非法字符
                    Dim invalidChars As Char() = Path.GetInvalidFileNameChars()
                    For Each c As Char In invalidChars
                        fileName = fileName.Replace(c, "_"c)
                    Next

                    System.Diagnostics.Debug.WriteLine("Adding to ZIP: " & fileName & ", Size: " & picData.Length & " bytes")

                    ' ✅ 直接使用字节数组
                    zip.AddEntry(fileName, picData)
                Next

                ' ✅ 保存到 MemoryStream，然后写入 Response
                Using ms As New MemoryStream()
                    zip.Save(ms)
                    System.Diagnostics.Debug.WriteLine("ZIP file size: " & ms.Length & " bytes")
                    ms.Position = 0
                    ms.WriteTo(Response.OutputStream)
                End Using
            End Using

            Response.End()

        Catch ex As Exception
            ShowError("批量下载失败: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 批量下载选中文件
    ''' </summary>
    Protected Sub btnBatchDownload_Click(sender As Object, e As EventArgs)
        ' 清除之前的消息状态
        ViewState("LastMsgType") = Nothing

        Try
            Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
            Dim selectedFiles As New List(Of String)()
            Dim fileCount As Integer = 0
            Dim folderCount As Integer = 0
            Dim dbPicCount As Integer = 0

            ' 收集选中的文件和文件夹
            For Each row As GridViewRow In gvFiles.Rows
                Dim chk = CType(row.FindControl("chkSelect"), CheckBox)
                If chk IsNot Nothing AndAlso chk.Checked Then
                    ' 安全地获取 SourceType
                    Dim sourceType As String = "File"
                    Try
                        Dim stValue = gvFiles.DataKeys(row.RowIndex).Values("SourceType")
                        If stValue IsNot Nothing Then
                            sourceType = stValue.ToString()
                        End If
                    Catch ex As Exception
                        sourceType = "File"
                    End Try

                    ' 跳过数据库中的图片（不支持批量下载）
                    If sourceType = "DB" Then
                        dbPicCount += 1
                        Continue For
                    End If

                    ' 安全地获取其他字段
                    Dim pathStr As String = ""
                    Dim isDir As Boolean = False
                    Dim name As String = ""

                    Try
                        pathStr = gvFiles.DataKeys(row.RowIndex).Values("FullName").ToString()
                        isDir = CBool(gvFiles.DataKeys(row.RowIndex).Values("IsDirectory"))
                        name = gvFiles.DataKeys(row.RowIndex).Values("Name").ToString()
                    Catch ex As Exception
                        ' 如果获取失败，跳过这一行
                        Continue For
                    End Try

                    If isDir Then
                        folderCount += 1
                    Else
                        fileCount += 1
                    End If
                    selectedFiles.Add(pathStr)
                End If
            Next

            If selectedFiles.Count = 0 Then
                If dbPicCount > 0 Then
                    ShowError("数据库中的图片不支持批量下载，请单独点击下载按钮")
                Else
                    ShowError("请先选择要下载的文件或文件夹")
                End If
                Return
            End If

            ' 创建ZIP文件并下载
            Response.Clear()
            Response.ContentType = "application/zip"
            Dim zipFileName As String = "批量下载_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".zip"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & HttpUtility.UrlEncode(zipFileName))

            Using zip As New ZipFile()
                zip.AlternateEncodingUsage = ZipOption.AsNecessary
                zip.AlternateEncoding = System.Text.Encoding.UTF8

                For Each filePath In selectedFiles
                    If Directory.Exists(filePath) Then
                        ' 添加文件夹（保持目录结构）
                        Dim dirName = Path.GetFileName(filePath)
                        zip.AddDirectory(filePath, dirName)
                    ElseIf File.Exists(filePath) Then
                        ' 添加单个文件
                        zip.AddFile(filePath, "")
                    End If
                Next

                zip.Save(Response.OutputStream)
            End Using

            Response.Flush()
            Response.SuppressContent = True
            HttpContext.Current.ApplicationInstance.CompleteRequest()

        Catch ex As Exception
            ShowError("批量下载失败: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 判断是否为图片文件
    ''' </summary>
    Private Function IsImageFile(fileName As String) As Boolean
        Dim imageExtensions() As String = {".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg", ".ico"}
        Dim ext = Path.GetExtension(fileName).ToLower()
        Return imageExtensions.Contains(ext)
    End Function

    ''' <summary>
    ''' 解析数据库中的日期时间字符串
    ''' 格式：yyyyMMddHHmmss
    ''' </summary>
    Private Function ParseDatabaseDateTime(dateTimeStr As String) As DateTime
        Try
            If String.IsNullOrEmpty(dateTimeStr) OrElse dateTimeStr.Length < 14 Then
                Return DateTime.Now
            End If

            Dim year As Integer = Integer.Parse(dateTimeStr.Substring(0, 4))
            Dim month As Integer = Integer.Parse(dateTimeStr.Substring(4, 2))
            Dim day As Integer = Integer.Parse(dateTimeStr.Substring(6, 2))
            Dim hour As Integer = Integer.Parse(dateTimeStr.Substring(8, 2))
            Dim minute As Integer = Integer.Parse(dateTimeStr.Substring(10, 2))
            Dim second As Integer = Integer.Parse(dateTimeStr.Substring(12, 2))

            Return New DateTime(year, month, day, hour, minute, second)
        Catch ex As Exception
            Return DateTime.Now
        End Try
    End Function

    ''' <summary>
    ''' 从数据库下载图片（已禁用）
    ''' </summary>
    ' Private Sub DownloadDatabasePicture(itemName As String)
    '     Try
    '         ' 解析 itemName: "picName (picUpdTime)"
    '         Dim matchStart = itemName.LastIndexOf("(")
    '         Dim matchEnd = itemName.LastIndexOf(")")
    '         
    '         If matchStart < 0 OrElse matchEnd < 0 Then
    '             ShowError("无法解析图片信息")
    '             Return
    '         End If
    '         
    '         Dim picName As String = itemName.Substring(0, matchStart).Trim()
    '         Dim picUpdTime As String = itemName.Substring(matchStart + 1, matchEnd - matchStart - 1)
    '         
    '         ' 从数据库获取图片数据
    '         Dim cm As New ComDDL
    '         Dim dtImg As Data.DataTable = cm.SelChkPictureKM(picName, picUpdTime)
    '         
    '         If dtImg.Rows.Count = 0 Then
    '             ShowError("数据库中未找到该图片")
    '             Return
    '         End If
    '         
    '         Dim picConn As Byte() = CType(dtImg.Rows(0)("pic_conn"), Byte())
    '         
    '         ' 下载图片
    '         Response.Clear()
    '         Response.ContentType = "application/octet-stream"
    '         Dim downloadName As String = HttpUtility.UrlEncode(picName)
    '         Response.AddHeader("Content-Disposition", "attachment; filename=" & downloadName)
    '         Response.BinaryWrite(picConn)
    '         Response.Flush()
    '         Response.SuppressContent = True
    '         HttpContext.Current.ApplicationInstance.CompleteRequest()
    '         
    '     Catch ex As Exception
    '         ShowError("下载失败: " & ex.Message)
    '     End Try
    ' End Sub

    ''' <summary>
    ''' 提供缩略图服务
    ''' </summary>
    Private Sub ServeThumbnail(relativePath As String)
        Try
            Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
            If String.IsNullOrEmpty(root) Then
                System.Diagnostics.Debug.WriteLine("ServeThumbnail Error: FileServerPath2 not configured")
                ServePlaceholderImage()
                Return
            End If

            Dim fullPath As String = Path.Combine(root, relativePath)

            If Not File.Exists(fullPath) Then
                System.Diagnostics.Debug.WriteLine("ServeThumbnail Error: File not found: " & fullPath)
                ServePlaceholderImage()
                Return
            End If

            If Not IsImageFile(fullPath) Then
                System.Diagnostics.Debug.WriteLine("ServeThumbnail Error: Not an image file: " & fullPath)
                ServePlaceholderImage()
                Return
            End If

            ' 读取原始图片
            Using originalImage As Image = Image.FromFile(fullPath)
                ' 计算缩略图尺寸（保持宽高比）
                Dim thumbWidth As Integer = 160
                Dim thumbHeight As Integer = 160
                Dim ratio As Double = Math.Min(CDbl(thumbWidth) / originalImage.Width, CDbl(thumbHeight) / originalImage.Height)

                Dim newWidth As Integer = CInt(originalImage.Width * ratio)
                Dim newHeight As Integer = CInt(originalImage.Height * ratio)

                ' 创建缩略图
                Using thumbnail As New Bitmap(newWidth, newHeight)
                    Using graphics As Graphics = Graphics.FromImage(thumbnail)
                        graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                        graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                        graphics.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                        graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight)
                    End Using

                    ' 输出缩略图
                    Response.ContentType = GetImageContentType(fullPath)
                    thumbnail.Save(Response.OutputStream, GetImageFormat(fullPath))
                End Using
            End Using

            Response.Flush()
            Response.SuppressContent = True
            HttpContext.Current.ApplicationInstance.CompleteRequest()

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("ServeThumbnail Error: " & ex.Message)
            ServePlaceholderImage()
        End Try
    End Sub

    ''' <summary>
    ''' 提供数据库图片缩略图服务
    ''' </summary>
    Private Sub ServeDatabasePicture(encodedInfo As String)
        Try
            ' 解码图片信息：picName|picUpdTime
            Dim decodedInfo As String = HttpUtility.UrlDecode(encodedInfo)
            Dim parts() As String = decodedInfo.Split("|"c)

            If parts.Length <> 2 Then
                Response.StatusCode = 400
                Response.Write("Invalid format")
                Response.End()
                Return
            End If

            Dim picName As String = parts(0).Trim()
            Dim picUpdTime As String = parts(1).Trim()

            ' 从数据库获取图片数据
            Dim cm As New ComDDL
            Dim dtImg As Data.DataTable = cm.SelChkPictureKM(picName, picUpdTime)

            ' 调试信息：记录查询结果
            Dim debugMsg As String = "DB Thumb Query - Name: " & picName & ", Time: " & picUpdTime & ", Rows: "
            If dtImg Is Nothing Then
                debugMsg &= "NULL"
            Else
                debugMsg &= dtImg.Rows.Count.ToString()
            End If
            System.Diagnostics.Debug.WriteLine(debugMsg)

            If dtImg Is Nothing OrElse dtImg.Rows.Count = 0 Then
                ' 如果没找到，尝试只按名称查询（忽略时间）
                System.Diagnostics.Debug.WriteLine("DB Thumb: Exact match failed, trying name-only query for: " & picName)
                Dim dtFallback As Data.DataTable = cm.SelChkPictureByName(picName)

                If dtFallback IsNot Nothing AndAlso dtFallback.Rows.Count > 0 Then
                    ' 使用第一条匹配的记录
                    Dim row = dtFallback.Rows(0)
                    Dim fallbackPicConn As Byte() = CType(row("pic_conn"), Byte())

                    If fallbackPicConn IsNot Nothing AndAlso fallbackPicConn.Length > 0 Then
                        ServeImageFromBytes(fallbackPicConn)
                        Return
                    End If
                End If

                ' 如果还是没找到，返回占位图
                ServePlaceholderImage()
                Return
            End If

            Dim picConn As Byte() = CType(dtImg.Rows(0)("pic_conn"), Byte())

            If picConn Is Nothing OrElse picConn.Length = 0 Then
                ' 如果图片数据为空，返回占位图
                ServePlaceholderImage()
                Return
            End If

            ' 成功获取到图片数据，生成缩略图
            ServeImageFromBytes(picConn)

        Catch ex As Exception
            ' 出错时返回占位图
            System.Diagnostics.Debug.WriteLine("DB Thumb Error: " & ex.Message)
            ServePlaceholderImage()
        End Try
    End Sub

    ''' <summary>
    ''' 从字节数组生成并输出缩略图
    ''' </summary>
    Private Sub ServeImageFromBytes(picConn As Byte())
        Try
            Using ms As New MemoryStream(picConn)
                Using originalImage As Image = Image.FromStream(ms)
                    ' 计算缩略图尺寸（保持宽高比）
                    Dim thumbWidth As Integer = 160
                    Dim thumbHeight As Integer = 160
                    Dim ratio As Double = Math.Min(CDbl(thumbWidth) / originalImage.Width, CDbl(thumbHeight) / originalImage.Height)

                    Dim newWidth As Integer = CInt(originalImage.Width * ratio)
                    Dim newHeight As Integer = CInt(originalImage.Height * ratio)

                    ' 创建缩略图
                    Using thumbnail As New Bitmap(newWidth, newHeight)
                        Using graphics As Graphics = Graphics.FromImage(thumbnail)
                            graphics.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                            graphics.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
                            graphics.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                            graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight)
                        End Using

                        ' 输出缩略图
                        Response.ContentType = "image/jpeg"
                        thumbnail.Save(Response.OutputStream, ImageFormat.Jpeg)
                    End Using
                End Using
            End Using

            Response.Flush()
            Response.SuppressContent = True
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("ServeImageFromBytes Error: " & ex.Message)
            ServePlaceholderImage()
        End Try
    End Sub

    ''' <summary>
    ''' 提供默认占位图片
    ''' </summary>
    Private Sub ServePlaceholderImage()
        Try
            ' 创建一个 80x80 的灰色占位图
            Using placeholder As New Bitmap(80, 80)
                Using graphics As Graphics = Graphics.FromImage(placeholder)
                    ' 填充灰色背景
                    graphics.Clear(Drawing.Color.LightGray)

                    ' 绘制文字
                    Dim font As New Drawing.Font("Arial", 10, Drawing.FontStyle.Bold)
                    Dim brush As New Drawing.SolidBrush(Drawing.Color.DarkGray)
                    Dim text As String = "DB"
                    Dim textSize As Drawing.SizeF = graphics.MeasureString(text, font)
                    Dim x As Single = (80 - textSize.Width) / 2
                    Dim y As Single = (80 - textSize.Height) / 2
                    graphics.DrawString(text, font, brush, x, y)
                End Using

                Response.ContentType = "image/png"
                placeholder.Save(Response.OutputStream, ImageFormat.Png)
            End Using

            Response.Flush()
            Response.SuppressContent = True
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            Response.StatusCode = 500
            Response.End()
        End Try
    End Sub

    ''' <summary>
    ''' 获取图片的Content-Type
    ''' </summary>
    Private Function GetImageContentType(filePath As String) As String
        Dim ext = Path.GetExtension(filePath).ToLower()
        Select Case ext
            Case ".jpg", ".jpeg"
                Return "image/jpeg"
            Case ".png"
                Return "image/png"
            Case ".gif"
                Return "image/gif"
            Case ".bmp"
                Return "image/bmp"
            Case ".webp"
                Return "image/webp"
            Case Else
                Return "image/jpeg"
        End Select
    End Function

    ''' <summary>
    ''' 获取图片格式
    ''' </summary>
    Private Function GetImageFormat(filePath As String) As ImageFormat
        Dim ext = Path.GetExtension(filePath).ToLower()
        Select Case ext
            Case ".jpg", ".jpeg"
                Return ImageFormat.Jpeg
            Case ".png"
                Return ImageFormat.Png
            Case ".gif"
                Return ImageFormat.Gif
            Case ".bmp"
                Return ImageFormat.Bmp
            Case Else
                Return ImageFormat.Jpeg
        End Select
    End Function

    ''' <summary>
    ''' 数据库 GridView 的 RowDataBound 事件
    ''' </summary>
    Protected Sub gvDBFiles_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim dataItem = CType(e.Row.DataItem, Object)
            Dim thumbnailUrl = ""
            Dim isImage = False

            ' 使用反射获取动态对象的属性
            If dataItem IsNot Nothing Then
                Dim propThumbnail = dataItem.GetType().GetProperty("ThumbnailUrl")
                Dim propIsImage = dataItem.GetType().GetProperty("IsImage")

                If propThumbnail IsNot Nothing Then
                    Dim thumbValue = propThumbnail.GetValue(dataItem, Nothing)
                    If thumbValue IsNot Nothing Then
                        thumbnailUrl = thumbValue.ToString()
                    End If
                End If
                If propIsImage IsNot Nothing Then
                    Dim imgValue = propIsImage.GetValue(dataItem, Nothing)
                    If imgValue IsNot Nothing Then
                        isImage = CBool(imgValue)
                    End If
                End If
            End If

            ' 显示缩略图或图标
            If isImage AndAlso Not String.IsNullOrEmpty(thumbnailUrl) Then
                Dim imgThumbnail As System.Web.UI.WebControls.Image = CType(e.Row.FindControl("imgThumbnail"), System.Web.UI.WebControls.Image)
                If imgThumbnail IsNot Nothing Then
                    imgThumbnail.Visible = True
                    imgThumbnail.ImageUrl = thumbnailUrl

                    ' 使用反射获取图片名称和 FullName
                    Dim imageName As String = ""
                    Dim fullName As String = ""
                    If dataItem IsNot Nothing Then
                        Dim propName = dataItem.GetType().GetProperty("Name")
                        If propName IsNot Nothing Then
                            Dim nameValue = propName.GetValue(dataItem, Nothing)
                            If nameValue IsNot Nothing Then
                                imageName = nameValue.ToString()
                            End If
                        End If

                        Dim propFullName = dataItem.GetType().GetProperty("FullName")
                        If propFullName IsNot Nothing Then
                            Dim fullNameValue = propFullName.GetValue(dataItem, Nothing)
                            If fullNameValue IsNot Nothing Then
                                fullName = fullNameValue.ToString()
                            End If
                        End If
                    End If

                    ' 生成原图URL（数据库来源使用 dbimg 参数）
                    Dim originalImageUrl As String = "FileServ2.aspx?dbimg=" & HttpUtility.UrlEncode(fullName)

                    imgThumbnail.Attributes.Add("onclick", "previewImage('" & originalImageUrl & "', '" & imageName.Replace("'", "\\'") & "'); event.stopPropagation();")
                    imgThumbnail.Style.Add("cursor", "pointer")
                End If
            Else
                Dim litIcon As Literal = CType(e.Row.FindControl("litIcon"), Literal)
                If litIcon IsNot Nothing Then
                    litIcon.Visible = True
                    litIcon.Text = "<span style='font-size: 40px;'>🖼️</span>"
                End If
            End If

            ' 设置行背景色为白色
            e.Row.BackColor = Drawing.Color.White
        End If
    End Sub

    ''' <summary>
    ''' 数据库 GridView 的 RowCommand 事件
    ''' </summary>
    Protected Sub gvDBFiles_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        Dim itemName As String = e.CommandArgument.ToString()

        If e.CommandName = "DownloadItem" Then
            Try
                ' 解析 itemName: "picName (picUpdTime)"
                Dim matchStart = itemName.LastIndexOf("(")
                Dim matchEnd = itemName.LastIndexOf(")")

                If matchStart < 0 OrElse matchEnd < 0 Then
                    ShowError("无法解析图片信息")
                    Return
                End If

                Dim picName As String = itemName.Substring(0, matchStart).Trim()
                Dim picUpdTime As String = itemName.Substring(matchStart + 1, matchEnd - matchStart - 1)

                ' 从数据库获取图片数据
                Dim cm As New ComDDL
                Dim dtImg As Data.DataTable = cm.SelChkPictureKM(picName, picUpdTime)

                If dtImg Is Nothing OrElse dtImg.Rows.Count = 0 Then
                    ShowError("数据库中未找到该图片")
                    Return
                End If

                Dim picConn As Byte() = CType(dtImg.Rows(0)("pic_conn"), Byte())

                ' 下载图片
                Response.Clear()
                Response.ContentType = "application/octet-stream"
                Dim downloadName As String = HttpUtility.UrlEncode(picName)
                Response.AddHeader("Content-Disposition", "attachment; filename=" & downloadName)
                Response.BinaryWrite(picConn)
                Response.Flush()
                Response.SuppressContent = True
                HttpContext.Current.ApplicationInstance.CompleteRequest()

            Catch ex As Exception
                ShowError("下载失败: " & ex.Message)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' 数据库 GridView 分页事件（完全独立，不受文件夹影响）
    ''' </summary>
    Protected Sub gvDBFiles_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        ' ✅ 只处理数据库相关逻辑，与文件夹完全无关
        Dim dbKeyword As String = SearchKeyword
        If Not String.IsNullOrEmpty(dbKeyword) Then
            Try
                Dim dbList As New List(Of Object)()
                Dim dbTotalCount As Integer = 0

                ' ✅ 设置 DBPageIndex（不使用 gvDBFiles.PageIndex）
                DBPageIndex = e.NewPageIndex

                ' 使用分页查询，只获取当前页需要的数据（不包含图片数据）
                Dim dbPagedData As Data.DataTable = GetDatabasePicturesPaged(dbKeyword, DBPageIndex, gvDBFiles.PageSize, dbTotalCount)

                ' 保存总记录数到 ViewState
                dbTotalCount = dbTotalCount

                If dbPagedData IsNot Nothing AndAlso dbPagedData.Rows.Count > 0 Then
                    For Each row As Data.DataRow In dbPagedData.Rows
                        ' 安全检查：确保字段存在且不为空
                        If row.Table.Columns.Contains("pic_name") AndAlso row("pic_name") IsNot DBNull.Value Then
                            Dim picName As String = row("pic_name").ToString()
                            Dim picUpdTime As String = ""

                            ' 安全获取更新时间
                            If row.Table.Columns.Contains("pic_upd_time") AndAlso row("pic_upd_time") IsNot DBNull.Value Then
                                picUpdTime = row("pic_upd_time").ToString()
                            End If

                            Dim thumbnailUrl As String = "FileServ2.aspx?dbthumb=" & HttpUtility.UrlEncode(picName & "|" & picUpdTime)

                            dbList.Add(New With {
                                .Name = picName & " (" & picUpdTime & ")",
                                .FullName = picName & "|" & picUpdTime,
                                .IsDirectory = False,
                                .LastWriteTime = CDate(ParseDatabaseDateTime(picUpdTime)),
                                .ThumbnailUrl = CStr(thumbnailUrl),
                                .IsImage = True,
                                .SourceType = CStr("DB"),
                                .PicData = Nothing
                            })
                        End If
                    Next
                End If

                lblDBCount.Text = "共 " & dbTotalCount & " 个项目，当前第 " & (DBPageIndex + 1) & "/" & Math.Ceiling(dbTotalCount / gvDBFiles.PageSize) & " 页"

                gvDBFiles.DataSource = dbList
                gvDBFiles.DataBind()
            Catch ex As Exception
                ShowError("数据库搜索失败: " & ex.Message)
            End Try
        End If

        ' 更新数据库分页控件
        UpdateDBPager()
    End Sub

    ''' <summary>
    ''' 更新文件夹分页控件（完全独立的手动分页）
    ''' </summary>
    Private Sub UpdateFolderPager()
        Try
            Dim totalPages As Integer = 0
            If FolderTotalCount > 0 AndAlso gvFiles.PageSize > 0 Then
                totalPages = CInt(Math.Ceiling(FolderTotalCount / CDbl(gvFiles.PageSize)))
            End If

            Dim currentPage As Integer = FolderPageIndex + 1

            If totalPages <= 1 Then
                folderPager.InnerHtml = ""
                Return
            End If

            Dim html As String = "<div class='manual-pager' style='margin-top: 10px;'>"

            ' 上一页
            If currentPage > 1 Then
                Dim prevIndex As Integer = currentPage - 2
                html &= "<a href=""javascript:__doPostBack('FolderPaging', '" & prevIndex & "')"" style='padding: 8px 14px; margin: 0 3px; border: 1px solid #4caf50; border-radius: 4px; text-decoration: none; color: #4caf50; font-size: 15px; display: inline-block;'>←</a>"
            End If

            ' 页码
            For i As Integer = 1 To totalPages
                If i = currentPage Then
                    html &= "<span style='padding: 8px 14px; margin: 0 3px; background-color: #4caf50; color: white; border: 1px solid #4caf50; border-radius: 4px; display: inline-block; font-size: 15px; font-weight: bold;'>" & i & "</span>"
                Else
                    Dim pageIndex As Integer = i - 1
                    html &= "<a href=""javascript:__doPostBack('FolderPaging', '" & pageIndex & "')"" style='padding: 8px 14px; margin: 0 3px; border: 1px solid #ddd; border-radius: 4px; text-decoration: none; color: #333; font-size: 15px; display: inline-block;'>" & i & "</a>"
                End If
            Next

            ' 下一页
            If currentPage < totalPages Then
                html &= "<a href=""javascript:__doPostBack('FolderPaging', '" & currentPage & "')"" style='padding: 8px 14px; margin: 0 3px; border: 1px solid #4caf50; border-radius: 4px; text-decoration: none; color: #4caf50; font-size: 15px; display: inline-block;'>→</a>"
            End If

            html &= "</div>"
            folderPager.InnerHtml = html
        Catch ex As Exception
            folderPager.InnerHtml = ""
        End Try
    End Sub

    ''' <summary>
    ''' 更新数据库分页控件（完全独立的手动分页）
    ''' </summary>
    Private Sub UpdateDBPager()
        Try
            ' 使用保存的总记录数计算总页数
            Dim totalPages As Integer = 0
            If DBTotalCount > 0 AndAlso gvDBFiles.PageSize > 0 Then
                totalPages = CInt(Math.Ceiling(DBTotalCount / CDbl(gvDBFiles.PageSize)))
            End If

            ' ✅ 使用 DBPageIndex 而不是 gvDBFiles.PageIndex
            Dim currentPage As Integer = DBPageIndex + 1

            If totalPages <= 1 Then
                dbPager.InnerHtml = ""
                Return
            End If

            Dim html As String = "<div class='manual-pager' style='margin-top: 10px;'>"

            ' 上一页
            If currentPage > 1 Then
                Dim prevIndex As Integer = currentPage - 2
                html &= "<a href=""javascript:__doPostBack('DBPaging', '" & prevIndex & "')"" style='padding: 8px 14px; margin: 0 3px; border: 1px solid #ff5722; "
                html &= "border-radius: 4px; text-decoration: none; color: #ff5722; "
                html &= "font-size: 15px; display: inline-block;'>←</a>"
            End If

            ' 页码
            For i As Integer = 1 To totalPages
                If i = currentPage Then
                    html &= "<span style='padding: 8px 14px; margin: 0 3px; background-color: #ff5722; color: white; "
                    html &= "border: 1px solid #ff5722; border-radius: 4px; display: inline-block; "
                    html &= "font-size: 15px; font-weight: bold;'>" & i & "</span>"
                Else
                    Dim pageIndex As Integer = i - 1
                    html &= "<a href=""javascript:__doPostBack('DBPaging', '" & pageIndex & "')"" "
                    html &= "style='padding: 8px 14px; margin: 0 3px; border: 1px solid #ddd; "
                    html &= "border-radius: 4px; text-decoration: none; color: #333; "
                    html &= "font-size: 15px; display: inline-block;'>" & i & "</a>"
                End If
            Next

            ' 下一页
            If currentPage < totalPages Then
                html &= "<a href=""javascript:__doPostBack('DBPaging', '" & (currentPage - 1) & "')"" "
                html &= "style='padding: 8px 14px; margin: 0 3px; border: 1px solid #ff5722; "
                html &= "border-radius: 4px; text-decoration: none; color: #ff5722; "
                html &= "font-size: 15px; display: inline-block;'>→</a>"
            End If

            html &= "</div>"
            dbPager.InnerHtml = html
        Catch ex As Exception
            dbPager.InnerHtml = ""
        End Try
    End Sub

    ''' <summary>
    ''' 分页查询数据库图片（不包含图片数据，减少内存占用）
    ''' </summary>
    Private Function GetDatabasePicturesPaged(ByVal keyword As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalCount As Integer) As Data.DataTable
        Try
            ' 使用与 ComDDL 相同的连接字符串
            Dim connStr As String = ""
            Try
                connStr = DataAccessManager.ConnStr
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("GetDatabasePicturesPaged - ConnStr Error: " & ex.Message)
                totalCount = 0
                Return Nothing
            End Try

            If String.IsNullOrEmpty(connStr) Then
                System.Diagnostics.Debug.WriteLine("GetDatabasePicturesPaged - ConnStr is empty!")
                totalCount = 0
                Return Nothing
            End If

            ' 先获取总数
            Dim countSql As String = "SELECT COUNT(*) FROM m_picture_km WHERE 1=1"
            If Not String.IsNullOrEmpty(keyword) Then
                countSql &= " AND pic_name LIKE N'%' + @keyword + '%'"
            End If

            Using conn As New System.Data.SqlClient.SqlConnection(connStr)
                conn.Open()

                ' 获取总数
                Using cmdCount As New System.Data.SqlClient.SqlCommand(countSql, conn)
                    If Not String.IsNullOrEmpty(keyword) Then
                        cmdCount.Parameters.AddWithValue("@keyword", keyword)
                    End If
                    totalCount = Convert.ToInt32(cmdCount.ExecuteScalar())
                End Using

                ' 如果总数为0，返回空表
                If totalCount = 0 Then
                    Return Nothing
                End If

                ' 分页查询（只查询元数据，不查询图片数据）
                Dim startRow As Integer = (pageIndex * pageSize) + 1
                Dim endRow As Integer = (pageIndex + 1) * pageSize

                Dim sql As String = "SELECT * FROM (" &
                                   "    SELECT ROW_NUMBER() OVER (ORDER BY pic_name, pic_upd_time DESC) AS RowNum, " &
                                   "    pic_name, pic_upd_time " &
                                   "    FROM m_picture_km " &
                                   "    WHERE 1=1"

                If Not String.IsNullOrEmpty(keyword) Then
                    sql &= "    AND pic_name LIKE N'%' + @keyword + '%'"
                End If

                sql &= ") AS PagedData WHERE RowNum BETWEEN @StartRow AND @EndRow"

                Using cmd As New System.Data.SqlClient.SqlCommand(sql, conn)
                    If Not String.IsNullOrEmpty(keyword) Then
                        cmd.Parameters.AddWithValue("@keyword", keyword)
                    End If
                    cmd.Parameters.AddWithValue("@StartRow", startRow)
                    cmd.Parameters.AddWithValue("@EndRow", endRow)

                    Dim dt As New Data.DataTable()
                    Using adapter As New System.Data.SqlClient.SqlDataAdapter(cmd)
                        adapter.Fill(dt)
                    End Using

                    Return dt
                End Using
            End Using

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("GetDatabasePicturesPaged Error: " & ex.Message)
            totalCount = 0
            ' エラーメッセージをデバッグ情報として表示
            dbPager.InnerHtml = "<!-- GetDatabasePicturesPaged Error: " & ex.Message & " -->"
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 从数据库获取单张图片数据（用于批量下载）
    ''' </summary>
    Private Function GetPictureDataFromDB(ByVal picName As String, ByVal picUpdTime As String) As Byte()
        Try
            Dim connStr As String = ""
            Try
                connStr = DataAccessManager.ConnStr
            Catch ex As Exception
                Return Nothing
            End Try

            If String.IsNullOrEmpty(connStr) Then
                Return Nothing
            End If

            Dim sql As String = "SELECT pic_conn FROM m_picture_km WHERE pic_name = @picName AND pic_upd_time = @picUpdTime"

            Using conn As New System.Data.SqlClient.SqlConnection(connStr)
                conn.Open()

                Using cmd As New System.Data.SqlClient.SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@picName", picName)
                    cmd.Parameters.AddWithValue("@picUpdTime", picUpdTime)

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                        Dim picData As Byte() = CType(result, Byte())
                        System.Diagnostics.Debug.WriteLine("GetPictureDataFromDB: " & picName & " | " & picUpdTime & ", Size: " & picData.Length & " bytes")
                        Return picData
                    Else
                        System.Diagnostics.Debug.WriteLine("GetPictureDataFromDB: No data found for " & picName & " | " & picUpdTime)
                    End If
                End Using
            End Using

            Return Nothing

        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("GetPictureDataFromDB Error: " & ex.Message)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 从文件系统绑定网格（支持搜索和手动分页）
    ''' </summary>
    Private Sub BindGridFromFileSystemSearch()
        Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
        If String.IsNullOrEmpty(root) Then
            ShowError("未配置文件服务器路径")
            Return
        End If

        Dim fullPath As String = Path.Combine(root, RelPath)
        If Not Directory.Exists(fullPath) Then
            ShowError("目录不存在: " & fullPath)
            Return
        End If

        Dim di As New DirectoryInfo(fullPath)
        Dim allItems As New List(Of Object)

        ' 获取所有文件夹
        For Each d In di.GetDirectories()
            ' 如果有搜索关键词，过滤文件夹名
            If String.IsNullOrEmpty(SearchKeyword) OrElse d.Name.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                allItems.Add(New With {
                    .Name = d.Name,
                    .FullName = d.FullName,
                    .IsDirectory = True,
                    .LastWriteTime = CDate(d.LastWriteTime),
                    .ThumbnailUrl = CStr(""),
                    .IsImage = False,
                    .SourceType = CStr("File")
                })
            End If
        Next

        ' 获取所有文件
        For Each f In di.GetFiles()
            ' 如果有搜索关键词，过滤文件名
            If String.IsNullOrEmpty(SearchKeyword) OrElse f.Name.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0 Then
                Dim isImage As Boolean = IsImageFile(f.Name)
                Dim thumbnailUrl As String = ""

                ' 如果是图片文件，生成缩略图URL
                If isImage Then
                    thumbnailUrl = "FileServ2.aspx?thumbnail=" & HttpUtility.UrlEncode(Path.Combine(RelPath, f.Name))
                End If

                allItems.Add(New With {
                    .Name = f.Name,
                    .FullName = f.FullName,
                    .IsDirectory = False,
                    .LastWriteTime = CDate(f.LastWriteTime),
                    .ThumbnailUrl = CStr(thumbnailUrl),
                    .IsImage = isImage,
                    .SourceType = CStr("File")
                })
            End If
        Next

        FolderTotalCount = allItems.Count

        ' 手動で現在のページのデータを切り出す
        Dim pageSize As Integer = gvFiles.PageSize
        Dim startIndex As Integer = FolderPageIndex * pageSize
        Dim pageData As New List(Of Object)
        For i As Integer = startIndex To Math.Min(startIndex + pageSize - 1, allItems.Count - 1)
            pageData.Add(allItems(i))
        Next

        litPath.Text = If(String.IsNullOrEmpty(SearchKeyword), "共享文件夹", "文件夹搜索结果")
        lblFolderCount.Text = "共 " & FolderTotalCount & " 个项目，当前第 " & (FolderPageIndex + 1) & "/" & Math.Ceiling(FolderTotalCount / pageSize) & " 页"

        gvFiles.DataSource = pageData
        gvFiles.DataBind()
        
        ' 更新分页控件
        UpdateFolderPager()
    End Sub

    ''' <summary>
    ''' 提供共享文件夹的原图
    ''' </summary>
    Private Sub ServeOriginalImage(relativePath As String)
        Try
            Dim root As String = ConfigurationManager.AppSettings("FileServerPath2")
            Dim fullPath As String = Path.Combine(root, relativePath)

            If Not File.Exists(fullPath) Then
                Response.StatusCode = 404
                Response.End()
                Return
            End If

            ' 读取并输出原图
            Dim fileBytes As Byte() = File.ReadAllBytes(fullPath)
            Response.ContentType = GetImageContentType(fullPath)
            Response.BinaryWrite(fileBytes)
            Response.Flush()
            Response.SuppressContent = True
            HttpContext.Current.ApplicationInstance.CompleteRequest()

        Catch ex As Exception
            Response.StatusCode = 500
            Response.End()
        End Try
    End Sub

    ''' <summary>
    ''' 提供数据库的原图
    ''' </summary>
    Private Sub ServeDatabaseOriginalImage(picInfo As String)
        Try
            ' 解析 picInfo: "picName|picUpdTime"
            Dim parts As String() = picInfo.Split("|"c)
            If parts.Length < 2 Then
                Response.StatusCode = 400
                Response.End()
                Return
            End If

            Dim picName As String = parts(0)
            Dim picUpdTime As String = parts(1)

            ' 从数据库获取图片数据
            Dim cm As New ComDDL
            Dim dtImg As Data.DataTable = cm.SelChkPictureKM(picName, picUpdTime)

            If dtImg Is Nothing OrElse dtImg.Rows.Count = 0 Then
                Response.StatusCode = 404
                Response.End()
                Return
            End If

            Dim picConn As Byte() = CType(dtImg.Rows(0)("pic_conn"), Byte())

            If picConn Is Nothing OrElse picConn.Length = 0 Then
                Response.StatusCode = 404
                Response.End()
                Return
            End If

            ' 输出原图
            Response.ContentType = "image/jpeg"
            Response.BinaryWrite(picConn)
            Response.Flush()
            Response.SuppressContent = True
            HttpContext.Current.ApplicationInstance.CompleteRequest()

        Catch ex As Exception
            Response.StatusCode = 500
            Response.End()
        End Try
    End Sub

End Class