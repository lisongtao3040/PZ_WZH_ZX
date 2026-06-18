using System;
using System.Configuration;

public partial class Inspection : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // 初期化処理があればここに記述
        }
    }
}
