Imports System.Data
Imports SqlHelper

Partial Class t_check_test
    Inherits System.Web.UI.Page

    Private BC As New t_checkBC

    Private Sub t_check_test_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not IsPostBack Then
            ViewState("user_cd") = "3333333"
            'ViewState("cd") = "CHADDLWXXX12345"
            'ViewState("no") = "makeno"

        End If


    End Sub

    '制作新的检查
    Protected Sub btnSelKm_Click(sender As Object, e As EventArgs) Handles btnSelKm.Click
        Dim ck_id As String = PageCom.GetNewCheckId()
        Dim user_cd As String = ViewState("user_cd")
        Dim cd As String = Me.tbxCd.Text
        Dim no As String = Me.tbxNo.Text
        Dim dt As DataTable = BC.GetKmDt(cd, no, ck_id, user_cd)
        Me.gvKm.DataSource = dt
        Me.gvKm.DataBind()
    End Sub

    '从检查模板获得检查项目




End Class
