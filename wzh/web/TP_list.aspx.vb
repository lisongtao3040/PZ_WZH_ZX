
Partial Class TP_list
    Inherits System.Web.UI.Page
    Private CLoginInfo As CLoginInfo
    Private myTpDA As New tpDA

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)
        If Not IsPostBack Then
            UserHeader.InitLoginInfo(CLoginInfo)
            MsInit()
        End If




    End Sub

    Sub MsInit()
        Dim dt As Data.DataTable = myTpDA.GetTPList("3")
        gv.DataSource = dt
        gv.DataBind()

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim lb As LinkButton = gv.Rows(i).FindControl("lbTp_no")
            'lb.ID = "lb" & dt.Rows(i).Item("托盘号")
            'lb.Text = dt.Rows(i).Item("托盘号").ToString()
            'AddHandler lb.Click, AddressOf Me.btClick
            lb.Attributes.Item("onclick") = "$('#hidTpNo').val('" & dt.Rows(i).Item("托盘号") & "');$('#btnMs').click();return false;"
        Next


    End Sub

    Private Sub btClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As LinkButton
        btn = CType(sender, LinkButton)
        'MsgBox(btn.Tag.ToString.PadLeft(3, "0") & ":" & btn.Text)
        Context.Items("tp_no") = btn.Text.Trim
        Context.Items("CLoginInfo") = ViewState("CLoginInfo")
        Server.Transfer("TP_ms.aspx")

    End Sub
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Context.Items("CLoginInfo") = ViewState("CLoginInfo")
        Server.Transfer("Menu.aspx")
    End Sub

    Public Function Ymd(ByVal v As String)

        Try
            Return Left(v, 4) & "-" & v.Substring(4, 2) & "-" & v.Substring(6, 2)
        Catch ex As Exception
            Return v
        End Try

    End Function
    Protected Sub btnMs_Click(sender As Object, e As EventArgs) Handles btnMs.Click
        Context.Items("tp_no") = Me.hidTpNo.Value
        Context.Items("CLoginInfo") = ViewState("CLoginInfo")
        Server.Transfer("TP_ms.aspx")
    End Sub
End Class
