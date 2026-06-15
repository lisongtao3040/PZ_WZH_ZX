Imports System.Threading
Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private simulatedCards As New List(Of String)()
    Private simulationThread As Thread
    Private isSimulating As Boolean = False
    Private listener As TcpListener
    Private activeClients As New List(Of TcpClient)()
    Private serverThread As Thread
    Private isServerRunning As Boolean = False

    ' 控件声明
    Private WithEvents lstCards As ListBox
    Private WithEvents txtNewCard As TextBox
    Private WithEvents btnAddCard As Button
    Private WithEvents btnRemoveCard As Button
    Private WithEvents btnStartSimulation As Button
    Private WithEvents btnStopSimulation As Button
    Private WithEvents btnSendCard As Button
    Private WithEvents numInterval As NumericUpDown
    Private WithEvents chkRandomOrder As CheckBox
    Private lblStatus As Label
    Private WithEvents btnStartServer As Button

    Public Sub New()
        InitializeComponent()
        InitializeForm()
        InitializeSimulatedCards()
    End Sub

    Private Sub InitializeForm()
        Me.Text = "RFID 虚拟设备模拟器 (TCP模拟COM4)"
        Me.Size = New Size(600, 500)
        Me.Font = New Font("Segoe UI", 10)

        ' 创建控件
        CreateControls()
    End Sub

    Private Sub CreateControls()
        ' 标题
        Dim lblTitle As New Label With {
            .Text = "RFID 虚拟设备模拟器 - TCP端口4004",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .Location = New Point(20, 15),
            .AutoSize = True
        }
        Me.Controls.Add(lblTitle)

        ' 状态标签
        lblStatus = New Label With {
            .Text = "状态: 服务器未启动",
            .Location = New Point(20, 50),
            .AutoSize = True,
            .ForeColor = Color.DarkRed
        }
        Me.Controls.Add(lblStatus)

        ' 服务器控制
        btnStartServer = New Button With {
            .Text = "启动虚拟端口",
            .Location = New Point(20, 80),
            .Size = New Size(150, 35),
            .BackColor = Color.LightGreen
        }
        AddHandler btnStartServer.Click, AddressOf StartServer_Click
        Me.Controls.Add(btnStartServer)

        ' 卡号列表标签
        Dim lblCards As New Label With {
            .Text = "模拟卡号列表:",
            .Location = New Point(20, 130),
            .AutoSize = True
        }
        Me.Controls.Add(lblCards)

        ' 卡号列表框
        lstCards = New ListBox With {
            .Location = New Point(20, 155),
            .Size = New Size(550, 120),
            .SelectionMode = SelectionMode.MultiExtended
        }
        Me.Controls.Add(lstCards)

        ' 添加卡号区域
        txtNewCard = New TextBox With {
            .Location = New Point(20, 290),
            .Size = New Size(300, 25)
        }
        Me.Controls.Add(txtNewCard)

        btnAddCard = New Button With {
            .Text = "添加卡号",
            .Location = New Point(330, 290),
            .Size = New Size(100, 30),
            .BackColor = Color.LightGreen
        }
        AddHandler btnAddCard.Click, AddressOf AddCard_Click
        Me.Controls.Add(btnAddCard)

        btnRemoveCard = New Button With {
            .Text = "移除选中",
            .Location = New Point(440, 290),
            .Size = New Size(130, 30),
            .BackColor = Color.LightCoral
        }
        AddHandler btnRemoveCard.Click, AddressOf RemoveCard_Click
        Me.Controls.Add(btnRemoveCard)

        ' 模拟控制区域
        Dim lblActions As New Label With {
            .Text = "模拟控制:",
            .Location = New Point(20, 330),
            .AutoSize = True
        }
        Me.Controls.Add(lblActions)

        btnStartSimulation = New Button With {
            .Text = "开始模拟",
            .Location = New Point(20, 360),
            .Size = New Size(120, 35),
            .BackColor = Color.LightBlue,
            .Enabled = False
        }
        AddHandler btnStartSimulation.Click, AddressOf StartSimulation_Click
        Me.Controls.Add(btnStartSimulation)

        btnStopSimulation = New Button With {
            .Text = "停止模拟",
            .Location = New Point(150, 360),
            .Size = New Size(120, 35),
            .BackColor = Color.LightSalmon,
            .Enabled = False
        }
        AddHandler btnStopSimulation.Click, AddressOf StopSimulation_Click
        Me.Controls.Add(btnStopSimulation)

        btnSendCard = New Button With {
            .Text = "发送选中卡号",
            .Location = New Point(280, 360),
            .Size = New Size(160, 35),
            .BackColor = Color.LightSkyBlue,
            .Enabled = False
        }
        AddHandler btnSendCard.Click, AddressOf SendCard_Click
        Me.Controls.Add(btnSendCard)

        ' 模拟设置
        Dim lblSettings As New Label With {
            .Text = "模拟设置:",
            .Location = New Point(20, 405),
            .AutoSize = True
        }
        Me.Controls.Add(lblSettings)

        Dim lblInterval As New Label With {
            .Text = "发送间隔 (ms):",
            .Location = New Point(150, 405),
            .AutoSize = True
        }
        Me.Controls.Add(lblInterval)

        numInterval = New NumericUpDown With {
            .Location = New Point(260, 403),
            .Size = New Size(80, 25),
            .Minimum = 500,
            .Maximum = 10000,
            .Value = 1500,
            .Increment = 100
        }
        Me.Controls.Add(numInterval)

        chkRandomOrder = New CheckBox With {
            .Text = "随机顺序发送",
            .Location = New Point(360, 405),
            .AutoSize = True,
            .Checked = True
        }
        Me.Controls.Add(chkRandomOrder)
    End Sub

    Private Sub InitializeSimulatedCards()
        ' 添加一些初始模拟卡号
        simulatedCards.Add("A1B2C3D4")
        simulatedCards.Add("F5E67890")
        simulatedCards.Add("12345678")
        simulatedCards.Add("9ABCDEF0")
        simulatedCards.Add("87654321")

        RefreshCardList()
    End Sub

    Private Sub RefreshCardList()
        lstCards.Items.Clear()
        For Each card In simulatedCards
            lstCards.Items.Add(card)
        Next
    End Sub

    Private Sub StartServer_Click(sender As Object, e As EventArgs)
        If Not isServerRunning Then
            ' 启动TCP服务器模拟虚拟端口
            Try
                listener = New TcpListener(IPAddress.Any, 4004)
                listener.Start()
                isServerRunning = True
                serverThread = New Thread(AddressOf ServerLoop)
                serverThread.IsBackground = True
                serverThread.Start()

                btnStartServer.Text = "停止虚拟端口"
                lblStatus.Text = "状态: 虚拟端口已启动 (TCP:4004)"
                lblStatus.ForeColor = Color.DarkGreen
                btnStartSimulation.Enabled = True
                btnSendCard.Enabled = True
            Catch ex As Exception
                lblStatus.Text = "状态: 启动失败 - " & ex.Message
                lblStatus.ForeColor = Color.DarkRed
            End Try
        Else
            ' 停止服务器
            isServerRunning = False
            listener.Stop()

            ' 关闭所有客户端连接
            For Each client In activeClients
                Try
                    client.Close()
                Catch
                End Try
            Next
            activeClients.Clear()

            btnStartServer.Text = "启动虚拟端口"
            lblStatus.Text = "状态: 服务器已停止"
            lblStatus.ForeColor = Color.DarkRed
            btnStartSimulation.Enabled = False
            btnSendCard.Enabled = False
        End If
    End Sub

    Private Sub ServerLoop()
        While isServerRunning
            Try
                If listener.Pending() Then
                    Dim client As TcpClient = listener.AcceptTcpClient()
                    SyncLock activeClients
                        activeClients.Add(client)
                    End SyncLock

                    UpdateStatus($"客户端已连接: {client.Client.RemoteEndPoint}")
                End If
                Thread.Sleep(100)
            Catch ex As Exception
                If isServerRunning Then
                    UpdateStatus($"服务器错误: {ex.Message}", True)
                End If
                Exit While
            End Try
        End While
    End Sub

    Private Sub AddCard_Click(sender As Object, e As EventArgs)
        If Not String.IsNullOrWhiteSpace(txtNewCard.Text) Then
            ' 验证卡号格式
            If txtNewCard.Text.Length >= 8 AndAlso txtNewCard.Text.Length <= 16 Then
                simulatedCards.Add(txtNewCard.Text)
                RefreshCardList()
                txtNewCard.Clear()
            Else
                MessageBox.Show("卡号长度应在8-16个字符之间", "无效卡号", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Else
            MessageBox.Show("请输入卡号", "添加卡号", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub RemoveCard_Click(sender As Object, e As EventArgs)
        If lstCards.SelectedItems.Count > 0 Then
            For i As Integer = lstCards.SelectedItems.Count - 1 To 0 Step -1
                simulatedCards.Remove(lstCards.SelectedItems(i))
            Next
            RefreshCardList()
        Else
            MessageBox.Show("请选择要移除的卡号", "移除卡号", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub StartSimulation_Click(sender As Object, e As EventArgs)
        If simulatedCards.Count = 0 Then
            MessageBox.Show("请先添加至少一个卡号", "开始模拟", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        isSimulating = True
        btnStartSimulation.Enabled = False
        btnStopSimulation.Enabled = True
        btnSendCard.Enabled = False
        lstCards.Enabled = False
        btnAddCard.Enabled = False
        btnRemoveCard.Enabled = False
        txtNewCard.Enabled = False

        ' 启动模拟线程
        simulationThread = New Thread(AddressOf SimulateCardReading)
        simulationThread.IsBackground = True
        simulationThread.Start()
    End Sub

    Private Sub StopSimulation_Click(sender As Object, e As EventArgs)
        isSimulating = False
        If simulationThread IsNot Nothing AndAlso simulationThread.IsAlive Then
            simulationThread.Join(500)
        End If

        btnStartSimulation.Enabled = True
        btnStopSimulation.Enabled = False
        btnSendCard.Enabled = True
        lstCards.Enabled = True
        btnAddCard.Enabled = True
        btnRemoveCard.Enabled = True
        txtNewCard.Enabled = True
    End Sub

    Private Sub SendCard_Click(sender As Object, e As EventArgs)
        If lstCards.SelectedIndex >= 0 Then
            SendCardToClients(lstCards.SelectedItem.ToString())
        Else
            MessageBox.Show("请从列表中选择一个卡号", "发送卡号", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub SimulateCardReading()
        Dim random As New Random()
        Dim interval As Integer = CInt(numInterval.Value)
        Dim currentIndex As Integer = 0

        While isSimulating
            ' 选择要发送的卡号
            Dim cardNumber As String
            If chkRandomOrder.Checked Then
                ' 随机选择卡号
                cardNumber = simulatedCards(random.Next(0, simulatedCards.Count))
            Else
                ' 顺序发送卡号
                cardNumber = simulatedCards(currentIndex)
                currentIndex = (currentIndex + 1) Mod simulatedCards.Count
            End If

            ' 发送卡号
            SendCardToClients(cardNumber)

            ' 更新状态
            UpdateStatus($"已发送卡号: {cardNumber} (间隔: {interval}ms)")

            ' 等待指定间隔
            Thread.Sleep(interval)
        End While
    End Sub

    Private Sub SendCardToClients(cardNumber As String)
        If Not isServerRunning OrElse activeClients.Count = 0 Then
            UpdateStatus("没有连接的客户端", True)
            Return
        End If

        Try
            ' 模拟真实RFID设备的数据格式
            Dim data As New List(Of Byte)

            ' 添加起始符 (STX)
            data.Add(&H2)

            ' 添加卡号 (ASCII格式)
            data.AddRange(Encoding.ASCII.GetBytes(cardNumber))

            ' 添加结束符 (ETX)
            data.Add(&H3)

            ' 添加回车换行
            data.Add(13)
            data.Add(10)

            ' 创建数据副本用于发送
            Dim dataBytes As Byte() = data.ToArray()

            ' 发送数据到所有客户端
            Dim clientsToRemove As New List(Of TcpClient)
            SyncLock activeClients
                For Each client In activeClients
                    If client.Connected Then
                        Try
                            Dim stream As NetworkStream = client.GetStream()
                            stream.Write(dataBytes, 0, dataBytes.Length)
                            stream.Flush()
                        Catch
                            clientsToRemove.Add(client)
                        End Try
                    Else
                        clientsToRemove.Add(client)
                    End If
                Next

                ' 移除断开连接的客户端
                For Each client In clientsToRemove
                    activeClients.Remove(client)
                Next
            End SyncLock

            If clientsToRemove.Count > 0 Then
                UpdateStatus($"移除了 {clientsToRemove.Count} 个断开连接的客户端")
            End If

            ' 更新状态
            UpdateStatus($"已发送卡号: {cardNumber}")
        Catch ex As Exception
            UpdateStatus($"发送失败: {ex.Message}", True)
        End Try
    End Sub

    Private Sub UpdateStatus(message As String, Optional isError As Boolean = False)
        If Me.InvokeRequired Then
            Me.Invoke(New Action(Of String, Boolean)(AddressOf UpdateStatus), message, isError)
            Return
        End If

        lblStatus.Text = "状态: " & message
        lblStatus.ForeColor = If(isError, Color.DarkRed, Color.DarkGreen)
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' 清理资源
        isSimulating = False
        isServerRunning = False

        If simulationThread IsNot Nothing AndAlso simulationThread.IsAlive Then
            simulationThread.Join(500)
        End If

        If serverThread IsNot Nothing AndAlso serverThread.IsAlive Then
            serverThread.Join(500)
        End If

        If listener IsNot Nothing Then
            listener.Stop()
        End If

        For Each client In activeClients
            Try
                client.Close()
            Catch
            End Try
        Next
    End Sub
End Class
