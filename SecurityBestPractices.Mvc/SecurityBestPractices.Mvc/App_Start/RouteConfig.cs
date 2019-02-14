using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.DashboardWeb.Mvc;

namespace SecurityBestPractices.Mvc {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.MapDashboardRoute(); // dashboards nessesary initialization
            routes.MapDashboardRoute("dashboard", "PublicDashboard", new[] { "SecurityBestPractices.Mvc.Controllers" });
            routes.MapDashboardRoute("dashboardAFT", "DashboardWithAntiForgegyToken", new[] { "SecurityBestPractices.Mvc.Controllers" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            routes.MapRoute(
                name: "Default", // Route name
                // url: "{controller}/{action}/{id}", // URL with parameters 
                url: "{controller}/{action}", // URL with parameters
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                name: "ReportsAuthorization", // Route name
                url: "{controller}/Reports/{action}/{name}", // URL with parameters
                defaults: new { controller = "Authorization", /*action = "Index", */name = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                name: "DashboardsAuthorization", // Route name
                url: "{controller}/Dashboards/{action}/{name}", // URL with parameters
                defaults: new { controller = "Authorization", /*action = "Index", */name = UrlParameter.Optional } // Parameter defaults
            );
        }
    }
}