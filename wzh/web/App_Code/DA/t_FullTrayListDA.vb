Imports System.Data
Imports SqlHelper.SqlHelper
Imports SqlHelper

Public Class t_FullTrayListDA

    Public Function GetFullTrayList() As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("    [innerCodeDes]")
        sb.AppendLine("    ,[stationNo]")
        sb.AppendLine("    ,[stationUse]")
        sb.AppendLine("    ,[trayNo]")
        sb.AppendLine("    ,[Ttxt]")
        sb.AppendLine("    ,[OrderNo]")
        sb.AppendLine("    ,[sapCode]")
        sb.AppendLine("    ,[packageAmount]")
        sb.AppendLine("    ,[destination]")
        sb.AppendLine("    ,[jizhong]")
        sb.AppendLine("    ,[lineCodeShort]")
        'sb.AppendLine("FROM [10.160.192.127].[scgl_PeiSong].[dbo].[v_TwoMetresFullTrayDetail]")
        sb.AppendLine("FROM [v_TwoMetresFullTrayDetail]")
        sb.AppendLine("ORDER BY [trayNo]")

        Return FillData(DataAccessManager.TCMConnStr, CommandType.Text, sb.ToString(), "GetFullTrayList")
    End Function

    ' m_line から line_cd・line_name を全件取得
    Public Function GetLineNames() As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT [line_cd], [line_name] FROM [m_line]")
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetLineNames")
    End Function

    ' t_check から no・result を取得（最新1件ずつ、OrderNo IN句で絞り込み）
    Public Function GetCheckResult(ByVal nosInClause As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT no, result")
        sb.AppendLine("FROM (")
        sb.AppendLine("    SELECT")
        sb.AppendLine("        no")
        sb.AppendLine("        ,result")
        sb.AppendLine("        ,ROW_NUMBER() OVER(PARTITION BY REPLACE(cd, '-', ''), no ORDER BY ck_id DESC) AS rn")
        sb.AppendLine("    FROM t_check")
        sb.AppendLine("    WHERE no IN (" & nosInClause & ")")
        sb.AppendLine(") t")
        sb.AppendLine("WHERE rn = 1")

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetCheckResult")
    End Function

End Class
