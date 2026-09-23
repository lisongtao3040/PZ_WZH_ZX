Imports System.Data
Imports Microsoft.VisualBasic
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.CodeDom.Compiler
Imports MSScriptControl


Public Class t_BuLiangDA


    '主表计划中 获得相关检查数据
    Function GetBuLiangList1(ByVal cd As String, ByVal no As String, ByVal rinei As String, ByVal department_cd As String) As DataTable

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

    '主表计划中 获得相关检查数据 (极致优化版)
    Function GetBuLiangList2(ByVal cd As String, ByVal no As String, ByVal rinei As String, ByVal department_cd As String) As DataTable

        Dim sb As New StringBuilder

        ' 使用 WITH 语句 (CTE) 将逻辑模块化，极大提升执行计划效率
        sb.AppendLine("WITH ")

        ' 1. 预先提取排除列表 (OK的数据)，减小最后 NOT EXISTS 的计算量
        sb.AppendLine("CTE_ExcludeCheck AS (")
        sb.AppendLine("    SELECT DISTINCT cd, no FROM t_check WHERE result = 'OK'")
        If cd <> "" Then sb.AppendLine("    AND cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("    AND no = '" & no & "'")
        sb.AppendLine("),")

        sb.AppendLine("CTE_ExcludeBook AS (")
        ' 优化点：剔除条件只用到了 specialBookNo，因此直接删掉原代码这里低效的 OR sapOderNo 的 JOIN 分支
        sb.AppendLine("    SELECT DISTINCT a.specialBookNo ")
        sb.AppendLine("    FROM m_bl_manage a")
        sb.AppendLine("    INNER JOIN t_check_plan c ON a.specialBookNo = c.specialBookNo AND a.line_cd = c.line_cd")
        sb.AppendLine("    INNER JOIN t_check t ON c.no = t.no")
        sb.AppendLine("    WHERE ISNULL(a.specialBookNo, '') <> '' AND t.result = 'OK'")
        sb.AppendLine("),")

        ' 2. 第一部分：原 UNION 的上半部分 (t_check 计划)
        sb.AppendLine("CTE_Part1 AS (")
        sb.AppendLine("    SELECT ")
        sb.AppendLine("        ISNULL(t.ck_id,'') AS ck_id,")
        sb.AppendLine("        t.cd,") ' 优化: ISNULL(c.cd,t.cd) 简化为 t.cd，恢复索引
        sb.AppendLine("        t.no,") ' 优化: ISNULL(c.no,t.no) 简化为 t.no，恢复索引
        sb.AppendLine("        ISNULL(c.line_cd, t.line_cd) AS line_cd,")
        sb.AppendLine("        t.chk_user,")
        sb.AppendLine("        CONVERT(varchar(100), ISNULL(c.yotei_chk_date, t.yotei_chk_date), 23) AS yotei_chk_date,")
        sb.AppendLine("        CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) AS chk_start_date,")
        sb.AppendLine("        CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) AS chk_end_date,")
        sb.AppendLine("        t.status,")
        sb.AppendLine("        t.result,")
        sb.AppendLine("        CASE ")
        sb.AppendLine("            WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') <> '' AND ISNULL(i.result,'') = 'OK' THEN '不良 已重检OK'")
        sb.AppendLine("            WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') <> '' AND ISNULL(i.result,'') <> 'OK' THEN 'NG 不良 已经重检 未OK'")
        sb.AppendLine("            WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') = '' THEN 'NG 不良 未重检'")
        sb.AppendLine("            ELSE '' ")
        sb.AppendLine("        END AS buliang,")
        sb.AppendLine("        t.chk_times,")
        sb.AppendLine("        ISNULL(c.suu, t.suu) AS suu,")
        sb.AppendLine("        t.b2bOderNo,")
        sb.AppendLine("        t.b2bIndexNo,")
        sb.AppendLine("        ISNULL(c.sapOderNo, t.sapOderNo) AS sapOderNo,")
        sb.AppendLine("        ISNULL(c.sapIndexNo, t.sapIndexNo) AS sapIndexNo,")
        sb.AppendLine("        b.user_name,")
        sb.AppendLine("        ISNULL(c.specialBookNo, t.specialBookNo) AS specialBookNo,")
        sb.AppendLine("        ISNULL(c.jxs_name,'') AS jxs_name,")
        ' 优化点：将全局 Group By 改为标量子查询，避免对 t_check_ms 全表扫描聚合
        sb.AppendLine("        (SELECT MAX(mark) FROM t_check_ms ms WHERE ms.ck_id = t.ck_id) AS mark,")
        sb.AppendLine("        ISNULL(c.DestinationCode,'') AS xiangxian")
        sb.AppendLine("    FROM t_check t")
        sb.AppendLine("    LEFT JOIN t_check_plan c ON t.cd = c.cd AND t.no = c.no AND CONVERT(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")
        sb.AppendLine("    LEFT JOIN m_user b ON t.chk_user = b.user_cd")
        sb.AppendLine("    LEFT JOIN t_pre_chk_id pre ON pre.pre_ck_id = t.ck_id")
        sb.AppendLine("    LEFT JOIN t_pre_chk_id chlid ON chlid.ck_id = t.ck_id")
        sb.AppendLine("    LEFT JOIN t_check i ON i.ck_id = chlid.ck_id")
        sb.AppendLine("    WHERE t.result = 'NG'") ' 优化: 去除 ISNULL(t.result,'')
        If cd <> "" Then sb.AppendLine("    AND t.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("    AND t.no = '" & no & "'")
        If rinei <> "" Then
            sb.AppendLine("    AND t.yotei_chk_date > DATEADD(day, -" & rinei & ", GETDATE())")
            sb.AppendLine("    AND t.yotei_chk_date < DATEADD(day, " & rinei & ", GETDATE())")
        End If
        If department_cd.Trim <> "" Then
            sb.AppendLine("    AND (t.department_cd IN (" & department_cd & ") OR ISNULL(t.department_cd,'') = '')")
        End If
        sb.AppendLine("),")

        ' 3. 第二部分A：将原 UNION 下半部分的 OR JOIN 拆分成两个独立的查询以利用索引 (按 specialBookNo 关联)
        sb.AppendLine("CTE_Part2_A AS (")
        sb.AppendLine("    SELECT ")
        sb.AppendLine("        ISNULL(t.ck_id,'') AS ck_id, c.[CD] AS cd, c.[no] AS no, c.[line_cd], t.chk_user,")
        sb.AppendLine("        CONVERT(varchar(100), c.yotei_chk_date , 23) AS yotei_chk_date, NULL AS chk_start_date, NULL AS chk_end_date,")
        sb.AppendLine("        t.status, t.result,")
        sb.AppendLine("        CASE WHEN ISNULL(t.no,'') = '' THEN 'NG 不良代替 未重检' WHEN t.result = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中' END AS buliang,")
        sb.AppendLine("        t.chk_times, ISNULL(c.suu, t.suu) AS suu, t.b2bOderNo, t.b2bIndexNo, a.[sapOderNo], a.[sapIndexNo],")
        sb.AppendLine("        b.user_name, a.[specialBookNo], a.[DealerAbbreviation] AS jxs_name, a.[mark], ISNULL(c.DestinationCode,'') AS xiangxian")
        sb.AppendLine("    FROM m_bl_manage a")
        sb.AppendLine("    INNER JOIN t_check_plan c ON a.specialBookNo = c.specialBookNo AND a.line_cd = c.line_cd")
        sb.AppendLine("    LEFT JOIN t_check t ON c.no = t.no")
        sb.AppendLine("    LEFT JOIN m_user b ON t.chk_user = b.user_cd")
        sb.AppendLine("    WHERE ISNULL(a.specialBookNo,'') <> ''")
        If cd <> "" Then sb.AppendLine("    AND c.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("    AND c.no = '" & no & "'")
        sb.AppendLine("),")

        ' 4. 第二部分B：将原 UNION 下半部分的 OR JOIN 拆分成两个独立的查询以利用索引 (按 sapOderNo 关联)
        sb.AppendLine("CTE_Part2_B AS (")
        sb.AppendLine("    SELECT ")
        sb.AppendLine("        ISNULL(t.ck_id,'') AS ck_id, c.[CD] AS cd, c.[no] AS no, c.[line_cd], t.chk_user,")
        sb.AppendLine("        CONVERT(varchar(100), c.yotei_chk_date , 23) AS yotei_chk_date, NULL AS chk_start_date, NULL AS chk_end_date,")
        sb.AppendLine("        t.status, t.result,")
        sb.AppendLine("        CASE WHEN ISNULL(t.no,'') = '' THEN 'NG 不良代替 未重检' WHEN t.result = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中' END AS buliang,")
        sb.AppendLine("        t.chk_times, ISNULL(c.suu, t.suu) AS suu, t.b2bOderNo, t.b2bIndexNo, a.[sapOderNo], a.[sapIndexNo],")
        sb.AppendLine("        b.user_name, a.[specialBookNo], a.[DealerAbbreviation] AS jxs_name, a.[mark], ISNULL(c.DestinationCode,'') AS xiangxian")
        sb.AppendLine("    FROM m_bl_manage a")
        sb.AppendLine("    INNER JOIN t_check_plan c ON a.sapOderNo = c.sapOderNo AND a.sapIndexNo = c.sapIndexNo AND a.line_cd = c.line_cd")
        sb.AppendLine("    LEFT JOIN t_check t ON c.no = t.no")
        sb.AppendLine("    LEFT JOIN m_user b ON t.chk_user = b.user_cd")
        sb.AppendLine("    WHERE ISNULL(a.specialBookNo,'') = ''")
        If cd <> "" Then sb.AppendLine("    AND c.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("    AND c.no = '" & no & "'")
        sb.AppendLine(")")

        ' 5. 组合全部结果并执行快速过滤
        sb.AppendLine("SELECT final.* FROM (")
        sb.AppendLine("    SELECT * FROM CTE_Part1")
        sb.AppendLine("    UNION")
        sb.AppendLine("    SELECT * FROM CTE_Part2_A")
        sb.AppendLine("    UNION")
        sb.AppendLine("    SELECT * FROM CTE_Part2_B")
        sb.AppendLine(") AS final")
        sb.AppendLine("WHERE NOT EXISTS (SELECT 1 FROM CTE_ExcludeCheck ec WHERE ec.cd = final.cd AND ec.no = final.no)")
        sb.AppendLine("  AND NOT EXISTS (SELECT 1 FROM CTE_ExcludeBook eb WHERE eb.specialBookNo = final.specialBookNo AND ISNULL(final.specialBookNo,'') <> '')")

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function



    '主表计划中 获得相关检查数据 (拆分合并极速版)
    Function GetBuLiangList3(ByVal cd As String, ByVal no As String, ByVal rinei As String, ByVal department_cd As String) As DataTable

        '定义三个 DataTable 分别接收不同维度的数据
        Dim dt1 As DataTable
        Dim dt2 As DataTable
        Dim dt3 As DataTable

        Dim sb As New StringBuilder

        ' =====================================================================================
        ' 第 1 部分：基于 t_check (计划) 的主数据查询
        ' =====================================================================================
        sb.AppendLine("SELECT ")
        sb.AppendLine("    ISNULL(t.ck_id,'') AS ck_id, t.cd, t.no, ISNULL(c.line_cd, t.line_cd) AS line_cd, t.chk_user,")
        sb.AppendLine("    CONVERT(varchar(100), ISNULL(c.yotei_chk_date, t.yotei_chk_date), 23) AS yotei_chk_date,")
        sb.AppendLine("    CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) AS chk_start_date,")
        sb.AppendLine("    CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) AS chk_end_date,")
        sb.AppendLine("    t.status, t.result,")
        sb.AppendLine("    CASE ")
        sb.AppendLine("        WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') <> '' AND ISNULL(i.result,'') = 'OK' THEN '不良 已重检OK'")
        sb.AppendLine("        WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') <> '' AND ISNULL(i.result,'') <> 'OK' THEN 'NG 不良 已经重检 未OK'")
        sb.AppendLine("        WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') = '' THEN 'NG 不良 未重检'")
        sb.AppendLine("        ELSE '' ")
        sb.AppendLine("    END AS buliang,")
        sb.AppendLine("    t.chk_times, ISNULL(c.suu, t.suu) AS suu, t.b2bOderNo, t.b2bIndexNo,")
        sb.AppendLine("    ISNULL(c.sapOderNo, t.sapOderNo) AS sapOderNo, ISNULL(c.sapIndexNo, t.sapIndexNo) AS sapIndexNo,")
        sb.AppendLine("    b.user_name, ISNULL(c.specialBookNo, t.specialBookNo) AS specialBookNo, ISNULL(c.jxs_name,'') AS jxs_name,")
        sb.AppendLine("    (SELECT MAX(mark) FROM t_check_ms ms WHERE ms.ck_id = t.ck_id) AS mark,") ' 标量子查询替代大表 JOIN
        sb.AppendLine("    ISNULL(c.DestinationCode,'') AS xiangxian")
        sb.AppendLine("FROM t_check t")
        sb.AppendLine("LEFT JOIN t_check_plan c ON t.cd = c.cd AND t.no = c.no AND CONVERT(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")
        sb.AppendLine("LEFT JOIN m_user b ON t.chk_user = b.user_cd")
        sb.AppendLine("LEFT JOIN t_pre_chk_id pre ON pre.pre_ck_id = t.ck_id")
        sb.AppendLine("LEFT JOIN t_pre_chk_id chlid ON chlid.ck_id = t.ck_id")
        sb.AppendLine("LEFT JOIN t_check i ON i.ck_id = chlid.ck_id")
        sb.AppendLine("WHERE t.result = 'NG'")
        If cd <> "" Then sb.AppendLine("  AND t.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("  AND t.no = '" & no & "'")
        If rinei <> "" Then
            sb.AppendLine("  AND t.yotei_chk_date > DATEADD(day, -" & rinei & ", GETDATE())")
            sb.AppendLine("  AND t.yotei_chk_date < DATEADD(day, " & rinei & ", GETDATE())")
        End If
        If department_cd.Trim <> "" Then
            sb.AppendLine("  AND (t.department_cd IN (" & department_cd & ") OR ISNULL(t.department_cd,'') = '')")
        End If
        ' 直接在第1部分剔除不符合条件的数据
        sb.AppendLine("  AND NOT EXISTS (SELECT 1 FROM t_check tc WHERE tc.cd = t.cd AND tc.no = t.no AND tc.result = 'OK')")
        sb.AppendLine("  AND NOT EXISTS (")
        sb.AppendLine("      SELECT 1 FROM m_bl_manage eb ")
        sb.AppendLine("      INNER JOIN t_check_plan ec ON eb.specialBookNo = ec.specialBookNo AND eb.line_cd = ec.line_cd")
        sb.AppendLine("      INNER JOIN t_check et ON ec.no = et.no ")
        sb.AppendLine("      WHERE et.result = 'OK' ")
        sb.AppendLine("        AND eb.specialBookNo = ISNULL(c.specialBookNo, t.specialBookNo)")
        sb.AppendLine("        AND ISNULL(ISNULL(c.specialBookNo, t.specialBookNo), '') <> ''")
        sb.AppendLine("  )")
        dt1 = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList1")
        ' =====================================================================================
        ' 第 2 部分：基于 m_bl_manage 的 specialBookNo 关联数据 (临时表 + 严格双表规范版)
        ' =====================================================================================
        sb.Clear()

        ' 0. 开启 NOCOUNT，防止 INTO 语句影响 DataTable 结果集的获取
        sb.AppendLine("SET NOCOUNT ON;")

        ' 1. m_bl_manage 与 t_check_plan 结合 (第 1 次双表)
        sb.AppendLine("SELECT a.sapOderNo, a.sapIndexNo, a.specialBookNo, a.DealerAbbreviation, a.mark,")
        sb.AppendLine("       c.CD, c.no, c.line_cd, c.yotei_chk_date, c.suu, c.DestinationCode")
        sb.AppendLine("INTO #Temp_Part2_ManagePlan")
        sb.AppendLine("FROM m_bl_manage a")
        sb.AppendLine("INNER JOIN t_check_plan c ON a.specialBookNo = c.specialBookNo AND a.line_cd = c.line_cd")
        sb.AppendLine("WHERE ISNULL(a.specialBookNo,'') <> ''")
        If cd <> "" Then sb.AppendLine("  AND c.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("  AND c.no = '" & no & "'")
        sb.AppendLine(";")

        ' 2. 加入 t_check (第 2 次双表)
        sb.AppendLine("SELECT mp.*, t.ck_id, t.chk_user, t.status, t.result, t.no AS t_no, t.chk_times, t.suu AS t_suu, t.b2bOderNo, t.b2bIndexNo")
        sb.AppendLine("INTO #Temp_Part2_Check")
        sb.AppendLine("FROM #Temp_Part2_ManagePlan mp")
        sb.AppendLine("LEFT JOIN t_check t ON mp.no = t.no;")

        ' 3. 构建排斥名单：计划表与检查表结合查出 OK 记录 (第 3 次双表)
        sb.AppendLine("SELECT ec.specialBookNo, ec.line_cd")
        sb.AppendLine("INTO #Temp_OK_Plans")
        sb.AppendLine("FROM t_check_plan ec")
        sb.AppendLine("INNER JOIN t_check et ON ec.no = et.no")
        sb.AppendLine("WHERE et.result = 'OK';")

        ' 4. 将排斥名单与 m_bl_manage 结合，找出需剔除的 specialBookNo (第 4 次双表)
        sb.AppendLine("SELECT DISTINCT eb.specialBookNo")
        sb.AppendLine("INTO #Temp_Exclude_Book")
        sb.AppendLine("FROM m_bl_manage eb")
        sb.AppendLine("INNER JOIN #Temp_OK_Plans p ON eb.specialBookNo = p.specialBookNo AND eb.line_cd = p.line_cd;")

        ' =====================================================================================
        ' 5. 输出最终查询，关闭 NOCOUNT 让 DataTable 成功捕获 SELECT
        ' =====================================================================================
        sb.AppendLine("SET NOCOUNT OFF;")

        ' 最终 JOIN m_user (第 5 次双表)
        sb.AppendLine("SELECT ")
        sb.AppendLine("    ISNULL(mpc.ck_id,'') AS ck_id, mpc.[CD] AS cd, mpc.[no] AS no, mpc.[line_cd], mpc.chk_user,")
        sb.AppendLine("    CONVERT(varchar(100), mpc.yotei_chk_date , 23) AS yotei_chk_date, CAST(NULL AS varchar(100)) AS chk_start_date, CAST(NULL AS varchar(100)) AS chk_end_date,")
        sb.AppendLine("    mpc.status, mpc.result,")
        sb.AppendLine("    CASE WHEN ISNULL(mpc.t_no,'') = '' THEN 'NG 不良代替 未重检' WHEN mpc.result = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中' END AS buliang,")
        sb.AppendLine("    mpc.chk_times, ISNULL(mpc.suu, mpc.t_suu) AS suu, mpc.b2bOderNo, mpc.b2bIndexNo, mpc.sapOderNo, mpc.sapIndexNo,")
        sb.AppendLine("    b.user_name, mpc.specialBookNo, mpc.DealerAbbreviation AS jxs_name, mpc.mark, ISNULL(mpc.DestinationCode,'') AS xiangxian")
        sb.AppendLine("FROM #Temp_Part2_Check mpc")
        sb.AppendLine("LEFT JOIN m_user b ON mpc.chk_user = b.user_cd")
        sb.AppendLine("WHERE NOT EXISTS (SELECT 1 FROM t_check tc WHERE tc.cd = mpc.CD AND tc.no = mpc.no AND tc.result = 'OK')")
        sb.AppendLine("  AND NOT EXISTS (SELECT 1 FROM #Temp_Exclude_Book ex WHERE ex.specialBookNo = mpc.specialBookNo);")

        ' 6. 清理临时表，释放数据库内存
        sb.AppendLine("DROP TABLE #Temp_Part2_ManagePlan; DROP TABLE #Temp_Part2_Check;")
        sb.AppendLine("DROP TABLE #Temp_OK_Plans; DROP TABLE #Temp_Exclude_Book;")

        dt2 = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList2")

        ' =====================================================================================
        ' 第 3 部分：基于 m_bl_manage 的 sapOderNo 关联数据 (specialBookNo 为空)
        ' =====================================================================================
        sb.Clear()
        sb.AppendLine("SELECT ")
        sb.AppendLine("    ISNULL(t.ck_id,'') AS ck_id, c.[CD] AS cd, c.[no] AS no, c.[line_cd], t.chk_user,")
        ' 【修改点】：将 NULL 强制转换为 varchar(100)
        sb.AppendLine("    CONVERT(varchar(100), c.yotei_chk_date , 23) AS yotei_chk_date, CAST(NULL AS varchar(100)) AS chk_start_date, CAST(NULL AS varchar(100)) AS chk_end_date,")
        sb.AppendLine("    t.status, t.result,")
        sb.AppendLine("    CASE WHEN ISNULL(t.no,'') = '' THEN 'NG 不良代替 未重检' WHEN t.result = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中' END AS buliang,")
        sb.AppendLine("    t.chk_times, ISNULL(c.suu, t.suu) AS suu, t.b2bOderNo, t.b2bIndexNo, a.[sapOderNo], a.[sapIndexNo],")
        sb.AppendLine("    b.user_name, a.[specialBookNo], a.[DealerAbbreviation] AS jxs_name, a.[mark], ISNULL(c.DestinationCode,'') AS xiangxian")
        sb.AppendLine("FROM m_bl_manage a")
        sb.AppendLine("INNER JOIN t_check_plan c ON a.sapOderNo = c.sapOderNo AND a.sapIndexNo = c.sapIndexNo AND a.line_cd = c.line_cd")
        sb.AppendLine("LEFT JOIN t_check t ON c.no = t.no")
        sb.AppendLine("LEFT JOIN m_user b ON t.chk_user = b.user_cd")
        sb.AppendLine("WHERE ISNULL(a.specialBookNo,'') = ''")
        If cd <> "" Then sb.AppendLine("  AND c.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("  AND c.no = '" & no & "'")
        ' 剔除逻辑
        sb.AppendLine("  AND NOT EXISTS (SELECT 1 FROM t_check tc WHERE tc.cd = c.cd AND tc.no = c.no AND tc.result = 'OK')")
        dt3 = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList3")

        ' =====================================================================================
        ' 合并 3 个 DataTable 的数据并返回结果
        ' =====================================================================================
        If dt1 IsNot Nothing Then
            If dt2 IsNot Nothing AndAlso dt2.Rows.Count > 0 Then dt1.Merge(dt2)
            If dt3 IsNot Nothing AndAlso dt3.Rows.Count > 0 Then dt1.Merge(dt3)

            ' 如果担心在原有 UNION 架构下存在的极其罕见的重复行，可以使用 DataView 取唯一值
            ' 正常情况下，这三个集合在业务逻辑上是互相独立的，直接返回 dt1 速度最快。
            Return dt1
        Else
            ' 万一 dt1 为 null 的异常降级处理
            Return New DataTable()
        End If

    End Function


    '主表计划中 获得相关检查数据 (终极临时表 + 严格双表结合版)
    Function GetBuLiangList(ByVal cd As String, ByVal no As String, ByVal rinei As String, ByVal department_cd As String) As DataTable

        Dim sb As New StringBuilder

        ' =====================================================================================
        ' 0. 开启 NOCOUNT，防止 ADO.NET 把 INSERT/INTO 的受影响行数当成结果集
        ' =====================================================================================
        sb.AppendLine("SET NOCOUNT ON;")

        ' === 预备数据：提取 OK 的单据用于后期快速过滤 (完全符合双表规范) ===
        sb.AppendLine("SELECT cd, no INTO #Temp_OK_Check FROM t_check WHERE result = 'OK';")

        sb.AppendLine("SELECT c.specialBookNo, c.line_cd, c.no INTO #Temp_Plan_Book FROM t_check_plan c WHERE ISNULL(c.specialBookNo,'') <> '';")
        sb.AppendLine("SELECT pb.specialBookNo INTO #Temp_OK_Book FROM #Temp_Plan_Book pb INNER JOIN t_check t ON pb.no = t.no WHERE t.result = 'OK';")

        ' =====================================================================================
        ' 第 1 部分：基于 t_check (直接输出到临时表 #Result1)
        ' =====================================================================================
        sb.AppendLine("SELECT ")
        sb.AppendLine("    ISNULL(t.ck_id,'') AS ck_id, t.cd, t.no, ISNULL(c.line_cd, t.line_cd) AS line_cd, t.chk_user,")
        sb.AppendLine("    CONVERT(varchar(100), ISNULL(c.yotei_chk_date, t.yotei_chk_date), 23) AS yotei_chk_date,")
        sb.AppendLine("    CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) AS chk_start_date,")
        sb.AppendLine("    CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) AS chk_end_date,")
        sb.AppendLine("    t.status, t.result,")
        sb.AppendLine("    CASE ")
        sb.AppendLine("        WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') <> '' AND ISNULL(i.result,'') = 'OK' THEN '不良 已重检OK'")
        sb.AppendLine("        WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') <> '' AND ISNULL(i.result,'') <> 'OK' THEN 'NG 不良 已经重检 未OK'")
        sb.AppendLine("        WHEN t.result = 'NG' AND ISNULL(pre.ck_id,'') = '' THEN 'NG 不良 未重检'")
        sb.AppendLine("        ELSE '' ")
        sb.AppendLine("    END AS buliang,")
        sb.AppendLine("    t.chk_times, ISNULL(c.suu, t.suu) AS suu, t.b2bOderNo, t.b2bIndexNo,")
        sb.AppendLine("    ISNULL(c.sapOderNo, t.sapOderNo) AS sapOderNo, ISNULL(c.sapIndexNo, t.sapIndexNo) AS sapIndexNo,")
        sb.AppendLine("    b.user_name, ISNULL(c.specialBookNo, t.specialBookNo) AS specialBookNo, ISNULL(c.jxs_name,'') AS jxs_name,")
        sb.AppendLine("    (SELECT MAX(mark) FROM t_check_ms ms WHERE ms.ck_id = t.ck_id) AS mark,")
        sb.AppendLine("    ISNULL(c.DestinationCode,'') AS xiangxian")
        sb.AppendLine("INTO #Result1")
        sb.AppendLine("FROM t_check t")
        sb.AppendLine("LEFT JOIN t_check_plan c ON t.cd = c.cd AND t.no = c.no AND CONVERT(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")
        sb.AppendLine("LEFT JOIN m_user b ON t.chk_user = b.user_cd")
        sb.AppendLine("LEFT JOIN t_pre_chk_id pre ON pre.pre_ck_id = t.ck_id")
        sb.AppendLine("LEFT JOIN t_pre_chk_id chlid ON chlid.ck_id = t.ck_id")
        sb.AppendLine("LEFT JOIN t_check i ON i.ck_id = chlid.ck_id")
        sb.AppendLine("WHERE t.result = 'NG'")
        If cd <> "" Then sb.AppendLine("  AND t.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("  AND t.no = '" & no & "'")
        If rinei <> "" Then
            sb.AppendLine("  AND t.yotei_chk_date > DATEADD(day, -" & rinei & ", GETDATE())")
            sb.AppendLine("  AND t.yotei_chk_date < DATEADD(day, " & rinei & ", GETDATE())")
        End If
        If department_cd.Trim <> "" Then
            sb.AppendLine("  AND (t.department_cd IN (" & department_cd & ") OR ISNULL(t.department_cd,'') = '')")
        End If
        sb.AppendLine("  AND NOT EXISTS (SELECT 1 FROM #Temp_OK_Check ok WHERE ok.cd = t.cd AND ok.no = t.no)")
        sb.AppendLine("  AND NOT EXISTS (SELECT 1 FROM #Temp_OK_Book ob WHERE ob.specialBookNo = ISNULL(c.specialBookNo, t.specialBookNo));")

        ' =====================================================================================
        ' 第 2 部分：m_bl_manage specialBookNo 关联 (严格双表临时表拆解)
        ' =====================================================================================
        ' 2.1 m_bl_manage 与 t_check_plan 结合
        sb.AppendLine("SELECT a.sapOderNo, a.sapIndexNo, a.specialBookNo, a.DealerAbbreviation, a.mark,")
        sb.AppendLine("       c.CD, c.no, c.line_cd, c.yotei_chk_date, c.suu, c.DestinationCode")
        sb.AppendLine("INTO #Temp_Part2_ManagePlan")
        sb.AppendLine("FROM m_bl_manage a")
        sb.AppendLine("INNER JOIN t_check_plan c ON a.specialBookNo = c.specialBookNo AND a.line_cd = c.line_cd")
        sb.AppendLine("WHERE ISNULL(a.specialBookNo,'') <> ''")
        If cd <> "" Then sb.AppendLine("  AND c.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("  AND c.no = '" & no & "'")
        sb.AppendLine(";")

        ' 2.2 加入 t_check (利用上一步的结果结合，符合双表规范)
        sb.AppendLine("SELECT mp.*, t.ck_id, t.chk_user, t.status, t.result, t.no AS t_no, t.chk_times, t.suu AS t_suu, t.b2bOderNo, t.b2bIndexNo")
        sb.AppendLine("INTO #Temp_Part2_Check")
        sb.AppendLine("FROM #Temp_Part2_ManagePlan mp")
        sb.AppendLine("LEFT JOIN t_check t ON mp.no = t.no;")

        ' 2.3 加入 m_user 并输出到最终块
        sb.AppendLine("SELECT ")
        sb.AppendLine("    ISNULL(mpc.ck_id,'') AS ck_id, mpc.[CD] AS cd, mpc.[no] AS no, mpc.[line_cd], mpc.chk_user,")
        sb.AppendLine("    CONVERT(varchar(100), mpc.yotei_chk_date , 23) AS yotei_chk_date, CAST(NULL AS varchar(100)) AS chk_start_date, CAST(NULL AS varchar(100)) AS chk_end_date,")
        sb.AppendLine("    mpc.status, mpc.result,")
        sb.AppendLine("    CASE WHEN ISNULL(mpc.t_no,'') = '' THEN 'NG 不良代替 未重检' WHEN mpc.result = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中' END AS buliang,")
        sb.AppendLine("    mpc.chk_times, ISNULL(mpc.suu, mpc.t_suu) AS suu, mpc.b2bOderNo, mpc.b2bIndexNo, mpc.sapOderNo, mpc.sapIndexNo,")
        sb.AppendLine("    b.user_name, mpc.specialBookNo, mpc.DealerAbbreviation AS jxs_name, mpc.mark, ISNULL(mpc.DestinationCode,'') AS xiangxian")
        sb.AppendLine("INTO #Result2")
        sb.AppendLine("FROM #Temp_Part2_Check mpc")
        sb.AppendLine("LEFT JOIN m_user b ON mpc.chk_user = b.user_cd")
        sb.AppendLine("WHERE NOT EXISTS (SELECT 1 FROM #Temp_OK_Check ok WHERE ok.cd = mpc.CD AND ok.no = mpc.no)")
        sb.AppendLine("  AND NOT EXISTS (SELECT 1 FROM #Temp_OK_Book ob WHERE ob.specialBookNo = mpc.specialBookNo);")


        ' =====================================================================================
        ' 第 3 部分：m_bl_manage sapOderNo 关联 (严格双表临时表拆解)
        ' =====================================================================================
        ' 3.1 m_bl_manage 与 t_check_plan 结合
        sb.AppendLine("SELECT a.sapOderNo, a.sapIndexNo, a.specialBookNo, a.DealerAbbreviation, a.mark,")
        sb.AppendLine("       c.CD, c.no, c.line_cd, c.yotei_chk_date, c.suu, c.DestinationCode")
        sb.AppendLine("INTO #Temp_Part3_ManagePlan")
        sb.AppendLine("FROM m_bl_manage a")
        sb.AppendLine("INNER JOIN t_check_plan c ON a.sapOderNo = c.sapOderNo AND a.sapIndexNo = c.sapIndexNo AND a.line_cd = c.line_cd")
        sb.AppendLine("WHERE ISNULL(a.specialBookNo,'') = ''")
        If cd <> "" Then sb.AppendLine("  AND c.cd = '" & cd & "'")
        If no <> "" Then sb.AppendLine("  AND c.no = '" & no & "'")
        sb.AppendLine(";")

        ' 3.2 加入 t_check
        sb.AppendLine("SELECT mp.*, t.ck_id, t.chk_user, t.status, t.result, t.no AS t_no, t.chk_times, t.suu AS t_suu, t.b2bOderNo, t.b2bIndexNo")
        sb.AppendLine("INTO #Temp_Part3_Check")
        sb.AppendLine("FROM #Temp_Part3_ManagePlan mp")
        sb.AppendLine("LEFT JOIN t_check t ON mp.no = t.no;")

        ' 3.3 加入 m_user 并输出到最终块
        sb.AppendLine("SELECT ")
        sb.AppendLine("    ISNULL(mpc.ck_id,'') AS ck_id, mpc.[CD] AS cd, mpc.[no] AS no, mpc.[line_cd], mpc.chk_user,")
        sb.AppendLine("    CONVERT(varchar(100), mpc.yotei_chk_date , 23) AS yotei_chk_date, CAST(NULL AS varchar(100)) AS chk_start_date, CAST(NULL AS varchar(100)) AS chk_end_date,")
        sb.AppendLine("    mpc.status, mpc.result,")
        sb.AppendLine("    CASE WHEN ISNULL(mpc.t_no,'') = '' THEN 'NG 不良代替 未重检' WHEN mpc.result = 'OK' THEN '不良代替 重检OK' ELSE 'NG 不良代替 重检中' END AS buliang,")
        sb.AppendLine("    mpc.chk_times, ISNULL(mpc.suu, mpc.t_suu) AS suu, mpc.b2bOderNo, mpc.b2bIndexNo, mpc.sapOderNo, mpc.sapIndexNo,")
        sb.AppendLine("    b.user_name, mpc.specialBookNo, mpc.DealerAbbreviation AS jxs_name, mpc.mark, ISNULL(mpc.DestinationCode,'') AS xiangxian")
        sb.AppendLine("INTO #Result3")
        sb.AppendLine("FROM #Temp_Part3_Check mpc")
        sb.AppendLine("LEFT JOIN m_user b ON mpc.chk_user = b.user_cd")
        sb.AppendLine("WHERE NOT EXISTS (SELECT 1 FROM #Temp_OK_Check ok WHERE ok.cd = mpc.CD AND ok.no = mpc.no);")

        ' =====================================================================================
        ' 最终输出合并结果并清理内存
        ' =====================================================================================
        ' 恢复计数，使 SqlDataAdapter 能成功捕获到 SELECT 出的结果集
        sb.AppendLine("SET NOCOUNT OFF;")

        ' 利用 UNION 将三个临时表合并后返回给 DataTable
        sb.AppendLine("SELECT * FROM #Result1")
        sb.AppendLine("UNION")
        sb.AppendLine("SELECT * FROM #Result2")
        sb.AppendLine("UNION")
        sb.AppendLine("SELECT * FROM #Result3;")

        ' 养成好习惯，执行完删掉临时表释放 SQL Server 内存
        sb.AppendLine("DROP TABLE #Temp_OK_Check; DROP TABLE #Temp_Plan_Book; DROP TABLE #Temp_OK_Book;")
        sb.AppendLine("DROP TABLE #Temp_Part2_ManagePlan; DROP TABLE #Temp_Part2_Check;")
        sb.AppendLine("DROP TABLE #Temp_Part3_ManagePlan; DROP TABLE #Temp_Part3_Check;")
        sb.AppendLine("DROP TABLE #Result1; DROP TABLE #Result2; DROP TABLE #Result3;")

        ' 直接返回一次查询的结果即可，不需要再在 VB.NET 里面搞 Merge 了
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function
End Class
