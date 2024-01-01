using Major_BeanScene_ReservationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Major_BeanScene_ReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Manager"))
            {
                return RedirectToAction("RedirectUser", "Home");
            }
            else
            {
                return View();
            }
        }

        public IActionResult RedirectUser()
        {
            if (User.IsInRole("Member"))
            {
                return RedirectToAction("Index", "Home", new { area = "Member" });
            }
            else if (User.IsInRole("Manager"))
            {
                return RedirectToAction("Index", "Home", new { area = "Manager" });
            }
            else if (User.IsInRole("Staff"))
            {
                return RedirectToAction("Index", "Home", new { area = "Staff" });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}