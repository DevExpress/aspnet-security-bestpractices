using System;
using System.IO;

namespace SecurityBestPractices.UploadingBinaryImages {
    public partial class BinaryImageViewer : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // Here an image should be obtained from a database
            // We read it from a file for demostration purposes
            byte[] image = File.ReadAllBytes(Server.MapPath("~/App_Data/UploadedData/avatar.jpg"));
            // Now 'image' contains harmfull html: "<body onload=\'alert(1)\'></body>"

            Response.ClearHeaders();
            //Response.ContentType = "image"; // It is not secure
            Response.ContentType = "image/jpeg"; // Specify content-type to prevent the vulnerability
            Response.Headers.Add("X-Content-Type-Options", "nosniff"); // Additional valitation 

            using(MemoryStream ms = new MemoryStream(image))
                ms.WriteTo(Response.OutputStream);
            Response.End();
        }
    }
}