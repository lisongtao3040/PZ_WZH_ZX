<%@ Page Language="VB" AutoEventWireup="false" CodeFile="FileServ.aspx.vb" Inherits="FileServ" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>文件服务器</title>
    <style type="text/css">
        /* 样式保持不变 */
        body {
            font-family: Arial, Helvetica, sans-serif;
            margin: 20px;
            background-color: #f5f5f5;
        }
        .container {
            max-width: 900px;
            margin: 0 auto;
            background-color: white;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            position: relative;
        }
        h1 {
            color: #333;
            border-bottom: 2px solid #4CAF50;
            padding-bottom: 10px;
        }
        .path-navigation {
            background-color: #f0f0f0;
            padding: 10px;
            margin-bottom: 20px;
            border-radius: 3px;
        }
        .path-navigation a {
            color: #2196F3;
            text-decoration: none;
            margin-right: 5px;
        }
        .path-navigation a:hover {
            text-decoration: underline;
        }
        .upload-section {
            background-color: #f9f9f9;
            padding: 15px;
            margin-bottom: 20px;
            border-radius: 5px;
        }
        .file-list {
            width: 100%;
            border-collapse: collapse;
        }
        .file-list th {
            background-color: #4CAF50;
            color: white;
            padding: 10px;
            text-align: left;
        }
        .file-list td {
            padding: 10px;
            border-bottom: 1px solid #ddd;
        }
        .file-list tr:hover {
            background-color: #f5f5f5;
        }
        .folder-row {
            background-color: #e8f4f8;
            font-weight: bold;
        }
        .folder-row:hover {
            background-color: #d1e7f0;
        }
        .btn {
            padding: 5px 10px;
            margin: 0 2px;
            border: none;
            border-radius: 3px;
            cursor: pointer;
            text-decoration: none;
            font-size: 12px;
        }
        .btn-download {
            background-color: #2196F3;
            color: white;
        }
        .btn-delete {
            background-color: #f44336;
            color: white;
        }
        .btn-upload {
            background-color: #4CAF50;
            color: white;
            padding: 8px 15px;
            font-size: 14px;
        }
        .btn-refresh {
            background-color: #FF9800;
            color: white;
            padding: 8px 15px;
            font-size: 14px;
        }
        .btn-folder {
            background-color: #9C27B0;
            color: white;
            padding: 8px 15px;
            font-size: 14px;
        }
        .message {
            padding: 10px;
            margin: 10px 0;
            border-radius: 3px;
        }
        .success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        .error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        .folder-icon {
            margin-right: 5px;
        }
        
        /* 等待遮罩层样式 */
        .wait-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 9999;
            text-align: center;
            justify-content: center;
            align-items: center;
        }
        
        .wait-content {
            background-color: white;
            padding: 30px 50px;
            border-radius: 10px;
            box-shadow: 0 0 20px rgba(0,0,0,0.3);
            display: inline-block;
        }
        
        .wait-content img {
            width: 50px;
            height: 50px;
            margin-bottom: 15px;
        }
        
        .wait-content p {
            font-size: 18px;
            color: #333;
            margin: 10px 0 0 0;
            font-weight: bold;
        }
        
        .spinner {
            border: 5px solid #f3f3f3;
            border-top: 5px solid #4CAF50;
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
            margin: 0 auto 15px auto;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>
    
    <script type="text/javascript">
        // 显示等待遮罩
        function showWaitOverlay() {
            var overlay = document.getElementById('waitOverlay');
            if (overlay) {
                overlay.style.display = 'flex';
            }
        }
        
        // 隐藏等待遮罩
        function hideWaitOverlay() {
            var overlay = document.getElementById('waitOverlay');
            if (overlay) {
                overlay.style.display = 'none';
            }
        }
        
        // 为所有可能导致刷新的按钮添加点击事件
        function initWaitOverlay() {
            // 上传按钮
            var btnUpload = document.getElementById('<%= btnUpload.ClientID %>');
            if (btnUpload) {
                btnUpload.onclick = function() {
                    // 检查是否有文件选择
                    var fileInput = document.getElementById('<%= fileUpload.ClientID %>');
                    if (fileInput && fileInput.value != '') {
                        showWaitOverlay();
                        return true;
                    } else {
                        alert('请选择要上传的文件');
                        return false;
                    }
                };
            }
            
            // 刷新按钮
            var btnRefresh = document.getElementById('<%= btnRefresh.ClientID %>');
            if (btnRefresh) {
                btnRefresh.onclick = function() {
                    showWaitOverlay();
                    return true;
                };
            }
            
            // 创建文件夹按钮
            var btnCreateFolder = document.getElementById('<%= btnCreateFolder.ClientID %>');
            if (btnCreateFolder) {
                btnCreateFolder.onclick = function() {
                    var folderName = document.getElementById('<%= txtFolderName.ClientID %>');
                    if (folderName && folderName.value.trim() != '') {
                        showWaitOverlay();
                        return true;
                    } else {
                        alert('请输入文件夹名称');
                        return false;
                    }
                };
            }
        }
        
        // 页面加载完成后初始化
        window.onload = function() {
            initWaitOverlay();
        };
        
        // 如果是部分回发，重新初始化
        function pageLoad() {
            initWaitOverlay();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <!-- 全屏等待遮罩层 -->
        <div id="waitOverlay" class="wait-overlay" runat="server">
            <div class="wait-content">
                <div class="spinner"></div>
                <p>请稍候，正在处理...</p>
            </div>
        </div>
        
        <div class="container">
            <h1>文件服务器</h1>
            
            <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="message"></asp:Label>
            
            <div class="path-navigation">
                <strong>当前位置：</strong>
                <asp:HyperLink ID="hlRoot" runat="server" Text="根目录" NavigateUrl="?path=" />
                <asp:Label ID="lblPath" runat="server" />
            </div>
            
            <div class="upload-section">
                <h3>上传文件</h3>
                <asp:FileUpload ID="fileUpload" runat="server" Width="400px" />
                <asp:Button ID="btnUpload" runat="server" Text="上传" CssClass="btn btn-upload" />
                
                <h3 style="margin-top:15px;">创建文件夹</h3>
                <asp:TextBox ID="txtFolderName" runat="server" Width="200px" placeholder="文件夹名称" />
                <asp:Button ID="btnCreateFolder" runat="server" Text="创建" CssClass="btn btn-folder" />
                <asp:Button ID="btnRefresh" runat="server" Text="刷新" CssClass="btn btn-refresh" />
            </div>
            
            <h3>文件列表</h3>
            <asp:GridView ID="gvFiles" runat="server" AutoGenerateColumns="False" 
                CssClass="file-list" GridLines="None" 
                OnRowCommand="gvFiles_RowCommand"
                OnRowDataBound="gvFiles_RowDataBound" 
                ShowHeaderWhenEmpty="True"
                OnRowDeleting="gvFiles_RowDeleting">
                <Columns>
                    <asp:TemplateField HeaderText="名称">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name") %>' />
                            <asp:HiddenField ID="hfFullPath" runat="server" Value='<%# Eval("FullPath") %>' />
                            <asp:HiddenField ID="hfIsFolder" runat="server" Value='<%# Eval("IsFolder") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Size" HeaderText="大小" />
                    <asp:BoundField DataField="ModifiedDate" HeaderText="修改日期" />
                    <asp:TemplateField HeaderText="操作">
                        <ItemTemplate>
                            <asp:Button ID="btnDownload" runat="server" Text="下载" 
                                CommandName="Download" CommandArgument='<%# Eval("FullPath") %>' 
                                CssClass="btn btn-download" />
                            <asp:Button ID="btnDelete" runat="server" Text="删除" 
                                CommandName="Delete" CommandArgument='<%# Eval("FullPath") %>' 
                                CssClass="btn btn-delete" 
                                OnClientClick="return confirm('确定要删除这个项目吗？');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div style="padding:20px; text-align:center;">文件夹为空</div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </form>
</body>
</html>