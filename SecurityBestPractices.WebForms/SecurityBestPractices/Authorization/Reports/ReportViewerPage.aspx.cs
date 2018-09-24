using System;
using System.Linq;
using System.Web;
using DevExpress.Utils.Extensions;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer;

namespace SecurityBestPractices.Authorization.Reports
{
    public partial class ReportViewerPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                ReportNames.Items.AddRange(ReportStorageWithAccessRules.GetViewableReportDisplayNamesForCurrentUser().ToArray());
                if(ReportNames.Items.Count > 0)
                    ReportNames.SelectedIndex = 0;

            }

            var selectedItemText = ReportNames.SelectedItem.Text;
            documentViewer.OpenReport(selectedItemText);

            var editableReportNames = ReportStorageWithAccessRules.GetEditableReportNamesForCurrentUser();
            EditButton.Enabled = editableReportNames.Contains(selectedItemText);
        }

        protected void ReportName_OnSelectedIndexChanged(object sender, EventArgs e) {
            documentViewer.OpenReport(ReportNames.SelectedItem.Text);
        }

        protected void EditButton_OnClick(object sender, EventArgs e) {
            Response.Redirect("~/Authorization/Reports/ReportDesignerPage.aspx?name="+ ReportNames.SelectedItem.Text);
        }
    }
}