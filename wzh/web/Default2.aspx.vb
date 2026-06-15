
Imports Newtonsoft.Json

Partial Class Default2
    Inherits System.Web.UI.Page

    Private Sub Default2_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim TPVB As New TPVB



        Dim jsonData = JsonConvert.DeserializeObject(TPVB.GetListByTrayTest2())

    End Sub
End Class
