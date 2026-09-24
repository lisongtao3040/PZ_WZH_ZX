Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data
Imports System.Collections.Generic
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

''' <summary>
''' 満台盤一覧（品質版）API
''' データ元：[v_TwoMetresFullTrayDetailPinzhi]
''' </summary>
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Public Class FullTrayListPZApi
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' 取得満托盘一览数据（品质版）
    ''' 画面の lblUserCd（ユーザーコード）から閲覧可能部門を判定し、その部門のデータだけ返す
    ''' ビューの全列をそのまま返す（項目追加時もここは変更しなくてよい）
    ''' </summary>
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Function GetData(ByVal userCd As String) As String
        Dim jss As New JavaScriptSerializer()
        Try
            Dim BC As New t_FullTrayListPZBC()

            ' 閲覧できる部門（m_user.department_cd の 1～4 で判定）
            Dim departments As List(Of String) = BC.GetUserDepartments(userCd)
            Dim dt As DataTable = BC.GetFullTrayListPinzhi(departments)

            Dim list As New List(Of Dictionary(Of String, Object))
            For Each row As DataRow In dt.Rows
                Dim item As New Dictionary(Of String, Object)
                For Each col As DataColumn In dt.Columns
                    item(col.ColumnName) = row(col).ToString()
                Next
                list.Add(item)
            Next

            ' 閲覧可能部門が無い場合は画面に知らせる
            Dim message As String = ""
            If departments.Count = 0 Then
                message = "该用户没有可查看的部门（m_user.department_cd 未包含 1～4）"
            End If

            Return jss.Serialize(New With {.success = True, .data = list, .message = message})
        Catch ex As Exception
            Return jss.Serialize(New With {.success = False, .message = ex.Message, .data = New List(Of Object)()})
        End Try
    End Function

End Class
