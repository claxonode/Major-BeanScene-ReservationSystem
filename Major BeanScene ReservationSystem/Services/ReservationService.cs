using Major_BeanScene_ReservationSystem.Data.Models;

namespace Major_BeanScene_ReservationSystem.Services
{
    public class ReservationService
    {
        public static IQueryable<Reservation> SortBy(IQueryable<Reservation> reservations,string query)
        {
            switch (query)
            {
                case "name_desc":
                    reservations = reservations.OrderByDescending(s => s.Person.FirstName);
                    break;
                case "reservationDate":
                    reservations = reservations.OrderBy(s => s.StartTime);
                    break;
                case "reservationDate_desc":
                    reservations = reservations.OrderByDescending(s => s.StartTime);
                    break;
                default: 
                    reservations = reservations.OrderBy(s => s.Person.FirstName);
                    break;
            }
            return reservations;
        }
    }
}
