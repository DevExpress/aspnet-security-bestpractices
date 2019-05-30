using System;

namespace SecurityBestPractices.InformationExposure {
    public partial class ErrorMessage : System.Web.UI.Page {
        protected void UpdateButton_Click(object sender, EventArgs e) {
            try {
                // DoSomething()
                throw new InvalidOperationException("Some sensitive information");
            } catch(Exception ex) {
                UpdateStatusLabel.Visible = true;

                // Unsafe approach - showing an Exception text
                // UpdateStatusLabel.Text = ex.Message; 

                // Safe approach - showing text without sensitive information
                if(ex is InvalidOperationException) {
                    UpdateStatusLabel.Text = "Some error occured...";
                } else {
                    UpdateStatusLabel.Text = "General error occured...";
                }

            }
        }
    }
}