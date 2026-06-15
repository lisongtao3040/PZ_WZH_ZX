#---------------------------------------------------------- 
#  SendGrid 送信処理
#  Histry
#  2025/07/10    Create     李松涛(大連）
#---------------------------------------------------------- 
#正常ログパス:D:\logs\＋yyyyMMddHHmmssfff_確認対象ID_連番.log

#---------------------------------
#このコマンドは、マルチスレッド環境でリソースの競合を防ぐための手段です。特に、
#複数のプロセスが同時に同じリソースにアクセスすることを防ぐために使用されます。
#---------------------------------
#$global:mutexRenBan = New-Object System.Threading.Mutex($false, "Global\mutexRenBan")
#$global:mutexPwxMail = New-Object System.Threading.Mutex($false, "Global\mutexPwxMail")

#******************************************
#本番時、Falseに設定してください。
$Global:IS_DEBUG = $false
#******************************************
$global:StatusEntries = @()

#共通関数
. "$PSScriptRoot\Common.ps1"

#Initial Parameters
#戻り値
#正常
$RC = 0
#WARNING
#$RC_WARNING=1
#異常
$RC_ERROR = 9

#--------------------------------- 
# PARAMETERS
#---------------------------------
$G_FILE_NAME = [string]$Args[0]
$G_ID = [string]$Args[1]


#--------------------------------- 
# DEBUG 情報
#---------------------------------
#******************************************
#テスト用 本番時削除
#Copy-Item -Path "E:\INI_SET_DIR\bk\lis6_ipt1.txt" -Destination "E:\INI_SET_DIR\input\lis6_ipt1.txt" -Force
#Copy-Item -Path "$PSScriptRoot\bk\zhaot_ipt.txt" -Destination "G:\PS1\input\zhaot_ipt.txt" -Force
#******************************************


#---------------------------------
#現在のパスに共通ps1
#---------------------------------
Add-Type -AssemblyName System.Collections
#. "$PSScriptRoot\FILEIO.ps1"
. "$PSScriptRoot\Logger.ps1"
. "$PSScriptRoot\HulftSendErr.ps1"
. "$PSScriptRoot\ReadIni.ps1"
#. "$PSScriptRoot\Renban.ps1"
. "$PSScriptRoot\Status.ps1"
#---------------------------------


#MWK添付ファイルのTMP保存場所
$G_MAIL_DIR = "$PSScriptRoot\MAIL\"

#--------------------------------- 
# INI変数の初期化
# $G_ConfigIni = @{}
#---------------------------------
$G_ConfigIni = GetConfigIni

#エラーメッセージフォルダ。実行中にエラーが発生した場合、最終的なエラー情報を1行にまとめて記録し、エラー確認を便利にします。
#$HulftDir = $G_ConfigIni['sys']['HulftDir']
#システムの実行用一時フォルダ。HULFTから送られたファイルはここに一時保存されます。
$MoveDir = $G_ConfigIni['sys']['MoveDir']
#送信状況フォルダ。一度の送信で複数行の記録が生成されます。
#$G_INI_STATUS_DIR = $G_ConfigIni['sys']['StatusManageDir']
#SendGrid-APIキー。
$SendGridApiKey = $G_ConfigIni['sys']['SendGridApiKey'] 
#送信成功フォルダ。送信が成功した後、すべてのTEMPファイルはここに保存されます。
$MailSendOKDir = $G_ConfigIni['sys']['MailSendOKDir']
#送信失敗フォルダ。送信に失敗した後、すべてのTEMPファイルはここに保存されます。
$MailSendFailDir = $G_ConfigIni['sys']['MailSendFailDir']
#送信試行回数。
$MailSendTimes = [int]$G_ConfigIni['sys']['MailSendTimes']
#失敗時の再送信までの待機時間（秒）。
$WaitSec = [int]$G_ConfigIni['sys']['WaitSec']
#送信者会社名
$Sender_Company = $G_ConfigIni['sys']['Sender_Company']
#失敗時に管理者に通知メールを送信します。
$HULFTID = $G_ConfigIni['sys']['HULFT-ID']
#Google Log Project name
$GoogleLogPjName = $G_ConfigIni['sys']['GoogleLogPjName']
#Google Log name
$GoogleLogName = $G_ConfigIni['sys']['GoogleLogName']
#エラー時ログの送り先 Cloud loggingのログ名
$GoogleHulftErrLogName = $G_ConfigIni['sys']['GoogleHulftErrName']
#1はBCCを送る。0は送らない。
$BCCMode = $G_ConfigIni['sys']['BCCMode']
#BCCの宛先
$BCCAddr = $G_ConfigIni['sys']['BCCAddr']

#--------------------------------- 
# DEBUG 用
#---------------------------------
if ($Global:IS_DEBUG -eq $true) {
    #エラーメッセージフォルダ。実行中にエラーが発生した場合、最終的なエラー情報を1行にまとめて記録し、エラー確認を便利にします。
    #$HulftDir = $G_ConfigIni['debug']['HulftDir']
    #システムの実行用一時フォルダ。HULFTから送られたファイルはここに一時保存されます。
    $MoveDir = $G_ConfigIni['debug']['MoveDir']
    #送信状況フォルダ。一度の送信で複数行の記録が生成されます。
    #$G_INI_STATUS_DIR = $G_ConfigIni['debug']['StatusManageDir']
    #SendGrid-APIキー。
    $SendGridApiKey = $G_ConfigIni['debug']['SendGridApiKey']
    #送信成功フォルダ。送信が成功した後、すべてのTEMPファイルはここに保存されます。
    $MailSendOKDir = $G_ConfigIni['debug']['MailSendOKDir']
    #送信失敗フォルダ。送信に失敗した後、すべてのTEMPファイルはここに保存されます。
    $MailSendFailDir = $G_ConfigIni['debug']['MailSendFailDir']
    #送信試行回数。
    $MailSendTimes = [int]$G_ConfigIni['debug']['MailSendTimes']
    #失敗時の再送信までの待機時間（秒）。
    $WaitSec = [int]$G_ConfigIni['debug']['WaitSec']
    #送信者会社名
    $Sender_Company = $G_ConfigIni['debug']['Sender_Company']
    #失敗時に管理者に通知メールを送信します。
    $HULFTID = $G_ConfigIni['debug']['HULFT-ID']
    #Google Log Project name
    $GoogleLogPjName = $G_ConfigIni['debug']['GoogleLogPjName']
    #Google Log name
    $GoogleLogName = $G_ConfigIni['debug']['GoogleLogName']
    #エラー時ログの送り先 Cloud loggingのログ名
    $GoogleHulftErrLogName = $G_ConfigIni['debug']['GoogleHulftErrName']
    #1はBCCを送る。0は送らない。
    $BCCMode = $G_ConfigIni['debug']['BCCMode']
    #BCCの宛先
    $BCCAddr = $G_ConfigIni['debug']['BCCAddr']


}

#実行時間を取得する
$start_time = $(Get-Date)
$ymdhmsf = $start_time.ToString("yyyyMMddHHmmssfff")
$ymd = $start_time.ToString("yyyyMMdd")

#INIステータスパスを取得
#$G_INI_STATUS_FILE_PATH = $G_INI_STATUS_DIR + $ymd + "_INIステータス管理.csv"
# $global:StatusEntries  += @(
#     @{ key = "HEADER"; value = "タイトル,フォーマット,MAIL区分,添付ファイル名,送信者会社名," +
#                     "送信者メールアドレス１,送信者メールアドレス２,受信者メールアドレス１," +
#                     "受信者メールアドレス２,備考,処理状態,送信登録日時,送信開始日時," +
#                     "送信終了日時,送信エラー日時,登録日,更新日,実行回数,確認対象ID," +
#                     "ログファイル名" }
# )
   
