Imports System.Data
Imports Microsoft.VisualBasic
Imports SqlHelper.SqlHelper
Imports SqlHelper

Public Class t_BuLiangBC
    Private DA As New t_BuLiangDA


    Public Function GetBuLiangList(ByVal cd As String, ByVal no As String, ByVal rinei As String, ByVal department_cd As String) As DataTable
        Return DA.GetBuLiangList(cd, no, rinei, department_cd)
    End Function

End Class
