using System;
using System.IO;
using Image = System.Drawing.Image;

namespace SecurityBestPractices.UploadingBinaryImages {
    public partial class UploadControl : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
        }

        protected void ASPxUploadControl1_FileUploadComplete(object sender,
            DevExpress.Web.FileUploadCompleteEventArgs e) {
            // Here contentBytes should be saved to a database
            using(var stream = e.UploadedFile.FileContent) {
                //if(!IsValidImage(stream)) return; // this string should be uncommented to prevent vulnerability

                // We save it to a file for demonstration purposes
                string fileName = Server.MapPath("~/App_Data/UploadedData/avatar.jpg");
                e.UploadedFile.SaveAs(fileName, true);
            }
        }

        static bool IsValidImage(Stream stream) {
            try {
                using(var image = Image.FromStream(stream)) {
                    return true;
                }
            }
            catch(Exception) {
                return false;
            }
        }
    }
}