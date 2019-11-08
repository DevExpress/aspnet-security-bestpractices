using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.HtmlEncoding {
    public partial class General : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(IsPostBack) {
                // *** Input ***
                // Not secure
                //SearchResultLiteral.Text = string.Format("Your search - {0} - did not match any documents.", SearchBox.Text);

                // Secure
                SearchResultLiteral.Text = string.Format("Your search - {0} - did not match any documents.", HttpUtility.HtmlEncode(SearchBox.Text));
                // or ASPxLabel
                SearchResultLabel.Text = string.Format("Your search - {0} - did not match any documents.", SearchBox.Text);
            }

            // *** URL ***
            // Not secure
            //if(Request.QueryString["returnUrl"] != null)
            //    urlLink.HRef = Request.QueryString["returnUrl"].ToString();

            // Secure
            if(Request.QueryString["returnUrl"] != null)
                urlLink.HRef = HttpUtility.HtmlEncode(Request.QueryString["returnUrl"].ToString());
                //urlLink.HRef = (Request.QueryString["returnUrl"].ToString()); // Not secure
        }
    }
}