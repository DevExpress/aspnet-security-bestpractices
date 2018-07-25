using SecurityBestPractices.Mvc.Models;
using System.Web.Mvc;

namespace SecurityBestPractices.Mvc.Controllers {
    public class UsingAntiForgegyTokenController : Controller {
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

    }
}