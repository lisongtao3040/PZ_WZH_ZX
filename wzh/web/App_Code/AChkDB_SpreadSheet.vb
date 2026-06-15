Option Strict Off

Imports Newtonsoft.Json
Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Linq
Imports System.Text
Imports System.Web
Imports System.Web.Services

<WebService([Namespace]:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService>
Public Class AChkDB_SpreadSheet
    Inherits System.Web.Services.WebService

    <WebMethod>
    Public Function GetTableInfo(ByVal table_name As String) As String
        Dim sql As String = "select * from TablesInfo where TableNameEn='" & table_name & "';"
        Dim tableInfo As System.Data.DataTable = Mss.ExecuteDataTable(sql.ToString(), System.Data.CommandType.Text, Nothing)
        tableInfo.TableName = "tableInfo"
        Dim sql2 As String = "select * from " & table_name & " order by ins_date"
        Dim ms As System.Data.DataTable = Mss.ExecuteDataTable(sql2.ToString(), System.Data.CommandType.Text, Nothing)
        ms.TableName = table_name
        Dim ds As DataSet = New DataSet()
        ds.Tables.Add(tableInfo)
        ds.Tables.Add(ms)
        Return JsonConvert.SerializeObject(ds)
    End Function

    <WebMethod>
    Public Function GetTableInfoByJouken(ByVal table_name As String, ByVal keyParam As Object) As String
        Dim sql As String = "select * from TablesInfo where TableNameEn='" & table_name & "';"
        Dim tableInfo As System.Data.DataTable = Mss.ExecuteDataTable(sql.ToString(), System.Data.CommandType.Text, Nothing)
        tableInfo.TableName = "tableInfo"
        Dim sql2 As StringBuilder = New StringBuilder()
        sql2.AppendLine("select * from " & table_name)
        sql2.AppendLine("WHERE 1=1 ")

        For Each Column In keyParam
            If Column("value") = "" Then Continue For

            If Column("value").ToString().Contains("%") Then
                sql2.AppendLine("AND " & Column("ColumnNameEn") & " like N'" & Column("value") & "'")
            Else
                sql2.AppendLine("AND " & Column("ColumnNameEn") & "='" & Column("value") & "'")
            End If
        Next

        sql2.AppendLine(" order by ins_date")
        Dim ms As System.Data.DataTable = Mss.ExecuteDataTable(sql2.ToString(), System.Data.CommandType.Text, Nothing)
        ms.TableName = table_name
        Dim ds As DataSet = New DataSet()
        ds.Tables.Add(tableInfo)
        ds.Tables.Add(ms)
        Return JsonConvert.SerializeObject(ds)
    End Function

    <WebMethod>
    Public Function InsMsOneRow(ByVal table_name As String, ByVal user_id As String, ByVal sqlParam As Object) As String
        Dim sql As StringBuilder = New StringBuilder()
        sql.AppendLine("INSERT INTO " & table_name & "(guid,")

        For Each Column In sqlParam
            sql.Append("" & Column("ColumnNameEn") & ", ")
        Next

        sql.Append("ins_sya, ")
        sql.Append("ins_date, ")
        sql.Append("upd_sya, ")
        sql.Append("upd_date ")
        sql.AppendLine(")values(")
        sql.Append("'" & CNewId.NewId() & "', ")

        For Each Column In sqlParam

            If Column("ColumnNameEn").ToString().ToLower().Contains("name") Then
                sql.Append("N'" & Column("value") & "', ")
            Else
                sql.Append("'" & Column("value") & "', ")
            End If
        Next

        sql.Append("'" & user_id & "', ")
        sql.Append("getdate(), ")
        sql.Append("'" & user_id & "', ")
        sql.Append("getdate() ")
        sql.AppendLine(");")
        Mss.ExecuteNonQuery(sql.ToString(), System.Data.CommandType.Text, Nothing)
        Return JsonConvert.SerializeObject("OK")
    End Function

    <WebMethod>
    Public Function InsMsRows(ByVal table_name As String, ByVal user_id As String, ByVal sqlParams As Object) As String
        Dim sql As StringBuilder = New StringBuilder()

        For Each sqlParam In sqlParams
            sql.AppendLine("INSERT INTO " & table_name & "(guid,")

            For Each Column In sqlParam
                sql.Append("" & Column("ColumnNameEn") & ", ")
            Next

            sql.Append("ins_sya, ")
            sql.Append("ins_date, ")
            sql.Append("upd_sya, ")
            sql.Append("upd_date ")
            sql.AppendLine(")values(")
            sql.Append("'" & CNewId.NewId() & "', ")

            For Each Column In sqlParam

                If Column("ColumnNameEn").ToString().ToLower().Contains("name") OrElse Column("ColumnNameEn").ToString().ToLower().Contains("content") Then
                    sql.Append("N'" & Column("value") & "', ")
                Else
                    sql.Append("'" & Column("value") & "', ")
                End If
            Next

            sql.Append("'" & user_id & "', ")
            sql.Append("getdate(), ")
            sql.Append("'" & user_id & "', ")
            sql.Append("getdate() ")
            sql.AppendLine(");")
        Next

        Mss.ExecuteNonQuery(sql.ToString(), System.Data.CommandType.Text, Nothing)
        Return JsonConvert.SerializeObject("OK")
    End Function

    <WebMethod>
    Public Function UpdMsOneRow(ByVal table_name As String, ByVal user_id As String, ByVal keyParam As Object, ByVal sqlParam As Object) As String
        If keyParam.Length = 0 OrElse keyParam(0).Count = 0 Then
            Return JsonConvert.SerializeObject("没有Key项目")
        End If

        Dim sql As StringBuilder = New StringBuilder()
        sql.AppendLine("UPDATE " & table_name & " SET ")

        For Each Column In sqlParam

            If Column("ColumnNameEn").ToString().ToLower().Contains("name") Then
                sql.Append("" & Column("ColumnNameEn") & "=N'" & Column("value") & "',")
            Else
                sql.Append("" & Column("ColumnNameEn") & "='" & Column("value") & "',")
            End If
        Next

        sql.Append("upd_sya= '" & user_id & "',")
        sql.Append("upd_date=getdate() ")
        sql.Append("WHERE 1=1 ")

        For Each Column In keyParam

            If Column("ColumnNameEn").ToString().ToLower().Contains("name") Then
                sql.Append("AND " & Column("ColumnNameEn") & "=N'" & Column("value") & "'")
            Else
                sql.Append("AND " & Column("ColumnNameEn") & "='" & Column("value") & "'")
            End If
        Next

        Mss.ExecuteNonQuery(sql.ToString(), System.Data.CommandType.Text, Nothing)
        Return JsonConvert.SerializeObject("OK")
    End Function

    Public Function IsNtxt(ByVal keyName As String) As Boolean
        If keyName.Contains("name") Then
            Return True
        Else
            Return False
        End If
    End Function

    <WebMethod>
    Public Function DelMsOneRow(ByVal table_name As String, ByVal keyParam As Object) As String
        If keyParam(0).Count = 0 Then
            Return JsonConvert.SerializeObject("没有Key项目")
        End If

        If table_name = "v_chk_group" Then table_name = "m_chk_group"
        Dim sql As StringBuilder = New StringBuilder()
        sql.AppendLine("DELETE FROM " & table_name & " ")
        sql.Append("WHERE 1=1 ")

        For Each Column In keyParam
            sql.Append("AND " & Column("ColumnNameEn") & "='" & Column("value") & "'")
        Next

        Mss.ExecuteNonQuery(sql.ToString(), System.Data.CommandType.Text, Nothing)
        Return JsonConvert.SerializeObject("OK")
    End Function
End Class
