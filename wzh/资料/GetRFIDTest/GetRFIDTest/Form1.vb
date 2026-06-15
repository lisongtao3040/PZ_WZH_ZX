
Imports System.IO
Imports System.Net.NetworkInformation
Imports System.Text
Imports UTRSDKV1PC


Public Class Form1

    'ログファイル
    Private logfilePath As String = "C:\Logs\GetRFIDTest.log"

    'RFIDControl
    Private WithEvents M_ctrl As RFIDControl
    Public Sub New()
        InitializeComponent()
        M_ctrl = New RFIDControl() With {
            .Timeout = 1000
        }
    End Sub


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    ''' <summary>
    ''' ResponseRFIDイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Ctrl_ResponseRFID(ByVal sender As Object, ByVal e As InputEventArgs) Handles M_ctrl.ResponseRFID

        Dim userdata As String = ""
        Dim pcstr As String = ""
        Dim epcstr As String = ""
        Dim rssistr As String = ""
        Dim tidstr As String = ""
        Dim pc, epc As Byte()

        Select Case e.SendCommand
            Case RFID_SendCommand.GetROMVersion
                WriteLog("GetROMVersion（Data）:", e.TextData.ToString)
                WriteLog("GetROMVersion（End）", logfilePath)
                MsgBox(e.TextData.ToString)

            Case RFID_SendCommand.UHFInventory, RFID_SendCommand.UHFInventoryCmd

                If e.TextData.Length <> 0 Then
                    userdata = e.TextData
                End If

                pc = e.PC

                If pc IsNot Nothing Then
                    pcstr = HexConverterToString(pc)
                End If

                epc = e.EPC

                If epc IsNot Nothing Then
                    epcstr = HexConverterToString(epc)
                End If

                rssistr = CInt((e.RSSI / 10)).ToString + "." + CInt(-(e.RSSI Mod 10)).ToString

                WriteLog("UHF_Inventory（Data）:epcstr = " & epcstr, logfilePath)
                WriteLog("UHF_Inventory（Data）:pcstr = " & pcstr, logfilePath)
                WriteLog("UHF_Inventory（Data）:rssistr = " & rssistr, logfilePath)
                WriteLog("UHF_Inventory（Data）:userdata = " & userdata, logfilePath)
                WriteLog("UHF_InventoryCmd（End）", logfilePath)
                Me.TextBox1.Text = userdata

        End Select

    End Sub

    ''' <summary>
    ''' RFID受信のテストボタン
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If M_ctrl.IsOpen Then
            WriteLog("GetROMVersion（Start）:", logfilePath)
            If M_ctrl.GetROMVersion() = -1 Then
                WriteLog("GetROMVersion（失敗）!", logfilePath)
            Else
                WriteLog("GetROMVersion（成功）!", logfilePath)
            End If


            WriteLog("UHF_InventoryCmd（Start）", logfilePath)
            If M_ctrl.UHF_InventoryCmd() = -1 Then
                WriteLog("UHF_InventoryCmd（失敗）!", logfilePath)
            Else
                WriteLog("UHF_InventoryCmd（成功）!", logfilePath)
            End If
            MsgBox("RFID受信完了", vbCritical, "Message")
        Else
            MsgBox("RFID受信失敗（IsOpen = false）", vbCritical, "Message")
        End If


    End Sub

    ''' <summary>
    ''' 接続テストボタン
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim result As Boolean = True

        result = M_ctrl.Open()

        If Not M_ctrl.IsOpen Then
            result = M_ctrl.Open(1, RFID_BaudRate.BaudRate115200)
        End If

        If result = True Then
            MsgBox("接続 OK", vbCritical, "Message")
            WriteLog("接続成功", logfilePath)
        Else
            MsgBox("接続 NG", vbCritical, "Message")
            WriteLog("接続失敗", logfilePath)
        End If

    End Sub


    ''' <summary>
    ''' ログ出力
    ''' </summary>
    ''' <param name="message"></param>
    ''' <param name="filePath"></param>
    Public Shared Sub WriteLog(ByVal message As String, ByVal filePath As String)

        Dim logDirectory As String = Path.GetDirectoryName(filePath)
        If Not Directory.Exists(logDirectory) Then
            Directory.CreateDirectory(logDirectory)
        End If

        Using writer As New StreamWriter(filePath, True)
            writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}")
        End Using

    End Sub

    ''' <summary>
    ''' ToString方法（SampleAppの方法）
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    Public Function HexConverterToString(ByVal source As Byte()) As String
        Const CHAR_SIZE As Integer = 2
        Dim length As Integer = source.Length
        Dim builder As New StringBuilder(length * CHAR_SIZE)

        Dim i As Integer
        For i = 0 To length - 1

            If (i <> 0) Then
                builder.Append(" ")    '「000102」→「00 01 02」
            End If

            If (source(i) = 0) Then
                builder.Append("00")
            Else
                builder.Append(source(i).ToString("X2"))
            End If
        Next
        Return builder.ToString()
    End Function

End Class
