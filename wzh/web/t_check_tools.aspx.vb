
Partial Class t_check_tools
    Inherits System.Web.UI.Page


    Private CLoginInfo As CLoginInfo
    Private BC As New t_checkBC
    Private DA As New t_checkDA

    Private Sub t_check_tools_Load(sender As Object, e As EventArgs) Handles Me.Load

        '加载画面参数
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)

        If Not IsPostBack Then

            Me.UserHeader.InitLoginInfo(CLoginInfo)

            Dim dt As Data.DataTable = BC.GetCheckToolsData(CLoginInfo.ck_id, CLoginInfo)
            If dt.Select("tools_scan_flg='1'").Length > 0 Then
                CLoginInfo.IsToolStyle = True
                Context.Items("CLoginInfo") = CLoginInfo
                Server.Transfer("t_check_ms.aspx")
                Exit Sub
            End If
            gvTools.DataSource = dt
            gvTools.DataBind()

            Me.tbxCd.Text = CLoginInfo.cd
            Me.tbxNo.Text = CLoginInfo.no
            Me.tbxCk_id.Text = CLoginInfo.ck_id
            'Me.tbxUserCd.Text = CLoginInfo.user_cd
            'Me.tbxUserName.Text = CLoginInfo.user_name
            'Me.lblLineCd.Text = CLoginInfo.line_cd
            'Me.lblLineName.Text = CLoginInfo.line_name
        End If

    End Sub

    Private Sub btnUpdToolsFlg_Click(sender As Object, e As EventArgs) Handles btnUpdToolsFlg.Click
        Result.UpdToolsFlg(CLoginInfo.ck_id, "1", CLoginInfo.user_cd)
        CLoginInfo.IsToolStyle = True
        Server.Transfer("t_check_ms.aspx")
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Server.Transfer("t_check_ms.aspx")
    End Sub
End Class
