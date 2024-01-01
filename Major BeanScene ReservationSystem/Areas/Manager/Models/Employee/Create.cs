using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Employee
{
    public class Create
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //prevent admin from not existing
        //later use logging to record weird behavior
        public bool AssignAdminRole { get; set; }
    }
}
