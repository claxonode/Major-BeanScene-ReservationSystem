using System.ComponentModel.DataAnnotations;

namespace MongoDbOrdersAPI.Model
{
    public class SectionData
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public List<MenuItem> Data { get; set; }
        
    }
}
