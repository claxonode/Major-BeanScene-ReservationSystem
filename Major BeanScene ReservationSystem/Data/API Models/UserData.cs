namespace Major_BeanScene_ReservationSystem.Data.API_Models
{
    public class UserData
    {
        public bool Authorized { get; set; }
        public string? Username { get; set; }
        public List<string>? Claims { get; set; }
    }
}
