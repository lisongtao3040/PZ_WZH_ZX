Imports System
Imports System.Threading


Public Class ChkWaiter
    Private Function ChkWait_Timer(ByVal type As ChkWaiter.CallType, ByVal waittime As UShort) As Boolean
        Dim result As Boolean = True

        Try

            If type = ChkWaiter.CallType.Start Then
                Me.StTime = (Environment.TickCount And Integer.MaxValue)
                result = False
            ElseIf type = ChkWaiter.CallType.Wait Then
                Dim num As Integer = Environment.TickCount And Integer.MaxValue

                If num < Me.StTime Then

                    If 2147483647 + (num - Me.StTime) > CInt(waittime) Then
                        result = True
                    End If
                ElseIf num - Me.StTime > CInt(waittime) Then
                    result = True
                End If
            End If

        Catch
            Me.StTime = (Environment.TickCount And Integer.MaxValue)
        End Try

        Return result
    End Function

    Public Property ExitTimer As Boolean
        Get
            Return Me.p_Exit
        End Get
        Set(ByVal value As Boolean)
            Me.p_Exit = value
        End Set
    End Property

    Public Function Wait(ByVal timeout As UShort) As Boolean
        Me.ChkWait_Timer(ChkWaiter.CallType.Start, timeout)

        Do
            Thread.Sleep(1)

            If Me.ExitTimer Then
                Return True
            End If
        Loop While Not Me.ChkWait_Timer(ChkWaiter.CallType.Wait, timeout)

        Return False
    End Function

    Private StTime As Integer
    Private p_Exit As Boolean

    Private Enum CallType
        Start
        Wait
    End Enum
End Class
