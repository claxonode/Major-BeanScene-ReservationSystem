using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    public class Guest
    {
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        [StringLength(12, MinimumLength = 3)]
        public string PhoneNumber { get; set; }
        public List<Reservation> Reservations { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
