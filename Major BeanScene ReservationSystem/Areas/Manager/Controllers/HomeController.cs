using Major_BeanScene_ReservationSystem.Areas.Manager.Models.Home;
using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Controllers
{
    public class HomeController : ManagerBaseController
    {
        public HomeController(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<IActionResult> Index()
        {
            Dictionary<string,int> reservationStatusStats = await _context.ReservationStatuses.Select(r => new
            {
                r.Name,
                r.Reservations.Count,
            }).ToDictionaryAsync(item=>item.Name,item=>item.Count);

            var reservationOriginsData = await _context.ReservationOrigins.Select(o => new
            {
                o.Name,
                o.Reservations.Count
            }).ToDictionaryAsync(item => item.Name, item => item.Count);
            var viewModel = new Models.Home.Index
            {
                ReservationStatus = reservationStatusStats,
                ReservationOrigin = reservationOriginsData
            };

            return View(viewModel);
        }

        public IActionResult Calendar()
        {
            return View();
        }
    }
}
