using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home
{
    public class SearchReservations
    {
        public PaginatedList<Reservation> Reservations { get; set; }

        public int PendingCount { get; set; }
        public int ConfirmedCount { get; set; }
        public int CancelledCount { get; set; }
        public int SeatedCount { get; set; }
        public int CompletedCount { get; set; }

        public string ? ReservationStatus { get; set; }
        public SelectList TableList { get; set; }
        //public SelectList StatusList { get; set; }
        public int? StatusId { get; set; }
        public int TableId { get; set; }
    }
}
