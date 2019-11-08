using DevExpress.Web;
using System.Collections.Generic;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using System.IO;
using System.Drawing;
using System;
using System.Web.Hosting;
using System.Net;

namespace SecurityBestPractices.Mvc.Controllers {
    public class DownloadingFilesController : Controller {

        [HttpGet]
        public ActionResult DownloadFileFromUrl() {
            string url = "https://demos.devexpress.com/ASPxImageAndDataNavigationDemos/Content/Images/widescreen/woman-using-laptop.jpg";
            return View("DownloadFileFromUrl", GetBinaryImageBytes(url));
        }

        [HttpPost]
        public ActionResult DownloadPublicFileFromUrl() {
            return RedirectToAction("DownloadFileFromUrl");
        }
        [HttpPost]
        public ActionResult DownloadConfedentialFileFromUrl() {
            string url = HostingEnvironment.MapPath(@"~\App_Data\ConfidentialImages\ConfedentialImageFile.jpg");
            return View("DownloadFileFromUrl", GetBinaryImageBytes(url));
        }

        byte[] GetBinaryImageBytes(string url) {
            // Not secure
            //using(var webClient = new WebClient()) {
            //    byte[] data = webClient.DownloadData(url);
            //    return data;
            //}
            
            // or
            // Not secure
            //WebRequest request = WebRequest.Create(url);
            //using(WebResponse response = request.GetResponse()) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // Secure
            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse()) { // Secure
                using(Stream stream = response.GetResponseStream()) {
                    using(BinaryReader reader = new BinaryReader(stream))
                        return ReadAllBytes(reader);
                }
            }
        }

        public static byte[] ReadAllBytes(BinaryReader reader) {
            const int bufferSize = 4096;
            using(var ms = new MemoryStream()) {
                byte[] buffer = new byte[bufferSize];
                int count;
                while((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                return ms.ToArray();
            }

        }
    }
}