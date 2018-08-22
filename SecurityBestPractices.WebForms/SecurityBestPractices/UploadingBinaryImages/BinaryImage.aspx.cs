using System;
using System.IO;

namespace SecurityBestPractices.UploadingBinaryImages {
    public partial class BinaryImage : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            if(!IsPostBack) {
                // contentBytes can be loaded from a database. We use a file for demonstration purposes
                string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
                if(File.Exists(fileName)) {
                    byte[] contentBytes = File.ReadAllBytes(fileName);
                    if(contentBytes.Length > 0)
                        ASPxBinaryImage1.ContentBytes = contentBytes;
                }
            }

        }

        protected void ASPxButton1_Click(object sender, EventArgs e) {
            byte[] contentBytes = ASPxBinaryImage1.ContentBytes; // Uploaded file contents are valided by ASPxBinaryImage

            // Here contentBytes should be saved to a database
            // We save it to a file for demonstration purposes
            string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
            File.WriteAllBytes(fileName, contentBytes != null ? contentBytes : new byte[0] { });
        }
    }
}