Imports System.Data
Imports Microsoft.VisualBasic
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.CodeDom.Compiler
Imports MSScriptControl


Public Class t_BuLiangDA


    '主表计划中 获得相关检查数据
    Function GetBuLiangList(ByVal cd As String, ByVal no As String, ByVal rinei As String, ByVal department_cd As String) As DataTable

        Dim sb As New StringBuilder

        sb.AppendLine("SELECT * FROM (")
        sb.AppendLine("SELECT isnull(t.ck_id,'') ck_id")
        'sb.AppendLine("SELECT ")
        sb.AppendLine("      ,isnull(c.cd,t.cd) cd")
        sb.AppendLine("      ,isnull(c.no,t.no) no")
        ' sb.AppendLine("      ,t.department_cd")
        sb.AppendLine("      ,isnull(c.line_cd,t.line_cd) line_cd")
        sb.AppendLine("      ,t.chk_user")
        sb.AppendLine("      ,Convert(varchar(100), isnull(c.yotei_chk_date,t.yotei_chk_date) , 23) as yotei_chk_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) as chk_start_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) as chk_end_date")
        sb.AppendLine("      ,t.status")
        sb.AppendLine("      ,t.result")
        sb.AppendLine("      ,CASE WHEN ISNULL(t.result,'') = 'NG' AND isnull(pre.ck_id,'')<>'' AND isnull(i.result,'')='OK' THEN '不良 已重检OK' ")
        sb.AppendLine("            WHEN ISNULL(t.result,'') = 'NG' AND isnull(pre.ck_id,'')<>'' AND isnull(i.result,'')<>'OK' THEN 'NG 不良 已经重检 未OK' ")
        sb.AppendLine("            WHEN ISNULL(t.result,'') = 'NG' AND isnull(pre.ck_id,'') = '' THEN 'NG 不良 未重检' ")

        sb.AppendLine("      ELSE '' END buliang")


        sb.AppendLine("      ,t.chk_times")
        sb.AppendLine("      ,isnull(c.suu,t.suu) suu")
        sb.AppendLine("      ,t.b2bOderNo")
        sb.AppendLine("      ,t.b2bIndexNo")
        sb.AppendLine("      ,isnull(c.sapOderNo,t.sapOderNo) sapOderNo")
        sb.AppendLine("      ,isnull(c.sapIndexNo,t.sapIndexNo) sapIndexNo")
        sb.AppendLine("      ,b.user_name")
        sb.AppendLine("      ,isnull(c.specialBookNo,t.specialBookNo) specialBookNo")
        sb.AppendLine("      ,isnull(c.jxs_name,'') jxs_name")
        sb.AppendLine("      ,ISNULL(ms.mark,'') mark")
        sb.AppendLine("      ,ISNULL(c.DestinationCode,'') xiangxian")
        sb.AppendLine("FROM t_check t")    '计划
        sb.AppendLine("LEFT JOIN (select max(mark) mark,ck_id from t_check_ms group by ck_id) ms")    '检查
        sb.AppendLine("  ON t.ck_id=ms.ck_id")
        sb.AppendLine("LEFT JOIN t_check_plan c")    '检查
        sb.AppendLine("     ON t.cd = c.cd")
        sb.AppendLine("     AND t.no = c.no")
        sb.AppendLine("     AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")
        sb.AppendLine("LEFT JOIN m_user b")     '用户
        sb.AppendLine("     ON t.chk_user = b.user_cd")
        'pre_ck_id 父
        sb.AppendLine("LEFT JOIN t_pre_chk_id pre")
        sb.AppendLine("ON pre.pre_ck_id = t.ck_id")
        sb.AppendLine("LEFT JOIN t_pre_chk_id chlid")
        sb.AppendLine("ON chlid.ck_id = t.ck_id")
        sb.AppendLine("LEFT JOIN t_check i")
        sb.AppendLine("ON i.ck_id = chlid.ck_id")
        sb.AppendLine("WHERE ISNULL(t.result,'') = 'NG'")
        If cd <> "" Then
            sb.AppendLine("	AND isnull(c.cd,t.cd) = '" & cd & "'")
        End If
        If no <> "" Then
            sb.AppendLine("	AND isnull(c.no,t.no) = '" & no & "'")
        End If
        If rinei <> "" Then
            sb.AppendLine("	    AND t.yotei_chk_date>dateadd(day,-" & rinei & ",getdate())")
            sb.AppendLine("	    AND t.yotei_chk_date<dateadd(day," & rinei & ",getdate())")
        End If
        If department_cd.Trim <> "" Then    '部门
            sb.AppendLine("	AND (t.department_cd in(" & department_cd & ") or t.department_cd='')")
        End If

        sb.AppendLine("	UNION   ")

        sb.AppendLine("SELECT isnull(t.ck_id,'') ck_id")
        sb.AppendLine("		  ,c.[CD]")
        sb.AppendLine("		  ,c.[no] as [no]")
        sb.AppendLine("		  ,c.[line_cd]")
        sb.AppendLine("      ,t.chk_user")
        sb.AppendLine("		  ,Convert(varchar(100), c.yotei_chk_date , 23) as yotei_chk_date")
        sb.AppendLine("		  ,null as chk_start_date")
        sb.AppendLine("		  ,null as chk_end_date")
        sb.AppendLine("		  ,t.status")
        sb.AppendLine("		  ,t.result")
        sb.AppendLine("		  ,CASE WHEN ISNULL(t.no,'') = '' THEN 'NG 不良代替 未重检'")
        sb.AppendLine("		        WHEN ISNULL(t.result,'') = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中'")

        sb.AppendLine("		  END buliang")

        sb.AppendLine("		  ,t.chk_times")
        sb.AppendLine("		  ,isnull(c.suu,t.suu) suu")
        sb.AppendLine("		  ,t.b2bOderNo")
        sb.AppendLine("		  ,t.b2bIndexNo")
        sb.AppendLine("		  ,a.[sapOderNo]")
        sb.AppendLine("		  ,a.[sapIndexNo]")
        sb.AppendLine("       ,b.user_name")
        sb.AppendLine("		  ,a.[specialBookNo]")
        sb.AppendLine("		  ,a.[DealerAbbreviation] jxs_name")
        sb.AppendLine("		  ,a.[mark]")
        sb.AppendLine("		  ,ISNULL(c.DestinationCode,'') xiangxian")
        sb.AppendLine("")
        sb.AppendLine("	  FROM [m_bl_manage] a")
        sb.AppendLine("	  INNER JOIN t_check_plan c")
        sb.AppendLine("	  ON (")
        sb.AppendLine("				(a.specialBookNo = c.specialBookNo AND ISNULL(a.specialBookNo,'')<>'')")
        sb.AppendLine("			OR  (a.sapOderNo = c.sapOderNo AND a.sapIndexNo = c.sapIndexNo AND ISNULL(a.specialBookNo,'') = '')")
        sb.AppendLine("			)")
        sb.AppendLine("		AND a.line_cd = c.line_cd")
        sb.AppendLine("	  LEFT JOIN t_check t")
        sb.AppendLine("	  ON c.no = t.no")
        sb.AppendLine("LEFT JOIN m_user b")     '用户
        sb.AppendLine("     ON t.chk_user = b.user_cd")

        sb.AppendLine("WHERE 1=1 ")
        If cd <> "" Then
            sb.AppendLine("	AND isnull(c.cd,t.cd) = '" & cd & "'")
        End If
        If no <> "" Then
            sb.AppendLine("	AND isnull(c.no,t.no) = '" & no & "'")
        End If
        'If rinei <> "" Then
        '    sb.AppendLine("	    AND a.upd_date>dateadd(day,-" & rinei & ",getdate())")
        '    sb.AppendLine("	    AND a.upd_date<dateadd(day," & rinei & ",getdate())")
        'End If


        sb.AppendLine(") all_buliang")

        sb.AppendLine("WHERE NOT exists(")

        sb.AppendLine("select 1 from t_check tc where tc.cd=all_buliang.cd and tc.no=all_buliang.no and tc.result='OK'")

        sb.AppendLine(")")

        sb.AppendLine("AND NOT exists(")

        sb.AppendLine("SELECT 1 FROM")
        sb.AppendLine("(")
        sb.AppendLine("SELECT ")
        sb.AppendLine("		  a.[sapOderNo]")
        sb.AppendLine("		  ,a.[sapIndexNo]")
        sb.AppendLine("		  ,a.[specialBookNo]")
        sb.AppendLine("		  ,ISNULL(c.DestinationCode,'') xiangxian")
        sb.AppendLine("	  FROM [m_bl_manage] a")
        sb.AppendLine("	  INNER JOIN t_check_plan c")
        sb.AppendLine("	  ON (")
        sb.AppendLine("				(a.specialBookNo = c.specialBookNo AND ISNULL(a.specialBookNo,'')<>'')")
        sb.AppendLine("			OR  (a.sapOderNo = c.sapOderNo AND a.sapIndexNo = c.sapIndexNo AND ISNULL(a.specialBookNo,'') = '')")
        sb.AppendLine("			)")
        sb.AppendLine("		AND a.line_cd = c.line_cd")
        sb.AppendLine("	  LEFT JOIN t_check t")
        sb.AppendLine("	  ON c.no = t.no")
        sb.AppendLine("LEFT JOIN m_user b")
        sb.AppendLine("     ON t.chk_user = b.user_cd")
        sb.AppendLine("WHERE 1=1 ")
        sb.AppendLine("AND ISNULL(t.result,'') = 'OK'")
        'sb.AppendLine("AND a.specialBookNo = 'DLS157854'")
        sb.AppendLine(") buliang_ok")
        sb.AppendLine("WHERE (all_buliang.specialBookNo = buliang_ok.specialBookNo AND ISNULL(all_buliang.specialBookNo,'')<>'')")

        sb.AppendLine(")")


        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function


End Class
