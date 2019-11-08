using DevExpress.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using System.IO;
using System.Drawing;
using System;
using System.Web.Hosting;
using System.Linq;

namespace SecurityBestPractices.Mvc.Controllers {
    public class UploadingFilesController : Controller {
        #region Validation
        public ActionResult Validation() {
            return View("Validation");
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
                                    string fileName = string.Format("{0}{1}", HostingEnvironment.MapPath("~/Upload/Images/"), file.FileName);
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

        #endregion

        #region In Memory Processing
        public ActionResult InMemoryProcessing() {
            return View("InMemoryProcessing");
        }

        public ActionResult UploadInMemoryProcessingHandler([ModelBinder(typeof(UploadInMemoryProcessingBinder))]IEnumerable<UploadedFile> uploadControl) {
            return null;
        }

        public class UploadInMemoryProcessingBinder : DevExpressEditorsBinder {
            public UploadInMemoryProcessingBinder() {
                UploadControlBinderSettings.FilesUploadCompleteHandler = uploadControl_FilesUploadInMemoryProcessingComplete;
            }

            private void uploadControl_FilesUploadInMemoryProcessingComplete(object sender, FilesUploadCompleteEventArgs e) {
                var uploadedFiles = ((MVCxUploadControl)sender).UploadedFiles;
                if(uploadedFiles != null && uploadedFiles.Length > 0) {
                    for(int i = 0; i < uploadedFiles.Length; i++) {
                        UploadedFile file = uploadedFiles[i];
                        if(file.IsValid && file.FileName != "") {
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

            void DoProcessing(byte[] contenBytes) {
                System.Threading.Thread.Sleep(60000); // Some processing emulation
            }

            void DoProcessing(Stream stream) {
                System.Threading.Thread.Sleep(60000); // Some processing emulation
            }
        }
        #endregion

        #region Saving Temporary Files
        public ActionResult SavingTemporaryFiles() {
            return View("SavingTemporaryFiles");
        }

        public ActionResult UploadSavingTemporaryFilesHandler([ModelBinder(typeof(UploadSavingTemporaryFilesBinder))]IEnumerable<UploadedFile> uploadControl) {
            return null;
        }

        public class UploadSavingTemporaryFilesBinder : DevExpressEditorsBinder {
            public UploadSavingTemporaryFilesBinder() {
                UploadControlBinderSettings.FilesUploadCompleteHandler = uploadControl_FilesUploadSavingTemporaryFilesComplete;
            }

            private void uploadControl_FilesUploadSavingTemporaryFilesComplete(object sender, FilesUploadCompleteEventArgs e) {
                var uploadedFiles = ((MVCxUploadControl)sender).UploadedFiles;
                if(uploadedFiles != null && uploadedFiles.Length > 0) {
                    for(int i = 0; i < uploadedFiles.Length; i++) {
                        UploadedFile file = uploadedFiles[i];
                        if(file.IsValid && file.FileName != "") {
                            string fileName = string.Format("{0}{1}", HostingEnvironment.MapPath("~/Upload/Processing/"),
                                Path.GetRandomFileName() + ".tmp");
                            file.SaveAs(fileName, true);
                            // DoFileProcessing(fileName);
                        }
                    }
                }
            }
        }
        #endregion

        #region Limit Directory Size
        public ActionResult LimitDirectorySize() {
            return View("LimitDirectorySize");
        }

        public ActionResult UploadLimitDirectorySizeHandler([ModelBinder(typeof(UploadLimitDirectorySizeBinder))]IEnumerable<UploadedFile> uploadControl) {
            return null;
        }

        public class UploadLimitDirectorySizeBinder : DevExpressEditorsBinder {
            public UploadLimitDirectorySizeBinder() {
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
                                // Check if the uploaded file overflows the maximum directory size
                                const long MaxDirectorySize = 10000000; // bytes
                                long directorySize = GetDirectorySize(HostingEnvironment.MapPath("~/Upload/Images/"));
                                if(file.ContentLength + directorySize > MaxDirectorySize) {
                                    file.IsValid = false;
                                    e.ErrorText = "Maximum directory size exceeded!";
                                } else {
                                    string fileName = string.Format("{0}{1}", HostingEnvironment.MapPath("~/Upload/Images/"), file.FileName);
                                    file.SaveAs(fileName, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        static long GetDirectorySize(string rootFolder) {
            var files = Directory.EnumerateFiles(rootFolder);
            var size = (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();

            var subDirectories = Directory.EnumerateDirectories(rootFolder);
            var subDirectoriesSize = (from directory in subDirectories select GetDirectorySize(directory)).Sum();

            return size + subDirectoriesSize;
        }

        #endregion

    }
}