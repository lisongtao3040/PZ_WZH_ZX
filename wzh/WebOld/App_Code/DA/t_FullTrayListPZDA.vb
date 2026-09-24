Imports System.Data
Imports SqlHelper.SqlHelper
Imports SqlHelper

''' <summary>
''' 満台盤一覧（品質版）用データアクセス
''' 対象ビュー：[v_TwoMetresFullTrayDetailPinzhi]
''' </summary>
Public Class t_FullTrayListPZDA

    ' 部門で絞り込むビューの列名（'二制品质' などの部門名が入っている列）
    ' ※ビュー側の列名が department_cd の場合は、ここを "[department_cd]" に変更する
    Private Const DEPARTMENT_COLUMN As String = "[stationDepartment]"

    ''' <summary>
    ''' m_user から department_cd を取得する（閲覧可能部門の判定用）
    ''' </summary>
    Public Function GetUserDepartmentCd(ByVal userCd As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT [department_cd]")
        sb.AppendLine("FROM [m_user]")
        sb.AppendLine("WHERE [user_cd] = '" & userCd.Replace("'", "''") & "'")

        ' 第4引数はユーザーごとに変える（万一キャッシュ用途でも他人の部門を返さないように）
        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetUserDepartmentCd_" & userCd)
    End Function

    ''' <summary>
    ''' 満台盤明細（品質版）を取得する
    ''' departmentInClause：閲覧可能部門の IN 句（例：'一制品质','二制品质'）
    ''' ※取得元DBは既存の v_TwoMetresFullTrayDetail と同じ（scgl_PeiSong）と想定
    ''' </summary>
    Public Function GetFullTrayListPinzhi(ByVal departmentInClause As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("    [stationDepartment]")
        sb.AppendLine("    ,[stationNo]")
        sb.AppendLine("    ,[existTrolleyNo]")
        sb.AppendLine("    ,[operatorLine]")
        sb.AppendLine("    ,[productCode]")
        sb.AppendLine("    ,[packageAmount]")
        sb.AppendLine("    ,[destination]")
        sb.AppendLine("    ,[BianCode]")
        sb.AppendLine("    ,[Dn]")
        sb.AppendLine("    ,[jizhong]")
        sb.AppendLine("    ,[lineCodeShort]")
        sb.AppendLine("FROM [v_TwoMetresFullTrayDetailPinzhi]")
        If departmentInClause = "" Then
            ' 閲覧可能部門が無い場合は１件も返さない
            sb.AppendLine("WHERE 1 = 0")
        Else
            sb.AppendLine("WHERE " & DEPARTMENT_COLUMN & " IN (" & departmentInClause & ")")
        End If
        sb.AppendLine("ORDER BY [stationNo], [existTrolleyNo]")

        Return FillData(DataAccessManager.TCMConnStr, CommandType.Text, sb.ToString(), "GetFullTrayListPinzhi")
    End Function

End Class
