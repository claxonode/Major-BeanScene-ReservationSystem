using Microsoft.AspNetCore.Mvc;

namespace Major_BeanScene_ReservationSystem.Controllers.MVC_Controllers
{
    public class ErrorController : Controller
    {
        //[Route("/error/status/{statusCode}")]
        public IActionResult Status(int statusCode)
        {
            if (statusCode == 404)
            {
                //log here
                return View("Error404");
            }
            return View();
        }
    }
}
