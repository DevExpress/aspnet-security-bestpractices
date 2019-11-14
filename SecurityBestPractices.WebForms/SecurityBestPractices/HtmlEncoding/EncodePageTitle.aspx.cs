using DevExpress.Web;
using System;
using System.Data;
using System.Web;

namespace SecurityBestPractices.HtmlEncoding {
    public partial class EncodePageTitle : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            var ds = SqlDataSource1.Select(new System.Web.UI.DataSourceSelectArguments()) as DataView;
            var value = ds[0]["ProductName"]; // value from DB = "</title><script>alert('XSS')</script>";

            //Title = "Product: " + value.ToString(); // Not secure
            Title = "Product: " + HttpUtility.HtmlEncode(value).ToString(); // Secure
        }
    }
}