
Imports System.Data
Imports Newtonsoft.Json

Partial Class t_check_list
    Inherits System.Web.UI.Page

    Private CLoginInfo As CLoginInfo
    Private BC As New t_checkBC
    Private DA As New t_checkDA

    'LOAD
    Private Sub t_check_list_Load(sender As Object, e As EventArgs) Handles Me.Load

        '加载画面参数
        PageCom.InitParam(Page, Context, ViewState, CLoginInfo)

        If Not IsPostBack Then
            '新规按钮不可用
            Me.btnNewChkNoPlan.Visible = False
            '加载Login信息
            UserHeader.InitLoginInfo(CLoginInfo)
            '生产性一览打开
            lbSCX.OnClientClick = "window.open('pt_v_A05_check_scx.aspx?kbn=1&user_cd=" & CLoginInfo.user_cd & "&user_name=" & CLoginInfo.user_name & "');return false;"
            lbSCX.Attributes.Item("href") = "#"
            'Gridview清空
            gvZen.DataSource = Nothing
            gvZen.DataBind()
            '前画面Panel非表示
            PanelZen.Visible = False
            CLoginInfo.IsToolStyle = False
            '加载日内列表
            HiListInit(Me.ddlHiinai)

            If CLoginInfo.cd <> "" Then
                Me.tbxCd.Text = CLoginInfo.cd
            End If
            If CLoginInfo.no <> "" Then
                Me.tbxNo.Text = CLoginInfo.no
            End If

            If CLoginInfo.TpNo <> "" Then
                Me.tbxTpNo.Text = CLoginInfo.TpNo
            End If

            If CLoginInfo.authority = "1" Then
                Me.btnBack.Enabled = True
            Else
                Me.btnBack.Enabled = False
            End If
            Me.tbxCd.Text = ""
            Me.tbxNo.Text = ""
            btnNewChk.Enabled = False
            btnSetDefault.Enabled = False
            btnReChk.Enabled = False
            'btnContinue.Enabled = False

            '设置下拉框
            If CLoginInfo.rinei <> "" Then
                Me.ddlHiinai.SelectedValue = CLoginInfo.rinei
            Else
                ddlHiinai.SelectedIndex = 3
            End If

            cb1.Checked = CLoginInfo.cb1
            cb2.Checked = CLoginInfo.cb2
            cb3.Checked = CLoginInfo.cb3
            cb4.Checked = CLoginInfo.cb4
            '加载一览
            InitMs(True, CLoginInfo.tpNos)

        End If
        '重新加载部门选择Checkbox
        CLoginInfo.cb1 = cb1.Checked
        CLoginInfo.cb2 = cb2.Checked
        CLoginInfo.cb3 = cb3.Checked
        CLoginInfo.cb4 = cb4.Checked

    End Sub

    ' 排序 DataTable 的方法
    Function SortDataTable(ByVal originalTable As DataTable, ByVal sortExpression As String) As DataTable
        ' 创建 DataView 来排序 DataTable
        Dim dataView As New DataView(originalTable)
        dataView.Sort = sortExpression

        ' 返回排序后的 DataTable
        Return dataView.ToTable()
    End Function

    ''' <summary>
    ''' 加载明细
    ''' </summary>
    ''' <param name="ShowMsg">表示Msg</param>
    ''' <param name="tpNos">托盘Nos</param>
    Sub InitMs(Optional ByVal ShowMsg As Boolean = True, Optional ByVal tpNos As String = "")

        '检索条件做成
        Dim cd As String = Me.tbxCd.Text.ToUpper.Trim '.Replace("-", "")
        Dim no As String = Me.tbxNo.Text.ToUpper.Trim
        ViewState("cd") = cd
        ViewState("no") = no

        Dim rinei As Integer = CInt(Me.ddlHiinai.SelectedValue.Trim)

        '部门 计划中的部门
        Dim department_cd As String = ""
        If cb1.Checked Then '1部
            department_cd &= IIf(department_cd = "", "'1'", ",'1'")
        End If
        If cb2.Checked Then '2部
            department_cd &= IIf(department_cd = "", "'2'", ",'2'")
        End If
        If cb3.Checked Then '3部
            department_cd &= IIf(department_cd = "", "'3'", ",'3'")
        End If
        If cb4.Checked Then '4部
            department_cd &= IIf(department_cd = "", "'4'", ",'4'")
        End If

        Me.btnNewChk.Enabled = False

        'tpNos = "'9017608139','9017398407','9017412844','9016580914'"
        'tpNos = "'9018315047'"

        'CD与工单没输入， 无计划新规按钮不可用
        If cd = "" AndAlso no = "" Then
            Me.btnNewChkNoPlan.Visible = False
        ElseIf cd <> "" AndAlso no = "" Then
            Me.btnNewChkNoPlan.Visible = True
        ElseIf cd <> "" AndAlso no <> "" Then
            Me.btnNewChkNoPlan.Visible = DA.GetCheckListForChkPlan(cd, no, tpNos).Rows.Count <= 0
        Else

        End If
        'CD 和托盘都没输入 那么不表示明细
        If cd = "" And tpNos = "" Then
            Return
        End If

        'Dim dt As Data.DataTable = DA.GetCheckList(cd, no, department_cd, CLoginInfo.line_cd, rinei, CLoginInfo.user_cd)
        '主表计划中 获得相关检查数据
        Dim dtByPlan As Data.DataTable = DA.GetCheckListForChkPlan(cd, no, tpNos)

        ''主表检查结果中 获得相关检查数据
        'Dim dtByChk As Data.DataTable = DA.GetCheckListByChk(cd, "", department_cd, CLoginInfo.line_cd, rinei, CLoginInfo.user_cd, tpNos)

        '画面明细确定， 有托盘No的 从计划取得 否则 从检查表取得
        Dim dtGenData As Data.DataTable
        If tpNos = "" Then  '输入了作番 和 CD
            '主表检查结果中 获得相关检查数据
            dtGenData = DA.GetCheckListByChk(cd, "", department_cd, CLoginInfo.line_cd, rinei, CLoginInfo.user_cd, tpNos)
        Else
            dtGenData = dtByPlan
        End If


        dtGenData.Columns.Add("buliangTxt")
        For i As Integer = 0 To dtGenData.Rows.Count - 1

            If dtGenData.Rows(i).Item("buliang_daiti") = "1" Then
                dtGenData.Rows(i).Item("buliangTxt") = "2不良替代品"

            Else
                If dtGenData.Rows(i).Item("buliang") = "0" Then
                    If dtGenData.Rows(i).Item("buliang_daiti") = "0" Then　'm_bl_manage 没有记录
                        dtGenData.Rows(i).Item("buliangTxt") = ""
                    Else
                        dtGenData.Rows(i).Item("buliangTxt") = "2不良替代品"
                    End If
                Else
                    dtGenData.Rows(i).Item("buliangTxt") = "3不良"
                End If


                If tpNos.Trim <> "" Then
                    '初回检查
                    If dtGenData.Rows(i).Item("ISFIRSTCHK") = "1" Then
                        dtGenData.Rows(i).Item("buliangTxt") = dtGenData.Rows(i).Item("buliangTxt") & "1初②" ' 1 是为了排序
                    End If

                    If dtGenData.Rows(i).Item("ISFIRSTCHK2") = "1" Then
                        dtGenData.Rows(i).Item("buliangTxt") = dtGenData.Rows(i).Item("buliangTxt") & "0初①" ' 0 是为了排序
                    End If
                End If


            End If
        Next

        ' 排序 DataTable
        Dim dtMs As DataTable = SortDataTable(dtGenData, "buliangTxt DESC")
        For i As Integer = 0 To dtMs.Rows.Count - 1

            If dtMs.Rows(i).Item("buliang_daiti") = "1" Then
                dtMs.Rows(i).Item("buliangTxt") = "2不良替代品"
                dtMs.Rows(i).Item("buliangTxt") =
                dtMs.Rows(i).Item("buliangTxt").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "")


            Else
                If dtMs.Rows(i).Item("buliang") = "0" Then
                    If dtMs.Rows(i).Item("buliang_daiti") = "0" Then 'm_bl_manage 没有记录
                        If (dtMs.Rows(i).Item("buliangTxt").ToString.Contains("初①")) Then
                            dtMs.Rows(i).Item("buliangTxt") =
                                dtMs.Rows(i).Item("buliangTxt").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "")

                        ElseIf (dtMs.Rows(i).Item("buliangTxt").ToString.Contains("初②")) Then
                            dtMs.Rows(i).Item("buliangTxt") =
                                dtMs.Rows(i).Item("buliangTxt").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "")
                        Else
                            dtMs.Rows(i).Item("buliangTxt") = ""
                        End If

                    Else
                        dtMs.Rows(i).Item("buliangTxt") =
                            dtMs.Rows(i).Item("buliangTxt").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "")
                    End If
                Else
                    dtMs.Rows(i).Item("buliangTxt") =
                    dtMs.Rows(i).Item("buliangTxt").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "")
                End If


                If tpNos.Trim <> "" Then
                    '初回检查
                    If dtMs.Rows(i).Item("ISFIRSTCHK") = "1" OrElse dtMs.Rows(i).Item("ISFIRSTCHK2") = "1" Then
                        dtMs.Rows(i).Item("buliangTxt") =
                        dtMs.Rows(i).Item("buliangTxt").Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "")
                    End If
                End If
            End If


            If dtMs.Rows(i).Item("pre_ck_id") <> "" Then
                dtMs.Rows(i).Item("buliangTxt") = dtMs.Rows(i).Item("buliangTxt") & " 不良的再检"
            End If
        Next





        '绑定画面明细
        Me.gvLastCheckResultMS.DataSource = dtMs
        Me.gvLastCheckResultMS.DataBind()

        '必要信息保存到TR
        For i As Integer = 0 To dtMs.Rows.Count - 1

            Me.gvLastCheckResultMS.Rows(i).Attributes.Item("onclick") = "ChooseRow(this,'" & dtMs.Rows(i).Item("cd").ToString & "','" & dtMs.Rows(i).Item("no").ToString & "','" & dtMs.Rows(i).Item("jxs_name").ToString & "')"

            Me.gvLastCheckResultMS.Rows(i).Attributes.Item("ck_id") = dtMs.Rows(i).Item("ck_id")

            'status   '  "0":检查中，  1:完了  2：默认结果完了 4:手入力
            If dtMs.Select("no='" & dtMs.Rows(i).Item("no") & "' and status<>'4'").Length > 0 Then
                Me.gvLastCheckResultMS.Rows(i).Attributes.Item("chk_zumi") = "1"    '检查済　あり
            Else
                Me.gvLastCheckResultMS.Rows(i).Attributes.Item("chk_zumi") = "0"
            End If

            Me.gvLastCheckResultMS.Rows(i).Attributes("ss_id") = dtMs.Rows(i).Item("ss_id")


            If dtMs.Rows(i).Item("buliang_daiti") = "1" Then
                Me.gvLastCheckResultMS.Rows(i).Cells(6).ForeColor = Drawing.Color.Red
                'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text = "不良替代品"
                Me.gvLastCheckResultMS.Rows(i).Attributes("buliang") = "2"
                'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text =
                '    Me.gvLastCheckResultMS.Rows(i).Cells(6).Text.Replace("1", "").Replace("2", "").Replace("3", "")



            Else
                If dtMs.Rows(i).Item("buliang") = "0" Then
                    If dtMs.Rows(i).Item("buliang_daiti") = "0" Then
                        'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text = ""
                        Me.gvLastCheckResultMS.Rows(i).Attributes("buliang") = "0"
                    Else
                        Me.gvLastCheckResultMS.Rows(i).Cells(6).ForeColor = Drawing.Color.Red
                        'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text = "不良替代品"
                        Me.gvLastCheckResultMS.Rows(i).Attributes("buliang") = "2"
                        '    Me.gvLastCheckResultMS.Rows(i).Cells(6).Text =
                        'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text.Replace("1", "").Replace("2", "").Replace("3", "")
                    End If
                Else
                    Me.gvLastCheckResultMS.Rows(i).Cells(6).ForeColor = Drawing.Color.Red
                    'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text = "不良"
                    Me.gvLastCheckResultMS.Rows(i).Attributes("buliang") = "1"
                    'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text =
                    'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text.Replace("1", "").Replace("2", "").Replace("3", "")
                End If


                If tpNos.Trim <> "" Then
                    '初回检查
                    If dtMs.Rows(i).Item("ISFIRSTCHK") = "1" OrElse dtMs.Rows(i).Item("ISFIRSTCHK2") = "1" Then
                        'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text = Me.gvLastCheckResultMS.Rows(i).Cells(6).Text & "<br>初"
                        Me.gvLastCheckResultMS.Rows(i).Cells(6).BackColor = Drawing.ColorTranslator.FromHtml("#33ff00")
                        '                    Me.gvLastCheckResultMS.Rows(i).Cells(6).Text =
                        'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text.Replace("1", "").Replace("2", "").Replace("3", "")
                    End If
                End If



            End If

            If dtMs.Rows(i).Item("pre_ck_id") <> "" Then
                'Me.gvLastCheckResultMS.Rows(i).Cells(6).Text = Me.gvLastCheckResultMS.Rows(i).Cells(6).Text & " 不良的再检"
            End If






        Next


        'cbQianpin
        '如果有计划检查数据 就可以新规
        Dim line_cd As String = ""
        Me.btnNewChk.Enabled = dtByPlan.Rows.Count > 0 AndAlso cd.Trim <> "" AndAlso no.Trim <> ""

        Dim tezhu_Kbn As Boolean = False
        If dtByPlan.Rows.Count > 0 Then
            ViewState("yotei_chk_date") = dtByPlan.Rows(0).Item("yotei_chk_date")
            ViewState("jxs_name") = dtByPlan.Rows(0).Item("jxs_name")
            line_cd = dtByPlan.Rows(0).Item("line_cd")

            If dtByPlan.Rows(0).Item("specialBookNo").ToString.Trim = "" Then
                lblGT.Text = "规格品"
                lblGT.ForeColor = Drawing.Color.Blue
                tezhu_Kbn = False
            ElseIf dtByPlan.Rows(0).Item("specialBookNo").ToString.Trim = "-1" Then
                lblGT.Text = "未知"
                lblGT.ForeColor = Drawing.Color.Silver
                tezhu_Kbn = False
            Else
                lblGT.Text = "特注品"
                lblGT.ForeColor = Drawing.Color.Red
                tezhu_Kbn = True
            End If

        End If

        If line_cd.Trim = "" Then
            line_cd = CLoginInfo.line_cd
        End If

        '如果不是走托盘，是直接检索商品CD和作番
        If tpNos = "" Then  '输入了作番 和 CD

            Dim dt As DataTable = DA.GetBuliangByCdNo(cd, no)

            '主表检查结果中 获得相关检查数据
            'dtMs 
            If dt.Rows.Count > 0 Then
                If dt.Rows(0).Item("buliang_daiti") <> "0" Then
                    lblGT.Text = lblGT.Text & "  不良替代品"
                End If
            End If

            'If (New m_first_chk_step2BC).Getm_first_chk_step2(cd, line_cd).Rows.Count = 0 _
            'AndAlso (New m_first_chk_step2BC).Getm_m_first_chk_cds(cd, line_cd).Rows.Count > 0 Then

            '    lblGT.Text = lblGT.Text & "  初检①"
            '    Me.btnSetDefault.Visible = False
            'End If
        Else

            Me.btnSetDefault.Visible = True
        End If

        '如果当日有检查完了的数据 ，那么就可以默认结果
        'status 0:检查中，  1:完了  2：默认结果完了 
        '如果有检查中的数据 继续检查按钮就可以使用
        'Me.btnContinue.Enabled = dt.Select("status='0'").Length > 0

        If dtByPlan.Select("status='0'").Length > 0 Then
            ViewState("ck_id_forContinue") = dtByPlan.Select("status='0'")(0).Item("ck_id").ToString
        End If

        '默认结果按钮 （如果当日生产线 已经有检查完了的数据）
        Dim sameCdDt As DataTable = DA.GetSameDaySameCd(cd, line_cd, department_cd)
        Me.btnSetDefault.Enabled = False
        If sameCdDt.Rows.Count > 0 Then
            '特注品 （相同 作番）
            If tezhu_Kbn Then
                'status   '  "0":检查中，  1:完了  2：默认结果完了 4:手入力
                If sameCdDt.Select("no='" & ViewState("no").ToString & "' and status<>'4'").Length > 0 Then
                    Dim dr As DataRow = sameCdDt.Select("no='" & ViewState("no").ToString & "' and status<>'4'")(0)
                    ViewState("ck_id_forSetDefault") = dr.Item("ck_id").ToString
                    Me.btnSetDefault.Enabled = True
                    If ShowMsg Then
                        If dr.Item("chk_end_date") = Now.ToString("yyyy-MM-dd") Then
                            PageCom.ShowMsg3(Page, "当日已经检查 " & lblGT.Text,, "green")
                        Else
                            'PageCom.ShowMsg3(Page, dr.Item("chk_end_date") & "日已经检查 " & lblGT.Text,, "green") //薛 提示 20211111 删除
                        End If
                    End If
                End If
            Else
                '规格品（CD相同即可）
                'status   '  "0":检查中，  1:完了  2：默认结果完了 4:手入力
                If sameCdDt.Select("status<>'4'").Length > 0 Then
                    Dim dr As DataRow = sameCdDt.Select("status<>'4'")(0)
                    ViewState("ck_id_forSetDefault") = sameCdDt.Select("status<>'4'")(0).Item("ck_id").ToString
                    Me.btnSetDefault.Enabled = True

                    If ShowMsg Then
                        If dr.Item("chk_end_date") = Now.ToString("yyyy-MM-dd") Then
                            PageCom.ShowMsg3(Page, "当日已经检查 " & lblGT.Text,, "green")
                        Else
                            'PageCom.ShowMsg3(Page, dr.Item("chk_end_date") & "日已经检查 " & lblGT.Text,, "green") //薛 提示 20211111 删除
                        End If
                        'PageCom.ShowMsg3(Page, "当日已经检查 " & lblGT.Text,, "green")
                    End If
                Else
                    Me.btnSetDefault.Enabled = False
                End If

            End If

            'If sameCdDt.Select("no='" & ViewState("no").ToString & "' and status<>'4'").Length > 0 Then
            '    ViewState("ck_id_forSetDefault") = sameCdDt.Select("no='" & ViewState("no").ToString & "' and status<>'4'")(0).Item("ck_id").ToString
            '    Me.btnSetDefault.Enabled = True
            '    PageCom.ShowMsg2(Page, "当日已经检查")
            'Else
            '    If sameCdDt.Select("status<>'4'").Length > 0 Then
            '        ViewState("ck_id_forSetDefault") = sameCdDt.Select("status<>'4'")(0).Item("ck_id").ToString
            '        Me.btnSetDefault.Enabled = True
            '        PageCom.ShowMsg2(Page, "当日已经检查")
            '    Else
            '        Me.btnSetDefault.Enabled = False
            '    End If
            'End If

        Else
        End If






        '以下 前回～ NG的 Gridview信息取得
        Dim dtZen As DataTable = DA.GetZenMs(cd, no, PageCom.GetNewCheckId())
        If dtZen IsNot Nothing AndAlso dtZen.Rows.Count > 0 Then
            gvZen.DataSource = dtZen
            gvZen.DataBind()
            PanelZen.Visible = True     '表示
        Else
            gvZen.DataSource = Nothing
            gvZen.DataBind()
            PanelZen.Visible = False    '不表示
        End If

        '是否是初检判断
        Dim goods_cd As String = cd

        Dim chujianMsg As String = ""

        If DA.Gettongyong_cd_step2(goods_cd) <> "" Then
            '初检商品CD判断
            If DA.GetFirstCheck_step2(goods_cd, line_cd) = "" Then
                Me.tbxCd.ForeColor = Drawing.Color.Yellow
                Dim csScript As New StringBuilder
                With csScript
                    .AppendLine("alert('①初检的CD：初次生产，需要确认机种。');")
                End With

                'chujianMsg = "初回检查①「初检的CDs」"
                ''ページ応答で、クライアント側のスクリプト ブロックを出力します
                ClientScript.RegisterStartupScript(Me.GetType(), "ShowMessage3", csScript.ToString, True)

                lblGT.Text = lblGT.Text & "  初检①"
                Me.btnSetDefault.Visible = False

            End If
        End If

        '通用商品CD判断
        If DA.Gettongyong_cd(goods_cd) <> "" Then
            '初检商品CD判断
            If DA.GetFirstCheck(goods_cd) = "" Then
                Me.tbxCd.ForeColor = Drawing.Color.Yellow
                Dim csScript As New StringBuilder
                With csScript
                    .AppendLine("alert('②相同CD：需要三方召合确认机种。');")
                End With
                'ページ応答で、クライアント側のスクリプト ブロックを出力します
                ClientScript.RegisterStartupScript(Me.GetType(), "ShowMessage4", csScript.ToString, True)

                'hujianMsg = chujianMsg & " 初回检查②「相同CD」"
            End If
        End If

        'Dim csScript As New StringBuilder
        'With csScript
        '    .AppendLine("alert('" + chujianMsg + "');")
        'End With
        ''ページ応答で、クライアント側のスクリプト ブロックを出力します
        'ClientScript.RegisterStartupScript(Me.GetType(), "ShowMessage3", csScript.ToString, True)




        '因为没有宽高等信息 ，所以不能检查
        'Else
        '    dt = DA.GetCheckList(cd, "", CLoginInfo.department_cd, CLoginInfo.line_cd, rinei, CLoginInfo.user_cd)
        '    Me.gvLastCheckResultMS.DataSource = Nothing
        '    Me.gvLastCheckResultMS.DataBind()

        '    If dt.Rows.Count > 0 Then
        '        PageCom.ShowMsg2(Me.Page, "请注意！ 不是存在的计划。只能进行新规检查。")
        '        Me.btnNewChk.Enabled = True
        '        Me.btnSetDefault.Enabled = False
        '        If dt.Select("status='0'").Length > 0 Then
        '            ViewState("ck_id_forContinue") = dt.Select("status='0'")(0).Item("ck_id").ToString
        '        End If
        '    End If
        'End If
        'btnReCheck 新规检查
        '同批次数据


    End Sub

    '几日以内
    Private Sub HiListInit(ByRef ddlHiinai As DropDownList)
        ddlHiinai.Items.Clear()
        PageCom.GetComRinei(ddlHiinai)
        ddlHiinai.SelectedIndex = 0
    End Sub

    '新规检查
    Protected Sub btnNewChk_Click(sender As Object, e As EventArgs) Handles btnNewChk.Click

        Dim ck_id As String = PageCom.GetNewCheckId()
        Dim cd As String = Me.tbxCd.Text
        Dim no As String = Me.tbxNo.Text
        Dim jxs_name As String = ViewState("jxs_name")

        Dim msg As String = BC.CreateNewChk(cd, no, ck_id, CLoginInfo.user_cd, CLoginInfo.department_cd, CLoginInfo.line_cd, ViewState("yotei_chk_date"))

        If msg <> "" Then
            PageCom.ShowMsg3(Me.Page, msg)
        Else
            CLoginInfo.cd = cd
            CLoginInfo.no = no
            CLoginInfo.ck_id = ck_id
            CLoginInfo.jxs_name = ViewState("jxs_name")
            CLoginInfo.TpNo = tbxTpNo.Text
            Session("CLoginInfo") = CLoginInfo
            Server.Transfer("t_check_ms.aspx")
        End If

    End Sub


    'NG 再检
    Private Sub btnReChk_Click(sender As Object, e As EventArgs) Handles btnReChk.Click

        Dim ck_id As String = PageCom.GetNewCheckId()
        Dim cd As String = Me.tbxCd.Text
        Dim no As String = Me.tbxNo.Text
        Dim jxs_name As String = ViewState("jxs_name")
        Dim pre_ck_id As String = Me.hid_ck_id.Value
        'CLoginInfo.jxs_name = ViewState("jxs_name")

        Dim msg As String = BC.CreateReChk(cd, no, ck_id, pre_ck_id， CLoginInfo.user_cd, CLoginInfo.department_cd, CLoginInfo.line_cd, ViewState("yotei_chk_date"))

        If msg <> "" Then
            PageCom.ShowMsg3(Me.Page, msg)
        Else
            CLoginInfo.cd = cd
            CLoginInfo.no = no
            CLoginInfo.ck_id = ck_id
            CLoginInfo.jxs_name = ViewState("jxs_name")
            CLoginInfo.TpNo = tbxTpNo.Text
            Session("CLoginInfo") = CLoginInfo
            Server.Transfer("t_check_ms.aspx")
        End If



    End Sub

    Private Sub btnNewChkNoPlan_Click(sender As Object, e As EventArgs) Handles btnNewChkNoPlan.Click

        Dim ck_id As String = PageCom.GetNewCheckId()
        Dim cd As String = Me.tbxCd.Text
        Dim no As String = Me.tbxNo.Text

        Dim msg As String = BC.CreateNewChkNoPlan(cd, no, ck_id, CLoginInfo.user_cd, CLoginInfo.department_cd, CLoginInfo.line_cd, ViewState("yotei_chk_date"))

        If msg <> "" Then
            PageCom.ShowMsg3(Me.Page, msg)
        Else
            CLoginInfo.cd = cd
            CLoginInfo.no = no
            CLoginInfo.ck_id = ck_id
            CLoginInfo.TpNo = tbxTpNo.Text
            CLoginInfo.jxs_name = ViewState("jxs_name")
            Session("CLoginInfo") = CLoginInfo
            Server.Transfer("t_check_ms.aspx")
        End If

    End Sub

    '默认结果
    Protected Sub btnSetDefault_Click(sender As Object, e As EventArgs) Handles btnSetDefault.Click
        'ViewState("ck_id_forSetDefault")

        Dim shared_ck_id As String = ViewState("ck_id_forSetDefault").ToString()

        Dim ck_id As String = PageCom.GetNewCheckId()

        Dim no As String = Me.tbxNo.Text

        Result.InsCopyDefault(shared_ck_id, no, ck_id, CLoginInfo)

        InitMs(False)


        '        update [t_check]
        'set suu = [t_check_plan].suu
        'From [t_check] inner Join [t_check_plan]
        'On [t_check].[no]=[t_check_plan].[no]
        'And [t_check].suu<>[t_check_plan].suu
        PageCom.ShowMsg3(Page, "OK 默认结果完成 ！！ " & lblGT.Text,, "green")

    End Sub

    '检索
    Protected Sub btnSel_Click(sender As Object, e As EventArgs) Handles btnSel.Click
        InitMs()
    End Sub

    '继续检查
    'Protected Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
    '    CLoginInfo.cd = ViewState("cd").ToString
    '    CLoginInfo.no = ViewState("no").ToString
    '    CLoginInfo.rinei = Me.ddlHiinai.SelectedValue.Trim
    '    CLoginInfo.ck_id = ViewState("ck_id_forContinue").ToString
    '    '设置参数
    '    PageCom.SetInitParam(Page, Context, ViewState, CLoginInfo)
    '    Server.Transfer("t_check_ms.aspx")
    'End Sub

    '手入力
    Protected Sub btnInputByHand_Click(sender As Object, e As EventArgs) Handles btnInputByHand.Click
        PageCom.SetInitParam(Page, Context, ViewState, CLoginInfo)
        Server.Transfer("InputByHand.aspx")
    End Sub

    '返回
    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        PageCom.SetInitParam(Page, Context, ViewState, CLoginInfo)
        Server.Transfer("Menu.aspx")
    End Sub


    '编辑按钮
    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        Dim cd As String = Me.tbxCd.Text
        Dim no As String = Me.tbxNo.Text
        CLoginInfo.cd = cd
        CLoginInfo.no = no
        CLoginInfo.TpNo = tbxTpNo.Text
        CLoginInfo.rinei = Me.ddlHiinai.SelectedValue.Trim
        CLoginInfo.ck_id = Me.hid_ck_id.Value
        CLoginInfo.jxs_name = ViewState("jxs_name")
        '设置参数
        PageCom.SetInitParam(Page, Context, ViewState, CLoginInfo)
        Server.Transfer("t_check_ms.aspx")
    End Sub


    Private Sub btnDel_Click(sender As Object, e As EventArgs) Handles btnDel.Click
        Result.DelCheckMsAndResult(Me.hid_ck_id.Value)
        InitMs(False)
    End Sub


    Public Function GetIdAndNo(ByVal ck_id As String, ByVal cd As String, ByVal no As String, ByVal status As String, ByVal chk_user As String) As String
        Dim sb As New StringBuilder

        '自己只能编辑自己记录
        If CLoginInfo.authority <> "1" AndAlso chk_user <> CLoginInfo.user_cd Then
            sb.Append("<a class="""" >")
            sb.Append(cd & "<br>" & no)
            sb.Append("</a>")
            Return sb.ToString()
        End If


        If ck_id = "" Then
            sb.Append("<a class="""" >")
            sb.Append(cd & "<br>" & no)
            sb.Append("</a>")



        Else
            If status <> "4" Then
                sb.Append("<a class=""edit_link"" onclick=""Edit('" & ck_id & "')"">")
                sb.Append(cd & "<br>" & no & "<br>" & ck_id)
                sb.Append("</a>")
            Else
                sb.Append("<a>")
                sb.Append(cd & "<br>" & no & "<br>" & ck_id)
                sb.Append("</a>")
            End If

        End If
        Return sb.ToString()
    End Function

    '结果文字颜色
    Public Function GetResultEle(ByVal ck_id As String, ByVal result As String, ByVal status As String) As String
        Dim sb As New StringBuilder
        sb.Append("<a ")
        sb.Append("style=""")
        If result = "NG" Then
            sb.Append("color:red;")
        ElseIf result = "OK" Then
            sb.Append("color:Green;")
        Else

        End If
        sb.Append("""")
        sb.Append(">")
        sb.Append(result)

        sb.Append("<br>")
        If status = "0" Then
            sb.Append("检查中")
        ElseIf status = "1" Then
            sb.Append("完了")
        ElseIf status = "2" Then
            sb.Append("继承")
        ElseIf status = "4" Then
            sb.Append("手入力")
        End If


        sb.Append("</a>")

        Return sb.ToString

    End Function

    '删除按钮状态
    Public Function GetDelBtnDisabled(ByVal ck_id As String) As String
        If ck_id = String.Empty Then
            Return String.Empty
        Else
            If CLoginInfo.authority = "1" Then
                Return "<input type=""button"" value=""删除"" class=""btn_common_new"" onclick=""Del('" & ck_id & "')"""
            Else
                Return String.Empty
            End If
        End If


    End Function

    '清除按钮
    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        CLoginInfo.cd = ""
        CLoginInfo.no = ""
        CLoginInfo.TpNo = ""
        CLoginInfo.jxs_name = ""
        Server.Transfer("t_check_list.aspx")
    End Sub
    Protected Sub gvZen_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvZen.SelectedIndexChanged

    End Sub

    Private Sub btnTpChkList_Click(sender As Object, e As EventArgs) Handles btnTpChkList.Click

        Dim TPVB As New TPVB

        Dim jsonData = JsonConvert.DeserializeObject(TPVB.GetListByTray(Me.tbxTpNo.Text))

        Dim tpNos As String = ""

        Dim showMsg As Boolean = False

        If jsonData IsNot Nothing Then

            For i As Integer = 0 To JsonConvert.DeserializeObject(jsonData("data")).Count - 1
                If i = 0 Then
                    tpNos = tpNos & "'" & JsonConvert.DeserializeObject(jsonData("data"))(i)("OrderId").Value & "'"
                Else
                    tpNos = tpNos & ",'" & JsonConvert.DeserializeObject(jsonData("data"))(i)("OrderId").Value & "'"
                End If

            Next


            If JsonConvert.DeserializeObject(jsonData("data")).Count = 0 Then
                showMsg = True
            End If

            'ページ応答で、クライアント側のスクリプト ブロックを出力します
            'ClientScript.RegisterStartupScript(Me.GetType(), "btnTpChkList_Click33", "alert('" & JsonConvert.DeserializeObject(jsonData("data")).Count & "');", True)
            CLoginInfo.tpNos = tpNos
            InitMs(False, tpNos)
        Else
            showMsg = True
        End If


        If showMsg Then
            Dim csScript As New StringBuilder
            With csScript
                .AppendLine("alert('" & Me.tbxTpNo.Text & ":没有托盘关联的检查一览数据！！');")
            End With
            'ページ応答で、クライアント側のスクリプト ブロックを出力します
            ClientScript.RegisterStartupScript(Me.GetType(), "btnTpChkList_Click", csScript.ToString, True)
        End If

    End Sub

End Class
