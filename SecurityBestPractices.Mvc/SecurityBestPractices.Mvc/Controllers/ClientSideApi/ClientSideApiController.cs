using DevExpress.DashboardWeb;
using SecurityBestPractices.Mvc.Models;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SecurityBestPractices.Mvc.Controllers {
    public class ClientSideApiController : Controller {
        // File Operations
        public ActionResult OfficeControlsFileOperations() {
            return View("OfficeControlsFileOperations");
        }
        public ActionResult OfficeControlsFileOperationsSpreadsheetPartial() {
            return PartialView("OfficeControlsFileOperationsSpreadsheetPartial");
        }
        public ActionResult OfficeControlsFileOperationsRichEditPartial() {
            return PartialView("OfficeControlsFileOperationsRichEditPartial");
        }

        // Spreadsheet Reading Mode Only
        public ActionResult SpreadsheetReadingModeOnly() {
            return View("SpreadsheetReadingModeOnly");
        }
        public ActionResult SpreadsheetReadingModeOnlyPartial() {
            return PartialView("SpreadsheetReadingModeOnlyPartial");
        }
    }
}