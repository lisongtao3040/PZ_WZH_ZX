Imports System
Imports System.Runtime.InteropServices
Imports System.Text

Public Class IniFile
    <DllImport("kernel32.dll")>
    Private Shared Function GetPrivateProfileString(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As StringBuilder, ByVal nSize As UInteger, ByVal lpFileName As String) As UInteger

    End Function

    <DllImport("kernel32.dll")>
    Private Shared Function GetPrivateProfileInt(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal nDefault As Integer, ByVal lpFileName As String) As UInteger

    End Function
    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function WritePrivateProfileString(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Boolean
    End Function
    Public Property FilePath As String

    Public Sub New(ByVal filePath As String)
        Me.FilePath = filePath
    End Sub

    Public Function GetString(ByVal section As String, ByVal key As String, ByVal Optional defaultValue As String = "") As String
        Dim stringBuilder As StringBuilder = New StringBuilder(1024)
        IniFile.GetPrivateProfileString(section, key, defaultValue, stringBuilder, CUInt(stringBuilder.Capacity), Me.FilePath)
        Return stringBuilder.ToString()
    End Function

    Public Function GetInt(ByVal section As String, ByVal key As String, ByVal Optional defaultValue As Integer = 0) As Integer
        Return CInt(IniFile.GetPrivateProfileInt(section, key, defaultValue, Me.FilePath))
    End Function

    Public Function WriteString(ByVal section As String, ByVal key As String, ByVal value As String) As Boolean
        Return IniFile.WritePrivateProfileString(section, key, value, Me.FilePath)
    End Function
End Class
