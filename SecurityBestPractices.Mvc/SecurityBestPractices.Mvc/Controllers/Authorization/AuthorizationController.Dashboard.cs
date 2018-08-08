using DevExpress.DashboardWeb;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SecurityBestPractices.Mvc.Controllers {
    public class PublicDashboardController : DevExpress.DashboardWeb.Mvc.DashboardController {
        static readonly DashboardConfigurator dashboardConfigurator;
        static PublicDashboardController() {
            var dashboardInMemoryStorage = new DashboardInMemoryStorage();
            dashboardInMemoryStorage.RegisterDashboard("publicId", XDocument.Load(HostingEnvironment.MapPath(@"/App_Data/PublicDashboard.xml")));

            dashboardConfigurator = new DashboardConfigurator();
            dashboardConfigurator.SetDashboardStorage(dashboardInMemoryStorage);
        }


        public PublicDashboardController() : base(dashboardConfigurator) {
        }
    }

    public partial class AuthorizationController : Controller {
        // GET: /Authorization/Dashboards/PublicDashboard
        [AllowAnonymous]
        public ActionResult PublicDashboard() {
            return View("Dashboards/PublicDashboard");
        }
        // GET: /Authorization/Dashboards/Dashboard
        [Authorize]
        public ActionResult Dashboard() {
            return View("Dashboards/Dashboard");
        }
    }
}