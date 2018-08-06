using DevExpress.Web;
using SecurityBestPractices.Mvc.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using DevExpress.Web.Mvc;
using System.IO;
using System.Drawing;
using System;

namespace SecurityBestPractices.Mvc.Controllers {
    public class UploadingFilesController : Controller {
        public ActionResult UploadValidation() {
            return View("UploadValidation");
        }
        public ActionResult UploadValidationHandler([ModelBinder(typeof(UploadValidationBinder))]IEnumerable<UploadedFile> uploadControl) {
            return null;
        }

        public class UploadValidationBinder : DevExpressEditorsBinder {
            public UploadValidationBinder() {
                UploadControlBinderSettings.ValidationSettings.AllowedFileExtensions = new[] { ".jpg", ".png" };
                UploadControlBinderSettings.FilesUploadCompleteHandler = uploadControl_FilesUploadComplete;
            }

            private void uploadControl_FilesUploadComplete(object sender, FilesUploadCompleteEventArgs e) {
                var uploadedFiles = ((MVCxUploadControl)sender).UploadedFiles;
                if(uploadedFiles != null && uploadedFiles.Length > 0) {
                    for(int i = 0; i < uploadedFiles.Length; i++) {
                        UploadedFile file = uploadedFiles[i];
                        if(file.IsValid && file.FileName != "") {
                            using(var stream = file.FileContent) {
                                // In case additional checks are needed perform them here before saving the file
                                if(!IsValidImage(stream)) {
                                    file.IsValid = false;
                                    e.ErrorText = "Validation failed!";
                                } else {
                                    string fileName = string.Format("{0}{1}", System.Web.HttpContext.Current.Server.MapPath("~/Upload/Images/"), file.FileName);
                                    file.SaveAs(fileName, true);
                                }
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
            } catch(Exception) {
                return false;
            }
        }
    }
}