#--------------------------------- 
# 連番作成
#---------------------------------
# $lockAcquired = $global:mutexRenBan.WaitOne(30000)
# if ($lockAcquired) {
#     try {
#         $G_RENBAN = GetRenban($ymd)
#     } finally {
#         $global:mutexRenBan.ReleaseMutex()
#     }
# } else {
#         LogAndErr_ms $G_log_path $G_err_log_path "E" (" [プロセス]  連番取得できませんでした。")
#         LogAndErr_ms $G_log_path $G_err_log_path "E" (" [プロセス]  連番が取得できないため、プロセスを中止します。")
#         $err_txt = "連番取得できませんでした。"
#         HulftSendErr $HULFT_ERR_PATH "ST010 失敗1" $err_txt
#         $RC = $RC_ERROR   
#         exit $RC
# }


#ログパスを取得
$G_log_dir = "$PSScriptRoot\LOG\"
#総ログファイル名　HulftSendErr用,Status記入用
$G_log_name = $ymdhmsf + "_" + $G_ID + ".log"
#総ログファイルパス
$G_log_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + ".log"
#総エラーログファイルパス
$G_err_log_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + "_err.log"



# 拡張子以外を取得する
$fileNameWithoutExtension = [System.IO.Path]::GetFileNameWithoutExtension($G_FILE_NAME)
# 拡張子を取得する
$fileExtension = [System.IO.Path]::GetExtension($G_FILE_NAME)

$G_input_file_name = $fileNameWithoutExtension + $fileExtension
#移動先ファイル名

#2025-08-22 変更
#$newFileName = $fileNameWithoutExtension + "_" + $ymdhmsf + "_" + $G_ID + $fileExtension
$newFileName = $ymdhmsf + "_" + $G_ID + $fileExtension

#移動先ファイルパス
$newFilePath = $MoveDir + $newFileName
#エラー簡単なログ
#$HULFT_ERR_PATH = $HulftDir + "HULFT_ERR.log"
$HULFT_ERR_PATH = "nouse"
#ＨＲＬＦＴ＿ＥＲＲ．ＬＯＧ

# HulftSendErrのSTATUS作成時、用
$S_Record_TiTle=""
$S_Record_Quality=""
$S_Record_Mail_kbn=""
$S_Record_Add_File_Name=""
$S_Record_Sender_Company=""
$S_Record_Sender_Mail1=""
$S_Record_Sender_Mail2=""
$S_Record_Dest_Mail1=""
$S_Record_Dest_Mail2=""
$S_Record_Bikou=""


#LOG 開始
Logger_txt $G_log_path "***************************************************************************************************************"
Logger_txt $G_log_path ($(Get-Date).ToString("yyyy-MM-dd HH:mm:ss") + "　処理開始")
Logger_txt $G_log_path "***************************************************************************************************************"
Logger_txt $G_log_path "=============================================================================================================="
Logger_txt $G_log_path " "
Logger_mss $G_log_path "I" ("  [処理開始]   ST010　HULFT受信通知処理が開始しました。")
Logger_mss $G_log_path "I" ("  [ファイルチック]  HULFT側ファイル存在チェックが開始しました。")

#----------------------------------------------
# STEP1：HULFT側    ファイル存在チェック
#　存在時
#     ファイル移動
#　以外
#　　　終了
#----------------------------------------------
try {    
    if (Test-Path $G_FILE_NAME) {
        Logger_mss $G_log_path "I" ("  [ファイルチック]  ファイルが存在しました。(ファイルパス=$G_FILE_NAME)")
    }
    else {
        LogAndErr_ms $G_log_path $G_err_log_path "E" (" [ファイルチック]  ファイルが見つかりませんでした。(ファイルパス=$G_FILE_NAME)")
        LogAndErr_ms $G_log_path $G_err_log_path "E" (" [プロセス]        ファイルが無いためプロセスを中止します。")
        $err_txt = "ファイルが見つかりませんでした。"
        HulftSendErr $HULFT_ERR_PATH "ST010 失敗1" $err_txt
        exit $RC_ERROR
    }
}
catch {
    $EXEP = FormatErrMsg(([string]$_.exception))
    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [ファイル存在チェック]  ファイル存在チェックエラーが発生しました。 ファイル名称=" + $ymdhmsf + "_" + $G_ID + ".txt ")
    LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [プロセス]        ファイル存在チェックが失敗したためプロセスを中止します。")

    $err_txt = "ファイルが見つかりませんでした。"
    HulftSendErr $HULFT_ERR_PATH "ST010 失敗2" $err_txt
    exit $RC_ERROR 
}

#-----------------------
# ファイル移動
#-----------------------
Logger_mss $G_log_path "I" ("  [ファイル移動]  ファイルを固定フォルダに移動が開始しました。")
try {
    Move-Item -Path $G_FILE_NAME -Destination $newFilePath -ErrorAction Stop
    Logger_mss $G_log_path "I" ("  [ファイル移動]  ファイル移動処理は正常に終了しました。 固定フォルダパス=$newFilePath")
    Logger_mss $G_log_path "I" ("  [処理終了]   ST010　HULFT受信通知処理が終了しました。")
    Logger_txt $G_log_path "                                                                                                             "
    Logger_txt $G_log_path "=============================================================================================================="
    Logger_txt $G_log_path "                                                                                                             "

}
catch {
    $EXEP = FormatErrMsg(([string]$_.exception))

    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [ファイル移動]  ファイル移動エラーが発生しました。 ファイル名称=" + $ymdhmsf + "_" + $G_ID + ".txt ")
    LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [プロセス]        ファイル移動が失敗したためプロセスを中止します。")

    $err_txt = "ファイル移動エラーが発生しました。"
    HulftSendErr $HULFT_ERR_PATH "ST010 失敗" $err_txt
    exit $RC_ERROR
}

#----------------------------------------------
# STEP2：HULFT側    ファイル変換処理
#----------------------------------------------
#　1-1
#   For ファイルの行
#　　　メール１作成
#　　　　　　メールTitle
#　　　　　　メール本文
#           メール添付ファイル
#　　　メール２作成
#　　　　　　メールTitle
#　　　　　　メール本文
#           メール添付ファイル
#   　　...
#　1-2
#   For メール１、メール２、...
#　　　メールのチェック実施
#----------------------------------------------

#-----------------------
# ファイル変換
#-----------------------
Logger_mss $G_log_path "I" (" [処理開始]   ST020　ファイル変換処理が開始しました。")

#-----------------------
# メール内容作成
#-----------------------

#-----------------------
# クラス定義
# MailObj
#-----------------------
class MailObj {
    [string[]]$csv1
    [System.Collections.Generic.List[string]]$mailTxtArr = [System.Collections.Generic.List[string]]::new()
    [System.Collections.Generic.List[string]]$tenpuArr = [System.Collections.Generic.List[string]]::new() 
    [string] $emailJson
    [byte[]] $utf8Bytes
    [string] $newMWKFileName
    [string] $newFileNameRenban
    [string] $jsonFileName
    [string] $statusFileName
    [bool] $IsMailSendOk
    [string] $baseStatus

