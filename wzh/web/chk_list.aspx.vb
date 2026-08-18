Partial Class chk_list
    Inherits System.Web.UI.Page

    'LOAD
    Private Sub chk_list_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            '直接浏览器访问（无登录信息），不加载用户信息
        End If

    End Sub
End Class
