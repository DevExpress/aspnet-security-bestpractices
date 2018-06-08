using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.UsingAntiForgegyToken {
    public partial class MasterPageWithAntiForgeryToken : System.Web.UI.MasterPage {
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            Page.PreLoad += OnPreLoad;
        }
        protected void OnPreLoad(object sender, EventArgs e) {
            if (IsPostBack)
                System.Web.Helpers.AntiForgery.Validate();
        }
    }
}