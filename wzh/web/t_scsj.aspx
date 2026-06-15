<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_scsj.aspx.vb" Inherits="t_scsj" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <!--JQUERY-->
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./jquery-ui-1.12.1/jquery-ui.min.js"></script>
    <link rel="stylesheet" href="./jquery-ui-1.12.1/jquery-ui.css" />

        <%--共通--%>
    <link href="./App_Themes/Css/Main.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>

    <link href="./scsj.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />
    <%--自分頁--%>
    <script type="text/javascript" src="./scsj.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="div_r1">

                <asp:TextBox ID="tbxDateStart" runat="server" CssClass="jqTxtDate" Width="160px"></asp:TextBox> 7:59
            ～
                <asp:TextBox ID="tbxDateEnd" runat="server" CssClass="jqTxtDate" Width="160px"></asp:TextBox> 8:00
            &nbsp;
               <%-- 线&nbsp;<asp:DropDownList ID="ddlLine" runat="server" Style="font-size: 30px"></asp:DropDownList>&nbsp;--%>
                检索:
                <asp:Button ID="btnSelChujian" runat="server" Width="120" Text="检索" CssClass="btn_sel" />    
                        <asp:Button ID="btnBack" runat="server"  Width="120"  Text="关闭" OnClientClick="window.close();return false;" />            
        </div>

                <article>



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
            <div style="width:840px; overflow: auto; height: 1000px;">
                <asp:GridView ID="gvLastCheckResultMS" runat="server" AutoGenerateColumns="False" Width="820px" CssClass="gv" ShowHeader="false">
                    <Columns>
                        <asp:TemplateField HeaderText="ID<br />作番">
                            <ItemTemplate>
                     <%--           <%#Eval("ck_id").ToString%><br>--%>
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
