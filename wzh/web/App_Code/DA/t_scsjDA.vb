Imports System.Data
Imports Microsoft.VisualBasic
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.CodeDom.Compiler
Imports MSScriptControl

Public Class t_scsjDA

    Public Function GetScsj(ByVal department_cd As String, ByVal startDate As String, ByVal endDate As String) As DataTable

        'SQLコメント
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("      Convert(varchar(100), isnull(c.yotei_chk_date,t.yotei_chk_date) , 11) as yotei_chk_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) as chk_start_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) as chk_end_date")

        sb.AppendLine("")
        sb.AppendLine("      ,b.user_name")
        sb.AppendLine("      ,ISNULL(ms.mark,'') mark")
        sb.AppendLine("      ,ISNULL(c.DestinationCode,'') xiangxian")
        sb.AppendLine("      ,isnull(c.sapOderNo,t.sapOderNo) sapOderNo")
        sb.AppendLine("      ,isnull(c.sapIndexNo,t.sapIndexNo) sapIndexNo")
        sb.AppendLine("      ,b.user_name")
        sb.AppendLine("      ,isnull(c.specialBookNo,t.specialBookNo) specialBookNo")
        sb.AppendLine("      ,isnull(c.jxs_name,'') jxs_name")
        sb.AppendLine("		  ,CASE WHEN ISNULL(t.no,'') = '' THEN 'NG 不良代替 未重检'")
        sb.AppendLine("		        WHEN ISNULL(t.result,'') = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中'")

        sb.AppendLine("		  END buliang")
        sb.AppendLine("		  ,t.*")
        sb.AppendLine("FROM t_check t")

        sb.AppendLine("LEFT JOIN (select max(mark) mark,ck_id from t_check_ms group by ck_id) ms")    '检查
        sb.AppendLine("  ON t.ck_id=ms.ck_id")

        sb.AppendLine("LEFT JOIN m_user b")     '用户
        sb.AppendLine("     ON t.chk_user = b.user_cd")
        sb.AppendLine("LEFT JOIN t_check_plan c")    '检查
        sb.AppendLine("     ON t.cd = c.cd")
        sb.AppendLine("     AND t.no = c.no")
        sb.AppendLine("     AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")

        sb.AppendLine("WHERE (result='OK' or result='OK')")

        'replace(c.cd,'-','') = '" & cd & "'
        If department_cd.Trim <> "" Then    '部门
            sb.AppendLine("	AND c.department_cd in(" & department_cd & ")")
        End If
        sb.AppendLine("	    AND t.yotei_chk_date>'" & startDate & " 07:59:59" & "'")
        sb.AppendLine("	    AND t.yotei_chk_date<'" & endDate & " 08:00:00" & "'")


        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetScsj")

    End Function

End Class
