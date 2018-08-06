using System;
using System.IO;
using DevExpress.Web;
using Image = System.Drawing.Image;

namespace SecurityBestPractices.UploadingFiles {
    public partial class UploadControl : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e) {
            // This code is not called be default because uploadControl.FileUploadMode = UploadControlFileUploadMode.BeforePageLoad
            // To set validation settings in run-time set uploadControl.FileUploadMode = UploadControlFileUploadMode.OnPageLoad or 
            // use uploadControl.Init envet

//            uploadControl.ValidationSettings.AllowedFileExtensions = new[] { ".jpg", ".png" };
        }

        protected void uploadControl_FilesUploadComplete(object sender, DevExpress.Web.FilesUploadCompleteEventArgs e) {
            if(uploadControl.UploadedFiles != null && uploadControl.UploadedFiles.Length > 0) {
                for(int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
                    UploadedFile file = uploadControl.UploadedFiles[i];
                    if(file.IsValid && file.FileName != "") {
                        using(var stream = file.FileContent) {
                            // In case additional checks are needed perform them here before saving the file
                            if(!IsValidImage(stream)) {
                                file.IsValid = false;
                                e.ErrorText = "Validation failed!";
                            } else {
                                string fileName = string.Format("{0}{1}", MapPath("~/UploadingFiles/Images/"), file.FileName);
                                file.SaveAs(fileName, true);
                            }
                        }
                    }
                }
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