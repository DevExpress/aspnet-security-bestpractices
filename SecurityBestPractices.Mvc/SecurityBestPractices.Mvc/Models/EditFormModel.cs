using System;
using System.Collections.Generic;
using System.Linq;

namespace SecurityBestPractices.Mvc.Models {
    public class EditFormItem {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public Guid SecretKey { get; set; }

        public string Url { get; set; }
        public string UrlCaption { get; set; }

        public EditFormItem() {
            SecretKey = Guid.NewGuid();
        }
    }

    public static class EditFormItems {
        static List<EditFormItem> items = new List<EditFormItem>();
        static EditFormItems() {
            items.Add(new EditFormItem { Id = 1, Price = 10,
                ProductName = "Chai <img src=1 onerror=alert('XSS') />", Url= "javascript:alert('XSS')", UrlCaption = "Link 1" });
            items.Add(new EditFormItem { Id = 2, Price = 20, ProductName = "Chang", Url = "https://demos.devexpress.com/RWA/ResponsiveTemplate/", UrlCaption = "Link 2" });
            items.Add(new EditFormItem { Id = 3, Price = 30, ProductName = "Aniseed Syrup", Url = "https://devexpress.com", UrlCaption = "Link 3" });
        }
        public static List<EditFormItem> GetList() {
            return items;
        }

        internal static void Update(EditFormItem item) {
            // throw new Exception("Some sensitive data ***");
            var updatedItem = items.SingleOrDefault(x => x.Id == item.Id);
            if(updatedItem != null) {
                updatedItem.ProductName = item.ProductName;
                updatedItem.Price = item.Price;
            }
        }

        internal static void Delete(int id) {
            var item = items.SingleOrDefault(x => x.Id == id);
            if(item != null)
                items.Remove(item);
        }
    }
}