
Imports System.Data
Imports System.IO
Imports Newtonsoft.Json.Linq

Partial Class t_check_ms

    Inherits System.Web.UI.Page
    Private CLoginInfo As CLoginInfo
    Private BC As New t_checkBC
    Private DA As New t_checkDA

    '画面加载时
    Private Sub t_check_ms_Load(sender As Object, e As EventArgs) Handles Me.Load

        '加载画面参数
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)

        'Dim dicPjnms As Dictionary(Of String, String)

        If Not IsPostBack Then

            If Context.Items("CLoginInfo") Is Nothing Then
                SnapVis(False)
                Exit Sub
            End If

            Me.tbxCd.Text = CLoginInfo.cd
            Me.tbxNo.Text = CLoginInfo.no
            Me.tbxCk_id.Text = CLoginInfo.ck_id
            Me.tbxUserCd.Text = CLoginInfo.user_cd
            Me.tbxUserName.Text = CLoginInfo.user_name
            Me.lblLineCd.Text = CLoginInfo.line_cd
            Me.lblLineName.Text = CLoginInfo.line_name

            Me.hid_jxs_name.Value = CLoginInfo.jxs_name

            'lbSCX.OnClientClick = "window.open('pt_v_A05_check_scx.aspx?kbn=1');return false;"
            'lbSCX.Attributes.Item("href") = "#"

            'ViewState("CLoginInfo") = Context.Items("CLoginInfo")
            'If Context.Items("CLoginInfo") IsNot Nothing Then
            '    CLoginInfo = Context.Items("CLoginInfo")
            'End If

            'ViewState("CLoginInfo") = CLoginInfo
            'JsLoad()
            'UserHeader.InitLoginInfo(CLoginInfo)

            Dim dt As Data.DataTable

            '设置 GV H W KW... 信息
            Dim dtChk As DataTable = DA.GetCheckOne(CLoginInfo.ck_id)
            If dtChk.Rows.Count > 0 Then
                Me.hid_jxs_name.Value = dtChk.Rows(0).Item("jxs_name").ToString
            End If



            If dtChk.Select("tools_scan_flg='1'").Length > 0 Then
                ViewState("tools_scan_flg") = dtChk.Rows(0).Item("tools_scan_flg").ToString
            End If

            If CLoginInfo.IsToolStyle = True Then
                SnapVis(False)
                btnChkTools.Visible = False
                btnChkKind.Visible = True
                '治具
                'dt = DA.GetCheckMs(CLoginInfo, CLoginInfo.ck_id, "", True)

                dt = GetToolsMsData()
                'tool_txt
                '取得项目种类link 数据
                'dicPjnms = New Dictionary(Of String, String)

                'Dim drs() As DataRow = dt.Select("", "tools_ma asc")


                'For i As Integer = 0 To drs.Count - 1
                '    If dicPjnms.ContainsKey(drs(i).Item("tool_txt").ToString) = False Then
                '        dicPjnms.Add(drs(i).Item("tool_txt").ToString, "")
                '    End If
                'Next
                'ViewState("dicPjnms") = dicPjnms
                '加载Links字典
                InitToolsDic()
                '加载 links
                InitLinkbuttonsTools()
                ClickKindNameLinkTool(PanelLinks.Controls(0))

                dt = DA.GetCheckMs(CLoginInfo, CLoginInfo.ck_id, "", True, CType(PanelLinks.Controls(0), LinkButton).Text.Trim)

            Else

                btnChkTools.Visible = True
                btnChkKind.Visible = False
                '所有数据
                dt = GetKindMsData()
                '加载Links字典
                InitLinksDic()
                '加载 links
                InitLinkbuttons()
                '默认点击第一个链接
                If PanelLinks.Controls.Count > 0 Then
                    ClickKindNameLink(PanelLinks.Controls(0))

                    dt = GetKindMsDataOneKind(CType(PanelLinks.Controls(0), LinkButton).Text.Trim.Split(":")(1))
                End If

            End If

            For i As Integer = 0 To dtChk.Columns.Count - 1
                gv.Attributes.Item(dtChk.Columns(i).ColumnName) = dtChk.Rows(0).Item(i).ToString
            Next


            '加载GV明细
            InitMs(dt)

        Else
            '加载 links
            If CLoginInfo.IsToolStyle = True Then

                InitLinkbuttonsTools()
            Else

                InitLinkbuttons()

            End If

        End If

        PicMSInit()

    End Sub

    Sub SnapVis(ByVal kbn As Boolean)
        GvPic.Visible = kbn
        btnUpload.Visible = kbn
        PicUpload1.Visible = kbn
        PicUpload2.Visible = kbn
        PicUpload3.Visible = kbn
        PicUpload4.Visible = kbn
        PicUpload5.Visible = kbn
    End Sub



