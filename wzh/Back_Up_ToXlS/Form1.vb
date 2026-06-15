Imports Microsoft.VisualBasic
Imports System.Data
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.Text
Imports System
Imports System.Configuration

Public Class Form1

    Public pub_dic As String = ConfigurationManager.AppSettings("SavePath").ToString()

    Public pub_Save_dic_path As String

    Public SqlHelper As New SqlHelper


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'fff
        'For i As Integer = -10 To 10

        '    SetXls()

        'Next

        SetXls()
        Me.Close()

    End Sub


    Function SetXls() As Boolean

        pub_Save_dic_path = pub_dic & "\"


        'If System.IO.Directory.Exists(pub_Save_dic_path) Then
        '    ' Return True
        '    If yotei_chk_date >= Now.ToString("yyyy-MM-dd") Then
        '        System.IO.Directory.Delete(pub_Save_dic_path, True)
        '    Else
        '        Return True
        '    End If
        'End If


        Dim dt As DataTable = GetData()
        Dim tmDt As DataTable = dt.Copy
        tmDt.Clear()

        Dim old_cd As String = "-1"
        Dim old_no As String = "-1"
        Dim old_sys_id As String = "-1"

        Dim cd As String
        Dim sys_id As String
        'Dim shunwei As String
        Dim kind_name As String
        Dim chk_pos As String
        Dim chk_km_name As String
        Dim chk_fs_txt As String

        For i As Integer = 0 To dt.Rows.Count - 1
            cd = dt.Rows(i).Item("cd").ToString
            sys_id = dt.Rows(i).Item("sys_id").ToString
            'shunwei = dt.Rows(i).Item("shunwei").ToString
            kind_name = dt.Rows(i).Item("kind_name").ToString
            chk_pos = dt.Rows(i).Item("chk_pos").ToString
            chk_km_name = dt.Rows(i).Item("chk_km_name").ToString
            chk_fs_txt = dt.Rows(i).Item("chk_fs_txt").ToString


            If (old_cd = cd AndAlso sys_id = old_sys_id) OrElse i = 0 Then
                tmDt.Rows.Add(dt.Rows(i).ItemArray)
            Else
                SetDataToFile(tmDt, old_cd, old_sys_id)
                tmDt.Clear()
            End If

            old_cd = cd
            old_sys_id = sys_id

        Next
    End Function

    Function SetDataToFile(ByVal dt As DataTable, ByVal inCd As String, ByVal in_sys_id As String)
        Dim cd As String
        Dim sys_id As String
        'Dim shunwei As String
        Dim kind_name As String
        Dim chk_pos As String
        Dim chk_km_name As String
        Dim chk_fs_txt As String
        Dim chk_times As Integer
        Dim tools_ma As String
        Dim pic_name As String


        Dim old_pic_name As String = "-1"
        Dim old_cd As String = "-1"
        Dim old_sys_id As String = "-1"
        'Dim old_shunwei As String = "-1"
        Dim old_kind_name As String = "-1"
        Dim old_chk_km_name As String = "-1"

        Dim sb As New StringBuilder
        sb.AppendLine("<table>")
        sb.AppendLine("<tr>")
        sb.AppendLine("<th style='' colspan='3'>" & inCd & "</th>")
        sb.AppendLine("<th style=''>" & in_sys_id & "</th>")
        sb.AppendLine("<th style=''></th>")
        sb.AppendLine("<th style=''></th>")
        sb.AppendLine("<th style=''></th>")

        sb.AppendLine("</tr>")


        sb.AppendLine("<tr>")

        sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>No</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>种类</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:100px'>位置</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:200px'>项目名</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:200px'>方法</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:130px'>实测值</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>备考</th>")
        sb.AppendLine("<th style='border:.5pt solid windowtext;width:30px'>图片</th>")
        sb.AppendLine("</tr>")

        Dim idx As Integer = 0
        Dim span, picSpan As Integer


        For i As Integer = 0 To dt.Rows.Count - 1


            cd = dt.Rows(i).Item("cd").ToString
            sys_id = dt.Rows(i).Item("sys_id").ToString
            ' shunwei = dt.Rows(i).Item("shunwei").ToString
            kind_name = dt.Rows(i).Item("kind_name").ToString
            chk_pos = dt.Rows(i).Item("chk_pos").ToString
            chk_km_name = dt.Rows(i).Item("chk_km_name").ToString
            chk_fs_txt = dt.Rows(i).Item("chk_fs_txt").ToString
            chk_times = CInt(dt.Rows(i).Item("chk_times").ToString)
            tools_ma = dt.Rows(i).Item("tools_ma").ToString
            pic_name = dt.Rows(i).Item("pic_name").ToString
            span = GetRowSpanBy_kind_name(dt, i, kind_name)
            picSpan = GetRowSpanBy_pic_name(dt, i, pic_name)
            idx = idx + 1
            'Dim sb2 As New StringBuilder
            sb.AppendLine("<tr>")
            sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & idx & "</td>")

            If kind_name <> old_kind_name Then
                sb.AppendLine("<td rowspan='" & span & "'  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("kind_name").ToString & "</td>")
            End If
            sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & chk_pos & "</td>")
            sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & chk_km_name & "</td>")
            sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & chk_fs_txt & "<br>" & dt.Rows(i).Item("jz").ToString & "</td>")
            sb.AppendLine("<td  style='border:.5pt solid windowtext;'>&nbsp;</td>")
            sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & tools_ma & "</td>")


            If old_pic_name <> pic_name Then
                If pic_name <> "" Then

                    sb.AppendLine("<td rowspan='" & picSpan & "'  style='border:.5pt solid windowtext;height:400px;width:500px;'>" & "<img src=""http://ot0940/wzh2021/Image/ChkImgs/" & pic_name & """ style=""border-width:0px;width: 500px;height:400px;"">" & "</td>")
                End If
            End If


            sb.AppendLine("</tr>")



            old_kind_name = kind_name
            old_chk_km_name = chk_km_name


            old_pic_name = pic_name
            ' sb.Append(sb2.ToString)
            For j As Integer = 2 To chk_times
                sb.AppendLine("<tr>")
                sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & idx & "</td>")
                sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_pos").ToString & "</td>")
                sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_km_name").ToString & "</td>")
                sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_fs_txt").ToString & "<br>" & dt.Rows(i).Item("jz").ToString & "</td>")
                sb.AppendLine("<td  style='border:.5pt solid windowtext;'>&nbsp;</td>")
                sb.AppendLine("<td  style='border:.5pt solid windowtext;'>&nbsp;</td>")
                sb.AppendLine("</tr>")
            Next
            'sb2.Length = 0
        Next
        sb.AppendLine("</table>")

        If Not System.IO.Directory.Exists(pub_Save_dic_path) Then
            System.IO.Directory.CreateDirectory(pub_Save_dic_path)

        End If

        System.IO.File.WriteAllText(pub_Save_dic_path & inCd & "_" & in_sys_id & ".xls", sb.ToString)

        Return True

    End Function








    Function GetRowSpanBy_kind_name(ByVal dt As DataTable, ByVal idx As Integer, ByVal kind_name As String) As Integer
        Dim Rsp As Integer = 0
        For i As Integer = idx To dt.Rows.Count - 1
            If dt.Rows(i).Item("kind_name").ToString = kind_name Then
                Rsp = Rsp + CInt(dt.Rows(i).Item("chk_times"))
            Else
                Return Rsp
            End If
        Next
        Return Rsp
    End Function


    Function GetRowSpanBy_pic_name(ByVal dt As DataTable, ByVal idx As Integer, ByVal pic_name As String) As Integer
        Dim Rsp As Integer = 0
        For i As Integer = idx To dt.Rows.Count - 1
            If dt.Rows(i).Item("pic_name").ToString = pic_name Then
                Rsp = Rsp + CInt(dt.Rows(i).Item("chk_times"))
            Else
                Return Rsp
            End If
        Next
        Return Rsp
    End Function





    Function GetRowSpanBy_chk_km_name(ByVal dt As DataTable, ByVal idx As Integer, ByVal kind_name As String, ByVal chk_km_name As String)
        Dim Rsp As Integer = 0
        For i As Integer = idx To dt.Rows.Count - 1
            If dt.Rows(i).Item("kind_name").ToString = kind_name AndAlso dt.Rows(i).Item("chk_km_name").ToString = chk_km_name Then
                Rsp = Rsp + 1
            Else
                Return Rsp
            End If
        Next
        Return Rsp
    End Function



    Function GetData() As DataTable

        Dim sb As New System.Text.StringBuilder
        sb.AppendLine("SELECT ")
        sb.AppendLine("	    b.cd")
        sb.AppendLine("	   ,b.sys_id")
        sb.AppendLine("       ,c.kind_name      ")
        sb.AppendLine("       ,c.chk_pos        ")
        sb.AppendLine("       ,c.chk_km_name    ")
        sb.AppendLine("       ,c.chk_fs_txt     ")
        sb.AppendLine("	   ,c.chk_times")
        sb.AppendLine("	   ,'（'+ c.k1+ ','+ c.k2+ ','+ c.k2+')' as jz")
        sb.AppendLine("       ,c.tools_ma     ")
        sb.AppendLine("       ,c.pic_name     ")

        sb.AppendLine("FROM   [wzh_new].[dbo].m_sys_join b")
        sb.AppendLine("       INNER JOIN [wzh_new].[dbo].m_km_template c")
        sb.AppendLine("               ON ( b.cd IS NULL")
        sb.AppendLine("                     OR b.sys_id = c.sys_name )")
        sb.AppendLine("                  AND ( ( c.f1 LIKE '%' + Substring(b.cd, 1, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f1) = '' )")
        sb.AppendLine("                         OR ( c.f1 = Substring(b.cd, 1, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f2 LIKE '%' + Substring(b.cd, 2, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f2) = '' )")
        sb.AppendLine("                         OR ( c.f2 = Substring(b.cd, 2, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f3 LIKE '%' + Substring(b.cd, 3, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f3) = '' )")
        sb.AppendLine("                         OR ( c.f3 = Substring(b.cd, 3, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f4 LIKE '%' + Substring(b.cd, 4, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f4) = '' )")
        sb.AppendLine("                         OR ( c.f4 = Substring(b.cd, 4, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f5 LIKE '%' + Substring(b.cd, 5, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f5) = '' )")
        sb.AppendLine("                         OR ( c.f5 = Substring(b.cd, 5, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f6 LIKE '%' + Substring(b.cd, 6, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f6) = '' )")
        sb.AppendLine("                         OR ( c.f6 = Substring(b.cd, 6, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f7 LIKE '%' + Substring(b.cd, 7, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f7) = '' )")
        sb.AppendLine("                         OR ( c.f7 = Substring(b.cd, 7, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f8 LIKE '%' + Substring(b.cd, 8, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f8) = '' )")
        sb.AppendLine("                         OR ( c.f8 = Substring(b.cd, 8, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f9 LIKE '%' + Substring(b.cd, 9, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f9) = '' )")
        sb.AppendLine("                         OR ( c.f9 = Substring(b.cd, 9, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f10 LIKE '%' + Substring(b.cd, 10, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f10) = '' )")
        sb.AppendLine("                         OR ( c.f10 = Substring(b.cd, 10, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f11 LIKE '%' + Substring(b.cd, 11, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f11) = '' )")
        sb.AppendLine("                         OR ( c.f11 = Substring(b.cd, 11, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f12 LIKE '%' + Substring(b.cd, 12, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f12) = '' )")
        sb.AppendLine("                         OR ( c.f12 = Substring(b.cd, 12, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f13 LIKE '%' + Substring(b.cd, 13, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f13) = '' )")
        sb.AppendLine("                         OR ( c.f13 = Substring(b.cd, 13, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f14 LIKE '%' + Substring(b.cd, 14, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f14) = '' )")
        sb.AppendLine("                         OR ( c.f14 = Substring(b.cd, 14, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f15 LIKE '%' + Substring(b.cd, 15, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f15) = '' )")
        sb.AppendLine("                         OR ( c.f15 = Substring(b.cd, 15, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f16 LIKE '%' + Substring(b.cd, 16, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f16) = '' )")
        sb.AppendLine("                         OR ( c.f16 = Substring(b.cd, 16, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f17 LIKE '%' + Substring(b.cd, 17, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f17) = '' )")
        sb.AppendLine("                         OR ( c.f17 = Substring(b.cd, 17, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f18 LIKE '%' + Substring(b.cd, 18, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f18) = '' )")
        sb.AppendLine("                         OR ( c.f18 = Substring(b.cd, 18, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f19 LIKE '%' + Substring(b.cd, 19, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f19) = '' )")
        sb.AppendLine("                         OR ( c.f19 = Substring(b.cd, 19, 1) ) )")
        sb.AppendLine("                  AND ( ( c.f20 LIKE '%' + Substring(b.cd, 20, 1 ) + '%'")
        sb.AppendLine("                           OR Ltrim(c.f20) = '' )")
        sb.AppendLine("                         OR ( c.f20 = Substring(b.cd, 20, 1) ) )")
        sb.AppendLine("")
        sb.AppendLine("ORDER  BY b.cd")
        sb.AppendLine("          ,b.sys_id ")
        sb.AppendLine("          ,c.kind_jun")
        sb.AppendLine("          ,c.hyouji_jun ")


        Return SqlHelper.FillData("Data Source=10.160.192.44; Initial Catalog=wzh_new;Persist Security Info=True;User ID=sa;Password=Scancheck1560", CommandType.Text, sb.ToString(), "dt")

    End Function

    'Function GetData(ByVal yotei_chk_date As String) As DataTable

    '    Dim sb As New System.Text.StringBuilder
    '    sb.AppendLine(" SELECT CONVERT(VARCHAR(100), a.yotei_chk_date, 23) AS yotei_chk_date")
    '    sb.AppendLine("       ,a.cd")
    '    sb.AppendLine("       ,a.no             ")
    '    sb.AppendLine("       ,a.shunwei        ")
    '    sb.AppendLine("       ,c.kind_name      ")
    '    sb.AppendLine("       ,c.chk_pos        ")
    '    sb.AppendLine("       ,c.chk_km_name    ")
    '    sb.AppendLine("       ,c.chk_fs_txt     ")
    '    sb.AppendLine("FROM   [wzh_new].[dbo].t_check_plan a")
    '    sb.AppendLine("       LEFT JOIN [wzh_new].[dbo].m_sys_join b")
    '    sb.AppendLine("              ON a.cd = b.cd")
    '    sb.AppendLine("       INNER JOIN [wzh_new].[dbo].m_km_template c")
    '    sb.AppendLine("               ON ( b.cd IS NULL")
    '    sb.AppendLine("                     OR b.sys_id = c.sys_name )")
    '    sb.AppendLine("                  AND ( ( c.f1 LIKE '%' + Substring(a.cd, 1, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f1) = '' )")
    '    sb.AppendLine("                         OR ( c.f1 = Substring(a .cd, 1, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f2 LIKE '%' + Substring(a.cd, 2, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f2) = '' )")
    '    sb.AppendLine("                         OR ( c.f2 = Substring(a .cd, 2, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f3 LIKE '%' + Substring(a.cd, 3, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f3) = '' )")
    '    sb.AppendLine("                         OR ( c.f3 = Substring(a .cd, 3, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f4 LIKE '%' + Substring(a.cd, 4, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f4) = '' )")
    '    sb.AppendLine("                         OR ( c.f4 = Substring(a .cd, 4, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f5 LIKE '%' + Substring(a.cd, 5, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f5) = '' )")
    '    sb.AppendLine("                         OR ( c.f5 = Substring(a .cd, 5, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f6 LIKE '%' + Substring(a.cd, 6, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f6) = '' )")
    '    sb.AppendLine("                         OR ( c.f6 = Substring(a .cd, 6, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f7 LIKE '%' + Substring(a.cd, 7, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f7) = '' )")
    '    sb.AppendLine("                         OR ( c.f7 = Substring(a .cd, 7, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f8 LIKE '%' + Substring(a.cd, 8, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f8) = '' )")
    '    sb.AppendLine("                         OR ( c.f8 = Substring(a .cd, 8, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f9 LIKE '%' + Substring(a.cd, 9, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f9) = '' )")
    '    sb.AppendLine("                         OR ( c.f9 = Substring(a .cd, 9, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f10 LIKE '%' + Substring(a.cd, 10, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f10) = '' )")
    '    sb.AppendLine("                         OR ( c.f10 = Substring(a .cd, 10, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f11 LIKE '%' + Substring(a.cd, 11, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f11) = '' )")
    '    sb.AppendLine("                         OR ( c.f11 = Substring(a .cd, 11, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f12 LIKE '%' + Substring(a.cd, 12, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f12) = '' )")
    '    sb.AppendLine("                         OR ( c.f12 = Substring(a .cd, 12, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f13 LIKE '%' + Substring(a.cd, 13, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f13) = '' )")
    '    sb.AppendLine("                         OR ( c.f13 = Substring(a .cd, 13, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f14 LIKE '%' + Substring(a.cd, 14, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f14) = '' )")
    '    sb.AppendLine("                         OR ( c.f14 = Substring(a .cd, 14, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f15 LIKE '%' + Substring(a.cd, 15, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f15) = '' )")
    '    sb.AppendLine("                         OR ( c.f15 = Substring(a .cd, 15, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f16 LIKE '%' + Substring(a.cd, 16, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f16) = '' )")
    '    sb.AppendLine("                         OR ( c.f16 = Substring(a .cd, 16, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f17 LIKE '%' + Substring(a.cd, 17, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f17) = '' )")
    '    sb.AppendLine("                         OR ( c.f17 = Substring(a .cd, 17, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f18 LIKE '%' + Substring(a.cd, 18, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f18) = '' )")
    '    sb.AppendLine("                         OR ( c.f18 = Substring(a .cd, 18, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f19 LIKE '%' + Substring(a.cd, 19, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f19) = '' )")
    '    sb.AppendLine("                         OR ( c.f19 = Substring(a .cd, 19, 1) ) )")
    '    sb.AppendLine("                  AND ( ( c.f20 LIKE '%' + Substring(a.cd, 20, 1 ) + '%'")
    '    sb.AppendLine("                           OR Ltrim(c.f20) = '' )")
    '    sb.AppendLine("                         OR ( c.f20 = Substring(a .cd, 20, 1) ) )")
    '    sb.AppendLine("WHERE a.yotei_chk_date = '" & yotei_chk_date & "'")
    '    sb.AppendLine("ORDER  BY a.yotei_chk_date")
    '    sb.AppendLine("          ,a.cd")
    '    sb.AppendLine("          ,a.no")
    '    sb.AppendLine("          ,a.shunwei")
    '    sb.AppendLine("          ,c.kind_jun")
    '    sb.AppendLine("          ,c.hyouji_jun ")



    '    Return FillData("Data Source=Ot1686; Initial Catalog=wzh_new;Persist Security Info=True;User ID=sa;Password=Scancheck1560", CommandType.Text, sb.ToString(), "dt")

    'End Function


    'Function SetXls(ByVal yotei_chk_date As String) As Boolean

    '    pub_Save_dic_path = pub_dic & yotei_chk_date & "\"


    '    If System.IO.Directory.Exists(pub_Save_dic_path) Then
    '        ' Return True
    '        If yotei_chk_date >= Now.ToString("yyyy-MM-dd") Then
    '            System.IO.Directory.Delete(pub_Save_dic_path, True)
    '        Else
    '            Return True
    '        End If
    '    End If


    '    Dim dt As DataTable = GetData(yotei_chk_date)
    '    Dim tmDt As DataTable = dt.Copy
    '    tmDt.Clear()

    '    Dim old_cd As String = "-1"
    '    Dim old_no As String = "-1"
    '    Dim old_shunwei As String = "-1"

    '    Dim cd As String
    '    Dim no As String
    '    Dim shunwei As String
    '    Dim kind_name As String
    '    Dim chk_pos As String
    '    Dim chk_km_name As String
    '    Dim chk_fs_txt As String

    '    For i As Integer = 0 To dt.Rows.Count - 1
    '        cd = dt.Rows(i).Item("cd").ToString
    '        no = dt.Rows(i).Item("no").ToString
    '        shunwei = dt.Rows(i).Item("shunwei").ToString
    '        kind_name = dt.Rows(i).Item("kind_name").ToString
    '        chk_pos = dt.Rows(i).Item("chk_pos").ToString
    '        chk_km_name = dt.Rows(i).Item("chk_km_name").ToString
    '        chk_fs_txt = dt.Rows(i).Item("chk_fs_txt").ToString


    '        If old_cd = cd AndAlso no = old_no AndAlso shunwei = old_shunwei Then
    '            tmDt.Rows.Add(dt.Rows(i).ItemArray)
    '        Else
    '            SetDataToFile(tmDt, old_cd, old_no, old_shunwei)
    '            tmDt.Clear()
    '        End If

    '        old_cd = cd
    '        old_no = no
    '        old_shunwei = shunwei
    '    Next
    'End Function

    'Function SetDataToFile(ByVal dt As DataTable, ByVal inCd As String, ByVal inNo As String, ByVal inShunWei As String)
    '    Dim cd As String
    '    Dim no As String
    '    Dim shunwei As String
    '    Dim kind_name As String
    '    Dim chk_pos As String
    '    Dim chk_km_name As String
    '    Dim chk_fs_txt As String

    '    Dim old_cd As String = "-1"
    '    Dim old_no As String = "-1"
    '    Dim old_shunwei As String = "-1"
    '    Dim old_kind_name As String = "-1"
    '    Dim old_chk_km_name As String = "-1"

    '    Dim sb As New StringBuilder
    '    sb.AppendLine("<table>")
    '    sb.AppendLine("<tr>")
    '    sb.AppendLine("<th colspan='2'>" & inCd & "</th>")
    '    sb.AppendLine("<th colspan='2'>" & inNo & "</th>")
    '    sb.AppendLine("<th colspan='2'>" & inShunWei & "</th>")

    '    sb.AppendLine("</tr>")


    '    sb.AppendLine("<tr>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;'>种类</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;'>位置</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;'>项目名</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;'>方法</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;'>实测值</th>")
    '    sb.AppendLine("<th style='border:.5pt solid windowtext;'>备考</th>")
    '    sb.AppendLine("</tr>")

    '    For i As Integer = 0 To dt.Rows.Count - 1
    '        sb.AppendLine("<tr>")
    '        cd = dt.Rows(i).Item("cd").ToString
    '        no = dt.Rows(i).Item("no").ToString
    '        shunwei = dt.Rows(i).Item("shunwei").ToString
    '        kind_name = dt.Rows(i).Item("kind_name").ToString
    '        chk_pos = dt.Rows(i).Item("chk_pos").ToString
    '        chk_km_name = dt.Rows(i).Item("chk_km_name").ToString
    '        chk_fs_txt = dt.Rows(i).Item("chk_fs_txt").ToString

    '        If kind_name <> old_kind_name Then
    '            sb.AppendLine("<td rowspan='" & GetRowSpanBy_kind_name(dt, i, kind_name) & "'  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("kind_name").ToString & "</td>")
    '        End If
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_pos").ToString & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_km_name").ToString & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>" & dt.Rows(i).Item("chk_fs_txt").ToString & "</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>&nbsp;</td>")
    '        sb.AppendLine("<td  style='border:.5pt solid windowtext;'>&nbsp;</td>")


    '        'If kind_name = old_kind_name OrElse chk_km_name = old_chk_km_name Then
    '        '    'sb.AppendLine("<td>" & dt.Rows(i).Item("kind_name").ToString & "</td>")
    '        '    'sb.AppendLine("<td>" & dt.Rows(i).Item("chk_pos").ToString & "</td>")
    '        '    sb.AppendLine("<td>" & dt.Rows(i).Item("chk_km_name").ToString & "</td>")
    '        '    sb.AppendLine("<td>" & dt.Rows(i).Item("chk_fs_txt").ToString & "</td>")
    '        '    sb.AppendLine("<td>&nbsp;</td>")
    '        '    sb.AppendLine("<td>&nbsp;</td>")
    '        'ElseIf kind_name <> old_kind_name Then
    '        '    sb.AppendLine("<td rowspan='" & GetRowSpanBy_kind_name(dt, i, kind_name) & "'>" & dt.Rows(i).Item("kind_name").ToString & "</td>")
    '        '    sb.AppendLine("<td>" & dt.Rows(i).Item("chk_pos").ToString & "</td>")
    '        '    sb.AppendLine("<td>" & dt.Rows(i).Item("chk_km_name").ToString & "</td>")
    '        '    sb.AppendLine("<td>" & dt.Rows(i).Item("chk_fs_txt").ToString & "</td>")
    '        '    sb.AppendLine("<td>&nbsp;</td>")
    '        '    sb.AppendLine("<td>&nbsp;</td>")
    '        'ElseIf chk_km_name <> old_chk_km_name Then
    '        'End If

    '        old_kind_name = kind_name
    '        old_chk_km_name = chk_km_name
    '        sb.AppendLine("</tr>")
    '    Next
    '    sb.AppendLine("</table>")

    '    If Not System.IO.Directory.Exists(pub_Save_dic_path) Then
    '        System.IO.Directory.CreateDirectory(pub_Save_dic_path)

    '    End If

    '    System.IO.File.WriteAllText(pub_Save_dic_path & inCd & "_" & inNo & "_" & inShunWei & ".xls", sb.ToString)

    '    Return True

    'End Function

End Class
