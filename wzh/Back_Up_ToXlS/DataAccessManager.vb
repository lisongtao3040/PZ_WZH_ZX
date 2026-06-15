Option Explicit On
Option Strict On

Imports System.Net

''' <summary>
''' DBアクセスに関する管理クラス
''' </summary>
''' <remarks>
'''  初回アクセス時にのみ起動し、DB接続文字列を設定する。
''' </remarks>
Public NotInheritable Class DataAccessManager

    ''' <summary>
    ''' DB接続文字列の設定
    ''' </summary>
    ''' <remarks></remarks>
    Private Shared SqlConnStr As String = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
    Private Shared SqlConnStrOt2414 As String = System.Configuration.ConfigurationManager.ConnectionStrings("connectionStringOt2414").ConnectionString
    Private Shared SqlConnStrN9EMD99 As String = System.Configuration.ConfigurationManager.ConnectionStrings("connectionStringN9EMD99").ConnectionString
    Private Shared SqlConnStrCmsdn As String = System.Configuration.ConfigurationManager.ConnectionStrings("connectionStringCmsdn").ConnectionString

    ''' <summary>
    ''' DB接続文字列の取得
    ''' </summary>
    ''' <value>DB接続文字列</value>
    ''' <returns>DB接続文字列</returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property ConnStr() As String

        Get
            If Dns.GetHostName().ToLower = ("ot5600").ToLower Then
                Return SqlConnStr
            ElseIf Dns.GetHostName().ToLower = ("DESKTOP-N9EMD99").ToLower Then
                Return SqlConnStrN9EMD99
            ElseIf Dns.GetHostName().ToLower = ("172_30_0_3").ToLower Then
                Return SqlConnStrCmsdn
            Else
                Return SqlConnStr
            End If


        End Get
    End Property

End Class