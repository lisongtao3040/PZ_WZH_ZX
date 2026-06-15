Imports Microsoft.VisualBasic
Imports EMAB = Itis.ApplicationBlocks.ExceptionManagement.UnTrappedExceptionManager
Imports MyMethod = System.Reflection.MethodBase
Imports Itis.ApplicationBlocks.Data.SQLHelper
Imports Itis.ApplicationBlocks.Data
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Transactions
Imports System.Configuration.ConfigurationSettings
Imports System.Collections.Generic
Imports System.IO
'Imports SqlHelper.SqlHelper
Imports SqlHelper

Public Class ComDDL



    ''' <summary>
    ''' 用户存在检查
    ''' </summary>
    ''' <param name="user_cd"></param>
    ''' <param name="user_pass"></param>
    ''' <returns></returns>
    Public Function CheckUserExist(ByVal user_cd As String, ByVal user_pass As String) As DataTable

        'SQLコメント
        '--**テーブル：用户MS : m_user
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT")
        sb.AppendLine(" user_name")                                               '用户名
        sb.AppendLine(", line_cd")                                                 '生产线CD
        sb.AppendLine("FROM m_user")
        sb.AppendLine("WHERE 1=1")
        sb.AppendLine("AND user_cd='" & user_cd & "'")   '用户CD
        sb.AppendLine("AND user_pass='" & user_pass & "'")   '用户CD
        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "m_user")

        Return dsInfo.Tables("m_user")

    End Function


    ''' <summary>
    ''' 
    ''' 图片MSInfoを検索する
    ''' </summary>
    '''<param name="picId_key">图片ID</param>
    '''<param name="picKbn_key">图片区分</param>
    ''' <returns>图片MSInfo</returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' <para>2019/03/20  更新日さん 新規作成 </para>
    ''' </history>

    Public Function SelTuzhang(ByVal picId_key As String, ByVal picKbn_key As String) As Object
        'SQLコメント
        '--**テーブル：图片MS : m_picture
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT")
        sb.AppendLine("pic_id")                                                    '图片ID
        sb.AppendLine(", pic_name")                                                '图片名
        sb.AppendLine(", line_cd")                                                 '生产线CD
        sb.AppendLine(", pic_conn")                                                '图片内容
        'sb.AppendLine(", pic_kbn")                                                 '图片区分
        'sb.AppendLine(", staus")                                                   '状態

        sb.AppendLine("FROM m_picture")
        sb.AppendLine("WHERE 1=1")
        If picId_key <> "" Then
            sb.AppendLine("AND pic_id=@pic_id_key")   '图片ID
        End If
        If picKbn_key <> "" Then
            sb.AppendLine("AND pic_kbn=@pic_kbn_key")   '图片区分
        End If

        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@pic_id_key", SqlDbType.VarChar, 20, picId_key))
        paramList.Add(MakeParam("@pic_kbn_key", SqlDbType.Char, 1, picKbn_key))

        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "m_picture", paramList.ToArray)

        If dsInfo.Tables(0).Rows.Count > 0 Then
            Return dsInfo.Tables(0).Rows(0).Item("pic_conn")
        Else
            Return ""
        End If


    End Function



    ''' <summary>
    ''' 
    ''' 图片MSInfoを検索する
    ''' </summary>
    '''<param name="chk_id">ID</param>
    '''<param name="pic_kbn">图片区分</param>
    ''' <returns>图片MSInfo</returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' <para>2019/03/20  更新日さん 新規作成 </para>
    ''' </history>

    Public Function SelTuzhangBar(ByVal chk_id As String, ByVal pic_kbn As String, ByVal no As String) As Object
        'SQLコメント
        '--**テーブル：图片MS : m_picture
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT")
        sb.AppendLine(" pic_conn")                                                '图片内容

        sb.AppendLine("FROM t_picture_bar")
        sb.AppendLine("WHERE 1=1")
        If chk_id <> "" Then
            sb.AppendLine("AND chk_id=@chk_id")   '图片ID
        End If
        If pic_kbn <> "" Then
            sb.AppendLine("AND pic_kbn=@pic_kbn")   '图片区分
        End If
        sb.AppendLine("AND no='" & no & "'")

        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@chk_id", SqlDbType.VarChar, 20, chk_id))
        paramList.Add(MakeParam("@pic_kbn", SqlDbType.Char, 1, pic_kbn))

        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "m_picture", paramList.ToArray)

        If dsInfo.Tables(0).Rows.Count > 0 Then
            Return dsInfo.Tables(0).Rows(0).Item("pic_conn")
        Else
            Return ""
        End If


    End Function



    Public Function InsMPicture(ByVal chk_id As String,
                                ByVal pic_kbn As String,
                                ByVal no As String,
                                ByVal picConn As Byte()) As Boolean

        'EMAB　ＥＲＲ
        'SQLコメント ByRef picCon As Byte()
        '--**テーブル： : m_picture
        Dim sb As New StringBuilder
        'SQL文

        sb.AppendLine("DELETE FROM t_picture_bar WHERE 1=1")
        sb.AppendLine("AND chk_id='" & chk_id & "'")   'pic_id
        sb.AppendLine("AND pic_kbn='" & pic_kbn & "'")   'line_cd
        sb.AppendLine("AND no='" & no & "'")   'line_cd

        sb.AppendLine("INSERT INTO  t_picture_bar")
        sb.AppendLine("(")
        sb.AppendLine("chk_id")                                                    '图片ID
        sb.AppendLine(", pic_kbn")                                                '图片名
        sb.AppendLine(", no")
        sb.AppendLine(", pic_conn")                                                '图片内容
        sb.AppendLine(")")

        sb.AppendLine("VALUES(")
        sb.AppendLine("@chk_id")                                                       '图片ID
        sb.AppendLine(", @pic_kbn")                                                   '图片名
        sb.AppendLine(", " & no & "")
        sb.AppendLine(", @pic_conn")                                                   '图片内容
        sb.AppendLine(")")
        'バラメタ格納
        'PARAM
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@chk_id", SqlDbType.VarChar, 20, chk_id))
        paramList.Add(MakeParam("@pic_kbn", SqlDbType.VarChar, 1, pic_kbn))
        paramList.Add(MakeParam("@pic_conn", SqlDbType.VarBinary, -1, picConn))
        'paramList.Add(SqlHelperNew.MakeParam("@pic_kbn", SqlDbType.Char, 1, picKbn))
        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), paramList.ToArray)

        Return True

    End Function


    ''' <summary>
    ''' 图片数据To Byte
    ''' </summary>
    ''' <param name="Imagepath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function imageToByte(ByVal Imagepath As String) As Byte()
        If System.IO.File.Exists(Imagepath) Then '检查图片路径是否正确
            Dim fs As FileStream = New FileStream(Imagepath, FileMode.Open)
            Dim br As BinaryReader = New BinaryReader(fs)
            Dim imageByte() As Byte = br.ReadBytes(CInt(fs.Length))
            br.Close()
            fs.Close()
            Return imageByte
        Else
            Return Nothing
        End If
    End Function


    Public Function InsMPictureChk(ByVal chk_id As String,
                                   ByVal path As String,
                                   ByVal user_cd As String) As Boolean



        'EMAB　ＥＲＲ
        'SQLコメント ByRef picCon As Byte()
        '--**テーブル： : m_picture
        Dim sb As New StringBuilder
        'SQL文

        sb.AppendLine("INSERT INTO  m_picture_chk")
        sb.AppendLine("(")
        sb.AppendLine("chk_id")                                                    '图片ID
        sb.AppendLine(", pic_conn")                                                '图片内容
        sb.AppendLine(", ins_user")
        sb.AppendLine(", ins_date")
        sb.AppendLine(")")

        sb.AppendLine("VALUES(")
        sb.AppendLine("@chk_id")                                                       '图片ID
        sb.AppendLine(", @pic_conn")                                                   '图片内容
        sb.AppendLine(", '" & user_cd & "'")
        sb.AppendLine(", getdate()")
        sb.AppendLine(")")
        'バラメタ格納
        'PARAM
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@chk_id", SqlDbType.VarChar, 20, chk_id))
        paramList.Add(MakeParam("@pic_conn", SqlDbType.VarBinary, -1, imageToByte(path)))

        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), paramList.ToArray)

        Return True

    End Function


    '登录图片
    Public Function InsPictureKm(ByVal pic_name As String, ByVal pic_upd_time As String) As Integer
        Dim sb As New StringBuilder
        sb.AppendLine(" DELETE FROM m_picture_km")
        sb.AppendLine(" WHERE pic_name=N'" & pic_name & "'")
        sb.AppendLine("   AND pic_upd_time=N'" & pic_upd_time & "'")



        sb.AppendLine(" INSERT INTO m_picture_km")
        sb.AppendLine("(")
        sb.AppendLine("pic_name")                                                    '图片ID
        sb.AppendLine(", pic_upd_time")                                                '图片内容
        sb.AppendLine(", pic_conn")

        sb.AppendLine(")")
        sb.AppendLine("VALUES(")
        sb.AppendLine(" N'" & pic_name & "' ")
        sb.AppendLine(",N'" & pic_upd_time & "' ")                                                 '图片ID
        sb.AppendLine(", @pic_conn")                                                   '图片内容
        sb.AppendLine(")")

        'sb.AppendLine(" SELECT ")
        'sb.AppendLine(" N'" & pic_name & "' ")
        'sb.AppendLine(" N'" & pic_upd_time & "' ")
        'sb.AppendLine(" N'" & pic_conn & "' ")

        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@pic_conn", SqlDbType.VarBinary, -1, imageToByte("F:\WEBSHARE\Wzh2023\Image\ChkImgs\" & pic_name)))
        '更新の実行
        Return ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), paramList.ToArray)

    End Function





    Public Function DelMPictureChk(ByVal chk_id As String, ByVal idx As String) As Boolean



        'EMAB　ＥＲＲ
        'SQLコメント ByRef picCon As Byte()
        '--**テーブル： : m_picture
        Dim sb As New StringBuilder
        'SQL文

        sb.AppendLine("Delete ")

        sb.AppendLine("FROM m_picture_chk")
        sb.AppendLine("WHERE idx=" & idx & "")

        sb.AppendLine("AND chk_id='" & chk_id & "'")   '图片ID

        ExecuteNonQuery(DataAccessManager.ConnStr, CommandType.Text, sb.ToString())

        Return True

    End Function

    Public Function SelChkPicture(ByVal chk_id As String) As DataTable
        'SQLコメント
        '--**テーブル：图片MS : m_picture
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT")
        sb.AppendLine("*")                                                    '图片ID
        sb.AppendLine("FROM m_picture_chk")
        sb.AppendLine("WHERE 1=1")
        sb.AppendLine("AND chk_id=@chk_id")   '图片ID

        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@chk_id", SqlDbType.VarChar, 20, chk_id))

        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "SelChkPicture", paramList.ToArray)

        'If dsInfo.Tables(0).Rows.Count > 0 Then
        '    Return dsInfo.Tables(0).Rows(0).Item("pic_conn")
        'Else
        '    Return ""
        'End If

        Return dsInfo.Tables(0)

    End Function


    Public Function SelChkPictureByName(ByVal pic_name As String) As DataTable
        'SQLコメント
        '--**テーブル：图片MS : m_picture
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT top 50")
        sb.AppendLine("*")                                                    '图片ID
        sb.AppendLine("FROM m_picture_km")
        sb.AppendLine("WHERE 1=1")
        sb.AppendLine("AND pic_name like N'%" & pic_name & "%'")   '图片ID
        sb.AppendLine("ORDER BY pic_name,pic_upd_time desc")
        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)


        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "SelChkPicture", paramList.ToArray)

        'If dsInfo.Tables(0).Rows.Count > 0 Then
        '    Return dsInfo.Tables(0).Rows(0).Item("pic_conn")
        'Else
        '    Return ""
        'End If

        Return dsInfo.Tables(0)

    End Function

    Public Function SelChkPictureKM(ByVal pic_name As String, ByVal pic_upd_time As String) As DataTable
        'SQLコメント
        '--**テーブル：图片MS : m_picture
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT")
        sb.AppendLine("*")                                                    '图片ID
        sb.AppendLine("FROM m_picture_km")
        sb.AppendLine("WHERE 1=1")
        sb.AppendLine("AND pic_name=@pic_name")   '图片ID
        sb.AppendLine("AND pic_upd_time=@pic_upd_time")   '图片ID
        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)
        paramList.Add(MakeParam("@pic_name", SqlDbType.NVarChar, 50, pic_name))
        paramList.Add(MakeParam("@pic_upd_time", SqlDbType.VarChar, 50, pic_upd_time))
        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "SelChkPicture", paramList.ToArray)

        'If dsInfo.Tables(0).Rows.Count > 0 Then
        '    Return dsInfo.Tables(0).Rows(0).Item("pic_conn")
        'Else
        '    Return ""
        'End If

        Return dsInfo.Tables(0)

    End Function

    Public Function SelChkResultPicture(ByVal idx As String) As DataTable
        'SQLコメント
        '--**テーブル：图片MS : m_picture
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT")
        sb.AppendLine("*")                                                    '图片ID
        sb.AppendLine("FROM m_picture_chk")
        sb.AppendLine("WHERE 1=1")
        sb.AppendLine("AND idx=" & idx)   '图片ID

        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)

        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "SelChkResultPicture", paramList.ToArray)

        'If dsInfo.Tables(0).Rows.Count > 0 Then
        '    Return dsInfo.Tables(0).Rows(0).Item("pic_conn")
        'Else
        '    Return ""
        'End If

        Return dsInfo.Tables(0)

    End Function

    Public Function SelChkMsPicture(ByVal pic_name As String, ByVal pic_upd_time As String) As DataTable
        'SQLコメント
        '--**テーブル：图片MS : m_picture
        Dim sb As New StringBuilder
        'SQL文
        sb.AppendLine("SELECT")
        sb.AppendLine("*")                                                    '图片ID
        sb.AppendLine("FROM m_picture_km")
        sb.AppendLine("WHERE 1=1")
        sb.AppendLine("AND pic_name='" & pic_name & "'")
        sb.AppendLine("AND pic_upd_time='" & pic_upd_time & "'")

        'バラメタ格納
        Dim paramList As New List(Of SqlParameter)

        Dim dsInfo As New Data.DataSet
        FillDataset(DataAccessManager.ConnStr, CommandType.Text, sb.ToString(), dsInfo, "SelChkResultPicture", paramList.ToArray)

        'If dsInfo.Tables(0).Rows.Count > 0 Then
        '    Return dsInfo.Tables(0).Rows(0).Item("pic_conn")
        'Else
        '    Return ""
        'End If

        Return dsInfo.Tables(0)

    End Function

    ''' <summary>
    ''' GetIntValue
    ''' </summary>
    ''' <param name="v"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetIntValue(ByVal v As Object) As Object

        If v Is Nothing OrElse v Is DBNull.Value OrElse v.ToString = "" Then
            Return 0
        Else
            Return Convert.ToInt32(v)
        End If
    End Function





End Class
