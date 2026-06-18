using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;
using System.Web;

public partial class PhotoUpload : System.Web.UI.Page
{
    /// <summary>
    /// 共有保存先パス（Web.config の AppSettings から取得）
    /// 例: \\192.168.1.100\shared\photos\ または D:\SharedPhotos\
    /// </summary>
    private string SharedPhotoPath
    {
        get
        {
            // 規定値：アプリケーションルート配下の SharedPhotos フォルダ
            string defaultPath = Server.MapPath("~/SharedPhotos");
            return ConfigurationManager.AppSettings["SharedPhotoPath"] ?? defaultPath;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFileList();
        }
    }

    /// <summary>
    /// 共有フォルダから保存済み写真の一覧を取得する
    /// </summary>
    private void LoadFileList()
    {
        var files = new List<PhotoFileInfo>();

        try
        {
            string photoDir = SharedPhotoPath;

            if (Directory.Exists(photoDir))
            {
                var dirInfo = new DirectoryInfo(photoDir);
                foreach (var file in dirInfo.GetFiles("*.jpg"))
                {
                    files.Add(new PhotoFileInfo
                    {
                        FileName = file.Name,
                        SavedAt = file.LastWriteTime.ToString("yyyy/MM/dd HH:mm")
                    });
                }

                // 新しい順に並び替え
                files.Sort((a, b) => string.Compare(b.SavedAt, a.SavedAt, StringComparison.Ordinal));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("ファイル一覧取得エラー: " + ex.Message);
        }

        fileListRepeater.DataSource = files;
        fileListRepeater.DataBind();
    }

    /// <summary>
    /// アップロードボタンクリック時の処理
    /// </summary>
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        string imageData = hiddenImageData.Value;

        if (string.IsNullOrEmpty(imageData))
        {
            ShowStatus("写真を撮影または選択してください。", "error");
            return;
        }

        try
        {
            // 画像のBase64データ部分（data:image/jpeg;base64, 以降）を抽出してファイル保存
            SavePhotoFromBase64(imageData);
            ShowStatus("写真を共有フォルダに保存しました。", "success");

            // hiddenフィールドをクリア
            hiddenImageData.Value = "";

            // ファイル一覧を更新
            LoadFileList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("保存エラー: " + ex.ToString());
            ShowStatus("写真の保存に失敗しました。" + ex.Message, "error");
        }
    }

    /// <summary>
    /// Base64エンコードされた画像データを共有フォルダに保存する
    /// </summary>
    private void SavePhotoFromBase64(string base64ImageData)
    {
        // 共有フォルダのパスを取得
        string photoDir = SharedPhotoPath;

        // フォルダが存在しなければ作成
        if (!Directory.Exists(photoDir))
        {
            Directory.CreateDirectory(photoDir);
        }

        // データURIスキームのプレフィックスを除去
        string base64Data = base64ImageData;
        if (base64Data.Contains(","))
        {
            base64Data = base64Data.Substring(base64Data.IndexOf(",") + 1);
        }

        // Base64文字列をバイト配列に変換
        byte[] imageBytes = Convert.FromBase64String(base64Data);

        // ファイル名を生成（タイムスタンプ＋ランダム文字列）
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string randomSuffix = Path.GetRandomFileName().Replace(".", "").Substring(0, 4);
        string fileName = $"PHOTO_{timestamp}_{randomSuffix}.jpg";
        string filePath = Path.Combine(photoDir, fileName);

        // ファイルに保存
        File.WriteAllBytes(filePath, imageBytes);
    }

    /// <summary>
    /// ステータスメッセージを表示する
    /// </summary>
    private void ShowStatus(string message, string type)
    {
        statusMessage.CssClass = "status " + type;
        statusMessage.Visible = true;
        statusText.Text = message;
    }

    /// <summary>
    /// ファイル情報を保持する内部クラス
    /// </summary>
    public class PhotoFileInfo
    {
        public string FileName { get; set; }
        public string SavedAt { get; set; }
    }
}
