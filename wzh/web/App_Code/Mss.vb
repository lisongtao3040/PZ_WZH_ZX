Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient
Imports SqlHelper.SqlHelper
Imports SqlHelper
Public Class Mss
    Public Sub New()
    End Sub

    Public Shared Sub CreateSqlTable(ByVal dt As DataTable, ByVal connectionString As String, ByVal tableName As String)
        Using connection As SqlConnection = New SqlConnection(connectionString)
            connection.Open()
            Dim createTableSql As String = "CREATE TABLE " & tableName & " ("

            For Each column As DataColumn In dt.Columns
                createTableSql += "[" & column.ColumnName & "] "

                Select Case column.DataType.ToString()
                    Case "System.Int32"
                        createTableSql += "INT"
                    Case "System.Decimal"
                        createTableSql += "DECIMAL(18,2)"
                    Case "System.String"
                        createTableSql += "NVARCHAR(MAX)"
                    Case "System.DateTime"
                        createTableSql += "DATETIME"
                    Case Else
                        createTableSql += "NVARCHAR(MAX)"
                End Select

                createTableSql += ","
            Next

            createTableSql = createTableSql.Remove(createTableSql.Length - 1)
            createTableSql += ")"
            Dim command As SqlCommand = New SqlCommand(createTableSql, connection)
            command.ExecuteNonQuery()
        End Using
    End Sub

    Public Shared Function ExecuteNonQuery(ByVal commandText As String, ByVal commandType As CommandType, ParamArray parameters As SqlParameter()) As Integer
        Using connection As SqlConnection = New SqlConnection(DataAccessManager.ConnStr)

            Using command As SqlCommand = New SqlCommand(commandText, connection)
                command.CommandType = commandType
                If parameters IsNot Nothing Then command.Parameters.AddRange(parameters)
                connection.Open()
                Return command.ExecuteNonQuery()
            End Using
        End Using
    End Function

    Public Shared Function ExecuteScalar(ByVal commandText As String, ByVal commandType As CommandType, ParamArray parameters As SqlParameter()) As Object
        Using connection As SqlConnection = New SqlConnection(DataAccessManager.ConnStr)

            Using command As SqlCommand = New SqlCommand(commandText, connection)
                command.CommandType = commandType
                If parameters IsNot Nothing Then command.Parameters.AddRange(parameters)
                connection.Open()
                Return command.ExecuteScalar()
            End Using
        End Using
    End Function

    Public Shared Function ExecuteDataTable(ByVal commandText As String, ByVal commandType As CommandType, ParamArray parameters As SqlParameter()) As DataTable
        Using connection As SqlConnection = New SqlConnection(DataAccessManager.ConnStr)

            Using command As SqlCommand = New SqlCommand(commandText, connection)
                command.CommandType = commandType
                If parameters IsNot Nothing Then command.Parameters.AddRange(parameters)

                Using adapter As SqlDataAdapter = New SqlDataAdapter(command)
                    Dim dataTable As DataTable = New DataTable()
                    adapter.Fill(dataTable)
                    Return dataTable
                End Using
            End Using
        End Using
    End Function

    Public Function OutLog(ByVal medthodFullName As String, ByVal className As String, ByVal fncName As String, ByVal tableName As String, ByVal cmdText As String, ByVal params As SqlParameter()) As String
        Dim log As System.Text.StringBuilder = New System.Text.StringBuilder()

        If True Then
            Dim withBlock = log
            withBlock.AppendLine("   从命名空间开始逐项列出:" & medthodFullName)
            withBlock.AppendLine("   类名:" & className)
            withBlock.AppendLine("   函数名:" & fncName)
            withBlock.AppendLine("   参数:")
            withBlock.AppendLine("       tableName:" & tableName)
            withBlock.AppendLine("       cmdText:")
            withBlock.AppendLine("        " & "-----------------------------------------------------")
            withBlock.AppendLine("        " & "SQL:")
            withBlock.AppendLine("        " & "-----------------------------------------------------")
            withBlock.AppendLine("            " & cmdText.Replace(vbCrLf, vbCrLf & "            "))

            If params IsNot Nothing Then
                withBlock.AppendLine("        " & "-----------------------------------------------------")
                withBlock.AppendLine("        " & "params:")

                For i As Integer = 0 To params.Length - 1
                    withBlock.AppendLine("        " & "   name:" & params(i).ParameterName & " type:" & params(i).SqlDbType.ToString() & " size:" & params(i).Size & " value:" & params(i).Value)
                Next
            End If

            withBlock.AppendLine("        " & "-----------------------------------------------------")
        End If

        Return log.ToString()
    End Function
End Class
