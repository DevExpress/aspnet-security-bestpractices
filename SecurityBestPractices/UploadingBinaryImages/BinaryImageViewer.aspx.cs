using System;
using System.IO;

namespace SecurityBestPractices.UploadingBinaryImages {
    public partial class BinaryImageViewer : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            // here should be fetching from database

            // for  demonstration purposes we are getting "image" from file
            byte[] image = File.ReadAllBytes(Server.MapPath("~/App_Data/TestData/avatar.jpg"));
            // Now 'image' contains this harmfull html "<body onload=\'alert(1)\'></body>"


            Response.ClearHeaders();
            Response.ContentType = "image"; 
            //Response.ContentType = "image/jpeg"; // use this line to prevent vulnerability
            using (MemoryStream ms = new MemoryStream(image))
                ms.WriteTo(Response.OutputStream);
            Response.End();
        }
    }
}