using System;
using System.Web.UI;

namespace SecurityBestPractices.UsingAntiForgeryToken {
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