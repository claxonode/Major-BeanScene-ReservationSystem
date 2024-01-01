using MongoDbOrdersAPI.Model;

namespace Major_BeanScene_ReservationSystem.Data.API_Models
{
    public class OrderDateDescend : IComparer<Order>
    {
        public int Compare(Order? x, Order? y)
        {
            if (x.OrderDate >y.OrderDate)
            {
                return -1;
            }
            else if (x.OrderDate <y.OrderDate) 
            {
                return 1;
            }
            else { return 0; }
        }
    }
}
