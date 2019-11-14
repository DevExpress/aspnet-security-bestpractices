using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.UsingAntiForgeryToken {
    public partial class GeneralEditForm : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // Init Value 
            if(!IsPostBack) {
                someEdit.Value = 55; // > MaxLength
                spinEdit.Value = 10000; // from DataSource; out of the Min..Max range
            }
        }

        protected void StandardButton_Click(object sender, EventArgs e) {
            ASPxEdit.ValidateEditorsInContainer(Page); // Required only for a standard button

            if(ASPxEdit.AreEditorsValid(Page)) {
                UpdateStatusLabel.Text = string.Format("Submitted value: {0}", spinEdit.Value);
            } else {
                UpdateStatusLabel.Text = "Editors are not valid!";
            }
        }
    }
}