Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Text
Imports System.Data
Imports System.Configuration.ConfigurationSettings
Imports System.Collections.Generic
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Runtime.Serialization.Json
Imports System
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports System.IO
Imports System.Net
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.Collections

' この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class TableApi
    Inherits System.Web.Services.WebService
    Private ReadOnly _arrDateColNames() As String = New String() {"datetime", "datetime2"}
    Private ReadOnly _arrNumberColNames() As String = New String() {"numeric", "float", "money", "smallmoney", "decimal"}
    Public Const ConRowSeparator As Char = "‡"c
    Public Const ConColumnSeparator As Char = "†"c

    '表结构信息取得
    '-----------------------------------------------
    Public Function GetTableInfoDt(ByVal tableName As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT ")
        sb.AppendLine("     a.NAME AS table_name")
        sb.AppendLine("   , b.NAME AS columns_name_en")
        sb.AppendLine("   , c.NAME AS columns_type")
        sb.AppendLine("    , case when c.NAME='tinyint' then '3'")
        sb.AppendLine("      when c.NAME='int' then '10'")
        sb.AppendLine("      when b.xscale =0 then ")
        sb.AppendLine("       cast(b.length as varchar) ")
        sb.AppendLine("     else ")
        sb.AppendLine("       cast(b.xprec as varchar) +','+ cast(b.xscale as varchar) ")
        sb.AppendLine("     end as columns_length")
        sb.AppendLine("   , CASE ")
        sb.AppendLine("     WHEN d.TABLE_NAME IS NULL THEN ''")
        sb.AppendLine("     ELSE 'P'")
        sb.AppendLine("     END AS pk")
        sb.AppendLine("   ,SM.TEXT AS DefaultValue")
        sb.AppendLine("   ,b.IsNullable")

        If Left(tableName, 2) = "v_" Then
            sb.AppendLine("   ,b.NAME As columns_name_cn")
        Else
            sb.AppendLine("   ,g.value As columns_name_cn")

        End If

        sb.AppendLine("   ,a.xtype As TableOrView")
        sb.AppendLine("FROM sysobjects a")
        sb.AppendLine(" INNER JOIN syscolumns b ON a.id = b.id")
        sb.AppendLine(" INNER JOIN systypes c ON b.xtype = c.xtype")
        sb.AppendLine(" LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE d")
        sb.AppendLine(" ON d.TABLE_NAME = a.NAME")
        sb.AppendLine("     AND d.COLUMN_NAME = b.NAME")
        sb.AppendLine(" LEFT JOIN dbo.syscomments SM ON b.cdefault = SM.id")
        sb.AppendLine(" left join sys.extended_properties g ")
        sb.AppendLine("on b.id=g.major_id AND b.colid = g.minor_id ")
        sb.AppendLine("WHERE a.xtype in ('U','V')")
        sb.AppendLine("       AND a.NAME = '" & tableName & "'  ")
        sb.AppendLine("and c.NAME <> 'sysname'")
        sb.AppendLine("ORDER BY b.colorder")


        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetTableInfo")

        If Left(tableName, 2) = "v_" Or Right(tableName, 2) = "_v" Then
            For i As Integer = 0 To dt.Rows.Count - 1
                If dt.Rows(i).Item("columns_name_cn") Is DBNull.Value Then
                    dt.Rows(i).Item("columns_name_cn") = GetName(dt.Rows(i).Item("columns_name_en"))
                End If
            Next
        End If


        Return dt
        'Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetTableInfo")
    End Function


    Function GetName(v1)

        If v1 = "ck_id" Then Return "检查id"
        If v1 = "cd" Then Return "CD"
        If v1 = "no" Then Return "作番"
        If v1 = "department_cd" Then Return "部门"
        If v1 = "line_cd" Then Return "生产线"
        If v1 = "chk_user" Then Return "检查者"
        If v1 = "chk_user_name" Then Return "NULL"
        If v1 = "yotei_chk_date" Then Return "预定检查日"
        If v1 = "chk_start_date" Then Return "开始检查时间"
        If v1 = "chk_end_date" Then Return "终了检查时间"
        If v1 = "status" Then Return "状态"
        If v1 = "result" Then Return "结果"
        If v1 = "chk_times" Then Return "次数"
        If v1 = "suu" Then Return "数量"
        If v1 = "del_flg" Then Return "删除区分"
        If v1 = "qianpin" Then Return "欠品区分"
        If v1 = "tools_scan_flg" Then Return "治具扫描区分"
        If v1 = "shared_ck_id" Then Return "继承CKID"
        If v1 = "shared_no" Then Return "共享No"
        If v1 = "h" Then Return "h"
        If v1 = "w" Then Return "w"
        If v1 = "dh" Then Return "dh"
        If v1 = "dw" Then Return "dw"
        If v1 = "sw" Then Return "sw"
        If v1 = "kw" Then Return "kw"
        If v1 = "b2bOderNo" Then Return "B2B订单号"
        If v1 = "b2bIndexNo" Then Return "B2B采购单号"
        If v1 = "edit_user" Then Return "NULL"
        If v1 = "upd_user" Then Return "更新者"
        If v1 = "upd_date" Then Return "更新日"
        If v1 = "ins_user" Then Return "登录者"
        If v1 = "ins_date" Then Return "登录日"



        Return v1

    End Function


    <WebMethod()>
    Public Function GetListBoxStr(ByVal sql As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Return JsonConvert.SerializeObject(FillData(DataAccessManager.ConnStr, CommandType.Text, sql, "GetListBoxStr"))
    End Function

    '行号排序
    '-----------------------------------------------
    Private Function OrderByStr(ByVal tableInfo As DataTable) As String
        Dim sb As New StringBuilder
        Dim kama As String = String.Empty
        For i As Integer = 0 To tableInfo.Rows.Count - 1
            If tableInfo.Rows(i).Item("pk") IsNot DBNull.Value AndAlso tableInfo.Rows(i).Item("pk").ToString() = "P" Then
                sb.AppendLine(kama & tableInfo.Rows(i).Item("columns_name_en").ToString())
                kama = ","
            End If
        Next
        Return sb.ToString
    End Function

    '检索的项目
    '-----------------------------------------------
    Private Function Get_SelectItemStr(ByVal dr As DataRow) As String
        Dim tp As String = dr.Item("columns_type").ToString()
        If _arrDateColNames.Contains(tp) Then
            Return ", CONVERT(varchar(100)," & dr.Item("columns_name_en").ToString & " ,20) " & dr.Item("columns_name_en").ToString
        Else
            Return "," & dr.Item("columns_name_en").ToString
        End If
    End Function

    '检索的Sql
    '-----------------------------------------------
    Private Function GetTableSql(ByVal tableName As String, ByVal rowValues As String, ByVal awaysWhere As String, ByVal IsGetCnt As Boolean, ByVal maxCnt As Integer) As String
        Dim dtCols As DataTable = GetTableInfoDt(tableName)

        Dim isTable As Boolean = dtCols.Rows(0).Item("TableOrView").ToString.Trim() = "U"

        '行号 与 明细排序用
        '   1
        '   Row number
        '   items 1...
        '   items 2...
        Dim orderBy As String = OrderByStr(dtCols)

        If tableName = "v_check_result_ms" Then
            orderBy = "ck_id,kind_jun,hyouji_jun"
        ElseIf tableName = "v_A01_result_list" Then
            orderBy = "生成实际日,cd"
        ElseIf tableName = "v_A02_check_result_ms" Then
            orderBy = "id,检查顺序"
        ElseIf tableName = "v_A03_mi_check_result" Then
            orderBy = "纳期日 desc"
        ElseIf tableName = "v_A05_check_scx" Then
            orderBy = "部门"
        ElseIf tableName = "t_check" Then
            orderBy = "yotei_chk_date desc,chk_start_date desc"
        End If

        Dim colsSb As New StringBuilder
        colsSb.AppendLine("'1' AS Del")                                                         '删除按钮列

        '如果是表
        If isTable Then
            colsSb.AppendLine(",ROW_NUMBER() OVER (ORDER BY " & orderBy & ") AS RowNumber")         '行号列
        ElseIf tableName = "m_mubiao_time" Then
            colsSb.AppendLine(",ROW_NUMBER() OVER (ORDER BY line_cd) AS RowNumber")
        ElseIf tableName = "v_A05_check_scx" Then
            colsSb.AppendLine(",ROW_NUMBER() OVER (ORDER BY 检查日期) AS RowNumber")
        ElseIf tableName = "v_A05_check_scx_one" Then
            colsSb.AppendLine(",ROW_NUMBER() OVER (ORDER BY 检查日期) AS RowNumber")
        Else
            '如果是VIEW 视图
            colsSb.AppendLine(",ROW_NUMBER() OVER (ORDER BY cd) AS RowNumber")
        End If

        For i As Integer = 0 To dtCols.Rows.Count - 1
            colsSb.AppendLine(Get_SelectItemStr(dtCols.Rows(i)))
        Next

        'SQLコメント
        '追加了行号的 默认 RowNumber
        Dim sb As New StringBuilder
        Dim wr As String = GetSelWhere(tableName, rowValues)

        If maxCnt > 0 Then
            sb.AppendLine("SELECT top " & maxCnt & " * FROM ")
        Else
            If IsGetCnt Then
                sb.AppendLine("SELECT * FROM ")
            Else
                If wr.Trim = "" Then
                    sb.AppendLine("SELECT top 1000 * FROM ")
                Else
                    sb.AppendLine("SELECT * FROM ")
                End If
            End If
        End If



        sb.AppendLine("(")
        sb.AppendLine("SELECT")
        sb.AppendLine(colsSb.ToString)
        sb.AppendLine("FROM " & tableName & " ")
        sb.AppendLine("WHERE 1=1 ")   '必须有
        sb.AppendLine(awaysWhere)     '必须有
        sb.AppendLine(wr)
        sb.AppendLine(" )a")
        sb.AppendLine("WHERE 1=1 ")   '必须有

        'If orderBy <> "" Then
        '    sb.AppendLine("order by " & orderBy)
        'End If

        Return sb.ToString
    End Function

#Region "WebMethod"


    ''' <summary>
    ''' 高性能动态 SQL 构建（适配直接传入的 SQL 过滤片段）
    ''' </summary>
    ''' <param name="filterSql">例如: "AND 检查日期YMD BETWEEN '2026-04-26' AND '2026-04-26'"</param>
    Public Function Get_v_A02_check_result_msSql_DirectFragmentBK(filterSql As String) As String
        Dim sql As New StringBuilder()

        ' --- 1. 自动预判：根据传入的字符串片段，判断需要提拔哪些关联表到第一层 ---
        Dim needH As Boolean = filterSql.Contains("生成实际日YMD")
        Dim needD As Boolean = filterSql.Contains("经销商") OrElse filterSql.Contains("发注书")
        Dim needG As Boolean = filterSql.Contains("商品名")
        Dim needMS As Boolean = filterSql.Contains("检查项目") OrElse filterSql.Contains("检查结果")

        sql.AppendLine("WITH MainTask AS (")
        sql.AppendLine("    SELECT ")
        sql.AppendLine("        a.ck_id,")
        ' 预计算检查日期，方便内部直接引用你的过滤片段
        sql.AppendLine("        CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, a.chk_start_date) < 8 ")
        sql.AppendLine("                                  THEN DATEADD(DAY, -1, a.chk_start_date) ")
        sql.AppendLine("                                  ELSE a.chk_start_date END, 23) AS [检查日期YMD],")
        sql.AppendLine("        ROW_NUMBER() OVER (ORDER BY ")
        sql.AppendLine("            CASE WHEN DATEPART(HOUR, a.chk_start_date) < 8 ")
        sql.AppendLine("                 THEN DATEADD(DAY, -1, a.chk_start_date) ")
        sql.AppendLine("                 ELSE a.chk_start_date END DESC, ")
        sql.AppendLine("            a.ck_id DESC")
        sql.AppendLine("        ) AS RowNumber")
        sql.AppendLine("    FROM dbo.t_check AS a WITH (NOLOCK)")

        ' --- 动态提拔联接（由 filterSql 决定） ---
        If needH Then
            sql.AppendLine("    LEFT JOIN [AvoidMiss_New].dbo.tb_completedata AS h_f WITH (NOLOCK) ON h_f.makenumber = a.no AND h_f.code = REPLACE(a.cd, '-', '')")
        End If
        If needD Then
            sql.AppendLine("    LEFT JOIN dbo.tcm_bianplan AS d_f WITH (NOLOCK) ON d_f.productcode = a.cd AND d_f.zuofan = a.no AND d_f.b2boderno = a.b2boderno AND d_f.b2bindexno = a.b2bindexno")
        End If
        If needG Then
            sql.AppendLine("    LEFT JOIN dbo.m_goods_tmp AS g_f WITH (NOLOCK) ON g_f.goods_cd = REPLACE(a.cd, '-', '')")
        End If
        If needMS Then
            sql.AppendLine("    LEFT JOIN dbo.t_check_ms AS ms_f WITH (NOLOCK) ON ms_f.ck_id = a.ck_id")
        End If

        sql.AppendLine("    WHERE 1=1 ")

        ' --- 2. 核心：处理传入的 SQL 片段 ---
        ' 为了让你的条件片段生效，我们需要通过别名映射，确保片段里的字段指向正确的表
        Dim processedFilter As String = filterSql

        ' 如果片段里有这些计算字段，我们需要把它们替换成对应的计算表达式
        processedFilter = processedFilter.Replace("生成实际日YMD",
        "CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, h_f.finish_date) < 8 THEN DATEADD(DAY, -1, h_f.finish_date) ELSE h_f.finish_date END, 23)")

        ' 将常用别名重定向到子查询内部的别名
        processedFilter = processedFilter.Replace("商品名", "g_f.goods_name")
        processedFilter = processedFilter.Replace("发注书", "d_f.specialBookNo")
        processedFilter = processedFilter.Replace("检查项目", "ms_f.chk_km_name")
        processedFilter = processedFilter.Replace("检查结果", "ms_f.result")

        sql.AppendLine("    " & processedFilter)
        sql.AppendLine(")")

        ' --- 3. 最终主查询 ---
        sql.AppendLine("SELECT ")
        sql.AppendLine("    '1' AS Del, m.RowNumber, a.ck_id AS id, a.cd, g.goods_name AS 商品名, a.no AS 作番,")
        sql.AppendLine("    CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, h.finish_date) < 8 THEN DATEADD(DAY, -1, h.finish_date) ELSE h.finish_date END, 23) AS [生成实际日YMD],")
        sql.AppendLine("    h.productionquantity AS 完工数量, a.line_cd AS 生产线, c.line_name AS 生产线名,")
        sql.AppendLine("    d.CD_Dealer + ':' + d.DealerAbbreviation AS 经销商, d.specialBookNo AS 发注书,")
        sql.AppendLine("    d.destinationcode AS 向先, d.billno AS 纳期, a.department_cd AS 部门, m.检查日期YMD,")
        sql.AppendLine("    a.chk_user + '(' + ISNULL(b.user_name, '') + ')' AS 检查者, b.line_cd AS 检查者生产线,")
        sql.AppendLine("    ms.tools_ma, ms.pic_name, ms.kind_name, ms.kind_jun, ms.chk_pos, ms.chk_km_name, ")
        sql.AppendLine("    ms.k_type, ms.k1, ms.k2, ms.k3, ms.chk_times, ms.chk_fs_txt, ms.in_1, ms.result, ms.mark,")
        sql.AppendLine("    a.qianpin, a.chk_start_date, a.chk_end_date, DATEDIFF(SECOND, a.chk_start_date, a.chk_end_date) AS 检查时长,")
        sql.AppendLine("    a.shared_ck_id, img.PicList AS 图片")
        sql.AppendLine("FROM MainTask AS m")
        sql.AppendLine("INNER JOIN dbo.t_check AS a WITH (NOLOCK) ON a.ck_id = m.ck_id")
        sql.AppendLine("LEFT JOIN dbo.m_user AS b WITH (NOLOCK) ON a.chk_user = b.user_cd")
        sql.AppendLine("LEFT JOIN dbo.m_line AS c WITH (NOLOCK) ON a.line_cd = c.line_cd")
        sql.AppendLine("LEFT JOIN dbo.t_check_ms AS ms WITH (NOLOCK) ON a.ck_id = ms.ck_id")
        sql.AppendLine("LEFT JOIN [AvoidMiss_New].dbo.tb_completedata AS h WITH (NOLOCK) ON h.makenumber = a.no AND h.code = REPLACE(a.cd, '-', '')")
        sql.AppendLine("LEFT JOIN dbo.tcm_bianplan AS d WITH (NOLOCK) ON d.productcode = a.cd AND d.zuofan = a.no AND d.b2boderno = a.b2boderno AND d.b2bindexno = a.b2bindexno AND CONVERT(DATETIME, d.plandate, 120) = a.yotei_chk_date")
        sql.AppendLine("LEFT JOIN dbo.m_goods_tmp AS g WITH (NOLOCK) ON g.goods_cd = REPLACE(a.cd, '-', '')")
        sql.AppendLine("OUTER APPLY (")
        sql.AppendLine("    SELECT (SELECT CAST([idx] AS VARCHAR) + '|' FROM [m_picture_chk] AS p WITH (NOLOCK) WHERE p.[chk_id] = a.[ck_id] FOR XML PATH('')) AS PicList")
        sql.AppendLine(") AS img")

        ' 分页
        sql.AppendLine("WHERE m.RowNumber BETWEEN @startRow AND @endRow")
        sql.AppendLine("ORDER BY m.RowNumber, ms.kind_jun;")

        Return sql.ToString()
    End Function


    '''' <summary>
    '''' 获取检查结果分页SQL（兼容VS2015）
    '''' </summary>
    '''' <param name="filterSql">外部传入的过滤条件，例如 "作番='123' AND 生产线名 LIKE '%A%'"</param>
    '''' <param name="startRow">起始行号</param>
    '''' <param name="endRow">结束行号</param>
    'Public Function Get_v_A02_check_result_msSql_DirectFragment(filterSql As String, startRow As Integer, endRow As Integer) As String
    '    Dim sql As New StringBuilder()

    '    ' --- 1. 定义字段映射表 ---
    '    ' 将前端显示的名称映射到数据库真实的别名和字段
    '    Dim fieldMap As New Dictionary(Of String, String)()
    '    fieldMap.Add("生成实际日YMD", "CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, h_f.finish_date) < 8 THEN DATEADD(DAY, -1, h_f.finish_date) ELSE h_f.finish_date END, 23)")
    '    fieldMap.Add("商品名", "g_f.goods_name")
    '    fieldMap.Add("作番", "a.no")
    '    fieldMap.Add("生产线", "a.line_cd")
    '    fieldMap.Add("生产线名", "c_f.line_name")
    '    fieldMap.Add("经销商", "d_f.CD_Dealer")
    '    fieldMap.Add("发注书", "d_f.specialBookNo")
    '    fieldMap.Add("部门", "a.department_cd")
    '    fieldMap.Add("检查者", "a.chk_user")
    '    fieldMap.Add("检查项目", "ms_f.chk_km_name")
    '    fieldMap.Add("检查结果", "ms_f.result")

    '    ' --- 2. 自动预判联接需求 ---
    '    ' 只有当 filterSql 包含相关字段时，才在 CTE 中进行 JOIN 以提高性能
    '    Dim safeFilter As String = If(filterSql, "")
    '    Dim needH As Boolean = safeFilter.Contains("生成实际日YMD") OrElse safeFilter.Contains("完工数量")
    '    Dim needD As Boolean = safeFilter.Contains("经销商") OrElse safeFilter.Contains("发注书")
    '    Dim needG As Boolean = safeFilter.Contains("商品名")
    '    Dim needMS As Boolean = safeFilter.Contains("检查项目") OrElse safeFilter.Contains("检查结果")
    '    Dim needC As Boolean = safeFilter.Contains("生产线名")

    '    ' --- 3. 构建 CTE 子查询 (MainTask) ---
    '    sql.AppendLine("WITH MainTask AS (")
    '    sql.AppendLine("    SELECT ")
    '    sql.AppendLine("        a.ck_id,")
    '    sql.AppendLine("        CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, a.chk_start_date) < 8 ")
    '    sql.AppendLine("                                  THEN DATEADD(DAY, -1, a.chk_start_date) ")
    '    sql.AppendLine("                                  ELSE a.chk_start_date END, 23) AS [检查日期YMD],")
    '    sql.AppendLine("        ROW_NUMBER() OVER (ORDER BY a.chk_start_date DESC, a.ck_id DESC) AS RowNumber")
    '    sql.AppendLine("    FROM dbo.t_check AS a WITH (NOLOCK)")

    '    ' 动态联接 (使用 _f 别名防止与外层冲突)
    '    If needH Then sql.AppendLine("    LEFT JOIN [AvoidMiss_New].dbo.tb_completedata AS h_f WITH (NOLOCK) ON h_f.makenumber = a.no AND h_f.code = REPLACE(a.cd, '-', '')")
    '    If needD Then sql.AppendLine("    LEFT JOIN dbo.tcm_bianplan AS d_f WITH (NOLOCK) ON d_f.productcode = a.cd AND d_f.zuofan = a.no AND d_f.b2boderno = a.b2boderno AND d_f.b2bindexno = a.b2bindexno")
    '    If needG Then sql.AppendLine("    LEFT JOIN dbo.m_goods_tmp AS g_f WITH (NOLOCK) ON g_f.goods_cd = REPLACE(a.cd, '-', '')")
    '    If needMS Then sql.AppendLine("   LEFT JOIN dbo.t_check_ms AS ms_f WITH (NOLOCK) ON ms_f.ck_id = a.ck_id")
    '    If needC Then sql.AppendLine("    LEFT JOIN dbo.m_line AS c_f WITH (NOLOCK) ON c_f.line_cd = a.line_cd")

    '    sql.AppendLine("    WHERE 1=1 ")

    '    ' --- 4. 解析并替换 filterSql 中的中文别名为数据库字段 ---
    '    Dim processedFilter As String = safeFilter

    '    ' 处理计算字段：检查日期YMD (WHERE 子句不能直接用别名，需替换为计算式)
    '    If processedFilter.Contains("检查日期YMD") Then
    '        Dim dateExpr As String = "CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, a.chk_start_date) < 8 THEN DATEADD(DAY, -1, a.chk_start_date) ELSE a.chk_start_date END, 23)"
    '        processedFilter = processedFilter.Replace("检查日期YMD", dateExpr)
    '    End If

    '    ' 遍历字典替换其他业务字段名
    '    Dim entry As KeyValuePair(Of String, String)
    '    For Each entry In fieldMap
    '        If processedFilter.Contains(entry.Key) Then
    '            processedFilter = processedFilter.Replace(entry.Key, entry.Value)
    '        End If
    '    Next

    '    ' 注入处理后的过滤条件
    '    If Not String.IsNullOrEmpty(processedFilter.Trim()) Then
    '        'sql.AppendLine("    AND (" & processedFilter & ")")
    '        sql.AppendLine(processedFilter)
    '    End If

    '    sql.AppendLine(")")

    '    ' --- 5. 最终查询结果集 ---
    '    sql.AppendLine("Select ")
    '    sql.AppendLine("    '1' AS Del, m.RowNumber, a.ck_id AS id, a.cd, g.goods_name AS 商品名, a.no AS 作番,")
    '    sql.AppendLine("    CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, h.finish_date) < 8 THEN DATEADD(DAY, -1, h.finish_date) ELSE h.finish_date END, 23) AS [生成实际日YMD],")
    '    sql.AppendLine("    h.productionquantity AS 完工数量, a.line_cd AS 生产线, c.line_name AS 生产线名,")
    '    sql.AppendLine("    d.CD_Dealer + ':' + d.DealerAbbreviation AS 经销商, d.specialBookNo AS 发注书,")
    '    sql.AppendLine("    d.destinationcode AS 向先, d.billno AS 纳期, a.department_cd AS 部门, m.检查日期YMD,")
    '    sql.AppendLine("    a.chk_user + '(' + ISNULL(b.user_name, '') + ')' AS 检查者, b.line_cd AS 检查者生产线,")
    '    sql.AppendLine("    ms.tools_ma, ms.pic_name, ms.kind_name, ms.kind_jun, ms.chk_pos, ms.chk_km_name, ")
    '    sql.AppendLine("    ms.k_type, ms.k1, ms.k2, ms.k3, ms.chk_times, ms.chk_fs_txt, ms.in_1, ms.result, ms.mark,")
    '    sql.AppendLine("    a.qianpin, a.chk_start_date, a.chk_end_date, DATEDIFF(SECOND, a.chk_start_date, a.chk_end_date) AS 检查时长,")
    '    sql.AppendLine("    a.shared_ck_id, img.PicList AS 图片")
    '    sql.AppendLine("FROM MainTask AS m")
    '    sql.AppendLine("INNER JOIN dbo.t_check AS a WITH (NOLOCK) ON a.ck_id = m.ck_id")
    '    sql.AppendLine("LEFT JOIN dbo.m_user AS b WITH (NOLOCK) ON a.chk_user = b.user_cd")
    '    sql.AppendLine("LEFT JOIN dbo.m_line AS c WITH (NOLOCK) ON a.line_cd = c.line_cd")
    '    sql.AppendLine("LEFT JOIN dbo.t_check_ms AS ms WITH (NOLOCK) ON a.ck_id = ms.ck_id")
    '    sql.AppendLine("LEFT JOIN [AvoidMiss_New].dbo.tb_completedata AS h WITH (NOLOCK) ON h.makenumber = a.no AND h.code = REPLACE(a.cd, '-', '')")
    '    sql.AppendLine("LEFT JOIN dbo.tcm_bianplan AS d WITH (NOLOCK) ON d.productcode = a.cd AND d.zuofan = a.no AND d.b2boderno = a.b2boderno AND d.b2bindexno = a.b2bindexno")
    '    sql.AppendLine("LEFT JOIN dbo.m_goods_tmp AS g WITH (NOLOCK) ON g.goods_cd = REPLACE(a.cd, '-', '')")
    '    sql.AppendLine("OUTER APPLY (")
    '    sql.AppendLine("    SELECT (SELECT CAST([idx] AS VARCHAR) + '|' FROM [m_picture_chk] AS p WITH (NOLOCK) WHERE p.[chk_id] = a.[ck_id] FOR XML PATH('')) AS PicList")
    '    sql.AppendLine(") AS img")

    '    ' 使用传入的参数进行分页过滤
    '    ' 注意：VB.NET 中 Integer 转 String 会根据区域设置，这里使用显式拼接
    '    sql.AppendLine("WHERE m.RowNumber BETWEEN " & startRow.ToString() & " AND " & endRow.ToString())
    '    sql.AppendLine("ORDER BY m.RowNumber, ms.kind_jun;")

    '    Return sql.ToString()
    'End Function


    Public Function Get_v_A02_check_result_msSql_DirectFragment(filterSql As String, startRow As Integer, endRow As Integer) As String
        Dim sql As New StringBuilder()

        ' --- 1. 定义字段映射 (注意别名现在统一指向 CTE 内部的表) ---
        Dim fieldMap As New Dictionary(Of String, String)()
        ' 检查表 a
        fieldMap.Add("作番", "a_f.no")
        fieldMap.Add("生产线", "a_f.line_cd")
        fieldMap.Add("部门", "a_f.department_cd")
        ' 明细表 ms
        fieldMap.Add("检查项目", "ms_f.chk_km_name")
        fieldMap.Add("检查结果", "ms_f.result")
        ' 关联表
        fieldMap.Add("生产线名", "c_f.line_name")
        fieldMap.Add("商品名", "g_f.goods_name")
        fieldMap.Add("发注书", "d_f.specialBookNo")
        fieldMap.Add("生成实际日YMD", "CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, h_f.finish_date) < 8 THEN DATEADD(DAY, -1, h_f.finish_date) ELSE h_f.finish_date END, 23)")

        ' --- 2. 判定联接需求 ---
        Dim safeFilter As String = If(filterSql, "")
        ' 由于是以 ms 为主，ms 表是必须联接的，其他按需联接
        Dim needA As Boolean = safeFilter.Contains("作番") OrElse safeFilter.Contains("生产线") OrElse safeFilter.Contains("部门") OrElse safeFilter.Contains("检查日期YMD")
        Dim needC As Boolean = safeFilter.Contains("生产线名")
        Dim needG As Boolean = safeFilter.Contains("商品名")
        Dim needD As Boolean = safeFilter.Contains("发注书")
        Dim needH As Boolean = safeFilter.Contains("生成实际日YMD")

        ' --- 3. 构建 CTE (核心：以 t_check_ms 为中心) ---
        sql.AppendLine("WITH MainTask AS (")
        sql.AppendLine("    SELECT ")
        sql.AppendLine("        ms_f.km_id,") ' 假设 ms 表的主键是 ms_id，请根据实际改为你的主键名
        sql.AppendLine("        ms_f.ck_id,")
        ' 生成唯一的 RowNumber
        sql.AppendLine("        ROW_NUMBER() OVER (ORDER BY a_f.chk_start_date DESC, ms_f.ck_id DESC, ms_f.kind_jun ASC, ms_f.km_id ASC) AS RowNumber")
        sql.AppendLine("    FROM dbo.t_check_ms AS ms_f WITH (NOLOCK)")
        ' 必须联接 a 表来获取日期和作番等基础信息
        sql.AppendLine("    INNER JOIN dbo.t_check AS a_f WITH (NOLOCK) ON ms_f.ck_id = a_f.ck_id")

        ' 动态联接过滤表
        If needC Then sql.AppendLine("    LEFT JOIN dbo.m_line AS c_f WITH (NOLOCK) ON c_f.line_cd = a_f.line_cd")
        If needG Then sql.AppendLine("    LEFT JOIN dbo.m_goods_tmp AS g_f WITH (NOLOCK) ON g_f.goods_cd = REPLACE(a_f.cd, '-', '')")
        If needD Then sql.AppendLine("    LEFT JOIN dbo.tcm_bianplan AS d_f WITH (NOLOCK) ON d_f.productcode = a_f.cd AND d_f.zuofan = a_f.no AND d_f.b2boderno = a_f.b2boderno AND d_f.b2bindexno = a_f.b2bindexno")
        If needH Then sql.AppendLine("    LEFT JOIN [AvoidMiss_New].dbo.tb_completedata AS h_f WITH (NOLOCK) ON h_f.makenumber = a_f.no AND h_f.code = REPLACE(a_f.cd, '-', '')")

        sql.AppendLine("    WHERE 1=1 ")

        ' 处理过滤片段
        Dim processedFilter As String = safeFilter
        ' 处理计算字段 (检查日期YMD)
        If processedFilter.Contains("检查日期YMD") Then
            Dim dateExpr As String = "CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, a_f.chk_start_date) < 8 THEN DATEADD(DAY, -1, a_f.chk_start_date) ELSE a_f.chk_start_date END, 23)"
            processedFilter = processedFilter.Replace("检查日期YMD", dateExpr)
        End If
        ' 字典映射替换
        Dim entry As KeyValuePair(Of String, String)
        For Each entry In fieldMap
            If processedFilter.Contains(entry.Key) Then
                processedFilter = processedFilter.Replace(entry.Key, entry.Value)
            End If
        Next

        If Not String.IsNullOrEmpty(processedFilter.Trim()) Then
            'sql.AppendLine("    AND (" & processedFilter & ")")
            sql.AppendLine(processedFilter & "")
        End If

        sql.AppendLine(")")

        ' --- 4. 最终主查询 ---
        sql.AppendLine("SELECT ")
        sql.AppendLine("    '1' AS Del, m.RowNumber, a.ck_id AS id, a.cd, g.goods_name AS 商品名, a.no AS 作番,")
        sql.AppendLine("    CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, h.finish_date) < 8 THEN DATEADD(DAY, -1, h.finish_date) ELSE h.finish_date END, 23) AS [生成实际日YMD],")
        sql.AppendLine("    h.productionquantity AS 完工数量, a.line_cd AS 生产线, c.line_name AS 生产线名,")
        sql.AppendLine("    d.CD_Dealer + ':' + d.DealerAbbreviation AS 经销商, d.specialBookNo AS 发注书,")
        sql.AppendLine("    d.destinationcode AS 向先, d.billno AS 纳期, a.department_cd AS 部门, ")
        sql.AppendLine("    CONVERT(VARCHAR(10), CASE WHEN DATEPART(HOUR, a.chk_start_date) < 8 THEN DATEADD(DAY, -1, a.chk_start_date) ELSE a.chk_start_date END, 23) AS [检查日期YMD],")
        sql.AppendLine("    a.chk_user + '(' + ISNULL(b.user_name, '') + ')' AS 检查者, b.line_cd AS 检查者生产线,")
        sql.AppendLine("    ms.tools_ma, ms.pic_name, ms.kind_name, ms.kind_jun, ms.chk_pos, ms.chk_km_name, ")
        sql.AppendLine("    ms.k_type, ms.k1, ms.k2, ms.k3, ms.chk_times, ms.chk_fs_txt, ms.in_1, ms.result, ms.mark,")
        sql.AppendLine("    a.qianpin, a.chk_start_date, a.chk_end_date, DATEDIFF(SECOND, a.chk_start_date, a.chk_end_date) AS 检查时长,")
        sql.AppendLine("    a.shared_ck_id")
        sql.AppendLine("FROM MainTask AS m")
        ' 通过 ms_id 联接回明细表，确保 RowNumber 与行一一对应
        sql.AppendLine("INNER JOIN dbo.t_check_ms AS ms WITH (NOLOCK) ON ms.ck_id = m.ck_id and  ms.km_id = m.km_id")
        sql.AppendLine("INNER JOIN dbo.t_check AS a WITH (NOLOCK) ON a.ck_id = ms.ck_id")
        sql.AppendLine("LEFT JOIN dbo.m_user AS b WITH (NOLOCK) ON a.chk_user = b.user_cd")
        sql.AppendLine("LEFT JOIN dbo.m_line AS c WITH (NOLOCK) ON a.line_cd = c.line_cd")
        sql.AppendLine("LEFT JOIN [AvoidMiss_New].dbo.tb_completedata AS h WITH (NOLOCK) ON h.makenumber = a.no AND h.code = REPLACE(a.cd, '-', '')")
        sql.AppendLine("LEFT JOIN dbo.tcm_bianplan AS d WITH (NOLOCK) ON d.productcode = a.cd AND d.zuofan = a.no AND d.b2boderno = a.b2boderno AND d.b2bindexno = a.b2bindexno")
        sql.AppendLine("LEFT JOIN dbo.m_goods_tmp AS g WITH (NOLOCK) ON g.goods_cd = REPLACE(a.cd, '-', '')")

        sql.AppendLine("WHERE m.RowNumber BETWEEN " & startRow & " AND " & endRow)
        sql.AppendLine("ORDER BY m.RowNumber;")

        Return sql.ToString()
    End Function


    '获得 行数 与 明细
    '-----------------------------------------------
    <WebMethod()>
    Public Function GetCntAndMs(ByVal tableName As String, ByVal pageIndex As Integer, ByVal OnePageRowCount As Integer, ByVal rowValues As String, ByVal awaysWhere As String, ByVal maxCnt As Integer) As String

        ''明细
        'Dim sb As New StringBuilder
        'sb.Append(GetTableSql(tableName, rowValues, awaysWhere))
        'sb.AppendLine("AND RowNumber BETWEEN  " & ((pageIndex - 1) * OnePageRowCount + 1).ToString() & " AND " & ((pageIndex) * OnePageRowCount).ToString)
        'Dim ms As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "MS")

        ''行数
        'sb.Length = 0
        'sb.Append(GetTableSql(tableName, rowValues, awaysWhere))
        'Dim sqlCount As New StringBuilder
        'sqlCount.AppendLine("Select count(*) as cnt From (")
        'sqlCount.AppendLine(sb.ToString)
        'sqlCount.AppendLine(") cnt")
        'Dim cnt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sqlCount.ToString(), "CNT")

        Dim sb As New StringBuilder
        'Dim cTblSql As String = GetTableSql(tableName, rowValues, awaysWhere)

        'sb.Append(GetTableSql(tableName, rowValues, awaysWhere))
        'Dim sqlCount As New StringBuilder
        sb.AppendLine("Select count(*) as cnt From (")
        sb.AppendLine(GetTableSql(tableName, rowValues, awaysWhere, True, 0))
        sb.AppendLine(") cnt")
        sb.Append(GetTableSql(tableName, rowValues, awaysWhere, False, maxCnt))
        If pageIndex >= 0 Then
            sb.AppendLine("AND RowNumber BETWEEN  " & ((pageIndex - 1) * OnePageRowCount + 1).ToString() & " AND " & ((pageIndex) * OnePageRowCount).ToString)
        End If
        sb.AppendLine(GetSelWhere(tableName, rowValues))
        Dim orderBy As String = ""
        If tableName = "v_check_result_ms" Then
            orderBy = "ck_id,kind_jun,hyouji_jun"
        ElseIf tableName = "v_A01_result_list" Then
            orderBy = "检查开始时间 desc,生成实际日,cd"
        ElseIf tableName = "v_A02_check_result_ms" Then
            orderBy = "检查日期YMD desc,id,检查顺序"
        ElseIf tableName = "v_A03_mi_check_result" Then
            orderBy = "纳期日 desc"
        ElseIf tableName = "v_A04_result_list" Then
            orderBy = "检查开始时间 desc"
        ElseIf tableName = "v_A05_check_scx" Then
            orderBy = "检查日期 desc"
        ElseIf tableName = "v_A05_check_scx_one" Then
            orderBy = "检查日期 desc,部门 asc"

        ElseIf tableName = "m_sys_join" Then
            orderBy = "sys_id"

        End If
        If orderBy <> "" Then
            sb.Append(" order by " & orderBy)
        End If

        If tableName = "v_A01_result_list" Then
            sb.Clear()
            sb.AppendLine("SELECT ")
            sb.AppendLine(" '1' AS Del ,ROW_NUMBER() OVER (ORDER BY 生成实际日YMD) AS RowNumber,*")
            sb.AppendLine("INTO #v_A01_result_list ")
            sb.AppendLine("FROM v_A01_result_list WHERE 1=1")
            sb.AppendLine(GetSelWhere(tableName, rowValues))
            sb.AppendLine(awaysWhere)
            sb.AppendLine(" order by 检查开始时间 desc")


            sb.AppendLine("SELECT COUNT(*)  as cnt FROM v_A01_result_list  WHERE 1=1")
            sb.AppendLine(GetSelWhere(tableName, rowValues))
            sb.AppendLine(awaysWhere)

            sb.AppendLine("SELECT *")
            sb.AppendLine("FROM #v_A01_result_list ")

            If pageIndex >= 0 Then
                sb.AppendLine("WHERE RowNumber BETWEEN  " & ((pageIndex - 1) * OnePageRowCount + 1).ToString() & " AND " & ((pageIndex) * OnePageRowCount).ToString)
            End If
        ElseIf tableName = "v_A02_check_result_ms" Then
            Dim where As String = GetSelWhere(tableName, rowValues)
            Dim sql As String = Get_v_A02_check_result_msSql_DirectFragment(where, ((pageIndex - 1) * OnePageRowCount + 1).ToString(), ((pageIndex) * OnePageRowCount).ToString)
            sb.Clear()
            sb.AppendLine(sql)
            'sb.AppendLine("SELECT ")
            'sb.AppendLine(" '1' AS Del ,ROW_NUMBER() OVER (ORDER BY 检查日期YMD desc,id desc) AS RowNumber,*")
            'sb.AppendLine("INTO #v_A02_check_result_ms ")
            'sb.AppendLine("FROM v_A02_check_result_ms WHERE 1=1")
            'sb.AppendLine(GetSelWhere(tableName, rowValues))
            'sb.AppendLine(awaysWhere)
            'sb.AppendLine(" order by 检查日期YMD desc")


            'sb.AppendLine("SELECT COUNT(*) as cnt FROM v_A02_check_result_ms  WHERE 1=1")
            'sb.AppendLine(GetSelWhere(tableName, rowValues))
            'sb.AppendLine(awaysWhere)

            'sb.AppendLine("SELECT *")
            'sb.AppendLine("FROM #v_A02_check_result_ms ")

            'If pageIndex >= 0 Then
            '    sb.AppendLine("WHERE RowNumber BETWEEN  " & ((pageIndex - 1) * OnePageRowCount + 1).ToString() & " AND " & ((pageIndex) * OnePageRowCount).ToString)
            'End If


        ElseIf tableName = "v_A03_mi_check_result" Then
            sb.Clear()
            sb.AppendLine("SELECT ")
            sb.AppendLine(" '1' AS Del ,ROW_NUMBER() OVER (ORDER BY 生成实际日YMD) AS RowNumber,*")
            sb.AppendLine("INTO #v_A03_mi_check_result ")
            sb.AppendLine("FROM v_A03_mi_check_result WHERE 1=1")
            sb.AppendLine(GetSelWhere(tableName, rowValues))
            sb.AppendLine(awaysWhere)

            sb.AppendLine("SELECT COUNT(*)  as cnt FROM v_A03_mi_check_result  WHERE 1=1")
            sb.AppendLine(GetSelWhere(tableName, rowValues))
            sb.AppendLine(awaysWhere)

            sb.AppendLine("SELECT *")
            sb.AppendLine("FROM #v_A03_mi_check_result ")

            If pageIndex >= 0 Then
                sb.AppendLine("WHERE RowNumber BETWEEN  " & ((pageIndex - 1) * OnePageRowCount + 1).ToString() & " AND " & ((pageIndex) * OnePageRowCount).ToString)
            End If


        ElseIf tableName = "v_A05_check_scx" Then
            sb.Clear()
            sb.AppendLine("SELECT ")
            sb.AppendLine(" '1' AS Del ,ROW_NUMBER() OVER (ORDER BY 检查者) AS RowNumber,*")
            sb.AppendLine("INTO #v_A05_check_scx ")
            sb.AppendLine("FROM v_A05_check_scx WHERE 1=1")
            sb.AppendLine(GetSelWhere(tableName, rowValues))
            sb.AppendLine(awaysWhere)

            sb.AppendLine("SELECT COUNT(*)  as cnt FROM v_A05_check_scx  WHERE 1=1")
            sb.AppendLine(GetSelWhere(tableName, rowValues))
            sb.AppendLine(awaysWhere)

            sb.AppendLine("SELECT *")
            sb.AppendLine("FROM #v_A05_check_scx ")

            If pageIndex >= 0 Then
                sb.AppendLine("WHERE RowNumber BETWEEN  " & ((pageIndex - 1) * OnePageRowCount + 1).ToString() & " AND " & ((pageIndex) * OnePageRowCount).ToString)
            End If


        End If


        Dim ds As DataSet = FillDataDs(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
        ds.Tables(0).TableName = "CNT"
        ds.Tables(1).TableName = "MS"
        sb.Length = 0
        '返回 行数 与 明细 的 DS
        'Dim ds As New DataSet
        'ds.Tables.Add(cnt)
        'ds.Tables.Add(ms)
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer

        Return JsonConvert.SerializeObject(ds)

    End Function
    '更新
    <WebMethod()>
    Public Function UpdTableJson(ByVal tableName As String, ByVal rowValues As String, ByVal user As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer

        If rowValues.Trim = "" Then
            Return jss.Serialize("没有要更新的数据")
        End If

        Dim oldTableName As String = tableName
        If tableName = "t_check_v" Then
            tableName = "t_check"
        End If



        Dim dtCols As DataTable = GetTableInfoDt(tableName)
        Dim rowsValues As String() = rowValues.Split(ConRowSeparator)
        Dim isHaveId As Boolean = dtCols.Select("columns_name_en ='id'").Count > 0




        '删除数组中的 chk_user_name 项目值
        If oldTableName = "t_check_v" Then

            Dim k As Integer


            For j As Integer = 0 To dtCols.Rows.Count - 1
                Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                If colEnName = "chk_user" Then
                    k = j
                    Exit For
                End If
            Next

            For i As Integer = 0 To rowsValues.Count - 1
                Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)
                cellsValue = cellsValue.Where(Function(val, idx) idx <> k + 1).ToArray()
                rowsValues(i) = String.Join(ConColumnSeparator, cellsValue)
            Next

        End If

        If tableName = "t_check_ms" Then
            isHaveId = False
        End If

        Dim sql As New StringBuilder


        For i As Integer = 0 To rowsValues.Count - 1

            Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)

            sql.AppendLine("Update " & tableName)
            sql.AppendLine("Set ")

            Dim kama As String = ""
            For j As Integer = 0 To dtCols.Rows.Count - 1
                Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()

                If isHaveId Then
                    If colEnName <> "id" Then
                        If colEnName = "upd_user" Then
                            sql.AppendLine(kama & colEnName & "='" & user & "'")
                        ElseIf colEnName = "edit_user" Then
                            sql.AppendLine(kama & colEnName & "='" & user & "'")
                        ElseIf colEnName = "upd_date" Then
                            sql.AppendLine(kama & colEnName & "=Getdate()")
                        ElseIf colEnName.Contains("_date") Then
                            If cellsValue(j) = "" Then
                                sql.AppendLine(kama & colEnName & "=null")
                            Else
                                sql.AppendLine(kama & colEnName & "=N'" & cellsValue(j) & "'")
                            End If
                        Else
                            sql.AppendLine(kama & colEnName & "=N'" & cellsValue(j) & "'")
                        End If
                        If kama = "" Then kama = ","
                    End If
                Else
                    If dtCols.Rows(j).Item("pk") <> "P" AndAlso colEnName <> "ins_user" AndAlso colEnName <> "ins_date" Then
                        If colEnName = "upd_user" Then
                            sql.AppendLine(kama & colEnName & "='" & user & "'")
                        ElseIf colEnName = "edit_user" Then
                            sql.AppendLine(kama & colEnName & "='" & user & "'")
                        ElseIf colEnName = "upd_date" Then
                            sql.AppendLine(kama & colEnName & "=Getdate()")
                        ElseIf colEnName.Contains("_date") Then
                            If cellsValue(j) = "" Then
                                sql.AppendLine(kama & colEnName & "=null")
                            Else
                                sql.AppendLine(kama & colEnName & "=N'" & cellsValue(j) & "'")
                            End If
                        Else
                            sql.AppendLine(kama & colEnName & "=N'" & cellsValue(j) & "'")
                        End If
                        If kama = "" Then kama = ","
                    End If
                End If




            Next

            sql.AppendLine("Where 1=1")

            Dim isHaveKey As Boolean = False

            If isHaveId Then
                sql.AppendLine("AND id='" & cellsValue(0) & "'")
                isHaveKey = True
            Else
                For j As Integer = 0 To dtCols.Rows.Count - 1
                    If dtCols.Rows(j).Item("pk") = "P" Then
                        sql.AppendLine("AND " & dtCols.Rows(j).Item("columns_name_en").ToString() & "='" & cellsValue(j) & "'")
                        isHaveKey = True
                    End If
                Next
            End If



            If Not isHaveKey Then
                Return jss.Serialize("没有Key 不能更新")
            End If
        Next




        'sql.AppendLine("Select getdate()")



        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sql.ToString())
        Return jss.Serialize("OK:" & rowsValues.Count & "件更新")
    End Function
    '登录
    <WebMethod()>
    Public Function InsTableJson(ByVal tableName As String, ByVal rowValues As String, ByVal user As String) As String

        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        If rowValues.Trim = "" Then
            Return jss.Serialize("没有要更新的数据")
        End If

        Dim oldTableName As String = tableName
        If tableName = "t_check_v" Then
            tableName = "t_check"
        End If

        Dim dtCols As DataTable = GetTableInfoDt(tableName)
        Dim rowsValues As String() = rowValues.Split(ConRowSeparator)
        Dim isHaveId As Boolean = dtCols.Select("columns_name_en ='id'").Count > 0
        'columns_name_en
        '删除数组中的 chk_user_name 项目值
        If oldTableName = "t_check_v" Then

            Dim k As Integer


            For j As Integer = 0 To dtCols.Rows.Count - 1
                Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                If colEnName = "chk_user" Then
                    k = j
                    Exit For
                End If
            Next

            For i As Integer = 0 To rowsValues.Count - 1
                Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)
                cellsValue = cellsValue.Where(Function(val, idx) idx <> k + 1).ToArray()
                rowsValues(i) = String.Join(ConColumnSeparator, cellsValue)
            Next

        End If

        Dim sql As New StringBuilder

        Dim orStr As String = ""
        '检查主键重复
        For i As Integer = 0 To rowsValues.Count - 1
            Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)
            If i = 0 Then

                If isHaveId Then    '如果有id行
                    sql.AppendLine("SELECT id FROM " & tableName & " WHERE ")
                Else
                    sql.AppendLine("SELECT ")
                    Dim tmpKama As String = ""
                    For j As Integer = 0 To dtCols.Rows.Count - 1
                        If dtCols.Rows(j).Item("pk") = "P" Then
                            Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                            sql.AppendLine(tmpKama & colEnName)
                            tmpKama = ","
                        End If
                    Next
                    sql.AppendLine("FROM " & tableName & " WHERE ")
                End If

            End If

            sql.AppendLine(orStr & "(")
            Dim tmpAnd As String = ""

            If isHaveId > 0 Then
                'id
                sql.AppendLine("id=" & cellsValue(0) & "")
            Else
                For j As Integer = 0 To dtCols.Rows.Count - 1
                    Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                    If dtCols.Rows(j).Item("pk") = "P" Then
                        sql.AppendLine(tmpAnd & colEnName & "=N'" & cellsValue(j) & "'")
                        tmpAnd = " AND "
                    End If
                Next
            End If

            sql.AppendLine(")")

            orStr = " OR "
        Next

        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sql.ToString(), "ms")
        If dt.Rows.Count > 0 Then
            Dim sb As New StringBuilder
            sb.AppendLine("主键重复(数据库中已经存在下列数据)" & "<br>")

            For i As Integer = 0 To dt.Rows.Count - 1
                For j As Integer = 0 To dt.Columns.Count - 1
                    sb.Append(" " & dt.Columns(j).ColumnName & " " & dt.Rows(i).Item(j).ToString)
                Next
                sb.Append("<br>")
            Next

            Dim myEx As New Exception((sb.ToString))
            Throw myEx

            'Return jss.Serialize(sb.ToString)

        End If


        sql.Length = 0

        Dim startColIdx As Integer
        If isHaveId Then
            startColIdx = 1
        Else
            startColIdx = 0
        End If

        For i As Integer = 0 To rowsValues.Count - 1

            Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)

            sql.AppendLine("INSERT INTO " & tableName & "(")

            Dim kama As String = ""
            For j As Integer = startColIdx To dtCols.Rows.Count - 1
                Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                sql.AppendLine(kama & colEnName)
                If kama = "" Then kama = ","
            Next


            sql.AppendLine(")VALUES( ")
            kama = ""
            For j As Integer = startColIdx To dtCols.Rows.Count - 1

                Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                'sql.AppendLine(cellsValue(j ))
                If colEnName = "upd_user" Or colEnName = "ins_user" Then
                    sql.AppendLine(kama & "'" & user & "'")
                ElseIf colEnName = "upd_date" Or colEnName = "ins_date" Then
                    sql.AppendLine(kama & "Getdate()")
                Else
                    sql.AppendLine(kama & "N'" & cellsValue(j) & "'")
                End If

                If kama = "" Then kama = ","
            Next
            sql.AppendLine(")")
        Next


        'sql.AppendLine("Select getdate()")

        Try
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sql.ToString())
            Return jss.Serialize("OK:" & rowsValues.Count & "件登录，")
            Return jss.Serialize("OK")
        Catch ex As Exception

            Dim myEx As New Exception((ex.Message))
            Throw myEx

            'If ex.Message.ToString.Contains("PRIMARY KEY") Then
            '    Return jss.Serialize("数据重复！（PRIMARY KEY）")
            'Else
            '    Return jss.Serialize(ex.Message)
            'End If

        End Try





        'Dim dtCols As DataTable = GetTableInfoDt(tableName)
        'Dim kama As String = ""
        'Dim orderBy As String = ""
        'For i As Integer = 0 To dtCols.Rows.Count - 1
        '    If dtCols.Rows(i).Item("pk") IsNot DBNull.Value AndAlso dtCols.Rows(i).Item("pk").ToString() = "P" Then
        '        orderBy = orderBy & kama & dtCols.Rows(i).Item("columns_name_en").ToString()
        '        kama = ","
        '    End If
        'Next



        'Return Dtb2Json(dtRows)

    End Function
    '删除 本页
    <WebMethod()>
    Public Function DelThisPageDataJson(ByVal tableName As String, ByVal rowValues As String, ByVal user As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer

        If rowValues.Trim = "" Then
            Return jss.Serialize("没有要更新的数据")
        End If
        Dim oldTableName As String = tableName
        If tableName = "t_check_v" Then
            tableName = "t_check"
        End If

        Dim dtCols As DataTable = GetTableInfoDt(tableName)
        Dim rowsValues As String() = rowValues.Split(ConRowSeparator)

        '删除数组中的 chk_user_name 项目值
        If oldTableName = "t_check_v" Then

            Dim k As Integer


            For j As Integer = 0 To dtCols.Rows.Count - 1
                Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                If colEnName = "chk_user" Then
                    k = j
                    Exit For
                End If
            Next

            For i As Integer = 0 To rowsValues.Count - 1
                Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)
                cellsValue = cellsValue.Where(Function(val, idx) idx <> k + 1).ToArray()
                rowsValues(i) = String.Join(ConColumnSeparator, cellsValue)
            Next

        End If

        Dim sql As New StringBuilder


        For i As Integer = 0 To rowsValues.Count - 1

            Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)

            If tableName = "t_check" Then
                sql.AppendLine("DELETE FROM t_check_ms WHERE ck_id in (")
                sql.AppendLine("SELECT ck_id FROM t_check")
                sql.AppendLine("Where 1=1")

                For j As Integer = 0 To dtCols.Rows.Count - 1
                    If dtCols.Rows(j).Item("pk") = "P" Then
                        If dtCols.Rows(j).Item("columns_type") = "datetime" Then
                            sql.AppendLine("AND CONVERT(varchar(100), " & dtCols.Rows(j).Item("columns_name_en").ToString() & ", 23)='" & CDate(cellsValue(j)).ToString("yyyy-MM-dd") & "'")
                        Else
                            sql.AppendLine("AND " & dtCols.Rows(j).Item("columns_name_en").ToString() & "='" & cellsValue(j) & "'")
                        End If
                    End If
                Next
                sql.AppendLine(")")
            End If


            sql.AppendLine("DELETE FROM " & tableName)
            sql.AppendLine("Where 1=1")
            Dim isHaveKey As Boolean = False
            For j As Integer = 0 To dtCols.Rows.Count - 1
                If dtCols.Rows(j).Item("pk") = "P" Then
                    If dtCols.Rows(j).Item("columns_type") = "datetime" Then
                        sql.AppendLine("AND CONVERT(varchar(100), " & dtCols.Rows(j).Item("columns_name_en").ToString() & ", 23)='" & CDate(cellsValue(j)).ToString("yyyy-MM-dd") & "'")
                    Else
                        sql.AppendLine("AND " & dtCols.Rows(j).Item("columns_name_en").ToString() & "='" & cellsValue(j) & "'")
                    End If
                    isHaveKey = True
                End If
            Next

            If Not isHaveKey Then
                Return jss.Serialize("没有Key 不能删除")
            End If
        Next



        'sql.AppendLine("Select getdate()")



        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sql.ToString())
        Return jss.Serialize("OK")

        Return ""
    End Function
    '删除 行
    <WebMethod()>
    Public Function DelTableJson(ByVal tableName As String, ByVal rowValues As String, ByVal user As String) As String

        Dim oldTableName As String = tableName
        If tableName = "t_check_v" Then
            tableName = "t_check"
        End If

        Dim dtCols As DataTable = GetTableInfoDt(tableName)

        Dim cellsValue As String() = rowValues.Split(ConColumnSeparator)


        '删除数组中的 chk_user_name 项目值
        If oldTableName = "t_check_v" Then

            Dim k As Integer


            For j As Integer = 0 To dtCols.Rows.Count - 1
                Dim colEnName As String = dtCols.Rows(j).Item("columns_name_en").ToString()
                If colEnName = "chk_user" Then
                    k = j
                    Exit For
                End If
            Next

            'For i As Integer = 0 To rowsValues.Count - 1
            'Dim cellsValue As String() = rowsValues(i).Split(ConColumnSeparator)
            cellsValue = cellsValue.Where(Function(val, idx) idx <> k + 1).ToArray()
            'rowsValues(i) = String.Join(ConColumnSeparator, cellsValue)
            'Next

        End If

        Dim sql As New StringBuilder

        If tableName = "t_check" Then
            sql.AppendLine("DELETE FROM t_check_ms WHERE ck_id in (")
            sql.AppendLine("SELECT ck_id FROM t_check")
            sql.AppendLine("Where 1=1")

            For j As Integer = 0 To dtCols.Rows.Count - 1
                If dtCols.Rows(j).Item("pk") = "P" Then
                    If dtCols.Rows(j).Item("columns_type") = "datetime" Then
                        sql.AppendLine("AND CONVERT(varchar(100), " & dtCols.Rows(j).Item("columns_name_en").ToString() & ", 23)='" & CDate(cellsValue(j)).ToString("yyyy-MM-dd") & "'")
                    Else
                        sql.AppendLine("AND " & dtCols.Rows(j).Item("columns_name_en").ToString() & "='" & cellsValue(j) & "'")
                    End If
                End If
            Next
            sql.AppendLine(")")
        End If
        sql.AppendLine("Delete from " & tableName)
        sql.AppendLine("Where 1=1")

        Dim isHaveKey As Boolean = False
        For j As Integer = 0 To dtCols.Rows.Count - 1
            If dtCols.Rows(j).Item("pk") = "P" Then
                If dtCols.Rows(j).Item("columns_type") = "datetime" Then
                    sql.AppendLine("AND CONVERT(varchar(100), " & dtCols.Rows(j).Item("columns_name_en").ToString() & ", 23)='" & CDate(cellsValue(j)).ToString("yyyy-MM-dd") & "'")
                Else
                    sql.AppendLine("AND " & dtCols.Rows(j).Item("columns_name_en").ToString() & "='" & cellsValue(j) & "'")
                End If

                isHaveKey = True
            End If
        Next

        sql.AppendLine("Select getdate()")
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer

        If isHaveKey Then

            FillData(DataAccessManager.ConnStr, CommandType.Text, sql.ToString(), "ms")
            Return jss.Serialize("OK")
        Else
            Return jss.Serialize("没有Key 不能删除")
        End If

    End Function

    '优化后未使用
    <WebMethod()>
    Public Function GetTableMsJson(ByVal tableName As String, ByVal pageIndex As Integer, ByVal OnePageRowCount As Integer, ByVal rowValues As String, ByVal awaysWhere As String) As String

        Dim sb As New StringBuilder
        sb.Append(GetTableSql(tableName, rowValues, awaysWhere, False, 0))
        sb.AppendLine("AND RowNumber BETWEEN  " & ((pageIndex - 1) * OnePageRowCount + 1).ToString() & " AND " & ((pageIndex) * OnePageRowCount).ToString)
        'sb.AppendLine(GetSelWhere(tableName, rowValues))
        'Try
        Dim dtRows As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetTableMsJson")

        Return Dtb2Json(dtRows)
        'Catch ex As Exception
        '    Throw New Exception(ex.Message)
        'End Try


    End Function
    <WebMethod()>
    Public Function GetTableCountJson(ByVal tableName As String, ByVal rowValues As String, ByVal awaysWhere As String) As String
        Dim sb As New StringBuilder
        sb.Append(GetTableSql(tableName, rowValues, awaysWhere, False, 0))
        'sb.AppendLine(GetSelWhere(tableName, rowValues))

        Dim sqlCount As New StringBuilder
        sqlCount.AppendLine("Select count(*) From (")
        sqlCount.AppendLine(sb.ToString)
        sqlCount.AppendLine(") cnt")

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sqlCount.ToString(), "dtAllPageCount").Rows(0).Item(0)
    End Function


