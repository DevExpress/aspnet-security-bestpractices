using System;
using System.IO;
using DevExpress.Web;
using Image = System.Drawing.Image;

namespace SecurityBestPractices.UploadingFiles {
    public partial class UploadControlTempFileName : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void uploadControl_FilesUploadComplete(object sender, DevExpress.Web.FilesUploadCompleteEventArgs e) {
            if(uploadControl.UploadedFiles != null && uploadControl.UploadedFiles.Length > 0) {
                for(int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
                    UploadedFile file = uploadControl.UploadedFiles[i];
                    if(file.IsValid) {
                        string fileName = string.Format("{0}{1}", MapPath("~/UploadingFiles/Processing/"),
                            Path.GetRandomFileName() + ".tmp");
                        file.SaveAs(fileName, true);
                        // DoFileProcessing(fileName);
                    }
                }
            }
        }
    }
}