#Region "Kind Ms"
    Private kindMsData As DataTable
    '加载kind全部数据
    Function GetKindMsData() As DataTable
        If kindMsData Is Nothing Then
            kindMsData = DA.GetCheckMs(CLoginInfo, CLoginInfo.ck_id, "")
        End If
        Return kindMsData
    End Function
    '加载一个kind数据
    Function GetKindMsDataOneKind(ByVal kind As String) As DataTable
        Dim tmp As DataTable = GetKindMsData()
        tmp.Clone()
        Dim dt As DataTable = tmp.Copy()
        dt.Clear()
        For i As Integer = 0 To tmp.Rows.Count - 1
            If tmp.Rows(i).Item("kind_name") = kind Then


                dt.Rows.Add(tmp.Rows(i).ItemArray)

            End If
        Next
        Return dt
    End Function
    Function InitLinksDic() As Dictionary(Of String, String)

        If ViewState("dicPjnms") Is Nothing Then
            Dim dt As DataTable = GetKindMsData()
            Dim dicPjnms = New Dictionary(Of String, String)
            Dim idx As Integer = 1

            For i As Integer = 0 To dt.Rows.Count - 1
                If dicPjnms.ContainsKey(dt.Rows(i).Item("kind_jun").ToString & ":" & dt.Rows(i).Item("kind_name").ToString) = False Then
                    dicPjnms.Add(dt.Rows(i).Item("kind_jun").ToString & ":" & dt.Rows(i).Item("kind_name").ToString, "")
                End If
            Next
            ViewState("dicPjnms") = dicPjnms
        End If
        Return ViewState("dicPjnms")
    End Function

    '加载 links
    Sub InitLinkbuttons()
        Dim dicPjnms As Dictionary(Of String, String) = InitLinksDic()

        Dim dicNo As Integer = 1

        For Each item In dicPjnms
            Dim lnk As New LinkButton
            'lnk.Text = item.Key
            lnk.Text = dicNo & ":" & item.Key.Split(":")(1)
            lnk.CssClass = "link_btn"
            AddHandler lnk.Click, AddressOf Link_Click
            PanelLinks.Controls.Add(lnk)
            dicNo = dicNo + 1
        Next
    End Sub

    '種類Click
    Sub Link_Click(sender As Object, e As System.EventArgs)

        Dim kind_name As String = CType(sender, LinkButton).Text.Trim.Split(":")(1)
        ViewState("zenkind_name") = kind_name
        ClickKindNameLink(sender)
        'Dim dt As Data.DataTable = DA.GetCheckMs(CLoginInfo, CLoginInfo.ck_id, kind_name)
        Dim dt As Data.DataTable = GetKindMsDataOneKind(kind_name)
        InitMs(dt)


        PageCom.SetLinkKindName(CLoginInfo.user_cd, kind_name)
    End Sub


    'Link button kind ng ok  red blue green
    Sub ClickKindNameLink(sender As Object)
        'Dim dt As Data.DataTable = DA.GetLinksMs(CLoginInfo, CLoginInfo.ck_id, "")
        Dim dt As Data.DataTable = GetKindMsData()
        For i As Integer = 0 To PanelLinks.Controls.Count - 1
            Dim lnk As LinkButton = CType(PanelLinks.Controls(i), LinkButton)
            lnk.Font.Bold = False
            lnk.ForeColor = PageCom.GetLinkColor(dt, CType(PanelLinks.Controls(i), LinkButton).Text.Trim.Trim.Split(":")(1))
            lnk.Font.Size = 13
            lnk.BackColor = Drawing.Color.White

            If lnk.Text.ToString.Contains("治具") OrElse lnk.Text.ToString.Contains("制品颜色") OrElse lnk.Text.ToString.Contains("#") Then
                If ViewState("tools_scan_flg") = "1" Then
                    lnk.Enabled = True
                Else
                    lnk.Enabled = False
                    lnk.BackColor = Drawing.Color.Silver
                End If



            Else
                lnk.Enabled = True
            End If


        Next
        CType(sender, LinkButton).Font.Bold = True
        'CType(sender, LinkButton).Font.Size = 16
        CType(sender, LinkButton).BackColor = Drawing.Color.GreenYellow

        If CType(sender, LinkButton).Text.ToString.Contains("外观") Then
            'GvPic.Visible = True
            'btnUpload.Visible = True
            'PicUpload.Visible = True
            SnapVis(True)
        Else
            'GvPic.Visible = False
            'btnUpload.Visible = False
            'PicUpload.Visible = False
            SnapVis(False)
        End If

    End Sub
