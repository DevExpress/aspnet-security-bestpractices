using System;
using System.Web;
using System.Xml.Linq;

namespace SecurityBestPractices.Authorization.PublicPages {
    public partial class PublicDashboardPage : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            PublicDashboard.OpenDashboard(XDocument.Load(HttpContext.Current.Server.MapPath(@"~/App_Data/PublicDashboard.xml")));
        }
    }
}