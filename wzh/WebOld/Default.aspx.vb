Imports UserControls

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click

        Dim userDA As New UserDA
        Dim userDt As Data.DataTable = UserDA.GetUser(Me.tbxUserCd.Text.Trim)

        If userDt.Rows.Count <= 0 Then
            PageCom.ShowMsg2(Me.Page, "用户不存在", Me.tbxUserCd.ClientID)
        Else
            If userDt.Rows(0).Item("password").ToString = tbxPassword.Text Then
                Dim cLoginInfo As New CLoginInfo
                cLoginInfo.user_cd = Me.tbxUserCd.Text.Trim
                cLoginInfo.user_name = userDt.Rows(0).Item("user_name").ToString
                cLoginInfo.password = Me.tbxPassword.Text
                cLoginInfo.authority = userDt.Rows(0).Item("authority").ToString
                cLoginInfo.department_cd = userDt.Rows(0).Item("department_cd").ToString
                cLoginInfo.line_cd = userDt.Rows(0).Item("line_cd").ToString
                cLoginInfo.line_name = userDt.Rows(0).Item("line_name").ToString
                Context.Items("CLoginInfo") = cLoginInfo
                If cLoginInfo.authority = "1" Then
                    Server.Transfer("Menu.aspx")
                Else
                    Server.Transfer("t_check_list.aspx")
                End If
            Else
                PageCom.ShowMsg2(Me.Page, "密码错误", Me.tbxPassword.ClientID)
            End If
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ClientScript.RegisterStartupScript(Page.GetType(), "", "")
        ' me.btnLogin.Enabled 
        If Not IsPostBack Then
            JsLoad()
        End If
    End Sub


    Private Sub JsLoad()

        Dim sb As New StringBuilder
        sb.AppendLine("<script language='javascript'>")
        sb.AppendLine("$(document).ready(function () {")
        sb.AppendLine("$.cookie('sl_user_cd', '');")
        sb.AppendLine("$.cookie('sl_user_name', '');")
        sb.AppendLine("$.cookie('sl_authority', '');")
        sb.AppendLine("$.cookie('sl_password', '');")
        sb.AppendLine("});")
        sb.AppendLine("</script>")
        ClientScript.RegisterStartupScript(Page.GetType(), "JsLoad", sb.ToString())

    End Sub
End Class
