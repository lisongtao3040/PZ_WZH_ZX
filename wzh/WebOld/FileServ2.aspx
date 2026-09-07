<%@ Page Language="VB" AutoEventWireup="false" CodeFile="FileServ2.aspx.vb" Inherits="FileServ2" EnableEventValidation="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>图片上传 - FileServ2</title>
    <style>
        body {
            font-family: "Microsoft YaHei", sans-serif;
            background: #f4f7f6;
            color: #333;
            margin: 30px;
        }

        .container {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        /* 文件夹行背景色 */
        .folder-row {
            background-color: #e3f2fd !important;
            cursor: pointer;
        }

        .file-row {
            background-color: #ffffff;
        }

        .breadcrumb {
            padding: 10px;
            background: #eceff1;
            margin-bottom: 15px;
            border-left: 5px solid #2196f3;
        }

            .breadcrumb a {
                text-decoration: none;
                color: #2196f3;
                font-weight: bold;
            }

        /* 全屏遮罩层 */
        #loadingOverlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.6);
            z-index: 10000;
            display: none;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            color: white;
        }

        .loader {
            border: 6px solid #f3f3f3;
            border-top: 6px solid #3498db;
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }

        .grid-header th {
            background: #2196f3;
            color: white;
            padding: 10px;
        }

        .btn {
            padding: 5px 15px;
            cursor: pointer;
            margin-right: 5px;
        }

        /* 分页样式 */
        .grid-pager {
            padding: 10px 0;
            background-color: #f9f9f9;
        }

        .grid-pager a, .grid-pager span {
            padding: 5px 10px;
            margin: 0 2px;
            border: 1px solid #ddd;
            border-radius: 3px;
            text-decoration: none;
            color: #333;
        }

        .grid-pager a:hover {
            background-color: #2196f3;
            color: white;
            border-color: #2196f3;
        }

        .grid-pager span {
            background-color: #2196f3;
            color: white;
            border-color: #2196f3;
        }

        .msg-box {
            margin: 10px 0;
            padding: 10px;
            border-radius: 4px;
        }

        /* 图片缩略图样式 */
        .thumbnail {
            width: 160px;
            height: 160px;
            object-fit: cover;
            cursor: pointer;
            border: 2px solid #ddd;
            border-radius: 4px;
            transition: all 0.3s;
        }

        .thumbnail:hover {
            border-color: #2196f3;
            transform: scale(1.05);
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }

        /* 图片预览遮罩层 */
        #imagePreviewOverlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0,0,0,0.85);
            z-index: 10001;
            display: none;
            flex-direction: column;
            align-items: center;
            justify-content: center;
        }

        #previewImg {
            max-width: 90%;
            max-height: 80%;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.5);
        }

        #previewTitle {
            color: white;
            margin-top: 15px;
            font-size: 18px;
            text-align: center;
        }

        .close-preview {
            position: absolute;
            top: 20px;
            right: 30px;
            color: white;
            font-size: 40px;
            cursor: pointer;
            font-weight: bold;
            transition: all 0.3s;
        }

        .close-preview:hover {
            color: #ff5722;
            transform: rotate(90deg);
        }

        /* 搜索框样式 */
        .search-container {
            display: inline-block;
            margin-right: 10px;
        }

        .search-input {
            padding: 8px 12px;
            border: 2px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
            width: 250px;
            transition: border-color 0.3s;
        }

        .search-input:focus {
            outline: none;
            border-color: #2196f3;
        }

        .btn-success {
            background-color: #4caf50;
            color: white;
            border: none;
        }

        .btn-success:hover {
            background-color: #45a049;
        }

        .btn-warning {
            background-color: #ff9800;
            color: white;
            border: none;
        }

        .btn-warning:hover {
            background-color: #f57c00;
        }

        /* 复选框放大样式 */
        input[type="checkbox"] {
            width: 20px;
            height: 20px;
            cursor: pointer;
            transform: scale(1.5);
            margin: 5px;
        }

        /* ✅ 将分页控件移动到标题行 */
        .grid-pager {
            text-align: right;
            padding: 10px;
            background-color: #f5f5f5;
            border-bottom: 1px solid #ddd;
        }

        .grid-pager table {
            display: inline-block;
        }

        .grid-pager a, .grid-pager span {
            padding: 3px 8px;
            margin: 0 2px;
            border: 1px solid #ddd;
            border-radius: 3px;
            text-decoration: none;
            color: #333;
            display: inline-block;
        }

        .grid-pager span {
            background-color: #4caf50;
            color: white;
            border-color: #4caf50;
        }

        .grid-pager a:hover {
            background-color: #f0f0f0;
        }
    </style>
    <script type="text/javascript">
        function showMask(msg) {
            document.getElementById('maskText').innerText = msg;
            document.getElementById('loadingOverlay').style.display = 'flex';
            return true;
        }

        function hideMask() {
            document.getElementById('loadingOverlay').style.display = 'none';
        }

        function toggleSelectAll(chk) {
            var grid = document.getElementById('<%= gvFiles.ClientID %>');
            var rows = grid.getElementsByTagName("tr");
            var selectedCount = 0;
            var skippedCount = 0;
            
            // 遍历所有数据行（跳过表头）
            for (var i = 1; i < rows.length; i++) {
                var checkboxes = rows[i].getElementsByTagName("input");
                var sourceLabel = rows[i].querySelector('[id*="lblSource"]');
                
                // 检查这一行的来源
                var isFromDB = false;
                if (sourceLabel && (sourceLabel.innerText.trim() === "DB" || sourceLabel.innerText.trim() === "数据库")) {
                    isFromDB = true;
                }
                
                // 遍历该行所有复选框
                for (var j = 0; j < checkboxes.length; j++) {
                    if (checkboxes[j].type === "checkbox" && checkboxes[j].id.indexOf("chkSelect") !== -1) {
                        if (isFromDB) {
                            // 数据库来源：强制不选中
                            checkboxes[j].checked = false;
                            skippedCount++;
                        } else {
                            // 文件夹来源：根据全选状态设置
                            checkboxes[j].checked = chk.checked;
                            if (chk.checked) {
                                selectedCount++;
                            }
                        }
                    }
                }
            }
            
            // 全选后检查批量删除按钮状态
            checkBatchDeleteButton();
        }

        // 单个复选框变化时检查
        function onCheckboxChange() {
            checkBatchDeleteButton();
        }

        // 数据库 GridView 全选功能
        function toggleDBSelectAll(chk) {
            var grid = document.getElementById('<%= gvDBFiles.ClientID %>');
            var rows = grid.getElementsByTagName("tr");
            
            // 遍历所有数据行（跳过表头）
            for (var i = 1; i < rows.length; i++) {
                var checkboxes = rows[i].getElementsByTagName("input");
                
                // 遍历该行所有复选框
                for (var j = 0; j < checkboxes.length; j++) {
                    if (checkboxes[j].type === "checkbox" && checkboxes[j].id.indexOf("chkSelect") !== -1) {
                        checkboxes[j].checked = chk.checked;
                    }
                }
            }
        }

        // 数据库单个复选框变化
        function onDBCheckboxChange() {
            // 可以在这里添加数据库批量操作的逻辑
        }

        // 检查批量删除按钮状态
        function checkBatchDeleteButton() {
            var grid = document.getElementById('<%= gvFiles.ClientID %>');
            var rows = grid.getElementsByTagName("tr");
            var hasDbSelected = false;
            
            // 遍历所有行，检查是否有数据库来源的记录被选中
            for (var i = 1; i < rows.length; i++) { // 从1开始跳过表头
                var checkboxes = rows[i].getElementsByTagName("input");
                var sourceLabel = rows[i].querySelector('[id*="lblSource"]');
                
                for (var j = 0; j < checkboxes.length; j++) {
                    if (checkboxes[j].type === "checkbox" && checkboxes[j].checked) {
                        // 检查来源标签（DB 或 数据库）
                        if (sourceLabel && (sourceLabel.innerText.trim() === "DB" || sourceLabel.innerText.trim() === "数据库")) {
                            hasDbSelected = true;
                            break;
                        }
                    }
                }
                if (hasDbSelected) break;
            }
            
            // 根据是否有数据库记录被选中来启用/禁用按钮
            var btnBatchDel = document.getElementById('<%= btnBatchDel.ClientID %>');
            if (btnBatchDel) {
                if (hasDbSelected) {
                    btnBatchDel.disabled = true;
                    btnBatchDel.style.opacity = '0.5';
                    btnBatchDel.style.cursor = 'not-allowed';
                    btnBatchDel.title = '不能批量删除数据库中的图片';
                } else {
                    btnBatchDel.disabled = false;
                    btnBatchDel.style.opacity = '1';
                    btnBatchDel.style.cursor = 'pointer';
                    btnBatchDel.title = '';
                }
            }
        }

        // 双击文件夹逻辑模拟（由于GridView限制，通过JS辅助触发）
        function onFolderDblClick(name) {
            showMask("正在进入目录...");
            __doPostBack('btnEnterFolder', name);
        }

        // 预览图片
        function previewImage(imgSrc, imgName) {
            var overlay = document.getElementById('imagePreviewOverlay');
            var img = document.getElementById('previewImg');
            var title = document.getElementById('previewTitle');
            
            img.src = imgSrc;
            title.innerText = imgName;
            overlay.style.display = 'flex';
        }

        function closePreview() {
            document.getElementById('imagePreviewOverlay').style.display = 'none';
        }

        // 验证搜索关键词
        function validateSearch() {
            // 允许空条件搜索，显示所有文件
            return true;
        }

        // 验证数据库搜索输入
        function validateDBSearch() {
            var keyword = document.getElementById('<%= txtDBSearch.ClientID %>').value.trim();
            if (keyword === '') {
                alert('请输入搜索关键词后再进行搜索！');
                document.getElementById('<%= txtDBSearch.ClientID %>').focus();
                return false;
            }
            return true;
        }

        // 批量下载前显示遮罩
        function beforeBatchDownload() {
            showMask('正在打包下载...');
            // 设置超时，5秒后自动关闭遮罩（以防下载失败或卡住）
            setTimeout(function() {
                hideMask();
            }, 5000);
            return true;
        }

        // 数据库批量下载前显示遮罩
        function beforeDBBatchDownload() {
            showMask('正在打包下载数据库图片...');
            // 设置超时，5秒后自动关闭遮罩
            setTimeout(function() {
                hideMask();
            }, 5000);
            return true;
        }

        // 页面加载完成后隐藏遮罩
        window.onload = function() {
            hideMask();
            // 初始化批量删除按钮状态
            checkBatchDeleteButton();
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="loadingOverlay">
            <div class="loader"></div>
            <p id="maskText" style="margin-top: 15px; font-size: 16px;"></p>
        </div>

        <div class="container">
            <!-- 上传面板 -->
            <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 20px; border-radius: 10px; margin-bottom: 20px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);">
                <h2 style="color: white; margin-top: 0; text-align: center;">📤 图片上传</h2>
                <div style="background: rgba(255,255,255,0.95); padding: 20px; border-radius: 8px; margin-top: 15px;">
                    <div style="display: flex; gap: 30px; align-items: center;">
                        <!-- 左侧：使用说明 -->
                        <div style="flex: 1; border-right: 2px solid #ddd; padding-right: 20px;">
                            <strong style="color: #667eea; font-size: 16px;">📋 使用方法：</strong>
                            <span style="color: #666; display: block; margin: 5px 0 10px 0; font-size: 13px;">(代替原来复制图片文件到共享文件夹)</span>
                            <ol style="margin: 0; padding-left: 20px; color: #555; line-height: 1.8;">
                                <li>把要上传的图片放到一个新建文件夹中</li>
                                <li>点击【选择文件】按钮，选择文件夹</li>
                                <li>点击【同步并开始上传】</li>
                            </ol>
                        </div>
                        
                        <!-- 右侧：上传控件 -->
                        <div style="flex: 1; padding-left: 10px;">
                            <strong style="color: #667eea; font-size: 16px; display: block; margin-bottom: 10px;">📁 上传文件/文件夹：</strong>
                            <input type="file" id="fileInput" name="fileInput" webkitdirectory multiple runat="server" style="margin-bottom: 15px; padding: 10px; font-size: 14px; border: 2px solid #ddd; border-radius: 5px; width: 100%; cursor: pointer;" />
                            <asp:Button ID="btnUpload" runat="server" Text="🚀 同步并开始上传" CssClass="btn btn-primary" OnClientClick="return showMask('正在处理文件并上传，请勿关闭窗口...');" Style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border: none; padding: 12px 40px; font-size: 16px; cursor: pointer; display: inline-block;" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="breadcrumb" style="display:none">
                <asp:LinkButton ID="btnBack" runat="server" Text="[返回上级]" OnClick="btnBack_Click" OnClientClick="return showMask('返回中...');" />
                | 当前位置：<asp:Literal ID="litPath" runat="server" />
            </div>

            <div style="background: #f9f9f9; padding: 15px; border: 1px solid #ddd; margin-bottom: 20px;">
                <asp:TextBox ID="txtNewFolder" runat="server" placeholder="新文件夹名称" style="display:none"></asp:TextBox>
                <asp:Button ID="btnCreateDir" runat="server" Text="新建文件夹" CssClass="btn" style="display:none"/>
                
                <!-- 搜索功能 - 左右分栏 -->
                <div style="display: flex; gap: 20px; margin-top: 10px;">
                    <!-- 左侧：文件夹搜索 -->
                    <div style="flex: 1; padding: 10px; border: 1px solid #4caf50; border-radius: 5px; background-color: #f1f8e9;">
                        <strong style="color: #4caf50;">📁 文件夹搜索：</strong>
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="输入图片名称关键词..." Style="width: 60%;" />
                        <asp:Button ID="btnSearch" runat="server" Text="搜索" CssClass="btn btn-primary" OnClick="btnSearch_Click" OnClientClick="return validateSearch();" />
                        <asp:Button ID="btnClearSearch" runat="server" Text="清除" CssClass="btn" OnClick="btnClearSearch_Click" />
                    </div>
                    
                    <!-- 右侧：数据库搜索 -->
                    <div style="flex: 1; padding: 10px; border: 1px solid #ff5722; border-radius: 5px; background-color: #fbe9e7;">
                        <strong style="color: #ff5722;">💾 数据库搜索：</strong>
                        <asp:TextBox ID="txtDBSearch" runat="server" CssClass="search-input" placeholder="输入图片名称关键词..." Style="width: 60%;" />
                        <asp:Button ID="btnDBSearch" runat="server" Text="搜索" CssClass="btn" BackColor="#ff5722" ForeColor="White" OnClick="btnDBSearch_Click" OnClientClick="return validateDBSearch();" />
                        <asp:Button ID="btnClearDBSearch" runat="server" Text="清除" CssClass="btn" OnClick="btnClearDBSearch_Click" />
                    </div>
                </div>
                
                <hr />
            </div>

            <asp:Label ID="pnlMsg" runat="server" Visible="false" CssClass="msg-box"></asp:Label>

            <!-- 左右分栏布局 -->
            <div style="display: flex; gap: 20px; margin-top: 20px;">
                <!-- 左侧：文件夹来源 -->
                <div style="flex: 1; border: 1px solid #ddd; padding: 15px; border-radius: 8px; background-color: #f9f9f9;">
                    <div style="display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #4caf50; padding-bottom: 10px; margin-bottom: 10px;">
                        <h3 style="margin: 0; color: #4caf50;">
                            📁 文件夹来源
                        </h3>
                        <div style="text-align: right;">
                            <asp:Label ID="lblFolderCount" runat="server" Text="" Style="color: #4caf50; font-weight: bold; font-size: 14px; display: block; margin-bottom: 5px;" />
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnBatchDownload" runat="server" Text="批量下载选中" CssClass="btn btn-success" OnClick="btnBatchDownload_Click" OnClientClick="return beforeBatchDownload();" Style="margin-right: 5px;" />
                        <asp:Button ID="btnBatchDel" runat="server" Text="批量删除选中" CssClass="btn" BackColor="#e57373" OnClientClick="return confirm('确认删除所有选中项？') && showMask('正在删除...');" />
                    </div>
                    <div id="folderPager" runat="server" style="font-size: 13px;"></div>
                </div>
            </div>
                    <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="False" Width="100%"
                        DataKeyNames="FullName,IsDirectory,Name,SourceType" OnRowDataBound="gvFiles_RowDataBound" OnRowCommand="gvFiles_RowCommand"
                        GridLines="None" CellPadding="8" AllowPaging="False" PageSize="10">
                        <HeaderStyle CssClass="grid-header" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <input type="checkbox" onclick="toggleSelectAll(this)" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" onclick="onCheckboxChange()" />
                                </ItemTemplate>
                                <ItemStyle Width="50px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="缩略图">
                                <ItemTemplate>
                                    <asp:Image ID="imgThumbnail" runat="server" CssClass="thumbnail" Visible="false" />
                                    <asp:Literal ID="litIcon" runat="server" Visible="false" />
                                </ItemTemplate>
                                <ItemStyle Width="180px" HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="文件信息">
                                <ItemTemplate>
                                    <div style="text-align: left;">
                                        <div style="margin-bottom: 8px;">
                                            <strong>名称：</strong>
                                            <asp:LinkButton ID="lnkName" runat="server" Text='<%# Eval("Name") %>'
                                                CommandArgument='<%# Eval("Name") %>' OnClick="lnkName_Click" />
                                        </div>
                                        <div style="margin-bottom: 8px; color: #666; font-size: 13px;">
                                            <strong>修改日期：</strong><%# Eval("LastWriteTime", "{0:yyyy-MM-dd HH:mm}") %>
                                        </div>
                                        <div>
                                            <asp:Button ID="btnDl" runat="server" Text="下载"
                                                CommandName="DownloadItem"
                                                CommandArgument='<%# Eval("Name") %>' CssClass="btn btn-primary" Style="margin-right: 5px;" />

                                            <asp:Button ID="btnDel" runat="server" Text="删除"
                                                CommandName="DelItem"
                                                CommandArgument='<%# Eval("Name") %>'
                                                OnClientClick="return confirm('确定要删除吗？');" CssClass="btn" BackColor="#e57373" Visible="true" />
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle Width="300px" VerticalAlign="Top" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>

                <!-- 右侧：数据库来源 -->
                <div style="flex: 1; border: 1px solid #ddd; padding: 15px; border-radius: 8px; background-color: #ffffff;">
                    <div style="display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #ff5722; padding-bottom: 10px; margin-bottom: 10px;">
                        <h3 style="margin: 0; color: #ff5722;">
                            💾 数据库来源
                        </h3>
                        <div style="text-align: right;">
                            <asp:Label ID="lblDBCount" runat="server" Text="" Style="color: #ff5722; font-weight: bold; font-size: 14px; display: block; margin-bottom: 5px;" />
                            <div style="margin-bottom: 5px;">
                                <asp:Button ID="btnDBBatchDownload" runat="server" Text="批量下载选中" CssClass="btn" BackColor="#ff5722" ForeColor="White" OnClick="btnDBBatchDownload_Click" OnClientClick="return beforeDBBatchDownload();" />
                            </div>
                            <div id="dbPager" runat="server" style="font-size: 13px;"></div>
                        </div>
                    </div>
                    <asp:GridView ID="gvDBFiles" runat="server" AutoGenerateColumns="False" Width="100%"
                        DataKeyNames="FullName,IsDirectory,Name,SourceType" OnRowDataBound="gvDBFiles_RowDataBound" OnRowCommand="gvDBFiles_RowCommand" OnPageIndexChanging="gvDBFiles_PageIndexChanging"
                        GridLines="None" CellPadding="8" AllowPaging="True" PageSize="10" PagerSettings-Visible="False">
                        <HeaderStyle CssClass="grid-header" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <input type="checkbox" onclick="toggleDBSelectAll(this)" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" onclick="onDBCheckboxChange()" />
                                </ItemTemplate>
                                <ItemStyle Width="50px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="缩略图">
                                <ItemTemplate>
                                    <asp:Image ID="imgThumbnail" runat="server" CssClass="thumbnail" Visible="false" />
                                    <asp:Literal ID="litIcon" runat="server" Visible="false" />
                                </ItemTemplate>
                                <ItemStyle Width="180px" HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="文件信息">
                                <ItemTemplate>
                                    <div style="text-align: left;">
                                        <div style="margin-bottom: 8px;">
                                            <strong>名称：</strong>
                                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                                        </div>
                                        <div style="margin-bottom: 8px; color: #666; font-size: 13px;">
                                            <strong>修改日期：</strong><%# Eval("LastWriteTime", "{0:yyyy-MM-dd HH:mm}") %>
                                        </div>
                                        <div>
                                            <asp:Button ID="btnDl" runat="server" Text="下载"
                                                CommandName="DownloadItem"
                                                CommandArgument='<%# Eval("Name") %>' CssClass="btn btn-primary" />
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <ItemStyle Width="300px" VerticalAlign="Top" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- 图片预览遮罩层 -->
        <div id="imagePreviewOverlay" onclick="closePreview()">
            <span class="close-preview">&times;</span>
            <img id="previewImg" src="" alt="预览图片" />
            <div id="previewTitle"></div>
        </div>

        <asp:LinkButton ID="btnEnterFolder" runat="server" Style="display: none;" OnClick="lnkName_Click" />
    </form>
</body>
</html>
