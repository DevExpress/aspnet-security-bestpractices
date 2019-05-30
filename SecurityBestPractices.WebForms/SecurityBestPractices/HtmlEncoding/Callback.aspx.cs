using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.HtmlEncoding {
    public partial class Callback : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {

        }

        protected void Callback_Callback(object source, DevExpress.Web.CallbackEventArgs e) {
            // Not secure
            // e.Result = "<img src=1 onerror=alert('XSS') /> ";
            // CallbackControl.JSProperties["cpSomeInfo"] = "<video src=1 onerror=alert(document.cookie)>";

            e.Result = HttpUtility.HtmlEncode("<img src=1 onerror=alert('XSS') /> ");
            CallbackControl.JSProperties["cpSomeInfo"] = HttpUtility.HtmlEncode("<video src=1 onerror=alert(document.cookie)>");

        }
    }
}