#End Region

#Region "Tools Ms"
    Private toolsMsData As DataTable
    '加载kind全部数据
    Function GetToolsMsData() As DataTable
        If toolsMsData Is Nothing Then
            toolsMsData = DA.GetCheckMs(CLoginInfo, CLoginInfo.ck_id, "", True)
        End If
        Return toolsMsData
    End Function

    Function InitToolsDic() As Dictionary(Of String, String)
        If ViewState("dicToolsNms") Is Nothing Then
            Dim dt As DataTable = GetToolsMsData()
            Dim dicPjnms = New Dictionary(Of String, String)
            Dim drs() As DataRow = dt.Select("", "tools_ma asc")
            For i As Integer = 0 To drs.Count - 1
                If dicPjnms.ContainsKey(drs(i).Item("tool_txt").ToString) = False Then
                    dicPjnms.Add(drs(i).Item("tool_txt").ToString, "")
                End If
            Next
            ViewState("dicToolsNms") = dicPjnms
        End If
        Return ViewState("dicToolsNms")
    End Function


    '加载 links
    Sub InitLinkbuttonsTools()
        Dim dicPjnms As Dictionary(Of String, String) = ViewState("dicToolsNms")
        For Each item In dicPjnms
            'If project_name <> dt.Rows(i).Item("project_name").ToString Then
            Dim lnk As New LinkButton
            lnk.Text = item.Key
            lnk.CssClass = "link_btn"
            AddHandler lnk.Click, AddressOf LinkTool_Click
            PanelLinks.Controls.Add(lnk)
        Next
    End Sub

    '種類Click
    Sub LinkTool_Click(sender As Object, e As System.EventArgs)

        Dim kind_name As String = CType(sender, LinkButton).Text.Trim
        ClickKindNameLinkTool(sender)

        ' Dim dt As Data.DataTable = DA.GetCheckMs(CLoginInfo, CLoginInfo.ck_id, "", True, kind_name)
        Dim dt As DataTable = GetToolsMsDataOneKind(kind_name)
        InitMs(dt)

        PageCom.SetLinkKindName(CLoginInfo.user_cd, kind_name)



    End Sub

    Function GetToolsMsDataOneKind(ByVal kind As String) As DataTable
        Dim tmp As DataTable = GetToolsMsData()
        tmp.Clone()
        Dim dt As DataTable = tmp.Copy()
        dt.Clear()
        For i As Integer = 0 To tmp.Rows.Count - 1
            If tmp.Rows(i).Item("tool_txt") = kind Then
                dt.Rows.Add(tmp.Rows(i).ItemArray)

            End If
        Next
        Return dt
    End Function

    'Link button kind ng ok  red blue green
    Sub ClickKindNameLinkTool(sender As Object)
        'Dim dt As Data.DataTable = DA.GetCheckMs(CLoginInfo, CLoginInfo.ck_id, "", True)
        Dim dt As DataTable = GetToolsMsData()
        For i As Integer = 0 To PanelLinks.Controls.Count - 1
            CType(PanelLinks.Controls(i), LinkButton).Font.Bold = False
            CType(PanelLinks.Controls(i), LinkButton).ForeColor = PageCom.GetLinkColorTools(dt, CType(PanelLinks.Controls(i), LinkButton).Text.Trim)
            CType(PanelLinks.Controls(i), LinkButton).Font.Size = 20
        Next
        CType(sender, LinkButton).Font.Bold = True
        CType(sender, LinkButton).Font.Size = 25
    End Sub


