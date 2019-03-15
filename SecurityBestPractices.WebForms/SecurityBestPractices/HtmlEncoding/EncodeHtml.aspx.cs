using DevExpress.Web;
using System;

namespace SecurityBestPractices.HtmlEncoding {
    public partial class EncodeHtml : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // Not safe content stored in Database, for example: <img src=1 onerror=alert('XSS') />
            // ((GridViewDataTextColumn)GridView.Columns["ProductName"]).PropertiesEdit.EncodeHtml = false;
        }
    }
}