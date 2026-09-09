<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge,chrome=1" />
    <meta http-equiv="pragma" content="no-cache" />
    <title>登录</title>

    <%--样式--%>
    <link href="./Default.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <%--脚本--%>
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
    <script type="text/javascript" src="./Default.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-page">
            <div class="login-card">
                <h1 class="login-title">用户登录</h1>

                <div class="login-body">
                    <label for="tbxUserCd">用户CD</label>
                    <asp:TextBox ID="tbxUserCd" runat="server" MaxLength="20" title="用户CD（半角英数字）" Text=""></asp:TextBox>

                    <label for="tbxPassword">密码</label>
                    <asp:TextBox ID="tbxPassword" runat="server" MaxLength="20" TextMode="Password" title="密码（半角英数字）" Text="1"></asp:TextBox>
                </div>

                <div class="login-actions">
                    <input id="Reset1" type="reset" value="重置" class="login-btn" />
                    <asp:Button ID="btnLogin" runat="server" Text="登录" CssClass="login-btn login-submit" OnClientClick="return CheckLoginInput();" />
                </div>
                <div>
                    <br />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
