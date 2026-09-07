
Partial Class pt_v_A05_check_scx
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
        sb.AppendLine("       cd:{IsEmpty_enable:true,width:150}")
        sb.AppendLine("       ,作番:{IsEmpty_enable:true,width:100}")


        sb.AppendLine("        ,user_cd: {")
        sb.AppendLine("            tooltip: ""用户CD：<br>主键 (不能与寄存数据重复)<br>最大20位 (必须 英数字)""")
        sb.AppendLine("            , sel_row_enable:true")
        sb.AppendLine("            , ms_row_enable:false")
        sb.AppendLine("            , ins_row_enable:true")
        sb.AppendLine("        }")
        sb.AppendLine("        , user_name: {")
        sb.AppendLine("            tooltip: ""用户名汉字：最大20位""")
        sb.AppendLine("")
        sb.AppendLine("        }")
        sb.AppendLine("        , password: {")
        sb.AppendLine("            tooltip: ""密码：(必须 英数字)""")
        sb.AppendLine("            , reg: /^[a-zA-Z0-9\-\%]+$/")
        sb.AppendLine("            , IsEmpty_enable:true")
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

        Dim user_cd As String
        Dim user_name As String
        Dim user_line_cd As String

        If CLoginInfo Is Nothing Then
            user_cd = Request.QueryString("user_cd")
            user_name = Request.QueryString("user_name")
        Else
            user_cd = CLoginInfo.user_cd
            user_name = CLoginInfo.user_name
            user_line_cd = CLoginInfo.line_cd
        End If



        Dim d As Date = Now
        'd = CDate("2023-07-06 00:00:01")
        If Request.QueryString("kbn") IsNot Nothing Then
            sb.AppendLine("    var awaysWhere = ""AND 检查者='" & user_cd & "(" & user_name & ")" & "'"";")

            sb.AppendLine("    var table = new dbTable(""article"", "".jqPanelTable"", "".jqPanelNumOfPages"", ""v_A05_check_scx_one"", ""检查生产性报表"", 1, """ & user_cd & """,""" & user_name & """, colsParams,pubInitParams, awaysWhere,500,false);")
            sb.AppendLine("    table.SetColumnsWidth(table.Columns);")

            '半夜12:00~早8:00  算前一天

            'Dim s As String
            Dim t As String
            If CInt(d.ToString("HH")) < 8 Then
                's = d.AddDays(-1).ToString("yyyy-MM-dd")
                t = d.AddDays(-1).ToString("yyyy-MM-dd")
            Else
                t = d.ToString("yyyy-MM-dd")
            End If

            sb.AppendLine("     $('.检查日期_start').val('" & t & "');")
            sb.AppendLine("     $('.检查日期_end').val('" & t & "');")

            sb.AppendLine("     $('.jqBtnSel').click();")
            sb.AppendLine("    changeColor(table);")


            '检查日期
            Me.btnBack.Visible = False
        Else
            'Dim line_cd As String
            'For Each myChar In CLoginInfo.line_cd.Split(",")
            '    If line_cd = "" Then
            '        line_cd = "'" & myChar & "'"
            '    Else
            '        line_cd = line_cd & ",'" & myChar & "'"
            '    End If
            'Next

            ''sb.AppendLine("    var awaysWhere = """";")
            'sb.AppendLine("    var awaysWhere = ""AND 检查者生产线 in(" & line_cd & ")"";")

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

            '半夜12:00~早8:00  算前一天

            Dim t As String
            If CInt(d.ToString("HH")) < 8 Then
                t = d.AddDays(-1).ToString("yyyy-MM-dd")
            Else
                t = d.ToString("yyyy-MM-dd")
            End If

            sb.AppendLine("  var awaysWhere = "" AND 部门 in(" & department_cd & ",'')"";")

            sb.AppendLine("    var table = new dbTable(""article"", "".jqPanelTable"", "".jqPanelNumOfPages"", ""v_A05_check_scx"", ""检查生产性报表"", 1, """ & user_cd & """,""" & user_name & """, colsParams,pubInitParams, awaysWhere,500,false);")
            sb.AppendLine("    table.SetColumnsWidth(table.Columns);")
            sb.AppendLine("     $('.检查日期_start').val('" & t & "');")
            sb.AppendLine("     $('.检查日期_end').val('" & t & "');")
            sb.AppendLine("     $('.jqBtnSel').click();")
            sb.AppendLine("    changeColor(table);")
        End If

        'If CLoginInfo.line_cd = "" Then

        'Else
        '    sb.AppendLine("    var awaysWhere = ""AND 检查者生产线='" & CLoginInfo.line_cd & "'"";")
        'End If
        'sb.AppendLine("    var awaysWhere = ""AND 纳期日 >= '" & now & "'";")
        'sb.AppendLine("    var awaysWhere = ""AND 纳期日='" & Now.ToString("yyyy-MM-dd") & "'"";")



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
