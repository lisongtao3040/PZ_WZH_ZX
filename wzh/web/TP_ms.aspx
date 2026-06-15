<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TP_ms.aspx.vb" Inherits="TP_ms" %>

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
        <div>
            <uc1:Header runat="server" ID="UserHeader" title="托盘一览" />
            <article>
                <asp:Button ID="btnBack" runat="server" Text="返回" Width="140px" Style="float: right" OnClientClick="history.back(-1);return false;" />
                <asp:GridView ID="gv" runat="server" Font-Size="24px" AutoGenerateColumns="false" Width="1000" Style="margin: 0px auto 0px auto">
                    <Columns>
                        <asp:TemplateField HeaderText="工单号">
                            <ItemTemplate>
                                <%#Eval("工单号").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="100" VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="商品CD">
                            <ItemTemplate>
                                <%#Eval("商品CD").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="130" VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="捆包数">
                            <ItemTemplate>
                                <%#Eval("捆包数").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="100" VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="经销商">
                            <ItemTemplate>
                                <%#Eval("经销商").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="230" VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="地址">
                            <ItemTemplate>
                                <%#Eval("地址代码").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="50" VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="预定出荷日">
                            <ItemTemplate>
                                <%#Eval("预定出荷日").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="130" VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="特注书号">
                            <ItemTemplate>
                                <%#Eval("特注书号").ToString%>
                            </ItemTemplate>
                            <ItemStyle  VerticalAlign="Middle" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle BackColor="#99CCFF" />
                </asp:GridView>
            </article>
        </div>


    </form>
</body>
</html>
