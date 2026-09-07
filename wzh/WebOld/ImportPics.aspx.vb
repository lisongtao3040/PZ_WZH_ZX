
Imports System.Diagnostics
Imports System.IO




Partial Class ImportPics
    Inherits System.Web.UI.Page

    Protected Sub btnImports_Click(sender As Object, e As EventArgs) Handles btnImports.Click

        'GetAllFile("\\10.160.192.44\Wzh2021\Image\ChkImgs\")
        GetAllFile("F:\WEBSHARE\Wzh2023\Image\ChkImgs\")

    End Sub



    Private Sub GetAllFile(ByVal path As String)

        Dim strFile As String() = System.IO.Directory.GetFiles(path)
        Dim date1 As New Date(1900, 1, 1, 1, 1, 1)
        Dim ymdKey As String = date1.ToString("yyyyMMddHHmmss")
        Dim ComDDL As New ComDDL

        Dim nowDate As Date = Now
        Dim nowYYMMDD As String = nowDate.ToString("yyyyMMddHHmmss")
        'InsPictureKm
        For i = 0 To strFile.Length - 1
            'Dim lastWriteTime As String = File.GetLastWriteTime(strFile(i)).ToString("yyyyMMddHHmmss")
            'Dim attr = File.GetAttributes(strFile(i))

            If System.IO.Path.GetExtension(strFile(i)).ToUpper = "DB" Then
            Else
                Dim crTm As String = File.GetCreationTime(strFile(i)).ToString("yyyyMMddHHmmss")
                If crTm = ymdKey OrElse crTm = "19000101020101" Then

                Else


                    ComDDL.InsPictureKm(System.IO.Path.GetFileName(strFile(i)), nowYYMMDD)
                    File.SetCreationTime(strFile(i), date1)
                    File.SetLastWriteTime(strFile(i), nowDate)
                End If

            End If


            'Dim strVersion = FileVersionInfo.GetVersionInfo(strFile(i)).FileDescription
            'Debug.Print(strFile(i))
        Next

    End Sub

    Public Function InsMPicture(ByVal picId As String,
                                   ByVal picName As String,
                                   ByVal lineCd As String,
                                   ByVal picConn As Byte(),
                                   ByVal picKbn As String,
                                   ByVal user_cd As String) As Boolean
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

    Protected Sub btnSel_Click(sender As Object, e As EventArgs) Handles btnSel.Click

        Dim cm As New ComDDL
        Dim dtImg As Data.DataTable = cm.SelChkPictureByName(tbxName.Text.Trim)
        GvPic.DataSource = dtImg
        GvPic.DataBind()

        For i As Integer = 0 To dtImg.Rows.Count - 1
            Dim img As System.Web.UI.WebControls.Image = CType(GvPic.Rows(i).FindControl("Image1"), System.Web.UI.WebControls.Image)
            Dim MStream As New MemoryStream(CType(dtImg.Rows(i).Item("pic_conn"), Byte()))
            Dim base64 As String = Convert.ToBase64String(MStream.ToArray())
            img.ImageUrl = "data:image/jpg;base64," + base64
            ' AddHandler btn.Click, AddressOf Me.btClick
        Next
    End Sub
End Class
