Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports System.Web.UI.ClientScriptManager

Namespace UserControls


    '自定义 Textbox
    <DefaultProperty("Text"), ToolboxData("<{0}:Ilike_TextBox runat=server></{0}:Ilike_TextBox>")>
    Public Class Ilike_TextBox
        Inherits TextBox
#Region "属性"

#Region "共通"

#End Region

#Region "Input type 数字 字母 英数字"
        Private _InputType As String = "InputType"

        Public Overridable Property InputType() As CommonType.TextboxInputParam.InputTypeParam
            Set(value As CommonType.TextboxInputParam.InputTypeParam)
                Me.Attributes.Item(_InputType) = CommonType.Com.EnumName(value)

                'Dim a As CommonType.TextboxInputParam.InputTypeParam
                'Me.Parent.Page 
            End Set
            Get
                Return CType(Me.Attributes.Item(_InputType).ToString, CommonType.TextboxInputParam.InputTypeParam)
            End Get
        End Property
#End Region

#Region "必须输入"
        Private _MustInputType As String = "MustInput"
        Public Enum MustInputTypeParam
            IsTrue = 0
            IsFalse = 1
        End Enum
        Public Overridable Property MustInputType() As MustInputTypeParam
            Set(value As MustInputTypeParam)
                Me.Attributes.Item(_MustInputType) = CommonType.Com.EnumName(value)
                'Me.Parent.Page 
            End Set
            Get
                Return CType(Me.Attributes.Item(_MustInputType).ToString, MustInputTypeParam)
            End Get
        End Property
#End Region

#Region "InputName"
        Private _InputName As String = "InputName"
        Public Overridable Property InputName() As String
            Set(value As String)
                Me.Attributes.Item(_InputName) = value
            End Set
            Get
                Return Me.Attributes.Item(_InputName).ToString()
            End Get
        End Property
#End Region

#End Region

#Region "主重载方法"
        '画面表示前 OnInit
        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            '-http://www.webdiyer.com/aspnetpager/docs/method_addattributestorender/
            If Not Me.CssClass.Contains("Ilike_TextBox") Then
                If Me.CssClass.Trim = "" Then
                    Me.CssClass = "Ilike_TextBox"
                Else
                    Me.CssClass += " Ilike_TextBox"
                End If
            End If
            MyBase.OnInit(e)
        End Sub

        '画面输出 AddAttributesToRender
        Protected Overrides Sub AddAttributesToRender(ByVal writer As HtmlTextWriter)
            '必须输入项目检查
            '类型
            If Me.Attributes.Item(_InputType) Is Nothing Then
                ' Me.Style.Item("back-groudcolor") = "red"
                Me.BackColor = Drawing.Color.Red
                Me.ForeColor = Drawing.Color.White
                Me.Text = "没有设置 InputType 属性"
                Me.Width = 1000
            End If
            '名
            If Me.Attributes.Item(_InputName) Is Nothing Then
                ' Me.Style.Item("back-groudcolor") = "red"
                Me.BackColor = Drawing.Color.Red
                Me.ForeColor = Drawing.Color.White
                Me.Text = "没有设置 _inputName 属性"
                Me.Width = 1000
            End If

            MyBase.AddAttributesToRender(writer)

        End Sub

#End Region

#Region "未使用"
        'Private Sub MakeJavaScript()
        '    Dim csType As Type = Page.GetType()
        '    Dim csName As String = "Ilike_TextBox"
        '    Dim csScript As New StringBuilder

        '    With csScript
        '        .AppendLine("<script language='javascript' type='text/javascript'>  ")
        '        .AppendLine("</script>")
        '    End With
        '    Page.ClientScript.RegisterStartupScript(csType, csName, csScript.ToString)
        'End Sub

#End Region

    End Class

End Namespace