
Imports System.IO

Partial Class Pics1
    Inherits System.Web.UI.Page

    Private Sub Pics1_Load(sender As Object, e As EventArgs) Handles Me.Load

        If (Request.QueryString("kbn") = "chk") Then

            Dim idx As String = Request.QueryString("idx")

            Try

            Catch ex As Exception

            End Try
            Dim cm As New ComDDL
            Dim dtImg As Data.DataTable = cm.SelChkResultPicture(idx)
            'GvPic.DataSource = dtImg
            'GvPic.DataBind()

            For i As Integer = 0 To dtImg.Rows.Count - 1

                Dim MStream As New MemoryStream(CType(dtImg.Rows(i).Item("pic_conn"), Byte()))
                Dim base64 As String = Convert.ToBase64String(MStream.ToArray())
                Image1.ImageUrl = "data:image/jpg;base64," + base64

                ' AddHandler btn.Click, AddressOf Me.btClick
            Next
        ElseIf (Request.QueryString("kbn") = "ms") Then

            'picNm,picTm
            Dim picNm As String = Request.QueryString("picNm")
            Dim picTm As String = Request.QueryString("picTm")
            Try

            Catch ex As Exception

            End Try
            Dim cm As New ComDDL
            Dim dtImg As Data.DataTable = cm.SelChkMsPicture(picNm, picTm)
            'GvPic.DataSource = dtImg
            'GvPic.DataBind()

            For i As Integer = 0 To dtImg.Rows.Count - 1

                Dim MStream As New MemoryStream(CType(dtImg.Rows(i).Item("pic_conn"), Byte()))
                Dim base64 As String = Convert.ToBase64String(MStream.ToArray())
                Image1.ImageUrl = "data:image/jpg;base64," + base64

                ' AddHandler btn.Click, AddressOf Me.btClick
            Next


        End If





    End Sub
End Class