    #-----------------------
    # 変換定義より変数を定義
    #-----------------------
    [string] $S_Record_Server_Kbn = "" #CSV #サーバ区分
    [string] $S_Record_Report_ID = "" #CSV #帳票ＩＤ
    [string] $S_Record_SendTime_Kbn = "" #CSV #送信時期
    [string] $S_Record_SendTime = "" #CSV #送信時刻
    [string] $S_Record_Quality = "" #CSV #品質
    [string] $S_Record_Cover_Sheet_Kbn = "" #CSV #カバーシート有無
    [string] $S_Record_PageHeader_kbn = "" #CSV #ページヘッダ指定
    [string] $S_Record_Mail_kbn = "" #CSV #MAIL区分
    [string] $S_Record_Dest_FAX1 = "" #CSV #受信者ＦＡＸＮＯ１
    [string] $S_Record_Dest_FAX2 = "" #CSV #受信者ＦＡＸＮＯ２
    [string] $S_Record_Dest_Company = "" #CSV #受信者会社名
    [string] $S_Record_Dest_Section1 = "" #CSV #受信者所属名1
    [string] $S_Record_Dest_Name = "" #CSV #受信者氏名
    [string] $S_Record_Sender_Section1 = "" #CSV #送信者所属名1
    [string] $S_Record_Sender_Name = "" #CSV #送信者氏名
    [string] $S_Record_Sender_Tel1 = "" #CSV #送信者ＴＥＬＮＯ１
    [string] $S_Record_SearchKey = "" #CSV #検索キー
    [string] $S_Record_Bikou = "" #CSV #備考

    #以下Header情報無し
    [string] $S_Record_TiTle = "" #タイトル
    [string] $S_Record_Cover_Sheet_Nm = "" #カバーシート名
    [string] $S_Record_Printe_Name = "" #プリンタ名
    [string] $S_Record_Add_File_Name = "" #添付ファイル名
    [string] $S_Record_Sender_Company = "" #送信者会社名
    [string] $S_Record_Sender_Section2 = "" #送信者所属名2
    [string] $S_Record_Sender_position = "" #送信者役職名
    [string] $S_Record_Sender_FAX1 = "" #送信者ＦＡＸＮＯ１
    [string] $S_Record_Sender_FAX2 = "" #送信者ＦＡＸＮＯ２
    [string] $S_Record_Sender_Tel2 = "" #送信者ＴＥＬＮＯ２
    [string] $S_Record_Sender_Mail1 = "" #送信者メールアドレス１
    [string] $S_Record_Sender_Mail2 = "" #送信者メールアドレス２
    [string] $S_Record_Sender_YubinNo = "" #送信者郵便番号
    [string] $S_Record_Sender_Address = "" #送信者住所
    [string] $S_Record_Dest_Section2 = "" #受信者所属名2
    [string] $S_Record_Dest_position = "" #受信者役職名
    [string] $S_Record_Dest_Tel1 = "" #受信者ＴＥＬＮＯ１
    [string] $S_Record_Dest_Tel2 = "" #受信者ＴＥＬＮＯ２
    [string] $S_Record_Dest_Mail1 = "" #受信者メールアドレス１
    [string] $S_Record_Dest_Mail2 = "" #受信者メールアドレス２
    [string] $S_Record_Dest_YubinNo = "" #受信者郵便番号
    [string] $S_Record_Dest_Address = "" #受信者住所

}

$mailList = New-Object System.Collections.Generic.List[MailObj]

$NG_SUU = 0
#-----------------------
# 変換定義内容取得
#-----------------------
$kms = @()
try {
    # ファイルを行ごとに読み込む
    # 1行目をスキップし、2行目からファイルデータを読み込む
    Get-Content -Path "$PSScriptRoot\変換定義.ini" | Select-Object -Skip 1 | ForEach-Object {
        # 現在の行のテキストを取得
        $line = $_

        # 現在の行をカンマで配列に分割する
        $fields = $line -split ','

        # 分割された配列を二次元配列に追加する
        $kms += , $fields
    }
}
catch {
    $EXEP = FormatErrMsg(([string]$_.exception))
    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [変換定義データ取得]  変換定義データ取得エラーが発生しました。 ファイル名称=" + $ymdhmsf + "_" + $G_ID + ".txt ")
    LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [プロセス]        変換定義データ取得が失敗したためプロセスを中止します。")
    $err_txt = "ファイル変換エラーが発生しました。"
    HulftSendErr $HULFT_ERR_PATH "ST020 失敗" $err_txt
    exit $RC_ERROR
}

Logger_mss $G_log_path "I" ("  [ヘッダ情報] ヘッダ情報処理が開始しました。")


