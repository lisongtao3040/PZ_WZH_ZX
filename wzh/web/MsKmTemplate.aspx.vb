
Partial Class MsKmTemplate
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
        '共通的画面设定
        sb.AppendLine("    var pubInitParams = {")
        sb.AppendLine("        fixKeyCols:false,")  '固定列
        sb.AppendLine("    };")


        sb.AppendLine("    var colsParams = {")

        sb.AppendLine("       id: {")
        sb.AppendLine("             visible:true")
        sb.AppendLine("            , IsEmpty_enable:true")
        sb.AppendLine("            , sel_row_enable:false")
        sb.AppendLine("            , ms_row_enable:false")
        sb.AppendLine("            , ins_row_enable:false")
        sb.AppendLine("        }")

        sb.AppendLine("       ,F1:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F2:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F3:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F4:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F5:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F6:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F7:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F8:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F9:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F10:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F11:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F12:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F13:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F14:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F15:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F16:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F17:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F18:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F19:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,F20:{IsEmpty_enable:true,width:20}")
        sb.AppendLine("       ,length:{width:30}")



        sb.AppendLine("        , department_cd: {")
        sb.AppendLine("            tooltip: ""部门""")
        sb.AppendLine("            , cType: ""listbox""")
        sb.AppendLine("            , listbox: [")
        sb.AppendLine("                  { text: "" 1 部"", value: ""1"" }")
        sb.AppendLine("                , { text: "" 2 部"", value: ""2"" }")
        sb.AppendLine("                , { text: "" 3 部"", value: ""3"" }")
        sb.AppendLine("                , { text: "" 国内贩卖"", value: ""4"" }")
        sb.AppendLine("            ]")
        sb.AppendLine("            ,width:100")

        sb.AppendLine("            ,changeAction:'that.ClearCell(cellTd, 26)'") '清空 【コード体系】

        sb.AppendLine("        }")
        'コード体系
        sb.AppendLine("        ,sys_name: {")

        sb.AppendLine("            tooltip: ""体系名称：<br>主键 (不能与寄存数据重复)<br>最大20位 (必须 英数字)""")
        sb.AppendLine("            , cType: ""listbox""")
        sb.AppendLine("            , listbox: []")
        'sb.AppendLine("            , sel_row_enable:true")
        'sb.AppendLine("            , ms_row_enable:false")
        'sb.AppendLine("            , ins_row_enable:true")
        sb.AppendLine("            ,width:140")

        sb.AppendLine("            ,ListDataSql:"" select distinct sys_id as id,sys_name as name,department_cd AS department_cd from m_code_sys """)
        sb.AppendLine("            ,listDataFilter:{key:'department_cd',value:'that.GetTdValueByIdx(cellTd,25)'}")


        'sb.AppendLine("            , listboxDataSourceParamChk:{value:""(that.GetTdValueByIdx(cellTd,25)!='')"",msg:""请输入式样分类""}")
        'sb.AppendLine("            , listboxDataSourceParamChkNgFocus:""(that.GetTdValueByIdx(cellTd,25))""")
        'sb.AppendLine("            , listboxDataSource:  ""('select distinct sys_id as id,sys_name as name from m_code_sys where department_cd = {0}').replace('{0}',that.GetTdValueByIdx(cellTd,25))""")
        'changeAction

        sb.AppendLine("        }")

        sb.AppendLine("       ,jizong_name:{width:200}")
        sb.AppendLine("       ,kind_name:{width:200}")
        sb.AppendLine("       ,pic_name:{width:200}")

        sb.AppendLine("       ,chk_km_name:{width:200}")
        sb.AppendLine("       ,chk_fmt:{IsEmpty_enable:true,width:500}")

        '检查方式
        sb.AppendLine("        , chk_fs: {")
        sb.AppendLine("            tooltip: """"")
        sb.AppendLine("            , cType: ""listbox""")
        sb.AppendLine("            , listbox: [")
        sb.AppendLine("                  { text: ""扫描"", value: ""1"" }")
        sb.AppendLine("                , { text: ""输入"", value: ""2"" }")
        sb.AppendLine("                , { text: ""目视"", value: ""9"" }")
        sb.AppendLine("            ]")
        sb.AppendLine("            ,width:60")
        sb.AppendLine("        }")
        'chk_fs

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
        sb.AppendLine("    var awaysWhere = """";")
        sb.AppendLine("    var table = new dbTable(""article"", "".jqPanelTable"", "".jqPanelNumOfPages"", ""m_km_template"", ""检查项目模板"", 1, """ & CLoginInfo.user_cd & """,""" & CLoginInfo.user_name & """, colsParams,pubInitParams, awaysWhere,100);")
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
