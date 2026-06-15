Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms
Imports SampleApp.Util
Imports UTRSDKV1PC

Namespace SampleApp

    Public Class MainForm

        Inherits Form

        <DllImport("winmm.dll")>
        Public Shared Function timeBeginPeriod(ByVal uMilliseconds As UInteger) As UInteger

        End Function

        Public Sub New()
            Me.InitializeComponent()
            KeyboardHook.Start()
            Me.m_ctrl = New RFIDControl With {
                .Timeout = 1000UI
            }
            'Me.m_ctrl.ResponseRFID += AddressOf Me.Ctrl_ResponseRFID
            AddHandler m_ctrl.ResponseRFID, AddressOf Me.Ctrl_ResponseRFID

            Me.AddLogDel = New MainForm.AddLogDelegate(AddressOf Me.AddLog)
            Me.UpdateWinFormThread = New Thread(New ThreadStart(AddressOf Me.UpdateWinForm))
            Me.UpdateWinFormThread.Start()
            Me.panel5.Enabled = False
        End Sub

        Protected Overrides Sub Finalize()
            If Me.m_ctrl IsNot Nothing Then
                Me.m_ctrl.Dispose()
            End If
        End Sub

        Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs)
            Me.btn_Connect.Enabled = True
            Me.btn_Disconnect.Enabled = False
            Dim num As UInteger = 1UI

            While num < 1000UI AndAlso MainForm.timeBeginPeriod(num) = 0UI
                num += 1UI
            End While
        End Sub

        Private Sub MainForm_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            KeyboardHook.[Stop]()

            If Me.m_ctrl IsNot Nothing Then
                Me.m_ctrl.Dispose()
            End If

            Dim obj As Object = Me.lockobj

            SyncLock obj

                Try

                    If Me.UpdateWinFormRunning Then
                        Me.UpdateWinFormRunning = False
                        Me.m_UpdateWinFormThreadEndWaiter.ExitTimer = False
                        Me.m_UpdateWinFormThreadEndWaiter.Wait(5000)
                    End If

                Catch ex As Exception
                    MessageBox.Show("エラーが発生しました。" & vbCrLf & vbCrLf & "[エラー詳細]" & vbCrLf & ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                End Try
            End SyncLock
        End Sub

        Private Sub UpdateWinForm()
            Me.UpdateWinFormRunning = True

            Try
                Dim i As Integer = 0
                Dim num As Integer = 0
                Dim updateLogData As MainForm.UpdateLogData = Nothing

                While True
                    Thread.Sleep(1)
                    Application.DoEvents()

                    If Not Me.UpdateWinFormRunning Then
                        Exit While
                    End If

                    num = Me.m_Queue.Count

                    If num > 0 Then

                        For i = 0 To num - 1
                            Dim queue As Queue = Me.m_Queue

                            SyncLock queue
                                updateLogData = CType(Me.m_Queue.Dequeue(), MainForm.UpdateLogData)
                            End SyncLock

                            If Not Me.UpdateWinFormRunning Then
                                Exit For
                            End If

                            If updateLogData.Target = MainForm.UpdateTarget.Log Then
                                Dim obj As Object = Me.lockobj

                                SyncLock obj
                                    MyBase.Invoke(Me.AddLogDel, New Object() {updateLogData.Log})
                                End SyncLock
                            End If
                        Next
                    End If
                End While

            Catch ex As Exception
                MessageBox.Show("画面更新処理中に例外が発生しました。" & vbCrLf & "アプリケーションを再起動してください。", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            Finally

                If Me.UpdateWinFormRunning Then
                    Me.UpdateWinFormRunning = False
                End If

                If Not Me.m_UpdateWinFormThreadEndWaiter.ExitTimer Then
                    Me.m_UpdateWinFormThreadEndWaiter.ExitTimer = True
                End If
            End Try
        End Sub

        Private Sub OutputMessage(ByVal msg As String)
            Try

                If Me.UpdateWinFormRunning Then
                    Dim updateLogData As MainForm.UpdateLogData = New MainForm.UpdateLogData With {
                        .Target = MainForm.UpdateTarget.Log
                    }
                    Dim log As String = Me.add_TimeStamp() & msg
                    updateLogData.Log = log
                    Dim queue As Queue = Me.m_Queue

                    SyncLock queue
                        Me.m_Queue.Enqueue(updateLogData)
                    End SyncLock
                End If

            Catch ex As Exception
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                MyBase.Close()
            End Try
        End Sub

        Private Sub OutputMessage(ByVal msg As String, ByVal showlog As Boolean)
            Try

                If Me.UpdateWinFormRunning Then

                    If showlog Then
                        Dim updateLogData As MainForm.UpdateLogData = New MainForm.UpdateLogData With {
                            .Target = MainForm.UpdateTarget.Log
                        }
                        Dim log As String = Me.add_TimeStamp() & msg
                        updateLogData.Log = log
                        Dim queue As Queue = Me.m_Queue

                        SyncLock queue
                            Me.m_Queue.Enqueue(updateLogData)
                        End SyncLock
                    End If
                End If

            Catch ex As Exception
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                MyBase.Close()
            End Try
        End Sub

        Private Sub OutputMessage(ByVal msg As String, ByVal showlog As Boolean, ByVal writer As StreamWriter)
            Try

                If Me.UpdateWinFormRunning Then
                    Dim updateLogData As MainForm.UpdateLogData = New MainForm.UpdateLogData With {
                        .Target = MainForm.UpdateTarget.Log
                    }
                    Dim text As String = (If((MainForm.TimeLog = 1), Me.add_TimeStamp(), "")) & msg

                    If showlog Then
                        updateLogData.Log = text
                        Dim queue As Queue = Me.m_Queue

                        SyncLock queue
                            Me.m_Queue.Enqueue(updateLogData)
                        End SyncLock
                    End If

                    writer.WriteLine(text)
                End If

            Catch ex As Exception
                MessageBox.Show(ex.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Hand)
                MyBase.Close()
            End Try
        End Sub

        Private Sub InformationMessage(ByVal msg As String)
            If MyBase.InvokeRequired Then
                MyBase.BeginInvoke(New MainForm.MessageDelegate(AddressOf Me.InformationMessage), New Object() {msg})
                Return
            End If

            Try
                Me.InformationLabel.Text = msg
            Catch __unusedObjectDisposedException1__ As ObjectDisposedException
            End Try
        End Sub

        Private Sub ClearMessage()
            Me.lbxLog.Items.Clear()
        End Sub

        Private Sub Ctrl_ResponseRFID(ByVal sender As Object, ByVal e As InputEventArgs)
            Dim sendCommand As RFID_SendCommand = e.SendCommand

            If (sendCommand = RFID_SendCommand.UHFInventory OrElse sendCommand = RFID_SendCommand.UHFInventoryCmd) AndAlso e.EPC IsNot Nothing AndAlso Me.Buffering(Me.recvdata, e.EPC) Then
                Me.recvdata.Add(e.EPC)
                Me.OutputMessage(HexConverter.ToString(e.EPC), True)
            End If
        End Sub

        Private Sub AddLog(ByVal str As String)
            Try

                If Me.lbxLog.Items.Count > 10000 Then
                    Me.ClearMessage()
                End If

                Me.lbxLog.Items.Add(str)
                Me.lbxLog.SelectedIndex = Me.lbxLog.Items.Count - 1
            Catch __unusedObjectDisposedException1__ As ObjectDisposedException
            End Try
        End Sub

        Private Function Exteventcheck() As Integer
            Application.DoEvents()
            Return 0
        End Function

        Private Function add_TimeStamp() As String
            Dim now As DateTime = DateTime.Now
            Dim text As String = now.Year.ToString("D4")
            Dim text2 As String = now.ToString("MM/dd")
            Dim text3 As String = now.ToString("HH:mm:ss")

            Select Case MainForm.TimeStamp
                Case 0
                    Return ""
                Case 1
                    Return text3 & "  "
                Case 2
                    Return text2 & " " & text3 & "  "
                Case 3
                    Return String.Concat(New String() {text, "/", text2, " ", text3, "  "})
                Case Else
                    Return ""
            End Select
        End Function

        Private Sub btn_Connect_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim flag As Boolean = True
            Dim baseDirectory As String = AppDomain.CurrentDomain.BaseDirectory
            Dim str As String = Path.GetFileName(Application.ExecutablePath).Replace(".exe", ".ini")
            Dim text As String = baseDirectory & str
            Dim text2 As String = ""
            Dim str2 As String = ""

            If Not File.Exists(text) Then
                Me.OutputMessage("iniファイルが存在しません", True)
                Me.OutputMessage("アプリと同じフォルダ内に、iniファイル[" & str & "]を置いてください。", True)
                flag = False
            Else
                Dim iniFile As IniFile = New IniFile(text)
                Dim tmp1 As String = iniFile.GetString("Log", "Folder", "")
                text2 = Directory.GetCurrentDirectory() & "\" & tmp1

                If Not Directory.Exists(text2) Then
                    Directory.CreateDirectory(text2)
                End If

                str2 = iniFile.GetString("Log", "FileName", "ReadLog_")
                MainForm.TimeStamp = iniFile.GetInt("Log", "TimeStamp", 1)
                MainForm.TimeLog = iniFile.GetInt("Log", "TimeLog", 0)
                Dim int As Integer = iniFile.GetInt("Connection", "ConType", 0)

                If Me.m_ctrl.IsOpen Then
                    Me.m_ctrl.Close()
                End If

                If int <> 1 Then

                    If int <> 2 Then
                        flag = False
                    Else
                        Dim string2 As String = iniFile.GetString("Connection", "IPaddress", "192.168.0.1")
                        Dim int2 As Integer = iniFile.GetInt("Connection", "PortNo", 9004)

                        If Me.m_ctrl.Connect(string2, int2) Then
                            Me.OutputMessage(String.Concat(New String() {"IPアドレス: ", string2, "、ポート番号: ", int2.ToString(), " との接続に成功しました。"}))
                        Else
                            Me.OutputMessage(String.Concat(New String() {"IPアドレス: ", string2, "、ポート番号: ", int2.ToString(), " との接続に失敗しました。"}))
                            Me.OutputMessage("IPアドレス、または、ポート番号を確認してください。")
                            flag = False
                        End If
                    End If
                Else
                    Dim int3 As Integer = iniFile.GetInt("Connection", "COMPort", 0)
                    Dim baudRate As RFID_BaudRate = RFID_BaudRate.BaudRate115200

                    If Me.m_ctrl.Open(int3, baudRate) Then
                        Me.OutputMessage("Port番号: " & int3.ToString() & " との接続に成功しました。")
                    Else
                        Me.OutputMessage("Port番号: " & int3.ToString() & " との接続に失敗しました。Port番号を確認してください。")
                        flag = False
                    End If
                End If
            End If

            If flag Then
                Dim iniFile2 As IniFile = New IniFile(text)
                Dim string3 As String = iniFile2.GetString("Control", "StartKey", "")
                Me.btn_Start.Text = "開始 (" & string3 & ")"
                [Enum].TryParse(Of Keys)(string3, MainForm.StartKey)
                Dim string4 As String = iniFile2.GetString("Control", "StopKey", "")
                Me.btn_Stop.Text = "停止 (" & string4 & ")"
                [Enum].TryParse(Of Keys)(string4, MainForm.StopKey)
                Dim [option] As ActionModeOption = New ActionModeOption With {
                    .UseBuzzer = RFID_UseBuzzer.Unuse
                }
                Me.panel5.Enabled = True
                Me.lb_Connect.Text = "接続中"
                Me.m_ctrl.SetActionMode(RFID_ScanMode.CommandScanMode, [option], False)
                Me.lb_Reader.Text = "停止中"
                Me.btn_Connect.Enabled = False
                Me.btn_Disconnect.Enabled = True
                Me.btn_Start.Enabled = True
                Me.btn_Stop.Enabled = False
                MainForm.isStartKeyPressed = False
                MainForm.isStopKeyPressed = False
                Me.mugenloop = True
                Me.recvdata.Clear()

                While Me.Exteventcheck() = 0

                    If MainForm.isStartKeyPressed Then
                        MainForm.isStartKeyPressed = False
                        MainForm.isStopKeyPressed = False
                        Me.recvdata.Clear()
                        Me.m_ctrl.SetActionMode(RFID_ScanMode.UHFInventoryMode, [option], False)
                        Me.OutputMessage("- - - - - - - - - - - - - - - - - - - -", True)
                        Me.OutputMessage("読み取りを開始しました。", True)
                        Me.btn_Start.Enabled = False
                        Me.btn_Stop.Enabled = True
                        Me.panel1.Enabled = False
                        Me.lb_Reader.Text = "読み取り中"

                        While Me.Exteventcheck() = 0 AndAlso Not MainForm.isStopKeyPressed
                        End While

                        MainForm.isStartKeyPressed = False
                        MainForm.isStopKeyPressed = False
                        Me.m_ctrl.SetActionMode(RFID_ScanMode.CommandScanMode, [option], False)
                        Me.btn_Start.Enabled = True
                        Me.btn_Stop.Enabled = False
                        Me.lb_Reader.Text = "停止中"
                        Me.OutputMessage("読み取りを終了しました。", True)

                        If Me.recvdata.Count <> 0 Then
                            Dim text3 As String = str2 & DateTime.Now.ToString("yyyyMMddHHmmss") & ".csv"
                            Dim path As String = text2 & "\" & text3
                            Me.OutputMessage("保存ファイル名: " & text3)
                            Dim utf As Encoding = Encoding.UTF8
                            Dim streamWriter As StreamWriter = New StreamWriter(path, True, utf)

                            For i As Integer = 0 To Me.recvdata.Count - 1
                                Me.OutputMessage(HexConverter.ToString(Me.recvdata(i)), False, streamWriter)
                            Next

                            streamWriter.Close()
                        End If

                        Me.recvdata.Clear()
                        Me.panel1.Enabled = True
                    End If

                    If Not Me.mugenloop Then
                        Exit While
                    End If
                End While

                If Me.m_ctrl.IsOpen Then
                    Me.m_ctrl.Close()
                    Me.OutputMessage("リーダライタとの接続を切断しました。")
                End If

                Me.lb_Connect.Text = "未接続"
                Me.m_ctrl.SetActionMode(RFID_ScanMode.CommandScanMode, [option], False)
                Me.btn_Connect.Enabled = True
                Me.btn_Disconnect.Enabled = False
                Me.panel5.Enabled = False
            End If
        End Sub

        Private Sub button7_Click(ByVal sender As Object, ByVal e As EventArgs)
            Me.mugenloop = False
            Me.lb_Connect.Text = "停止処理中..."
        End Sub

        Private Function Buffering(ByVal list As List(Of Byte()), ByVal array As Byte()) As Boolean
            For i As Integer = 0 To list.Count - 1

                If Me.CompareByteArray(list(i), array) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Function CompareByteArray(ByVal array1 As Byte(), ByVal array2 As Byte()) As Boolean
            If array1 Is Nothing OrElse array2 Is Nothing OrElse array1.Length <> array2.Length Then
                Return False
            End If

            For i As Integer = 0 To array1.Length - 1

                If Not array1(i).Equals(array2(i)) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Private Sub button2_Click(ByVal sender As Object, ByVal e As EventArgs)
            MainForm.isStartKeyPressed = True
        End Sub

        Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs)
            MainForm.isStopKeyPressed = True
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso Me.components IsNot Nothing Then
                Me.components.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Me.SuspendLayout()
            '
            'MainForm
            '
            Me.ClientSize = New System.Drawing.Size(1065, 597)
            Me.Name = "MainForm"
            Me.ResumeLayout(False)

        End Sub

        Private Const TIMERR_NOERROR As Integer = 0
        Private m_ctrl As RFIDControl
        Public Shared writer As StreamWriter
        Private recvdata As List(Of Byte()) = New List(Of Byte())()
        Private mugenloop As Boolean
        Private UpdateWinFormThread As Thread
        Private UpdateWinFormRunning As Boolean
        Private m_UpdateWinFormThreadEndWaiter As ChkWaiter = New ChkWaiter()
        Private lockobj As Object = New Object()
        Private AddLogDel As MainForm.AddLogDelegate
        Private AddDataToListDel As MainForm.AddDataToListDelegate
        Private m_Queue As Queue = Queue.Synchronized(New Queue())
        Public Shared StartKey As Keys = Keys.None
        Public Shared StopKey As Keys = Keys.None
        Public Shared TimeStamp As Integer = 1
        Public Shared TimeLog As Integer = 0
        Public Shared isStartKeyPressed As Boolean = False
        Public Shared isStopKeyPressed As Boolean = False
        Private m_lastScanMode As RFID_ScanMode
        Private m_lastActionModeOption As ActionModeOption
        Private m_lastGeneralIOPortState As Boolean() = New Boolean(7) {}
        Private components As IContainer
        Private panel3 As Panel
        Private panel4 As Panel
        Private takayaTToolStripMenuItem As ToolStripMenuItem
        Private toolStripSeparator1 As ToolStripSeparator
        Private toolStripSeparator2 As ToolStripSeparator
        Private iSOToolStripMenuItem As ToolStripMenuItem
        Private toolStripSeparator4 As ToolStripSeparator
        Private toolStripSeparator10 As ToolStripSeparator
        Private toolStripSeparator5 As ToolStripSeparator
        Private panel2 As Panel
        Private contextList As ContextMenuStrip
        Private ItmClear2 As ToolStripMenuItem
        Private lbxLog As ListBox
        Private Timer1 As System.Windows.Forms.Timer
        Private Timer2 As System.Windows.Forms.Timer
        Private InformationLabel As Label
        Private itmTxOn As ToolStripMenuItem
        Private itemTxOff As ToolStripMenuItem
        Private itemTxOffOn As ToolStripMenuItem
        Private btn_Connect As Button
        Private btn_Disconnect As Button
        Private lb_Connect As Label
        Private label1 As Label
        Private lb_Reader As Label
        Private panel1 As Panel
        Private panel5 As Panel
        Private btn_Stop As Button
        Private btn_Start As Button
        Private label2 As Label

        Private Enum UpdateTarget
            Log
            List
        End Enum

        Private Structure UpdateLogData
            Public Target As MainForm.UpdateTarget
            Public Log As String
            Public EPC As String
            Public UserData As String
            Public TID As String
            Public RSSI As String
        End Structure

        Private Delegate Sub MessageDelegate(ByVal msg As String)
        Private Delegate Sub AddLogDelegate(ByVal str As String)
        Private Delegate Sub AddDataToListDelegate(ByVal epc As String, ByVal rssi As String, ByVal user As String, ByVal tid As String)
    End Class
End Namespace