#添付あり？
$blnStartTenpu = $false
#-----------------------
# TXT ファイル内容取得、メールファイルを作成する
#-----------------------
try {
    # UTF-8 チェック Shift-JIS ？
    # if(Is-Utf8File -filePath $newFilePath ){
    #     Write-Output "UTF-8"
    #     $content = [System.IO.File]::ReadAllLines($newFilePath)
        
    # }else{
    #     Write-Output "JIS"
    #     $content = [System.IO.File]::ReadAllLines($newFilePath, [System.Text.Encoding]::GetEncoding("shift_jis"))
    # }

    #現状全部Inputファイルはshift_jisです
    $content = [System.IO.File]::ReadAllLines($newFilePath, [System.Text.Encoding]::GetEncoding("shift_jis"))

    #添付ファイルのバイト数
    $TenpuByteCount = 0
    #メールTXTのバイト数
    $MailByteCount = 0
    #foreachのIdx
    $index = 0    

    # ファイルの行
    foreach ($line in $content) {  

        $fr = $line[0]
        #ファイルの1行目の先頭は'1'ではなく
        if ($index -eq 0 -and $fr -ne '1') {
            $err_txt = " [ヘッダ情報] ヘッダー区分が空白か、桁数を超えています。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" (" [ヘッダ情報] ヘッダー区分が空白か、桁数を超えています。")
            LogAndErr_ms $G_log_path $G_err_log_path "E" (" [ヘッダ情報] ヘッダ情報処理が失敗したためプロセスを中止します。")
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗2" $err_txt
            exit $RC_ERROR
        }
     
        #1行目は１の場合、メールOBJを作成する
        if ($fr -eq '1') {
            $emptyMailObj = [MailObj]::new()
            $mailList.Add($emptyMailObj)
            $blnStartTenpu = $false
            $TenpuByteCount = 0
            $MailByteCount = 0
        }

          
        # 最初の文字が'1'であるかどうかを確認する
        if ($fr -eq '1') {           
            # 行の先頭の'1'を削除する
            $line = $line.Substring(1)
            # 1の場合は、行をカンマで区切って配列に格納する
            $mailList[($mailList.Count - 1)].csv1 = $line -split ','

            # ヘーダデータと変換定義配列を比較する、項目数　が一致しない場合はエラーを表示する
            if ($mailList[($mailList.Count - 1)].csv1.Length -eq $kms.Length + 1) {
                Logger_mss $G_log_path "I" ("[ヘッダ情報]ファイルと【INI変換定義】の項目数が合います。")
            }
            else {
                LogAndErr_ms $G_log_path $G_err_log_path "E" (" [ヘッダ情報] ファイルと【INI変換定義】の項目数が合いません。")
                LogAndErr_ms $G_log_path $G_err_log_path "E" (" [ヘッダ情報] ヘッダ情報処理が失敗したためプロセスを中止します。")
                $err_txt = "ファイル変換エラーが発生しました。"
                HulftSendErr $HULFT_ERR_PATH "ST020 失敗1" $err_txt
                exit $RC_ERROR
            }

            # ヘッダ区分
            $Hedda_Kbn = [string]($mailList[($mailList.Count - 1)].csv1[0])
            $Hedda_Kbn = $Hedda_Kbn.Trim('"')
            # 文字列が空か長さが2でないかを判断する
            if ([string]::IsNullOrEmpty($Hedda_Kbn) -or $Hedda_Kbn.Length -ne 2) {
                $err_txt = " [ヘッダ情報] ヘッダー区分が空白か、桁数を超えています。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" (" [ヘッダ情報] ヘッダー区分が空白か、桁数を超えています。")
                LogAndErr_ms $G_log_path $G_err_log_path "E" (" [ヘッダ情報] ヘッダ情報処理が失敗したためプロセスを中止します。")
                HulftSendErr $HULFT_ERR_PATH "ST020 失敗2" $err_txt
                exit $RC_ERROR
            }
            else {
                Logger_mss $G_log_path "I" (" [ヘッダ情報] 文字列有効，ヘッダー区分:01（メール）")
            }

        }
        elseif ($fr -eq '2') {
            $line = $line.Substring(1)
            if ($blnStartTenpu -eq $true) {
                $mailList[($mailList.Count - 1)].tenpuArr.Add($line)
                #添付ファイルサイズ
                #$TenpuByteCount += [System.Text.Encoding]::UTF8.GetByteCount($line.ToString())
                $TenpuByteCount += [System.Text.Encoding]::GetEncoding("shift-jis").GetByteCount($line.ToString())
                if ($TenpuByteCount -gt 10MB) {
                    $err_txt = " [添付ファイル]  添付ファイルサイズは 10 MB を超えています。"
                    LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                    HulftSendErr $HULFT_ERR_PATH "ST020 失敗" $err_txt
                    exit $RC_ERROR
                }
            }
            else {
                $mailList[($mailList.Count - 1)].mailTxtArr.Add($line)
                #メールファイル
                #$MailbyteCount += [System.Text.Encoding]::UTF8.GetByteCount($line.ToString())
                $MailbyteCount += [System.Text.Encoding]::GetEncoding("shift-jis").GetByteCount($line.ToString())
                if (($MailbyteCount + $TenpuByteCount) -gt 20MB) {
                    $err_txt = "  [メールファイル]  ファイルサイズは 20 MB を超えています。"
                    LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                    HulftSendErr $HULFT_ERR_PATH "ST020 失敗" $err_txt
                    exit $RC_ERROR
                }
            }
        }
        elseif ($fr -eq '8') {
            if ($blnStartTenpu -eq $true) {
                $blnStartTenpu = $false   
            }
            else {
                $blnStartTenpu = $true   
            }
        }
        elseif ($fr -eq '9') {
            #と既存違う点、既存は処理無し
            break
        }
        else {
            #その他の場合、無視です
            # # それ以外の場合は、行をそのまま処理する
            # $err_txt = "[フォーマット]  ファイルフォーマットが不正です。"
            # LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
            # LogAndErr_ms $G_log_path $G_err_log_path "E" " [フォーマット]  ファイルフォーマットが不正ためプロセスを中止します。"
            # HulftSendErr $HULFT_ERR_PATH "ST020 失敗3" $err_txt
            # exit $RC_ERROR       
        }

        $index++
    }
}
catch {
    $EXEP = FormatErrMsg(([string]$_.exception))
    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [メール情報処理]  メール情報処理エラーが発生しました。 ファイル名称=" + $ymdhmsf + "_" + $G_ID + ".txt ")
    LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
    LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [プロセス]        メール情報処理が失敗したためプロセスを中止します。")
    $err_txt = "メール情報処理エラーが発生しました。"
    HulftSendErr $HULFT_ERR_PATH "ST020 失敗" $err_txt
    exit $RC_ERROR
}

Logger_mss $G_log_path "I" ("  [ヘッダ情報] ヘッダ情報処理が終了しました。")
Logger_txt $G_log_path "=============================================================================================================="
  
#-----------------------
# FOR　メールS情報、　チェック...
#-----------------------
$index = 0
#$IsMailSendOk = $true
$intRenban = 1

# 連番ログファイルパス
$G_log_renban_path = ""
# 連番ERRログファイルパス
$G_err_log_renban_path = ""
# 連番ログファイル名
$G_log_renban_name = ""

