using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbOrdersAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Major_BeanScene_ReservationSystem.Models.Orders
{
    public class Post
    {
        public string TableNumber { get; set; }

        //paid, not paid
        public string OrderStatus { get; set; }
        //addition info for order.. maybe takeway?
        public string? Notes { get; set; }

        [Required]
        //
        [BsonElement("orderItems")]
        [JsonPropertyName("orderItems")]
        public HashSet<ProductItem> ProductItems { get; set; }
    }
}
