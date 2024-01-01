using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    public class Sitting
    {
        public int Id { get; set; }
        //[Required]
        //[Range(0,1000000)]
        public int Capacity { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public bool IsClosed { get; set; }
        [ConcurrencyCheck]
        public DateTime StartTime { get; set; }
        [ConcurrencyCheck]
        public DateTime EndTime { get; set; }
        public Restaurant Restaurant { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public List<Reservation> Reservations { get; set; } = new();
        public SittingType SittingType { get; set; }
        [Required]
        public int SittingTypeId { get; set; }

        public int CurrentCapacity { get { return Capacity-Reservations.Sum(x => x.Guests); } }


    }
}
