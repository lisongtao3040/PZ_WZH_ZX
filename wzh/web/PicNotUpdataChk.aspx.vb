
Partial Class PicNotUpdataChk
    Inherits System.Web.UI.Page

    Private Sub PicNotUpdataChk_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then

            Me.GridView1.DataSource = DbCom.PicNotUpdataChk
            Me.GridView1.DataBind()




        End If
    End Sub
End Class
