
Imports System.IO.Ports
Imports System.Threading
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Net.Sockets
Imports System.Net


Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    ' 使用虚拟串口技术创建模拟COM端口
    Private WithEvents virtualPort As New VirtualSerialPort("COM4")
    Private simulatedCards As New List(Of String)()
    Private simulationThread As Thread
    Private isSimulating As Boolean = False

    Public Sub New()
        InitializeComponent()
        InitializeForm()
        InitializeSimulatedCards()
        InitializeVirtualPort()
    End Sub

    Private Sub InitializeForm()
        Me.Text = "RFID 虚拟设备模拟器 (COM4)"
        Me.Size = New Size(600, 450)
        Me.Font = New Font("Segoe UI", 10)

        ' 创建控件
        CreateControls()
    End Sub

    Private Sub CreateControls()
        ' 标题
        Dim lblTitle As New Label With {
            .Text = "RFID 虚拟设备模拟器 - COM4",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .Location = New Point(20, 15),
            .AutoSize = True
        }
        Me.Controls.Add(lblTitle)

        ' 状态标签
        lblStatus = New Label With {
            .Text = "状态: COM4 准备就绪",
            .Location = New Point(20, 50),
            .AutoSize = True,
            .ForeColor = Color.DarkGreen
        }
        Me.Controls.Add(lblStatus)

        ' 卡号列表标签
        Dim lblCards As New Label With {
            .Text = "模拟卡号列表:",
            .Location = New Point(20, 85),
            .AutoSize = True
        }
        Me.Controls.Add(lblCards)

        ' 卡号列表框
        lstCards = New ListBox With {
            .Location = New Point(20, 110),
            .Size = New Size(550, 120),
            .SelectionMode = SelectionMode.MultiExtended
        }
        Me.Controls.Add(lstCards)

        ' 添加卡号区域
        txtNewCard = New TextBox With {
            .Location = New Point(20, 245),
            .Size = New Size(300, 25)
        }
        Me.Controls.Add(txtNewCard)

        btnAddCard = New Button With {
            .Text = "添加卡号",
            .Location = New Point(330, 245),
            .Size = New Size(100, 30),
            .BackColor = Color.LightGreen
        }
        AddHandler btnAddCard.Click, AddressOf AddCard_Click
        Me.Controls.Add(btnAddCard)

        btnRemoveCard = New Button With {
            .Text = "移除选中",
            .Location = New Point(440, 245),
            .Size = New Size(130, 30),
            .BackColor = Color.LightCoral
        }
        AddHandler btnRemoveCard.Click, AddressOf RemoveCard_Click
        Me.Controls.Add(btnRemoveCard)

        ' 模拟控制区域
        Dim lblActions As New Label With {
            .Text = "模拟控制:",
            .Location = New Point(20, 290),
            .AutoSize = True
        }
        Me.Controls.Add(lblActions)

        btnStartSimulation = New Button With {
            .Text = "开始模拟",
            .Location = New Point(20, 320),
            .Size = New Size(120, 35),
            .BackColor = Color.LightBlue
        }
        AddHandler btnStartSimulation.Click, AddressOf StartSimulation_Click
        Me.Controls.Add(btnStartSimulation)

        btnStopSimulation = New Button With {
            .Text = "停止模拟",
            .Location = New Point(150, 320),
            .Size = New Size(120, 35),
            .BackColor = Color.LightSalmon,
            .Enabled = False
        }
        AddHandler btnStopSimulation.Click, AddressOf StopSimulation_Click
        Me.Controls.Add(btnStopSimulation)

        btnSendCard = New Button With {
            .Text = "发送选中卡号",
            .Location = New Point(280, 320),
            .Size = New Size(160, 35),
            .BackColor = Color.LightSkyBlue
        }
        AddHandler btnSendCard.Click, AddressOf SendCard_Click
        Me.Controls.Add(btnSendCard)

        ' 模拟设置
        Dim lblSettings As New Label With {
            .Text = "模拟设置:",
            .Location = New Point(20, 365),
            .AutoSize = True
        }
        Me.Controls.Add(lblSettings)

        Dim lblInterval As New Label With {
            .Text = "发送间隔 (ms):",
            .Location = New Point(150, 365),
            .AutoSize = True
        }
        Me.Controls.Add(lblInterval)

        numInterval = New NumericUpDown With {
            .Location = New Point(260, 363),
            .Size = New Size(80, 25),
            .Minimum = 500,
            .Maximum = 10000,
            .Value = 1500,
            .Increment = 100
        }
        Me.Controls.Add(numInterval)

        chkRandomOrder = New CheckBox With {
            .Text = "随机顺序发送",
            .Location = New Point(360, 365),
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

    Private Sub InitializeVirtualPort()
        ' 启动虚拟串口
        Try
            virtualPort.Start()
            lblStatus.Text = "状态: COM4 已激活 - 等待连接"
            lblStatus.ForeColor = Color.DarkBlue
        Catch ex As Exception
            lblStatus.Text = "状态: 无法创建虚拟端口 - " & ex.Message
            lblStatus.ForeColor = Color.DarkRed
        End Try
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
            SendCard(lstCards.SelectedItem.ToString())
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
            SendCard(cardNumber)

            ' 更新状态
            UpdateStatus($"已发送卡号: {cardNumber} (间隔: {interval}ms)")

            ' 等待指定间隔
            Thread.Sleep(interval)
        End While
    End Sub

    Private Sub SendCard(cardNumber As String)
        Try
            ' 模拟真实RFID设备的数据格式
            ' 典型格式: [STX] + 卡号数据 + [ETX]
            Dim data As New List(Of Byte)

            ' 添加起始符 (STX)
            data.Add(&H2)

            ' 添加卡号 (ASCII格式)
            data.AddRange(System.Text.Encoding.ASCII.GetBytes(cardNumber))

            ' 添加结束符 (ETX)
            data.Add(&H3)

            ' 添加回车换行 (可选)
            data.Add(13)
            data.Add(10)

            ' 发送数据
            virtualPort.SendData(data.ToArray())

            ' 更新状态
            UpdateStatus($"已发送卡号: {cardNumber}")
        Catch ex As Exception
            UpdateStatus($"发送失败: {ex.Message}", True)
        End Try
    End Sub

    Private Sub UpdateStatus(message As String, Optional isError As Boolean = False)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() UpdateStatus(message, isError))
            Return
        End If

        lblStatus.Text = "状态: " & message
        lblStatus.ForeColor = If(isError, Color.DarkRed, Color.DarkGreen)
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' 清理资源
        isSimulating = False
        If simulationThread IsNot Nothing AndAlso simulationThread.IsAlive Then
            simulationThread.Join(500)
        End If
        virtualPort.Stop()
    End Sub

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
End Class




