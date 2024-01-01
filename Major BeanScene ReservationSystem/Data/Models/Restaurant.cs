using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        [StringLength(250, MinimumLength = 3)]
        public string Name { get; set; }
        public List<Area> Areas { get; set; } = new();
        public List<Sitting> Sittings { get; set; } = new();

    }
}
