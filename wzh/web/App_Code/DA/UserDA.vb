Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationSettings
Imports System.Collections.Generic

Imports SqlHelper.SqlHelper
Imports SqlHelper


Public Class UserDA


    'Public SqlHelper As New Global.SqlHelper.SqlHelper

    Public Function GetUser(ByVal user_cd As String, ByVal password As String) As DataTable

        'SQLコメント
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("*")
        sb.AppendLine("FROM m_user")
        sb.AppendLine("WHERE user_cd=@user_cd AND password=@password")
        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@user_cd", SqlDbType.NVarChar, 20, user_cd))
        paramList.Add(MakeParam("@password", SqlDbType.NVarChar, 20, password))

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_user", paramList.ToArray)

    End Function

    Public Function GetUser(ByVal user_cd As String) As DataTable

        'SQLコメント
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("a.*")
        sb.AppendLine(",b.line_name")
        sb.AppendLine("FROM m_user a")
        sb.AppendLine("LEFT JOIN m_line b")
        sb.AppendLine("on a.line_cd=b.line_cd")
        sb.AppendLine("WHERE user_cd=@user_cd")
        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@user_cd", SqlDbType.NVarChar, 20, user_cd))

        Return SqlHelper.SqlHelper.FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_user", paramList.ToArray)

    End Function


    Public Function GetUser() As DataTable

        'SQLコメント
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("*")
        sb.AppendLine("FROM m_user")

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_user")

    End Function

End Class
