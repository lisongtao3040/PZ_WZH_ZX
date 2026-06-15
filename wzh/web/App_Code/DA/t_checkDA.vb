Imports System.Data
Imports Microsoft.VisualBasic
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.CodeDom.Compiler
Imports MSScriptControl


Public Class t_checkDA

    '从CD 获得模版项目
    Function GetKmByCode(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String) As Data.DataTable

        '检索cd 长度 CHADDLWXXX12345
        Dim cdLength As Integer = Trim(cd).Length

        Dim sb As New StringBuilder
        sb.AppendLine("SELECT sys_id FROM m_sys_join WHERE cd = '" & cd & "'")
        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_sys_join")

        sb.Length = 0


        sb.AppendLine("SELECT ROW_NUMBER() over(order by kind_jun,hyouji_jun) AS id")    '检查明细的ID
        sb.AppendLine("     ,id AS km_id")
        For i As Integer = 1 To 20
            sb.AppendLine("     ,F" & i)
        Next
        sb.AppendLine("     ,[length]")
        sb.AppendLine("     ,jizong_name")
        sb.AppendLine("     ,department_cd")
        sb.AppendLine("     ,sys_name")
        sb.AppendLine("     ,kind_name")
        sb.AppendLine("     ,kind_jun")
        sb.AppendLine("     ,isnull(a.pic_name,'')+'|'+isnull(b.pic_upd_time,'')")
        sb.AppendLine("     ,tools_ma")
        sb.AppendLine("     ,chk_pos")
        sb.AppendLine("     ,chk_km_name")
        sb.AppendLine("     ,hyouji_jun")
        sb.AppendLine("     ,yousen_jun")
        sb.AppendLine("     ,k_type")
        sb.AppendLine("     ,k1")
        sb.AppendLine("     ,k2")
        sb.AppendLine("     ,k3")
        sb.AppendLine("     ,chk_fmt")
        sb.AppendLine("     ,chk_fs")
        sb.AppendLine("     ,chk_fs_txt")
        sb.AppendLine("     ,chk_times")

        'sb.AppendLine("     ,ck_id")
        sb.AppendLine("     ,'" & ck_id & "' ck_id")
        sb.AppendLine("     ,'" & cd & "' cd")
        sb.AppendLine("     ,'" & no & "' no")
        sb.AppendLine("     ,'' in_1")
        sb.AppendLine("     ,'' result")
        sb.AppendLine("     ,'' mark")
        'sb.AppendLine("     ,no")
        'sb.AppendLine("     ,in_1")
        'sb.AppendLine("     ,mark")

        sb.AppendLine("     ,'" & user_cd & "' upd_user")
        sb.AppendLine("     ,getdate() upd_date")
        sb.AppendLine("     ,'" & user_cd & "' ins_user")
        sb.AppendLine("     ,getdate() ins_date")
        sb.AppendLine("     ,filter")
        'sb.AppendLine("     ,upd_user")
        'sb.AppendLine("     ,upd_date")
        'sb.AppendLine("     ,ins_user")
        'sb.AppendLine("     ,ins_date")

        'sb.AppendLine(",*")
        sb.AppendLine("FROM m_km_template a")

        sb.AppendLine("  left join (")
        sb.AppendLine("  select ")
        sb.AppendLine("  [pic_name]")
        sb.AppendLine("  ,max([pic_upd_time]) pic_upd_time")
        sb.AppendLine("  from")
        sb.AppendLine("  m_picture_km")
        sb.AppendLine("    group by ")
        sb.AppendLine("  [pic_name]) b on a.[pic_name]=b.[pic_name]")




        sb.AppendLine("WHERE length = " & cdLength & "")
        For i As Integer = 1 To cdLength
            Dim fd As String = "F" & i
            Dim tcd As String = cd.Substring(i - 1, 1)
            sb.AppendLine(" AND (" & fd & " like '%" & tcd & "%'")
            sb.AppendLine("     OR  ltrim(" & fd & ")=''")
            sb.AppendLine(")")
        Next

        If dt.Rows.Count > 0 Then
            sb.AppendLine("AND sys_name='" & dt.Rows(0).Item(0).ToString & "'")
        End If

        sb.AppendLine("ORDER BY kind_jun,hyouji_jun")


        Dim dtMs As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetTableInfo")


        Return dtMs




    End Function

    Public Function GetZenMs(ByVal cd As String, ByVal no As String, ByVal ck_id As String) As DataTable

        Dim sb As New StringBuilder
        'sb.AppendLine("  select a.* from [t_check_ms] a")
        'sb.AppendLine("  inner join t_check b")
        'sb.AppendLine("  on a.ck_id=b.ck_id")
        'sb.AppendLine("  where a.ck_id in")
        'sb.AppendLine("  (")
        'sb.AppendLine("  select max(ck_id)")
        'sb.AppendLine("  FROM [t_check]")
        'sb.AppendLine("  where ck_id<'" & ck_id & "'")
        'sb.AppendLine("  and cd = '" & cd & "' ")
        'sb.AppendLine("  and no = '" & no & "'")
        'sb.AppendLine("  )")
        'sb.AppendLine("  and b.result = 'NG'")

        'Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_sys_join")
        'If dt.Rows.Count = 0 Then
        '    Return Nothing
        'End If
        'sb.Clear()

        sb.AppendLine("  select a.* from [t_check_ms] a")
        sb.AppendLine("  inner join t_check b")
        sb.AppendLine("  on a.ck_id=b.ck_id")
        sb.AppendLine("  where a.ck_id in")
        sb.AppendLine("  (")
        'sb.AppendLine("  select max(ck_id)")
        sb.AppendLine("  select ck_id")
        sb.AppendLine("  FROM [t_check]")
        sb.AppendLine("  where ck_id<'" & ck_id & "'")
        sb.AppendLine("  and cd = '" & cd & "' ")
        sb.AppendLine("  and no = '" & no & "'")
        sb.AppendLine("  and result <> 'OK'")
        sb.AppendLine("  )")
        sb.AppendLine("  and b.result = 'NG'")
        sb.AppendLine("  and isnull(a.result,'') not in('OK','SD')")

        sb.AppendLine("ORDER BY a.ck_id desc,a.kind_jun asc,hyouji_jun asc")

        Dim dtZen As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_sys_join")

        Dim oldCk_id As String = ""
        dtZen.Columns.Add("row_no")
        Dim idx As Integer = 0
        Dim maxIdx As Integer = 0
        For i As Integer = 0 To dtZen.Rows.Count - 1
            If (oldCk_id <> dtZen.Rows(i).Item("ck_id").ToString) Then
                idx = idx + 1
            End If
            oldCk_id = dtZen.Rows(i).Item("ck_id").ToString
            dtZen.Rows(i).Item("row_no") = idx

        Next

        maxIdx = idx + 1

        For i As Integer = 0 To dtZen.Rows.Count - 1
            'If maxIdx - CInt(dtZen.Rows(i).Item("row_no")) = maxIdx - 1 Then
            '    dtZen.Rows(i).Item("row_no") = "前回"
            'ElseIf maxIdx - CInt(dtZen.Rows(i).Item("row_no")) = maxIdx - 2 Then
            '    dtZen.Rows(i).Item("row_no") = "前前回"
            'Else
            '    dtZen.Rows(i).Item("row_no") = maxIdx - CInt(dtZen.Rows(i).Item("row_no"))
            'End If
            dtZen.Rows(i).Item("row_no") = maxIdx - CInt(dtZen.Rows(i).Item("row_no"))
        Next



        Return dtZen

        'For k As Integer = 0 To dtZen.Rows.Count - 1
        '    For i As Integer = 0 To dtMs.Rows.Count - 1
        '        If (dtMs.Rows(i).Item("id").ToString = dtZen.Rows(k).Item("id").ToString AndAlso dtMs.Rows(i).Item("chk_times").ToString = dtZen.Rows(k).Item("chk_times").ToString) Then
        '            dtMs.Rows(i).Item("mark") = dtZen.Rows(k).Item("mark").ToString

        '        End If
        '    Next
        'Next

    End Function

    't_check 表的 宽高信息等
    Function GetCheckOne(ByVal ck_cd As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("      a.h")
        sb.AppendLine("      ,a.w")
        sb.AppendLine("      ,a.dh")
        sb.AppendLine("      ,a.dw")
        sb.AppendLine("      ,a.sw")
        sb.AppendLine("      ,a.kw")
        sb.AppendLine("      ,a.b2bOderNo")
        sb.AppendLine("      ,a.b2bIndexNo")
        sb.AppendLine("      ,a.tools_scan_flg")
        sb.AppendLine("      ,b.jxs_name")
        sb.AppendLine("      FROM t_check a")
        sb.AppendLine("      LEFT JOIN t_check_plan b")
        sb.AppendLine("      ON a.cd = b.cd and a.no = b.no")
        sb.AppendLine("WHERE a.ck_id = '" & ck_cd & "'")
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkOne")
    End Function

    Function GetCheckLineCd(ByVal ck_cd As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("      a.line_cd,isnull(a.result,'') result")

        sb.AppendLine("      FROM t_check a")

        sb.AppendLine("WHERE a.ck_id = '" & ck_cd & "'")
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "line_cd")
    End Function


    Function GetCheckList(ByVal cd As String, ByVal no As String, ByVal department_cd As String, ByVal line_cd As String, ByVal rinei As String, ByVal user_cd As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT isnull(t.ck_id,'') ck_id")
        sb.AppendLine("      ,isnull(c.cd,t.cd) cd")
        sb.AppendLine("      ,isnull(c.no,t.no) no")
        sb.AppendLine("      ,t.department_cd")
        'sb.AppendLine("      ,c.line_cd")
        sb.AppendLine("      ,isnull(c.line_cd,t.line_cd) line_cd")
        sb.AppendLine("      ,t.chk_user")
        'sb.AppendLine("      ,Convert(varchar(100), c.yotei_chk_date, 23) as yotei_chk_date")
        sb.AppendLine("      ,Convert(varchar(100), isnull(c.yotei_chk_date,t.yotei_chk_date) , 23) as yotei_chk_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) as chk_start_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) as chk_end_date")
        'sb.AppendLine("      ,t.chk_start_date")
        'sb.AppendLine("      ,t.chk_end_date")
        sb.AppendLine("      ,t.status")
        sb.AppendLine("      ,t.result")

        sb.AppendLine("      ,t.chk_times")

        'sb.AppendLine("      ,c.suu")
        sb.AppendLine("      ,isnull(c.suu,t.suu) suu")
        sb.AppendLine("      ,t.del_flg")
        sb.AppendLine("      ,isnull(t.qianpin,'0') qianpin")
        sb.AppendLine("      ,isnull(t.tools_scan_flg,'0') tools_scan_flg")
        sb.AppendLine("      ,t.shared_ck_id")
        sb.AppendLine("      ,t.shared_no")

        sb.AppendLine("      ,t.h")
        sb.AppendLine("      ,t.w")
        sb.AppendLine("      ,t.dh")
        sb.AppendLine("      ,t.dw")
        sb.AppendLine("      ,t.sw")
        sb.AppendLine("      ,t.kw")
        sb.AppendLine("      ,t.b2bOderNo")
        sb.AppendLine("      ,t.b2bIndexNo")
        sb.AppendLine("      ,isnull(c.sapOderNo,t.sapOderNo) sapOderNo")
        sb.AppendLine("      ,isnull(c.sapIndexNo,t.sapIndexNo) sapIndexNo")

        sb.AppendLine("      ,t.upd_user")
        sb.AppendLine("      ,t.upd_date")
        sb.AppendLine("      ,t.ins_user")
        sb.AppendLine("      ,t.ins_date")
        sb.AppendLine("      ,b.user_name")
        'sb.AppendLine("      ,c.specialBookNo")
        sb.AppendLine("      ,isnull(c.specialBookNo,t.specialBookNo) specialBookNo")
        sb.AppendLine("FROM t_check t")
        sb.AppendLine("LEFT JOIN m_user b")
        sb.AppendLine("ON t.chk_user = b.user_cd")
        sb.AppendLine("RIGHT JOIN t_check_plan c")
        sb.AppendLine("ON t.cd = c.cd")
        sb.AppendLine("AND t.no = c.no")
        sb.AppendLine("AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")

        sb.AppendLine("WHERE  1=1")

        If cd.Trim <> "" Then   '商品cd
            sb.AppendLine("	AND isnull(c.cd,t.cd) = '" & cd & "'")
            'sb.AppendLine("	AND (c.cd = '" & cd & "' OR replace(c.cd,'-','') = '" & cd & "')")
        End If
        If no.Trim <> "" Then   '作番
            sb.AppendLine("	AND isnull(c.no,t.no) = '" & no & "'")
        End If

        If department_cd.Trim <> "" Then    '部门
            sb.AppendLine("	AND c.department_cd in(" & department_cd & ")")
        End If

        If user_cd <> "admin" Then
            If line_cd.Trim <> "" Then  '生产线
                'sb.AppendLine("	AND c.line_cd = '" & line_cd & "'")
            End If
        End If

        sb.AppendLine("	    AND isnull(c.yotei_chk_date,t.yotei_chk_date)>dateadd(day,-" & rinei & ",getdate())")
        sb.AppendLine("	    AND isnull(c.yotei_chk_date,t.yotei_chk_date)<dateadd(day," & 1 & ",getdate())")

        'sb.AppendLine("	ORDER BY t.chk_end_date desc,t.chk_start_date desc")
        sb.AppendLine("	ORDER BY isnull(t.chk_start_date,getdate()) desc,t.chk_end_date desc")
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function



    '主表计划中 获得相关检查数据
    Function GetBuliangByCdNo(ByVal cd As String, ByVal no As String) As DataTable

        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")

        sb.AppendLine("      CASE WHEN ISNULL(buliangdaiti.CD,'') = '' THEN '0' ELSE '1' END buliang_daiti")


        sb.AppendLine("FROM t_check_plan c")    '计划
        sb.AppendLine("LEFT JOIN (")
        sb.AppendLine("SELECT a.* ")
        sb.AppendLine("FROM ")
        sb.AppendLine("	[m_bl_manage] a")
        sb.AppendLine("INNER JOIN (")
        sb.AppendLine("	 SELECT MAX([guid]) AS [guid]")
        sb.AppendLine("	  FROM [m_bl_manage]")
        sb.AppendLine("	  GROUP BY ")
        sb.AppendLine("		   [CD]")
        sb.AppendLine("		  ,[specialBookNo]")
        sb.AppendLine("		  ,[sapOderNo]")
        sb.AppendLine("		  ,[sapIndexNo]")
        sb.AppendLine("	  ) b")
        sb.AppendLine("ON   a.[guid] = b.[guid]")
        sb.AppendLine(") buliangdaiti")

        sb.AppendLine("     ON c.cd = buliangdaiti.CD")
        sb.AppendLine("     AND c.line_cd = buliangdaiti.line_cd")
        sb.AppendLine("     AND(")
        sb.AppendLine("          ltrim(c.specialBookNo) = ltrim(isnull(buliangdaiti.specialBookNo,''))")
        sb.AppendLine("          OR(     ltrim(c.sapOderNo) = ltrim(isnull(buliangdaiti.sapOderNo,''))")
        sb.AppendLine("              AND ltrim(c.sapIndexNo) = ltrim(isnull(buliangdaiti.sapIndexNo,'')) )")
        sb.AppendLine("     )")

        'sb.AppendLine("RIGHT JOIN t_check_plan c")
        'sb.AppendLine("ON t.cd = c.cd")
        'sb.AppendLine("AND t.no = c.no")
        'sb.AppendLine("AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")
        sb.AppendLine("WHERE  1=1")

        '托盘Nos 不为空 以托盘Nos检索
        If cd <> "" Then
            sb.AppendLine("	AND isnull(c.cd,c.cd) = '" & cd & "'")
        End If
        If no <> "" Then
            sb.AppendLine("	AND isnull(c.no,c.no) = '" & no & "'")
        End If


        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function


    '主表计划中 获得相关检查数据
    Function GetCheckListForChkPlan(ByVal cd As String, ByVal no As String, ByVal tpNos As String) As DataTable

        Dim sb As New StringBuilder
        sb.AppendLine("SELECT isnull(t.ck_id,'') ck_id")
        sb.AppendLine("      ,isnull(c.cd,t.cd) cd")
        sb.AppendLine("      ,isnull(c.no,t.no) no")
        sb.AppendLine("      ,t.department_cd")
        'sb.AppendLine("      ,c.line_cd")
        sb.AppendLine("      ,isnull(c.line_cd,t.line_cd) line_cd")
        sb.AppendLine("      ,t.chk_user")
        'sb.AppendLine("      ,Convert(varchar(100), c.yotei_chk_date, 23) as yotei_chk_date")
        sb.AppendLine("      ,Convert(varchar(100), isnull(c.yotei_chk_date,t.yotei_chk_date) , 23) as yotei_chk_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) as chk_start_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) as chk_end_date")
        'sb.AppendLine("      ,t.chk_start_date")
        'sb.AppendLine("      ,t.chk_end_date")
        sb.AppendLine("      ,t.status")
        sb.AppendLine("      ,t.result")
        sb.AppendLine("      ,CASE WHEN ISNULL(t.result,'') = 'NG' THEN '1' ELSE '0' END buliang")
        sb.AppendLine("      ,CASE WHEN ISNULL(buliangdaiti.CD,'') = '' THEN '0' ELSE '1' END buliang_daiti")
        sb.AppendLine("      ,t.chk_times")
        'sb.AppendLine("      ,c.suu")
        sb.AppendLine("      ,isnull(c.suu,t.suu) suu")
        sb.AppendLine("      ,t.del_flg")
        sb.AppendLine("      ,isnull(t.qianpin,'0') qianpin")
        sb.AppendLine("      ,isnull(t.tools_scan_flg,'0') tools_scan_flg")
        sb.AppendLine("      ,t.shared_ck_id")
        sb.AppendLine("      ,t.shared_no")
        sb.AppendLine("      ,t.h")
        sb.AppendLine("      ,t.w")
        sb.AppendLine("      ,t.dh")
        sb.AppendLine("      ,t.dw")
        sb.AppendLine("      ,t.sw")
        sb.AppendLine("      ,t.kw")
        sb.AppendLine("      ,t.b2bOderNo")
        sb.AppendLine("      ,t.b2bIndexNo")
        sb.AppendLine("      ,isnull(c.sapOderNo,t.sapOderNo) sapOderNo")
        sb.AppendLine("      ,isnull(c.sapIndexNo,t.sapIndexNo) sapIndexNo")
        sb.AppendLine("      ,t.upd_user")
        sb.AppendLine("      ,t.upd_date")
        sb.AppendLine("      ,t.ins_user")
        sb.AppendLine("      ,t.ins_date")
        sb.AppendLine("      ,b.user_name")
        'sb.AppendLine("      ,c.specialBookNo")
        sb.AppendLine("      ,isnull(c.specialBookNo,t.specialBookNo) specialBookNo")
        sb.AppendLine("      ,isnull(c.jxs_name,'') jxs_name")

        sb.AppendLine("      ,isnull(pre.ck_id,'') chlid_ck_id")
        sb.AppendLine("      ,isnull(chlid.pre_ck_id,'') pre_ck_id")
        sb.AppendLine("      ,isnull(i.result,'') child_result")

        sb.AppendLine("      ,isnull(i.ck_id,'') ss_id")

        sb.AppendLine("      ,isnull(vifc.ISFIRSTCHK,'') ISFIRSTCHK")
        sb.AppendLine("      ,case when isnull(first2_ty.cd,'') <> '' and  first2.ISFIRSTCHK is null then '1' else '0' end ISFIRSTCHK2") '初①
        'sb.AppendLine("      ,case when first_chk_cds.cd is not null then isnull(first2.cd,'') else 'ok' end ISFIRSTCHK2")

        sb.AppendLine("FROM t_check_plan c")    '计划
        sb.AppendLine("LEFT JOIN t_check t")    '检查
        sb.AppendLine("     ON t.cd = c.cd")
        sb.AppendLine("     AND t.no = c.no")
        sb.AppendLine("     AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")
        sb.AppendLine("LEFT JOIN m_user b")     '用户
        sb.AppendLine("     ON t.chk_user = b.user_cd")

        sb.AppendLine("LEFT JOIN v_is_first_chk vifc")
        sb.AppendLine("     ON vifc.cd = c.cd")

        'm_first_chk_cds

        sb.AppendLine("LEFT JOIN m_first_chk_cds first2_ty")
        sb.AppendLine("     ON replace(first2_ty.cd,'-','') = replace(c.cd,'-','')")


        sb.AppendLine("LEFT JOIN v_is_first_chk2 first2")
        sb.AppendLine("     ON first2.cd = c.cd")
        sb.AppendLine("     AND first2.line_cd = c.line_cd")

        'pre_ck_id 父
        sb.AppendLine("LEFT JOIN t_pre_chk_id pre")
        sb.AppendLine("ON pre.pre_ck_id = t.ck_id")

        sb.AppendLine("LEFT JOIN t_pre_chk_id chlid")
        sb.AppendLine("ON chlid.ck_id = t.ck_id")

        sb.AppendLine("LEFT JOIN t_check i")
        sb.AppendLine("ON i.ck_id = pre.ck_id")


        '不良代替
        sb.AppendLine("LEFT JOIN (")
        sb.AppendLine("SELECT a.* ")
        sb.AppendLine("FROM ")
        sb.AppendLine("	[m_bl_manage] a")
        sb.AppendLine("INNER JOIN (")
        sb.AppendLine("	 SELECT MAX([guid]) AS [guid]")
        sb.AppendLine("	  FROM [m_bl_manage]")
        sb.AppendLine("	  GROUP BY ")
        sb.AppendLine("		   [CD]")
        sb.AppendLine("		  ,[specialBookNo]")
        sb.AppendLine("		  ,[sapOderNo]")
        sb.AppendLine("		  ,[sapIndexNo]")
        sb.AppendLine("	  ) b")
        sb.AppendLine("ON   a.[guid] = b.[guid]")
        sb.AppendLine(") buliangdaiti")

        sb.AppendLine("     ON c.cd = buliangdaiti.CD")
        sb.AppendLine("     AND(")
        sb.AppendLine("          ltrim(isnull(c.specialBookNo,t.specialBookNo)) = ltrim(isnull(buliangdaiti.specialBookNo,''))")
        sb.AppendLine("          OR(     ltrim(isnull(c.sapOderNo,t.sapOderNo)) = ltrim(isnull(buliangdaiti.sapOderNo,''))")
        sb.AppendLine("              AND ltrim(isnull(c.sapIndexNo,t.sapIndexNo)) = ltrim(isnull(buliangdaiti.sapIndexNo,'')) )")
        sb.AppendLine("     )")

        'sb.AppendLine("RIGHT JOIN t_check_plan c")
        'sb.AppendLine("ON t.cd = c.cd")
        'sb.AppendLine("AND t.no = c.no")
        'sb.AppendLine("AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")
        sb.AppendLine("WHERE  1=1")

        '托盘Nos 不为空 以托盘Nos检索
        If tpNos <> "" Then
            sb.AppendLine("	AND isnull(c.no,t.no) in (" & tpNos & ")")
        Else
            If cd <> "" Then
                sb.AppendLine("	AND isnull(c.cd,t.cd) = '" & cd & "'")
            End If
            If no <> "" Then
                sb.AppendLine("	AND isnull(c.no,t.no) = '" & no & "'")
            End If
        End If

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function





    Function GetCheckListByChk(ByVal cd As String, ByVal no As String, ByVal department_cd As String, ByVal line_cd As String, ByVal rinei As String, ByVal user_cd As String, ByVal tpNos As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT isnull(t.ck_id,'') ck_id")
        sb.AppendLine("      ,t.cd cd")
        sb.AppendLine("      ,t.no no")
        sb.AppendLine("      ,t.department_cd")
        'sb.AppendLine("      ,c.line_cd")
        sb.AppendLine("      ,t.line_cd line_cd")
        sb.AppendLine("      ,t.chk_user")
        'sb.AppendLine("      ,Convert(varchar(100), c.yotei_chk_date, 23) as yotei_chk_date")
        sb.AppendLine("      ,Convert(varchar(100), t.yotei_chk_date , 23) as yotei_chk_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_start_date, 11) + ' ' + CONVERT(varchar(100), t.chk_start_date, 24) as chk_start_date")
        sb.AppendLine("      ,CONVERT(varchar(100), t.chk_end_date, 11) + ' ' + CONVERT(varchar(100), t.chk_end_date, 24) as chk_end_date")
        'sb.AppendLine("      ,t.chk_start_date")
        'sb.AppendLine("      ,t.chk_end_date")
        sb.AppendLine("      ,t.status")
        sb.AppendLine("      ,t.result")
        sb.AppendLine("      ,CASE WHEN ISNULL(t.result,'') = 'NG' THEN '1' ELSE '0' END buliang")
        sb.AppendLine("      ,CASE WHEN ISNULL(buliangdaiti.CD,'') = '' THEN '0' ELSE '1' END buliang_daiti")

        sb.AppendLine("      ,t.chk_times")
        'sb.AppendLine("      ,c.suu")
        sb.AppendLine("      ,t.suu suu")
        sb.AppendLine("      ,t.del_flg")
        sb.AppendLine("      ,isnull(t.qianpin,'0') qianpin")
        sb.AppendLine("      ,isnull(t.tools_scan_flg,'0') tools_scan_flg")
        sb.AppendLine("      ,t.shared_ck_id")
        sb.AppendLine("      ,t.shared_no")

        sb.AppendLine("      ,t.h")
        sb.AppendLine("      ,t.w")
        sb.AppendLine("      ,t.dh")
        sb.AppendLine("      ,t.dw")
        sb.AppendLine("      ,t.sw")
        sb.AppendLine("      ,t.kw")
        sb.AppendLine("      ,t.b2bOderNo")
        sb.AppendLine("      ,t.b2bIndexNo")
        sb.AppendLine("      ,isnull(c.sapOderNo,t.sapOderNo) sapOderNo")
        sb.AppendLine("      ,isnull(c.sapIndexNo,t.sapIndexNo) sapIndexNo")

        sb.AppendLine("      ,t.upd_user")
        sb.AppendLine("      ,t.upd_date")
        sb.AppendLine("      ,t.ins_user")
        sb.AppendLine("      ,t.ins_date")
        sb.AppendLine("      ,b.user_name")
        sb.AppendLine("      ,isnull(c.specialBookNo,t.specialBookNo) specialBookNo")
        sb.AppendLine("      ,isnull(c.jxs_name,'') jxs_name")
        'sb.AppendLine("      ,'' specialBookNo")

        sb.AppendLine("      ,isnull(pre.ck_id,'') chlid_ck_id")
        sb.AppendLine("      ,isnull(chlid.pre_ck_id,'') pre_ck_id")
        sb.AppendLine("      ,isnull(i.result,'') child_result")
        sb.AppendLine("      ,isnull(i.ck_id,'') ss_id")
        sb.AppendLine("      ,isnull(vifc.ISFIRSTCHK,'') ISFIRSTCHK")
        'sb.AppendLine("      ,isnull(first2.ISFIRSTCHK,'') ISFIRSTCHK2")
        sb.AppendLine("      ,case when isnull(first2_ty.cd,'') <> '' and first2.ISFIRSTCHK is null then '1' else '0' end ISFIRSTCHK2")

        'sb.AppendLine("      ,case when first_chk_cds.cd is not null then isnull(first2.cd,'') else 'ok' end ISFIRSTCHK2")

        sb.AppendLine("FROM t_check t")
        sb.AppendLine("LEFT JOIN m_user b")
        sb.AppendLine("     ON t.chk_user = b.user_cd")


        sb.AppendLine("LEFT JOIN t_check_plan c")
        sb.AppendLine("     ON t.cd = c.cd")
        sb.AppendLine("     AND t.no = c.no")

        sb.AppendLine("LEFT JOIN v_is_first_chk vifc")
        sb.AppendLine("     ON vifc.cd = c.cd")


        sb.AppendLine("LEFT JOIN m_first_chk_cds first2_ty")
        sb.AppendLine("     ON replace(first2_ty.cd,'-','') = replace(c.cd,'-','')")


        sb.AppendLine("LEFT JOIN v_is_first_chk2 first2")
        sb.AppendLine("     ON first2.cd = c.cd")
        sb.AppendLine("     AND first2.line_cd = c.line_cd")

        'pre_ck_id 父
        sb.AppendLine("LEFT JOIN t_pre_chk_id pre")
        sb.AppendLine("ON pre.pre_ck_id = t.ck_id")

        sb.AppendLine("LEFT JOIN t_pre_chk_id chlid")
        sb.AppendLine("ON chlid.ck_id = t.ck_id")

        sb.AppendLine("LEFT JOIN t_check i")
        sb.AppendLine("ON i.ck_id = pre.ck_id")

        '不良代替
        sb.AppendLine("LEFT JOIN (")
        sb.AppendLine("SELECT a.* ")
        sb.AppendLine("FROM ")
        sb.AppendLine("	[m_bl_manage] a")
        sb.AppendLine("INNER JOIN (")
        sb.AppendLine("	 SELECT MAX([guid]) AS [guid]")
        sb.AppendLine("	  FROM [m_bl_manage]")
        sb.AppendLine("	  GROUP BY ")
        sb.AppendLine("		   [CD]")
        sb.AppendLine("		  ,[specialBookNo]")
        sb.AppendLine("		  ,[sapOderNo]")
        sb.AppendLine("		  ,[sapIndexNo]")
        sb.AppendLine("	  ) b")
        sb.AppendLine("ON   a.[guid] = b.[guid]")
        sb.AppendLine(") buliangdaiti")

        sb.AppendLine("     ON t.cd = buliangdaiti.CD")

        sb.AppendLine("     AND(")
        sb.AppendLine("          ltrim(isnull(c.specialBookNo,t.specialBookNo)) = ltrim(isnull(buliangdaiti.specialBookNo,''))")
        sb.AppendLine("          OR(     ltrim(isnull(c.sapOderNo,t.sapOderNo)) = ltrim(isnull(buliangdaiti.sapOderNo,''))")
        sb.AppendLine("              AND ltrim(isnull(c.sapIndexNo,t.sapIndexNo)) = ltrim(isnull(buliangdaiti.sapIndexNo,'')) )")
        sb.AppendLine("     )")

        'sb.AppendLine("AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")

        sb.AppendLine("WHERE  1=1")

        If department_cd.Trim <> "" Then    '部门
            sb.AppendLine("	AND (t.department_cd in(" & department_cd & ") or t.department_cd='')")
        End If

        If user_cd <> "admin" Then
            If line_cd.Trim <> "" Then  '生产线
                'sb.AppendLine("	AND c.line_cd = '" & line_cd & "'")
            End If
        End If

        If tpNos <> "" Then
            sb.AppendLine("	AND isnull(t.no,'') in (" & tpNos & ")")
        Else
            If cd.Trim <> "" Then   '商品cd
                sb.AppendLine("	AND t.cd = '" & cd & "'")
            End If
            If no.Trim <> "" Then   '作番
                sb.AppendLine("	AND t.no = '" & no & "'")
            End If
            sb.AppendLine("	    AND t.yotei_chk_date>dateadd(day,-" & rinei & ",getdate())")
            sb.AppendLine("	    AND t.yotei_chk_date<dateadd(day," & rinei & ",getdate())")
        End If

        sb.AppendLine("	ORDER BY isnull(t.chk_start_date,getdate()) desc,t.chk_end_date desc")
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function

    Function GetSameDaySameCd(ByVal cd As String, ByVal line_cd As String, ByVal department_cd As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT isnull(t.ck_id,'') ck_id")
        sb.AppendLine("      ,c.cd")
        sb.AppendLine("      ,c.no")
        sb.AppendLine("      ,t.status")
        sb.AppendLine("      ,Convert(varchar(100), t.chk_end_date, 23) chk_end_date")
        sb.AppendLine("FROM t_check t")
        sb.AppendLine("LEFT JOIN m_user b")
        sb.AppendLine("ON t.chk_user = b.user_cd")
        sb.AppendLine("RIGHT JOIN t_check_plan c")
        sb.AppendLine("ON t.cd = c.cd")
        sb.AppendLine("AND t.no = c.no")
        'sb.AppendLine("AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")

        sb.AppendLine("WHERE  1=1")

        If cd.Trim <> "" Then   '商品cd
            sb.AppendLine("	AND c.cd = '" & cd & "'")
        End If

        If department_cd.Trim <> "" Then    '部门
            sb.AppendLine("	AND c.department_cd in(" & department_cd & ")")
        End If

        'If user_cd <> "admin" Then
        If line_cd.Trim <> "" Then  '生产线
            sb.AppendLine("	AND c.line_cd = '" & line_cd & "'")
        End If
        'End If

        '手动输入完了状态
        sb.AppendLine("	AND t.status = '1'")
        'sb.AppendLine("	AND Convert(varchar(100), t.chk_start_date, 23) <= Convert(varchar(100), getdate(), 23)")
        sb.AppendLine("	AND Convert(varchar(100), t.chk_start_date, 23) >= Convert(varchar(100), dateadd(day,-1,getdate()), 23)")
        sb.AppendLine("	ORDER BY t.chk_end_date desc")
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkList")

    End Function

    Function GetCheckMs(ByVal cLoginInfo As CLoginInfo, ByVal ck_id As String, ByVal kind_name As String, Optional ByVal toolStyle As Boolean = False, Optional ByVal tool_txt As String = "") As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT ms.[id]")
        sb.AppendLine("      , ms.[km_id]")
        sb.AppendLine("      , ms.[F1]")
        sb.AppendLine("      , ms.[F2]")
        sb.AppendLine("      , ms.[F3]")
        sb.AppendLine("      , ms.[F4]")
        sb.AppendLine("      , ms.[F5]")
        sb.AppendLine("      , ms.[F6]")
        sb.AppendLine("      , ms.[F7]")
        sb.AppendLine("      , ms.[F8]")
        sb.AppendLine("      , ms.[F9]")
        sb.AppendLine("      , ms.[F10]")
        sb.AppendLine("      , ms.[F11]")
        sb.AppendLine("      , ms.[F12]")
        sb.AppendLine("      , ms.[F13]")
        sb.AppendLine("      , ms.[F14]")
        sb.AppendLine("      , ms.[F15]")
        sb.AppendLine("      , ms.[F16]")
        sb.AppendLine("      , ms.[F17]")
        sb.AppendLine("      , ms.[F18]")
        sb.AppendLine("      , ms.[F19]")
        sb.AppendLine("      , ms.[F20]")
        sb.AppendLine("      , ms.[length]")
        sb.AppendLine("      , ms.[jizong_name]")
        sb.AppendLine("      , ms.[department_cd]")
        sb.AppendLine("      , ms.[sys_name]")
        sb.AppendLine("      , ms.[kind_name]")
        sb.AppendLine("      , ms.[kind_jun]")
        sb.AppendLine("      , ms.[pic_name]")
        sb.AppendLine("      , ms.[tools_ma]")
        sb.AppendLine("      , ms.[chk_pos]")
        sb.AppendLine("      , ms.[chk_km_name]")
        sb.AppendLine("      , ms.[hyouji_jun]")
        sb.AppendLine("      , ms.[yousen_jun]")
        sb.AppendLine("      , ms.[k_type]")
        sb.AppendLine("      , ms.[k1]")
        sb.AppendLine("      , ms.[k2]")
        sb.AppendLine("      , ms.[k3]")
        sb.AppendLine("      , ms.[chk_fmt]")
        sb.AppendLine("      , ms.[chk_fs]")
        sb.AppendLine("      , ms.[chk_fs_txt]")
        sb.AppendLine("      , ms.[chk_times]")
        sb.AppendLine("      , ms.[ck_id]")
        sb.AppendLine("      , ms.[cd]")
        sb.AppendLine("      , ms.[no]")
        sb.AppendLine("      , ms.[in_1]")
        sb.AppendLine("      , ms.[result]")
        sb.AppendLine("      , ms.[mark]")
        sb.AppendLine("      , ms.[upd_user]")
        sb.AppendLine("      , ms.[upd_date]")
        sb.AppendLine("      , ms.[ins_user]")
        sb.AppendLine("      , ms.[ins_date]")

        sb.AppendLine("      , t.suu")    '未实装  数量
        sb.AppendLine("      , ms.[tools_ma] tool_txt")

        sb.AppendLine("  FROM t_check t")
        sb.AppendLine("  INNER JOIN [t_check_ms] ms")
        sb.AppendLine("  ON t.ck_id = ms.ck_id")

        'sb.AppendLine("  LEFT JOIN t_check_plan c")
        'sb.AppendLine("  ON t.cd = c.cd")
        'sb.AppendLine("  AND t.no = c.no")
        'sb.AppendLine("  AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")

        sb.AppendLine("  LEFT JOIN m_tools_new d")
        sb.AppendLine("  ON ms.tools_ma = d.barcode")
        sb.AppendLine("  AND t.department_cd = d.department_cd")

        sb.AppendLine("WHERE  ms.ck_id = '" & ck_id & "' AND t.ck_id = '" & ck_id & "'")

        If kind_name.Trim <> "" Then   '项目种类
            sb.AppendLine("	AND ms.kind_name = '" & kind_name & "'")
        End If

        If toolStyle Then
            sb.AppendLine("	AND isnull(ms.tools_ma,'')<>''")
        Else
            sb.AppendLine("	AND isnull(ms.tools_ma,'')=''")
        End If

        If tool_txt.Trim <> "" Then
            sb.AppendLine("	AND isnull(d.barcode,ms.[tools_ma])='" & tool_txt.Trim & "'")
        End If


        sb.AppendLine("ORDER BY ms.[kind_jun],ms.hyouji_jun")

        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkMs")
        dt.Columns.Add("row_no")
        For i As Integer = 0 To dt.Rows.Count - 1
            dt.Rows(i).Item("row_no") = （i + 1).ToString
        Next

        Return dt


    End Function




    Public Function GetCheckToolsData(ByVal ck_id As String, ByVal cLoginInfo As CLoginInfo) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT Distinct ")
        'sb.AppendLine("      , ms.[km_id]")
        'sb.AppendLine("      , ms.[km_id]")
        'sb.AppendLine("      , ms.[jizong_name]")
        'sb.AppendLine("      , ms.[department_cd]")
        sb.AppendLine("       ms.[tools_ma]")
        sb.AppendLine("      , t.[tools_scan_flg]")

        sb.AppendLine("      , mt.[remarks]")
        sb.AppendLine("  FROM [t_check_ms] ms")
        sb.AppendLine("  INNER JOIN t_check t")

        sb.AppendLine("  ON t.ck_id = ms.ck_id")
        sb.AppendLine("  LEFT JOIN m_tools_new mt")
        sb.AppendLine("  ON ms.tools_ma = mt.barcode")
        sb.AppendLine("  AND t.department_cd = mt.department_cd")


        'cLoginInfo
        sb.AppendLine("WHERE  t.ck_id = '" & ck_id & "'")
        sb.AppendLine("AND  isnull(ms.[tools_ma],'')<>'' ")
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkMs")
    End Function

    Function GetKmDt(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String) As DataTable




        Dim dt As Data.DataTable = GetKmByCode(cd, no, ck_id, user_cd)

        '检查次数 变形
        Dim dtReal As DataTable = dt.Clone
        dtReal.Rows.Clear()

        Dim id As Integer = 1
        For i As Integer = 0 To dt.Rows.Count - 1

            Dim times As Integer = CInt(dt.Rows(i).Item("chk_times"))
            For j As Integer = 1 To times
                Dim dr As DataRow = dtReal.NewRow
                For k As Integer = 0 To dt.Columns.Count - 1
                    If k = 0 Then
                        dr.Item(k) = id
                    Else
                        dr.Item(k) = dt.Rows(i).Item(k)
                    End If
                Next
                dr.Item("chk_times") = j
                dtReal.Rows.Add(dr)
                id = id + 1
            Next
        Next

        'Dim dtZen As DataTable = GetZenMs(cd, no, ck_id)

        'For k As Integer = 0 To dtZen.Rows.Count - 1
        '    For i As Integer = 0 To dtReal.Rows.Count - 1
        '        If (dtReal.Rows(i).Item("id").ToString = dtZen.Rows(k).Item("id").ToString AndAlso dtReal.Rows(i).Item("chk_times").ToString = dtZen.Rows(k).Item("chk_times").ToString) Then
        '            dtReal.Rows(i).Item("mark") = dtZen.Rows(k).Item("mark").ToString
        '        End If
        '    Next
        'Next


        Return dtReal
    End Function

    Public Function Calculate(ByVal expression As String) As Object
        Dim className As String = "clsF"
        Dim methodName As String = "funCal"

        Dim classSource As New System.Text.StringBuilder

        classSource.Append("public   class   " + className + vbCrLf)
        classSource.Append("         public  function " + methodName + "() as object" + vbCrLf)
        classSource.Append("                 return   " + expression + vbCrLf)
        classSource.Append("         end function" + vbCrLf)
        classSource.Append("end class")

        Dim codeProvider As New VBCodeProvider
        Dim cParams As New CompilerParameters
        cParams.GenerateExecutable = False
        cParams.GenerateInMemory = False
        Dim cResults As CompilerResults = codeProvider.CompileAssemblyFromSource(cParams, classSource.ToString)
        Dim asm As System.Reflection.Assembly = cResults.CompiledAssembly
        Dim eval As Object = asm.CreateInstance(className)
        Dim method As System.Reflection.MethodInfo = eval.GetType().GetMethod(methodName)
        Dim args() As String = Nothing
        Dim reObj As Object = method.Invoke(eval, args)

        GC.Collect()
        Return reObj
    End Function


    '登录新的检查项目
    Public Function CreateNewChk(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String, ByVal login_department_cd As String, ByVal line_cd As String, ByVal yotei_chk_date As String) As String

        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO t_check")
        sb.AppendLine("       (")
        sb.AppendLine("       ck_id")
        sb.AppendLine("      ,cd")
        sb.AppendLine("      ,no")
        sb.AppendLine("      ,department_cd")
        sb.AppendLine("      ,line_cd")
        sb.AppendLine("      ,chk_user")
        sb.AppendLine("      ,yotei_chk_date")
        sb.AppendLine("      ,chk_start_date")
        sb.AppendLine("      ,chk_end_date")
        sb.AppendLine("      ,status")
        sb.AppendLine("      ,result")
        sb.AppendLine("      ,chk_times")
        sb.AppendLine("      ,suu")
        sb.AppendLine("      ,del_flg")
        sb.AppendLine("      ,qianpin")
        sb.AppendLine("      ,tools_scan_flg")
        sb.AppendLine("      ,shared_ck_id")
        sb.AppendLine("      ,shared_no")
        sb.AppendLine("      ,h")
        sb.AppendLine("      ,w")
        sb.AppendLine("      ,dh")
        sb.AppendLine("      ,dw")
        sb.AppendLine("      ,sw")
        sb.AppendLine("      ,kw")
        sb.AppendLine("      ,specialBookNo")
        sb.AppendLine("      ,b2bOderNo")
        sb.AppendLine("      ,b2bIndexNo")
        sb.AppendLine("      ,sapOderNo")
        sb.AppendLine("      ,sapIndexNo")
        sb.AppendLine("      ,edit_user")
        sb.AppendLine("      ,upd_user")
        sb.AppendLine("      ,upd_date")
        sb.AppendLine("      ,ins_user")
        sb.AppendLine("      ,ins_date")
        sb.AppendLine("      ,cd_no_sign")
        sb.AppendLine("       )")

        sb.AppendLine("SELECT '" & ck_id & "' AS ck_id")           '	ck_id
        sb.AppendLine("      ,'" & cd & "' AS cd")          '	cd
        sb.AppendLine("      ,'" & no & "' AS no")          '	no
        'sb.AppendLine("      ,'" & department_cd & "' AS department_cd")           '	department_cd
        sb.AppendLine("      ,department_cd AS department_cd")
        sb.AppendLine("      ,line_cd AS line_cd")         '	line_cd
        sb.AppendLine("      ,'" & user_cd & "' AS chk_user")            '	chk_user
        sb.AppendLine("      ,yotei_chk_date AS yotei_chk_date")          '	yotei_chk_date
        sb.AppendLine("      ,getdate() as  chk_start_date")          '	chk_start_date
        sb.AppendLine("      ,NULL as chk_end_date")            '	chk_end_date
        sb.AppendLine("      ,'0' AS status")          '	status   '  "0":检查中，  1:完了  2：默认结果完了
        sb.AppendLine("      ,'待' AS result")

        sb.AppendLine("      ,1 AS chk_times")           '	chk_times
        sb.AppendLine("      ,suu AS suu")         '	suu
        sb.AppendLine("      ,'0' AS del_flg")         '	del
        sb.AppendLine("      ,'0' AS qianpin")         '	qianpin
        sb.AppendLine("      ,'0' AS tools_scan_flg")
        sb.AppendLine("      ,'' AS shared_ck_id")
        sb.AppendLine("      ,'' AS shared_no")

        sb.AppendLine("      ,h")
        sb.AppendLine("      ,w")
        sb.AppendLine("      ,dh")
        sb.AppendLine("      ,dw")
        sb.AppendLine("      ,sw")
        sb.AppendLine("      ,kw")
        sb.AppendLine("      ,specialBookNo")
        sb.AppendLine("      ,b2bOderNo")
        sb.AppendLine("      ,b2bIndexNo")
        sb.AppendLine("      ,sapOderNo")
        sb.AppendLine("      ,sapIndexNo")
        'sb.AppendLine("      ,chk_fmt")
        'sb.AppendLine("      ,chk_fs")

        sb.AppendLine("      ,''")
        sb.AppendLine("      ,'" & user_cd & "'")            '	upd_user
        sb.AppendLine("      ,getdate() as upd_date")            '	upd_date
        sb.AppendLine("      ,'" & user_cd & "'")             '	ins_user
        sb.AppendLine("      ,getdate() as ins_date")            '	ins_date
        sb.AppendLine("      ,'" & cd.Replace("-", "") & "'")
        sb.AppendLine("From t_check_plan ")
        sb.AppendLine("WHERE  1=1")

        If cd.Trim <> "" Then   '商品cd
            sb.AppendLine("	AND cd = '" & cd & "'")
        End If
        If no.Trim <> "" Then   '作番
            sb.AppendLine("	AND no = '" & no & "'")
        End If

        If no.Trim = "" Then   '作番
            sb.AppendLine("	AND Convert(varchar(100), yotei_chk_date, 23) = '" & yotei_chk_date & "'")
        End If


        Dim filter As String = "", oldFilter As String = ""
        Try
            '登录检查结果明细
            Dim dtTm As DataTable = GetKmDt(cd, no, ck_id, user_cd)
            If dtTm.Rows.Count = 0 Then
                Return "没有找到相关的检查项目"
            End If

            '登录检查结果
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
            sb.Length = 0
            sb.Clear()


            sb.Length = 0
            sb.AppendLine("SELECT")
            sb.AppendLine("       h")
            sb.AppendLine("      ,w")
            sb.AppendLine("      ,dh")
            sb.AppendLine("      ,dw")
            sb.AppendLine("      ,sw")
            sb.AppendLine("      ,kw")
            sb.AppendLine("From t_check ")
            sb.AppendLine("WHERE  ck_id='" & ck_id & "'")
            Dim dtChkInfo As Data.DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "dtChkInfo")



            Dim dtReal As DataTable = dtTm.Clone
            dtReal.Rows.Clear()
            Dim ScriptControl As New MSScriptControl.ScriptControl
            Dim id As Integer = 1
            For i As Integer = 0 To dtTm.Rows.Count - 1

                oldFilter = dtTm.Rows(i).Item("filter").ToString
                filter = dtTm.Rows(i).Item("filter").ToString.Replace("{h}", dtChkInfo.Rows(0).Item("h").ToString)
                filter = filter.Replace("{w}", dtChkInfo.Rows(0).Item("w").ToString)
                filter = filter.Replace("{dh}", dtChkInfo.Rows(0).Item("dh").ToString)
                filter = filter.Replace("{dw}", dtChkInfo.Rows(0).Item("dw").ToString)
                filter = filter.Replace("{sw}", dtChkInfo.Rows(0).Item("sw").ToString)
                filter = filter.Replace("{kw}", dtChkInfo.Rows(0).Item("kw").ToString)

                '不符合规则的 不添加
                If filter <> "" Then
                    If ScriptControl Is Nothing Then
                        ScriptControl = New MSScriptControl.ScriptControl
                    End If
                    'Dim ScriptControl As New MSScriptControl.ScriptControl
                    ScriptControl.Language = "JavaScript" '设置语言种类
                    ScriptControl.AddCode("function TestFunc(){return " & filter & ";}") '添加脚本代码

                    Try
                        If (ScriptControl.Run("TestFunc")) = False Then
                            Continue For
                        End If
                    Catch ex As Exception
                        Return "Filter公式出错 模板行ID:" & dtTm.Rows(i).Item("km_id") & " 错误公式：（" & filter & "）-----" & ex.Message
                    End Try
                End If

                Dim dr As DataRow = dtReal.NewRow
                For k As Integer = 0 To dtTm.Columns.Count - 1
                    If k = 0 Then
                        dr.Item(k) = id
                    Else
                        dr.Item(k) = dtTm.Rows(i).Item(k)
                    End If
                Next
                dtReal.Rows.Add(dr)
                id = id + 1

            Next
            ScriptControl = Nothing
            sb = Nothing

            dtReal.Columns.Remove("filter")

            Dim bCopy As New SqlClient.SqlBulkCopy(DataAccessManager.ConnStr)
            bCopy.BulkCopyTimeout = 30
            bCopy.DestinationTableName = "t_check_ms"
            bCopy.WriteToServer(dtReal)
            dtReal.Clear()
            dtReal = Nothing
            Return ""

        Catch ex As Exception
            Dim sb2 As New StringBuilder
            sb2.AppendLine("delete from t_check WHERE ck_id='" & ck_id & "'")
            sb2.AppendLine("delete from t_check_ms WHERE ck_id='" & ck_id & "'")
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb2.ToString())
            Return oldFilter & "   " & ex.Message
        End Try


    End Function

    'NG再检查
    Public Function CreateReChk(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal pre_ck_id As String, ByVal user_cd As String, ByVal login_department_cd As String, ByVal line_cd As String, ByVal yotei_chk_date As String) As String

        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO t_pre_chk_id(ck_id,pre_ck_id)")
        sb.AppendLine("SELECT '" & ck_id & "' AS ck_id")
        sb.AppendLine(", '" & pre_ck_id & "' AS pre_ck_id")

        sb.AppendLine("INSERT INTO t_check")
        sb.AppendLine("       (")
        sb.AppendLine("       ck_id")
        sb.AppendLine("      ,cd")
        sb.AppendLine("      ,no")
        sb.AppendLine("      ,department_cd")
        sb.AppendLine("      ,line_cd")
        sb.AppendLine("      ,chk_user")
        sb.AppendLine("      ,yotei_chk_date")
        sb.AppendLine("      ,chk_start_date")
        sb.AppendLine("      ,chk_end_date")
        sb.AppendLine("      ,status")
        sb.AppendLine("      ,result")
        sb.AppendLine("      ,chk_times")
        sb.AppendLine("      ,suu")
        sb.AppendLine("      ,del_flg")
        sb.AppendLine("      ,qianpin")
        sb.AppendLine("      ,tools_scan_flg")
        sb.AppendLine("      ,shared_ck_id")
        sb.AppendLine("      ,shared_no")
        sb.AppendLine("      ,h")
        sb.AppendLine("      ,w")
        sb.AppendLine("      ,dh")
        sb.AppendLine("      ,dw")
        sb.AppendLine("      ,sw")
        sb.AppendLine("      ,kw")
        sb.AppendLine("      ,specialBookNo")
        sb.AppendLine("      ,b2bOderNo")
        sb.AppendLine("      ,b2bIndexNo")
        sb.AppendLine("      ,sapOderNo")
        sb.AppendLine("      ,sapIndexNo")
        sb.AppendLine("      ,edit_user")
        sb.AppendLine("      ,upd_user")
        sb.AppendLine("      ,upd_date")
        sb.AppendLine("      ,ins_user")
        sb.AppendLine("      ,ins_date")
        sb.AppendLine("      ,cd_no_sign")
        sb.AppendLine("       )")

        sb.AppendLine("SELECT '" & ck_id & "' AS ck_id")           '	ck_id
        sb.AppendLine("      ,'" & cd & "' AS cd")          '	cd
        sb.AppendLine("      ,'" & no & "' AS no")          '	no
        'sb.AppendLine("      ,'" & department_cd & "' AS department_cd")           '	department_cd
        sb.AppendLine("      ,department_cd AS department_cd")
        sb.AppendLine("      ,line_cd AS line_cd")         '	line_cd
        sb.AppendLine("      ,'" & user_cd & "' AS chk_user")            '	chk_user
        sb.AppendLine("      ,yotei_chk_date AS yotei_chk_date")          '	yotei_chk_date
        sb.AppendLine("      ,getdate() as  chk_start_date")          '	chk_start_date
        sb.AppendLine("      ,NULL as chk_end_date")            '	chk_end_date
        sb.AppendLine("      ,'0' AS status")          '	status   '  "0":检查中，  1:完了  2：默认结果完了
        sb.AppendLine("      ,'待' AS result")

        sb.AppendLine("      ,1 AS chk_times")           '	chk_times
        sb.AppendLine("      ,suu AS suu")         '	suu
        sb.AppendLine("      ,'0' AS del_flg")         '	del
        sb.AppendLine("      ,'0' AS qianpin")         '	qianpin
        sb.AppendLine("      ,'0' AS tools_scan_flg")
        sb.AppendLine("      ,'' AS shared_ck_id")
        sb.AppendLine("      ,'' AS shared_no")

        sb.AppendLine("      ,h")
        sb.AppendLine("      ,w")
        sb.AppendLine("      ,dh")
        sb.AppendLine("      ,dw")
        sb.AppendLine("      ,sw")
        sb.AppendLine("      ,kw")
        sb.AppendLine("      ,specialBookNo")
        sb.AppendLine("      ,b2bOderNo")
        sb.AppendLine("      ,b2bIndexNo")
        sb.AppendLine("      ,sapOderNo")
        sb.AppendLine("      ,sapIndexNo")
        'sb.AppendLine("      ,chk_fmt")
        'sb.AppendLine("      ,chk_fs")

        sb.AppendLine("      ,''")
        sb.AppendLine("      ,'" & user_cd & "'")            '	upd_user
        sb.AppendLine("      ,getdate() as upd_date")            '	upd_date
        sb.AppendLine("      ,'" & user_cd & "'")             '	ins_user
        sb.AppendLine("      ,getdate() as ins_date")            '	ins_date
        sb.AppendLine("From t_check_plan ")
        sb.AppendLine("WHERE  1=1")

        If cd.Trim <> "" Then   '商品cd
            sb.AppendLine("	AND cd = '" & cd & "'")
        End If
        If no.Trim <> "" Then   '作番
            sb.AppendLine("	AND no = '" & no & "'")
        End If

        If no.Trim = "" Then
            sb.AppendLine("	AND Convert(varchar(100), yotei_chk_date, 23) = '" & yotei_chk_date & "'")
        End If





        Dim filter As String = "", oldFilter As String = ""
        Try
            '登录检查结果明细
            Dim dtTm As DataTable = GetKmDt(cd, no, ck_id, user_cd)
            If dtTm.Rows.Count = 0 Then
                Return "没有找到相关的检查项目"
            End If

            '登录检查结果
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
            sb.Length = 0
            sb.Clear()


            sb.Length = 0
            sb.AppendLine("SELECT")
            sb.AppendLine("       h")
            sb.AppendLine("      ,w")
            sb.AppendLine("      ,dh")
            sb.AppendLine("      ,dw")
            sb.AppendLine("      ,sw")
            sb.AppendLine("      ,kw")
            sb.AppendLine("From t_check ")
            sb.AppendLine("WHERE  ck_id='" & ck_id & "'")
            Dim dtChkInfo As Data.DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "dtChkInfo")



            Dim dtReal As DataTable = dtTm.Clone
            dtReal.Rows.Clear()
            Dim ScriptControl As New MSScriptControl.ScriptControl
            Dim id As Integer = 1
            For i As Integer = 0 To dtTm.Rows.Count - 1

                oldFilter = dtTm.Rows(i).Item("filter").ToString
                filter = dtTm.Rows(i).Item("filter").ToString.Replace("{h}", dtChkInfo.Rows(0).Item("h").ToString)
                filter = filter.Replace("{w}", dtChkInfo.Rows(0).Item("w").ToString)
                filter = filter.Replace("{dh}", dtChkInfo.Rows(0).Item("dh").ToString)
                filter = filter.Replace("{dw}", dtChkInfo.Rows(0).Item("dw").ToString)
                filter = filter.Replace("{sw}", dtChkInfo.Rows(0).Item("sw").ToString)
                filter = filter.Replace("{kw}", dtChkInfo.Rows(0).Item("kw").ToString)

                '不符合规则的 不添加
                If filter <> "" Then
                    If ScriptControl Is Nothing Then
                        ScriptControl = New MSScriptControl.ScriptControl
                    End If
                    'Dim ScriptControl As New MSScriptControl.ScriptControl
                    ScriptControl.Language = "JavaScript" '设置语言种类
                    ScriptControl.AddCode("function TestFunc(){return " & filter & ";}") '添加脚本代码

                    Try
                        If (ScriptControl.Run("TestFunc")) = False Then
                            Continue For
                        End If
                    Catch ex As Exception
                        Return "Filter公式出错 模板行ID:" & dtTm.Rows(i).Item("km_id") & " 错误公式：（" & filter & "）-----" & ex.Message
                    End Try
                End If

                Dim dr As DataRow = dtReal.NewRow
                For k As Integer = 0 To dtTm.Columns.Count - 1
                    If k = 0 Then
                        dr.Item(k) = id
                    Else
                        dr.Item(k) = dtTm.Rows(i).Item(k)
                    End If
                Next
                dtReal.Rows.Add(dr)
                id = id + 1

            Next
            ScriptControl = Nothing
            sb = Nothing

            dtReal.Columns.Remove("filter")

            Dim bCopy As New SqlClient.SqlBulkCopy(DataAccessManager.ConnStr)
            bCopy.BulkCopyTimeout = 30
            bCopy.DestinationTableName = "t_check_ms"
            bCopy.WriteToServer(dtReal)
            dtReal.Clear()
            dtReal = Nothing
            Return ""

        Catch ex As Exception
            Dim sb2 As New StringBuilder
            sb2.AppendLine("delete from t_check WHERE ck_id='" & ck_id & "'")
            sb2.AppendLine("delete from t_check_ms WHERE ck_id='" & ck_id & "'")
            sb2.AppendLine("delete from t_pre_chk_id WHERE ck_id='" & ck_id & "'")


            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb2.ToString())
            Return oldFilter & "   " & ex.Message
        End Try


    End Function




    '登录新的检查项目
    Public Function CreateNewChkNoPlan(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String, ByVal login_department_cd As String, ByVal line_cd As String, ByVal yotei_chk_date As String) As String

        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO t_check")
        sb.AppendLine("       (")
        sb.AppendLine("       ck_id")
        sb.AppendLine("      ,cd")
        sb.AppendLine("      ,no")
        sb.AppendLine("      ,department_cd")
        sb.AppendLine("      ,line_cd")
        sb.AppendLine("      ,chk_user")
        sb.AppendLine("      ,yotei_chk_date")
        sb.AppendLine("      ,chk_start_date")
        sb.AppendLine("      ,chk_end_date")
        sb.AppendLine("      ,status")
        sb.AppendLine("      ,result")
        sb.AppendLine("      ,chk_times")
        sb.AppendLine("      ,suu")
        sb.AppendLine("      ,del_flg")
        sb.AppendLine("      ,qianpin")
        sb.AppendLine("      ,tools_scan_flg")
        sb.AppendLine("      ,shared_ck_id")
        sb.AppendLine("      ,shared_no")
        sb.AppendLine("      ,h")
        sb.AppendLine("      ,w")
        sb.AppendLine("      ,dh")
        sb.AppendLine("      ,dw")
        sb.AppendLine("      ,sw")
        sb.AppendLine("      ,kw")
        sb.AppendLine("      ,specialBookNo")
        sb.AppendLine("      ,b2bOderNo")
        sb.AppendLine("      ,b2bIndexNo")
        sb.AppendLine("      ,sapOderNo")
        sb.AppendLine("      ,sapIndexNo")
        sb.AppendLine("      ,edit_user")
        sb.AppendLine("      ,upd_user")
        sb.AppendLine("      ,upd_date")
        sb.AppendLine("      ,ins_user")
        sb.AppendLine("      ,ins_date")
        sb.AppendLine("      ,cd_no_sign")
        sb.AppendLine("       )")

        sb.AppendLine("SELECT '" & ck_id & "' AS ck_id")           '	ck_id
        sb.AppendLine("      ,'" & cd & "' AS cd")          '	cd
        sb.AppendLine("      ,'" & no & "' AS no")          '	no
        'sb.AppendLine("      ,'" & department_cd & "' AS department_cd")           '	department_cd
        sb.AppendLine("      ,'' AS department_cd")
        sb.AppendLine("      ,'' AS line_cd")         '	line_cd
        sb.AppendLine("      ,'" & user_cd & "' AS chk_user")            '	chk_user
        sb.AppendLine("      ,getdate() AS yotei_chk_date")          '	yotei_chk_date
        sb.AppendLine("      ,getdate() as  chk_start_date")          '	chk_start_date
        sb.AppendLine("      ,NULL as chk_end_date")            '	chk_end_date
        sb.AppendLine("      ,'0' AS status")          '	status   '  "0":检查中，  1:完了  2：默认结果完了
        sb.AppendLine("      ,'待' AS result")

        sb.AppendLine("      ,1 AS chk_times")           '	chk_times
        sb.AppendLine("      ,0 AS suu")         '	suu
        sb.AppendLine("      ,'0' AS del_flg")         '	del
        sb.AppendLine("      ,'0' AS qianpin")         '	qianpin
        sb.AppendLine("      ,'0' AS tools_scan_flg")
        sb.AppendLine("      ,'' AS shared_ck_id")
        sb.AppendLine("      ,'' AS shared_no")

        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        sb.AppendLine("      ,''")
        'sb.AppendLine("      ,chk_fmt")
        'sb.AppendLine("      ,chk_fs")

        sb.AppendLine("      ,''")
        sb.AppendLine("      ,'" & user_cd & "'")            '	upd_user
        sb.AppendLine("      ,getdate() as upd_date")            '	upd_date
        sb.AppendLine("      ,'" & user_cd & "'")             '	ins_user
        sb.AppendLine("      ,getdate() as ins_date")            '	ins_date
        sb.AppendLine("      ,'" & cd.Replace("-", "") & "'")
        'sb.AppendLine("From t_check_plan ")
        'sb.AppendLine("WHERE  1=1")

        'If cd.Trim <> "" Then   '商品cd
        '    sb.AppendLine("	AND cd = '" & cd & "'")
        'End If
        'If no.Trim <> "" Then   '作番
        '    sb.AppendLine("	AND no = '" & no & "'")
        'End If
        'sb.AppendLine("	AND Convert(varchar(100), yotei_chk_date, 23) = '" & yotei_chk_date & "'")

        Dim filter As String = "", oldFilter As String = ""
        Try
            '登录检查结果明细
            Dim dtTm As DataTable = GetKmDt(cd, no, ck_id, user_cd)
            If dtTm.Rows.Count = 0 Then
                Return "没有找到相关的检查项目"
            End If

            '登录检查结果
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
            sb.Length = 0
            sb.Clear()


            'sb.Length = 0
            'sb.AppendLine("SELECT")
            'sb.AppendLine("       h")
            'sb.AppendLine("      ,w")
            'sb.AppendLine("      ,dh")
            'sb.AppendLine("      ,dw")
            'sb.AppendLine("      ,sw")
            'sb.AppendLine("      ,kw")
            'sb.AppendLine("From t_check ")
            'sb.AppendLine("WHERE  ck_id='" & ck_id & "'")
            'Dim dtChkInfo As Data.DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "dtChkInfo")



            Dim dtReal As DataTable = dtTm
            'dtReal.Rows.Clear()
            'Dim ScriptControl As New MSScriptControl.ScriptControl
            'Dim id As Integer = 1
            For i As Integer = 0 To dtTm.Rows.Count - 1

                If (dtTm.Rows(i).Item("filter").ToString.Contains("{")) Then

                    If dtTm.Rows(i).Item("filter").Contains("{") Then
                        dtTm.Rows(i).Item("chk_km_name") = dtTm.Rows(i).Item("chk_km_name") & "###" & dtTm.Rows(i).Item("filter").ToString.Trim()
                    End If

                    'chk_km_name
                    If dtTm.Rows(i).Item("k1").Contains("{") Then
                        dtTm.Rows(i).Item("chk_km_name") = dtTm.Rows(i).Item("chk_km_name") & "***" & dtTm.Rows(i).Item("k1").ToString.Trim()
                    End If

                    'filter
                End If


                '    oldFilter = dtTm.Rows(i).Item("filter").ToString
                '    filter = dtTm.Rows(i).Item("filter").ToString.Replace("{h}", dtChkInfo.Rows(0).Item("h").ToString)
                '    filter = filter.Replace("{w}", dtChkInfo.Rows(0).Item("w").ToString)
                '    filter = filter.Replace("{dh}", dtChkInfo.Rows(0).Item("dh").ToString)
                '    filter = filter.Replace("{dw}", dtChkInfo.Rows(0).Item("dw").ToString)
                '    filter = filter.Replace("{sw}", dtChkInfo.Rows(0).Item("sw").ToString)
                '    filter = filter.Replace("{kw}", dtChkInfo.Rows(0).Item("kw").ToString)

                '    '不符合规则的 不添加
                '    If filter <> "" Then
                '        If ScriptControl Is Nothing Then
                '            ScriptControl = New MSScriptControl.ScriptControl
                '        End If
                '        'Dim ScriptControl As New MSScriptControl.ScriptControl
                '        ScriptControl.Language = "JavaScript" '设置语言种类
                '        ScriptControl.AddCode("function TestFunc(){return " & filter & ";}") '添加脚本代码

                '        Try
                '            If (ScriptControl.Run("TestFunc")) = False Then
                '                Continue For
                '            End If
                '        Catch ex As Exception
                '            Return "Filter公式出错 模板行ID:" & dtTm.Rows(i).Item("km_id") & " 错误公式：（" & filter & "）-----" & ex.Message
                '        End Try
                '    End If

                '    Dim dr As DataRow = dtReal.NewRow
                '    For k As Integer = 0 To dtTm.Columns.Count - 1
                '        If k = 0 Then
                '            dr.Item(k) = id
                '        Else
                '            dr.Item(k) = dtTm.Rows(i).Item(k)
                '        End If
                '    Next
                '    dtReal.Rows.Add(dr)
                '    id = id + 1

            Next
            'ScriptControl = Nothing
            sb = Nothing

            dtReal.Columns.Remove("filter")

            Dim bCopy As New SqlClient.SqlBulkCopy(DataAccessManager.ConnStr)
            bCopy.BulkCopyTimeout = 30
            bCopy.DestinationTableName = "t_check_ms"
            bCopy.WriteToServer(dtReal)
            dtReal.Clear()
            dtReal = Nothing
            Return ""

        Catch ex As Exception
            Dim sb2 As New StringBuilder
            sb2.AppendLine("delete from t_check WHERE ck_id='" & ck_id & "'")
            sb2.AppendLine("delete from t_check_ms WHERE ck_id='" & ck_id & "'")
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb2.ToString())
            Return oldFilter & "   " & ex.Message
        End Try


    End Function

    '登录新的检查项目
    Function CreateNewChkHand(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String, ByVal department_cd As String, ByVal line_cd As String, ByVal suu As String, ByVal result As String) As String

        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO t_check")

        sb.AppendLine("       (")
        sb.AppendLine("       ck_id")
        sb.AppendLine("      ,cd")
        sb.AppendLine("      ,no")
        sb.AppendLine("      ,department_cd")
        sb.AppendLine("      ,line_cd")
        sb.AppendLine("      ,chk_user")
        sb.AppendLine("      ,yotei_chk_date")
        sb.AppendLine("      ,chk_start_date")
        sb.AppendLine("      ,chk_end_date")
        sb.AppendLine("      ,status")
        sb.AppendLine("      ,result")
        sb.AppendLine("      ,chk_times")
        sb.AppendLine("      ,suu")
        sb.AppendLine("      ,del_flg")
        sb.AppendLine("      ,qianpin")
        sb.AppendLine("      ,tools_scan_flg")
        sb.AppendLine("      ,shared_ck_id")
        sb.AppendLine("      ,shared_no")
        sb.AppendLine("      ,h")
        sb.AppendLine("      ,w")
        sb.AppendLine("      ,dh")
        sb.AppendLine("      ,dw")
        sb.AppendLine("      ,sw")
        sb.AppendLine("      ,kw")
        sb.AppendLine("      ,specialBookNo")
        sb.AppendLine("      ,b2bOderNo")
        sb.AppendLine("      ,b2bIndexNo")
        sb.AppendLine("      ,sapOderNo")
        sb.AppendLine("      ,sapIndexNo")
        sb.AppendLine("      ,edit_user")
        sb.AppendLine("      ,upd_user")
        sb.AppendLine("      ,upd_date")
        sb.AppendLine("      ,ins_user")
        sb.AppendLine("      ,ins_date")
        sb.AppendLine("      ,cd_no_sign")
        sb.AppendLine("       )")



        sb.AppendLine("SELECT '" & ck_id & "' AS ck_id")           '	ck_id
        sb.AppendLine("      ,'" & cd & "' AS cd")          '	cd
        sb.AppendLine("      ,'" & no & "' AS no")          '	no
        sb.AppendLine("      ,'" & department_cd & "' AS department_cd")           '	department_cd
        sb.AppendLine("      ,'" & line_cd & "' AS line_cd")         '	line_cd
        sb.AppendLine("      ,'" & user_cd & "' AS chk_user")            '	chk_user
        sb.AppendLine("      ,getdate() AS yotei_chk_date")          '	yotei_chk_date
        sb.AppendLine("      ,getdate() as  chk_start_date")          '	chk_start_date
        sb.AppendLine("      ,getdate() as chk_end_date")            '	chk_end_date
        sb.AppendLine("      ,'4' AS status")          '	status   '  "0":检查中，  1:完了  2：默认结果完了 4:手入力
        sb.AppendLine("      ,'" & result & "' AS result")
        sb.AppendLine("      ,1 AS chk_times")           '	chk_times
        sb.AppendLine("      ,'" & suu & "' AS suu")         '	suu
        sb.AppendLine("      ,'0' AS del_flg")         '	del
        sb.AppendLine("      ,'0' AS qianpin")         '	qianpin
        sb.AppendLine("      ,'0' AS tools_scan_flg")
        sb.AppendLine("      ,'' AS shared_ck_id")
        sb.AppendLine("      ,'' AS shared_no")
        sb.AppendLine("      ,'' AS h")
        sb.AppendLine("      ,'' AS w")
        sb.AppendLine("      ,'' AS dh")
        sb.AppendLine("      ,'' AS dw")
        sb.AppendLine("      ,'' AS sw")
        sb.AppendLine("      ,'' AS kw")
        sb.AppendLine("      ,'' AS specialBookNo")
        sb.AppendLine("      ,'' AS b2bOderNo")
        sb.AppendLine("      ,'' AS b2bIndexNo")
        sb.AppendLine("      ,'' AS sapOderNo")
        sb.AppendLine("      ,'' AS sapIndexNo")
        sb.AppendLine("      ,'" & user_cd & "'")
        sb.AppendLine("      ,'" & user_cd & "'")            '	upd_user
        sb.AppendLine("      ,getdate() as upd_date")            '	upd_date
        sb.AppendLine("      ,'" & user_cd & "'")             '	ins_user
        sb.AppendLine("      ,getdate() as ins_date")            '	ins_date
        sb.AppendLine("      ,'" & cd.Replace("-", "") & "'")
        Try
            '登录检查结果
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
            sb.Length = 0
            sb.Clear()
            sb = Nothing
            Return ""
        Catch ex As Exception
            Dim sb2 As New StringBuilder
            sb.AppendLine("delete from t_check WHERE ck_id='" & ck_id & "'")
            sb.AppendLine("delete from t_check_ms WHERE ck_id='" & ck_id & "'")
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
            Return ex.Message
        End Try
    End Function



    Function GetLinksMs(ByVal cLoginInfo As CLoginInfo, ByVal ck_id As String, ByVal kind_name As String, Optional ByVal toolStyle As Boolean = False, Optional ByVal tool_txt As String = "") As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT ms.[id]")
        sb.AppendLine("      , ms.[kind_name]")
        sb.AppendLine("      , ms.[kind_jun]")
        sb.AppendLine("      , ms.[result]")
        sb.AppendLine("  FROM [t_check_ms] ms")
        sb.AppendLine("  INNER JOIN t_check t")
        sb.AppendLine("  ON t.ck_id = ms.ck_id")

        sb.AppendLine("  LEFT JOIN t_check_plan c")
        sb.AppendLine("  ON t.cd = c.cd")
        sb.AppendLine("  AND t.no = c.no")
        sb.AppendLine("  AND Convert(Datetime, t.yotei_chk_date, 120) = c.yotei_chk_date")

        sb.AppendLine("  LEFT JOIN m_tools_new d")
        sb.AppendLine("  ON ms.tools_ma = d.barcode")
        sb.AppendLine("  AND t.department_cd = d.department_cd")

        sb.AppendLine("WHERE  t.ck_id = '" & ck_id & "'")

        If kind_name.Trim <> "" Then   '项目种类
            sb.AppendLine("	AND ms.kind_name = '" & kind_name & "'")
        End If

        If toolStyle Then
            sb.AppendLine("	AND isnull(ms.tools_ma,'')<>''")
        End If

        If tool_txt.Trim <> "" Then
            sb.AppendLine("	AND isnull(d.remarks,ms.[tools_ma])='" & tool_txt.Trim & "'")
        End If


        sb.AppendLine("ORDER BY ms.[kind_jun],ms.hyouji_jun")

        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkMs")
        dt.Columns.Add("row_no")
        For i As Integer = 0 To dt.Rows.Count - 1
            dt.Rows(i).Item("row_no") = （i + 1).ToString
        Next

        Return dt


    End Function


    '商品分类列表
    Public Function GetFirstCheck(ByVal cd As String) As String

        Dim sb As New StringBuilder
        sb.AppendLine("SELECT tongyong_cd FROM t_first_check where tongyong_cd in (")
        sb.AppendLine("SELECT tongyong_cd FROM t_first_check where replace(good_cd,'-','') = '" & cd.Replace("-", "") & "')")
        sb.AppendLine("and checked_flg='1'")
        'Dim ds As New DataSet
        '検索の実行
        'FillDataset(DataAccessManager.Connection, CommandType.Text, sb.ToString(), ds, "GetFirstCheck", paramList.ToArray)

        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetFirstCheck")

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item(0).ToString
        Else
            Return ""
        End If

    End Function


    Public Function GetFirstCheck_step2(ByVal cd As String, ByVal line_cd As String) As String

        Dim sb As New StringBuilder
        'sb.AppendLine("SELECT tongyong_cd FROM m_first_chk_cds where tongyong_cd in (")
        ''sb.AppendLine("SELECT tongyong_cd FROM m_first_chk_cds where replace(cd,'-','') = '" & cd.Replace("-", "") & "' and line_cd='" & line_cd & "')")
        'sb.AppendLine("SELECT tongyong_cd FROM m_first_chk_cds where replace(cd,'-','') = '" & cd.Replace("-", "") & "')")
        'sb.AppendLine("and checked_flg='1'")

        'sb.AppendLine("SELECT 1 FROM m_first_chk_step2 where replace(CD,'-','') = '" & cd.Replace("-", "") & "'  and line_cd='" & line_cd & "'")



        sb.AppendLine("SELECT 1 FROM m_first_chk_step2 where replace(CD,'-','') in (")
        sb.AppendLine("    SELECT replace(CD,'-','') FROM m_first_chk_cds WHERE tongyong_cd in(")
        sb.AppendLine("        SELECT tongyong_cd FROM m_first_chk_cds where replace(cd,'-','') = '" & cd.Replace("-", "") & "'")
        sb.AppendLine("    )")
        sb.AppendLine(")")
        sb.AppendLine("and line_cd='" & line_cd & "'")

        'Dim ds As New DataSet
        '検索の実行
        'FillDataset(DataAccessManager.Connection, CommandType.Text, sb.ToString(), ds, "GetFirstCheck", paramList.ToArray)

        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetFirstCheck_step2")

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item(0).ToString
        Else
            Return ""
        End If

    End Function

    Public Function Gettongyong_cd_step2(ByVal cd As String) As String

        Dim sb As New StringBuilder
        sb.AppendLine(" SELECT tongyong_cd FROM m_first_chk_cds where replace(cd,'-','') = '" & cd.Replace("-", "") & "'")

        '検索の実行
        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "Gettongyong_cd")

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item(0).ToString
        Else
            Return ""
        End If

    End Function

    Public Function Gettongyong_cd(ByVal cd As String) As String

        Dim sb As New StringBuilder
        'sb.AppendLine(" SELECT tongyong_cd FROM m_first_chk_cds where replace(cd,'-','') = '" & cd.Replace("-", "") & "'")
        sb.AppendLine(" SELECT tongyong_cd FROM t_first_check where replace(good_cd,'-','') = '" & cd.Replace("-", "") & "'")
        '検索の実行
        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "Gettongyong_cd")

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item(0).ToString
        Else
            Return ""
        End If

    End Function

    Public Function UpdFirstCheck(ByVal tongyong_cd As String) As Integer
        Dim sb As New StringBuilder
        sb.AppendLine(" UPDATE ")
        sb.AppendLine(" t_first_check ")
        sb.AppendLine(" SET ")
        sb.AppendLine(" checked_flg='1' ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine(" tongyong_cd = '" & tongyong_cd & "' ")
        '更新の実行
        Return ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
    End Function


    Public Function UpdFirstCheck2(ByVal tongyong_cd As String) As Integer
        Dim sb As New StringBuilder
        sb.AppendLine(" UPDATE ")
        sb.AppendLine(" m_first_chk_cds ")
        sb.AppendLine(" SET ")
        sb.AppendLine(" checked_flg='1' ")
        sb.AppendLine(" WHERE ")
        sb.AppendLine(" tongyong_cd = '" & tongyong_cd & "' ")
        ' sb.AppendLine(" and line_cd = '" & line_cd & "' ")
        '更新の実行
        Return ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
    End Function

End Class
