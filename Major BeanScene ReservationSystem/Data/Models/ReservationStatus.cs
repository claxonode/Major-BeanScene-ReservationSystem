using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    //  status of the reservation whether confirmed, pending or 
    public class ReservationStatus
    {
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        public List<Reservation> Reservations { get; set; } = new();

    }
}
