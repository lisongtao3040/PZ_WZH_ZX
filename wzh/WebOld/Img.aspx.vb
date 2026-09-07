Imports System.Data

Partial Class Img
    Inherits System.Web.UI.Page

    Private ComDDL As New ComDDL

    ''' <summary>
    ''' 图片取得
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim bt As Byte()
            If Request.QueryString("pic_name") = "" Then
                Response.End()
            End If
            Dim pic_name As String = Request.QueryString("pic_name").Split("|")(0)

            Dim pic_upd_time As String = Request.QueryString("pic_name").Split("|")(1)



            Dim dt As DataTable = ComDDL.SelChkPictureKM(pic_name, pic_upd_time)
            If dt.Rows.Count > 0 Then
                bt = DirectCast(dt.Rows(0).Item("pic_conn"), Byte())
                Response.BinaryWrite(bt)
            End If


            Response.End()
        Catch ex As Exception
        End Try

    End Sub

End Class
