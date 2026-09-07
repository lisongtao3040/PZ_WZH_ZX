
Partial Class MsLine
    Inherits System.Web.UI.Page
    Private CLoginInfo As CLoginInfo
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)
        If Not IsPostBack Then
            JsLoad()
        End If
    End Sub

    Private Sub JsLoad()


        Dim sb As New StringBuilder
        sb.AppendLine("<script language='javascript'>")
        sb.AppendLine("$(document).ready(function () {")
        sb.AppendLine("")
        '共通的画面设定
        sb.AppendLine("    var pubInitParams = {")
        sb.AppendLine("        fixKeyCols:false,")  '固定列
        sb.AppendLine("    };")

        sb.AppendLine("    var colsParams = {")
        sb.AppendLine("        line_cd: {")
        sb.AppendLine("            tooltip: ""组：(必须 英数字)<br>*请注意： 相同组成员只能操作自己组数据""")
        sb.AppendLine("            , reg: /^[a-zA-Z0-9-\%]+$/")
        sb.AppendLine("        }")
        sb.AppendLine("        ,line_name: {")
        sb.AppendLine("            tooltip: """"")
        sb.AppendLine("            ,width:300")
        sb.AppendLine("        }")

        sb.AppendLine("    };")
        sb.AppendLine("")
        sb.AppendLine("    var awaysWhere = """";")
        sb.AppendLine("    var table = new dbTable(""article"", "".jqPanelTable"", "".jqPanelNumOfPages"", ""m_line"", ""生产线MS"", 1, """ & CLoginInfo.user_cd & """,""" & CLoginInfo.user_name & """, colsParams,pubInitParams, awaysWhere,500,true);")
        sb.AppendLine("    table.SetColumnsWidth(table.Columns);")

        'sb.AppendLine("    table.SetColumnsWidth(table.Columns);")

        'sb.AppendLine("     $('.upd_date_start').val('" & Now.AddDays(-3).ToString("yyyy-MM-dd") & "');")
        'sb.AppendLine("     $('.upd_date_end').val('" & Now.AddDays(3).ToString("yyyy-MM-dd") & "');")
        'sb.AppendLine("     $('.jqBtnSel').click();")
        sb.AppendLine("    queryHintCallback('query_hint');")
        sb.AppendLine("});")
        sb.AppendLine("</script>")


        ClientScript.RegisterStartupScript(Page.GetType(), "JsLoad", sb.ToString())
    End Sub

    'back
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Context.Items("CLoginInfo") = ViewState("CLoginInfo")
        Server.Transfer("Menu.aspx")
    End Sub
End Class
