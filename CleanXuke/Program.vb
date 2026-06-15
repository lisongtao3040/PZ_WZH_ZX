Module Program
    ''' <summary>
    ''' 应用程序的主入口点。
    ''' </summary>
    <STAThread()>
    Sub Main()
        System.Windows.Forms.Application.EnableVisualStyles()
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(False)
        System.Windows.Forms.Application.Run(New MainForm())
    End Sub
End Module
