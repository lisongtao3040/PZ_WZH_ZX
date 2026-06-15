<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InputByHand.aspx.vb" Inherits="InputByHand" %>

<%@ Register Src="~/UserControls/Header.ascx" TagPrefix="uc1" TagName="Header" %>

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
    <link href="./App_Themes/Css/Main.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>

    <%--自分頁--%>
    <link rel="stylesheet" href="./t_check_list.aspx.css" />
    <script type="text/javascript" src="./t_check_list.aspx.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <uc1:Header runat="server" ID="UserHeader" title="手入力" />

        <article>
            <asp:Button ID="btnBack" runat="server" Text="返回" />
            <hr />
            <asp:TextBox ID="tbxCd" runat="server" placeholder="商品CD" Text="" Width="300" Font-Size="26px" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="tbxNo" runat="server" placeholder="作番" Text="" Width="160" Font-Size="26px" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="tbxSuu" runat="server" placeholder="数量" Text="" Width="160" Font-Size="26px" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="tbxBumen" runat="server" placeholder="部门" Text="" Width="160" Font-Size="26px" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:TextBox ID="tbxLine" runat="server" placeholder="生产线" Text="" Width="160" Font-Size="26px" AutoCompleteType="Disabled"></asp:TextBox>
            <asp:RadioButton ID="rbtnOk" runat="server" Text="  OK  " GroupName="jieguoAA" CssClass="JQ_RBTN" Checked="true" ForeColor="Red" Font-Size="26px" />
            <asp:RadioButton ID="rbtnNG" runat="server" Text="  NG  " GroupName="jieguoAA" CssClass="JQ_RBTN" Font-Size="26px" />

            <asp:Button ID="btnIns" runat="server" Text="登录" Width="140" />
        </article>
    </form>
</body>
</html>
