<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MsCodeSys.aspx.vb" Inherits="MsCodeSys" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <!--JQUERY-->
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./jquery-ui-1.12.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="./jquery/jquery.cookie.js"></script>
    <link rel="stylesheet" href="./jquery-ui-1.12.1/jquery-ui.css" />

    <%--共通--%>
    <%
        Response.Write("<link href=""./SingleTable.css?randomId=" & PageCom.GetYmdhmsf() & """ rel=""stylesheet"" />")
    %>
    <script charset="utf-8" src="js/jquery.table2excel.js"></script>
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
    <script charset="utf-8" type="text/javascript" src="./js/BASE64.js"></script>
    <script type="text/javascript" src="./SingleTable.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
    <%--自分頁--%>
    <script type="text/javascript" src="js/jquery-ui-timepicker-addon.js"></script>
    <script type="text/javascript" src="js/jquery-ui-timepicker-zh-CN.js"></script>
</head>
<body>
<form id="form1" runat="server">
    <div id="query_hint" class="query_hint">
            <img src="./Image/waitSmall.gif" />正在查询，请稍等...
        </div>
    <header>
        <div class="topdiv_left"></div>
        <div class="topdiv_right">
            <asp:Button ID="btnBack" runat="server" Text="返回" />
        </div>

    </header>
    <article>
    </article>
    <footer>
    </footer>
</form>
</body>
</html>
