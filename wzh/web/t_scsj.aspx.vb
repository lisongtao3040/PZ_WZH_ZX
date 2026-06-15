
Partial Class t_scsj
    Inherits System.Web.UI.Page

    Public scsjBC As New t_scsjBC
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then

            'Dim ymd As String = DateAdd(DateInterval.Day, -1, Now).ToString("yyyy-MM-dd")
            Me.tbxDateStart.Text = DateAdd(DateInterval.Day, -1, Now).ToString("yyyy-MM-dd")
            Me.tbxDateEnd.Text = DateAdd(DateInterval.Day, 0, Now).ToString("yyyy-MM-dd")

            Dim department_cd As String = Request.QueryString("bu")
            If department_cd Is Nothing Then department_cd = ""

            If department_cd = "" Then

            Else
                department_cd = "'" & department_cd.Replace("-", "','") & "'"
            End If
            ViewState("department_cd") = department_cd

            'Dim line_cd As String = Request.QueryString("line_cd")
            initMs()
        End If
    End Sub



    Sub initMs()

        Dim data As Data.DataTable = scsjBC.GetScsj(ViewState("department_cd"), Me.tbxDateStart.Text, Me.tbxDateEnd.Text)

        Me.gvLastCheckResultMS.DataSource = data
        Me.gvLastCheckResultMS.DataBind()
    End Sub

    '结果文字颜色
    Public Function GetResultEle(ByVal ck_id As String, ByVal result As String, ByVal status As String) As String
        Dim sb As New StringBuilder
        sb.Append("<a ")
        sb.Append("style=""")
        If result = "NG" Then
            sb.Append("color:red;")
        ElseIf result = "OK" Then
            sb.Append("color:Green;")
        Else

        End If
        sb.Append("""")
        sb.Append(">")
        sb.Append(result)

        sb.Append("<br>")
        If status = "0" Then
            sb.Append("检查中")
        ElseIf status = "1" Then
            sb.Append("完了")
        ElseIf status = "2" Then
            sb.Append("继承")
        ElseIf status = "4" Then
            sb.Append("手入力")
        End If


        sb.Append("</a>")

        Return sb.ToString

    End Function

    Protected Sub btnSelChujian_Click(sender As Object, e As EventArgs) Handles btnSelChujian.Click
        initMs()
    End Sub
End Class
