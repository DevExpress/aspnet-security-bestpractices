using DevExpress.DashboardWeb;
using SecurityBestPractices.Mvc.Models;
using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SecurityBestPractices.Mvc.Controllers {
    public class UsingAntiForgegyTokenController : Controller {
        /* EditForm */

        // GET: /UsingAntiForgegyTokenController/EditForm/
        public ActionResult EditForm() {
            return View("EditForm", EditFormItems.GetList());
        }

        public ActionResult EditFormPartial() {
            return PartialView("EditFormPartial", EditFormItems.GetList());
        }

        [Authorize]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditFormUpdatePartial(EditFormItem item) {
            if(ModelState.IsValid)
                EditFormItems.Update(item);
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return EditFormPartial();
        }

        [Authorize]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditFormDeletePartial(int id = -1) {
            if(id >= 0)
                EditFormItems.Delete(id);

            return EditFormPartial();
        }


        /* Dashboard View */

        // GET: /UsingAntiForgegyTokenController/EditDashboard/
        [Authorize]
        public ActionResult EditDashboard() {
            return View("EditDashboard");
        }
    }

    /* Dashboard Helper */

    // DashboardValidateAntiForgeryToken Attribute for Dashboard Controlles
    public sealed class DashboardValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter {
        public void OnAuthorization(AuthorizationContext filterContext) {
            if(filterContext == null) {
                throw new ArgumentNullException(nameof(filterContext));
            }

            HttpContextBase httpContext = filterContext.HttpContext;
            HttpCookie cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            AntiForgery.Validate(cookie?.Value, httpContext.Request.Headers["__RequestVerificationToken"]);
        }
    }

    /* Dashboard with ValidateAntiForgeryToken */
    [DashboardValidateAntiForgeryToken]
    public class DashboardWithAntiForgegyTokenController : DevExpress.DashboardWeb.Mvc.DashboardController {
        static readonly DashboardConfigurator dashboardConfigurator;
        static DashboardWithAntiForgegyTokenController() {
            // sample data
            var dashboardInMemoryStorage = new DashboardInMemoryStorage();
            dashboardInMemoryStorage.RegisterDashboard("editId", XDocument.Load(HostingEnvironment.MapPath(@"~/App_Data/PublicDashboard.xml")));

            dashboardConfigurator = new DashboardConfigurator();
            dashboardConfigurator.SetDashboardStorage(dashboardInMemoryStorage);
        }

        public DashboardWithAntiForgegyTokenController() : base(dashboardConfigurator) {
        }
    }
}