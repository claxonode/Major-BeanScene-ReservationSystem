using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Tables
{
    public class Create
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int AreaId { get; set; }
        public SelectList? Areas { get; set; }
    }
}
