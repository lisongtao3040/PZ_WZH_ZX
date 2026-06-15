<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Header.ascx.vb" Inherits="UserControls_Header" %>

        <div id="dialog" title="Information" style="display: none; z-index: 41111;">
            <p id="dialogMsg"></p>
        </div>
<header>

    <div style="float: left; text-align: center; font-size: 40px; width: 100%; position: fixed; top: 0px;">
        <asp:Label ID="lblTitle" runat="server" Text="ログイン"></asp:Label>
    </div>

    <div style="float: right;">
        <table style="margin-right: 0px; width: 290px; float: right; position: relative; z-index: 2;">
            <tr>
                <td style="text-align: right;">
                    <asp:Label ID="lblUserCd" runat="server" Text=""></asp:Label>
                    
                    (<asp:Label ID="lblUserName" runat="server" Text="未登录"></asp:Label>)
        
                    <br />
                    <asp:Label ID="lblLineCd" runat="server" Text=""></asp:Label>
                    (<asp:Label ID="lblLineName" runat="server" Text=""></asp:Label>)
                </td>
                <td style="text-align: right">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Image/UserIcon.png" Style="height: 46px; width: 46px;" />
                </td>
            </tr>
        </table>
    </div>

    <div id="cover" style="position: absolute; width: 100%; height: 100%; left: 0px; top: 0px; z-index: 2; opacity: 0.5; background-color: #ddd; text-align: center;">
        <img alt="" src="./Image/waitSmall.gif" style="margin: 45px auto 0 auto" />
    </div>

    <div id="dialog_pd" style="position: absolute; width: 100%; height: 100%; left: 0px; top: 0px; display: none; opacity: 0.3; background-color: #000000">

    </div>


</header>


