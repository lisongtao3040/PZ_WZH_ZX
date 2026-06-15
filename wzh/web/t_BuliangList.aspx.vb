
Partial Class t_BuliangList
    Inherits System.Web.UI.Page
    Public BC As New t_BuLiangBC
    Private Sub form1_Load(sender As Object, e As EventArgs) Handles form1.Load

        If Not IsPostBack Then


            Dim department_cd As String = Request.QueryString("bu")
            'Dim cd As String = Request.QueryString("cd")
            'Dim no As String = Request.QueryString("no")
            Dim rinei As String = Request.QueryString("rinei")

            If department_cd = "" Then

            Else
                department_cd = "'" & department_cd.Replace("-", "','") & "'"
            End If
            ViewState("department_cd") = department_cd

            '加载日内列表
            HiListInit(Me.ddlHiinai)

            ddlHiinai.SelectedValue = rinei


            InitMS("", "", rinei, department_cd)




        End If



    End Sub

    Sub InitMS(ByVal cd As String, ByVal no As String, ByVal rinei As String, ByVal department_cd As String)
        Dim dt2 As Data.DataTable = BC.GetBuLiangList(cd, no, rinei, department_cd)

        Dim dt As Data.DataTable = dt2.Clone
        dt.Clear()

        For Each dr As Data.DataRow In dt2.Select("buliang<>'不良 已重检OK' and buliang <> '不良代替 重检OK'")
            dt.Rows.Add(dr.ItemArray)
        Next

        gvLastCheckResultMS.DataSource = dt
        gvLastCheckResultMS.DataBind()


        For i As Integer = 0 To gvLastCheckResultMS.DataSource.Rows.Count - 1

            If gvLastCheckResultMS.DataSource.Rows(i).Item("buliang").ToString().Contains("NG") Then
                gvLastCheckResultMS.Rows(i).Cells(5).BackColor = Drawing.Color.OrangeRed

            End If


        Next

    End Sub

    '几日以内
    Private Sub HiListInit(ByRef ddlHiinai As DropDownList)
        ddlHiinai.Items.Clear()
        PageCom.GetComRinei(ddlHiinai)
        ddlHiinai.SelectedIndex = 0
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

    Protected Sub btnSel_Click(sender As Object, e As EventArgs) Handles btnSel.Click

        Dim rinei As String = (Me.ddlHiinai.SelectedValue.Trim)
        InitMS(Me.tbxCd.Text.Trim, Me.tbxNo.Text.Trim, rinei, ViewState("department_cd"))

    End Sub
End Class
