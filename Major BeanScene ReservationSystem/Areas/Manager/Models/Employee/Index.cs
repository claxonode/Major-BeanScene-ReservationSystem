

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Employee
{
    public class Index
    {
        public class UserInfo
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public bool IsAdmin { get; set; }

            public string ? FullName { get; set; }

        }
        public List<UserInfo> Users { get; set; }
    }
}
