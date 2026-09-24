Imports System.Data
Imports System.Collections.Generic

''' <summary>
''' 満台盤一覧（品質版）業務クラス
''' </summary>
Public Class t_FullTrayListPZBC

    Private DA As New t_FullTrayListPZDA

    ' m_user.department_cd に含まれる数字 → 閲覧できる部門名
    Private Shared ReadOnly DEPARTMENT_MAP As New Dictionary(Of Char, String) From {
        {"1"c, "一制品质"},
        {"2"c, "二制品质"},
        {"3"c, "三制品质"},
        {"4"c, "国内贩卖"}
    }

    ''' <summary>
    ''' m_user の department_cd（生の値）を返す
    ''' </summary>
    Public Function GetDepartmentCd(ByVal userCd As String) As String
        userCd = userCd.Trim()
        If userCd = "" Then Return ""

        Dim dt As DataTable = DA.GetUserDepartmentCd(userCd)
        If dt.Rows.Count = 0 Then Return ""

        Return dt.Rows(0)("department_cd").ToString().Trim()
    End Function

    ''' <summary>
    ''' ユーザーが閲覧できる部門名を返す（m_user.department_cd の 1～4 を判定）
    ''' 例："12" → 一制品质＋二制品质、"4" → 国内贩卖、該当なし → 空
    ''' </summary>
    Public Function GetUserDepartments(ByVal userCd As String) As List(Of String)
        Dim departments As New List(Of String)

        Dim departmentCd As String = GetDepartmentCd(userCd)
        For Each ch As Char In departmentCd
            If DEPARTMENT_MAP.ContainsKey(ch) AndAlso Not departments.Contains(DEPARTMENT_MAP(ch)) Then
                departments.Add(DEPARTMENT_MAP(ch))
            End If
        Next

        Return departments
    End Function

    ''' <summary>
    ''' 満台盤明細（品質版）を取得する（閲覧可能部門のみ）
    ''' </summary>
    Public Function GetFullTrayListPinzhi(ByVal departments As List(Of String)) As DataTable
        ' IN 句を作成（シングルクォートエスケープ）
        Dim parts As New List(Of String)
        For Each department As String In departments
            parts.Add("'" & department.Replace("'", "''") & "'")
        Next

        Return DA.GetFullTrayListPinzhi(String.Join(",", parts.ToArray()))
    End Function

End Class
