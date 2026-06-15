Imports System.Globalization
Imports System.Text

Friend NotInheritable Class HexConverter
    Public Shared Function ToHex(ByVal source As String) As Byte()
        If source = "" Then
            Return Nothing
        End If

        source = source.Replace(" ", "")
        Dim num As Integer = 2
        Dim num2 As Integer = source.Length / num
        Dim array As Byte() = New Byte(num2 - 1) {}

        For i As Integer = 0 To num2 - 1
            array(i) = Byte.Parse(source.Substring(i * num, num), NumberStyles.HexNumber)
        Next

        Return CType(array.Clone(), Byte())
    End Function

    Public Shared Function ToString(ByVal source As Byte()) As String
        Dim num As Integer = 2
        Dim num2 As Integer = source.Length
        Dim stringBuilder As StringBuilder = New StringBuilder(num2 * num)

        For i As Integer = 0 To num2 - 1

            If i <> 0 Then
                stringBuilder.Append(" ")
            End If

            If source(i) = 0 Then
                stringBuilder.Append("00")
            Else
                stringBuilder.Append(source(i).ToString("X2"))
            End If
        Next

        Return stringBuilder.ToString()
    End Function
End Class
