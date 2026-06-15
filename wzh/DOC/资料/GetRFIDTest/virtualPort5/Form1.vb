Imports System.IO.Ports
Imports System.Threading

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        ' 定义串口
        Dim portName As String = "COM4" ' 串口名称
        Dim baudRate As Integer = 115200 ' 波特率

        ' 创建并配置 SerialPort 对象
        Using serialPort As New SerialPort(portName, baudRate)
            Try
                ' 打开串口
                serialPort.Open()
                Console.WriteLine("串口已打开")

                ' 循环监听
                While True
                    Console.WriteLine("等待扫描 RFID 卡...")
                    ' 模拟接收 RFID 数据
                    Dim rfidCardNumber As String = SimulateRFIDScan()

                    If Not String.IsNullOrEmpty(rfidCardNumber) Then
                        ' 发送 RFID 卡号
                        Dim dataToSend As Byte() = System.Text.Encoding.ASCII.GetBytes(rfidCardNumber)
                        serialPort.Write(dataToSend, 0, dataToSend.Length)
                        Console.WriteLine("已发送数据: " & rfidCardNumber)
                    End If

                    ' 暂停一段时间以防止过于快速的循环（可根据需要调整）
                    Thread.Sleep(2000) ' 2秒
                End While

            Catch ex As Exception
                Console.WriteLine("发生错误: " & ex.Message)
            Finally
                ' 确保在结束时关闭串口
                If serialPort.IsOpen Then
                    serialPort.Close()
                    Console.WriteLine("串口已关闭")
                End If
            End Try
        End Using

    End Sub

    ' 模拟 RFID 扫描
    Private Function SimulateRFIDScan() As String
        ' 这里可以随机生成一个 12 位的 RFID 卡号
        Dim random As New Random()
        Dim rfidCardNumber As String = ""

        ' 生成一个 12 位数字字符串
        For i As Integer = 1 To 12
            rfidCardNumber &= random.Next(0, 10).ToString()
        Next

        Return rfidCardNumber
    End Function
End Class
