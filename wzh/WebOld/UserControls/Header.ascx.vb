
Partial Class UserControls_Header
    Inherits System.Web.UI.UserControl
    Public Property title As String

    '装载登录信息
    Public Sub InitLoginInfo(ByVal CLoginInfo As CLoginInfo)
        Me.lblUserCd.Text = CLoginInfo.user_cd
        Me.lblUserName.Text = CLoginInfo.user_name
        Me.lblLineCd.Text = CLoginInfo.line_cd
        Me.lblLineName.Text = CLoginInfo.line_name
    End Sub

    Private Sub UserControls_Header_Init(sender As Object, e As EventArgs) Handles Me.Init
        'ログイン
        Me.lblTitle.Text = title
        Me.Page.Title = title
    End Sub

End Class
