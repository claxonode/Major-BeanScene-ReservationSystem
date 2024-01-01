using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    public class Area
    {
        private List<Table> _tables;
        public Area()
        {
            
        }
        public Area(ILazyLoader lazyLoader)
        {
           LazyLoader = lazyLoader;
        }
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public int RestaurantId { get; set; }
        public ILazyLoader LazyLoader { get; set; }
        public Restaurant Restaurant { get; set; }

        public List<Table> Tables {
            get => LazyLoader.Load(this, ref _tables);
            set => _tables = value;
        }


    }
}
