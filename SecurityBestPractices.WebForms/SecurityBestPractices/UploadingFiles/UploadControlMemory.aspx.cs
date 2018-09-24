using System;
using System.IO;
using DevExpress.Web;

namespace SecurityBestPractices.UploadingFiles {
    public partial class UploadControlMemory : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {

        }

        void DoProcessing(byte[] contenBytes) {
            System.Threading.Thread.Sleep(60000); // Delay to emulate processig time
        }

        void DoProcessing(Stream stream) {
            System.Threading.Thread.Sleep(60000); // Delay to emulate processig time
        }

        protected void uploadControl_FilesUploadComplete(object sender, DevExpress.Web.FilesUploadCompleteEventArgs e) {
            for(int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
                UploadedFile file = uploadControl.UploadedFiles[i];
                if(file.IsValid) {
                    // Bad approach - possible Denial of Service
                    // DoProcessing(file.FileBytes);

                    // Good approach - use stream for large files
                    using(var stream = file.FileContent) {
                        DoProcessing(stream);
                    }
                }
            }
        }
    }
}