foreach ($tmpMailObj in $mailList) {
    #連番
    $G_RENBAN = $intRenban.ToString("D3") 

    #2025-08-22 変更
    # #添付ファイル名
    # $newMWKFileName = $fileNameWithoutExtension + "_" + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".MWK"
    # #移動先ファイル名（連番）
    # $newFileNameRenban = $fileNameWithoutExtension + "_" + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + $fileExtension
    #添付ファイル名
    $newMWKFileName = $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".MWK"
    #移動先ファイル名（連番）
    $newFileNameRenban = $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + $fileExtension


    #添付ファイル名
    $mailList[$index].newMWKFileName = $newMWKFileName
    #移動先ファイル名（連番）
    $mailList[$index].newFileNameRenban = $newFileNameRenban
    # 連番ログファイル名
    $G_log_renban_name = $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".log"
    # 連番ログファイルパス
    $G_log_renban_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".log"
    # 連番ERRログファイルパス
    $G_err_log_renban_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + "_err.log"

    #連番＋＋
    $intRenban++

    try {
        Logger_mss $G_log_path "I" ("  連番： " + $G_RENBAN)
        Logger_mss $G_log_renban_path "I" ("  連番： " + $G_RENBAN)
       
        #現状連番のみ利用を削除
        # if ($index -gt 0) {
        # }
        #子連番（未使用）
        #$idx_nm = ""
        # if ($mailList.Count -gt 0) {
        #     $idx_nm = "_" + ($index + 1)
        # }
        #$newMWKFileName = $fileNameWithoutExtension + "_" + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + $idx_nm + ".MWK"

        #メールヘーダ
        $csv1 = $tmpMailObj.csv1
        #メール本文
        $mailTxtArr = $tmpMailObj.mailTxtArr
        #添付ファイル
        $tenpuArr = $tmpMailObj.tenpuArr

        #添付情報内容あり場合、添付ファイルを作成する（MWK）
        try {
            if ($tenpuArr.Length -gt 0) {
                Logger_mss $G_log_path "I" (" [添付情報]   添付ファイル処理が開始しました。")
                Logger_mss $G_log_renban_path "I" (" [添付情報]   添付ファイル処理が開始しました。")
                try {
                    [Io.file]::AppendAllText(($G_MAIL_DIR + $newMWKFileName), ($tenpuArr -join [System.Environment]::NewLine), [text.encoding]::Unicode)
                }
                catch {
                    $EXEP = FormatErrMsg(([string]$_.exception))
                    $err_txt = "[添付情報]   添付ファイルを生成失敗しました。"

                    LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                    LogAndErr_ms $G_log_path $G_err_log_path "E" "[添付情報]   添付ファイル処理失敗によりプロセスを中止しています。"
                    LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP

                    LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
                    LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[添付情報]   添付ファイル処理失敗によりプロセスを中止しています。"
                    LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP             
                    
                    HulftSendErr $HULFT_ERR_PATH "ST020 失敗4" $err_txt

                    exit $RC_ERROR
                }
                Logger_mss $G_log_path "I" (" [添付情報]   添付ファイル処理が終了しました。")
                Logger_mss $G_log_renban_path "I" (" [添付情報]   添付ファイル処理が終了しました。")

            }
        }
        catch {

            $EXEP = FormatErrMsg(([string]$_.exception))

            LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [添付情報]   添付ファイル処理エラーが発生しました。 ファイル名称=" + $ymdhmsf + "_" + $G_ID + ".txt ")
            LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
            LogAndErr_ms $G_log_path $G_err_log_path "E" ("  [プロセス]        添付ファイル処理が失敗したためプロセスを中止します。")

            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" ("  [添付情報]   添付ファイル処理エラーが発生しました。 ファイル名称=" + $ymdhmsf + "_" + $G_ID + ".txt ")
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" ("  [プロセス]        添付ファイル処理が失敗したためプロセスを中止します。")

            $err_txt = "添付ファイル処理エラーが発生しました。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗" $err_txt

            exit $RC_ERROR
        }






        # 二次元配列の印刷（行ごとに列ごとに出力）
        #Write-Host "======= 二次元配列出力 ======="
        for ($i = 0; $i -lt $kms.Count; $i++) {
            $row = $kms[$i]

            $F_NO = $row[0] -as [int]
            $KNO = $row[1] -as [int]

            if ($KNO -lt $csv1.Length) {

                $item = $csv1[$KNO]
            }
            else {

                $item = ""
            }

            $z = ""

            if ($item.Length -eq 2) {
                $z = ""
            }
            else {
                $z = $item.Trim().Trim('"')
            }


            # 1: S_Record.TiTle            :=z;             //タイトル
            switch ($F_NO) {
                1	{ $mailList[$index].S_Record_Server_Kbn = $z }             #サーバ区分
                2	{ $mailList[$index].S_Record_Report_ID = $z }             #帳票ＩＤ
                3	{ $mailList[$index].S_Record_SendTime_Kbn = $z }             #送信時期
                4	{ $mailList[$index].S_Record_SendTime = $z }             #送信時刻
                5	{ $mailList[$index].S_Record_Quality = $z }             #品質
                6	{ $mailList[$index].S_Record_Cover_Sheet_Kbn = $z }             #カバーシート有無
                7	{ $mailList[$index].S_Record_PageHeader_kbn = $z }             #ページヘッダ指定
                8	{ $mailList[$index].S_Record_Mail_kbn = $z }             #MAIL区分
                9	{ $mailList[$index].S_Record_Dest_FAX1 = $z }             #受信者ＦＡＸＮＯ１
                10	{ $mailList[$index].S_Record_Dest_FAX2 = $z }             #受信者ＦＡＸＮＯ２
                11	{ $mailList[$index].S_Record_Dest_Company = $z }             #受信者会社名
                12	{ $mailList[$index].S_Record_Dest_Section1 = $z }             #受信者所属名1
                13	{ $mailList[$index].S_Record_Dest_Name = $z }             #受信者氏名
                14	{ $mailList[$index].S_Record_Sender_Section1 = $z }             #送信者所属名1
                15	{ $mailList[$index].S_Record_Sender_Name = $z }             #送信者氏名
                16	{ $mailList[$index].S_Record_Sender_Tel1 = $z }             #送信者ＴＥＬＮＯ１
                17	{ $mailList[$index].S_Record_SearchKey = $z }             #検索キー
                18	{ $mailList[$index].S_Record_Bikou = $z }             #備考
            }
        }

        #-----------------------
        #情報再設定
        #-----------------------
        #受信者メールアドレス１
        if (($mailList[$index].S_Record_Dest_Mail1.Trim()) -eq "") {
            $mailList[$index].S_Record_Dest_Mail1 = $mailList[$index].S_Record_Dest_FAX1
        }
        #/受信者メールアドレス２
        if (($mailList[$index].S_Record_Dest_Mail2.Trim()) -eq "") {
            $mailList[$index].S_Record_Dest_Mail2 = $mailList[$index].S_Record_Dest_FAX2
        }
        #/送信者メールアドレス1
        if (($mailList[$index].S_Record_Sender_Mail1.Trim()) -eq "") {
            $mailList[$index].S_Record_Sender_Mail1 = $mailList[$index].S_Record_Sender_Name #送信者氏名をセット
        }
        #タイトル""の時。
        if (($mailList[$index].S_Record_TiTle.Trim()) -eq "") {
            $mailList[$index].S_Record_TiTle = $mailList[$index].S_Record_Dest_Company #受信者会社名をセット
        }
        #添付ファイル名""の時。
        if (($mailList[$index].S_Record_Add_File_Name.Trim()) -eq "") {
            $mailList[$index].S_Record_Add_File_Name = $mailList[$index].S_Record_Bikou #受信者会社名をセット
        }

        if (($mailList[$index].S_Record_Sender_Company.Trim()) -eq "") {
            $mailList[$index].S_Record_Sender_Company = $Sender_Company #受信者会社名をセット
        }


        $S_Record_TiTle = $mailList[$index].S_Record_TiTle
        $S_Record_Quality = $mailList[$index].S_Record_Quality
        $S_Record_Mail_kbn = $mailList[$index].S_Record_Mail_kbn
        $S_Record_Add_File_Name = $mailList[$index].S_Record_Add_File_Name
        $S_Record_Sender_Company = $mailList[$index].S_Record_Sender_Company
        $S_Record_Sender_Mail1 = $mailList[$index].S_Record_Sender_Mail1
        $S_Record_Sender_Mail2 = $mailList[$index].S_Record_Sender_Mail2
        $S_Record_Dest_Mail1 = $mailList[$index].S_Record_Dest_Mail1
        $S_Record_Dest_Mail2 = $mailList[$index].S_Record_Dest_Mail2
        $S_Record_Bikou = $mailList[$index].S_Record_Bikou



        # Write-Output $flds[10]

        #-----------------------
        #STATUS登録　MS
        #-----------------------
        $s_fields = @(
            $mailList[$index].S_Record_TiTle,
            $mailList[$index].S_Record_Quality,
            $mailList[$index].S_Record_Mail_kbn,
            $mailList[$index].S_Record_Add_File_Name,
            $mailList[$index].S_Record_Sender_Company,
            $mailList[$index].S_Record_Sender_Mail1,
            $mailList[$index].S_Record_Sender_Mail2,
            $mailList[$index].S_Record_Dest_Mail1,
            $mailList[$index].S_Record_Dest_Mail2,
            $mailList[$index].S_Record_Bikou,
            "準備送信",
            $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
            $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
            "",
            "",
            $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
            $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
            "0",
            $G_ID,
            $G_log_renban_name
        )

        # 使用 -join 运算符连接字段
        $recordLine = ($s_fields -join ",")
        $mailList[$index].baseStatus=$recordLine+","+$G_FILE_NAME

        $global:StatusEntries += @{ key = "MS" + $global:StatusEntries.Length; value = $recordLine }

        # Write-Output $S_Record_TiTle
        # Write-Output "準備送信"

        #添付ファイル名を設定する（送信ファイルの添付ファイル表示名）
        $newMWKFileName2 = ""
        if (($mailList[$index].S_Record_Add_File_Name.Trim()) -eq "") {
            #品質は２の場合は、ファイル名を設定しない
            if ($mailList[$index].S_Record_Quality -eq "2") {
                $newMWKFileName2 = $mailList[$index].S_Record_Report_ID + ".TXT"
            }
            else {
                $newMWKFileName2 = $mailList[$index].S_Record_Report_ID + ".CSV"       
            }
        }
        else {
            $newMWKFileName2 = $mailList[$index].S_Record_Add_File_Name
        }

        #添付情報内容あり場合
        if ($tenpuArr.Length -gt 0) {
            Logger_mss $G_log_path "I" (" [添付情報]   添付ファイルを正常に生成しました。 ファイル名称=" + $newMWKFileName2 + " ")
        }


        Logger_mss $G_log_path "I" ("  [処理開始]   ST021　メールチェックが開始しました。 ")
        #メールタイトルチェック
        if (($mailList[$index].S_Record_TiTle.Trim()) -eq "") {

            LogAndErr_ms $G_log_path $G_err_log_path "E" "タイトルに値がありません。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "タイトルに値がありません。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"

            $err_txt = "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗5" $err_txt
            exit $RC_ERROR
        }
        #帳票ＩＤチェック
        if (($mailList[$index].S_Record_Report_ID.Trim()) -eq "") {
            LogAndErr_ms $G_log_path $G_err_log_path "E" "帳票ＩＤに値がありません。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "帳票ＩＤに値がありません。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"

            $err_txt = "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗5" $err_txt
            exit $RC_ERROR
        }
        #品質チェック
        if (($mailList[$index].S_Record_Quality.Trim()) -ne '2' -and ($mailList[$index].S_Record_Quality.Trim()) -ne '3') {
            LogAndErr_ms $G_log_path $G_err_log_path "E" "品質に値に誤りがあります。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "品質に値に誤りがあります。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"

            $err_txt = "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗5" $err_txt
            exit $RC_ERROR
        }

        #MAIL区分チェック
        if (($mailList[$index].S_Record_Mail_kbn.Trim()) -eq "") {
            LogAndErr_ms $G_log_path $G_err_log_path "E" "MAIL区分に値がありません。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "MAIL区分に値がありません。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"

            $err_txt = "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗5" $err_txt
            exit $RC_ERROR 
        }

        #MAIL区分チェック(メールのみ)
        if (($mailList[$index].S_Record_Mail_kbn.Trim()) -ne "1") {
            LogAndErr_ms $G_log_path $G_err_log_path "E" "MAIL区分に1以外の値が入力されています。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "MAIL区分に1以外の値が入力されています。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            $err_txt = "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗5" $err_txt
            exit $RC_ERROR
        }

        #送信者メールアドレス１チェック
        if (($mailList[$index].S_Record_Sender_Mail1.Trim()) -eq "") {
            LogAndErr_ms $G_log_path $G_err_log_path "E" "送信者メールアドレス１に値がありません。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "送信者メールアドレス１に値がありません。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            $err_txt = "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗5" $err_txt
            exit $RC_ERROR
        }

        #受信者メールアドレス１チェック
        if (($mailList[$index].S_Record_Dest_Mail1.Trim()) -eq "") {
            LogAndErr_ms $G_log_path $G_err_log_path "E" "受信者メールアドレス１に値がありません。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "受信者メールアドレス１に値がありません。"
            LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            $err_txt = "[プロセス]   ヘッダ情報処理が失敗したためプロセスを中止します。"
            HulftSendErr $HULFT_ERR_PATH "ST020 失敗5" $err_txt
            exit $RC_ERROR
        }

        Logger_mss $G_log_path "I" ("  [処理終了]   ST021　メールチェックが終了しました。 ")

        #メール内容作成
        $sbMailTxt = New-Object System.Text.StringBuilder
        $mailTxtArr | ForEach-Object {
            [void]$sbMailTxt.AppendLine($_)
        }
        #$value = $sbMailTxt.ToString()
        $toArray = @()
        if (![string]::IsNullOrWhiteSpace($mailList[$index].S_Record_Dest_Mail1.Trim())) {

            $toEmails = $mailList[$index].S_Record_Dest_Mail1 -split ';' | ForEach-Object { $_.Trim() }

            foreach ($email1 in $toEmails) {
                if (![string]::IsNullOrWhiteSpace($email1)) {
                    # 确保电子邮件地址不为空
                    $toArray += @{
                        email = $email1
                    }
                }
            }
        }

        # $toArray = $S_Record_Dest_Mail1.Split(';').Trim() | Where-Object { $_ } | ForEach-Object {
        #     @{ "email" = $_ }
        # }


        #CCメールアドレス１チェック
        $ccArray = @()
        if (![string]::IsNullOrWhiteSpace($mailList[$index].S_Record_Dest_Mail2.Trim())) {

            $ccEmails = $mailList[$index].S_Record_Dest_Mail2 -split ';' | ForEach-Object { $_.Trim() }

            foreach ($email2 in $ccEmails) {
                if (![string]::IsNullOrWhiteSpace($email2)) {
                    # 确保电子邮件地址不为空
                    $ccArray += @{
                        email = $email2
                    }
                    #Write-Output("CC", $email2)
                }
            }
        }
        if ($ccArray.Count -eq 0) {
            $ccArray = $null
        }



        $bccArray = @()
        if ($BCCMode -eq 1) {
            if (![string]::IsNullOrWhiteSpace($BCCAddr.Trim())) {
                $bccEmails = $BCCAddr -split ';' | ForEach-Object { $_.Trim() }
                foreach ($email3 in $bccEmails) {
                    if (![string]::IsNullOrWhiteSpace($email3)) {
                        # 确保电子邮件地址不为空
                        $bccArray += @{
                            email = $email3
                        }
                        Write-Output("BCC", $email3)
                    }
                }
            }
        }

        if ($bccArray.Count -eq 0) {
            $bccArray = $null
        }

        # メール内容を定義する
        $email = @{
            "personalizations" = @(
                @{
                    "to" = @(@{ email = $mailList[$index].S_Record_Dest_Mail1 })
                    "cc" = $ccArray
                    "bcc" = $bccArray                    
                }
            )

            from             = @{
                email = $mailList[$index].S_Record_Sender_Mail1
            }
            subject          = $mailList[$index].S_Record_TiTle


            content          = @(
                @{
                    type  = "text/plain"
                    value = $sbMailTxt.ToString()
                }
            )

        }

        Logger_mss $G_log_path "I" ("  [メールファイル]   送信データの取得に成功しました。 転換ファイル：" + $newFilePath + " ")
        Logger_mss $G_log_renban_path "I" ("  [メールファイル]   送信データの取得に成功しました。 転換ファイル：" + $newFilePath + " ")



        #添付情報内容あり場合,添付ファイルをShiftを作成
        if ($tenpuArr.Length -gt 0) {

            $originalContent = [System.IO.File]::ReadAllText($G_MAIL_DIR + $newMWKFileName, [System.Text.Encoding]::GetEncoding("shift_jis"))
            $shiftJISBytes = [System.Text.Encoding]::GetEncoding("shift_jis").GetBytes($originalContent)
            $base64Content = [Convert]::ToBase64String($shiftJISBytes)
            $email.attachments = @(
                @{
                    content     = $base64Content
                    filename    = $newMWKFileName2
                    type        = "text/plain"
                    disposition = "attachment"
                }
            )
        }

        # メール内容をJSON形式に変換
        #$emailJson = $email | ConvertTo-Json -Depth 4
        $emailJson = $email | ConvertTo-Json -Depth 10
        $utf8Bytes = [System.Text.Encoding]::UTF8.GetBytes($emailJson)
        #$utf8Bytes = [System.Text.Encoding]::GetEncoding("shift-jis").GetBytes($emailJson)

        $mailList[$index].emailJson = $emailJson
        $mailList[$index].utf8Bytes = $utf8Bytes

        #Logger_txt $G_log_path $emailJson
        Logger_txt $G_log_path "=============================================================================================================="
        Logger_txt $G_log_renban_path "=============================================================================================================="
   
    }
    catch {
        $EXEP = [string]$_.exception
        $EXEP = $EXEP.Replace(" ", "_")
        $EXEP = $EXEP.Replace(",", "_")
        $EXEP = [string]::join("_", $EXEP)
        $err_txt = "[プロセス]   情報処理が失敗したためプロセスを中止します。"
        LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
        LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP

        LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
        LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP

        HulftSendErr $HULFT_ERR_PATH "ST020" $err_txt
        exit $RC_ERROR
    }
    $index++
}

