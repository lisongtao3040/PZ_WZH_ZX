<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ImportPics.aspx.vb" Inherits="ImportPics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnImports" runat="server" Text="导入图片s" />
        </div>
        <hr />
        <asp:TextBox ID="tbxName" runat="server" Width="412px"></asp:TextBox>
        <asp:Button ID="btnSel" runat="server" Text="检索" />
        <hr />


        <asp:GridView ID="GvPic"
            runat="server"
            CssClass="chk_ms"
            AutoGenerateColumns="false"
            ShowHeader="false"
            Font-Size="24px"
            CellPadding="0"
            CellSpacing="0"
            Width="100%"
            Style="font-size: 24px;">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Image ID="Image1" runat="server" Width="700" />
                    </ItemTemplate>
                    <ItemStyle Width="130px" HorizontalAlign="Left" CssClass="" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        图片名：<%#Eval("pic_name").ToString %>
                        <br />
                        <hr />
                        更新时间：<%# Left(Eval("pic_upd_time").ToString, 4) & "-" & Eval("pic_upd_time").ToString.Substring(4, 2) _
                                            & "-" & Eval("pic_upd_time").ToString.Substring(6, 2) _
                                            & " " & Eval("pic_upd_time").ToString.Substring(8, 2) _
                                            & ":" & Eval("pic_upd_time").ToString.Substring(10, 2) _
                                            & ":" & Eval("pic_upd_time").ToString.Substring(12, 2)
                        %>
                           <hr />

                        编号：<%# Eval("pic_upd_time").ToString  %>
                    </ItemTemplate>
                    <ItemStyle Width="230px" HorizontalAlign="Left" CssClass="" />
                </asp:TemplateField>

            </Columns>
        </asp:GridView>

    </form>
</body>
</html>
