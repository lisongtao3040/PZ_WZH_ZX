
Imports System.Data
Imports System.Text
Imports System.IO
Imports SqlHelper
Partial Class Menu
    Inherits System.Web.UI.Page

    Private CLoginInfo As CLoginInfo


    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles Me.Load

        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)

        If Not IsPostBack Then
            UserHeader.InitLoginInfo(CLoginInfo)
            ExecuteMyTask()
        End If

    End Sub

    '用户登录画面
    Protected Sub btnUserIns_Click(sender As Object, e As EventArgs) Handles btnUserIns.Click
        Server.Transfer("MsUser.aspx")
    End Sub

    'Back
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Server.Transfer("Default.aspx")
    End Sub

    '模版
    Protected Sub btntemplate_Click(sender As Object, e As EventArgs) Handles btntemplate.Click
        Server.Transfer("MsKmTemplate.aspx")
    End Sub
    Protected Sub btnTmpAndLevelJoin_Click(sender As Object, e As EventArgs) Handles btnTmpAndLevelJoin.Click
        Server.Transfer("MstemplateLevel.aspx")
    End Sub
    Protected Sub btnChkMs_Click(sender As Object, e As EventArgs) Handles btnChkMs.Click
        Server.Transfer("t_check_list.aspx")
    End Sub
    Protected Sub btnMsLine_Click(sender As Object, e As EventArgs) Handles btnMsLine.Click
        Server.Transfer("MsLine.aspx")
    End Sub
    Protected Sub btnMsTools_Click(sender As Object, e As EventArgs) Handles btnMsTools.Click
        Server.Transfer("MsTools.aspx")
    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Server.Transfer("t_check_editor.aspx")
    End Sub
    'Protected Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
    '    Server.Transfer("MiCHeckItiran.aspx")
    'End Sub
    'Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    '    Server.Transfer("VCheckNewResult.aspx")
    'End Sub
    'Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
    '    Server.Transfer("v_check_result_sou.aspx")
    'End Sub
    'Protected Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
    '    Server.Transfer("v_check_result_ms.aspx")
    'End Sub

    '关联商品CD 与 体系
    Protected Sub btnCdSysJoin_Click(sender As Object, e As EventArgs) Handles btnCdSysJoin.Click
        Server.Transfer("MsSysJoin.aspx")
    End Sub

    '检查结果明细表
    Protected Sub btnChkResultList_Click(sender As Object, e As EventArgs) Handles btnChkResultList.Click
        Server.Transfer("pt_v_A01_check_result.aspx")
    End Sub
    Protected Sub btnChkResultListMS_Click(sender As Object, e As EventArgs) Handles btnChkResultListMS.Click
        Server.Transfer("pt_v_A02_check_result_ms.aspx")
    End Sub
    Protected Sub btnMiCheckList_Click(sender As Object, e As EventArgs) Handles btnMiCheckList.Click
        Server.Transfer("pt_v_A03_mi_check_result.aspx")
    End Sub
    Protected Sub btnChkResultListEdit_Click(sender As Object, e As EventArgs) Handles btnChkResultListEdit.Click
        Server.Transfer("pt_v_A04_check_result.aspx")
    End Sub
    Protected Sub btnMubiao_Click(sender As Object, e As EventArgs) Handles btnMubiao.Click
        Server.Transfer("MsMubiaoTime.aspx")
    End Sub
    Protected Sub btnSCX_Click(sender As Object, e As EventArgs) Handles btnSCX.Click
        Server.Transfer("pt_v_A05_check_scx.aspx")
    End Sub
    Protected Sub btnQuanshu_Click(sender As Object, e As EventArgs) Handles btnQuanshu.Click
        Server.Transfer("TB_SetAllCheck.aspx")
    End Sub
    Protected Sub btnSame_Click(sender As Object, e As EventArgs) Handles btnSame.Click
        Server.Transfer("MSt_first_check.aspx")
    End Sub
    Protected Sub btnGoodsName_Click(sender As Object, e As EventArgs) Handles btnGoodsName.Click
        Server.Transfer("MsSyouhin.aspx")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Server.Transfer("MsCheckMs.aspx")
    End Sub
    Protected Sub btn23_Click(sender As Object, e As EventArgs) Handles btn23.Click
        'If (ViewState("tp_page_kbn") = "3") Then
        '    Context.Items("tp_page_kbn") = "4"
        'Else
        '    Context.Items("tp_page_kbn") = "3"
        'End If
        Context.Items("tp_page_kbn") = "3"
        Server.Transfer("TP_StatusUpd.aspx")
    End Sub
    Protected Sub btn34_Click(sender As Object, e As EventArgs) Handles btn34.Click
        Context.Items("tp_page_kbn") = "4"
        Server.Transfer("TP_StatusUpd.aspx")
    End Sub
    Protected Sub btnTpList_Click(sender As Object, e As EventArgs) Handles btnTpList.Click
        Server.Transfer("TP_list.aspx")
    End Sub
    Protected Sub btnFirstChk_Click(sender As Object, e As EventArgs) Handles btnFirstChk.Click

        Server.Transfer("m_first_chk_cds.aspx")

    End Sub
    Protected Sub btnMakePlan_Click(sender As Object, e As EventArgs) Handles btnMakePlan.Click
        Dim rtv As String = ImportPlan()

        PageCom.ShowMsg2(Me.Page, "导入的数据" & rtv & "件", "")
    End Sub
    Function ImportPlan() As String

        Dim startDate As String = DateAdd(DateInterval.Day, -15, Now).ToString("yyyy-MM-dd HH:mm:ss.fff")


        Dim dtLastUpdDate As DataTable = ComDDL.Se1_PlanLastUpdateDate()

        Dim dt As DataTable = ComDDL.Se1_15DaysPlan(startDate)

        Try

            Using conn As New SqlClient.SqlConnection(DataAccessManager.ConnStr)

                conn.Open()

                Dim sql As String
                sql = "DELETE FROM m_plan_no_sign where insertdate >'" & startDate & "'"
                Dim SqlCommand As System.Data.SqlClient.SqlCommand = conn.CreateCommand()
                SqlCommand.CommandText = sql
                SqlCommand.CommandTimeout = 0
                SqlCommand.ExecuteNonQuery()

                Dim bCopy As New SqlClient.SqlBulkCopy(conn)
                bCopy.BulkCopyTimeout = 30
                bCopy.DestinationTableName = "m_plan_no_sign"
                bCopy.WriteToServer(dt)

                Dim tableName As String = dtLastUpdDate.Rows(0).Item("TableName").ToString
                Dim LastUpdatedTime As String = Convert.ToDateTime(dtLastUpdDate.Rows(0).Item("LastUpdatedTime")).ToString("yyyy-MM-dd HH:mm:ss")

                Dim sql2 As String
                sql2 = "delete from [m_plan_upd_kanri] where [TableName] = '" & tableName & "'" & vbCrLf
                sql2 = sql2 & "INSERT INTO [m_plan_upd_kanri] select '" & tableName & "','" & LastUpdatedTime & "'" & vbCrLf

                Dim SqlCommand2 As System.Data.SqlClient.SqlCommand = conn.CreateCommand()
                SqlCommand2.CommandText = sql2
                SqlCommand2.CommandTimeout = 0
                SqlCommand2.ExecuteNonQuery()

                Return dt.Rows.Count

            End Using
        Catch ex As Exception
            Return ex.Message
        End Try




    End Function

    ''' <summary>
    ''' 实际要执行的业务代码
    ''' </summary>
    Private Sub DoRealWork()
        ' TODO: 在这里写你的业务逻辑
        If (ComDDL.Se1_IsPlanNeedUpd) Then
            ImportPlan()
        End If
    End Sub


    ''' <summary>
    ''' 需要限制频率的主函数
    ''' </summary>
    Public Sub ExecuteMyTask()
        ' 定义该任务在 Cache 中的唯一 Key（可根据业务调整，如加用户ID: "MyTask_User123"）
        Dim cacheKey As String = "LastExecuteTime_MyTask"

        ' 1. 检查 Cache 中是否存在该 Key
        If HttpRuntime.Cache(cacheKey) IsNot Nothing Then
            ' 存在说明 15 分钟内执行过，直接退出
            ' 如果是页面，这里可以提示用户："操作太频繁，请15分钟后再试"
            Return
        End If


        ' 3. 执行成功后，写入 Cache，并设置 15 分钟后绝对过期
        HttpRuntime.Cache.Insert(cacheKey, True, Nothing, DateTime.Now.AddMinutes(15), Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)


        ' 2. 执行你的核心业务逻辑
        DoRealWork()


    End Sub
End Class