Logger_mss $G_log_path "I" (" [処理終了]   ST020　ファイル変換処理が終了しました。")
Logger_txt $G_log_path "                                                                                                             "
Logger_txt $G_log_path "=============================================================================================================="
Logger_txt $G_log_path "                                                                                                             "



#-----------------------
# FOR　メールS情報、　メール送信...
#-----------------------

#送信SendGrid
$IsAllMailSendOk = $true
$index = 0
$intRenban = 1
#メール送信回数
$statusTimes = 0

foreach ($tmpMailObj in $mailList) {

    #連番
    $G_RENBAN = $intRenban.ToString("D3")
    # 連番ログファイルパス
    $G_log_renban_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".log"
    # 連番ログファイル名
    $G_log_renban_name = $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".log"
    # 連番ERRログファイルパス
    $G_err_log_renban_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + "_err.log"
    # メール送信JSON
    $jsonFileName = $G_MAIL_DIR + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".json"
    #status
    $statusFileName = $G_MAIL_DIR + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".status"

    $mailList[$index].jsonFileName = $jsonFileName
    $mailList[$index].statusFileName = $statusFileName

    # SendGrid API URLを定義する
    $sendGridApiUrl = "https://api.sendgrid.com/v3/mail/send"
    #$sendGridApiUrl = "https://api.sendgrid.com/v3/suppression/bounces?limit=100"

    # HTTP請求ヘッダの定義
    $headers = @{
        "Authorization" = "Bearer $SendGridApiKey"
        "Content-Type"  = "application/json"
    }

    $utf8Bytes = $mailList[$index].utf8Bytes


    Logger_mss $G_log_path "I" ("  連番： " + $G_RENBAN)
    Logger_mss $G_log_renban_path "I" ("  連番： " + $G_RENBAN)

    #-----------------------
    # 送信
    #-----------------------
    for ($i = 1; $i -le $MailSendTimes; $i++) {
        try {

            $statusTimes = $i

            if ($i -eq 1) {
                # メール送信JSON 作成
                $emailJson = $mailList[$index].emailJson
                $emailJson | Out-File -FilePath $jsonFileName -Encoding UTF8 -ErrorAction Stop    

                $emailStatus = $mailList[$index].baseStatus
                $emailStatus | Out-File -FilePath $statusFileName -Encoding UTF8 -ErrorAction Stop
            }

            Logger_mss $G_log_path "I" ("  [送信処理]  ST030 メール送信は開始しました。 ")
            Logger_mss $G_log_renban_path "I" ("  [送信処理]  ST030 メール送信は開始しました。 ")

            $response = Invoke-RestMethod -Uri $sendGridApiUrl -Method Post -Headers $headers -Body $utf8Bytes -ErrorAction Stop

            # Write-Host "Status Code: $($response.StatusCode)"
            # Write-Host "Response Content:"
            # $response.Content | ConvertFrom-Json | Format-List
            $mailList[$index].IsMailSendOk = $true
             
            Logger_mss $G_log_path "I" ("  [送信処理]  ST030 メール送信は成功しました。 ")
            Logger_mss $G_log_renban_path "I" ("  [送信処理]  ST030 メール送信は成功しました。 ")

            Logger_txt $G_log_path "                                                                                                             "
            
            # レスポンスを出力する
            Logger_txt $G_log_path "=============================================================================================================="
            Logger_txt $G_log_path ("                                   *** 本回処理は、正常終了しました。 ***                                     ")
            Logger_txt $G_log_path "=============================================================================================================="

            Logger_txt $G_log_renban_path "=============================================================================================================="
            Logger_txt $G_log_renban_path ("                                   *** 本回処理は、正常終了しました。 ***                                     ")
            Logger_txt $G_log_renban_path "=============================================================================================================="


            $s_fields = @(
                $mailList[$index].S_Record_TiTle,
                $mailList[$index].S_Record_Quality,
                $mailList[$index].S_Record_Mail_kbn,
                $mailList[$index].S_Record_Add_File_Name,
                $mailList[$index].S_Record_Sender_Company,
                $mailList[$index].S_Record_Sender_Mail1,
                $mailList[$index].S_Record_Sender_Mail2,
                $mailList[$index].S_Record_Dest_Mail1,
                $mailList[$index].S_Record_Dest_Mail2,
                $mailList[$index].S_Record_Bikou,
                "送信済み",
                $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                "",
                "",
                $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                $i,
                $G_ID,
                $G_log_renban_name
            )
            $recordLine = ($s_fields -join ",") + "`r`n"
            $global:StatusEntries += @{ key = "MS" + $global:StatusEntries.Length; value = $recordLine }

            # Write-Output $S_Record_TiTle
            # Write-Output "送信済み"

            break
        }
        catch {

            $EXEP = [string]$_.exception
            $EXEP = $EXEP.Replace(" ", "_")
            $EXEP = $EXEP.Replace(",", "_")
            $EXEP = [string]::join("_", $EXEP)


            Start-Sleep -Seconds $WaitSec  # 暂停1秒以便观察输出

            if ($i -eq $MailSendTimes) {

                $IsAllMailSendOk = $false
                $mailList[$index].IsMailSendOk = $false
                $NG_SUU++
                LogAndErr_ms $G_log_path $G_err_log_path "E" " [送信処理] メール送信は全て失敗しました。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP

                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" " [送信処理] メール送信は全て失敗しました。"
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP

                $err_txt = "メール送信処理が全て失敗したためプロセスを中止します。リトライ："+$MailSendTimes+"回目を実施します。" + $EXEP

                $s_fields = @(
                    $mailList[$index].S_Record_TiTle,
                    $mailList[$index].S_Record_Quality,
                    $mailList[$index].S_Record_Mail_kbn,
                    $mailList[$index].S_Record_Add_File_Name,
                    $mailList[$index].S_Record_Sender_Company,
                    $mailList[$index].S_Record_Sender_Mail1,
                    $mailList[$index].S_Record_Sender_Mail2,
                    $mailList[$index].S_Record_Dest_Mail1,
                    $mailList[$index].S_Record_Dest_Mail2,
                    $mailList[$index].S_Record_Bikou,
                    "送信失敗",
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $i,
                    $G_ID,
                    $G_log_renban_name
                )
                $recordLine = ($s_fields -join ",") + "`r`n"
                $global:StatusEntries += @{ key = "MS" + $global:StatusEntries.Length; value = $recordLine }
                
                Write-Output $mailList[$index].S_Record_TiTle


                HulftSendErrOnly $HULFT_ERR_PATH  "ST030 失敗1" $err_txt

                #exit $RC_ERROR

            }
            else {

                Logger_mss $G_log_path "W" (" [送信処理] メール送信は失敗しました。リトライ：" + $i + "回を実施しました。も一度実施します")
                Logger_mss $G_log_path "W" $EXEP

                Logger_mss $G_log_renban_path "W" (" [送信処理] メール送信は失敗しました。リトライ：" + $i + "回を実施しました。も一度実施します")
                Logger_mss $G_log_renban_path "W" $EXEP

                $s_fields = @(
                    $mailList[$index].S_Record_TiTle,
                    $mailList[$index].S_Record_Quality,
                    $mailList[$index].S_Record_Mail_kbn,
                    $mailList[$index].S_Record_Add_File_Name,
                    $mailList[$index].S_Record_Sender_Company,
                    $mailList[$index].S_Record_Sender_Mail1,
                    $mailList[$index].S_Record_Sender_Mail2,
                    $mailList[$index].S_Record_Dest_Mail1,
                    $mailList[$index].S_Record_Dest_Mail2,
                    $mailList[$index].S_Record_Bikou,
                    "送信失敗",
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $(Get-Date).ToString("yyyy-MM-dd HH:mm:ss"),
                    $i,
                    $G_ID,
                    $G_log_renban_name
                )
                $recordLine = ($s_fields -join ",") + "`r`n"
                $global:StatusEntries += @{ key = "MS" + $global:StatusEntries.Length; value = $recordLine }

                Write-Output $mailList[$index].S_Record_TiTle
            }
        }
    }

    $index++
    $intRenban++
}


