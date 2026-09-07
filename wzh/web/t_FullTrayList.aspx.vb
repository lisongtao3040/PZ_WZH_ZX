
Partial Class t_FullTrayList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' データはAJAXで取得するため、ここでは何もしない
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Server.Transfer("t_check_list.aspx")
    End Sub

    ''' <summary>
    ''' 検査ボタン：t_check_list へ Server.Transfer
    ''' </summary>
    Protected Sub btnChkServer_Click(sender As Object, e As EventArgs) Handles btnChkServer.Click
        Dim cd As String = hid_chk_cd.Value.Trim()
        Dim no As String = hid_chk_no.Value.Trim()
        If cd <> "" AndAlso no <> "" Then
            Server.Transfer("t_check_list.aspx?cd=" & Server.UrlEncode(cd) _
                          & "&no=" & Server.UrlEncode(no) _
                          & "&autoNewChk=1")
        End If
    End Sub

    ''' <summary>
    ''' 自动OK ボタン：検査データを自動登録して OK にする
    ''' </summary>
    Protected Sub btnAutoOkServer_Click(sender As Object, e As EventArgs) Handles btnAutoOkServer.Click
        Dim cd As String = hid_chk_cd.Value.Trim()
        Dim no As String = hid_chk_no.Value.Trim()
        If cd <> "" AndAlso no <> "" Then
            ' 自动OK 专用处理（此处可追加自动设置 OK 结果的逻辑）
            Server.Transfer("t_check_list.aspx?cd=" & Server.UrlEncode(cd) _
                          & "&no=" & Server.UrlEncode(no) _
                          & "&autoNewChk=1")
        End If
    End Sub

End Class
