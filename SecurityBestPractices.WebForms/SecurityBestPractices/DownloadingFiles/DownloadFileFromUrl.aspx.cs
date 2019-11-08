using System;
using System.IO;
using System.Net;
using DevExpress.Web;
using Image = System.Drawing.Image;

namespace SecurityBestPractices.DownloadingFiles {
    public partial class DownloadFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) {
        }

        protected void Download_Click(object sender, EventArgs e) {
            string url = edUrl.Text;
            if(string.IsNullOrEmpty(url)) return;

            // Not secure
            //using(var webClient = new WebClient()) {
            //    byte[] data = webClient.DownloadData(url);
            //    BinaryImage.ContentBytes = data;
            //}

            // or
            // Not secure
            //WebRequest request = WebRequest.Create(url);
            //using(WebResponse response = request.GetResponse()) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); // Secure
            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse()) { // Secure
                using(Stream stream = response.GetResponseStream()) {
                    using(BinaryReader reader = new BinaryReader(stream))
                        BinaryImage.ContentBytes = ReadAllBytes(reader);
                }
            }
        }
        protected void DownloadConfedentialImage_Click(object sender, EventArgs e) {
            edUrl.Text = Server.MapPath(@"~\App_Data\ConfidentialImages\ConfedentialImageFile.jpg");
            Download_Click(sender, e);
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