#-----------------------
# FOR　メールS情報、　メール送信後、ファイル移動...
#-----------------------
$intRenban = 1
foreach ($tmpMailObj in $mailList) {
    #連番
    $G_RENBAN = $intRenban.ToString("D3") 
    # 連番ログファイルパス
    $G_log_renban_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + ".log"
    # 連番ERRログファイルパス
    $G_err_log_renban_path = $G_log_dir + $ymdhmsf + "_" + $G_ID + "_" + $G_RENBAN + "_err.log"

    
    $jsonFileName = $tmpMailObj.jsonFileName
    $statusFileName = $tmpMailObj.statusFileName
    $newMWKFileName = $tmpMailObj.newMWKFileName

    if ($tmpMailObj.IsMailSendOk -eq $true) {

        #添付ファイル
        if (Test-Path ($G_MAIL_DIR + $newMWKFileName)) {
            try {
                #throw [System.Exception]::new("XXXX")
                Copy-Item -Path ($G_MAIL_DIR + $newMWKFileName) -Destination $MailSendOKDir -Force
                Remove-Item -Path ($G_MAIL_DIR + $newMWKFileName) -ErrorAction Stop
            }
            catch {
                $EXEP = [string]$_.exception
                $EXEP = $EXEP.Replace(" ", "_")
                $EXEP = $EXEP.Replace(",", "_")
                $EXEP = [string]::join("_", $EXEP)
                $err_txt = "メールは送信されましたが、添付ファイルを 【MailSendOK】フォルダに移動できませんでした。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP                
            }
        }
        #JSONファイル
        if (Test-Path $jsonFileName) {
            try {
                Copy-Item -Path $jsonFileName -Destination $MailSendOKDir -Force
                Remove-Item -Path $jsonFileName -ErrorAction Stop
            }
            catch {
                $EXEP = [string]$_.exception
                $EXEP = $EXEP.Replace(" ", "_")
                $EXEP = $EXEP.Replace(",", "_")
                $EXEP = [string]::join("_", $EXEP)
                $err_txt = "メールは送信されましたが、メールJSONファイルを 【MailSendOK】フォルダに移動できませんでした。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP  
            }
        }

        #STATUSファイル
        if (Test-Path $statusFileName) {
            try {
                Copy-Item -Path $statusFileName -Destination $MailSendOKDir -Force
                Remove-Item -Path $statusFileName -ErrorAction Stop
            }
            catch {
                $EXEP = [string]$_.exception
                $EXEP = $EXEP.Replace(" ", "_")
                $EXEP = $EXEP.Replace(",", "_")
                $EXEP = [string]::join("_", $EXEP)
                $err_txt = "メールは送信されましたが、メールSTATUSファイルを削除できませんでした。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP  
            }
        }

    }
    else {


        #添付ファイル
        if (Test-Path ($G_MAIL_DIR + $newMWKFileName)) {
            try {
                Copy-Item -Path ($G_MAIL_DIR + $newMWKFileName) -Destination $MailSendFailDir -Force
                Remove-Item -Path ($G_MAIL_DIR + $newMWKFileName) -ErrorAction Stop
            }
            catch {
                $EXEP = [string]$_.exception
                $EXEP = $EXEP.Replace(" ", "_")
                $EXEP = $EXEP.Replace(",", "_")
                $EXEP = [string]::join("_", $EXEP)
                $err_txt = "メールの送信に失敗し、添付ファイルを 【MailSendFail】フォルダに移動することもできませんでした。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP     
            }
        }
        #JSONファイル
        if (Test-Path $jsonFileName) {
            try {
                Copy-Item -Path $jsonFileName -Destination $MailSendFailDir -Force
                Remove-Item -Path $jsonFileName -ErrorAction Stop
            }
            catch {
                $EXEP = [string]$_.exception
                $EXEP = $EXEP.Replace(" ", "_")
                $EXEP = $EXEP.Replace(",", "_")
                $EXEP = [string]::join("_", $EXEP)
                $err_txt = "メールの送信に失敗し、メールJSONファイルを 【MailSendFail】フォルダに移動することもできませんでした。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP  
            }
        }

        #STATUSファイル
        if (Test-Path $statusFileName) {
            try {
                Copy-Item -Path $statusFileName -Destination $MailSendFailDir -Force
                Remove-Item -Path $statusFileName -ErrorAction Stop
            }
            catch {
                $EXEP = [string]$_.exception
                $EXEP = $EXEP.Replace(" ", "_")
                $EXEP = $EXEP.Replace(",", "_")
                $EXEP = [string]::join("_", $EXEP)
                $err_txt = "メールは送信されましたが、メールSTATUSファイルを削除できませんでした。"
                LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
                LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $err_txt
                LogAndErr_ms $G_log_renban_path $G_err_log_renban_path "E" $EXEP  
            }
        }        


    }
    $intRenban++
} 

