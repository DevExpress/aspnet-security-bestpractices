using System;
using System.IO;
using System.Linq;
using DevExpress.Web;
using Image = System.Drawing.Image;

namespace SecurityBestPractices.UploadingFiles {
    public partial class LimitDirectorySize : System.Web.UI.Page {
        protected void uploadControl_FilesUploadComplete(object sender, DevExpress.Web.FilesUploadCompleteEventArgs e) {
            if(uploadControl.UploadedFiles != null && uploadControl.UploadedFiles.Length > 0) {
                for(int i = 0; i < uploadControl.UploadedFiles.Length; i++) {
                    UploadedFile file = uploadControl.UploadedFiles[i];
                    if(file.IsValid && file.FileName != "") {
                        // Check if the uploaded file overflows the maximum directory size
                        const long MaxDirectorySize = 10000000; // bytes
                        long directorySize = GetDirectorySize(MapPath("~/UploadingFiles/Images/"));
                        if(file.ContentLength + directorySize > MaxDirectorySize) {
                            file.IsValid = false;
                            e.ErrorText = "Maximum directory size exceeded!";
                        } else {
                            string fileName = string.Format("{0}{1}", MapPath("~/UploadingFiles/Images/"), file.FileName);
                            file.SaveAs(fileName, true);
                        }
                    }
                }
            }
        }

        long GetDirectorySize(string rootFolder) {
            var files = Directory.EnumerateFiles(rootFolder);
            var size = (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();

            var subDirectories = Directory.EnumerateDirectories(rootFolder);
            var subDirectoriesSize = (from directory in subDirectories select GetDirectorySize(directory)).Sum();

            return size + subDirectoriesSize;
        }
    }
}