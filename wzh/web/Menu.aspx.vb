
Partial Class Menu
    Inherits System.Web.UI.Page

    Private CLoginInfo As CLoginInfo


    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles Me.Load

        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)

        If Not IsPostBack Then
            UserHeader.InitLoginInfo(CLoginInfo)
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
End Class
