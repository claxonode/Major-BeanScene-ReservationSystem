using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    public class Table
    {
        private Area _area;
        public Table()
        {
             
        }
        public Table(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        public ILazyLoader LazyLoader { get; set; }
        public Area Area { 
            get => LazyLoader.Load(this, ref _area);
            set => _area = value;
        }
        public int AreaId { get; set; }
        public List<Reservation> Reservations { get; set; } = new();
    }
}
