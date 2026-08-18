<%@ WebHandler Language="VB" Class="ChkListHandler" %>

Imports System.Web
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports System.Web.Script.Serialization
Imports SqlHelper.SqlHelper
Imports SqlHelper

Public Class ChkListHandler
    Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "application/json; charset=utf-8"
        context.Response.Charset = "utf-8"

        Dim jss As New JavaScriptSerializer()

        ' 动作区分：search=检索  delete=删除
        Dim action As String = context.Request.Form("action")
        If action = "delete" Then
            DeleteChk(context, jss)
            Return
        End If

        ' 检索条件（空则不作为条件） 预定检查日/开始检查日 各支持 开始~结束 范围
        Dim yotei_chk_date_from As String = context.Request.Form("yotei_chk_date_from")
        Dim yotei_chk_date_to As String = context.Request.Form("yotei_chk_date_to")
        Dim chk_start_date_from As String = context.Request.Form("chk_start_date_from")
        Dim chk_start_date_to As String = context.Request.Form("chk_start_date_to")

        ' 分页
        Dim page_index As Integer = 1
        Dim page_size As Integer = 20
        Integer.TryParse(context.Request.Form("page_index"), page_index)
        Integer.TryParse(context.Request.Form("page_size"), page_size)
        If page_index < 1 Then page_index = 1
        If page_size < 1 Then page_size = 20
        If page_size > 200 Then page_size = 200

        Try
            Dim sb As New StringBuilder()
            sb.AppendLine(";WITH CTE AS (")
            sb.AppendLine("    SELECT")
            sb.AppendLine("        ck_id")
            sb.AppendLine("        ,REPLACE(cd, '-', '') AS cd")
            sb.AppendLine("        ,no")
            sb.AppendLine("        ,t_check.department_cd")
            sb.AppendLine("        ,t_check.line_cd")
            sb.AppendLine("        ,t_check.chk_user")
            sb.AppendLine("        ,CONVERT(varchar(23), t_check.yotei_chk_date, 121) AS yotei_chk_date")
            sb.AppendLine("        ,CONVERT(varchar(23), t_check.chk_start_date, 121) AS chk_start_date")
            sb.AppendLine("        ,CONVERT(varchar(23), t_check.chk_end_date, 121) AS chk_end_date")
            sb.AppendLine("        ,t_check.status")
            sb.AppendLine("        ,t_check.result")
            sb.AppendLine("        ,t_check.chk_times")
            sb.AppendLine("        ,t_check.suu")
            sb.AppendLine("        ,t_check.del_flg")
            sb.AppendLine("        ,t_check.qianpin")
            sb.AppendLine("        ,t_check.tools_scan_flg")
            sb.AppendLine("        ,t_check.shared_ck_id")
            sb.AppendLine("        ,t_check.h")
            sb.AppendLine("        ,t_check.w")
            sb.AppendLine("        ,t_check.dh")
            sb.AppendLine("        ,t_check.dw")
            sb.AppendLine("        ,t_check.sw")
            sb.AppendLine("        ,t_check.kw")
            sb.AppendLine("        ,t_check.specialBookNo")
            sb.AppendLine("        ,t_check.b2bOderNo")
            sb.AppendLine("        ,t_check.b2bIndexNo")
            sb.AppendLine("        ,t_check.sapOderNo")
            sb.AppendLine("        ,t_check.sapIndexNo")
            sb.AppendLine("        ,t_check.edit_user")
            sb.AppendLine("        ,ISNULL(mu.user_name, t_check.upd_user) AS upd_user")
            sb.AppendLine("        ,CONVERT(varchar(23), t_check.upd_date, 121) AS upd_date")
            sb.AppendLine("        ,t_check.ins_user")
            sb.AppendLine("        ,CONVERT(varchar(23), t_check.ins_date, 121) AS ins_date")
            sb.AppendLine("        ,ROW_NUMBER() OVER(PARTITION BY REPLACE(cd, '-', ''), no ORDER BY ck_id ASC) AS rn")
            sb.AppendLine("    FROM t_check")
            sb.AppendLine("    LEFT JOIN m_user mu ON t_check.upd_user = mu.user_cd")
            sb.AppendLine("    WHERE t_check.[status] = '0' AND t_check.[result] = '待' and isnull(t_check.no,'')<>''")

            ' 检索条件做成（from/to 范围）
            Dim cond As New StringBuilder()
            Dim paramList As New List(Of SqlParameter)
            If yotei_chk_date_from.Trim() <> "" Then
                cond.AppendLine("    AND CONVERT(varchar(10), t_check.yotei_chk_date, 120) >= @yotei_chk_date_from")
                paramList.Add(New SqlParameter("@yotei_chk_date_from", yotei_chk_date_from.Trim()))
            End If
            If yotei_chk_date_to.Trim() <> "" Then
                cond.AppendLine("    AND CONVERT(varchar(10), t_check.yotei_chk_date, 120) <= @yotei_chk_date_to")
                paramList.Add(New SqlParameter("@yotei_chk_date_to", yotei_chk_date_to.Trim()))
            End If
            If chk_start_date_from.Trim() <> "" Then
                cond.AppendLine("    AND CONVERT(varchar(10), t_check.chk_start_date, 120) >= @chk_start_date_from")
                paramList.Add(New SqlParameter("@chk_start_date_from", chk_start_date_from.Trim()))
            End If
            If chk_start_date_to.Trim() <> "" Then
                cond.AppendLine("    AND CONVERT(varchar(10), t_check.chk_start_date, 120) <= @chk_start_date_to")
                paramList.Add(New SqlParameter("@chk_start_date_to", chk_start_date_to.Trim()))
            End If

            ' 分页参数
            paramList.Add(New SqlParameter("@offset", (page_index - 1) * page_size))
            paramList.Add(New SqlParameter("@page_size", page_size))

            sb.Append(cond.ToString())
            sb.AppendLine(")")
            sb.AppendLine("SELECT * FROM CTE WHERE rn = 1")
            sb.AppendLine("ORDER BY no")
            sb.AppendLine("OFFSET @offset ROWS FETCH NEXT @page_size ROWS ONLY")

            ' 总数查询参数（不含分页）
            Dim countParamList As New List(Of SqlParameter)
            If yotei_chk_date_from.Trim() <> "" Then
                countParamList.Add(New SqlParameter("@yotei_chk_date_from", yotei_chk_date_from.Trim()))
            End If
            If yotei_chk_date_to.Trim() <> "" Then
                countParamList.Add(New SqlParameter("@yotei_chk_date_to", yotei_chk_date_to.Trim()))
            End If
            If chk_start_date_from.Trim() <> "" Then
                countParamList.Add(New SqlParameter("@chk_start_date_from", chk_start_date_from.Trim()))
            End If
            If chk_start_date_to.Trim() <> "" Then
                countParamList.Add(New SqlParameter("@chk_start_date_to", chk_start_date_to.Trim()))
            End If

            Dim sbCount As New StringBuilder()
            sbCount.AppendLine(";WITH CTE AS (")
            sbCount.AppendLine("    SELECT")
            sbCount.AppendLine("        REPLACE(t_check.cd, '-', '') AS cd")
            sbCount.AppendLine("        ,t_check.no")
            sbCount.AppendLine("        ,ROW_NUMBER() OVER(PARTITION BY REPLACE(t_check.cd, '-', ''), t_check.no ORDER BY t_check.ck_id DESC) AS rn")
            sbCount.AppendLine("    FROM t_check")
            sbCount.AppendLine("    WHERE t_check.[status] = '0' AND t_check.[result] = '待' and isnull(t_check.no,'')<>''")
            sbCount.Append(cond.ToString())
            sbCount.AppendLine(")")
            sbCount.AppendLine("SELECT COUNT(*) AS cnt FROM CTE")

            Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "list", paramList.ToArray())

            Dim total As Integer = 0
            Dim dtCnt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sbCount.ToString(), "cnt", countParamList.ToArray())
            If dtCnt.Rows.Count > 0 Then
                total = Convert.ToInt32(dtCnt.Rows(0).Item("cnt"))
            End If

            ' 数据转 JSON
            Dim rowList As New List(Of Dictionary(Of String, Object))()
            For Each row As DataRow In dt.Rows
                Dim rowDict As New Dictionary(Of String, Object)()
                For Each col As DataColumn In dt.Columns
                    rowDict.Add(col.ColumnName, row.Item(col.ColumnName))
                Next
                rowList.Add(rowDict)
            Next

            Dim response As New Dictionary(Of String, Object)()
            response.Add("success", True)
            response.Add("total", total)
            response.Add("page_index", page_index)
            response.Add("page_size", page_size)
            response.Add("data", rowList)

            context.Response.Write(jss.Serialize(response))

        Catch ex As Exception
            Dim errResponse As New Dictionary(Of String, Object)()
            errResponse.Add("success", False)
            errResponse.Add("message", ex.Message)
            context.Response.Write(jss.Serialize(errResponse))
        End Try

    End Sub

    ''' <summary>
    ''' 删除指定 ck_id 的检查数据（t_check 和 t_check_ms）
    ''' </summary>
    Private Sub DeleteChk(ByVal context As HttpContext, ByVal jss As JavaScriptSerializer)

        Dim ck_id As String = context.Request.Form("ck_id")

        If ck_id.Trim() = "" Then
            Dim errResp As New Dictionary(Of String, Object)()
            errResp.Add("success", False)
            errResp.Add("message", "ck_id 不能为空")
            context.Response.Write(jss.Serialize(errResp))
            Return
        End If

        Try
            Dim sb As New StringBuilder()
            ' 先删子表 t_check_ms，再删主表 t_check
            sb.AppendLine("DELETE FROM t_check_ms WHERE ck_id = '" & ck_id.Replace("'", "''") & "'")
            sb.AppendLine("DELETE FROM t_check WHERE ck_id = '" & ck_id.Replace("'", "''") & "'")
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())

            Dim resp As New Dictionary(Of String, Object)()
            resp.Add("success", True)
            resp.Add("message", "删除成功")
            context.Response.Write(jss.Serialize(resp))

        Catch ex As Exception
            Dim errResp As New Dictionary(Of String, Object)()
            errResp.Add("success", False)
            errResp.Add("message", ex.Message)
            context.Response.Write(jss.Serialize(errResp))
        End Try

    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
