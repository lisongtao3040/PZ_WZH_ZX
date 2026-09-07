<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Snap.aspx.vb" Inherits="Snap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <input type="file" id="iphoneCamera" accept="image/*" capture="camera" style="display: none;" />
            <button type="button" id="scanBtn" style="padding: 20px; font-size: 18px;">
                📷 点击打开相机拍照
            </button>

            <script>
                // 1. 代替 URLSearchParams 的 ES5 传统获取 URL 参数的方法
                function getQueryString(name) {
                    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
                    var r = window.location.search.substr(1).match(reg);
                    if (r != null) return unescape(r[2]);
                    return null;
                }

                var scanBtn = document.getElementById('scanBtn');
                var iphoneCamera = document.getElementById('iphoneCamera');

                // 2. 绑定点击事件
                scanBtn.addEventListener('click', function () {
                    iphoneCamera.click();
                });

                // 3. 监听拍照/文件选择完成事件
                // 监听拍照组件的变化
                iphoneCamera.addEventListener('change', function (e) {
                    var file = e.target.files[0];
                    if (!file) return;

                    var sessionId = getQueryString("sid");
                    var myId = getQueryString("no"); // 💡 这里放你要决定的文件名 ID（可以从页面元素获取，或者从 URL 获取）

                    // 打包数据
                    var formData = new FormData();
                    formData.append("sessionId", sessionId);
                    formData.append("id", myId);      // 🚀 把 ID 传给后端
                    formData.append("photo", file);    // 拍照的文件

                    var xhr = new XMLHttpRequest();
                    xhr.open("POST", "Phone.asmx/UploadPhoto", true);

                    xhr.onreadystatechange = function () {
                        if (xhr.readyState === 4 && xhr.status === 200) {
                            alert("上传并保存成功！");
                        }
                    };
                    xhr.send(formData);
                });
            </script>
        </div>
    </form>
</body>
</html>