' =============================================================================
' 虚拟串口实现类 - 创建模拟的COM4端口
' =============================================================================
Public Class VirtualSerialPort
    Private portName As String
    Private isRunning As Boolean = False
    Private receiveThread As Thread
    Private clients As New List(Of TcpClient)()
    Private listener As TcpListener

    Public Event DataReceived(data As Byte())
    Public Event PortStatusChanged(status As String)

    Public Sub New(port As String)
        portName = port
    End Sub

    Public Sub Start()
        If isRunning Then Return

        ' 使用TCP模拟串口 - 使用端口号映射COM端口 (COM4 -> 4004)
        Dim portNumber As Integer = 4000 + CInt(portName.Substring(3))

        Try
            listener = New TcpListener(IPAddress.Loopback, portNumber)
            listener.Start()

            isRunning = True
            receiveThread = New Thread(AddressOf ListenForClients)
            receiveThread.IsBackground = True
            receiveThread.Start()

            RaiseEvent PortStatusChanged($"{portName} 已启动 - 监听 TCP:{portNumber}")
        Catch ex As Exception
            Throw New Exception($"无法启动虚拟端口 {portName}: {ex.Message}")
        End Try
    End Sub

    Public Sub [Stop]()
        isRunning = False

        Try
            listener.Stop()

            For Each client In clients
                If client.Connected Then
                    client.Close()
                End If
            Next

            clients.Clear()

            If receiveThread IsNot Nothing AndAlso receiveThread.IsAlive Then
                receiveThread.Join(500)
            End If

            RaiseEvent PortStatusChanged($"{portName} 已停止")
        Catch ex As Exception
            ' 忽略停止时的错误
        End Try
    End Sub

    Public Sub SendData(data As Byte())
        If Not isRunning Then Return

        Try
            For Each client In clients.ToList()
                If client.Connected Then
                    Dim stream As NetworkStream = client.GetStream()
                    stream.Write(data, 0, data.Length)
                    stream.Flush()
                Else
                    clients.Remove(client)
                End If
            Next
        Catch ex As Exception
            ' 客户端连接断开
            'clients.RemoveWhere(Function(c) Not c.Connected)
            For i As Integer = clients.Count - 1 To 0 Step -1
                If Not clients(i).Connected Then
                    clients.RemoveAt(i)
                End If
            Next
        End Try
    End Sub

    Private Sub ListenForClients()
        While isRunning
            Try
                If listener.Pending() Then
                    Dim client As TcpClient = listener.AcceptTcpClient()
                    clients.Add(client)

                    Dim clientThread As New Thread(Sub() HandleClient(client))
                    clientThread.IsBackground = True
                    clientThread.Start()

                    RaiseEvent PortStatusChanged($"客户端已连接到 {portName}")
                End If
                Thread.Sleep(100)
            Catch ex As Exception
                If isRunning Then
                    RaiseEvent PortStatusChanged($"{portName} 监听错误: {ex.Message}")
                End If
                Exit While
            End Try
        End While
    End Sub

    Private Sub HandleClient(client As TcpClient)
        Dim buffer(1023) As Byte
        Dim stream As NetworkStream = client.GetStream()

        Try
            While client.Connected AndAlso isRunning
                If stream.DataAvailable Then
                    Dim bytesRead As Integer = stream.Read(buffer, 0, buffer.Length)
                    If bytesRead > 0 Then
                        Dim receivedData(bytesRead - 1) As Byte
                        Array.Copy(buffer, receivedData, bytesRead)
                        RaiseEvent DataReceived(receivedData)
                    End If
                End If
                Thread.Sleep(50)
            End While
        Catch ex As Exception
            ' 客户端断开连接
        Finally
            client.Close()
            clients.Remove(client)
            RaiseEvent PortStatusChanged($"客户端已断开 {portName}")
        End Try
    End Sub
End Class