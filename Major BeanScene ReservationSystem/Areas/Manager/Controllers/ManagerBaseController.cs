using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = "Manager")]
    public class ManagerBaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        public ManagerBaseController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
