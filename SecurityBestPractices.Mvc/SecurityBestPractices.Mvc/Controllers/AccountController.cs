using System;
using System.Web;
using System.Web.Mvc;
using SecurityBestPractices.Mvc.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace SecurityBestPractices.Mvc.Controllers {
    [Authorize]
    public class AccountController : Controller {
        private CustomUserManager CustomUserManager { get; set; }

        public AccountController() {
            CustomUserManager = new CustomUserManager();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel() { UserName = "Admin" }); // Default test value
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            if(ModelState.IsValid) {
                var user = await CustomUserManager.FindAsync(model.UserName, null);
                if(user != null) {
                    await SignInAsync(user, true);
                    return RedirectToLocal(returnUrl);
                } else {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string returnUrl) {
            AuthenticationManager.SignOut();
            //return RedirectToLocal(Request.UrlReferrer.PathAndQuery);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing) {
            if(disposing && CustomUserManager != null) {
                CustomUserManager.Dispose();
                CustomUserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent) {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            var identity = await CustomUserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result) {
            foreach(var error in result.Errors) {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl) {
            if(Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }

    // UserManager for testing purposes
    public class CustomUserManager : UserManager<ApplicationUser> {
        public CustomUserManager()
            : base(new CustomUserStore<ApplicationUser>()) {
        }

        public override Task<ApplicationUser> FindAsync(string userName, string password) {
            var taskInvoke = Task<ApplicationUser>.Factory.StartNew(() => {
                return new ApplicationUser { Id = userName, UserName = userName };
            });

            return taskInvoke;
        }
    }

    public class ApplicationUser : IUser<string> {
        public string Id { get; set; }
        public string UserName { get; set; }
    }

    public class CustomUserStore<T> : IUserStore<T> where T : ApplicationUser {
        public Task CreateAsync(T user) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T user) {
            throw new NotImplementedException();
        }

        public void Dispose() {
        }

        public Task<T> FindByIdAsync(string userId) {
            throw new NotImplementedException();
        }

        public Task<T> FindByNameAsync(string userName) {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T user) {
            throw new NotImplementedException();
        }
    }
}