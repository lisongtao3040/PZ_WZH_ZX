<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_FullTrayListPZ.aspx.vb" Inherits="t_FullTrayListPZ" %>

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
    <link rel="stylesheet" href="./t_FullTrayListPZ.aspx.css?randomId=<%=PageCom.GetYmdhmsf()%>" />
    <script type="text/javascript">var ftlIsPostBack = <%=IsPostBack.ToString().ToLower()%>;</script>
    <script type="text/javascript" src="./t_FullTrayListPZ.aspx.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">

        <asp:HiddenField ID="hid_err_msg" runat="server" ClientIDMode="Static" />

        <div class="ftl-top-panel">

            <asp:Button ID="btnBack" runat="server" Text="返回" CssClass="ftl-btn" />
            <input type="button" id="btnSearch" value="检索" class="ftl-btn" />
            <input type="button" id="btnToggleView" value="切换" class="ftl-btn ftl-btn-toggle" />

            <%-- CD 筛选：点击输入框弹出虚拟键盘 --%>
            <span class="ftl-filter-bar">
                <input type="text" id="txtCdFilter" class="ftl-cd-input" readonly placeholder="CD筛选" title="点击弹出键盘输入CD" maxlength="30" />
                <input type="button" id="btnCdClear" value="清空" class="ftl-btn" />
            </span>

            <asp:Label ID="lblUserCd" runat="server" Text="Label" ClientIDMode="Static"></asp:Label>
            <asp:Label ID="lblUserName" runat="server" Text="Label" ClientIDMode="Static"></asp:Label>
            <%-- 部門コード（例：（2））：ユーザー名の後ろに表示 --%>
            <asp:Label ID="lblUserDept" runat="server" Text=""></asp:Label>
            <span id="lblStatus" class="ftl-status"></span>
               
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
