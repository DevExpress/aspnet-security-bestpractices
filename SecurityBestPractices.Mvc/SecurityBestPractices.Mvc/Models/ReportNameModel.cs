using System.ComponentModel.DataAnnotations;

namespace SecurityBestPractices.Mvc.Models {
    public class ReportNameModel {
        [Required]
        [Display(Name = "Report Name")]
        public string ReportName { get; set; }
    }
}