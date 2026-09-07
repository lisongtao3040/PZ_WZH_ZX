Imports System.Data
Imports Microsoft.VisualBasic
Imports SqlHelper.SqlHelper
Imports SqlHelper
Imports System.CodeDom.Compiler


Public Class tpDA
    '品质区托盘一览
    ''' <summary>
    ''' </summary>
    ''' <param name="trolleyStatus">【03.品质待检】【04.品质检查完】</param>
    ''' <returns></returns>
    Public Function GetTPList(ByVal trolleyStatus As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT")
        sb.AppendLine("     tl.trolleyNo '托盘号'")
        sb.AppendLine("    ,min(bp.BillNo)    '最早出荷日期' ")
        sb.AppendLine("    ,sum(bp.PackageAmount) '制品捆包数'")
        sb.AppendLine("--	 ,tlsta.innerCodeContent")
        sb.AppendLine("FROM   (SELECT tt.trolleyNo")
        sb.AppendLine("               ,tt.trolleyStatus")
        sb.AppendLine("               ,tld.zuofan")
        sb.AppendLine("        FROM   [dbo].[Inner_TrolleyList] tt")
        sb.AppendLine("               LEFT JOIN (SELECT trolleyNo,zuofan,deleteFlag")
        sb.AppendLine("                          FROM   [dbo].[inner_trolleyLoadDetail]")
        sb.AppendLine("                          WHERE  deleteFlag = '0'")
        sb.AppendLine("                                 AND curRemainPkQuantity > 0) tld")
        sb.AppendLine("                      ON tt.trolleyNo = tld.trolleyNo --取得装载的明细书")
        sb.AppendLine("        WHERE  1 = 1")
        sb.AppendLine("               AND tt.deleteFlag <> '1' --有效的托盘")
        sb.AppendLine("               AND tld.deleteFlag <> '1' --有效的")
        sb.AppendLine("       ) tl")
        sb.AppendLine("       LEFT JOIN TCM_BianPlan bp")
        sb.AppendLine("              ON tl.zuofan = bp.ZuoFan")
        sb.AppendLine("       --LEFT JOIN [dbo].[TCM_InnerCode] tlSta")
        sb.AppendLine("       --       ON tl.trolleyStatus = tlSta.innerCodeName")
        sb.AppendLine("       --          AND tlSta.innerCode = 'G001'")
        sb.AppendLine("WHERE  1 = 1")
        sb.AppendLine("       AND bp.Enable = '1' ")
        sb.AppendLine("       AND bp.BillNo IS NOT NULL")
        sb.AppendLine("       AND tl.trolleyStatus = '" & trolleyStatus & "'")
        sb.AppendLine("GROUP BY")
        sb.AppendLine("	tl.trolleyNo")
        sb.AppendLine("	--,tlsta.innerCodeContent")
        sb.AppendLine("order by cast(tl.trolleyNo as int)")
        Dim dt As DataTable = FillData(DataAccessManager.TCMConnStr, CommandType.Text, sb.ToString(), "GetTPList")
        Return dt
    End Function

    Public Function GetTPMs(ByVal tp_no As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("select")
        sb.AppendLine("	bp.ZuoFan '工单号'")
        sb.AppendLine("	,bp.ProductCode '商品CD'")
        sb.AppendLine("	,bp.PackageAmount '捆包数'")
        sb.AppendLine("	,bp.DealerAbbreviation '经销商'")
        sb.AppendLine("	,bp.CD_DeliveryAddress '地址代码'")
        sb.AppendLine("	,bp.BillNo  '预定出荷日'")
        sb.AppendLine("	,bp.specialBookNo '特注书号'")
        sb.AppendLine(" --,* ")
        sb.AppendLine(" from Inner_TrolleyList tt")
        sb.AppendLine("left join inner_trolleyLoadDetail tld")
        sb.AppendLine("on tld.deleteFlag = '0'")
        sb.AppendLine("and tld.curRemainPkQuantity > 0")
        sb.AppendLine("and tt.trolleyNo = tld.trolleyNo")
        sb.AppendLine("inner JOIN TCM_BianPlan bp")
        sb.AppendLine("ON tld.zuofan = bp.ZuoFan")
        sb.AppendLine(" WHERE  1 = 1")
        sb.AppendLine("  AND tt.deleteFlag <> '1' --有效的托盘")
        sb.AppendLine("   AND tld.deleteFlag <> '1' --有效的")
        sb.AppendLine("   AND tt.trolleyNo = " & tp_no)
        Dim dt As DataTable = FillData(DataAccessManager.TCMConnStr, CommandType.Text, sb.ToString(), "GetTPMs")
        Return dt
    End Function


    Public Function GetTPListSigle(ByVal trolleyStatus As String) As DataTable
        Dim sb As New StringBuilder
        sb.AppendLine("SELECT * FROM Inner_TrolleyList tl")
        sb.AppendLine("WHERE tl.trolleyStatus = '" & trolleyStatus & "'")
        sb.AppendLine("order by cast(tl.trolleyNo as int)")
        Dim dt As DataTable = FillData(DataAccessManager.TCMConnStr, CommandType.Text, sb.ToString(), "GetTPListSigle")
        Return dt
    End Function


    ''' <summary>
    ''' 更新托盘状态
    ''' </summary>
    ''' <returns></returns>
    Function UpdTrolleyStatus(ByVal trolleyNo As String, ByVal trolleyStatusFrom As String, ByVal trolleyStatusTo As String) As String

        Dim sb As New StringBuilder
        sb.AppendLine("select trolleyNo,trolleyStatus from Inner_TrolleyList where trolleyNo = '" & trolleyNo & "'")
        Dim dt As DataTable = FillData(DataAccessManager.TCMConnStr, CommandType.Text, sb.ToString(), "UpdTrolleyStatus")

        If dt.Rows.Count <= 0 Then
            Return "未能找到托盘：(" & trolleyNo & ")"
        End If

        If dt.Rows(0).Item("trolleyStatus") <> trolleyStatusFrom Then
            Return "已经找到托盘：(" & trolleyNo & ")" & "但是未找到对应状态：（" & trolleyStatusFrom & "）"
        End If

        sb.Length = 0
        sb.Clear()

        sb.AppendLine("UPDATE Inner_TrolleyList SET trolleyStatus = '" & trolleyStatusTo & "' where trolleyNo = '" & trolleyNo & "'")

        '登录检查结果
        ExecuteNonQuery(DataAccessManager.TCMConnStr, CommandType.Text, sb.ToString())
        'sb.Length = 0
        'sb.Clear()
        'sb = Nothing
        Return ""

    End Function



End Class
