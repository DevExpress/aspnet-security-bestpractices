using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.UsingAntiForgeryToken {
    public partial class SvgInline : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // Obtain image data from the data source
            var svgImageWithJavaScriptCode = "<svg height=100 width=100><circle cx=50 cy=50 r=40 stroke=black stroke-width=2 fill=red /><script>alert('XXS')</script></svg>";
            svgInlineImageContainer.InnerHtml = svgImageWithJavaScriptCode;
        }
    }
}