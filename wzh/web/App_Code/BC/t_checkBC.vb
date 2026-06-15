Imports System.Data
Imports Microsoft.VisualBasic
Imports SqlHelper.SqlHelper
Imports SqlHelper

Public Class t_checkBC
    Private DA As New t_checkDA


    Public Function GetKmDt(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String) As DataTable
        Return DA.GetKmDt(cd, no, ck_id, user_cd)
    End Function



    '登录新的检查项目
    Public Function CreateNewChk(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String, ByVal department_cd As String, ByVal line_cd As String, ByVal yotei_chk_date As String) As String
        Return DA.CreateNewChk(cd, no, ck_id, user_cd, department_cd, line_cd, yotei_chk_date)
    End Function

    Public Function CreateNewChkNoPlan(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String, ByVal department_cd As String, ByVal line_cd As String, ByVal yotei_chk_date As String) As String
        Return DA.CreateNewChkNoPlan(cd, no, ck_id, user_cd, department_cd, line_cd, yotei_chk_date)
    End Function

    '登录新的检查项目
    Public Function CreateNewChkHand(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal user_cd As String, ByVal department_cd As String, ByVal line_cd As String, ByVal suu As String, ByVal result As String) As String
        Return DA.CreateNewChkHand(cd, no, ck_id, user_cd, department_cd, line_cd, suu, result)
    End Function

    Public Function GetCheckToolsData(ByVal ck_id As String, ByVal CLoginInfo As CLoginInfo) As DataTable
        Return DA.GetCheckToolsData(ck_id, CLoginInfo)
    End Function

    Public Function GetFirstCheck(ByVal cd As String) As String
        Return DA.GetFirstCheck(cd)
    End Function

    Public Function Gettongyong_cd(ByVal cd As String) As String
        Return DA.Gettongyong_cd(cd)
    End Function


    Public Function GetFirstCheck_step2(ByVal cd As String, ByVal line_cd As String) As String
        Return DA.GetFirstCheck_step2(cd, line_cd)
    End Function

    Public Function Gettongyong_cd_step2(ByVal cd As String) As String
        Return DA.Gettongyong_cd_step2(cd)
    End Function


    Public Function CreateReChk(ByVal cd As String, ByVal no As String, ByVal ck_id As String, ByVal re_ck_id As String, ByVal user_cd As String, ByVal department_cd As String, ByVal line_cd As String, ByVal yotei_chk_date As String) As String
        Return DA.CreateReChk(cd, no, ck_id, re_ck_id, user_cd, department_cd, line_cd, yotei_chk_date)
    End Function


End Class
