namespace Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home
{
    public class ChangeStatus
    {
        public int ReservationId { get; set; }
        public int ReservationStatus { get; set; }

        public string? Redirect { get; set; }
    }
}
