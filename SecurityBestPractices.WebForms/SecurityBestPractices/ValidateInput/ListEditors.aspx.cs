using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecurityBestPractices.UsingAntiForgeryToken {
    public partial class ListEditors : System.Web.UI.Page {
        protected void Page_Init(object sender, EventArgs e) {
            // Fill Items in code on Page_Init when ViewState disabled
            ComboBoxInStrictMode.DataSource = SqlDataSource1;
            ComboBoxInStrictMode.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e) {
            // Init Value 
            if(!IsPostBack) {
                ComboBoxInStrictMode.Value = 10000; // Value from DataSource - out of List items
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e) {
            if(ASPxEdit.AreEditorsValid(Page)) {
                UpdateStatusLabel.Text = string.Format("Submitted value: {0}", ComboBoxInStrictMode.Value);
            } else {
                UpdateStatusLabel.Text = "Editors are not valid!";
            }
        }
    }
}