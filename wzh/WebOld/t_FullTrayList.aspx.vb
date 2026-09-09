
Partial Class t_FullTrayList
    Inherits System.Web.UI.Page

    Private CLoginInfo As CLoginInfo
    Private BC As New t_checkBC

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '加载画面参数
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)
        lblMsg.Text = ""
        If Not IsPostBack Then
            lblUserCd.Text = CLoginInfo.user_cd
            lblUserName.Text = CLoginInfo.user_name
        End If
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Server.Transfer("t_check_list.aspx")
    End Sub

    ''' <summary>
    ''' 検査ボタン：btnNewChk_Click と同じ処理で t_check_ms へ遷移
    ''' </summary>
    Protected Sub btnChkServer_Click(sender As Object, e As EventArgs) Handles btnChkServer.Click
        Dim cd As String = hid_chk_cd.Value.Trim()
        Dim no As String = hid_chk_no.Value.Trim()
        Dim tpNo As String = hid_chk_tpno.Value.Trim()

        If cd = "" OrElse no = "" Then Exit Sub

        Dim ck_id As String = PageCom.GetNewCheckId()
        Dim msg As String = BC.CreateNewChkAutoOK(cd, no, ck_id, CLoginInfo.user_cd, CLoginInfo.department_cd, CLoginInfo.line_cd, "")

        If msg <> "" Then

            lblMsg.Text = msg
        Else
            CLoginInfo.cd = cd
            CLoginInfo.no = no
            CLoginInfo.ck_id = ck_id
            CLoginInfo.jxs_name = ""
            CLoginInfo.TpNo = tpNo
            Session("CLoginInfo") = CLoginInfo
            Server.Transfer("t_check_ms.aspx")
        End If
    End Sub

    ''' <summary>
    ''' 自动OK ボタン：検査データを自動登録して OK にする
    ''' </summary>
    Protected Sub btnAutoOkServer_Click(sender As Object, e As EventArgs) Handles btnAutoOkServer.Click
        Dim cd As String = hid_chk_cd.Value.Trim()
        Dim no As String = hid_chk_no.Value.Trim()
        Dim tpNo As String = hid_chk_tpno.Value.Trim()

        If cd = "" OrElse no = "" Then Exit Sub

        Dim ck_id As String = PageCom.GetNewCheckId()
        Dim msg As String = BC.CreateNewChkAutoOK(cd, no, ck_id, CLoginInfo.user_cd, CLoginInfo.department_cd, CLoginInfo.line_cd, "")

        If msg <> "" Then
            lblMsg.Text = msg
        Else
            lblMsg.Text = "自动OK 专用处理O 完成"
            '' 自动OK 专用处理（此处可追加自动设置 OK 结果的逻辑）
            'CLoginInfo.cd = cd
            'CLoginInfo.no = no
            'CLoginInfo.ck_id = ck_id
            'CLoginInfo.jxs_name = ""
            'CLoginInfo.TpNo = tpNo
            'Session("CLoginInfo") = CLoginInfo
            'Server.Transfer("t_check_ms.aspx")
        End If
    End Sub

End Class
