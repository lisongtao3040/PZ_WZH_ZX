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
Public Class CheckApi
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' 根据传入的no（多个用逗号分隔）查询t_check表的result结果
    ''' </summary>
    ''' <param name="nos">多个no，用逗号分隔，例如 "NO001,NO002,NO003"</param>
    ''' <returns>JSON格式的查询结果</returns>
    <WebMethod()>
    Public Function GetCheckResult(ByVal nos As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer

        If String.IsNullOrWhiteSpace(nos) Then
            Dim errResp As New Dictionary(Of String, Object)
            errResp.Add("success", False)
            errResp.Add("message", "nos is empty")
            Return jss.Serialize(errResp)
        End If

        ' 将逗号分隔的no拆分为数组，并过滤空值
        Dim noList As String() = nos.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)
        If noList.Length = 0 Then
            Dim errResp As New Dictionary(Of String, Object)
            errResp.Add("success", False)
            errResp.Add("message", "nos format error")
            Return jss.Serialize(errResp)
        End If

        ' 构建IN查询条件
        Dim inClause As New StringBuilder()
        For i As Integer = 0 To noList.Length - 1
            If i > 0 Then
                inClause.Append(",")
            End If
            inClause.Append("'" & noList(i).Trim().Replace("'", "''") & "'")
        Next


        Dim sb As New StringBuilder()
        sb.AppendLine("WITH CTE AS (")
        sb.AppendLine("    SELECT")
        sb.AppendLine("        ck_id")
        sb.AppendLine("        ,REPLACE(cd, '-', '') AS cd")
        sb.AppendLine("        ,no")
        sb.AppendLine("        ,department_cd")
        sb.AppendLine("        ,line_cd")
        sb.AppendLine("        ,chk_user")
        sb.AppendLine("        ,CONVERT(varchar(23), yotei_chk_date, 121) AS yotei_chk_date")
        sb.AppendLine("        ,CONVERT(varchar(23), chk_start_date, 121) AS chk_start_date")
        sb.AppendLine("        ,CONVERT(varchar(23), chk_end_date, 121) AS chk_end_date")
        sb.AppendLine("        ,status")
        sb.AppendLine("        ,result")
        sb.AppendLine("        ,chk_times")
        sb.AppendLine("        ,suu")
        sb.AppendLine("        ,del_flg")
        sb.AppendLine("        ,qianpin")
        sb.AppendLine("        ,tools_scan_flg")
        sb.AppendLine("        ,shared_ck_id")
        sb.AppendLine("        ,h")
        sb.AppendLine("        ,w")
        sb.AppendLine("        ,dh")
        sb.AppendLine("        ,dw")
        sb.AppendLine("        ,sw")
        sb.AppendLine("        ,kw")
        sb.AppendLine("        ,specialBookNo")
        sb.AppendLine("        ,b2bOderNo")
        sb.AppendLine("        ,b2bIndexNo")
        sb.AppendLine("        ,sapOderNo")
        sb.AppendLine("        ,sapIndexNo")
        sb.AppendLine("        ,edit_user")
        sb.AppendLine("        ,upd_user")
        sb.AppendLine("        ,CONVERT(varchar(23), upd_date, 121) AS upd_date")
        sb.AppendLine("        ,ins_user")
        sb.AppendLine("        ,CONVERT(varchar(23), ins_date, 121) AS ins_date")
        sb.AppendLine("        ,ROW_NUMBER() OVER(PARTITION BY REPLACE(cd, '-', ''), no ORDER BY ck_id DESC) AS rn")
        sb.AppendLine("    FROM t_check")
        sb.AppendLine("    WHERE no IN (" & inClause.ToString() & ")")
        sb.AppendLine(")")
        sb.AppendLine("SELECT * FROM CTE WHERE rn = 1")
        sb.AppendLine("ORDER BY no")

        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "t_check")

        ' 转换为JSON
        Dim resultList As New List(Of Dictionary(Of String, Object))()
        For Each row As DataRow In dt.Rows
            Dim rowDict As New Dictionary(Of String, Object)()
            For Each col As DataColumn In dt.Columns
                rowDict.Add(col.ColumnName, row.Item(col.ColumnName))
            Next
            resultList.Add(rowDict)
        Next

        Dim response As New Dictionary(Of String, Object)()
        response.Add("success", True)
        response.Add("total", resultList.Count)
        response.Add("data", resultList)

        Return jss.Serialize(response)
    End Function

End Class