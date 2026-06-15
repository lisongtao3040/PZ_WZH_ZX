Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Class MergeGridViewCell
    Public Shared Sub MergeRow(ByVal gv As GridView, ByVal startCol As Integer, ByVal endCol As Integer)
        If startCol < 0 Then Throw New ArgumentOutOfRangeException("startCol", "开始列不能小于0")
        If endCol < 0 Then Throw New ArgumentOutOfRangeException("endCol", "结束列不能小于0")
        If startCol > endCol Then Throw New ArgumentException("开始列不能小于结束列")
        Dim init = New RowArg() With {
            .StartRowIndex = 0,
            .EndRowIndex = gv.Rows.Count - 1
        }

        For i As Integer = startCol To endCol + 1 - 1

            If i > 0 Then
                Dim list = New List(Of RowArg)()
                IteratePrevCol(gv, i - 1, list)

                For Each item In list
                    MergeRow(gv, i, item.StartRowIndex, item.EndRowIndex)
                Next
            Else
                MergeRow(gv, i, init.StartRowIndex, init.EndRowIndex)
            End If
        Next
    End Sub

    Public Shared Sub MergeRow(ByVal gv As GridView, ParamArray cols As Integer())
        If cols.Any(Function(t) t < 0) Then
            Throw New ArgumentOutOfRangeException("参数中不能包含小于0列")
        End If

        Dim init = New RowArg() With {
            .StartRowIndex = 0,
            .EndRowIndex = gv.Rows.Count - 2
        }

        For i As Integer = 0 To cols.Length - 1

            If i > 0 Then
                Dim list = New List(Of RowArg)()
                IteratePrevCol(gv, cols(i - 1), list)

                For Each item In list
                    MergeRow(gv, cols(i), item.StartRowIndex, item.EndRowIndex)
                Next
            Else
                MergeRow(gv, i, init.StartRowIndex, init.EndRowIndex)
            End If
        Next
    End Sub

    Public Shared Sub MergeColumn(ByVal gv As GridView, ByVal startCol As Integer, ByVal endCol As Integer, ByVal containHeader As Boolean)
        If containHeader Then
            IterateRowCells(gv.HeaderRow, startCol, endCol)
        End If

        For Each row As GridViewRow In gv.Rows
            IterateRowCells(row, startCol, endCol)
        Next
    End Sub

    Private Shared Sub MergeRow(ByVal gv As GridView, ByVal currentCol As Integer, ByVal startRow As Integer, ByVal endRow As Integer)
        For rowIndex As Integer = endRow To startRow
            Dim currentRow As GridViewRow = gv.Rows(rowIndex)
            Dim prevRow As GridViewRow = gv.Rows(rowIndex + 1)

            If currentRow.Cells(currentCol).Text <> "" AndAlso currentRow.Cells(currentCol).Text <> " " Then

                If currentRow.Cells(currentCol).Text = prevRow.Cells(currentCol).Text Then
                    currentRow.Cells(currentCol).RowSpan = If(prevRow.Cells(currentCol).RowSpan < 1, 2, prevRow.Cells(currentCol).RowSpan + 1)
                    prevRow.Cells(currentCol).Visible = False
                End If
            End If
        Next
    End Sub

    Private Shared Sub IterateRowCells(ByVal row As GridViewRow, ByVal start As Integer, ByVal [end] As Integer)
        For i As Integer = start + 1 To [end]
            Dim currCell As TableCell = row.Cells(i)
            Dim prevCell As TableCell = row.Cells(i - 1)

            If Not String.IsNullOrEmpty(currCell.Text) AndAlso Not String.IsNullOrEmpty(prevCell.Text) Then

                If currCell.Text = prevCell.Text Then
                    currCell.ColumnSpan = If(prevCell.ColumnSpan < 1, 2, prevCell.ColumnSpan + 1)
                    prevCell.Visible = False
                End If
            End If
        Next
    End Sub

    Private Shared Sub IteratePrevCol(ByVal gv As GridView, ByVal prevCol As Integer, ByVal list As List(Of RowArg))
        If list Is Nothing Then
            list = New List(Of RowArg)()
        End If

        For Each row As GridViewRow In gv.Rows
            If Not row.Cells(prevCol).Visible Then Continue For
            list.Add(New RowArg With {
                .StartRowIndex = row.RowIndex,
                .EndRowIndex = row.RowIndex + row.Cells(prevCol).RowSpan - 2
            })
        Next
    End Sub

    Class RowArg
        Public Property StartRowIndex As Integer
        Public Property EndRowIndex As Integer
    End Class
End Class
