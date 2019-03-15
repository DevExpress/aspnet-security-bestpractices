using System;

namespace SecurityBestPractices.InformationExposure {
    public partial class ErrorMessage : System.Web.UI.Page {
        protected void UpdateButton_Click(object sender, EventArgs e) {
            try {
                // DoSomething()
                throw new InvalidOperationException("Some sensitive information");
            } catch(Exception ex) {
                UpdateStatusLabel.Visible = true;

                // UpdateStatusLabel.Text = ex.Message; // showing an Exception text - not a safe way

                // Safe way - show sensitive info free text

                if(ex is InvalidOperationException) {
                    UpdateStatusLabel.Text = "Some error occured...";
                } else {
                    UpdateStatusLabel.Text = "General error occured...";
                }

            }
        }
    }
}