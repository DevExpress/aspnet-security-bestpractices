using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices {
    public partial class Login : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
        }

        protected void ASPxButton1_Click(object sender, EventArgs e) {
            FormsAuthentication.RedirectFromLoginPage(UserName.Text, true);
        }
    }
}