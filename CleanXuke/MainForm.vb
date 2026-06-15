Imports System.IO

Public Class MainForm
    ' 保存选中的文件夹路径
    Private selectedFolderPath As String = Nothing

    Private Sub btnSelectFolder_Click(sender As Object, e As EventArgs) Handles btnSelectFolder.Click
        Using fileDialog As New System.Windows.Forms.OpenFileDialog()
            fileDialog.Title = "请选择文件夹中的任意一个文件"
            fileDialog.Filter = "所有文件|*.*"
            fileDialog.Multiselect = False
            fileDialog.InitialDirectory = "C:\Users\11043787\Pictures\fff"

            If fileDialog.ShowDialog() = DialogResult.OK Then
                ' 获取文件所在的文件夹路径
                Dim selectedFile As String = fileDialog.FileName
                selectedFolderPath = Path.GetDirectoryName(selectedFile)

                txtFolderPath.Text = selectedFolderPath
                btnUnlock.Enabled = True
                AppendLog($"已选择文件夹: {selectedFolderPath}")
                AppendLog($"(通过文件: {Path.GetFileName(selectedFile)})")
            End If
        End Using
    End Sub

    Private Sub btnUnlock_Click(sender As Object, e As EventArgs) Handles btnUnlock.Click
        If String.IsNullOrEmpty(selectedFolderPath) OrElse Not Directory.Exists(selectedFolderPath) Then
            System.Windows.Forms.MessageBox.Show("请先选择文件夹！", "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error)
            Return
        End If

        Try
            ' 清空日志
            txtLog.Clear()
            AppendLog("========================================")
            AppendLog($"开始时间: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}")
            AppendLog($"目标文件夹: {selectedFolderPath}")
            AppendLog("========================================")
            AppendLog("")

            ' 显示进度
            lblStatus.Text = "正在处理..."
            System.Windows.Forms.Application.DoEvents()

            Dim successCount As Integer = 0
            Dim failCount As Integer = 0
            Dim skipCount As Integer = 0

            ' 获取文件夹中的所有文件（包括子文件夹）
            Dim files() As String = Directory.GetFiles(selectedFolderPath, "*.*", SearchOption.AllDirectories)
            Dim totalCount As Integer = files.Length

            AppendLog($"找到 {totalCount} 个文件，开始处理...")
            AppendLog("")

            For Each filePath As String In files
                Try
                    ' 直接尝试删除 Zone.Identifier，不预先检查
                    Dim result As Boolean = UnlockFile(filePath)
                    If result Then
                        successCount += 1
                        AppendLog($"[成功] {filePath}")
                    Else
                        skipCount += 1
                    End If
                Catch ex As Exception
                    failCount += 1
                    AppendLog($"[异常] {filePath} - {ex.Message}")
                End Try

                ' 更新状态
                lblStatus.Text = $"正在处理: {Path.GetFileName(filePath)} (成功: {successCount}, 跳过: {skipCount}, 失败: {failCount})"
                System.Windows.Forms.Application.DoEvents()
            Next

            AppendLog("")
            AppendLog("========================================")
            AppendLog($"处理完成时间: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}")
            AppendLog("========================================")
            AppendLog($"总文件数: {totalCount}")
            AppendLog($"成功解除阻止: {successCount} 个文件")
            AppendLog($"无需解除: {skipCount} 个文件")
            AppendLog($"处理失败: {failCount} 个文件")
            AppendLog("========================================")

            Dim resultMessage As String = $"处理完成！" & vbCrLf & vbCrLf &
                                         $"成功解除阻止: {successCount} 个文件" & vbCrLf &
                                         $"无需解除: {skipCount} 个文件" & vbCrLf &
                                         $"处理失败: {failCount} 个文件"

            System.Windows.Forms.MessageBox.Show(resultMessage, "完成", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information)
            lblStatus.Text = "处理完成"

        Catch ex As Exception
            AppendLog($"")
            AppendLog($"[严重错误] {ex.Message}")
            AppendLog($"[堆栈跟踪] {ex.StackTrace}")
            System.Windows.Forms.MessageBox.Show($"发生错误: {ex.Message}", "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error)
            lblStatus.Text = "处理失败"
        End Try
    End Sub

    ''' <summary>
    ''' 解除单个文件的阻止状态
    ''' </summary>
    Private Function UnlockFile(ByVal filePath As String) As Boolean
        Try
            ' 使用 PowerShell 命令删除 Zone.Identifier 备用数据流
            Dim escapedPath As String = filePath.Replace("'", "''")
            Dim psi As New System.Diagnostics.ProcessStartInfo()
            psi.FileName = "powershell.exe"

            ' 构建 PowerShell 命令 - 先检查是否存在，存在则删除
            Dim psCommand As String = "if (Test-Path -LiteralPath '" & escapedPath & ":Zone.Identifier') { Remove-Item -LiteralPath '" & escapedPath & ":Zone.Identifier' -Force -ErrorAction Stop; exit 0 } else { exit 1 }"
            psi.Arguments = "-NoProfile -WindowStyle Hidden -Command """ & psCommand & """"

            psi.UseShellExecute = False
            psi.CreateNoWindow = True
            psi.RedirectStandardOutput = True
            psi.RedirectStandardError = True

            Using process As System.Diagnostics.Process = System.Diagnostics.Process.Start(psi)
                Dim stderr As String = process.StandardError.ReadToEnd()
                Dim stdout As String = process.StandardOutput.ReadToEnd()
                process.WaitForExit(5000) ' 等待最多5秒

                If process.ExitCode = 0 Then
                    Return True
                ElseIf process.ExitCode = 1 Then
                    ' 文件没有 Zone.Identifier，无需处理
                    Return False
                Else
                    AppendLog($"  PowerShell 错误: {stderr}")
                    Return False
                End If
            End Using
        Catch ex As Exception
            AppendLog($"  处理异常: {ex.Message}")
            Return False
        End Try
    End Function

    ''' <summary>
    ''' 添加日志到文本框
    ''' </summary>
    Private Sub AppendLog(ByVal message As String)
        If txtLog.InvokeRequired Then
            txtLog.Invoke(New Action(Of String)(AddressOf AppendLog), message)
        Else
            txtLog.AppendText(message & vbCrLf)
            ' 自动滚动到底部
            txtLog.SelectionStart = txtLog.Text.Length
            txtLog.ScrollToCaret()
        End If
    End Sub

    Private Sub btnCopyLog_Click(sender As Object, e As EventArgs) Handles btnCopyLog.Click
        If Not String.IsNullOrEmpty(txtLog.Text) Then
            System.Windows.Forms.Clipboard.SetText(txtLog.Text)
            System.Windows.Forms.MessageBox.Show("日志已复制到剪贴板！", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information)
        End If
    End Sub

End Class
