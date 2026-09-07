
Imports System.Data

Partial Class test
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim ck_id As String = "250609145633390"
        Dim cd As String = "YY-AA055HR-MWH4"
        Dim ResultDA As New t_checkDA
        'Dim dtChk As DataTable = ResultDA.GetCheckLineCd(ck_id)

        'Dim step2DA As m_first_chk_step2DA = New m_first_chk_step2DA
        'If (dtChk.Rows.Count > 0) Then

        '    Dim line_cd As String = dtChk.Rows(0).Item(0).ToString
        '    Dim result As String = dtChk.Rows(0).Item(1).ToString

        '    If result = "OK" Then
        '        Dim first2Dt = step2DA.Getm_first_chk_step2(cd, dtChk.Rows(0).Item(0).ToString)
        '        If (first2Dt.Rows.Count = 0) Then
        '            step2DA.Ins_m_first_chk_step2(cd, line_cd, "")
        '        End If
        '    End If


        'End If
        Dim tongyong_cd As String

        Dim dtChk As DataTable = ResultDA.GetCheckLineCd(ck_id)

        If dtChk.Rows.Count > 0 Then

            Dim line_cd As String = dtChk.Rows(0).Item(0).ToString
            Dim result As String = dtChk.Rows(0).Item(1).ToString

            If result = "OK" Then
                tongyong_cd = ResultDA.Gettongyong_cd_step2(cd)

                If tongyong_cd <> "" Then
                    ResultDA.UpdFirstCheck2(tongyong_cd)

                    Dim step2DA As m_first_chk_step2DA = New m_first_chk_step2DA
                    step2DA.Ins_m_first_chk_step2(cd, line_cd, "fff")
                    'Else
                    '    ResultDA.InsFirstCheck(Me.tbxGoodsCd.Text.Trim, Me.tbxGoodsCd.Text.Trim)
                End If
            End If

        End If

    End Sub
End Class
