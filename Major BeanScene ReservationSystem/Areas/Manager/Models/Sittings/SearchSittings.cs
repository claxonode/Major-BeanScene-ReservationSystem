using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Sittings
{
    public class SearchSittings
    {
        public SelectList SittingType { get; set; }
        public int? SittingTypeId { get; set; }
        public PaginatedList<Sitting> Sittings { get; set; }
        public DateTime? SelectedDate { get; set; }
    }

}
