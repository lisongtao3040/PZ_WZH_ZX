
Partial Class TP_ms
    Inherits System.Web.UI.Page
    Private CLoginInfo As CLoginInfo
    Private myTpDA As New tpDA
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)
        If Not IsPostBack Then
            UserHeader.InitLoginInfo(CLoginInfo)

            ViewState("tp_no") = Context.Items("tp_no")
            MsInit()
        End If
    End Sub
    Sub MsInit()
        Dim tp_no As String = ViewState("tp_no")
        Dim dt As Data.DataTable = myTpDA.GetTPMs(tp_no)
        Me.gv.DataSource = dt
        Me.gv.DataBind()

    End Sub
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Context.Items("CLoginInfo") = ViewState("CLoginInfo")
        Server.Transfer("TP_list.aspx")
    End Sub
End Class
