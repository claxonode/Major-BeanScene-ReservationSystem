using Major_BeanScene_ReservationSystem.Data.Models;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home
{
    public class AssignTablesToReservations
    {
        public List<Table> Tables { get; set; }
        public List<Reservation> Reservations { get; set; }
        public List<Sitting> Sittings { get; set; }
    }
}
