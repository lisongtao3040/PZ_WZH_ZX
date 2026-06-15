
Partial Class pt_v_A03_mi_check_result
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
        sb.AppendLine("       生成实际日YMD:{IsEmpty_enable:true,width:100}")
        sb.AppendLine("       ,纳期日:{IsEmpty_enable:true,width:100}")
        sb.AppendLine("       ,cd:{IsEmpty_enable:true,width:150}")
        sb.AppendLine("       ,作番:{IsEmpty_enable:true,width:100}")


        sb.AppendLine("        ,user_cd: {")
        sb.AppendLine("            tooltip: ""用户CD：<br>主键 (不能与寄存数据重复)<br>最大20位 (必须 英数字)""")
        sb.AppendLine("            , sel_row_enable:true")
        sb.AppendLine("            , ms_row_enable:false")
        sb.AppendLine("            , ins_row_enable:true")
        sb.AppendLine("        }")
        sb.AppendLine("        , 部门: {")
        sb.AppendLine("            tooltip: ""部门""")
        sb.AppendLine("            , cType: ""listbox""")
        sb.AppendLine("            , listbox: [")
        sb.AppendLine("                  { text: "" 1 部"", value: ""1"" }")
        sb.AppendLine("                , { text: "" 2 部"", value: ""2"" }")
        sb.AppendLine("                , { text: "" 3 部"", value: ""3"" }")
        sb.AppendLine("                , { text: "" 国内贩卖"", value: ""4"" }")
        sb.AppendLine("            ]")
        sb.AppendLine("        }")

        sb.AppendLine("        , line_cd: {")
        sb.AppendLine("            tooltip: ""组：(必须 英数字)<br>*请注意： 相同组成员只能操作自己组数据""")
        sb.AppendLine("            , reg: /^[a-zA-Z0-9-\%]+$/")
        sb.AppendLine("        }")
        sb.AppendLine("        , authority: {")
        sb.AppendLine("            tooltip: ""权限：(数字,只能 1 或 2 )<br>1:管理员<br>2:普通用户""")
        sb.AppendLine("            , reg: /^[1-2]+$/")
        sb.AppendLine("            , cType: ""listbox""")
        sb.AppendLine("            , listbox: [")
        sb.AppendLine("                { text: ""管理员"", value: ""1"" }")
        sb.AppendLine("                , { text: ""普通用户"", value: ""2"" }")
        sb.AppendLine("            ]")
        sb.AppendLine("        }")


        sb.AppendLine("      , upd_user: {")
        sb.AppendLine("             visible:true")
        sb.AppendLine("            , sel_row_enable:false")
        sb.AppendLine("            , ms_row_enable:false")
        sb.AppendLine("            , ins_row_enable:false")
        sb.AppendLine("        }")

        sb.AppendLine("      , upd_date: {")
        sb.AppendLine("             visible:true")
        sb.AppendLine("            , sel_row_enable:false")
        sb.AppendLine("            , ms_row_enable:false")
        sb.AppendLine("            , ins_row_enable:false")
        sb.AppendLine("        }")

        sb.AppendLine("      , ins_user: {")
        sb.AppendLine("             visible:true")
        sb.AppendLine("            , sel_row_enable:false")
        sb.AppendLine("            , ms_row_enable:false")
        sb.AppendLine("            , ins_row_enable:false")
        sb.AppendLine("        }")

        sb.AppendLine("      , ins_date: {")
        sb.AppendLine("             visible:true")
        sb.AppendLine("            , sel_row_enable:false")
        sb.AppendLine("            , ms_row_enable:false")
        sb.AppendLine("            , ins_row_enable:false")
        sb.AppendLine("        }")

        sb.AppendLine("    };")
        sb.AppendLine("")

        'If CLoginInfo.line_cd = "" Then
        'sb.AppendLine("    var awaysWhere = """";")
        'Else
        '    sb.AppendLine("    var awaysWhere = ""AND 检查者生产线='" & CLoginInfo.line_cd & "'"";")
        'End If
        'sb.AppendLine("    var awaysWhere = ""AND 纳期日 >= '" & now & "'";")
        'sb.AppendLine("    var awaysWhere = ""AND 纳期日='" & Now.ToString("yyyy-MM-dd") & "'"";")

        Dim department_cd As String = ""
        'department_cd = "'" & String.Join("','", CLoginInfo.department_cd.Split("")) & "'"

        Dim myChar As Char
        For Each myChar In CLoginInfo.department_cd
            If department_cd = "" Then
                department_cd = "'" & myChar & "'"
            Else
                department_cd = department_cd & ",'" & myChar & "'"
            End If
        Next
        sb.AppendLine("    var awaysWhere = "" AND 部门 in(" & department_cd & ",'')"";")




        sb.AppendLine("    var table = new dbTable(""article"", "".jqPanelTable"", "".jqPanelNumOfPages"", ""v_A03_mi_check_result"", ""未检查一览"", 1, """ & CLoginInfo.user_cd & """,""" & CLoginInfo.user_name & """, colsParams,pubInitParams, awaysWhere,500,false);")
        sb.AppendLine("    table.SetColumnsWidth(table.Columns);")
        sb.AppendLine("     $('.生成实际日YMD_start').val('" & Now.AddDays(-1).ToString("yyyy-MM-dd") & "');")
        sb.AppendLine("     $('.生成实际日YMD_end').val('" & Now.AddDays(1).ToString("yyyy-MM-dd") & "');")

        'sb.AppendLine("     $('.upd_date_start').val('" & Now.AddDays(-3).ToString("yyyy-MM-dd") & "');")
        'sb.AppendLine("     $('.upd_date_end').val('" & Now.AddDays(3).ToString("yyyy-MM-dd") & "');")
        sb.AppendLine("     $('.jqBtnSel').click();")

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
