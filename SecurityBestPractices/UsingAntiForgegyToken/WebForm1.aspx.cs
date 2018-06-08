using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.UsingAntiForgegyToken {
    public partial class WebForm1 : System.Web.UI.Page {
        protected void Page_PreLoad(object sender, EventArgs e) {
            if (IsPostBack)
                AntiForgery.Validate();
        }

        protected void Button1_Click(object sender, EventArgs e) {
            Button1.Text = "Deleted!";
        }
    }
}