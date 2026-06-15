Public Class Com

    Public Shared Function EnumName(Of T)(ByVal value As T) As String
        Return [Enum].GetName(GetType(T), value)
    End Function
End Class
