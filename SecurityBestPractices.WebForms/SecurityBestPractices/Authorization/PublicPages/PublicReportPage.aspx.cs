using System;

namespace SecurityBestPractices.Authorization.PublicPages {
    public partial class PublicReportPage : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            documentViewer.OpenReport("Public Report");
        }
    }
}