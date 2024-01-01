using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    [Table("People")]
    public class Person
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        //later restrict for number 04
        [StringLength(12, MinimumLength = 3)]
        public string PhoneNumber { get; set; }

        public List<Reservation> Reservations { get; set; } = new();

        public string FullName { 
            get {return FirstName +" "+ LastName; } 
        }
    }

}
