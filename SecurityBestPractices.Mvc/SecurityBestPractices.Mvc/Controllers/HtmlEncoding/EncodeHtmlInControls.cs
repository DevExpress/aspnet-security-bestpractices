using DevExpress.DashboardWeb;
using SecurityBestPractices.Mvc.Models;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SecurityBestPractices.Mvc.Controllers {
    public class HtmlEncodingController : Controller {
        // GET: /HtmlEncoding/EncodeHtmlInControls/
        public ActionResult EncodeHtmlInControls() {
            return View("EncodeHtmlInControls", EditFormItems.GetList());
        }

        public ActionResult EncodeHtmlInControlsPartial() {
            return PartialView("EncodeHtmlInControlsPartial", EditFormItems.GetList());
        }

        // GET: /HtmlEncoding/EncodeHtmlInTemplates/
        public ActionResult EncodeHtmlInTemplates() {
            return View("EncodeHtmlInTemplates", EditFormItems.GetList());
        }

        public ActionResult EncodeHtmlInTemplatesPartial() {
            return PartialView("EncodeHtmlInTemplatesPartial", EditFormItems.GetList());
        }
    }
}