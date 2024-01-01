using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home
{
    public class Today
    {
        public ReservationStatusModel PendingsReservations { get; set; }
        public ReservationStatusModel CancelledReservations { get; set; }
        public ReservationStatusModel ConfirmedReservations { get; set; }
        public ReservationStatusModel SeatedReservations { get; set; }
        public ReservationStatusModel CompletedReservations { get; set; }

        public class ReservationStatusModel
        {
            public PaginatedList<Reservation> Reservations { get; set; }
            public SelectList TableList { get; set; }
            public int TableId { get; set; }
        }
        public int? StatusId { get; set; }
    }
}
