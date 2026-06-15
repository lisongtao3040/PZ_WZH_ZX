

<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PicNotUpdataChk.aspx.vb" Inherits="PicNotUpdataChk" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        模版利用了，但是没有导入的图片（别忘了是点导入按钮，才算导入）
        <asp:GridView ID="GridView1" runat="server"></asp:GridView>
    </div>
    </form>
</body>
</html>
