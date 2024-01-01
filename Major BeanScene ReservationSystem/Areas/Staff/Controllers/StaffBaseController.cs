using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles ="Staff")]
    public class StaffBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public StaffBaseController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
