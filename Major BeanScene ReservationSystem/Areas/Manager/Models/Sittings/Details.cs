using Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home;
using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Sittings
{
    public class Details 
    {
        public int Id { get; set; }
        //[Required]
        //[Range(0,1000000)]
        public int Capacity { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public bool IsClosed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Restaurant RestaurantNotAdapted { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public List<Reservation> ReservationsNotAdapted { get; set; } = new();
        public SittingType SittingTypeNotAdapted { get; set; }

        public UpdateReservation Reservation { get; set; }
        public class UpdateReservation :StaffCreateReservation
        {
            public int Id { get; set; }
            public SelectList? ReservationStatus { get; set; }
            public int ReservationStatusId { get; set; }
            public int PersonId { get; set; }
        }
        
        [Required]
        public int SittingTypeId { get; set; }
        public int BookedGuest
        {
            get
            {
                return ReservationsNotAdapted.Sum(x => x.Guests);
            }
        }
    }
}
