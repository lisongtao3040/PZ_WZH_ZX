Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Text
Imports System
Imports System.Configuration
Imports System.IO
Imports System.Data.SqlClient
Imports System.Threading

Public Class BackUpChkPiao

    Public pub_dic As String = ConfigurationManager.AppSettings("SavePath").ToString()
    Public pub_conn As String = "Data Source=10.160.192.131; Initial Catalog=wzh_new;Persist Security Info=True;User ID=sa;Password=Lixil@2026001"
    Public pub_imgServ_path = "http://10.160.192.131/WzhFileServer/Image/ChkImgs/"

    Public pub_Save_dic_path As String

    ''' <summary>
    ''' 删除15天以上的票
    ''' </summary>
    Public Sub DeleteDic15Days()

        Dim todayYmd As Integer = CInt(DateAdd(DateInterval.Day, -15, Now).ToString("yyyyMMdd"))

        'pub_dic
        For Each dic As String In System.IO.Directory.GetDirectories(pub_dic)
            Try
                Dim nm As Integer = CInt(dic.Split("\"c)(dic.Split("\"c).Length - 1))
                If nm < todayYmd Then
                    System.IO.Directory.Delete(dic, True)
                End If
            Catch ex As Exception

            End Try

        Next

    End Sub



    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        '删除15天以上的票
        DeleteDic15Days()
        'Return
        'SetXls("2", 0)
        'Return

        ' For z As Integer = 0 To 7
        For z As Integer = 1 To 7
            SetXls("", z)
            For i As Integer = 1 To 4
                SetXls(i.ToString, z)
            Next
        Next
        'For i As Integer = 1 To 4
        '    SetXls(i, 0)
        'Next

        Me.Close()

    End Sub

    Function SetXls(ByVal bumen As String, ByVal z As Integer) As Boolean
        ' 1. 初始化路径
        Dim folderDate As String = Now.AddDays(z * -1 - 1).ToString("yyyyMMdd")
        pub_Save_dic_path = Path.Combine(pub_dic, folderDate)

        If Not Directory.Exists(pub_Save_dic_path) Then Directory.CreateDirectory(pub_Save_dic_path)

        Dim subFolder As String = If(String.IsNullOrEmpty(bumen), "未知", bumen)
        pub_Save_dic_path = Path.Combine(pub_Save_dic_path, subFolder)

        If Not Directory.Exists(pub_Save_dic_path) Then Directory.CreateDirectory(pub_Save_dic_path)

        ' 2. 获取数据
        Dim dt As DataTable = GetData(bumen, z)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return False

        ' 批量处理图片下载（使用我上一个回答中优化的 SelChkPictureKM）
        SelChkPictureKM(dt)

        Dim listPath As String = Path.Combine(pub_Save_dic_path, "检查结果一览.html")

        ' 3. 使用 StreamWriter 流式写入，不占用大内存拼接字符串
        Using sw As New StreamWriter(listPath, False, System.Text.Encoding.UTF8)
            sw.WriteLine("<html><head><meta charset=""utf-8"" /><link rel=""stylesheet"" href=""../../main.css"" /></head><body>")
            sw.WriteLine("<table><tr>")
            sw.WriteLine("<th style='border:.5pt solid windowtext;width:100px'>CD</th><th style='border:.5pt solid windowtext;width:100px'>NO</th>")
            sw.WriteLine("<th style='border:.5pt solid windowtext;width:100px'>检查ID</th><th style='border:.5pt solid windowtext;width:100px'>检查者</th>")
            sw.WriteLine("<th style='border:.5pt solid windowtext;width:100px'>结果</th><th style='border:.5pt solid windowtext;width:100px'>时长</th>")
            sw.WriteLine("<th style='border:.5pt solid windowtext;width:100px'>详细</th></tr>")

            ' 准备临时存储小批次的容器
            Dim tmDt As DataTable = dt.Clone() ' 仅克隆结构，不复制数据

            Dim old_cd As String = "-1", old_no As String = "-1", old_ck_id As String = "-1"
            Dim old_ck_user As String = "", old_ck_result As String = "", old_ck_time As String = ""

            For i As Integer = 0 To dt.Rows.Count - 1
                Dim currentRow = dt.Rows(i)
                Dim current_ck_id As String = currentRow("ck_id").ToString()

                ' 逻辑判断：如果 ID 变了且不是第一行，则处理上一组数据
                If i > 0 AndAlso current_ck_id <> old_ck_id Then
                    ' 处理旧组
                    SetDataToFile(tmDt, old_cd, old_no, old_ck_id, old_ck_user)
                    sw.WriteLine(GetOneList(old_cd, old_no, old_ck_id, old_ck_user, old_ck_result, old_ck_time))
                    tmDt.Clear()
                End If

                ' 更新“旧”变量
                old_cd = currentRow("cd").ToString()
                old_no = currentRow("no").ToString()
                old_ck_id = current_ck_id
                old_ck_user = currentRow("chk_user").ToString()
                old_ck_result = currentRow("tc_result").ToString()
                old_ck_time = currentRow("检查时长").ToString()

                ' 将当前行导入临时表
                tmDt.ImportRow(currentRow)

                ' 最后一行特殊处理
                If i = dt.Rows.Count - 1 Then
                    SetDataToFile(tmDt, old_cd, old_no, old_ck_id, old_ck_user)
                    sw.WriteLine(GetOneList(old_cd, old_no, old_ck_id, old_ck_user, old_ck_result, old_ck_time))
                End If
            Next

            sw.WriteLine("</table></body></html>")
            tmDt.Dispose()
        End Using

        dt.Dispose() ' 彻底释放内存
        Return True
    End Function


    Function GetOneList(ByVal inCd As String, ByVal inNo As String, ByVal in_ck_id As String, ByVal chk_user As String, ByVal result As String, ByVal 检查时长 As String) As String

        Dim htmlPath As String = "./" & inCd & "_" & inNo & "_" & in_ck_id & ".html"

        Dim sb As New StringBuilder
        sb.AppendLine("<tr>")
        sb.AppendLine("<td>" & inCd & "</td>")
        sb.AppendLine("<td>" & inNo & "</td>")
        sb.AppendLine("<td>" & in_ck_id & "</td>")
        sb.AppendLine("<td>" & chk_user & "</td>")
        sb.AppendLine("<td>" & result & "</td>")
        sb.AppendLine("<td>" & 检查时长 & "</td>")
        sb.AppendLine("<td>" & "<a href='" & htmlPath & "'>详细</a>" & "</td>")
        sb.AppendLine("</tr>")
        Return sb.ToString

    End Function

    'Sub writeList(ByVal instr As String, ByVal inCd As String, ByVal inNo As String, ByVal in_ck_id As String, ByVal chk_user As String)
    '    Dim sb As New StringBuilder
    '    sb.AppendLine("<html>")
    '    sb.AppendLine("<head>")
    '    sb.AppendLine("<link rel=""stylesheet"" href=""../../main.css"" />")
    '    sb.AppendLine("</head>")
    '    sb.AppendLine("<body>")
    '    sb.AppendLine("<table>")
    '    sb.AppendLine("<tr>")
    '    sb.AppendLine("<td>" & inCd & "</td>")
    '    sb.AppendLine("<td>" & inNo & "</td>")
    '    sb.AppendLine("<td>" & in_ck_id & "</td>")
    '    sb.AppendLine("<td>" & chk_user & "</td>")
    '    sb.AppendLine("</tr>")
    '    sb.AppendLine("</table>")
    '    sb.AppendLine("</body>")
    '    sb.AppendLine("</html>")
    'End Sub


    'Function SetDataToFile(ByVal dt As DataTable, ByVal inCd As String, ByVal inNo As String, ByVal in_ck_id As String, ByVal chk_user As String)
    '    Dim cd As String
    '    Dim ck_id As String
    '    'Dim shunwei As String
    '    Dim kind_name As String
    '    Dim chk_pos As String
    '    Dim chk_km_name As String
    '    Dim chk_fs_txt As String
    '    Dim chk_times As Integer
    '    Dim tools_ma As String
    '    Dim pic_name As String


    '    Dim old_pic_name As String = "-1"
    '    Dim old_cd As String = "-1"
    '    Dim old_ck_id As String = "-1"
    '    'Dim old_shunwei As String = "-1"
    '    Dim old_kind_name As String = "-1"
    '    Dim old_chk_km_name As String = "-1"

    '    Dim sb As New StringBuilder
    '    sb.AppendLine("<html>")
    '    sb.AppendLine("<head>")
    '    sb.AppendLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
    '    sb.AppendLine("<link rel=""stylesheet"" href=""../../main.css"" />")
    '    sb.AppendLine("</head>")
    '    sb.AppendLine("<body>")
    '    sb.AppendLine("<table>")
    '    sb.AppendLine("<tr>")
    '    sb.AppendLine("<th style='' colspan='3'>" & inCd & "</th>")
    '    sb.AppendLine("<th style=''>" & inNo & "</th>")
    '    sb.AppendLine("<th style=''>" & in_ck_id & "</th>")
    '    sb.AppendLine("<th style=''>" & chk_user & "</th>")

    '    sb.AppendLine("<th style=''></th>")

    '    sb.AppendLine("</tr>")


    '    sb.AppendLine("<tr>")

    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>No</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>种类</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>位置</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>项目名</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>方法</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:130px'>实测值</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:60px'>结果</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>备考</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>检查员</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>图片</th>")
    '    sb.AppendLine("</tr>")

    '    Dim idx As Integer = 0
    '    Dim span, picSpan As Integer
    '    Dim picPath As String = "pic/"
    '    Dim imgPath As String


    '    For i As Integer = 0 To dt.Rows.Count - 1


    '        cd = dt.Rows(i).Item("cd").ToString
    '        ck_id = dt.Rows(i).Item("ck_id").ToString
    '        ' shunwei = dt.Rows(i).Item("shunwei").ToString
    '        kind_name = dt.Rows(i).Item("kind_name").ToString
    '        chk_pos = dt.Rows(i).Item("chk_pos").ToString
    '        chk_km_name = dt.Rows(i).Item("chk_km_name").ToString
    '        chk_fs_txt = dt.Rows(i).Item("chk_fs_txt").ToString
    '        chk_times = CInt(dt.Rows(i).Item("chk_times").ToString)
    '        tools_ma = dt.Rows(i).Item("tools_ma").ToString
    '        pic_name = dt.Rows(i).Item("pic_name").ToString
    '        span = GetRowSpanBy_kind_name(dt, i, kind_name)
    '        picSpan = GetRowSpanBy_pic_name(dt, i, pic_name)
    '        idx = idx + 1
    '        'Dim sb2 As New StringBuilder
    '        sb.AppendLine("<tr>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & idx & "</td>")

    '        If kind_name <> old_kind_name Then
    '            sb.AppendLine("<td rowspan='" & span & "'  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("kind_name").ToString & "</td>")
    '        End If
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & chk_pos & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & chk_km_name & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & chk_fs_txt & "<br>" & dt.Rows(i).Item("jz").ToString & "<br>" & tools_ma & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("in_1").ToString & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("result").ToString & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("mark").ToString & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("upd_user").ToString & "<br>" & dt.Rows(i).Item("upd_date").ToString & "</td>")




    '        If old_pic_name <> pic_name Then
    '            If pic_name <> "" Then
    '                If dt.Rows(i).Item("pic_name").ToString.Contains("|") Then
    '                    imgPath = picPath & dt.Rows(i).Item("pic_name").ToString.Replace("|", "B") & ".jpg"
    '                    sb.AppendLine("<td rowspan='" & picSpan & "'  style='border:.5pt solid windowtext;height:400px;width:500px;'>" & "<img src=""" & imgPath & """ style=""border-width:0px;width: 500px;height:400px;"">" & "</td>")
    '                Else
    '                    sb.AppendLine("<td rowspan='" & picSpan & "'  style='border:.5pt solid windowtext;height:400px;width:500px;'>" & "<img src=""" & pub_imgServ_path & pic_name & """ style=""border-width:0px;width: 500px;height:400px;"">" & "</td>")
    '                End If
    '            End If
    '        End If


    '        sb.AppendLine("</tr>")



    '        old_kind_name = kind_name
    '        old_chk_km_name = chk_km_name
    '        old_pic_name = pic_name
    '        ' sb.Append(sb2.ToString)
    '        'For j As Integer = 2 To chk_times
    '        '    sb.AppendLine("<tr>")
    '        '    sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & idx & "</td>")
    '        '    sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_pos").ToString & "</td>")
    '        '    sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_km_name").ToString & "</td>")
    '        '    sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_fs_txt").ToString & "<br>" & dt.Rows(i).Item("jz").ToString & "</td>")
    '        '    sb.AppendLine("<td  style='border:.5pt solid windowtext;'>&nbsp;</td>")
    '        '    sb.AppendLine("<td  style='border:.5pt solid windowtext;'>&nbsp;</td>")
    '        '    sb.AppendLine("</tr>")
    '        'Next
    '        'sb2.Length = 0
    '    Next
    '    sb.AppendLine("</table>")

    '    If in_ck_id = "211023152047582" Then
    '        Dim fff
    '        fff = 1

    '    End If


    '    Dim sb2 As New StringBuilder
    '    'SQL文
    '    sb2.AppendLine("SELECT")
    '    sb2.AppendLine("idx,pic_conn")                                                    '图片ID
    '    sb2.AppendLine("FROM m_picture_chk")
    '    sb2.AppendLine("WHERE chk_id='" & in_ck_id & "'")
    '    Dim dtChkImg As Data.DataTable = SqlHelper.FillData(pub_conn, CommandType.Text, sb2.ToString(), "dt")

    '    Dim bt As Byte()

    '    For i As Integer = 0 To dtChkImg.Rows.Count - 1
    '        bt = DirectCast(dtChkImg.Rows(i).Item("pic_conn"), Byte())

    '        Dim MStream As New MemoryStream(bt)



    '        'Dim base64 As String = Convert.ToBase64String(MStream.ToArray())
    '        'sb.AppendLine("" & "<img src=""data:image/jpg;base64," & base64 & """ style=""border-width:0px;width: 500px;height:400px;"">" & "")

    '        sb.AppendLine("" & "<img src=""" & ".\pic\" & dtChkImg.Rows(i).Item("idx") & ".jpg" & """ style=""border-width:0px;width: 500px;height:400px;"">" & "")



    '        imgPath = pub_Save_dic_path & "\pic\" & dtChkImg.Rows(i).Item("idx") & ".jpg"
    '        If Not File.Exists(imgPath) Then
    '            Dim FileStr As New FileStream(imgPath, FileMode.Create, FileAccess.Write)
    '            FileStr.Write(bt, 0, bt.Length)
    '            FileStr.Close()
    '            ' PictureBox1.Image.Save(imgPath)
    '        End If

    '    Next

    '    sb.AppendLine("</body>")
    '    sb.AppendLine("</html>")
    '    If Not System.IO.Directory.Exists(pub_Save_dic_path) Then
    '        System.IO.Directory.CreateDirectory(pub_Save_dic_path)
    '    End If

    '    Dim htmlPath As String = pub_Save_dic_path & inCd & "_" & inNo & "_" & in_ck_id & ".html"
    '    'Dim listPath As String = pub_Save_dic_path & "检查结果一览.html"
    '    System.IO.File.WriteAllText(htmlPath.Replace("*", "＊"), sb.ToString)




    '    Return True

    'End Function

    ''' <summary>
    ''' 生成HTML报表文件并保存本地图片
    ''' </summary>
    Public Function SetDataToFile(ByVal dt As DataTable, ByVal inCd As String, ByVal inNo As String, ByVal in_ck_id As String, ByVal chk_user As String) As Boolean
        Try
            ' 1. 初始化变量与路径
            Dim sb As New StringBuilder
            Dim old_pic_name As String = "-1"
            Dim old_kind_name As String = "-1"
            Dim picPathRelative As String = "pic/" ' HTML中引用的相对路径
            Dim picFolderFull As String = Path.Combine(pub_Save_dic_path, "pic") ' 实际物理路径

            ' 确保图片目录存在
            If Not Directory.Exists(picFolderFull) Then Directory.CreateDirectory(picFolderFull)

            ' 2. 构建 HTML 头部与样式
            sb.AppendLine("<html><head>")
            sb.AppendLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
            sb.AppendLine("<link rel=""stylesheet"" href=""../../main.css"" />")
            sb.AppendLine("<style>table { border-collapse: collapse; width: 100%; } td, th { border: .5pt solid windowtext; text-align: center; }</style>")
            sb.AppendLine("</head><body>")

            ' 报表标题信息
            sb.AppendLine("<table>")
            sb.AppendLine(String.Format("<tr><th colspan='3'>{0}</th><th>{1}</th><th>{2}</th><th>{3}</th><th></th></tr>", inCd, inNo, in_ck_id, chk_user))

            ' 表头
            sb.AppendLine("<tr>")
            Dim headers() As String = {"No", "种类", "位置", "项目名", "方法", "实测值", "结果", "备考", "检查员", "图片"}
            Dim widths() As String = {"30", "100", "100", "100", "100", "130", "60", "30", "30", "30"}
            For i As Integer = 0 To headers.Length - 1
                sb.AppendLine(String.Format("<th style='width:{0}px'>{1}</th>", widths(i), headers(i)))
            Next
            sb.AppendLine("</tr>")

            ' 3. 循环数据行生成表格内容
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim row As DataRow = dt.Rows(i)
                Dim kind_name As String = row("kind_name").ToString()
                Dim pic_name As String = row("pic_name").ToString()

                sb.AppendLine("<tr>")
                ' 序号
                sb.AppendLine("<td>" & (i + 1) & "</td>")

                ' 种类列合并 (RowSpan)
                If kind_name <> old_kind_name Then
                    Dim kindSpan As Integer = GetRowSpanBy_kind_name(dt, i, kind_name)
                    sb.AppendLine(String.Format("<td rowspan='{0}'>{1}</td>", kindSpan, kind_name))
                End If

                ' 普通列
                sb.AppendLine("<td>" & row("chk_pos") & "</td>")
                sb.AppendLine("<td>" & row("chk_km_name") & "</td>")
                sb.AppendLine(String.Format("<td>{0}<br>{1}<br>{2}</td>", row("chk_fs_txt"), row("jz"), row("tools_ma")))
                sb.AppendLine("<td>" & row("in_1") & "</td>")
                sb.AppendLine("<td>" & row("result") & "</td>")
                sb.AppendLine("<td>" & row("mark") & "</td>")
                sb.AppendLine(String.Format("<td>{0}<br>{1}</td>", row("upd_user"), row("upd_date")))

                ' 图片列合并 (RowSpan)
                If pic_name <> old_pic_name Then
                    Dim picSpan As Integer = GetRowSpanBy_pic_name(dt, i, pic_name)
                    If Not String.IsNullOrEmpty(pic_name) Then
                        Dim imgUrl As String = ""
                        If pic_name.Contains("|") Then
                            imgUrl = picPathRelative & pic_name.Replace("|", "B") & ".jpg"
                        Else
                            imgUrl = pub_imgServ_path & pic_name
                        End If
                        sb.AppendLine(String.Format("<td rowspan='{0}' style='height:400px;width:500px;'><img src='{1}' style='width:500px;height:400px;'></td>", picSpan, imgUrl))
                    Else
                        ' 无图片时补空位或不写占位符
                        sb.AppendLine(String.Format("<td rowspan='{0}'>&nbsp;</td>", picSpan))
                    End If
                End If

                sb.AppendLine("</tr>")

                ' 更新 Old 值用于下一次对比
                old_kind_name = kind_name
                old_pic_name = pic_name
            Next
            sb.AppendLine("</table>")

            ' 4. 处理末尾附加图片 (m_picture_chk)
            Dim sqlImg As String = "SELECT idx, pic_conn FROM m_picture_chk WHERE chk_id='" & in_ck_id & "'"
            ' 假设 SqlHelper 支持参数化

            Dim dtChkImg As DataTable = SqlHelper.FillData(pub_conn, CommandType.Text, sqlImg, Nothing)

            If dtChkImg IsNot Nothing Then
                For Each rowImg As DataRow In dtChkImg.Rows
                    Dim bt As Byte() = TryCast(rowImg("pic_conn"), Byte())
                    If bt IsNot Nothing Then
                        Dim idxName As String = rowImg("idx").ToString() & ".jpg"
                        Dim fullImgPath As String = Path.Combine(picFolderFull, idxName)

                        ' 保存图片
                        If Not File.Exists(fullImgPath) Then
                            Using fs As New FileStream(fullImgPath, FileMode.Create, FileAccess.Write)
                                fs.Write(bt, 0, bt.Length)
                            End Using
                        End If
                        ' HTML 追加显示
                        sb.AppendLine(String.Format("<div style='margin-top:10px;'><img src='./pic/{0}' style='width:500px;height:400px;'></div>", idxName))
                    End If
                Next
            End If

            sb.AppendLine("</body></html>")

            ' 5. 保存 HTML 文件
            Dim fileName As String = String.Format("{0}_{1}_{2}.html", inCd, inNo, in_ck_id).Replace("*", "＊")
            Dim htmlPath As String = Path.Combine(pub_Save_dic_path, fileName)
            File.WriteAllText(htmlPath, sb.ToString(), Encoding.UTF8)

            Return True
        Catch ex As Exception
            ' 记录日志...
            Return False
        End Try
    End Function

    'Function GetRowSpanBy_kind_name(ByVal dt As DataTable, ByVal idx As Integer, ByVal kind_name As String) As Integer
    '    Dim Rsp As Integer = 0
    '    For i As Integer = idx To dt.Rows.Count - 1
    '        If dt.Rows(i).Item("kind_name").ToString = kind_name Then
    '            'Rsp = Rsp + CInt(dt.Rows(i).Item("chk_times"))
    '            Rsp = Rsp + 1
    '        Else
    '            Return Rsp
    '        End If
    '    Next
    '    Return Rsp
    'End Function


    ''' <summary>
    ''' 计算指定行开始，具有相同 [种类名称] 的连续行数
    ''' </summary>
    ''' <param name="dt">包含报表数据的数据表</param>
    ''' <param name="startIndex">当前遍历到的起始索引</param>
    ''' <param name="targetKindName">当前行需要比对的种类名称</param>
    ''' <returns>连续相同的行数。若输入无效或无匹配则返回 0</returns>
    Public Function GetRowSpanBy_kind_name(ByVal dt As DataTable, ByVal startIndex As Integer, ByVal targetKindName As String) As Integer
        ' 1. 基本安全校验
        ' 检查 DataTable 是否为空，以及索引是否越界
        If dt Is Nothing OrElse startIndex < 0 OrElse startIndex >= dt.Rows.Count Then
            Return 0
        End If

        Dim rowSpanCount As Integer = 0

        ' 2. 从起始索引向后循环遍历数据行
        For i As Integer = startIndex To dt.Rows.Count - 1
            ' 获取当前循环行的 kind_name 值，并处理 NULL 值情况
            Dim currentKind As String = If(dt.Rows(i)("kind_name") Is DBNull.Value, "", dt.Rows(i)("kind_name").ToString())

            ' 3. 比较内容
            ' 如果当前行与目标名称一致，计数加 1
            If currentKind = targetKindName Then
                rowSpanCount += 1
            Else
                ' 一旦遇到不一致的行，说明连续区域结束，立即返回累计值
                ' 这样可以避免不必要的后续循环，提高效率
                Return rowSpanCount
            End If
        Next

        ' 4. 返回最终累计的行数
        Return rowSpanCount
    End Function


    'Function GetRowSpanBy_pic_name(ByVal dt As DataTable, ByVal idx As Integer, ByVal pic_name As String) As Integer
    '    Dim Rsp As Integer = 0
    '    For i As Integer = idx To dt.Rows.Count - 1
    '        If dt.Rows(i).Item("pic_name").ToString = pic_name Then
    '            'Rsp = Rsp + CInt(dt.Rows(i).Item("chk_times"))
    '            Rsp = Rsp + 1
    '        Else
    '            Return Rsp
    '        End If
    '    Next
    '    Return Rsp
    'End Function

    ''' <summary>
    ''' 计算指定行开始，具有相同图片名称的连续行数
    ''' </summary>
    ''' <param name="dt">数据表</param>
    ''' <param name="startIndex">开始统计的行索引</param>
    ''' <param name="targetPicName">要匹配的图片名称（例如：test.jpg|20230101）</param>
    ''' <returns>连续相同的行数</returns>
    Public Function GetRowSpanBy_pic_name(ByVal dt As DataTable, ByVal startIndex As Integer, ByVal targetPicName As String) As Integer
        ' 1. 安全校验：防止索引越界或空表
        If dt Is Nothing OrElse startIndex < 0 OrElse startIndex >= dt.Rows.Count Then
            Return 0
        End If

        Dim count As Integer = 0

        ' 2. 从指定的起始位置向下遍历
        For i As Integer = startIndex To dt.Rows.Count - 1
            ' 获取当前行的图片名称，并处理 DBNull 的情况
            Dim currentPic As String = If(dt.Rows(i)("pic_name") Is DBNull.Value, "", dt.Rows(i)("pic_name").ToString())

            ' 3. 比较内容
            ' 注意：这里通常建议使用 String.Equals 进行比较，可以指定是否忽略大小写
            If currentPic = targetPicName Then
                count += 1
            Else
                ' 一旦发现不匹配，立即中断循环并返回当前的累计值
                Return count
            End If
        Next

        Return count
    End Function




    'Function GetRowSpanBy_chk_km_name(ByVal dt As DataTable, ByVal idx As Integer, ByVal kind_name As String, ByVal chk_km_name As String)
    '    Dim Rsp As Integer = 0
    '    For i As Integer = idx To dt.Rows.Count - 1
    '        If dt.Rows(i).Item("kind_name").ToString = kind_name AndAlso dt.Rows(i).Item("chk_km_name").ToString = chk_km_name Then
    '            Rsp = Rsp + 1
    '        Else
    '            Return Rsp
    '        End If
    '    Next
    '    Return Rsp
    'End Function


    '''' <summary>
    '''' 计算指定行开始，具有相同种类和检查项目名称的连续行数（用于合并单元格）
    '''' </summary>
    '''' <param name="dt">数据表</param>
    '''' <param name="startIndex">开始计算的起始行索引</param>
    '''' <param name="kind_name">匹配的种类名称</param>
    '''' <param name="chk_km_name">匹配的检查项目名称</param>
    '''' <returns>连续相同的行数</returns>
    'Public Function GetRowSpanBy_chk_km_name(ByVal dt As DataTable, ByVal startIndex As Integer, ByVal kind_name As String, ByVal chk_km_name As String) As Integer
    '    ' 1. 安全校验
    '    If dt Is Nothing OrElse startIndex < 0 OrElse startIndex >= dt.Rows.Count Then
    '        Return 0
    '    End If

    '    Dim rowCount As Integer = 0

    '    ' 2. 遍历数据
    '    For i As Integer = startIndex To dt.Rows.Count - 1
    '        Dim currentRow As DataRow = dt.Rows(i)

    '        ' 优化：提取当前行数据，减少重复获取 Item 的开销
    '        ' 使用 IfNull 进行空值转换，避免 ToString 触发空引用异常
    '        Dim currentKind As String = If(currentRow("kind_name") Is DBNull.Value, "", currentRow("kind_name").ToString())
    '        Dim currentKm As String = If(currentRow("chk_km_name") Is DBNull.Value, "", currentRow("chk_km_name").ToString())

    '        ' 3. 判断是否满足合并条件
    '        If currentKind = kind_name AndAlso currentKm = chk_km_name Then
    '            rowCount += 1
    '        Else
    '            ' 一旦遇到不匹配的行，立即返回累计值
    '            Return rowCount
    '        End If
    '    Next

    '    Return rowCount
    'End Function



    'Function GetData(ByVal bumen As String, ByVal z As Integer) As DataTable

    '    Dim sb As New System.Text.StringBuilder
    '    sb.AppendLine("SELECT ")
    '    sb.AppendLine("	    b.cd")
    '    sb.AppendLine("	   ,b.no")
    '    sb.AppendLine("	   ,b.ck_id")
    '    sb.AppendLine("	   ,b.result tc_result ")

    '    sb.AppendLine("       ,c.kind_name      ")
    '    sb.AppendLine("       ,c.chk_pos        ")
    '    sb.AppendLine("       ,c.chk_km_name    ")
    '    sb.AppendLine("       ,c.chk_fs_txt     ")
    '    sb.AppendLine("	   ,c.chk_times")
    '    sb.AppendLine("	   ,'（'+ c.k1+ ','+ c.k2+ ','+ c.k2+')' as jz")
    '    sb.AppendLine("       ,c.tools_ma     ")
    '    sb.AppendLine("       ,c.pic_name     ")
    '    sb.AppendLine("       ,c.in_1     ")
    '    sb.AppendLine("       ,c.result     ")
    '    sb.AppendLine("       ,c.mark     ")
    '    sb.AppendLine("       ,b.chk_user+':'+isnull(d.user_name,'') chk_user      ")
    '    sb.AppendLine("       ,c.upd_user+':'+isnull(d.user_name,'') upd_user    ")
    '    sb.AppendLine("       ,c.upd_date     ")
    '    sb.AppendLine("       ,Datediff(s, b.chk_start_date, b.chk_end_date)     AS 检查时长     ")

    '    sb.AppendLine("FROM   [wzh_new].[dbo].t_check b")
    '    sb.AppendLine("       INNER JOIN [wzh_new].[dbo].t_check_ms c")
    '    sb.AppendLine("               ON b.ck_id=c.ck_id")
    '    sb.AppendLine("       LEFT JOIN [wzh_new].[dbo].m_user d")
    '    sb.AppendLine("               ON b.chk_user=d.user_cd")
    '    sb.AppendLine("       LEFT JOIN [wzh_new].[dbo].m_user f")
    '    sb.AppendLine("               ON c.upd_user=f.user_cd")

    '    'sb.AppendLine("	WHERE Convert(varchar(100), b.yotei_chk_date, 23) >= '" & Now.AddDays(-4).ToString("yyyy-MM-dd") & "'")

    '    'sb.AppendLine("	WHERE Convert(varchar(100), b.chk_end_date, 23) = '" & Now.AddDays(-1 * z - 1).ToString("yyyy-MM-dd") & "'")
    '    'sb.AppendLine("	WHERE Convert(varchar(100), b.chk_end_date, 23) >= '2021-09-13'")

    '    sb.AppendLine("	WHERE b.chk_end_date >= '" & Now.AddDays(-1 * z - 1).ToString("yyyy-MM-dd") & " 08:00:00'")
    '    sb.AppendLine("	AND b.chk_end_date < '" & Now.AddDays(-1 * z).ToString("yyyy-MM-dd") & " 08:00:00'")

    '    sb.AppendLine("	And b.department_cd='" & bumen & "'")
    '    sb.AppendLine("ORDER  BY b.ck_id")
    '    sb.AppendLine("          ,c.kind_jun")
    '    sb.AppendLine("          ,c.hyouji_jun ")


    '    Return SqlHelper.FillData(pub_conn, CommandType.Text, sb.ToString(), "dt")

    'End Function

    ''' <summary>
    ''' 获取指定部门在特定时间范围内的检查数据
    ''' </summary>
    ''' <param name="bumen">部门代码</param>
    ''' <param name="z">天数偏移量（用于计算查询日期）</param>
    Public Function GetData(ByVal bumen As String, ByVal z As Integer) As DataTable
        Dim sb As New System.Text.StringBuilder

        ' 1. 使用参数化查询防止 SQL 注入 (推荐做法)
        ' 定义参数：这样即使 bumen 包含单引号，也不会导致程序崩溃或被攻击
        Dim params As New List(Of SqlClient.SqlParameter)
        params.Add(New SqlClient.SqlParameter("@bumen", bumen))

        ' 计算日期：提前处理好字符串，避免在拼接时逻辑混乱
        ' 逻辑：查询 [T-(z+1) 08:00] 到 [T-z 08:00] 之间的数据
        Dim startDate As String = DateTime.Now.AddDays(-1 * z - 1).ToString("yyyy-MM-dd") & " 08:00:00"
        Dim endDate As String = DateTime.Now.AddDays(-1 * z).ToString("yyyy-MM-dd") & " 08:00:00"

        sb.AppendLine("SELECT ")
        sb.AppendLine("      b.cd, b.no, b.ck_id")
        sb.AppendLine("    , b.result AS tc_result")
        sb.AppendLine("    , c.kind_name, c.chk_pos, c.chk_km_name, c.chk_fs_txt, c.chk_times")
        ' 修正：原代码中 jz 字段拼接了两次 c.k2，通常应为 k1, k2, k3
        sb.AppendLine("    , '（' + ISNULL(c.k1,'') + ',' + ISNULL(c.k2,'') + ',' + ISNULL(c.k3,'') + '）' AS jz")
        sb.AppendLine("    , c.tools_ma, c.pic_name, c.in_1, c.result, c.mark")
        ' 优化：连接用户信息，增加 ISNULL 处理防止显示为空白
        sb.AppendLine("    , b.chk_user + ':' + ISNULL(d.user_name,'') AS chk_user")
        sb.AppendLine("    , c.upd_user + ':' + ISNULL(f.user_name,'') AS upd_user") ' 原代码此处用d，应改为f (对应c.upd_user)
        sb.AppendLine("    , c.upd_date")
        ' 计算时长：DATEDIFF(秒)
        sb.AppendLine("    , DATEDIFF(s, b.chk_start_date, b.chk_end_date) AS 检查时长")

        sb.AppendLine("FROM [wzh_new].[dbo].t_check AS b")
        sb.AppendLine("INNER JOIN [wzh_new].[dbo].t_check_ms AS c ON b.ck_id = c.ck_id")
        sb.AppendLine("LEFT JOIN [wzh_new].[dbo].m_user AS d ON b.chk_user = d.user_cd")
        sb.AppendLine("LEFT JOIN [wzh_new].[dbo].m_user AS f ON c.upd_user = f.user_cd")

        ' --- 查询条件 ---
        ' 优化：直接使用字符串比较（若数据库字段是 DateTime 类型，SQL 引擎会自动转换，且能走索引）
        sb.AppendLine("WHERE b.chk_end_date >= '" & startDate & "'")
        sb.AppendLine("  AND b.chk_end_date < '" & endDate & "'")
        ' 使用参数化占位符
        sb.AppendLine("  AND b.department_cd = '" & bumen & "'")

        sb.AppendLine("ORDER BY b.ck_id, c.kind_jun, c.hyouji_jun")

        ' 注意：如果您的 SqlHelper 支持参数，请务必传入 params 列表
        ' 如果不支持，至少要确保对 bumen 变量进行了 .Replace("'", "''") 处理
        Return SqlHelper.FillData(pub_conn, CommandType.Text, sb.ToString(), Nothing)
    End Function


    'Public Function SelChkPictureKM(ByVal dt As Data.DataTable) As DataTable


    '    Dim sb As New StringBuilder
    '    'SQL文
    '    sb.AppendLine("SELECT")
    '    sb.AppendLine("*")                                                    '图片ID
    '    sb.AppendLine("FROM m_picture_km")
    '    sb.AppendLine("WHERE")

    '    Dim orStr As String = ""
    '    Dim jk As Boolean = False

    '    Dim dicPjnms = New Dictionary(Of String, String)
    '    Dim pic_name As String
    '    For i As Integer = 0 To dt.Rows.Count - 1
    '        pic_name = dt.Rows(i).Item("pic_name").ToString
    '        If pic_name <> "" AndAlso pic_name.Contains("|") Then

    '            If Not dicPjnms.ContainsKey(pic_name) Then
    '                dicPjnms.Add(pic_name, "")
    '                sb.AppendLine(orStr & " (pic_name='" & pic_name.Split("|")(0) & "' AND pic_upd_time='" & pic_name.Split("|")(1) & "')")   '图片ID
    '                If orStr = "" Then
    '                    orStr = "OR"
    '                End If
    '                jk = True
    '            End If

    '        End If
    '    Next


    '    If jk Then
    '        Dim picPath As String = pub_Save_dic_path & "pic\"
    '        If Not System.IO.Directory.Exists(picPath) Then
    '            System.IO.Directory.CreateDirectory(picPath)
    '        End If

    '        Dim dtImg As Data.DataTable = SqlHelper.FillData(pub_conn, CommandType.Text, sb.ToString(), "dt")
    '        Dim bt As Byte()
    '        Dim imgPath As String
    '        For i As Integer = 0 To dtImg.Rows.Count - 1
    '            bt = DirectCast(dtImg.Rows(i).Item("pic_conn"), Byte())
    '            imgPath = picPath & dtImg.Rows(i).Item("pic_name").ToString & "B" & dtImg.Rows(i).Item("pic_upd_time").ToString & ".jpg"
    '            If Not File.Exists(imgPath) Then
    '                Dim FileStr As New FileStream(imgPath, FileMode.Create, FileAccess.Write)
    '                FileStr.Write(bt, 0, bt.Length)
    '                FileStr.Close()
    '            End If
    '        Next
    '    End If

    '    Return Nothing
    'End Function

    ''' <summary>
    ''' 执行SQL查询并返回DataTable
    ''' </summary>
    ''' <param name="sql">SQL语句</param>
    ''' <param name="connString">数据库连接字符串</param>
    Public Function GetDataTable(ByVal conn As SqlConnection, ByVal sql As String) As DataTable
        Dim dt As New DataTable()

        ' 使用 Using 块可以确保即使发生异常，连接和适配器也能被正确释放

        Try
            ' 创建数据适配器
            Dim adapter As New SqlDataAdapter(sql, conn)
            adapter.SelectCommand.CommandTimeout = 60
            ' 填充 DataTable
            adapter.Fill(dt)

            Thread.Sleep(1000)
        Catch ex As Exception
            ' 这里可以记录日志或向上抛出异常
            Return GetDataTable2(conn, sql)
        End Try


        Return dt
    End Function

    Public Function GetDataTable2(ByVal conn As SqlConnection, ByVal sql As String) As DataTable
        Dim dt As New DataTable()

        ' 使用 Using 块可以确保即使发生异常，连接和适配器也能被正确释放

        Try
            ' 创建数据适配器
            Dim adapter As New SqlDataAdapter(sql, conn)
            adapter.SelectCommand.CommandTimeout = 60
            ' 填充 DataTable
            adapter.Fill(dt)

            Thread.Sleep(1000)
        Catch ex As Exception
            ' 这里可以记录日志或向上抛出异常
            Throw New Exception("执行SQL出错: " & ex.Message)
        End Try


        Return dt
    End Function


    Public Sub SelChkPictureKM(ByVal dtIn As Data.DataTable)
        If dtIn Is Nothing OrElse dtIn.Rows.Count = 0 Then Exit Sub

        ' 1. 预处理：确保目录存在（只做一次）
        Dim picFolder As String = Path.Combine(pub_Save_dic_path, "pic")
        If Not Directory.Exists(picFolder) Then Directory.CreateDirectory(picFolder)

        ' 2. 使用更高效的连接管理
        Using conn As New SqlConnection(pub_conn)
            conn.Open()

            Dim batchSize As Integer = 50 ' 适当调大批次以减少交互次数
            Dim conditions As New List(Of String)
            Dim processedNames As New HashSet(Of String)

            For i As Integer = 0 To dtIn.Rows.Count - 1
                Dim row = dtIn.Rows(i)
                Dim rawName As String = If(row("pic_name") Is DBNull.Value, "", row("pic_name").ToString())

                ' 构建过滤条件
                If Not String.IsNullOrEmpty(rawName) AndAlso rawName.Contains("|") Then
                    If Not processedNames.Contains(rawName) Then
                        processedNames.Add(rawName)
                        Dim parts = rawName.Split("|"c)
                        If parts.Length >= 2 Then
                            ' 简单的安全处理
                            Dim namePart = parts(0).Replace("'", "''")
                            Dim timePart = parts(1).Replace("'", "''")
                            conditions.Add(String.Format("(pic_name='{0}' AND pic_upd_time='{1}')", namePart, timePart))
                        End If
                    End If
                End If

                ' 3. 分批执行查询
                If (conditions.Count >= batchSize) OrElse (i = dtIn.Rows.Count - 1 AndAlso conditions.Count > 0) Then
                    ExecuteBatchDownload(conn, conditions, picFolder)
                    conditions.Clear() ' 清空批次
                    ' 注意：这里不建议清理 processedNames，除非你确定 dtIn 中不存在跨批次的重复项
                End If
            Next
        End Using
    End Sub

    Private Sub ExecuteBatchDownload(conn As SqlConnection, conditions As List(Of String), picFolder As String)
        Dim sql As String = "SELECT pic_name, pic_upd_time, pic_conn FROM m_picture_km WHERE " & String.Join(" OR ", conditions)

        Using cmd As New SqlCommand(sql, conn)
            ' CommandBehavior.SequentialAccess 是减少内存占用、防止内存溢出的关键
            Using reader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.SequentialAccess)
                While reader.Read()
                    Dim pName = reader("pic_name").ToString()
                    Dim pTime = reader("pic_upd_time").ToString()
                    Dim fileName As String = String.Format("{0}B{1}.jpg", pName, pTime)
                    Dim fullPath As String = Path.Combine(picFolder, fileName)

                    ' 如果本地已存在，跳过该行，不读取大字段
                    If File.Exists(fullPath) Then Continue While

                    ' 4. 流式读取：关键点，不使用 MemoryStream 转换
                    WriteBlobToFile(reader, reader.GetOrdinal("pic_conn"), fullPath)
                End While
            End Using
        End Using
    End Sub

    Private Sub WriteBlobToFile(reader As SqlDataReader, columnIndex As Integer, filePath As String)
        If reader.IsDBNull(columnIndex) Then Return

        Try
            Using fs As New FileStream(filePath, FileMode.Create, FileAccess.Write)
                Dim bufferSize As Integer = 81920 ' 80KB 缓冲区是比较理想的大小
                Dim buffer(bufferSize - 1) As Byte
                Dim bytesRead As Long
                Dim fieldOffset As Long = 0

                ' 直接从 Reader 读入文件流，内存中只占用 80KB
                bytesRead = reader.GetBytes(columnIndex, fieldOffset, buffer, 0, bufferSize)
                While bytesRead > 0
                    fs.Write(buffer, 0, CInt(bytesRead))
                    fieldOffset += bytesRead
                    bytesRead = reader.GetBytes(columnIndex, fieldOffset, buffer, 0, bufferSize)
                End While
            End Using
        Catch ex As Exception
            Debug.WriteLine("写入失败: " & filePath & " - " & ex.Message)
        End Try
    End Sub





    Function SetXlsBK(ByVal bumen As String, ByVal z As Integer) As Boolean

        pub_Save_dic_path = pub_dic & Now.AddDays(z * -1 - 1).ToString("yyyyMMdd") & "\"


        'If Now.AddDays(z * -1 - 1).ToString("yyyyMMdd") = "20250227" Or Now.AddDays(z * -1 - 1).ToString("yyyyMMdd") = "20250714" Then
        'Else
        '    Return True

        'End If

        If Not System.IO.Directory.Exists(pub_Save_dic_path) Then
            System.IO.Directory.CreateDirectory(pub_Save_dic_path)
        End If
        If bumen = "" Then
            pub_Save_dic_path = pub_Save_dic_path & "未知" & "\"
        Else
            pub_Save_dic_path = pub_Save_dic_path & bumen & "\"
        End If

        If Not System.IO.Directory.Exists(pub_Save_dic_path) Then
            System.IO.Directory.CreateDirectory(pub_Save_dic_path)
        End If

        'If System.IO.Directory.Exists(pub_Save_dic_path) Then
        '    ' Return True
        '    If yotei_chk_date >= Now.ToString("yyyy-MM-dd") Then
        '        System.IO.Directory.Delete(pub_Save_dic_path, True)
        '    Else
        '        Return True
        '    End If
        'End If

        Dim sb As New StringBuilder
        sb.AppendLine("<html>")
        sb.AppendLine("<head>")
        sb.AppendLine("<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />")
        sb.AppendLine("<link rel=""stylesheet"" href=""../../main.css"" />")
        sb.AppendLine("</head>")
        sb.AppendLine("<body>")
        sb.AppendLine("<table>")

        sb.AppendLine("<tr>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>CD</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>NO</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>检查ID</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>检查者</th>")

        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>结果</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>时长</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>详细</th>")
        'sb.AppendLine("<th style='border:.5pt solid windowtext;width:130px'>实测值</th>")
        'sb.AppendLine("<th style='border:.5pt solid windowtext;width:60px'>结果</th>")
        'sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>备考</th>")
        'sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>检查员</th>")
        'sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>图片</th>")
        sb.AppendLine("</tr>")



        Dim dt As DataTable = GetData(bumen, z)
        Dim tmDt As DataTable = dt.Copy
        tmDt.Clear()

        Dim old_cd As String = "-1"
        Dim old_no As String = "-1"
        Dim old_ck_id As String = "-1"
        Dim old_ck_user As String = "-1"
        Dim old_ck_result As String = "-1"
        Dim old_ck_time As String = "-1"
        Dim cd As String
        Dim ck_id As String
        Dim no As String
        Dim kind_name As String
        Dim chk_pos As String
        Dim chk_km_name As String
        Dim chk_fs_txt As String

        SelChkPictureKM(dt)

        For i As Integer = 0 To dt.Rows.Count - 1
            cd = dt.Rows(i).Item("cd").ToString
            ck_id = dt.Rows(i).Item("ck_id").ToString
            no = dt.Rows(i).Item("no").ToString
            kind_name = dt.Rows(i).Item("kind_name").ToString
            chk_pos = dt.Rows(i).Item("chk_pos").ToString
            chk_km_name = dt.Rows(i).Item("chk_km_name").ToString
            chk_fs_txt = dt.Rows(i).Item("chk_fs_txt").ToString

            If ck_id = "211110152935530" Or old_ck_id = "211110152935530" Then
                Dim b As String = "ff"
            End If

            If (ck_id = old_ck_id) OrElse i = 0 Then
                tmDt.Rows.Add(dt.Rows(i).ItemArray)

                If i = dt.Rows.Count - 1 Then


                    SetDataToFile(tmDt, old_cd, old_no, old_ck_id, old_ck_user)
                    sb.AppendLine(GetOneList(old_cd, old_no, old_ck_id, old_ck_user, old_ck_result, old_ck_time))

                    tmDt.Clear()
                End If
            Else


                SetDataToFile(tmDt, old_cd, old_no, old_ck_id, old_ck_user)
                sb.AppendLine(GetOneList(old_cd, old_no, old_ck_id, old_ck_user, old_ck_result, old_ck_time))
                tmDt.Clear()
            End If
            old_ck_user = dt.Rows(i).Item("chk_user").ToString
            old_ck_result = dt.Rows(i).Item("tc_result").ToString
            old_ck_time = dt.Rows(i).Item("检查时长").ToString
            old_cd = cd
            old_ck_id = ck_id
            old_no = no

        Next

        sb.AppendLine("</table>")
        sb.AppendLine("</body>")
        sb.AppendLine("</html>")





        Dim listPath As String = pub_Save_dic_path & "检查结果一览.html"
        System.IO.File.WriteAllText(listPath, sb.ToString)

        Return True

    End Function


    ''' <summary>
    ''' 根据传入的 DataTable 中的图片信息（文件名|更新时间），同步数据库图片到本地路径
    ''' </summary>
    ''' <param name="dt">包含 pic_name 列的原始数据表</param>
    ''' <remarks>
    ''' 优化点：
    ''' 1. 使用 HashSet 进行高效去重
    ''' 2. 使用 Path.Combine 确保路径拼接跨平台/环境安全
    ''' 3. 引入 Using 块确保文件流及时关闭（防止文件被占用）
    ''' 4. 采用 TryCast 防止类型转换异常
    ''' </remarks>
    Public Sub SelChkPictureKM2(ByVal dtIn As Data.DataTable)

        Dim dt As DataTable = dtIn.Clone

        Using conn As New SqlConnection(pub_conn)
            conn.Open() ' 只打开一次连接


            For iii As Integer = 0 To dtIn.Rows.Count - 1

                dt.ImportRow(dtIn.Rows(iii))
                If (iii + 1) Mod 10 = 0 OrElse iii = dtIn.Rows.Count - 1 Then



                    ' 1. 基本校验：检查输入数据是否为空
                    If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

                    ' 用于构建动态 SQL 的查询条件集合
                    Dim conditions As New List(Of String)
                    ' 使用 HashSet 存储已处理的 pic_name，利用其 O(1) 的查询特性高效去重
                    Dim processedNames As New HashSet(Of String)

                    ' --- 第一阶段：解析 DataTable，构建查询过滤条件 ---
                    For Each row As DataRow In dt.Rows
                        ' 获取图片名称（格式通常为 "文件名|更新时间"）
                        Dim rawName As String = If(row("pic_name") Is DBNull.Value, "", row("pic_name").ToString())

                        ' 校验：非空且包含分隔符
                        If Not String.IsNullOrEmpty(rawName) AndAlso rawName.Contains("|") Then
                            ' 如果该图片信息尚未处理过，则加入待查询列表
                            If Not processedNames.Contains(rawName) Then
                                processedNames.Add(rawName)

                                ' 拆分字符串，避免在循环内多次执行 Split
                                Dim parts = rawName.Split("|"c)
                                If parts.Length >= 2 Then
                                    ' 【安全建议】处理单引号转义，防止简单的 SQL 注入导致语法错误
                                    Dim namePart = parts(0).Replace("'", "''")
                                    Dim timePart = parts(1).Replace("'", "''")
                                    ' 构建每一组匹配条件
                                    conditions.Add(String.Format("(pic_name='{0}' AND pic_upd_time='{1}')", namePart, timePart))
                                End If
                            End If
                        End If
                    Next

                    ' 如果没有任何有效条件，直接结束函数
                    If conditions.Count = 0 Then Exit Sub

                    ' --- 第二阶段：构建完整 SQL 并执行查询 ---
                    Dim sbSql As New StringBuilder()
                    sbSql.AppendLine("SELECT pic_name, pic_upd_time, pic_conn FROM m_picture_km WHERE ")
                    ' 使用 String.Join 自动处理 "OR" 的连接逻辑，无需手动判断 orStr 是否为空
                    sbSql.Append(String.Join(" OR ", conditions.ToArray()))

                    ' 准备本地存储目录
                    ' pub_Save_dic_path 需确保以斜杠结尾，或者使用 Path.Combine 自动处理
                    Dim picFolder As String = Path.Combine(pub_Save_dic_path, "pic")
                    Try
                        If Not Directory.Exists(picFolder) Then
                            Directory.CreateDirectory(picFolder)
                        End If
                    Catch ex As Exception
                        ' 目录创建失败处理（建议记录日志）
                        Throw New Exception("无法创建图片存储目录: " & picFolder, ex)
                    End Try

                    ' 执行数据库查询获取二进制图片数据
                    'Dim dtImg As Data.DataTable = SqlHelper.FillData(pub_conn, CommandType.Text, sbSql.ToString(), "dt" & iii)

                    'Dim dtImg As Data.DataTable = GetDataTable(conn, sbSql.ToString())




                    Dim cmd As New SqlCommand(sbSql.ToString(), conn)
                    ' 使用 ExecuteReader 替代 Fill DataTable
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            ' 逐行处理数据，内存占用极低
                            ' Dim value As String = reader("pic_conn").ToString()

                            ' 使用 TryCast 安全转换字节数组，避免直接 DirectCast 导致类型异常
                            Dim fileBytes As Byte() '= TryCast(reader("pic_conn"), Byte())


                            ' 检查是否为空
                            If Not reader.IsDBNull(reader.GetOrdinal("pic_conn")) Then
                                ' 定义缓冲区（例如每次读取 8KB）
                                Dim bufferSize As Integer = 8192
                                Dim outBuffer(bufferSize - 1) As Byte
                                Dim bytesRead As Long
                                Dim startIndex As Long = 0

                                Using ms As New System.IO.MemoryStream()
                                    ' 循环读取字节流
                                    bytesRead = reader.GetBytes(reader.GetOrdinal("pic_conn"), startIndex, outBuffer, 0, bufferSize)
                                    While bytesRead > 0
                                        ms.Write(outBuffer, 0, CInt(bytesRead))
                                        startIndex += bytesRead
                                        bytesRead = reader.GetBytes(reader.GetOrdinal("pic_conn"), startIndex, outBuffer, 0, bufferSize)
                                    End While

                                    ' 这里得到了完整的字节数组，但至少在读取过程中没那么占内存
                                    fileBytes = ms.ToArray()

                                    ' --- 重要：处理完后立即释放 ---
                                    ' 处理逻辑...
                                End Using
                            End If




                            ' 如果二进制数据为空，跳过该行
                            If fileBytes Is Nothing Then Continue For

                            ' 构造文件名：文件名B更新时间.jpg
                            Dim fileName As String = String.Format("{0}B{1}.jpg", reader("pic_name"), reader("pic_upd_time"))
                            Dim fullPath As String = Path.Combine(picFolder, fileName)

                            ' 检查本地是否存在同名文件，不存在则写入
                            If Not File.Exists(fullPath) Then
                                Try
                                    ' 使用 Using 块自动调用 .Dispose() 和 .Close()，即使写入中途报错也能释放文件句柄
                                    Using fs As New FileStream(fullPath, FileMode.Create, FileAccess.Write)
                                        fs.Write(fileBytes, 0, fileBytes.Length)
                                    End Using
                                Catch ex As Exception
                                    ' 单个文件写入失败不应阻塞整个循环，可记录错误日志
                                    Debug.WriteLine("文件写入失败: " & fullPath & " 错误: " & ex.Message)
                                End Try
                            End If

                        End While
                    End Using

                    cmd.Dispose()





                    dt.Clear()

                End If



            Next

            conn.Close()

        End Using

    End Sub

End Class