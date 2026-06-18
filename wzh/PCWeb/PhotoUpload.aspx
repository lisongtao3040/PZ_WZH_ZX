<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PhotoUpload.aspx.cs" Inherits="PhotoUpload" ResponseEncoding="UTF-8" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no" />
    <title>写真アップロード</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #1a1a2e; display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 16px; }
        .container { background: #fff; border-radius: 16px; box-shadow: 0 8px 30px rgba(0,0,0,0.3); padding: 32px; text-align: center; max-width: 480px; width: 100%; }
        h1 { font-size: 22px; color: #1a1a2e; margin-bottom: 4px; }
        .subtitle { color: #888; font-size: 13px; margin-bottom: 24px; }

        /* カメラプレビューエリア */
        .camera-area { position: relative; background: #000; border-radius: 12px; overflow: hidden; margin-bottom: 16px; min-height: 240px; display: flex; align-items: center; justify-content: center; }
        #video { width: 100%; display: block; }
        #canvas { display: none; }
        #preview { max-width: 100%; max-height: 300px; border-radius: 8px; display: none; margin: 0 auto; }

        /* 撮影ボタン */
        .btn-row { display: flex; gap: 12px; justify-content: center; margin-bottom: 16px; flex-wrap: wrap; }
        .btn { display: inline-flex; align-items: center; gap: 6px; padding: 12px 28px; border: none; border-radius: 40px; font-size: 16px; font-weight: 600; cursor: pointer; transition: all 0.2s; text-decoration: none; }
        .btn-primary { background: #1a73e8; color: #fff; }
        .btn-primary:hover { background: #1557b0; }
        .btn-success { background: #34a853; color: #fff; }
        .btn-success:hover { background: #2d8f47; }
        .btn-secondary { background: #f1f3f4; color: #444; }
        .btn-secondary:hover { background: #e2e4e6; }
        .btn-danger { background: #ea4335; color: #fff; }
        .btn-danger:hover { background: #c5331e; }
        .btn:disabled { opacity: 0.5; cursor: not-allowed; }
        .btn input[type="file"] { display: none; }

        /* ステータス */
        .status { padding: 12px; border-radius: 8px; font-size: 13px; margin-bottom: 12px; display: none; }
        .status.success { display: block; background: #e6f4ea; color: #1e7e34; }
        .status.error { display: block; background: #fce8e6; color: #c5221f; }
        .status.info { display: block; background: #e8f0fe; color: #1a73e8; }
        .status.loading { display: block; background: #fef7e0; color: #e37400; }

        /* ファイルリスト */
        .file-list { margin-top: 16px; text-align: left; }
        .file-list h3 { font-size: 14px; color: #555; margin-bottom: 8px; }
        .file-item { background: #f7f8fa; border-radius: 8px; padding: 10px 14px; margin-bottom: 6px; font-size: 13px; color: #333; display: flex; justify-content: space-between; align-items: center; }
        .file-item .time { color: #888; font-size: 11px; }
        .file-item .icon { color: #34a853; margin-right: 6px; }

        /* iPhone最適化 */
        @media (max-width: 480px) {
            .container { padding: 20px; border-radius: 12px; }
            .btn { padding: 10px 20px; font-size: 14px; }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>📷 写真撮影・アップロード</h1>
            <p class="subtitle">検査写真を撮影して共有フォルダに保存します</p>

            <!-- ステータスメッセージ -->
            <asp:Panel ID="statusMessage" runat="server" CssClass="status" Visible="false">
                <asp:Literal ID="statusText" runat="server" />
            </asp:Panel>

            <!-- カメラプレビュー -->
            <div class="camera-area" id="cameraArea">
                <video id="video" autoplay playsinline></video>
                <canvas id="canvas"></canvas>
                <img id="preview" alt="プレビュー" />
            </div>

            <!-- 操作ボタン -->
            <div class="btn-row">
                <button type="button" class="btn btn-primary" id="btnCapture" onclick="capturePhoto()">
                    📸 写真を撮る
                </button>
                <button type="button" class="btn btn-secondary" id="btnSelectFile" onclick="document.getElementById('fileInput').click();">
                    🖼 アルバムから選択
                </button>
                <asp:FileUpload ID="fileInput" runat="server" accept="image/*" style="display:none;" />
                <input type="hidden" id="hiddenImageData" runat="server" />
            </div>

            <div class="btn-row">
                <asp:Button ID="btnUpload" runat="server" Text="💾 アップロード" CssClass="btn btn-success" OnClick="btnUpload_Click" />
                <button type="button" class="btn btn-danger" id="btnRetake" onclick="resetCamera()" style="display:none;">🔄 撮り直す</button>
            </div>

            <!-- 保存済みファイル一覧 -->
            <div class="file-list">
                <h3>📁 保存済み写真</h3>
                <asp:Repeater ID="fileListRepeater" runat="server">
                    <ItemTemplate>
                        <div class="file-item">
                            <span>
                                <span class="icon">✅</span>
                                <%# Eval("FileName") %>
                            </span>
                            <span class="time"><%# Eval("SavedAt") %></span>
                        </div>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="file-item" style="justify-content:center;color:#999;font-size:12px;">
                            まだ写真が保存されていません
                        </div>
                    </EmptyDataTemplate>
                </asp:Repeater>
            </div>
        </div>
    </form>

    <script>
        // === カメラ制御 ===
        var video = document.getElementById('video');
        var canvas = document.getElementById('canvas');
        var preview = document.getElementById('preview');
        var hiddenImageData = document.getElementById('hiddenImageData');
        var btnCapture = document.getElementById('btnCapture');
        var btnRetake = document.getElementById('btnRetake');
        var btnUpload = document.getElementById('btnUpload');
        var cameraArea = document.getElementById('cameraArea');

        var stream = null;

        // カメラを起動
        async function startCamera() {
            try {
                stream = await navigator.mediaDevices.getUserMedia({
                    video: { facingMode: 'environment', width: { ideal: 1280 }, height: { ideal: 720 } },
                    audio: false
                });
                video.srcObject = stream;
                video.style.display = 'block';
                preview.style.display = 'none';
                btnCapture.style.display = '';
                btnRetake.style.display = 'none';
            } catch (err) {
                console.error('カメラ起動エラー:', err);
                // カメラが使えない場合はファイル選択のみ
                video.style.display = 'none';
                cameraArea.innerHTML = '<p style="color:#fff;font-size:14px;">📱 カメラを起動できませんでした。<br/>「アルバムから選択」をお使いください。</p>';
            }
        }

        // 写真を撮影
        function capturePhoto() {
            if (!stream) return;
            var context = canvas.getContext('2d');
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            context.drawImage(video, 0, 0, canvas.width, canvas.height);

            // データURIとして保存
            var dataUrl = canvas.toDataURL('image/jpeg', 0.85);
            hiddenImageData.value = dataUrl;

            // プレビュー表示
            preview.src = dataUrl;
            preview.style.display = 'block';
            video.style.display = 'none';
            btnCapture.style.display = 'none';
            btnRetake.style.display = '';
        }

        // カメラをリセット（撮り直し）
        function resetCamera() {
            preview.src = '';
            preview.style.display = 'none';
            video.style.display = 'block';
            btnCapture.style.display = '';
            btnRetake.style.display = 'none';
            hiddenImageData.value = '';
        }

        // ファイル選択時の処理
        document.getElementById('fileInput').onchange = function (e) {
            var file = e.target.files[0];
            if (!file) return;

            var reader = new FileReader();
            reader.onload = function (event) {
                var dataUrl = event.target.result;
                hiddenImageData.value = dataUrl;

                preview.src = dataUrl;
                preview.style.display = 'block';
                video.style.display = 'none';
                btnCapture.style.display = 'none';
                btnRetake.style.display = '';

                // カメラストリーム停止
                if (stream) {
                    stream.getTracks().forEach(function (track) { track.stop(); });
                    stream = null;
                }
            };
            reader.readAsDataURL(file);
        };

        // アップロード前に画像があるか確認
        btnUpload.addEventListener('click', function (e) {
            if (!hiddenImageData.value) {
                alert('先に写真を撮影または選択してください。');
                e.preventDefault();
            }
        });

        // ページ読み込み時にカメラ起動
        window.addEventListener('load', function () {
            startCamera();
        });

        // ページ遷移時にカメラリソースを解放
        window.addEventListener('beforeunload', function () {
            if (stream) {
                stream.getTracks().forEach(function (track) { track.stop(); });
            }
        });
    </script>
</body>
</html>
