<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_BuliangList.aspx.vb" Inherits="t_BuliangList" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./jquery-ui-1.12.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="./jquery/jquery.cookie.js"></script>
    <link rel="stylesheet" href="./jquery-ui-1.12.1/jquery-ui.css" />

    <%--共通--%>
    <link href="./App_Themes/Css/Main.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
    <link rel="stylesheet" href="./t_BuliangList.css?randomId=1" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="float: left; text-align: center; font-size: 40px; width: 100%; position: fixed; top: 0px; background-color: #c7eff3">
            <span id="UserHeader_lblTitle">不良一览</span>
        </div>

        <div style="float: right;">
            <table style="margin-right: 0px; width: 290px; float: right; position: relative; z-index: 2;">
                <tr>
                    <td style="text-align: right;">
                        <span id="UserHeader_lblUserCd"></span>

                        <span id="UserHeader_lblUserName"></span>

                        <br />
                        <span id="UserHeader_lblLineCd"></span>
                        <span id="UserHeader_lblLineName"></span>
                    </td>
                    <td style="text-align: right">
                        <img id="UserHeader_Image1" src="Image/UserIcon.png" style="height: 46px; width: 46px;" />
                    </td>
                </tr>
            </table>
        </div>

        <article>

            <asp:TextBox ID="tbxCd" runat="server" placeholder="商品CD" Text="" Width="300" Font-Size="26px" AutoCompleteType="Disabled"></asp:TextBox>


            <asp:TextBox ID="tbxNo" runat="server" placeholder="作番" Text="" Width="160" Font-Size="26px" AutoCompleteType="Disabled"></asp:TextBox>
            <%--              
<input runat ="server"  type="tel"  id="tbxCd" placeholder="商品CD" Text="CHFDEDMAAAAXXJX" Width="300" Font-Size="26px" />
<input runat ="server" type="tel" id="tbxNo"  placeholder="作番" Text="9006331505" Width="160" Font-Size="26px"  />
            --%>

            <asp:DropDownList ID="ddlHiinai" runat="server" Width="100px" Style="font-size: 26px;">
            </asp:DropDownList>
            日

            <asp:Button ID="btnSel" runat="server" Text="检索" />
            <asp:Button ID="btnBack" runat="server" Text="关闭" OnClientClick="window.close();return false;" />

            <table class="gvtitle" cellspacing="0" style="width: 820px;">
                <tr class="">
                    <td class="c1">线<br />
                        CD<br />
                        作番
                    </td>
                    <td class="c2">数量<br />
                        预定日
                    </td>
                    <td class="c3">结果<br />
                        状态
                    </td>
                    <td class="c4">特注号<br />
                        订单号/序号</td>
                    <td class="c5">开始<br />
                        结束时间<br />
                        检查员
                    </td>
                    <td class="c6">不良内容
                    </td>
                    <%--<td style="width: 50px">欠品</td>--%>
                    <td class="c7">向先、经销商</td>

                </tr>
            </table>
            <div style="width: 840px; overflow: auto; height: 1000px;">
                <asp:GridView ID="gvLastCheckResultMS" runat="server" AutoGenerateColumns="False" Width="820px" CssClass="gv" ShowHeader="false">
                    <Columns>
                        <asp:TemplateField HeaderText="ID<br />作番">
                            <ItemTemplate>
                                <%#Eval("ck_id").ToString%><br>
                                <%#Eval("line_cd").ToString%><br>

                                <%#Eval("cd").ToString%><br>
                                <%#Eval("no").ToString%>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="数量<br />预定检查日">
                            <ItemTemplate>
                                <%#Eval("suu").ToString%><br />
                                <%#Right(Eval("yotei_chk_date").ToString, 8)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="结果<br />状态">
                            <ItemTemplate>
                                <%#GetResultEle(Eval("ck_id").ToString, Eval("result").ToString, Eval("status").ToString)%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="特注号<br />订单号<br />序号">
                            <ItemTemplate>
                                <%#Left(Eval("specialBookNo").ToString, 14)%><br />
                                <%#Left(Eval("sapOderNo").ToString, 14)%>/
                                <%#Eval("sapIndexNo").ToString%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="开始<br />结束时间">
                            <ItemTemplate>
                                <%#Left(Eval("chk_start_date").ToString, 14)%><br />
                                <%#Left(Eval("chk_end_date").ToString, 14)%><br />
                                <%#Eval("chk_user").ToString%> <%#Eval("user_name").ToString%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <%#Eval("mark").ToString%>
                                <hr />
                                <%#Eval("buliang").ToString%>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%--                        <asp:TemplateField HeaderText="欠品">
                            <ItemTemplate>
                                <asp:CheckBox ID="cbQianpin" runat="server" Text="欠品" />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField HeaderText="向先、经销商">
                            <ItemTemplate>
                                <%#Eval("xiangxian").ToString%><br />
                                <%#Eval("jxs_name").ToString%>
                            </ItemTemplate>
                        </asp:TemplateField>


                    </Columns>
                    <HeaderStyle CssClass="tbl_ms_title_new" />
                </asp:GridView>
            </div>
        </article>
    </form>
</body>
</html>
