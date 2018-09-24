using System.Linq;
using System.Web.Mvc;
using SecurityBestPractices.Mvc.Models;
using SecurityBestPractices.Mvc.Services.Reports;

namespace SecurityBestPractices.Mvc.Controllers {
    [Authorize]
    public partial class AuthorizationController : Controller {
        // GET: /Authorization/Reports/PublicReport
        [AllowAnonymous]
        public ActionResult PublicReport() {
            return View("Reports/PublicReport");
        }

        // GET: /Authorization/Reports/ReportViewer
        public ActionResult ReportViewer() {
            return View("Reports/ReportViewer", 
                new ReportNameModel() { ReportName = 
                ReportStorageWithAccessRules.GetViewableReportDisplayNamesForCurrentUser().FirstOrDefault()});
        }
        // POST: /Authorization/Reports/ReportViewer
        [HttpPost]
        public ActionResult ReportViewer(ReportNameModel model) {
            if(!ModelState.IsValid)
                return RedirectToAction("Index", "Home");
            return View("Reports/ReportViewer", model);
        }

        // GET: /Authorization/Reports/ReportDesigner/<Report Url>
        public ActionResult ReportDesigner(string name) {
            var reportNames = ReportStorageWithAccessRules.GetEditableReportNamesForCurrentUser();
            if(reportNames.Contains(name))
                return View("Reports/ReportDesigner", new ReportNameModel() { ReportName = name });
            else
                return RedirectToAction("ReportViewer", "Authorization");
        }
    }
}