#End Region

    'WHERE
    Private Function GetSelWhere(ByVal tableName As String, ByVal rowValues As String) As String
        If rowValues.Trim = "" Then Return ""

        Dim dtCols As DataTable = GetTableInfoDt(tableName)
        Dim cellsValue As String() = rowValues.Split(ConColumnSeparator)
        Dim sql As New StringBuilder
        Dim st As String
        Dim ed As String
        Dim colEnName As String
        For j As Integer = 0 To dtCols.Rows.Count - 1
            colEnName = dtCols.Rows(j).Item("columns_name_en").ToString()

            If colEnName = "upd_date" Or colEnName = "ins_date" Or colEnName = "yotei_chk_date" Or colEnName = "chk_start_date" Or colEnName = "chk_end_date" _
                Or colEnName = "执行检查日" _
                Or colEnName = "生成实际日" _
                      Or colEnName = "检查日期" _
                Or colEnName = "检查日期YMD" _
                Or colEnName = "检查开始时间" _
                Or colEnName = "检查终了时间" _
                Or colEnName = "生成实际日YMD" _
                Or colEnName = "纳期日" Then
                st = cellsValue(j).Split("|"c)(0).Trim
                ed = cellsValue(j).Split("|"c)(1).Trim
                If st <> "" Or ed <> "" Then
                    If st = "" Then
                        st = "1999-01-01"
                    End If
                    If ed = "" Then
                        ed = "2999-01-01"
                    End If
                    '
                    If colEnName.Contains("YMD") OrElse st.Contains(":") OrElse ed.Contains(":") OrElse colEnName = "检查日期" Then
                        sql.AppendLine("AND " & colEnName & " BETWEEN  '" & st & "' AND '" & ed & "'")
                    Else
                        'st = CDate(st).AddMilliseconds(-1).ToString("yyyy-MM-dd")
                        st = CDate(st).ToString("yyyy-MM-dd")
                        sql.AppendLine("AND " & colEnName & " BETWEEN  '" & st & " 00:00:00' AND '" & ed & " 23:59:59'")
                    End If

                End If

            Else
                If cellsValue(j) <> "" Then
                    If cellsValue(j).Contains("%") OrElse cellsValue(j).Contains("_") Then
                        sql.AppendLine("AND " & colEnName & " like N'" & cellsValue(j) & "'")
                    ElseIf cellsValue(j).Contains("*") Then
                        sql.AppendLine("AND " & colEnName & " like N'%" & cellsValue(j).Replace("*", "") & "%'")
                    Else
                        sql.AppendLine("AND " & colEnName & " = N'" & cellsValue(j) & "'")
                    End If

                End If

            End If
        Next
        Return sql.ToString
    End Function


#Region "共通函数"


    '获得 Datatable Json
    '-----------------------------------------------
    Public Function Dtb2Json(ByVal dtb As DataTable) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Dim dic As ArrayList = New ArrayList()
        For Each item As DataRow In dtb.Rows
            Dim drow As Dictionary(Of String, Object) = New Dictionary(Of String, Object)()
            For Each col As DataColumn In dtb.Columns
                drow.Add(col.ColumnName, item.Item(col.ColumnName))
            Next
            dic.Add(drow)
        Next
        Return jss.Serialize(dic)
    End Function

    '表结构信息取得JSON
    '-----------------------------------------------
    <WebMethod()>
    Public Function GetTableInfoJson(ByVal tableName As String) As String
        'Try
        'Return JsonConvert.SerializeObject(GetTableInfoDt(tableName))

        Return Dtb2Json(GetTableInfoDt(tableName))
        'Catch ex As Exception
        'Return "SYSTEM_ERROR:" & ex.Message
        'End Try

        '返回Dataset
        'https://blog.csdn.net/longchenghui20/article/details/77045426
        'JsonConvert.SerializeObject()

    End Function
#End Region

End Class
