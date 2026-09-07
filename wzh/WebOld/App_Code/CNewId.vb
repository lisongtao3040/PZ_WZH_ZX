Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Public Class CNewId
    Public Sub New()
    End Sub

    Shared lockObject As Object = New Object()
    Shared idx As Integer = 0

    Public Shared Function NewId() As String
        SyncLock lockObject
            Dim id As String = DateTime.Now.ToString("yyMMddHHmmssffff")
            idx += 1
            If idx = 9999 Then idx = 0
            Return id & idx.ToString("D4")
        End SyncLock
    End Function
End Class
