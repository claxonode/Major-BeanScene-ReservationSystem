using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Major_BeanScene_ReservationSystem.Areas.Member.Controllers
{
    public class HomeController : MemberBaseController
    {
        protected readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : base(context)
        {
            _userManager= userManager;
            _signInManager= signInManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MyReservations()
        {
            var person = _context.People.FirstOrDefault(p => p.Email == User.Identity.Name);

            if (person != null)
            {
                var reservation = _context.Reservations.Where(r => r.PersonId == person.Id)
                    .Include(r => r.Guest)
                    .Include(r => r.Person)
                    .ToList();
                
                return View(reservation);
            }
            return View();
        }
    }
}
