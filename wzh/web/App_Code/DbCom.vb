Imports Microsoft.VisualBasic
Imports System.Data
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.Text
Imports System

Public Class DbCom
    Public Shared Function GetTableInfo(ByVal tableName As String) As Data.DataTable

        Dim sb As New StringBuilder
        sb.AppendLine("SELECT ")
        sb.AppendLine("     a.NAME AS table_name")
        sb.AppendLine("   , b.NAME AS columns_name")
        sb.AppendLine("   , c.NAME AS columns_type")
        sb.AppendLine("   , case when b.xscale =0 then ")
        sb.AppendLine("       cast(b.length as varchar) ")
        sb.AppendLine("     else ")
        sb.AppendLine("       cast(b.xprec as varchar) +','+ cast(b.xscale as varchar) ")
        sb.AppendLine("     end as columns_length")
        sb.AppendLine("   , CASE ")
        sb.AppendLine("     WHEN d.TABLE_NAME IS NULL THEN ''")
        sb.AppendLine("     ELSE 'P'")
        sb.AppendLine("     END AS pk")
        sb.AppendLine("   ,SM.TEXT AS DefaultValue")
        sb.AppendLine("   ,b.IsNullable")
        sb.AppendLine("   ,g.value As cn_chars")
        sb.AppendLine("FROM sysobjects a")
        sb.AppendLine(" INNER JOIN syscolumns b ON a.id = b.id")
        sb.AppendLine(" INNER JOIN systypes c ON b.xtype = c.xtype")
        sb.AppendLine(" LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE d")
        sb.AppendLine(" ON d.TABLE_NAME = a.NAME")
        sb.AppendLine("     AND d.COLUMN_NAME = b.NAME")
        sb.AppendLine(" LEFT JOIN dbo.syscomments SM ON b.cdefault = SM.id")
        sb.AppendLine(" left join sys.extended_properties g ")
        sb.AppendLine("on b.id=g.major_id AND b.colid = g.minor_id ")
        sb.AppendLine("WHERE a.xtype = 'U'")
        sb.AppendLine("       AND a.NAME = '" & tableName & "'  ")
        sb.AppendLine("and c.NAME <> 'sysname'")
        sb.AppendLine("ORDER BY b.colorder")

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "GetTableInfo")

    End Function

    Public Shared Function PicNotUpdataChk() As Data.DataTable

        Dim sb As New StringBuilder
        sb.AppendLine("SELECT ")
        sb.AppendLine("distinct")
        sb.AppendLine("    [pic_name]")
        sb.AppendLine("FROM [m_km_template]")
        sb.AppendLine("WHERE [pic_name] not in(")
        sb.AppendLine("select [pic_name] from [m_picture_km] )")

        Return FillData(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), "PicNotUpdataChk")

    End Function


End Class
