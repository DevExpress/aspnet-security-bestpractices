using DevExpress.DashboardWeb;
using SecurityBestPractices.Mvc.Models;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SecurityBestPractices.Mvc.Controllers {
    [ValidateInput(false)]
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

        // GET: /HtmlEncoding/General/
        [HttpGet]
        public ActionResult General() {
            return View("General", new EditFormItem() { Id = 1, ProductName = "\"<b>'test'</b>" });
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult General(EditFormItem item) {
            if(ModelState.IsValid) {
                // DoSomething()
            } 
            return View(item);
        }

    }
}