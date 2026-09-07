<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="UserControls" Namespace="UserControls.UserControls" TagPrefix="cc2" %>

<%@ Register Src="~/UserControls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/UserControls/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge,chrome=1" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <!--JQUERY-->
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./jquery-ui-1.12.1/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="./jquery-ui-1.12.1/jquery-ui.css" />

    <%--共通--%>
    <link href="./App_Themes/Css/Main.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>

    <%--自分頁--%>
    <script type="text/javascript" src="./Default.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <uc1:Header runat="server" ID="UserHeader" title="登录" />
        <article>

            <div style="overflow: auto;">

                <div style="width: 400px; height: 250px; background-color: #fff; margin: 100px auto 0 auto; padding: 50px;">

                    <%--   <asp:TextBox ID="tbxUserCd" runat="server" CssClass="jqIptChk" chkFmt="must english_number" chkName="用户CD" MaxLength="20" Style="margin-top: 0px; width: 380px; ime-mode: disabled" placeholder="ユーザーコード" title="用户CD（半角英数字）">  </asp:TextBox>
                    --%>
                    <cc2:Ilike_TextBox ID="tbxUserCd" runat="server" InputName="用户CD" InputType="EnglishNumber" MustInputType="IsTrue"
                        MaxLength="20" Style="margin-top: 0px; width: 380px; ime-mode: disabled" placeholder="用户CD" title="用户CD（半角英数字）" Text=""></cc2:Ilike_TextBox>
                    <a style="position: static; margin-left: -40px; font-size: 22px; vertical-align: middle"></a>


                    <%--<asp:TextBox ID="tbxPassword" runat="server" CssClass="jqIptChk" chkFmt="must english_number" chkName="密码" MaxLength="20" Style="margin-top: 40px; width: 380px;" placeholder="パスワード" TextMode="Password" title="密码（半角英数字）"></asp:TextBox>--%>
                    <cc2:Ilike_TextBox ID="tbxPassword" runat="server" InputName="密码" InputType="EnglishNumber" MustInputType="IsTrue"
                        MaxLength="20" Style="margin-top: 40px; width: 380px;" placeholder="密码" TextMode="Password" title="密码（半角英数字）" Text="1"></cc2:Ilike_TextBox>

                    <a style="position: static; margin-left: -40px; font-size: 22px; vertical-align: middle"></a>
                    <input id="Reset1" type="reset" value="Reset" class="button" style="margin-top: 40px; width: 160px; float: left;" />
                    <asp:Button ID="btnLogin" runat="server" Text="登录" CssClass="button" Style="margin-top: 40px; width: 160px; float: right;" OnClientClick="return CheckAllInput();" />
                </div>
            </div>
        </article>
        <uc1:Footer runat="server" ID="UserFooter" />
    </form>
</body>
</html>
