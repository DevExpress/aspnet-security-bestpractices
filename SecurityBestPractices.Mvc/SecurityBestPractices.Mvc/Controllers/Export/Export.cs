using DevExpress.Utils;
using DevExpress.XtraPrinting;
using SecurityBestPractices.Mvc.Models;
using System.Web;
using System.Web.Mvc;

namespace SecurityBestPractices.Mvc.Controllers {
    [ValidateInput(false)]
    public class ExportController : Controller {
        // GET: /Export/ExportToCsv/
        [HttpGet]
        public ActionResult ExportToCsv() {
            return View("ExportToCsv", EditFormItems.GetList());
        }

        public ActionResult ExportToCsvPartial() {
            return PartialView("ExportToCsvPartial", EditFormItems.GetList());
        }
    }
}