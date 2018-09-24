using System.Web.Mvc;

namespace SecurityBestPractices.Mvc.Controllers {
    [Authorize]
    public partial class AuthorizationController : Controller {
        // GET: /Authorization/QueryBuilder/
        [AllowAnonymous]
        public ActionResult QueryBuilder() {
            return View("QueryBuilder/Index");
        }
    }
}