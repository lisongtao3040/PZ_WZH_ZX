
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationSettings
Imports System.Collections.Generic

Imports SqlHelper.SqlHelper
Imports SqlHelper
Public Class m_first_chk_step2DA

    Public Function Getm_first_chk_step2(ByVal CD As String, ByVal line_cd As String) As DataTable

        'SQLコメント
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("*")
        sb.AppendLine("FROM m_first_chk_step2")
        sb.AppendLine("WHERE replace(CD,'-','')=@CD AND line_cd=@line_cd")

        'replace(c.cd,'-','') = '" & cd & "'
        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@CD", SqlDbType.NVarChar, 30, CD.Replace("-", "")))
        paramList.Add(MakeParam("@line_cd", SqlDbType.VarChar, 20, line_cd))

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_first_chk_step2", paramList.ToArray)

    End Function


    Public Function Getm_m_first_chk_cds(ByVal CD As String, ByVal line_cd As String) As DataTable

        'SQLコメント
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("*")
        sb.AppendLine("FROM m_first_chk_cds")
        sb.AppendLine("WHERE replace(CD,'-','')=@CD ")

        'replace(c.cd,'-','') = '" & cd & "'
        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@CD", SqlDbType.NVarChar, 30, CD.Replace("-", "")))
        paramList.Add(MakeParam("@line_cd", SqlDbType.VarChar, 20, line_cd))

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "m_first_chk_cds", paramList.ToArray)

    End Function




    '登录新的检查项目
    Function Ins_m_first_chk_step2(ByVal CD As String, ByVal line_cd As String, ByVal user_cd As String) As String

        Dim sb As New StringBuilder

        sb.AppendLine("IF NOT EXISTS (select 1 from m_first_chk_step2 where CD = '" & CD & "' and line_cd = '" & line_cd & "')")
        sb.AppendLine(" BEGIN")

        sb.AppendLine("INSERT INTO m_first_chk_step2")
        sb.AppendLine(" (")
        sb.AppendLine("CD")
        sb.AppendLine(",line_cd")
        sb.AppendLine(",chk_date")
        sb.AppendLine(",upd_sya")
        sb.AppendLine(",upd_date")
        sb.AppendLine(",ins_sya")
        sb.AppendLine(",ins_date")
        sb.AppendLine(" )")
        sb.AppendLine("SELECT")           '	ck_id
        sb.AppendLine("      '" & CD & "' AS cd")               '	cd
        sb.AppendLine("      ,'" & line_cd & "' AS line_cd")     '	line_cd
        sb.AppendLine("      ,getdate() as chk_date")            '	upd_date
        sb.AppendLine("      ,'" & user_cd & "'")                '	upd_user
        sb.AppendLine("      ,getdate() as upd_date")            '	upd_date
        sb.AppendLine("      ,'" & user_cd & "'")                '	ins_user
        sb.AppendLine("      ,getdate() as ins_date")            '	ins_date

        sb.AppendLine(" END")


        Try
            '登录检查结果
            ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())
            sb.Length = 0
            sb.Clear()
            sb = Nothing
            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

End Class
