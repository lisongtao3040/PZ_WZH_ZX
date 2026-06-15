
Partial Class TP_StatusUpd
    Inherits System.Web.UI.Page
    Private CLoginInfo As CLoginInfo
    Private myTpDA As New tpDA

    Private Sub TP_StatusUpd_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit

        If Not IsPostBack Then

            If Context.Items("tp_page_kbn") Is Nothing Then
                ViewState("tp_page_kbn") = "3"
            Else
                ViewState("tp_page_kbn") = Context.Items("tp_page_kbn")
            End If

        End If

        If (ViewState("tp_page_kbn") = "3") Then
            UserHeader.title = "状态(2.生产完➡3.待检)"
            Me.btnRe.Text = "4.品检查完了迁移"
        Else
            UserHeader.title = "状态(3.待检➡4.检查完了)"
            Me.btnRe.Text = "3.待检迁移"
        End If


    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblMsg.Text = ""
        lblMsgOK.Text = ""

        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)
        If Not IsPostBack Then
            UserHeader.InitLoginInfo(CLoginInfo)

            If Context.Items("tp_page_kbn") Is Nothing Then
                ViewState("tp_page_kbn") = "3"
            Else
                ViewState("tp_page_kbn") = Context.Items("tp_page_kbn")
            End If
        End If
        MsInit()


    End Sub

    Private Sub MsInit()

        Dim dt As Data.DataTable
        Dim trolleyStatus As String

        'If (ViewState("tp_page_kbn") = "3") Then
        '    Context.Items("tp_page_kbn") = "4"
        'Else
        '    Context.Items("tp_page_kbn") = "3"
        'End If

        If ViewState("tp_page_kbn") = "3" Then
            trolleyStatus = "2"
            dt = myTpDA.GetTPListSigle(trolleyStatus) '生产完
        Else
            trolleyStatus = "3" '待检
            dt = myTpDA.GetTPListSigle(trolleyStatus)
        End If

        divBtns.Controls.Clear()

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim btn As New Button
            btn.ID = "btnUpdTp" & dt.Rows(i).Item("trolleyNo")
            btn.CommandArgument = "btnUpdTpCommandArgument" & dt.Rows(i).Item("trolleyNo")
            btn.Text = dt.Rows(i).Item("trolleyNo").ToString()
            divBtns.Controls.Add(btn)
            AddHandler btn.Click, AddressOf Me.btClick

        Next

    End Sub

    Private Sub btClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button
        btn = CType(sender, Button)
        'MsgBox(btn.Tag.ToString.PadLeft(3, "0") & ":" & btn.Text)

        Dim rtv As String

        If ViewState("tp_page_kbn") = "3" Then
            rtv = myTpDA.UpdTrolleyStatus(btn.Text.ToString.Trim, "2", "3")
        Else
            rtv = myTpDA.UpdTrolleyStatus(btn.Text.ToString.Trim, "3", "4")
        End If

        If rtv <> "" Then
            lblMsg.Text = rtv
        Else
            lblMsgOK.Text = "OK: 托盘(" & btn.Text.ToString.Trim & ")更新成功"
        End If

        MsInit()

    End Sub


    Protected Sub btnRe_Click(sender As Object, e As EventArgs) Handles btnRe.Click
        If (ViewState("tp_page_kbn") = "3") Then
            Context.Items("tp_page_kbn") = "4"
        Else
            Context.Items("tp_page_kbn") = "3"
        End If
        Server.Transfer("TP_StatusUpd.aspx")
    End Sub
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click

        Context.Items("CLoginInfo") = ViewState("CLoginInfo")
        Server.Transfer("Menu.aspx")


    End Sub
    Protected Sub btnRefesh_Click(sender As Object, e As EventArgs) Handles btnRefesh.Click
        If (ViewState("tp_page_kbn") = "3") Then
            Context.Items("tp_page_kbn") = "3"
        Else
            Context.Items("tp_page_kbn") = "4"
        End If
        Server.Transfer("TP_StatusUpd.aspx")
    End Sub
End Class
