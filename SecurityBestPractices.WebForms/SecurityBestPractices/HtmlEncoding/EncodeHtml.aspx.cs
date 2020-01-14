using DevExpress.Web;
using System;
using System.Web;

namespace SecurityBestPractices.HtmlEncoding {
    public partial class EncodeHtml : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // Unsafe content stored in the database, for example: <img src=1 onerror=alert('XSS') />
            // ((GridViewDataTextColumn)GridView.Columns["ProductName"]).PropertiesEdit.EncodeHtml = false;
        }

        protected void GridView_HeaderFilterFillItems(object sender, ASPxGridViewHeaderFilterEventArgs e) {
            if(e.Column.FieldName == "ProductName") {
                e.Values.Clear();
                // Adding custom values from unsafe data source

                // Safe approach - Display Text is encoded
                e.AddValue(HttpUtility.HtmlEncode("<b>T</b>est <img src=1 onerror=alert('XSS') />"), "1");

                // Unsafe approach - Display Text is not encoded
                //e.AddValue("<b>T</b>est <img src=1 onerror=alert('XSS') />", "1");
            }
        }
    }
}