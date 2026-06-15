Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports CardTestVB.SampleApp



Public Class KeyboardHook
    ' Hook procedure delegate definition
    Public Delegate Function HookProc(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    ' Importing necessary functions from user32.dll and kernel32.dll
    <DllImport("user32.dll")>
    Private Shared Function SetWindowsHookEx(ByVal idHook As Integer, ByVal lpfn As HookProc, ByVal hMod As IntPtr, ByVal dwThreadId As UInteger) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Private Shared Function UnhookWindowsHookEx(ByVal hhk As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function CallNextHookEx(ByVal hhk As IntPtr, ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("kernel32.dll")>
    Private Shared Function GetModuleHandle(ByVal lpModuleName As String) As IntPtr
    End Function


    Public Shared Sub Start()
        KeyboardHook.hookID = KeyboardHook.SetHook(KeyboardHook.proc)
    End Sub

    Public Shared Sub [Stop]()
        KeyboardHook.UnhookWindowsHookEx(KeyboardHook.hookID)
    End Sub

    Private Shared Function SetHook(ByVal proc As KeyboardHook.HookProc) As IntPtr
        Dim result As IntPtr

        Using currentProcess As Process = Process.GetCurrentProcess()

            Using mainModule As ProcessModule = currentProcess.MainModule
                result = KeyboardHook.SetWindowsHookEx(13, proc, KeyboardHook.GetModuleHandle(mainModule.ModuleName), 0UI)
            End Using
        End Using

        Return result
    End Function

    Private Shared Function HookCallback(ByVal nCode As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
        If nCode >= 0 AndAlso wParam = CType(256, IntPtr) Then
            Dim num As Integer = Marshal.ReadInt32(lParam)

            If num = CInt(MainForm.StartKey) Then
                MainForm.isStartKeyPressed = True
            End If

            If num = CInt(MainForm.StopKey) Then
                MainForm.isStopKeyPressed = True
            End If
        End If

        Return KeyboardHook.CallNextHookEx(KeyboardHook.hookID, nCode, wParam, lParam)
    End Function

    Private Shared proc As KeyboardHook.HookProc = New KeyboardHook.HookProc(AddressOf KeyboardHook.HookCallback)
    Private Shared hookID As IntPtr = IntPtr.Zero
    Private Const WH_KEYBOARD_LL As Integer = 13
    Private Const WM_KEYDOWN As Integer = 256


End Class
