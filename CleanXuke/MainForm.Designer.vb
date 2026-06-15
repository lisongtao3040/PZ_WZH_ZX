<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnSelectFolder = New System.Windows.Forms.Button()
        Me.txtFolderPath = New System.Windows.Forms.TextBox()
        Me.btnUnlock = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.txtLog = New System.Windows.Forms.TextBox()
        Me.btnCopyLog = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnSelectFolder
        '
        Me.btnSelectFolder.Location = New System.Drawing.Point(338, 12)
        Me.btnSelectFolder.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnSelectFolder.Name = "btnSelectFolder"
        Me.btnSelectFolder.Size = New System.Drawing.Size(75, 24)
        Me.btnSelectFolder.TabIndex = 0
        Me.btnSelectFolder.Text = "选择文件夹"
        Me.btnSelectFolder.UseVisualStyleBackColor = True
        '
        'txtFolderPath
        '
        Me.txtFolderPath.Location = New System.Drawing.Point(15, 14)
        Me.txtFolderPath.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtFolderPath.Name = "txtFolderPath"
        Me.txtFolderPath.ReadOnly = True
        Me.txtFolderPath.Size = New System.Drawing.Size(316, 19)
        Me.txtFolderPath.TabIndex = 1
        '
        'btnUnlock
        '
        Me.btnUnlock.Location = New System.Drawing.Point(15, 48)
        Me.btnUnlock.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnUnlock.Name = "btnUnlock"
        Me.btnUnlock.Size = New System.Drawing.Size(398, 32)
        Me.btnUnlock.TabIndex = 2
        Me.btnUnlock.Text = "解除阻止"
        Me.btnUnlock.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(15, 96)
        Me.lblStatus.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(59, 12)
        Me.lblStatus.TabIndex = 3
        Me.lblStatus.Text = "等待操作..."
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft YaHei UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(15, 80)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(257, 17)
        Me.lblTitle.TabIndex = 4
        Me.lblTitle.Text = "解除文件安全阻止标记工具 (Zone.Identifier)"
        '
        'txtLog
        '
        Me.txtLog.Font = New System.Drawing.Font("Consolas", 9.0!)
        Me.txtLog.Location = New System.Drawing.Point(15, 124)
        Me.txtLog.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ReadOnly = True
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLog.Size = New System.Drawing.Size(398, 161)
        Me.txtLog.TabIndex = 5
        '
        'btnCopyLog
        '
        Me.btnCopyLog.Location = New System.Drawing.Point(11, 303)
        Me.btnCopyLog.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnCopyLog.Name = "btnCopyLog"
        Me.btnCopyLog.Size = New System.Drawing.Size(75, 24)
        Me.btnCopyLog.TabIndex = 6
        Me.btnCopyLog.Text = "复制日志"
        Me.btnCopyLog.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(428, 353)
        Me.Controls.Add(Me.btnCopyLog)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.btnUnlock)
        Me.Controls.Add(Me.txtFolderPath)
        Me.Controls.Add(Me.btnSelectFolder)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "文件阻止解除工具"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnSelectFolder As System.Windows.Forms.Button
    Friend WithEvents txtFolderPath As System.Windows.Forms.TextBox
    Friend WithEvents btnUnlock As System.Windows.Forms.Button
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents btnCopyLog As System.Windows.Forms.Button

End Class