#ここはメール送信エラーのみ実行できる
try {
    WriteStatus
}
catch {

}

#-----------------------
# 最終判定、ファイル移動...
#-----------------------
#最終判定 正常
if ($IsAllMailSendOk -eq $true) {
    #Write-Output "【OK】："
    if (Test-Path $newFilePath) {
        try {
            Copy-Item -Path $newFilePath -Destination $MailSendOKDir -Force        
            Remove-Item -Path $newFilePath -ErrorAction Stop
        }
        catch {
            $EXEP = [string]$_.exception
            $EXEP = $EXEP.Replace(" ", "_")
            $EXEP = $EXEP.Replace(",", "_")
            $EXEP = [string]::join("_", $EXEP)
            $err_txt = "メールは送信されましたが、元ファイルを 【MailSendOK】フォルダに移動できませんでした。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
            LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
        }
    }  
    Logger_txt $G_log_path "***************************************************************************************************************"
    Logger_txt $G_log_path ("正常：" + ($mailList.Count - $NG_SUU) + "件　　　異常" + $NG_SUU + "件")
    Logger_txt $G_log_path "***************************************************************************************************************"
}
else {
    #最終判定 異常
    if (Test-Path $newFilePath) {
        try {        
            Copy-Item -Path $newFilePath -Destination $MailSendFailDir -Force     
            Remove-Item -Path $newFilePath -ErrorAction Stop
        }
        catch {
            $EXEP = [string]$_.exception
            $EXEP = $EXEP.Replace(" ", "_")
            $EXEP = $EXEP.Replace(",", "_")
            $EXEP = [string]::join("_", $EXEP)
            $err_txt = "メールの送信に失敗し、元ファイルを 【MailSendFail】フォルダに移動することもできませんでした。"
            LogAndErr_ms $G_log_path $G_err_log_path "E" $err_txt
            LogAndErr_ms $G_log_path $G_err_log_path "E" $EXEP
        }
    }        
    Logger_txt $G_log_path " " 
    Logger_txt $G_log_path "=============================================================================================================="
    Logger_txt $G_log_path "***************************************************************************************************************"
    Logger_txt $G_log_path ("正常：" + ($mailList.Count - $NG_SUU) + "件　　　異常" + $NG_SUU + "件")
    Logger_txt $G_log_path "***************************************************************************************************************"
}

exit $RC