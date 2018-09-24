using System;
using System.Linq;

namespace SecurityBestPractices.Authorization.Reports {
    public partial class ReportDesignerPage : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            var name = Request.QueryString["name"];
            var reportNames = ReportStorageWithAccessRules.GetEditableReportNamesForCurrentUser();
            if(reportNames.Contains(name))
                ASPxReportDesigner1.OpenReport(name);
            else
                Response.Redirect("~/Authorization/Reports/ReportViewerPage.aspx");
        }
    }
}