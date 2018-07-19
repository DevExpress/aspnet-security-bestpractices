using System;

namespace SecurityBestPractices
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e) {
            if(Request.Url.AbsolutePath.ToLower() == "/login.aspx")
                LoginStatus.Visible = false;
        }
    }
}