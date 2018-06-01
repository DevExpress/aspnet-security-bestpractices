using System;
using System.IO;

namespace SecurityBestPractices.UploadingBinaryImages {
    public partial class BinaryImage : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {

        }

        protected void ASPxButton1_Click(object sender, EventArgs e) {
            byte[] contentBytes = ASPxBinaryImage1.ContentBytes;
            // Here contentBytes should be saved to a database
            // We will save it to a file for demonstration purposes
            string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
            File.WriteAllBytes(fileName, contentBytes);
        }
    }
}