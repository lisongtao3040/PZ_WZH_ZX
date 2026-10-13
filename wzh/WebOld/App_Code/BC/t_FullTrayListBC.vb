Imports System.Data
Imports System.Collections.Generic
Imports System.Linq

Public Class t_FullTrayListBC

    Private DA As New t_FullTrayListDA

    ''' <summary>
    ''' 満台盤一覧取得後、OrderNo を IN 句として t_check を絞り込み result をマージして返す
    ''' </summary>
    Public Function GetFullTrayListWithResult() As DataTable
        Dim dtTray As DataTable = DA.GetFullTrayList()

        ' result・line_name 列を追加
        If Not dtTray.Columns.Contains("result") Then
            dtTray.Columns.Add("result", GetType(String))
        End If
        If Not dtTray.Columns.Contains("line_name") Then
            dtTray.Columns.Add("line_name", GetType(String))
        End If

        If Not dtTray.Columns.Contains("sortKey1") Then
            dtTray.Columns.Add("sortKey1", GetType(String))
        End If

        If Not dtTray.Columns.Contains("sortKey2") Then
            dtTray.Columns.Add("sortKey2", GetType(String))
        End If


        ' m_line から line_cd→line_name 辞書を作成してマージ
        Dim dtLine As DataTable = DA.GetLineNames()
        Dim lineDict As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In dtLine.Rows
            Dim cd As String = row("line_cd").ToString().Trim()
            If cd <> "" AndAlso Not lineDict.ContainsKey(cd) Then
                lineDict(cd) = row("line_name").ToString().Trim()
            End If
        Next
        For Each row As DataRow In dtTray.Rows
            Dim cd As String = row("lineCodeShort").ToString().Trim()
            row("line_name") = If(lineDict.ContainsKey(cd), lineDict(cd), "")
        Next


        ' dtTray から OrderNo を重複排除で収集
        Dim noSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In dtTray.Rows
            Dim orderNo As String = row("OrderNo").ToString().Trim()
            If orderNo <> "" Then noSet.Add(orderNo)
        Next

        ' OrderNo が1件もなければここで終了
        If noSet.Count = 0 Then Return dtTray

        ' IN 句文字列を作成（シングルクォートエスケープ）
        Dim nosInClause As String = String.Join(",",
            noSet.Select(Function(n) "'" & n.Replace("'", "''") & "'").ToArray())

        ' t_check から result を取得
        Dim dtCheck As DataTable = DA.GetCheckResult(nosInClause)

        ' no → result の辞書を作成
        Dim resultDict As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In dtCheck.Rows
            Dim no As String = row("no").ToString().Trim()
            If no <> "" AndAlso Not resultDict.ContainsKey(no) Then
                resultDict(no) = row("result").ToString()
            End If
        Next

        ' マージ
        For Each row As DataRow In dtTray.Rows
            Dim orderNo As String = row("OrderNo").ToString().Trim()
            If orderNo <> "" AndAlso resultDict.ContainsKey(orderNo) Then
                row("result") = resultDict(orderNo)
            Else
                row("result") = ""
            End If
        Next

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

            If dtTray.Select("trayNo='" & row("trayNo") & "' AND (sortKey2='1' or sortKey2='2')").Length > 0 Then
                row("sortKey1") = "1"
            Else
                row("sortKey1") = "2"
            End If

        Next

        Dim dv As DataView = dtTray.DefaultView
        dv.Sort = "sortKey1 ASC, trayNo ASC, sortKey2 DESC"

        Return dv.ToTable

        'Return dtTray
    End Function

End Class
