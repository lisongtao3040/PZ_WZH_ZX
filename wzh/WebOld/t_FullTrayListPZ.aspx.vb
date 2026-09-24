
Partial Class t_FullTrayListPZ
    Inherits System.Web.UI.Page

    Private CLoginInfo As CLoginInfo
    Private BC As New t_FullTrayListPZBC

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '加载画面参数
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)
        If Not IsPostBack Then
            lblUserCd.Text = CLoginInfo.user_cd
            lblUserName.Text = CLoginInfo.user_name

            '部門コードをユーザー名の後ろに括弧付きで表示（例：（2））
            Dim departmentCd As String = BC.GetDepartmentCd(CLoginInfo.user_cd)
            lblUserDept.Text = If(departmentCd = "", "", "（" & departmentCd & "）")
        End If
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Server.Transfer("t_check_list.aspx")
    End Sub

End Class
