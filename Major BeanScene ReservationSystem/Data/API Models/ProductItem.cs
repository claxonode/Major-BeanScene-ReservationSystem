using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbOrdersAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MongoDbOrdersAPI.Model
{

    public class ProductItem
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.05,500)]
        public decimal Price { get; set; }

        [Required]
        [Range(1,100)]
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public string ? Status { get; set; } //SERVED/PENDING/PREPARING

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    MenuItemService? menuItemService = (MenuItemService)validationContext.GetService(typeof(MenuItemService));
        //    MenuItem? menuItem = menuItemService.GetByNameAsync(Name).Result;
        //    //if (menuItemService == null)
        //    //{
        //    //    yield return new ValidationResult("Menu item does not exist.", new[] { nameof(MenuItemName) });
        //    //}

        //    //if (menuItem.MenuItemName != MenuItemName)
        //    //{
        //    //    yield return new ValidationResult("Menu name does not match.", new[] { nameof(MenuItemName) });
        //    //}
        //    if (menuItem.Price != Price)
        //    {
        //        yield return new ValidationResult("Menu price does not match.", new[] { nameof(Price) });
        //    }
        //    if (Status != "Preparing" || Status != "Served" ||Status!="PENDING")
        //    {
        //        yield return new ValidationResult("Menu price does not match.", new[] { nameof(Price) });
        //    }
        //}
    }
}
