<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TP_list.aspx.vb" Inherits="TP_list" %>

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
    <link rel="stylesheet" href="./TP_list.css" />
    <script type="text/javascript" src="./TP_list.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <uc1:Header runat="server" ID="UserHeader" title="托盘一览" />
        <article>
            <asp:Button ID="btnMs" runat="server" Text="Button" Style="display: none" /><asp:HiddenField ID="hidTpNo" runat="server" />
            <asp:Button ID="btnBack" runat="server" Text="返回" Width="140px" Style="float: right" />
            <asp:GridView ID="gv" runat="server" Font-Size="40px" AutoGenerateColumns="false" Style="margin: 0px auto 0px auto">
                <Columns>
                    <asp:TemplateField HeaderText="托盘号">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbTp_no" runat="server"><%#Eval("托盘号").ToString%></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="130" VerticalAlign="Middle" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="最早出荷日期">
                        <ItemTemplate>
                            <%#Ymd(Eval("最早出荷日期").ToString)%>
                        </ItemTemplate>
                        <ItemStyle Width="290" VerticalAlign="Middle" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="制品捆包数">
                        <ItemTemplate>
                            <%#Eval("制品捆包数").ToString%>
                        </ItemTemplate>
                        <ItemStyle Width="230" VerticalAlign="Middle" HorizontalAlign="Right" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle BackColor="#99CCFF" />
            </asp:GridView>

        </article>
    </form>
</body>
</html>
