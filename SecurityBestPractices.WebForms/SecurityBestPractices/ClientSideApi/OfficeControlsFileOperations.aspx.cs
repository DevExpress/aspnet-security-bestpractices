using DevExpress.Web.ASPxSpreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.ClientSideApi {
    public partial class OfficeControlsFileOperations : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            spreadsheet.Open(Server.MapPath(@"~\app_data\WorkDirectory\test.xlsx"));

            // Hide the File tab
            spreadsheet.CreateDefaultRibbonTabs(true);
            spreadsheet.RibbonTabs.RemoveAt(0);
            spreadsheet.ActiveTabIndex = 0;

            richEdit.CreateDefaultRibbonTabs(true);
            richEdit.RibbonTabs.RemoveAt(0);
            richEdit.ActiveTabIndex = 0;
        }
    }
}