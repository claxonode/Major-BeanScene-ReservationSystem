using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MongoDbOrdersAPI.Model
{
    public class MenuItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [MaxLength(50)]
        [MinLength(1)]
        [Required]
        public string Name { get; set; }
        [MaxLength(200)]
        [MinLength(1)]
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(0.05, 500)]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; } //MAIN,DRINK,
        public List<string> ? Ingredients { get; set; }
        public List<string> ? Dietary { get; set; }

        //json property name: JSON name for deserialising and serialising
        //bsonelement: element name for mongodb
    }
}
