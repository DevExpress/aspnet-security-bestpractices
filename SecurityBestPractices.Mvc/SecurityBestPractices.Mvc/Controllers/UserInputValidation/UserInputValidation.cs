using DevExpress.Web.Mvc;
using SecurityBestPractices.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SecurityBestPractices.Mvc.Controllers {
    [ValidateInput(false)]
    public class UserInputValidationController : Controller {
        // * General *
        // GET: /UserInputValidation/General/
        [HttpGet]
        public ActionResult General() {
            return View("General", new UserProfile());
        }

        // POST: /UserInputValidation/General/
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult General(UserProfile userProfile, string UserPassword) {
            if(ModelState.IsValid) {
                if(IsUserPasswordCorrect(UserPassword)) { // Custom Validation
                    DoSomethig(userProfile);
                } else {
                    ModelState.AddModelError("UserPassword", "Password is not correct!");
                }
            }
            return View("General", userProfile);
        }

        private bool IsUserPasswordCorrect(string value) {
            return value == "111"; // test 
        }

        private void DoSomethig(UserProfile userProfile) {
            
        }

        // * AdditionalDataAnnotationAttributes *
        // GET: /UserInputValidation/AdditionalDataAnnotationAttributes/
        [HttpGet]
        public ActionResult AdditionalDataAnnotationAttributes() {
            var model = new UserProfileEx();
            model.StartDate = DateTime.Now.AddYears(-500);
            model.EndDate = model.StartDate.AddDays(50);
            return View("AdditionalDataAnnotationAttributes", model);
        }

        // POST: /UserInputValidation/AdditionalDataAnnotationAttributes/
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult AdditionalDataAnnotationAttributes(UserProfileEx userProfileEx) {
            if(ModelState.IsValid) {
                // DoSomethig(userProfileEx);
            }
            return View("AdditionalDataAnnotationAttributes", userProfileEx);
        }

        // * ListEditors *
        // GET: /UserInputValidation/ListEditors/
        [HttpGet]
        public ActionResult ListEditors() {
            return View("ListEditors", ProductItems.GetAvailableForUserList()[0].Id);
        }

        // POST: /UserInputValidation/ListEditors/
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ListEditors(int productItemId) {
            if(ModelState.IsValid) {
                // Custom Validation: the selected combobox value should be in the list
                if(ProductItems.GetAvailableForUserList().FindIndex(i=>i.Id == productItemId) != -1) { 
                    // DoSomethig(productItemId);
                } else {
                    ModelState.AddModelError("", "Invalid input. Looks like a hacker attack.");
                }
            }
            return View("ListEditors", productItemId);
        }

        [NonAction]
        protected void ChangePassword(string newPassword) {
            
        }

        // * SvgInline *
        // GET: /UserInputValidation/SvgInline/
        [HttpGet]
        public ActionResult SvgInline() {
            return View("SvgInline");
        }

    }
}