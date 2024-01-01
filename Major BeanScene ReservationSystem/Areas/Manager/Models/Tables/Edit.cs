using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Tables
{
    public class Edit : Create
    {
        [Required]
        public int Id { get; set; }
    }
}
