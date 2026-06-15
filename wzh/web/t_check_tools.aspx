<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_check_tools.aspx.vb" Inherits="t_check_tools" %>


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
    <link rel="stylesheet" href="./t_check_tools.css" />
    <script type="text/javascript" src="./t_check_tools.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <uc1:Header runat="server" ID="UserHeader" title="检查一览" />

        <div style="height: 50px; float: left; padding-left: 5px; width: 790px;margin-top:70px;">


            <asp:Label ID="tbxCd" runat="server" Text="tbxCd"></asp:Label>
            <asp:Label ID="tbxNo" runat="server" Text="tbxCd"></asp:Label>
            <asp:Label ID="tbxCk_id" runat="server" Text="tbxCd" Style="color: #727610; font-size: 12px;"></asp:Label>

            <%--<asp:LinkButton ID="lbSCX" runat="server">查看生产性</asp:LinkButton>--%>
            <hr />

        </div>
        <article>
            <div style="text-align: right;">
                <asp:Button ID="btnBack" runat="server" Text="返回" Width="200" />

            </div>
            <hr />
            <div style="text-align: center;">
                <input type="tel" id="tbxScan" placeholder="扫描" autocomplete="off" style="width:400px;" />
            </div>
            <hr />


            <asp:GridView ID="gvTools" runat="server" AutoGenerateColumns="False"
                Width="100%" CssClass="gv">
                <Columns>
                    <asp:TemplateField HeaderText="治具编号">
                        <ItemTemplate>
                            <%-- <%#GetIdAndNo(Eval("ck_id").ToString, Eval("cd").ToString, Eval("no").ToString, Eval("status").ToString)%>--%>
                            <input type="text" class="barcode" placeholder="<%#Eval("tools_ma").ToString.Trim.ToUpper() %>" readonly="readonly" style="background-color: #cdcdcd; width: 400px;" />

                        </ItemTemplate>
                        <ItemStyle Width="160" HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="治具描述">
                        <ItemTemplate>
                            <%#Eval("remarks").ToString%>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="tbl_ms_title_new" />
            </asp:GridView>

            <asp:Button ID="btnUpdToolsFlg" runat="server" Text="更新 治具扫描状态" Style="display: none;" />

        </article>
    </form>
</body>
</html>
