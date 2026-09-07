<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_check_test.aspx.vb" Inherits="t_check_test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       
        CD：<asp:TextBox ID="tbxCd" runat="server" Text="CHADDLWXXX12345"></asp:TextBox>
        作番：<asp:TextBox ID="tbxNo" runat="server" Text="text make no"></asp:TextBox>
        <asp:Button ID="btnSelKm" runat="server" Text="检索" />
        <asp:GridView ID="gvKm" runat="server">
            

        </asp:GridView>
    </div>
    </form>
</body>
</html>