#End Region





    '加载明细
    Sub InitMs(ByVal dt As Data.DataTable)

        'For i As Integer = 0 To dt.Rows.Count - 1
        '    'chk_km_name
        '    If dt.Rows(i).Item("k1").Contains("{") Then
        '        dt.Rows(i).Item("chk_km_name") = dt.Rows(i).Item("chk_km_name") & "*****" & dt.Rows(i).Item("k1").ToString.Trim()
        '    End If


        '    If dt.Rows(i).Item("filter").Contains("{") Then
        '        dt.Rows(i).Item("chk_km_name") = dt.Rows(i).Item("chk_km_name") & "#####" & dt.Rows(i).Item("filter").ToString.Trim()
        '    End If
        '    'filter
        'Next

        '绑定数据
        Me.gv.DataSource = dt
        Me.gv.DataBind()
        '设置行属性
        For i As Integer = 0 To dt.Rows.Count - 1
            'Key 项目
            gv.Rows(i).Attributes.Item("ck_id") = dt.Rows(i).Item("ck_id").ToString.Trim()
            gv.Rows(i).Attributes.Item("id") = dt.Rows(i).Item("id").ToString.Trim()
            gv.Rows(i).Attributes.Item("cd") = dt.Rows(i).Item("cd").ToString.Trim()
            gv.Rows(i).Attributes.Item("no") = dt.Rows(i).Item("no").ToString.Trim()

            '检查用项目
            gv.Rows(i).Attributes.Item("k_type") = dt.Rows(i).Item("k_type").ToString.Trim()
            gv.Rows(i).Attributes.Item("k1") = dt.Rows(i).Item("k1").ToString.Trim()
            gv.Rows(i).Attributes.Item("k2") = dt.Rows(i).Item("k2").ToString.Trim()
            gv.Rows(i).Attributes.Item("k3") = dt.Rows(i).Item("k3").ToString.Trim()
            gv.Rows(i).Attributes.Item("chk_fmt") = dt.Rows(i).Item("chk_fmt").ToString.Trim()
            gv.Rows(i).Attributes.Item("chk_fs") = dt.Rows(i).Item("chk_fs").ToString.Trim()





            gv.Rows(i).Attributes.Item("pic_name") = dt.Rows(i).Item("pic_name").ToString.Trim()

            '数量 未实装等 计划项目
            gv.Rows(i).Attributes.Item("suu") = dt.Rows(i).Item("suu").ToString.Trim()

            Dim conCellIdx As Integer = 5
            Select Case dt.Rows(i).Item("result").ToString.Trim
                Case "OK", "SD"
                    gv.Rows(i).Cells(conCellIdx).Attributes.CssStyle.Item("background-color") = "#93FF93" '绿

                Case ""
                Case Else
                    gv.Rows(i).Cells(conCellIdx).Attributes.CssStyle.Item("background-color") = "#FF2D2D" '红
                    CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = "(" & dt.Rows(i).Item("k1").ToString.Trim() & "," & dt.Rows(i).Item("k3").ToString.Trim() & "," & dt.Rows(i).Item("k2").ToString.Trim() & ")"

                    CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = Replace(CType(gv.Rows(i).FindControl("lblJZ"), Label).Text, "{h}", gv.Attributes.Item("h"))
                    CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = Replace(CType(gv.Rows(i).FindControl("lblJZ"), Label).Text, "{w}", gv.Attributes.Item("w"))
                    CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = Replace(CType(gv.Rows(i).FindControl("lblJZ"), Label).Text, "{dh}", gv.Attributes.Item("dh"))
                    CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = Replace(CType(gv.Rows(i).FindControl("lblJZ"), Label).Text, "{dw}", gv.Attributes.Item("dw"))
                    CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = Replace(CType(gv.Rows(i).FindControl("lblJZ"), Label).Text, "{kw}", gv.Attributes.Item("kw"))
                    'CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = Replace(CType(gv.Rows(i).FindControl("lblJZ"), Label).Text, "{dw}", gv.Attributes.Item("dw"))
                    'CType(gv.Rows(i).FindControl("lblJZ"), Label).Text = Replace(CType(gv.Rows(i).FindControl("lblJZ"), Label).Text, "{dw}", gv.Attributes.Item("dw"))
            End Select
        Next
    End Sub



    'Sub MerCol(colIdx)
    '    Dim RowSpan As Integer = 1
    '    Dim acIdx As Integer = 1
    '    For i As Integer = 0 To gv.Rows.Count - 1
    '        acIdx = i
    '        RowSpan = 1
    '        For j As Integer = i + 1 To gv.Rows.Count - 1
    '            If gv.Rows(i).Cells(colIdx).Text = gv.Rows(j).Cells(colIdx).Text Then
    '                gv.Rows(j).Cells(colIdx).Visible = False
    '                RowSpan = RowSpan + 1
    '                acIdx = j
    '            End If
    '        Next
    '        gv.Rows(i).Cells(colIdx).RowSpan = RowSpan
    '        i = acIdx
    '    Next

    'End Sub





    Public Function GetJieguoBgColor(ByVal jieguo As String) As String
        If jieguo = "OK" Then
            Return "green"
        ElseIf jieguo = "SD" Then
            Return "green"
        ElseIf jieguo = "M1" Then
            Return "red"
        ElseIf jieguo = "M2" Then
            Return "red"
        ElseIf jieguo = "M3" Then
            Return "red"
        ElseIf jieguo = "NG" Then
            Return "red"
        ElseIf jieguo = "JN" Then
            Return "red"
        Else
            Return "#fff"
        End If
    End Function





    '返回
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        Server.Transfer("t_check_list.aspx")
    End Sub

    '完了按钮
    Protected Sub btnComplete_Click(sender As Object, e As EventArgs) Handles btnComplete.Click
        Result.UpdCheckResultStatus(CLoginInfo.ck_id, CLoginInfo.cd, CLoginInfo.user_cd, "1", True)
        Context.Items("CLoginInfo") = ViewState("CLoginInfo")
        Server.Transfer("t_check_list.aspx")
    End Sub

    '检查治具
    Protected Sub btnChkTools_Click(sender As Object, e As EventArgs) Handles btnChkTools.Click

        Dim dt As Data.DataTable = BC.GetCheckToolsData(CLoginInfo.ck_id, CLoginInfo)
        If dt.Select("tools_scan_flg='1'").Length > 0 Then
            CLoginInfo.IsToolStyle = True
            Context.Items("CLoginInfo") = CLoginInfo
            Server.Transfer("t_check_ms.aspx")
            Exit Sub
        Else


            Context.Items("CLoginInfo") = ViewState("CLoginInfo")
            Server.Transfer("t_check_tools.aspx")
        End If

    End Sub
    Protected Sub btnChkKind_Click(sender As Object, e As EventArgs) Handles btnChkKind.Click
        CLoginInfo.IsToolStyle = False

        Context.Items("CLoginInfo") = CLoginInfo
        Server.Transfer("t_check_ms.aspx")
    End Sub


    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Dim FilePath As String = Server.MapPath(".") & ("/IMG/")
        Dim Extension As String
        Dim NewFilePath As String


        Dim cm As New ComDDL
        With PicUpload1
            If .HasFile Then
                Extension = System.IO.Path.GetExtension(.PostedFile.FileName)
                NewFilePath = FilePath & System.IO.Path.GetFileNameWithoutExtension(.PostedFile.FileName) & DateTime.Now.ToString("yyyyMMddHHmmssffff") & Extension
                .SaveAs(NewFilePath)
                cm.InsMPictureChk(CLoginInfo.ck_id.ToString, (NewFilePath), CLoginInfo.user_cd)
                File.Delete(NewFilePath)
            End If
        End With

        With PicUpload2
            If .HasFile Then
                Extension = System.IO.Path.GetExtension(.PostedFile.FileName)
                NewFilePath = FilePath & System.IO.Path.GetFileNameWithoutExtension(.PostedFile.FileName) & DateTime.Now.ToString("yyyyMMddHHmmssffff") & Extension
                .SaveAs(NewFilePath)
                cm.InsMPictureChk(CLoginInfo.ck_id.ToString, (NewFilePath), CLoginInfo.user_cd)
                File.Delete(NewFilePath)
            End If
        End With

        With PicUpload3
            If .HasFile Then
                Extension = System.IO.Path.GetExtension(.PostedFile.FileName)
                NewFilePath = FilePath & System.IO.Path.GetFileNameWithoutExtension(.PostedFile.FileName) & DateTime.Now.ToString("yyyyMMddHHmmssffff") & Extension
                .SaveAs(NewFilePath)
                cm.InsMPictureChk(CLoginInfo.ck_id.ToString, (NewFilePath), CLoginInfo.user_cd)
                File.Delete(NewFilePath)
            End If
        End With
        With PicUpload4
            If .HasFile Then
                Extension = System.IO.Path.GetExtension(.PostedFile.FileName)
                NewFilePath = FilePath & System.IO.Path.GetFileNameWithoutExtension(.PostedFile.FileName) & DateTime.Now.ToString("yyyyMMddHHmmssffff") & Extension
                .SaveAs(NewFilePath)
                cm.InsMPictureChk(CLoginInfo.ck_id.ToString, (NewFilePath), CLoginInfo.user_cd)
                File.Delete(NewFilePath)
            End If
        End With
        With PicUpload5
            If .HasFile Then
                Extension = System.IO.Path.GetExtension(.PostedFile.FileName)
                NewFilePath = FilePath & System.IO.Path.GetFileNameWithoutExtension(.PostedFile.FileName) & DateTime.Now.ToString("yyyyMMddHHmmssffff") & Extension
                .SaveAs(NewFilePath)
                cm.InsMPictureChk(CLoginInfo.ck_id.ToString, (NewFilePath), CLoginInfo.user_cd)
                File.Delete(NewFilePath)
            End If
        End With


        Dim dt As Data.DataTable = GetKindMsDataOneKind(ViewState("zenkind_name"))
        InitMs(dt)

        PicMSInit()
    End Sub


    Sub PicMSInit()
        Dim cm As New ComDDL
        Dim dtImg As Data.DataTable = cm.SelChkPicture(CLoginInfo.ck_id.ToString)
        GvPic.DataSource = dtImg
        GvPic.DataBind()

        For i As Integer = 0 To dtImg.Rows.Count - 1
            Dim img As System.Web.UI.WebControls.Image = CType(GvPic.Rows(i).FindControl("Image1"), System.Web.UI.WebControls.Image)
            Dim btn As Button = GvPic.Rows(i).FindControl("btnDel")

            Dim MStream As New MemoryStream(CType(dtImg.Rows(i).Item("pic_conn"), Byte()))
            Dim base64 As String = Convert.ToBase64String(MStream.ToArray())
            img.ImageUrl = "data:image/jpg;base64," + base64
            btn.Attributes.Item("idx") = dtImg.Rows(i).Item("idx")

            ' AddHandler btn.Click, AddressOf Me.btClick
        Next


    End Sub
    Public Sub btClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim btn As Button
        btn = CType(sender, Button)
        Dim cm As New ComDDL
        cm.DelMPictureChk(CLoginInfo.ck_id.ToString, btn.Attributes.Item("idx"))
        PicMSInit()
    End Sub
End Class
