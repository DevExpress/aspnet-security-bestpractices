using DevExpress.Web;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace SecurityBestPractices.Mvc.Models {
    [Bind(Exclude = "Salary")]
    public class UserProfile {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        public string Email { get; set; }

        public double Salary { get; set; } // Not editable by an end user
    }

    public class UserProfileEx {
        [Mask("+1 (999) 000-0000", IncludeLiterals = MaskIncludeLiteralsMode.None, ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DateRange(StartDateEditFieldName = "StartDate", MinDayCount = 1, MaxDayCount = 30)]
        public DateTime EndDate { get; set; }
    }

    // Products (for lookups)
    public class ProductItem {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public static class ProductItems {
        static List<ProductItem> items = new List<ProductItem>();
        static ProductItems() {
            items.Add(new ProductItem { Id = 1, Price = 10, ProductName = "Small headphones" });
            items.Add(new ProductItem { Id = 2, Price = 100, ProductName = "Hi-fi headphones" });
            items.Add(new ProductItem { Id = 3, Price = 500, ProductName = "Mobile phone" });
            items.Add(new ProductItem { Id = 4, Price = 1000, ProductName = "Laptop" });
        }
        public static List<ProductItem> GetAllItems() {
            return items;
        }
        
        public static List<ProductItem> GetAvailableForUserList() {
            int userBonusLimit = 100;
            return GetAllItems().FindAll(i=>i.Price <= userBonusLimit);
        }
    }
}