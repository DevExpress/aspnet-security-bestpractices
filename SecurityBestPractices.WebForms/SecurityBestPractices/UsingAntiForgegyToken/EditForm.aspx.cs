using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.UsingAntiForgegyToken {
    public partial class EditForm : System.Web.UI.Page {
        protected void UpdateButton_Click(object sender, EventArgs e) {
            // if (ChangeUserEmail(EmailTextBox.Text))
            UpdateStatusLabel.Text = "Email is updated.";
        }
    }
}