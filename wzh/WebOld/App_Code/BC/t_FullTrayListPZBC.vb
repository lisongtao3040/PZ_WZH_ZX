Imports System.Data
Imports System.Collections.Generic

''' <summary>
''' 満台盤一覧（品質版）業務クラス
''' </summary>
Public Class t_FullTrayListPZBC

    Private DA As New t_FullTrayListPZDA

    ' m_user.department_cd に含まれる数字 → 閲覧できる部門名
    Private Shared ReadOnly DEPARTMENT_MAP As New Dictionary(Of Char, String) From {
        {"1"c, "一制品质"},
        {"2"c, "二制品质"},
        {"3"c, "三制品质"},
        {"4"c, "国内贩卖"}
    }

    ''' <summary>
    ''' m_user の department_cd（生の値）を返す
    ''' </summary>
    Public Function GetDepartmentCd(ByVal userCd As String) As String
        userCd = userCd.Trim()
        If userCd = "" Then Return ""

        Dim dt As DataTable = DA.GetUserDepartmentCd(userCd)
        If dt.Rows.Count = 0 Then Return ""

        Return dt.Rows(0)("department_cd").ToString().Trim()
    End Function

    ''' <summary>
    ''' ユーザーが閲覧できる部門名を返す（m_user.department_cd の 1～4 を判定）
    ''' 例："12" → 一制品质＋二制品质、"4" → 国内贩卖、該当なし → 空
    ''' </summary>
    Public Function GetUserDepartments(ByVal userCd As String) As List(Of String)
        Dim departments As New List(Of String)

        Dim departmentCd As String = GetDepartmentCd(userCd)
        For Each ch As Char In departmentCd
            If DEPARTMENT_MAP.ContainsKey(ch) AndAlso Not departments.Contains(DEPARTMENT_MAP(ch)) Then
                departments.Add(DEPARTMENT_MAP(ch))
            End If
        Next

        Return departments
    End Function

    ''' <summary>
    ''' 満台盤明細（品質版）を取得する（閲覧可能部門のみ）
    ''' 工单号（OrderNo）をキーに t_check の検査結果を突合して result 列にマージする
    ''' （画面側で OK 以外のときだけ「检查／自动OK」ボタンを出す）
    ''' </summary>
    Public Function GetFullTrayListPinzhi(ByVal departments As List(Of String)) As DataTable
        ' IN 句を作成（シングルクォートエスケープ）
        Dim parts As New List(Of String)
        For Each department As String In departments
            parts.Add("'" & department.Replace("'", "''") & "'")
        Next

        Dim dtTray As DataTable = DA.GetFullTrayListPinzhi(String.Join(",", parts.ToArray()))

        ' result 列を追加
        If Not dtTray.Columns.Contains("result") Then
            dtTray.Columns.Add("result", GetType(String))
        End If

        If Not dtTray.Columns.Contains("sortKey1") Then
            dtTray.Columns.Add("sortKey1", GetType(String))
        End If

        If Not dtTray.Columns.Contains("sortKey2") Then
            dtTray.Columns.Add("sortKey2", GetType(String))
        End If

        ' dtTray から 工单号（OrderNo）を重複排除で収集
        Dim orderNoSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In dtTray.Rows
            Dim orderNo As String = row("OrderNo").ToString().Trim()
            If orderNo <> "" Then orderNoSet.Add(orderNo)
        Next

        ' t_check から no → result の辞書を作成
        Dim resultDict As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        If orderNoSet.Count > 0 Then
            Dim orderNoParts As New List(Of String)
            For Each key As String In orderNoSet
                orderNoParts.Add("'" & key.Replace("'", "''") & "'")
            Next

            Dim dtCheck As DataTable = DA.GetCheckResult(String.Join(",", orderNoParts.ToArray()))
            For Each row As DataRow In dtCheck.Rows
                Dim no As String = row("no").ToString().Trim()
                If no <> "" AndAlso Not resultDict.ContainsKey(no) Then
                    resultDict(no) = row("result").ToString()
                End If
            Next
        End If

        ' 工单号（OrderNo）でマージ
        For Each row As DataRow In dtTray.Rows
            Dim orderNo As String = row("OrderNo").ToString().Trim()
            row("sortKey2") = "4"
            row("result") = If(resultDict.ContainsKey(orderNo), resultDict(orderNo), "")

            If row("result") = "" Then
                row("sortKey2") = "1"
            ElseIf row("result") = "待" Then
                row("sortKey2") = "2"
            ElseIf row("result") = "NG" Then
                row("sortKey2") = "3"
            ElseIf row("result") = "OK" Then
                row("sortKey2") = "4"
            End If
        Next

        For Each row As DataRow In dtTray.Rows

            If dtTray.Select("existTrolleyNo='" & row("existTrolleyNo") & "' AND (sortKey2='1' or sortKey2='2')").Length > 0 Then
                row("sortKey1") = "1"
            Else
                row("sortKey1") = "2"
            End If

        Next




        ' 初检／三方 列を追加
        AddFirstCheckColumns(dtTray)

        Dim dv As DataView = dtTray.DefaultView
        dv.Sort = "sortKey1 ASC, existTrolleyNo ASC, sortKey2 DESC"

        Return dv.ToTable
    End Function

    ''' <summary>
    ''' 初检／三方 列を追加する
    ''' 判定内容は既存 t_FullTrayList（FullTrayListApi.GetData）と同一
    ''' </summary>
    Private Sub AddFirstCheckColumns(ByVal dtTray As DataTable)
        If Not dtTray.Columns.Contains("firstCheck") Then
            dtTray.Columns.Add("firstCheck", GetType(String))
        End If
        If Not dtTray.Columns.Contains("thirdParty") Then
            dtTray.Columns.Add("thirdParty", GetType(String))
        End If

        Dim CheckDA As New t_checkDA

        ' 通用CD判定用に CD（productCode）を重複排除で収集
        Dim cdSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In dtTray.Rows
            Dim productCode As String = row("productCode").ToString().Trim()
            If productCode <> "" Then cdSet.Add(productCode)
        Next

        Dim dic_cd_tongyong As New Dictionary(Of String, String)
        Dim dic_f As New Dictionary(Of String, String)
        If cdSet.Count > 0 Then
            Dim cdParts As New List(Of String)
            For Each productCode As String In cdSet
                cdParts.Add("'" & productCode.Replace("'", "''") & "'")
            Next

            Dim dtMerged As DataTable = CheckDA.Gettongyong_cd_Merged_CDS_Large(String.Join(",", cdParts.ToArray()))
            For Each row As DataRow In dtMerged.Rows
                Dim cd As String = row("cd").ToString().Trim()
                Dim tongyong_cd As String = row("tongyong_cd").ToString().Trim()
                Dim source_type As String = row("source_type").ToString().Trim()

                If source_type = "STEP2" Then
                    '三方
                    If Not dic_cd_tongyong.ContainsKey(cd) Then
                        dic_cd_tongyong.Add(cd, tongyong_cd)
                    End If
                Else
                    '初检
                    If Not dic_f.ContainsKey(cd) Then
                        dic_f.Add(cd, tongyong_cd)
                    End If
                End If
            Next
        End If

        ' 既存 t_FullTrayList と同じ判定（dic_cd_tongyong＝STEP2／dic_f＝初检）
        For Each row As DataRow In dtTray.Rows
            Dim goods_cd As String = row("productCode").ToString()
            Dim line_cd As String = row("lineCodeShort").ToString()

            Dim firstCheck As String = ""
            Dim thirdParty As String = ""

            If dic_cd_tongyong.ContainsKey(goods_cd.Replace("-", "")) Then
                If CheckDA.GetFirstCheck_step2(goods_cd, line_cd) = "" Then
                    firstCheck = "YES"
                End If
            End If

            If dic_f.ContainsKey(goods_cd.Replace("-", "")) Then
                If CheckDA.GetFirstCheck(goods_cd) = "" Then
                    thirdParty = "YES"
                End If
            End If

            row("firstCheck") = firstCheck
            row("thirdParty") = thirdParty
        Next
    End Sub

End Class
