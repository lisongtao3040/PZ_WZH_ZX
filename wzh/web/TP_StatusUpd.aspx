<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TP_StatusUpd.aspx.vb" Inherits="TP_StatusUpd" %>

<!DOCTYPE html>
<%@ Register Src="~/UserControls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/UserControls/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>

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
    <link href="./App_Themes/Css/Main.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>

    <%--自分頁--%>
    <link rel="stylesheet" href="./TP_StatusUpd.css" />
    <script type="text/javascript" src="./TP_StatusUpd.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <uc1:Header runat="server" ID="UserHeader" title="" />


        <article>
            <asp:Label ID="lblMsg" runat="server" Text="" Font-Size="30px" ForeColor="Red"></asp:Label>
            <asp:Label ID="lblMsgOK" runat="server" Text="" Font-Size="30px" ForeColor="blue"></asp:Label>
            <br />
            方式1：扫托盘码&nbsp;&nbsp;托盘No： &nbsp;&nbsp;<asp:TextBox ID="tbxScan" runat="server" Text="      " Font-Size="40px" ForeColor="blue" Width="100px" Height="40px"></asp:TextBox>

            <asp:Button ID="btnTpChkList" runat="server" Text="托盘检查一览" Width="210px" />
            <%--<input type="button" id="btnUpd" value="更新" style="width: 100px;" />--%>
            <asp:Button ID="btnRefesh" runat="server" Text="刷新" Width="110px" Visible="false" />
            <asp:Button ID="btnRe" runat="server" Text="品检查完了迁移" Visible="false"  />
            <asp:Button ID="btnBack" runat="server" Text="返回" Width="140px" />
            <br />
            <hr />
            方式2：以下点击对应的托盘No，执行更新
            <asp:Panel ID="divBtns" runat="server"></asp:Panel>
        </article>
    </form>
</body>
</html>
