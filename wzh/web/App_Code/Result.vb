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
Public Class Result
    Inherits System.Web.Services.WebService

    '更新检查明细MS结果
    <WebMethod()>
    Public Function UpdCheckResult(ByVal ck_id As String, ByVal qianpin As String, ByVal upd_user As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Dim Sql As New StringBuilder
        Sql.AppendLine("Update t_check")
        Sql.AppendLine("Set ")
        Sql.AppendLine("    qianpin='" & qianpin & "' ")
        Sql.AppendLine("    ,upd_user='" & upd_user & "' ")
        Sql.AppendLine("    ,upd_date=getdate() ")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id='" & ck_id & "' ")
        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, Sql.ToString())
        Return jss.Serialize("OK")
    End Function

    '更新检查结果 结果
    Public Shared Function UpdCheckResultStatus(ByVal ck_id As String, ByVal cd As String, ByVal upd_user As String, ByVal status As String, Optional ByVal updResult As Boolean = False) As String

        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Dim Sql As New StringBuilder

        Sql.AppendLine("DECLARE @ck_id NVARCHAR(25)")
        Sql.AppendLine("DECLARE @result NVARCHAR(20)")
        Sql.AppendLine("DECLARE @upd_user NVARCHAR(20)")
        Sql.AppendLine("DECLARE @status NVARCHAR(20)")

        Sql.AppendLine("SET @status = '" & status & "'")
        Sql.AppendLine("SET @ck_id = '" & ck_id & "'")
        Sql.AppendLine("SET @upd_user = '" & upd_user & "'")

        If updResult Then
            Sql.AppendLine("IF (SELECT Count(id)")
            Sql.AppendLine("    FROM   [t_check_ms]")
            Sql.AppendLine("    WHERE  ck_id = @ck_id")
            Sql.AppendLine("           AND Isnull(result, '') NOT IN ( 'OK', 'SD', 'JN' )) > 0")
            Sql.AppendLine("  BEGIN")
            Sql.AppendLine("      SET @result = 'NG'")
            Sql.AppendLine("  END")
            Sql.AppendLine("  ELSE")
            Sql.AppendLine("  BEGIN")
            Sql.AppendLine("      SET @result = 'OK'")
            Sql.AppendLine("  END")
        End If


        Sql.AppendLine("Update t_check")
        Sql.AppendLine("Set ")
        Sql.AppendLine("     status=@status ")   'status 0:检查中，  1:完了  2：默认结果完了 

        If updResult Then
            Sql.AppendLine("    ,result=@result ")
        End If

        Sql.AppendLine("    ,upd_user= @upd_user ")
        Sql.AppendLine("    ,chk_end_date=getdate() ")
        Sql.AppendLine("    ,upd_date=getdate() ")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id=@ck_id ")
        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, Sql.ToString())




        Dim sb As New StringBuilder
        sb.AppendLine("  select * from [t_check_ms]")
        sb.AppendLine("  where ck_id = '" & ck_id & "'")
        sb.AppendLine("  And isnull(result,'') = ''")
        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "t_check_ms")

        Dim ResultDA As New t_checkDA


        If dt.Rows.Count = 0 Then

            Dim tongyong_cd As String = ResultDA.Gettongyong_cd(cd)
            If tongyong_cd <> "" Then
                ResultDA.UpdFirstCheck(tongyong_cd)
                'Else
                '    ResultDA.InsFirstCheck(Me.tbxGoodsCd.Text.Trim, Me.tbxGoodsCd.Text.Trim)
            End If

            Dim dtChk As DataTable = ResultDA.GetCheckLineCd(ck_id)

            If dtChk.Rows.Count > 0 Then

                Dim line_cd As String = dtChk.Rows(0).Item(0).ToString
                Dim result As String = dtChk.Rows(0).Item(1).ToString

                If result = "OK" Then
                    tongyong_cd = ResultDA.Gettongyong_cd_step2(cd)

                    If tongyong_cd <> "" Then
                        ResultDA.UpdFirstCheck2(tongyong_cd)

                        Dim step2DA As m_first_chk_step2DA = New m_first_chk_step2DA
                        step2DA.Ins_m_first_chk_step2(cd, line_cd, upd_user)
                        'Else
                        '    ResultDA.InsFirstCheck(Me.tbxGoodsCd.Text.Trim, Me.tbxGoodsCd.Text.Trim)
                    End If
                End If

            End If


        End If


        'Dim dtChk As DataTable = ResultDA.GetCheckLineCd(ck_id)

        'Dim step2DA As m_first_chk_step2DA = New m_first_chk_step2DA
        'If (dtChk.Rows.Count > 0) Then

        '    Dim line_cd As String = dtChk.Rows(0).Item(0).ToString
        '    Dim result As String = dtChk.Rows(0).Item(1).ToString

        '    If result = "OK" Then
        '        Dim first2Dt = step2DA.Getm_first_chk_step2(cd, dtChk.Rows(0).Item(0).ToString)
        '        Dim first2Cds = step2DA.Getm_m_first_chk_cds(cd, dtChk.Rows(0).Item(0).ToString)

        '        If (first2Dt.Rows.Count = 0 AndAlso
        '            first2Cds.Rows.Count > 0) Then
        '            step2DA.Ins_m_first_chk_step2(cd, line_cd, upd_user)
        '        End If
        '    End If


        'End If




        Return jss.Serialize("OK")
    End Function

    '删除检查明细
    Public Shared Function DelCheckMsAndResult(ByVal ck_id As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Dim Sql As New StringBuilder
        Sql.AppendLine("DELETE FROM t_check_ms")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id='" & ck_id & "' ")

        Sql.AppendLine("DELETE FROM t_check")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id='" & ck_id & "' ")

        Sql.AppendLine("DELETE FROM t_pre_chk_id")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id='" & ck_id & "' ")

        Sql.AppendLine("DELETE FROM t_pre_chk_id")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    pre_ck_id='" & ck_id & "' ")

        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, Sql.ToString())
        Return jss.Serialize("OK")
    End Function


    '更新检查明细MS结果
    <WebMethod()>
    Public Function UpdCheckMsResult(ByVal ck_id As String, ByVal id As String, ByVal in_1 As String, ByVal result As String, ByVal mark As String, ByVal upd_user As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Dim Sql As New StringBuilder
        Sql.AppendLine("Update t_check_ms")
        Sql.AppendLine("Set ")
        Sql.AppendLine("    in_1='" & in_1 & "' ")
        Sql.AppendLine("    ,result='" & result & "' ")
        Sql.AppendLine("    ,mark=N'" & mark.Replace("'", "’") & "' ")
        Sql.AppendLine("    ,upd_user='" & upd_user & "' ")
        Sql.AppendLine("    ,upd_date=getdate() ")

        'If mark.Trim <> "" Then
        '    Sql.AppendLine("    ,upd_user='" & upd_user & "' ")
        'End If
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id='" & ck_id & "' ")
        Sql.AppendLine("    And id='" & id & "' ")
        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, Sql.ToString())
        Return jss.Serialize("OK")
    End Function

    '更新注释
    <WebMethod()>
    Public Function UpdMark(ByVal ck_id As String, ByVal id As String, ByVal mark As String, ByVal upd_user As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Dim Sql As New StringBuilder
        Sql.AppendLine("Update t_check_ms")
        Sql.AppendLine("Set ")
        Sql.AppendLine("    mark=N'" & mark.Replace("'", "’") & "' ")
        Sql.AppendLine("    ,upd_user='" & upd_user & "' ")
        Sql.AppendLine("    ,upd_date=getdate() ")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id='" & ck_id & "' ")
        Sql.AppendLine("    And id='" & id & "' ")
        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, Sql.ToString())
        Return jss.Serialize("OK")
    End Function

    Public Shared Function UpdToolsFlg(ByVal ck_id As String, ByVal tools_scan_flg As String, ByVal upd_user As String) As String
        Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        Dim Sql As New StringBuilder
        Sql.AppendLine("Update t_check")
        Sql.AppendLine("Set ")
        Sql.AppendLine("    tools_scan_flg=N'" & tools_scan_flg & "' ")
        Sql.AppendLine("    ,upd_user='" & upd_user & "' ")
        Sql.AppendLine("    ,upd_date=getdate() ")
        Sql.AppendLine("Where ")
        Sql.AppendLine("    ck_id='" & ck_id & "' ")
        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, Sql.ToString())
        Return jss.Serialize("OK")
    End Function

    '默认结果SQL
    Public Shared Function InsCopyDefault(ByVal shared_ck_id As String, ByVal no As String, ByVal ck_id As String, ByVal CLoginInfo As CLoginInfo) As String

        Dim sb As New StringBuilder
        sb.AppendLine("INSERT INTO [t_check]")


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
        sb.AppendLine("SELECT '" & ck_id & "'")
        sb.AppendLine("      ,a.[cd]")
        sb.AppendLine("      ,'" & no & "'")
        sb.AppendLine("      ,a.[department_cd]")
        sb.AppendLine("      ,a.[line_cd]")
        'sb.AppendLine("      ,[chk_user]")
        sb.AppendLine("      ,'" & CLoginInfo.user_cd & "' ")
        sb.AppendLine("      ,b.[yotei_chk_date]")
        sb.AppendLine("      ,Getdate()") 'chk_start_date
        sb.AppendLine("      ,Getdate()") 'chk_end_date
        'sb.AppendLine("      ,[chk_start_date]")
        'sb.AppendLine("      ,[chk_end_date]")
        sb.AppendLine("      ,'2'")   'status 0:检查中，  1:完了  2：默认结果完了 
        sb.AppendLine("      ,a.[result]")
        sb.AppendLine("      ,a.[chk_times]")
        sb.AppendLine("      ,b.[suu]")
        sb.AppendLine("      ,a.[del_flg]")
        sb.AppendLine("      ,a.[qianpin]")
        sb.AppendLine("      ,a.[tools_scan_flg]")
        sb.AppendLine("      ,'" & shared_ck_id & "'")
        sb.AppendLine("      ,a.[no]")


        sb.AppendLine("      ,b.h")
        sb.AppendLine("      ,b.w")
        sb.AppendLine("      ,b.dh")
        sb.AppendLine("      ,b.dw")
        sb.AppendLine("      ,b.sw")
        sb.AppendLine("      ,b.kw")
        sb.AppendLine("      ,b.specialBookNo")
        sb.AppendLine("      ,b.b2bOderNo")
        sb.AppendLine("      ,b.b2bIndexNo")
        sb.AppendLine("      ,b.sapOderNo")
        sb.AppendLine("      ,b.sapIndexNo")

        sb.AppendLine("      ,'" & CLoginInfo.user_cd & "' ")
        sb.AppendLine("      ,'" & CLoginInfo.user_cd & "' ")
        sb.AppendLine("      ,Getdate()")
        sb.AppendLine("      ,'" & CLoginInfo.user_cd & "' ")
        sb.AppendLine("      ,Getdate()")

        sb.AppendLine("      ,a.cd_no_sign")


        sb.AppendLine("FROM   [t_check] a")
        sb.AppendLine("LEFT JOIN   [t_check_plan] b")
        'sb.AppendLine("ON a.[no]=b.[no]")
        sb.AppendLine("ON b.[no]='" & no & "'")
        sb.AppendLine("WHERE")
        sb.AppendLine("        a.[ck_id] = '" & shared_ck_id & "'")



        sb.AppendLine("INSERT INTO [t_check_ms]")
        sb.AppendLine("SELECT [id]")
        sb.AppendLine("      ,[km_id]")
        sb.AppendLine("      ,[F1]")
        sb.AppendLine("      ,[F2]")
        sb.AppendLine("      ,[F3]")
        sb.AppendLine("      ,[F4]")
        sb.AppendLine("      ,[F5]")
        sb.AppendLine("      ,[F6]")
        sb.AppendLine("      ,[F7]")
        sb.AppendLine("      ,[F8]")
        sb.AppendLine("      ,[F9]")
        sb.AppendLine("      ,[F10]")
        sb.AppendLine("      ,[F11]")
        sb.AppendLine("      ,[F12]")
        sb.AppendLine("      ,[F13]")
        sb.AppendLine("      ,[F14]")
        sb.AppendLine("      ,[F15]")
        sb.AppendLine("      ,[F16]")
        sb.AppendLine("      ,[F17]")
        sb.AppendLine("      ,[F18]")
        sb.AppendLine("      ,[F19]")
        sb.AppendLine("      ,[F20]")
        sb.AppendLine("      ,[length]")
        sb.AppendLine("      ,[jizong_name]")
        sb.AppendLine("      ,[department_cd]")
        sb.AppendLine("      ,[sys_name]")
        sb.AppendLine("      ,[kind_name]")
        sb.AppendLine("      ,[kind_jun]")
        sb.AppendLine("      ,[pic_name]")
        sb.AppendLine("      ,[tools_ma]")
        sb.AppendLine("      ,[chk_pos]")
        sb.AppendLine("      ,[chk_km_name]")
        sb.AppendLine("      ,[hyouji_jun]")
        sb.AppendLine("      ,[yousen_jun]")
        sb.AppendLine("      ,[k_type]")
        sb.AppendLine("      ,[k1]")
        sb.AppendLine("      ,[k2]")
        sb.AppendLine("      ,[k3]")
        sb.AppendLine("      ,[chk_fmt]")
        sb.AppendLine("      ,[chk_fs]")
        sb.AppendLine("      ,chk_fs_txt")
        sb.AppendLine("      ,[chk_times]")

        sb.AppendLine("      ," & ck_id & "")

        sb.AppendLine("      , [cd]")
        sb.AppendLine("      , [no]")
        sb.AppendLine("      , [in_1]")
        sb.AppendLine("      , [Result]")
        sb.AppendLine("      , [mark]")
        sb.AppendLine("      ,'" & CLoginInfo.user_cd & "' ")
        sb.AppendLine("      ,Getdate()")
        sb.AppendLine("      ,'" & CLoginInfo.user_cd & "' ")
        sb.AppendLine("      ,Getdate()")
        sb.AppendLine("FROM   [t_check_ms]")
        sb.AppendLine("WHERE")
        sb.AppendLine("       [ck_id] = '" & shared_ck_id & "'")

        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())

        Return ""

    End Function


    <WebMethod()>
    Public Function GetResultSum(ByVal ck_id As String, ByVal ky As String) As String
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT ms.[id]")
        sb.AppendLine(", isnull(ms.[result],'') result")
        sb.AppendLine("FROM [t_check_ms] ms")
        sb.AppendLine("WHERE  ms.ck_id = '" & ck_id & "'")
        Dim dt As DataTable = FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "ChkMs")

        Return dt.Select("result='OK'").Count _
                & "," & dt.Select("result='SD'").Count _
                & "," & dt.Select("result='JN'").Count _
                & "," & dt.Select("result='M1'").Count _
                & "," & dt.Select("result='M2'").Count _
                & "," & dt.Select("result='M3'").Count _
                & "," & dt.Select("result='NG'").Count _
                & "," & dt.Select("result=''").Count _
                & "," & dt.Rows.Count
        'Dim jss As JavaScriptSerializer = New JavaScriptSerializer
        'Return JsonConvert.SerializeObject(dt)

    End Function

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

End Class
