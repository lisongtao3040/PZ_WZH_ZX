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

        <%-- 変更1: 左上角部品选择面板 --%>
        <div class="top-left-panel">
            <asp:LinkButton ID="lbSCX" runat="server">生产性</asp:LinkButton>
            <div class="bubetsu-group">
                <asp:CheckBox ID="cb1" runat="server" Text="1部" Checked="true" />
                <asp:CheckBox ID="cb2" runat="server" Text="2部" Checked="true" />
                <asp:CheckBox ID="cb3" runat="server" Text="3部" Checked="true" />
                <asp:CheckBox ID="cb4" runat="server" Text="4部" Checked="true" />
            </div>

            <input type="button" id="btnScsj" class=""  value="实际" />
            <input type="button" id="btnBuliang" class=""  value="不良" />
           <%-- <input type="button" id="btnFullTrayList" value="满托盘" onclick="location.href='t_FullTrayList.aspx';" style="background:#d0e8ff;" />--%>

                        <asp:Button ID="btnFullTrayList" runat="server" Text="托盘list" />
            <asp:Button ID="btnInputByHand" runat="server" Text="手" />
            <asp:Button ID="btnBack" runat="server" Text="返回" />
             <asp:Button ID="btnBack2" runat="server" Text="返回" />
        </div>
        <%-- 変更3: UserHeader 标题清空 --%>
        <uc1:Header runat="server" ID="UserHeader" title="" />


        <article>
            <%-- 変更4: 第一行 移除 float:right div，改用 class --%>
            <div class="top_button_panel">
                <asp:TextBox ID="tbxTpNo" CssClass="tp_barcode" runat="server" placeholder="托盘CD" Text=""></asp:TextBox>
                <asp:Button ID="btnTpChkList" runat="server" Text="托盘检查一览" Width="182px" />
                <input type="button" id="btnClearTp" value="清空" style="width:100px;"/>
                <%-- 変更2: lblGT 移到 4部 右侧 --%>
                <asp:Label ID="lblGT" runat="server" Text="" Style="margin-left: 8px; font-weight: 700; color: #333; font-size: 16px;"></asp:Label>

            </div>

            <%-- 変更5: 第二行 tbxCd / tbxNo 强制高度 60px + 删除 lblGT --%>
            <div class="top_button_panel">

                <%-- CHADDLWKKABTAXX 9006160969 --%>
                <%-- CHFDEDMAAAAXXJX 9006331505 --%>
                <%--CHADDLW3A1BTAXX 9006160965--%>
                <asp:TextBox ID="tbxCd" runat="server" placeholder="商品CD" Text="" Width="300" Font-Size="26px" AutoCompleteType="Disabled" Style="height: 50px !important"></asp:TextBox>

                <asp:TextBox ID="tbxNo" runat="server" placeholder="作番" Text="" Width="180" Font-Size="26px" AutoCompleteType="Disabled" Style="height: 50px !important;text-align:center;"></asp:TextBox>
                <%--              
<input runat ="server"  type="tel"  id="tbxCd" placeholder="商品CD" Text="CHFDEDMAAAAXXJX" Width="300" Font-Size="26px" />
<input runat ="server" type="tel" id="tbxNo"  placeholder="作番" Text="9006331505" Width="160" Font-Size="26px"  />
                --%>

                <asp:DropDownList ID="ddlHiinai" runat="server" Width="100px" Style="font-size: 24px;">
                </asp:DropDownList>
                日
                <asp:Button ID="btnSel" runat="server" Text="检索" Width="80" />
                           <asp:Button ID="btnClear" runat="server" Text="清除" Width="80" />

            </div>
                       <div class="top_button_panel">
                <asp:Button ID="btnNewChk" runat="server" Text="新规" Width="146" />
                <asp:Button ID="btnSetDefault" runat="server" Text="默认" Width="146" />

                <asp:Button ID="btnReChk" runat="server" Text="NG再检" Width="185" />


                <%--                <asp:Button ID="btnContinue" runat="server" Text="继续检查"  Width="150"/>--%>

     

                <asp:Button ID="btnNewChkNoPlan" runat="server" Text="无计划新规" Width="160" style="margin-left:0px" />

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

            <table class="gvtitle" cellspacing="0" style="width: 830px;">
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
                <asp:GridView ID="gvLastCheckResultMS" runat="server" AutoGenerateColumns="False" Width="830px" CssClass="gv" ShowHeader="false">
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
