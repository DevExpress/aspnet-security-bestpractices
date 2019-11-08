using SecurityBestPractices.Mvc.Models;
using System.Web;
using System.Web.Mvc;

namespace SecurityBestPractices.Mvc.Controllers {
    [ValidateInput(false)]
    public class HtmlEncodingController : Controller {
        // GET: /HtmlEncoding/EncodeAjaxResponse/
        public ActionResult EncodeAjaxResponse() {
            return View("EncodeAjaxResponse");
        }
        public ActionResult EncodeAjaxResponseCallback() {
            // return Content("1</title><script>alert(1);</script><title>");  // Not secure

            return Content(HttpUtility.HtmlEncode("1</title><script>alert(1);</script><title>")); // Secure
        }

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