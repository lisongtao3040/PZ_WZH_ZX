<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Inspection.aspx.cs" Inherits="Inspection" ResponseEncoding="UTF-8" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>检查画面 - 拍照共有</title>
    <script src="https://cdn.jsdelivr.net/npm/qrcodejs@1.0.0/qrcode.min.js"></script>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background: #f0f2f5; display: flex; justify-content: center; align-items: center; min-height: 100vh; }
        .container { background: #fff; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.1); padding: 40px; text-align: center; max-width: 500px; width: 90%; }
        h1 { font-size: 24px; color: #1a1a2e; margin-bottom: 8px; }
        .subtitle { color: #666; font-size: 14px; margin-bottom: 24px; }
        #qrcode { display: inline-block; padding: 16px; background: #fff; border: 2px dashed #ddd; border-radius: 8px; margin-bottom: 20px; }
        #qrcode img { display: block; margin: 0 auto; }
        .url-display { background: #f7f8fa; border-radius: 6px; padding: 10px 14px; font-size: 12px; color: #888; word-break: break-all; margin-bottom: 20px; border: 1px solid #eee; }
        .instructions { background: #e8f4fd; border-radius: 8px; padding: 16px; text-align: left; }
        .instructions h3 { font-size: 14px; color: #1a73e8; margin-bottom: 8px; }
        .instructions ol { padding-left: 20px; font-size: 13px; color: #444; line-height: 1.8; }
        .badge { display: inline-block; background: #34a853; color: #fff; font-size: 12px; padding: 4px 12px; border-radius: 20px; margin-bottom: 16px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="badge">📸 拍照共有システム</div>
            <h1>検査画面</h1>
            <p class="subtitle">スマートフォンでQRコードをスキャンして写真を共有してください</p>

            <div id="qrcode"></div>

            <div class="url-display" id="urlDisplay"></div>

            <div class="instructions">
                <h3>操作手順</h3>
                <ol>
                    <li>iPhone のカメラアプリを開く</li>
                    <li>画面上のQRコードをスキャン</li>
                    <li>表示されたリンクをタップ</li>
                    <li>アップロード画面で写真を撮影／選択</li>
                    <li>写真が共有フォルダに保存されます</li>
                </ol>
            </div>
        </div>
    </form>

    <script>
        // ページの完全なURLを取得（PhotoUpload.aspx へのリンク）
        var baseUrl = window.location.protocol + "//" + window.location.host;
        var uploadUrl = baseUrl + "/PhotoUpload.aspx";

        // QRコードを生成
        new QRCode(document.getElementById("qrcode"), {
            text: uploadUrl,
            width: 220,
            height: 220,
            colorDark: "#1a1a2e",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });

        // URLを表示
        document.getElementById("urlDisplay").textContent = "🔗 " + uploadUrl;
    </script>
</body>
</html>
