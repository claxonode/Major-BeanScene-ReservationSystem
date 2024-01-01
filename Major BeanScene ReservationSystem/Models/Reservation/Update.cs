using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Models.Reservation
{
    public class Update 
    {
        [Required]
        public DateTime SelectedDate { get; set; }
    }
}
