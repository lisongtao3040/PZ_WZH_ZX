<%@ Page Language="VB" AutoEventWireup="false" CodeFile="t_check_ms.aspx.vb" Inherits="t_check_ms" EnableEventValidation="false" %>


<%@ Register Src="~/UserControls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<%@ Register Src="~/UserControls/Footer.ascx" TagPrefix="uc1" TagName="Footer" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <%--  <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />--%>
    <title></title>
    <!--JQUERY-->
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./jquery-ui-1.12.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="./jquery/jquery.cookie.js"></script>
    <link rel="stylesheet" href="./jquery-ui-1.12.1/jquery-ui.css" />

    <%--共通--%>
    <%-- <link href="./App_Themes/Css/Main.css?randomId=<%=PageCom.GetYmdhmsf()%>" rel="stylesheet" type="text/css" />--%>
    <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
    
    <%--自分頁--%>
    <%
        Response.Write("<link href=""./t_check_ms.aspx.css?randomId=" & PageCom.GetYmdhmsf() & """ rel=""stylesheet"" />")
    %>

<%--    <link href='./t_check_ms.aspx.css?randomId=<%=PageCom.GetYmdhmsf()%>'  rel="stylesheet" />--%>
    <script type="text/javascript" src="./t_check_ms.aspx.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>

    <script type="text/javascript" src="./Js/Qrcode.js"></script>

</head>
<body>
    <form id="form1" runat="server">

        <article>
            <div class="top_button_panel" style="width: 100%; height: 50px; position: relative; margin-bottom: 50px; font-size: 20px;">

                <div class="divBanner" style="">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/Image/UserIcon.png" Style="height: 26px; width: 26px;" />
                    <asp:Label ID="tbxUserCd" runat="server" Text="tbxCd"></asp:Label>
                    (<asp:Label ID="tbxUserName" runat="server" Text="tbxCd"></asp:Label>)
                    <asp:Label ID="lblLineCd" runat="server" Text=""></asp:Label>
                    (<asp:Label ID="lblLineName" runat="server" Text=""></asp:Label>)

                    <asp:Button ID="btnChkTools" runat="server" Text="检查治具" />
                    <asp:Button ID="btnChkKind" runat="server" Text="分类检查" />
                    <asp:Button ID="btnComplete" runat="server" Text="完了" />
                    <asp:Button ID="btnBack" runat="server" Text="返回" />

                </div>

                <div style="height: 50px; float: left; padding-left: 5px; width: 790px;">


                    <asp:Label ID="tbxCd" runat="server" Text="tbxCd"></asp:Label>
                    <asp:Label ID="tbxNo" runat="server" Text="tbxCd"></asp:Label>
                    <asp:Label ID="tbxCk_id" runat="server" Text="tbxCd" Style="color: #727610; font-size: 12px;"></asp:Label>
                   
                    <%--<asp:LinkButton ID="lbSCX" runat="server">查看生产性</asp:LinkButton>--%>
                     <hr />
                    <div id="SumRlt" style="text-align: right; width: 700px;">
                    </div>

                </div>


            </div>


            <asp:Panel ID="PanelLinks" runat="server" CssClass="chk_div_links" nowrap="nowrap"></asp:Panel>

            <div class="gvDiv" style="width: 800px; height: 500px; background-color: #fff; overflow: auto; border: 1px dotted;">

                <asp:GridView ID="gv" runat="server" AutoGenerateColumns="False"
                    Width="780px" CssClass="gv">
                    <Columns>

                        <asp:TemplateField HeaderText="NO">
                            <ItemTemplate>
                                <%#Eval("row_no").ToString%>
                            </ItemTemplate>
                            <ItemStyle Width="50" />
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
                                <input type="text" class='jqIn1'
                                    value='<%#Eval("in_1").ToString%>'
                                    style="background-color: <%# PageCom.GetInBgColor(Eval("chk_fs").ToString) %>; width: 200px;" />
                            </ItemTemplate>
                            <ItemStyle Width="210px" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="结果">
                            <ItemTemplate>
                                <%# PageCom.GetTextJieguo(Eval("result").ToString)%>
                            </ItemTemplate>
                            <ItemStyle Width="40" HorizontalAlign="Center" CssClass="JQ_JIEGUO" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="备注">
                            <ItemTemplate>
                                <textarea rows="3" id="TextArea1" class='mark' maxlength="100" style="width: 100%; height: 100%;"><%#Eval("mark").ToString %></textarea>
                                <asp:Label ID="lblJZ" CssClass="JZ" runat="server" Text=""></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Font-Size="12" CssClass="JQ_BEIZHU" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="tbl_ms_title_new" />
                </asp:GridView>

                <div style="height: 500px;">
                    &nbsp;
                </div>
            </div>
            <div style="overflow: hidden; margin-bottom: 0px; width: 800px;" class="btmDiv">
                <table>
                    <tr>
                        <td>
                            <div class="imgButton">
                                <input type="button" id="xuanKK" class="jqBflMin" value="☆" xf="0" />
                                <input type="button" id="xuanfu" class="jqBflMin" value="悬浮" xf="0" />
                                <input type="button" class="jqBflMin" value="-" v="1" />

                                <input type="button" class="jqBflMax" value="+" v="2" />
                            </div>
                            <br />
                            <div class="ImgDiv" style="overflow: auto;">

                                <asp:Image ID="imgLook" runat="server" CssClass="JQ_IMG" ImageUrl="~/Image/wait.gif" />

                            </div>
                        </td>
                        <td>
                            <div class="keyBdDiv" style="top: 0px;">
                                <table cellspacing="0" border="1" style="width: 300px;" class="keybd">
                                    <tr class="suu">
                                        <td>7</td>
                                        <td>8</td>
                                        <td>9</td>
                                    </tr>
                                    <tr class="suu">
                                        <td>4</td>
                                        <td>5</td>
                                        <td>6</td>
                                    </tr>
                                    <tr class="suu">
                                        <td>1</td>
                                        <td>2</td>
                                        <td>3</td>
                                    </tr>
                                    <tr class="suu">
                                        <td>0</td>
                                        <td>.</td>
                                        <td class="delkeybord">删</td>
                                    </tr>
                                    <tr class="suu">
                                        <td colspan="3">回车</td>
                                    </tr>
                                    <tr class="panBtn">
                                        <td runat="server" id="tdJinggao" onclick="  " style="background-color: Red;">警</td>
                                        <td style="background-color: #93FF93;">微</td>

                                        <td style="background-color: #93FF93;">合</td>
                                    </tr>
                                    <tr class="panBtn">
                                        <td style="background-color: Red;">轻</td>
                                        <td style="background-color: Red;">中</td>
                                        <td style="background-color: Red;">重</td>
                                        
                                    </tr>
                                </table>

                            </div>
                        </td>
                    </tr>
                </table>
            </div>
                   </article>
        <hr />
         <br />
            <br />
            <br />
        <asp:FileUpload ID="PicUpload1" runat="server" Font-Size="20" Height="80" />
        <asp:FileUpload ID="PicUpload2" runat="server" Font-Size="20" Height="80" />
        <asp:FileUpload ID="PicUpload3" runat="server" Font-Size="20" Height="80" />
        <asp:FileUpload ID="PicUpload4" runat="server" Font-Size="20" Height="80" />
        <asp:FileUpload ID="PicUpload5" runat="server" Font-Size="20" Height="80" />

        <asp:Button ID="btnUpload" runat="server" Text="拍照上传" Font-Size="30" Height="80"  />

        <asp:GridView ID="GvPic"
            runat="server"
            CssClass="chk_ms"
            AutoGenerateColumns="false"
            ShowHeader="false"
            Font-Size="24px"
            CellPadding="0"
            CellSpacing="0"
            Width="800px"
            Style="font-size: 24px;">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Image ID="Image1" runat="server"  Width="700" />
                    </ItemTemplate>
                    <ItemStyle Width="130px" HorizontalAlign="Left" CssClass="" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Button ID="btnDel" runat="server" Text="删除" OnClick="btClick" Font-Size="40" Height="80" Width="60px" />
                    </ItemTemplate>
                    <ItemStyle Width="130px" HorizontalAlign="Left" CssClass="" />
                </asp:TemplateField>


            </Columns>
        </asp:GridView>
 
        <footer>
        </footer>

        <asp:HiddenField ID="hid_jxs_name" runat="server" />
        <asp:HiddenField ID="hidQR1" runat="server" />
        <div style="margin:25px;">
            <div id="QR1" class="QRDIV"></div>
        </div>

        <br /><br />
    </form>
</body>
</html>
