Imports Microsoft.VisualBasic
Imports System.Data



Public Class t_scsjBC

    Private DA As New t_scsjDA

    Public Function GetScsj(ByVal department_cd As String, ByVal startDate As String, ByVal endDate As String) As DataTable

        Return DA.GetScsj(department_cd, startDate, endDate)
    End Function
End Class
