using System.ComponentModel.DataAnnotations;

namespace SecurityBestPractices.Mvc.Models {
    public class LoginViewModel {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}
