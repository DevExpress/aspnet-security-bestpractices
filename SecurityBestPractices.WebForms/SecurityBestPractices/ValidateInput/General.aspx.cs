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
        }

        protected void spinEdit_CustomValidation(object sender, ValidationEventArgs e) {
            if((sender as ASPxSpinEdit).Number == 5) {
                e.IsValid = false;
                e.ErrorText = "5 is not acceptable";
            }
        }

        protected void UpdateButton_Click(object sender, EventArgs e) {
            // ASPxEdit.ValidateEditorsInContainer(Page); // Not required if data is submitted using ASPxButton
            if(ASPxEdit.AreEditorsValid(Page)) {
                UpdateStatusLabel.Text = string.Format("Submitted value: {0}", spinEdit.Value);
            } else {
                UpdateStatusLabel.Text = "Editors are not valid!";
            }
        }

        protected void StandardButton_Click(object sender, EventArgs e) {
            ASPxEdit.ValidateEditorsInContainer(Page); // When data is submitted using a Standard button
            if(ASPxEdit.AreEditorsValid(Page)) {
                UpdateStatusLabel.Text = string.Format("Submitted value: {0}", spinEdit.Value);
            } else {
                UpdateStatusLabel.Text = "Editors are not valid!";
            }

        }
    }
}