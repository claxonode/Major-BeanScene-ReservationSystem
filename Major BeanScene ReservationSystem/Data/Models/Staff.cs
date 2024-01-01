using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    public class Staff
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string LastName { get; set; }
        public IdentityUser User { get; set; }
        public string FullName { get
            {
                return FirstName + " " + LastName;
            }
        }

    }
}
