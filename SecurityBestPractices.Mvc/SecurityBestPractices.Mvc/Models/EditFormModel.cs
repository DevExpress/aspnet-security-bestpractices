using System;
using System.Collections.Generic;
using System.Linq;

namespace SecurityBestPractices.Mvc.Models {
    public class EditFormItem {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public static class EditFormItems {
        static List<EditFormItem> items = new List<EditFormItem>();
        static EditFormItems() {
            items.Add(new EditFormItem { Id = 1, Price = 10, ProductName = "Chai" });
            items.Add(new EditFormItem { Id = 2, Price = 20, ProductName = "Chang" });
            items.Add(new EditFormItem { Id = 3, Price = 30, ProductName = "Aniseed Syrup" });
        }
        public static List<EditFormItem> GetList() {
            return items;
        }

        internal static void Update(EditFormItem item) {
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