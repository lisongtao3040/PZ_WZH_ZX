<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_check_list.aspx.vb" Inherits="t_check_list" %>

<%@ Register Src="~/UserControls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/UserControls/Footer.ascx" TagPrefix="uc2" TagName="Footer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
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
    <link rel="stylesheet" href="./t_check_list.aspx.css?randomId=1" />
    <script type="text/javascript" src="./t_check_list.aspx.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <%--cd	no CHADZDH1CABXXXX	9013568433--%>
    <form id="form1" runat="server">

        <div style="position: absolute; z-index: 100000; top: 10px; left: 10px;">
            <asp:LinkButton ID="lbSCX" runat="server">查看生产性</asp:LinkButton>
            <asp:CheckBox ID="cb1" runat="server" Text="1部" Checked="true" />
            <asp:CheckBox ID="cb2" runat="server" Text="2部" Checked="true" />
            <asp:CheckBox ID="cb3" runat="server" Text="3部" Checked="true" />
            <asp:CheckBox ID="cb4" runat="server" Text="4部" Checked="true" />

        </div>
        <uc1:Header runat="server" ID="UserHeader" title="检查一览" />


        <article>
            <div class="top_button_panel">
                <asp:TextBox ID="tbxTpNo" CssClass="tp_barcode" runat="server" placeholder="托盘CD" Text="" Font-Size="40px" ForeColor="blue" Width="180px" Height="40px"></asp:TextBox>
                <asp:Button ID="btnTpChkList" runat="server" Text="托盘检查一览" Width="210px" />
                <input type="button" id="btnClearTp" value="清空" />
                <div style="float: right">
                     <input type="button" id="btnScsj" value="生产实际" />
                    <input type="button" id="btnBuliang" value="不良一览" />
                </div>
            </div>

            <div class="top_button_panel">

                <%-- CHADDLWKKABTAXX 9006160969 --%>
                <%-- CHFDEDMAAAAXXJX 9006331505 --%>
                <%--CHADDLW3A1BTAXX 9006160965--%>
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
                <asp:Button ID="btnBack" runat="server" Text="返回" />
                <hr />
                <asp:Button ID="btnNewChk" runat="server" Text="新规" Width="100" />
                <asp:Button ID="btnSetDefault" runat="server" Text="默认" Width="100" />

                <asp:Button ID="btnReChk" runat="server" Text="NG再检" Width="140" />


                <%--                <asp:Button ID="btnContinue" runat="server" Text="继续检查"  Width="150"/>--%>
                <asp:Button ID="btnInputByHand" runat="server" Text="手入力" Width="120" />
                <asp:Button ID="btnClear" runat="server" Text="清除" Width="100" />

                <asp:Button ID="btnNewChkNoPlan" runat="server" Text="无计划新规" Width="170" />
                <asp:Label ID="lblGT" runat="server" Text=""></asp:Label>
            </div>


            <asp:Panel ID="PanelZen" runat="server" Height="400px" CssClass="PGvZen">
                &nbsp;&nbsp;&nbsp;
                <a style="font-size: 40px; color: red;">以下 前回～ NG</a>&nbsp;&nbsp;&nbsp;<input type="button" value=" 关闭 " onclick="$('#PanelZen').hide();" />
                <asp:GridView ID="gvZen" runat="server" AutoGenerateColumns="False"
                    Width="780px" CssClass="gv">
                    <Columns>
                        <asp:TemplateField HeaderText="回">
                            <ItemTemplate>
                                <%#Eval("row_no").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="70" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="分类">
                            <ItemTemplate>
                                <%#Eval("kind_name").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="120" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="位置">
                            <ItemTemplate>
                                <%#Eval("chk_pos").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="120" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="项目名">
                            <ItemTemplate>
                                <%#Eval("chk_km_name").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="150" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="方法">
                            <ItemTemplate>
                                <%#Eval("chk_fs_txt").ToString%><br />
                            </ItemTemplate>
                            <ItemStyle Width="40" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="实测1">
                            <ItemTemplate>
                                <%#Eval("in_1").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="110px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="结果">
                            <ItemTemplate>
                                <%# PageCom.GetTextJieguo(Eval("result").ToString)%>
                            </ItemTemplate>
                            <ItemStyle Width="40" HorizontalAlign="Center" CssClass="JQ_JIEGUO" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="备注">
                            <ItemTemplate>
                                <%#Eval("mark").ToString %>
                            </ItemTemplate>
                            <ItemStyle Font-Size="12" CssClass="JQ_BEIZHU" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="tbl_ms_title_new" />
                </asp:GridView>
            </asp:Panel>

            <table class="gvtitle" cellspacing="0" style="width: 820px;">
                <tr class="">
                    <td style="width: 160px">ID<br />
                        作番
                    </td>
                    <td style="width: 80px">数量<br />
                        预定日
                    </td>
                    <td style="width: 60px">结果<br />
                        状态
                    </td>
                    <td style="width: 120px">特注号<br />
                        订单号/序号</td>
                    <td style="width: 140px">开始<br />
                        结束时间<br />
                        检查员
                    </td>
                    <td style="width: 100px">继承<br />
                        结果
                    </td>
                    <%--<td style="width: 50px">欠品</td>--%>
                    <td style="width: 50px">不良</td>
                    <td style="border-right: solid 1px #000;">谨慎
                    </td>
                </tr>
            </table>
            <div style="width: 840px; overflow: auto; height: 1000px;">
                <asp:GridView ID="gvLastCheckResultMS" runat="server" AutoGenerateColumns="False" Width="820px" CssClass="gv" ShowHeader="false">
                    <Columns>
                        <asp:TemplateField HeaderText="ID<br />作番">
                            <ItemTemplate>
                                <%#GetIdAndNo(Eval("ck_id").ToString, Eval("cd").ToString, Eval("no").ToString, Eval("status").ToString, Eval("chk_user").ToString)%>
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
                        <asp:TemplateField HeaderText="继承<br />结果">
                            <ItemTemplate>
                                <%#Eval("shared_ck_id").ToString%>
                                <br />
                                <%#Eval("shared_no").ToString%>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <%--                        <asp:TemplateField HeaderText="欠品">
                            <ItemTemplate>
                                <asp:CheckBox ID="cbQianpin" runat="server" Text="欠品" />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField HeaderText="不良">
                            <ItemTemplate>
                                <%#Eval("buliangTxt").ToString%>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="谨慎使用">
                            <ItemTemplate>
                                <%# GetDelBtnDisabled(Eval("ck_id").ToString)%>
                            </ItemTemplate>
                            <ItemStyle />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="tbl_ms_title_new" />
                </asp:GridView>
            </div>
        </article>

        <asp:HiddenField ID="hid_ck_id" runat="server" />
        <asp:HiddenField ID="hid_jxs_name" runat="server" />
        <asp:Button ID="btnEdit" runat="server" Text="编辑执行" />
        <asp:Button ID="btnDel" runat="server" Text="删除执行" />
    </form>
</body>
</html>
