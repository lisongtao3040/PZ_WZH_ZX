<%@ Page Language="VB" AutoEventWireup="false" CodeFile="chk_list.aspx.vb" Inherits="chk_list" %>

<%@ Register Src="~/UserControls/Header.ascx" TagPrefix="uc1" TagName="Header" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <title>检查中一览</title>
    <!--JQUERY-->
    <script type="text/javascript" src="./jquery/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="./jquery-ui-1.12.1/jquery-ui.min.js"></script>
    <script type="text/javascript" src="./jquery/jquery.cookie.js"></script>
    <link rel="stylesheet" href="./jquery-ui-1.12.1/jquery-ui.css" />

    <%--自分頁（不使用 Main.css）--%>
    <link rel="stylesheet" href="./chk_list.aspx.css?randomId=1" />
    <script type="text/javascript" src="./chk_list.aspx.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
        <script type="text/javascript" src="./Js/Main.js?randomId=<%=PageCom.GetYmdhmsf()%>"></script>
</head>
<body>
    <form id="form1" runat="server">

        <div class="page_banner">
            <span class="page_banner_title">检查中一览</span>
        </div>

        <article>
            <%-- 检索条件区 --%>
            <div class="search_panel">
                <table class="search_table">
                    <tr>
                        <td class="search_label">预定检查日</td>
                        <td>
                            <asp:TextBox ID="tbxYoteiChkDateFrom" runat="server" CssClass="date_input" placeholder="开始" Width="120"></asp:TextBox>
                            ～
                            <asp:TextBox ID="tbxYoteiChkDateTo" runat="server" CssClass="date_input" placeholder="结束" Width="120"></asp:TextBox>
                        </td>
                        <td class="search_label">开始检查日</td>
                        <td>
                            <asp:TextBox ID="tbxChkStartDateFrom" runat="server" CssClass="date_input" placeholder="开始" Width="120"></asp:TextBox>
                            ～
                            <asp:TextBox ID="tbxChkStartDateTo" runat="server" CssClass="date_input" placeholder="结束" Width="120"></asp:TextBox>
                        </td>
                        <td>
                            <input type="button" id="btnSearch" value="检索" class="btn_common_new" />
                            <input type="button" id="btnClear" value="清除" class="btn_common_new" />
                        </td>
                    </tr>
                </table>
            </div>

            <%-- 一览区（AJAX 填充） --%>
            <div class="list_panel">
                <table class="list_table" id="chkListTable">
                    <thead>
                        <tr class="list_header">
                            <th style="width: 60px;">操作</th>
                            <th style="width: 50px;">RN</th>
                            <th style="width: 150px;">商品CD</th>
                            <th style="width: 100px;">作番</th>
                            <th style="width: 60px;">部门</th>
                            <th style="width: 60px;">生产线</th>
                            <th style="width: 100px;">检查员</th>
                            <th style="width: 150px;">预定检查日</th>
                            <th style="width: 150px;">开始检查日</th>
                            <th style="width: 150px;">结束检查日</th>
                            <th style="width: 60px;">状态</th>
                            <th style="width: 60px;">结果</th>
                            <th style="width: 60px;">检查次数</th>
                            <th style="width: 60px;">数量</th>
                            <th style="width: 60px;">不良</th>
                            <th style="width: 80px;">继承ID</th>
                            <th style="width: 100px;">更新者</th>
                            <th style="width: 150px;">更新日期</th>
                        </tr>
                    </thead>
                    <tbody id="chkListBody">
                        <tr><td colspan="18" class="list_empty">加载中...</td></tr>
                    </tbody>
                </table>
            </div>

            <%-- 分页区 --%>
            <div class="pager_panel">
                <input type="button" id="btnFirst" value="|<" class="pager_btn" />
                <input type="button" id="btnPrev" value="<" class="pager_btn" />
                <span id="pagerInfo" class="pager_info"></span>
                <input type="button" id="btnNext" value=">" class="pager_btn" />
                <input type="button" id="btnLast" value=">|" class="pager_btn" />
                <span class="pager_info">跳至</span>
                <select id="ddlPage" class="pager_size"></select>
                <span class="pager_info">页</span>
                <select id="ddlPageSize" class="pager_size">
                    <option value="20">20 / 页</option>
                    <option value="50">50 / 页</option>
                    <option value="100">100 / 页</option>
                </select>
            </div>
        </article>
    </form>
</body>
</html>