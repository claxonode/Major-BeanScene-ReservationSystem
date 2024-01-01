using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home
{
    public class ReservationViewModel
    {
        public List<Reservation> ReservationsNotAdapted { get; set; }

        public SelectList TableList { get; set; }
        public int TableId { get; set; }
    }

}
