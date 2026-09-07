Imports Microsoft.VisualBasic
Imports System.Data


Public Class m_first_chk_step2BC

    Private DA As New m_first_chk_step2DA


    Public Function Getm_first_chk_step2(ByVal CD As String, ByVal line_cd As String) As DataTable
        Return DA.Getm_first_chk_step2(CD, line_cd)
    End Function


    Public Function Getm_m_first_chk_cds(ByVal CD As String, ByVal line_cd As String) As DataTable
        Return DA.Getm_m_first_chk_cds(CD, line_cd)
    End Function

    Public Function Ins_m_first_chk_step2(ByVal CD As String, ByVal line_cd As String, ByVal user_cd As String) As String
        Return DA.Ins_m_first_chk_step2(CD, line_cd, user_cd)
    End Function

End Class
