<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_FullTrayList.aspx.vb" Inherits="t_FullTrayList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <title></title>

    <!--JQUERY-->
    <script type="text/javascript" src="./Jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./jquery-ui-1.12.1/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="./jquery-ui-1.12.1/jquery-ui.css" />

    <%--共通--%>
    <link href="./App_Themes/Css/Main.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>

    <%--自分頁--%>
    <link rel="stylesheet" href="./t_FullTrayList.aspx.css?randomId=<%=PageCom.GetYmdhmsf()%>" />
    <script type="text/javascript">var ftlIsPostBack = <%=IsPostBack.ToString().ToLower()%>;</script>
    <script type="text/javascript" src="./t_FullTrayList.aspx.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">

        <asp:HiddenField ID="hid_err_msg" runat="server" ClientIDMode="Static" />

        <div id="ftlLoadingOverlay" style="display:none;">
            <div class="ftl-loading-box">加载中...</div>
        </div>
        <div style="position:absolute;float:right;right:2px;">    <asp:Label ID="lblMsg" runat="server" Text="" ForeColor="red"></asp:Label></div>

        <div class="ftl-top-panel">

            <asp:Button ID="btnBack" runat="server" Text="返回" CssClass="ftl-btn" />
            <input type="button" id="btnSearch" value="检索" class="ftl-btn" />
            <input type="button" id="btnToggleView" value="切换" class="ftl-btn ftl-btn-toggle" />
    <asp:Label ID="lblUserCd" runat="server" Text="Label"></asp:Label>
                <asp:Label ID="lblUserName" runat="server" Text="Label"></asp:Label>
            <span id="lblStatus" class="ftl-status"></span>
               
        </div>

        <div id="ftlMsgArea" style="display:none;"></div>

        <%-- 検査ボタン用：隠しフィールド＋サーバーボタン --%>
        <asp:HiddenField ID="hid_chk_cd" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hid_chk_no" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hid_chk_tpno" runat="server" ClientIDMode="Static" />
        <asp:Button ID="btnChkServer" runat="server" Text="" ClientIDMode="Static" Style="display:none;" />
        <asp:Button ID="btnAutoOkServer" runat="server" Text="" ClientIDMode="Static" Style="display:none;" />
        <br />
        <%-- 自动OK 確認ダイアログ --%>
        <div id="autoOkConfirmDialog" style="display:none;">
            <p style="font-size:15px;margin:12px 0;">是否真的要自动OK？</p>
        </div>

        <%-- AGV 呼叫確認ダイアログ --%>
        <div id="agvCallDialog" style="display:none;">
            <table class="agv-param-table">
                <thead>
                    <tr><th>参数名</th><th>值</th></tr>
                </thead>
                <tbody>
                    <tr><td>callIEQ（呼叫设备编号）</td><td id="dp_callIEQ"></td></tr>
                    <tr><td>callTaskTypeID（任务类型ID）</td><td>98</td></tr>
                    <tr><td>trolleyNo（托盘号）</td><td id="dp_trolleyNo"></td></tr>
                    <tr><td>stationNo1（起点站点）</td><td id="dp_stationNo1"></td></tr>
                    <tr><td>stationNo2（目标站点）</td><td>（空）</td></tr>
                    <tr><td>dueTime（任务时限）</td><td>（空）</td></tr>
                    <tr><td>opUserID（操作人ID）</td><td id="dp_opUserID"></td></tr>
                    <tr><td>opUserName（操作人名称）</td><td id="dp_opUserName"></td></tr>
                    <tr><td>TrolleyType（托盘类型）</td><td>（空）</td></tr>
                </tbody>
            </table>
            <div class="agv-dlg-footer">
                <button type="button" class="ftl-dlg-exec" onclick="$('#agvCallDialog').dialog('close'); executeAgv();" style="width:100px;">执行</button>
                <button type="button" onclick="$('#agvCallDialog').dialog('close');">取消</button>
            </div>
        </div>

        <article>
            <%-- 行视图 --%>
            <div id="rowView">
                <div id="rowViewBody" class="ftl-list-body"></div>
            </div>

            <%-- 面板视图 --%>
            <div id="panelView" style="display:none;">
                <div id="panelViewBody" class="ftl-panel-body"></div>
            </div>
        </article>

    </form>
</body>
</html>
