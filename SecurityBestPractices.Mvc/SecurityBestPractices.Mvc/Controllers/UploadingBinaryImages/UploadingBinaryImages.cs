using DevExpress.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using System.IO;
using System.Drawing;
using System;
using System.Web.Hosting;

namespace SecurityBestPractices.Mvc.Controllers {
    public class UploadingBinaryImagesController : Controller {

        #region Usage UploadControl
        public ActionResult UploadControl() {
            return View("UploadControl");
        }
        public ActionResult UploadControlHandler([ModelBinder(typeof(UploadControlBinder))]IEnumerable<UploadedFile> uploadControl) {
            return null;
        }
        public class UploadControlBinder : DevExpressEditorsBinder {
            public UploadControlBinder() {
                UploadControlBinderSettings.ValidationSettings.AllowedFileExtensions = new[] { ".jpg", ".jpeg" };
                UploadControlBinderSettings.FileUploadCompleteHandler = uploadControl_FileUploadComplete;
            }

            private void uploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
                if(!e.UploadedFile.IsValid) return;

                // Here contentBytes should be saved to a database
                using(var stream = e.UploadedFile.FileContent) {
                    if(!IsValidImage(stream)) {
                        e.ErrorText = "Invalid image.";
                        e.IsValid = false;
                    } else {
                        // We save it to a file for demonstration purposes
                        string fileName = HostingEnvironment.MapPath("~/App_Data/UploadedData/avatar.jpg");
                        e.UploadedFile.SaveAs(fileName, true);
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

        #region Binary Image Viewer
        [HttpGet]
        public ActionResult BinaryImageViewer() {
            // Here an image should be obtained from a database
            // We read it from a file for demostration purposes
            byte[] image = System.IO.File.ReadAllBytes(Server.MapPath("~/App_Data/UploadedData/avatar.jpg"));
            // Now 'image' contains harmfull html: "<body onload=\'alert(1)\'></body>"

            Response.ClearHeaders();

            // Response.ContentType = "image"; // It is not secure
            Response.ContentType = "image/jpeg"; // Specify content-type to prevent the vulnerability
            Response.Headers.Add("X-Content-Type-Options", "nosniff"); // Additional valitation 

            using(MemoryStream ms = new MemoryStream(image))
                ms.WriteTo(Response.OutputStream);

            return new FileContentResult(image, Response.ContentType);
        }
        #endregion

        #region Usage BinaryImage
        public ActionResult BinaryImage() {
            return View("BinaryImage", GetBinaryImageBytes());
        }

        byte[] GetBinaryImageBytes() {
            string fileName = HostingEnvironment.MapPath("~/App_Data/UploadedData/avatar.jpg");
            if(!System.IO.File.Exists(fileName)) return null;

            using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                byte[] imageInByteArray = new byte[stream.Length];
                stream.Read(imageInByteArray, 0, (int)stream.Length);
                if(imageInByteArray.Length == 0)
                    imageInByteArray = null;
                return imageInByteArray;
            }
        }

        public ActionResult UpdateBinaryImagePartial() {
            return BinaryImageEditExtension.GetCallbackResult();
        }

        [HttpPost]
        public ActionResult SaveBinaryImage() {
            byte[] contentBytes = BinaryImageEditExtension.GetValue<byte[]>("BinaryImage"); // Uploaded file content are valided by ASPxBinaryImage

            // Here contentBytes should be saved to a database
            // We will save it to a file for demonstration purposes
            string fileName = HostingEnvironment.MapPath("~/App_Data/UploadedData/avatar.jpg");
            System.IO.File.WriteAllBytes(fileName, contentBytes != null ? contentBytes : new byte[0] { });

            return RedirectToAction("BinaryImage");
        }
        #endregion
    }
}