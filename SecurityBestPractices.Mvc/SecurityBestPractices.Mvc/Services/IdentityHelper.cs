using System.Web;

namespace SecurityBestPractices.Mvc.Services {
    public static class IdentityHelper {
        public static string GetIdentityName() {
            return HttpContext.Current.User?.Identity?.Name;
        }
    }
}