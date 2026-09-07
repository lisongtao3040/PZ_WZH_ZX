Imports System.Runtime.Remoting.Contexts
Imports Microsoft.VisualBasic
Imports System.Text
Imports System.Threading
Imports System.Web.UI
Public Class PageCom

    Public Shared Function GetYmdhmsf() As String
        'test use
        Return "222&c=11234"
        Return Now.ToString("yyMMddHHmmssfff")
    End Function

    '根据检查方法 设置输入框背景色
    Public Shared Function GetInBgColor(ByVal chk_fs As String) As String
        If chk_fs = "1" Then       '扫描
            Return "#edeb8c"
        ElseIf chk_fs = "9" Then   '目视
            Return "#ccc"
        ElseIf chk_fs = "2" Then   '输入
            Return "#fff"
        End If
        Return "#000"
    End Function

    '根据结果 设置结果栏汉字
    Public Shared Function GetTextJieguo(ByVal jieguo As String) As String
        If jieguo = "OK" Then
            jieguo = "合"
        ElseIf jieguo = "SD" Then
            jieguo = "微"
        ElseIf jieguo = "M1" Then
            jieguo = "轻"
        ElseIf jieguo = "M2" Then
            jieguo = "中"
        ElseIf jieguo = "M3" Then
            jieguo = "重"
        ElseIf jieguo = "NG" Then
            jieguo = "误"
        ElseIf jieguo = "JN" Then
            jieguo = "警"
        Else
            jieguo = ""
        End If
        Return jieguo
    End Function

    Public Shared Sub InitParam(ByVal page As Page, ByRef Context As System.Web.HttpContext, ByRef ViewState As System.Web.UI.StateBag, ByRef CLoginInfo As CLoginInfo)

        If Not page.IsPostBack Then

            If Context.Items("CLoginInfo") Is Nothing Then
                CLoginInfo = ViewState("CLoginInfo")

            Else
                ViewState("CLoginInfo") = Context.Items("CLoginInfo")
                CLoginInfo = Context.Items("CLoginInfo")
            End If


        Else
            If ViewState("CLoginInfo") Is Nothing Then
                Context.Items("CLoginInfo") = Nothing

            Else


                Context.Items("CLoginInfo") = ViewState("CLoginInfo")
                CLoginInfo = ViewState("CLoginInfo")
            End If

        End If



        'If Context.Items("CLoginInfo") IsNot Nothing Then
        '    CLoginInfo = Context.Items("CLoginInfo")
        'ElseIf page.Session("CLoginInfo") IsNot Nothing Then
        '    CLoginInfo = page.Session("CLoginInfo")
        'ElseIf ViewState("CLoginInfo") IsNot Nothing Then
        '    CLoginInfo = ViewState("CLoginInfo")
        'End If

        'ViewState("CLoginInfo") = CLoginInfo
        'page.Session("CLoginInfo") = CLoginInfo
        'Context.Items("CLoginInfo") = CLoginInfo

    End Sub


    Public Shared Sub SetInitParam(ByVal page As Page, ByRef Context As System.Web.HttpContext, ByRef ViewState As System.Web.UI.StateBag, ByRef CLoginInfo As CLoginInfo)
        ViewState("CLoginInfo") = CLoginInfo
        page.Session("CLoginInfo") = CLoginInfo
        Context.Items("CLoginInfo") = CLoginInfo
    End Sub



    ''' <summary>
    ''' Message
    ''' </summary>
    ''' <param name="page"></param>
    ''' <param name="msg"></param>
    ''' <remarks></remarks>
    Public Shared Sub ShowMsg3(ByVal page As Page, ByVal msg As String, Optional ByVal errType As String = "○", Optional ByVal text_color As String = "red")
        'errType,text_color
        Dim csScript As New StringBuilder

        With csScript
            .AppendLine("alert2('" & msg.Replace("'", "").Replace(vbCr, "").Replace(vbLf, "") & "','','" & errType & "','" & text_color & "');")
        End With

        'ページ応答で、クライアント側のスクリプト ブロックを出力します
        page.ClientScript.RegisterStartupScript(page.GetType(), "ShowMessage", csScript.ToString, True)

    End Sub

    Public Shared Sub ShowMsg2(ByVal page As Page, ByVal msg As String, ByVal focusId As String)

        Dim csScript As New StringBuilder

        With csScript
            .AppendLine("alert2('" & msg.Replace("'", "").Replace(vbCr, "").Replace(vbLf, "") & "',function () { $('#" & focusId & "').select(); });")
        End With

        'ページ応答で、クライアント側のスクリプト ブロックを出力します
        page.ClientScript.RegisterStartupScript(page.GetType(), "ShowMessage", csScript.ToString, True)

    End Sub


    Public Shared Function GetNewCheckId() As String
        SyncLock "GetIndex"
            Thread.Sleep(1)
            Return Now.ToString("yyMMddHHmmssfff")
        End SyncLock
    End Function


    'Link 的字体颜色
    Public Shared Function GetLinkColor(ByVal dt As Data.DataTable, ByVal kind_name As String) As Drawing.Color
        If dt.Select("(result='NG' OR result='M1' OR result='M2' OR result='M3' OR result='JN' ) and kind_name = '" & kind_name & "'").Length > 0 Then
            Return Drawing.Color.Red
        ElseIf dt.Select("(result='' or result is null) and kind_name = '" & kind_name & "'").Length <> dt.Select("kind_name = '" & kind_name & "'").Length And dt.Select("(result='' or result is null) and kind_name = '" & kind_name & "'").Length > 0 Then
            Return Drawing.Color.Red
        ElseIf dt.Select("(result='' or result is null) and kind_name = '" & kind_name & "'").Length = dt.Select("kind_name = '" & kind_name & "'").Length And dt.Select("(result='' or result is null) and kind_name = '" & kind_name & "'").Length > 0 Then
            Return Drawing.Color.Black
        Else
            Return Drawing.Color.Green
        End If
    End Function

    'Tools Link 的字体颜色
    Public Shared Function GetLinkColorTools(ByVal dt As Data.DataTable, ByVal tool_txt As String) As Drawing.Color
        If dt.Select("(result='NG' OR result='M1' OR result='M2' OR result='M3' OR result='JN'  ) and tool_txt = '" & tool_txt & "'").Length > 0 Then
            Return Drawing.Color.Red
        ElseIf dt.Select("(result='' or result is null) and tool_txt = '" & tool_txt & "'").Length <> dt.Select("tool_txt = '" & tool_txt & "'").Length And dt.Select("(result='' or result is null) and tool_txt = '" & tool_txt & "'").Length > 0 Then
            Return Drawing.Color.Red
        ElseIf dt.Select("(result='' or result is null) and tool_txt = '" & tool_txt & "'").Length = dt.Select("tool_txt = '" & tool_txt & "'").Length And dt.Select("(result='' or result is null) and tool_txt = '" & tool_txt & "'").Length > 0 Then
            Return Drawing.Color.Black
        Else
            Return Drawing.Color.Green
        End If
    End Function


    Public Shared DicComputers As Dictionary(Of String, String) = Nothing

    'Dim dicPjnms As Dictionary(Of String, String)

    Public Shared Function GetLinkKindName(ByVal name As String) As String
        'Dim name As String
        'name = System.Net.Dns.GetHostName()
        'name = page.Request.UserHostName
        If DicComputers Is Nothing Then
            DicComputers = New Dictionary(Of String, String)
        End If
        If DicComputers.ContainsKey(name) Then
            Return DicComputers.Item(name)
        Else
            Return ""
        End If
    End Function

    Public Shared Sub SetLinkKindName(ByVal name As String, ByVal linkname As String)
        If DicComputers Is Nothing Then
            DicComputers = New Dictionary(Of String, String)
        End If
        If DicComputers.ContainsKey(name) Then
            DicComputers.Item(name) = linkname
        Else
            DicComputers.Add(name, linkname)
        End If
    End Sub

    Public Shared Sub GetComRinei(ByVal ddlHiinai As DropDownList)
        For i As Integer = 1 To 9
            Dim item As New ListItem
            item.Text = i * 1
            item.Value = i * 1
            ddlHiinai.Items.Add(item)
        Next
        For i As Integer = 1 To 9
            Dim item As New ListItem
            item.Text = i * 10
            item.Value = i * 10
            ddlHiinai.Items.Add(item)
        Next
        For i As Integer = 1 To 9
            Dim item As New ListItem
            item.Text = i * 100
            item.Value = i * 100
            ddlHiinai.Items.Add(item)
        Next
        For i As Integer = 1 To 9
            Dim item As New ListItem
            item.Text = i * 1000
            item.Value = i * 1000
            ddlHiinai.Items.Add(item)
        Next
    End Sub


End Class
