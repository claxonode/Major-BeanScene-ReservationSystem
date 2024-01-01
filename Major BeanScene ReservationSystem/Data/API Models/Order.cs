using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbOrdersAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MongoDbOrdersAPI.Model
{
    public class Order
    {
        [Required]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string TableNumber { get; set; }

        //paid, not paid
        //need to do enum
        public string OrderStatus { get; set; }
        //addition info for order.. maybe takeway?
        public string? Notes { get; set; }
        [Required]
        //
        [BsonElement("orderItems")]
        [JsonPropertyName("orderItems")]
        public HashSet<ProductItem> ProductItems { get; set; }
        public decimal TotalPrice { 
            get {
                decimal total = 0;
                foreach (var productItem in ProductItems)
                {
                    total += productItem.Price * productItem.Quantity;
                }
                return total;
            } 
            set {
                decimal total = 0;
                foreach (var productItem in ProductItems)
                {
                    total += productItem.Price * productItem.Quantity;
                }
                value= total;
            } 
        }
        [Required]
        public string CreatedBy { get; set; }
        [EmailAddress]
        //[Required]
        //public string StaffEmail { get; set; }

        public int ItemCount
        {
            get
            {
                int total = 0;
                foreach (var productItem in ProductItems)
                {
                    total += productItem.Quantity;
                }
                return total;
            }
            set
            {
                int total = 0;
                foreach (var productItem in ProductItems)
                {
                    total += productItem.Quantity;
                }
                value= total;
            }
        }
    }
}
