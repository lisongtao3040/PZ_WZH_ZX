
Partial Class TB_CompleteDataNEW
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

        sb.AppendLine("        department_cd: {")
        sb.AppendLine("            tooltip: ""部门""")
        sb.AppendLine("            , cType: ""listbox""")
        sb.AppendLine("            , listbox: [")
        sb.AppendLine("                  { text: "" 1 部"", value: ""1"" }")
        sb.AppendLine("                , { text: "" 2 部"", value: ""2"" }")
        sb.AppendLine("                , { text: "" 3 部"", value: ""3"" }")
        sb.AppendLine("                , { text: "" 国内贩卖"", value: ""4"" }")
        sb.AppendLine("            ]")
        sb.AppendLine("            ,width:120")
        sb.AppendLine("        }")

        sb.AppendLine("        , barcode: {")
        sb.AppendLine("            tooltip: ""治具上的一维码内容""")
        sb.AppendLine("            , reg: /^[a-zA-Z0-9-\%]+$/")
        sb.AppendLine("            , width: 200")
        sb.AppendLine("        }")


        sb.AppendLine("        , remarks: {")
        sb.AppendLine("            tooltip: ""治具的描述""")
        sb.AppendLine("            , width: 400")
        sb.AppendLine("        }")


        sb.AppendLine("    };")
        sb.AppendLine("")
        sb.AppendLine("    var awaysWhere = """";")
        sb.AppendLine("    var table = new dbTable(""article"", "".jqPanelTable"", "".jqPanelNumOfPages"", ""TB_SetAllCheck"", ""全数检查MS"", 1, """ & CLoginInfo.user_cd & """,""" & CLoginInfo.user_name & """, colsParams,pubInitParams, awaysWhere,500);")
        sb.AppendLine("    table.SetColumnsWidth(table.Columns);")

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
