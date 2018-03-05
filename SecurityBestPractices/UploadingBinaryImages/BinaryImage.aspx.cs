using System;
using System.IO;

namespace SecurityBestPractices.UploadingBinaryImages {
    public partial class BinaryImage : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {

        }

        protected void ASPxButton1_Click(object sender, EventArgs e) {
            byte[] contentBytes = ASPxBinaryImage1.ContentBytes;
            // here contentBytes should be saved to database

            // for demonstration purposes we will save it to file
            string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
            File.WriteAllBytes(fileName, contentBytes);
        }
    }
}