
Partial Class InputByHand
    Inherits System.Web.UI.Page

    Private CLoginInfo As CLoginInfo
    Private BC As New t_checkBC
    Private DA As New t_checkDA

    Private Sub InputByHand_Load(sender As Object, e As EventArgs) Handles Me.Load
        '加载画面参数
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)

        If Not IsPostBack Then

            UserHeader.InitLoginInfo(CLoginInfo)

        End If
    End Sub

    '返回
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Server.Transfer("t_check_list.aspx")
    End Sub

    '登录
    Protected Sub btnIns_Click(sender As Object, e As EventArgs) Handles btnIns.Click

        Dim ck_id As String = PageCom.GetNewCheckId()
        Dim cd As String = Me.tbxCd.Text.Trim
        Dim no As String = Me.tbxNo.Text.Trim
        Dim suu As String = Me.tbxSuu.Text.Trim
        Dim department_cd As String = tbxBumen.Text.Trim
        Dim line_cd As String = Me.tbxLine.Text.Trim
        Dim result As String
        If rbtnOk.Checked Then
            result = "OK"
        Else
            result = "NG"
        End If
        If cd = "" Or no = "" Or suu = "" Or department_cd = "" Then
            PageCom.ShowMsg3(Me.Page, "请输入完整")
            Exit Sub
        End If

        Dim msg As String = BC.CreateNewChkHand(cd, no, ck_id, CLoginInfo.user_cd, department_cd, line_cd, suu, result)

        If msg = "" Then
            Server.Transfer("t_check_list.aspx")
        Else
            PageCom.ShowMsg3(Me.Page, msg)
        End If



    End Sub
End Class
