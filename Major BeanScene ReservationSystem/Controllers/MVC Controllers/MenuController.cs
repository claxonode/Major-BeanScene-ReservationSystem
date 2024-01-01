using Microsoft.AspNetCore.Mvc;
using MongoDbOrdersAPI.Services;

namespace Major_BeanScene_ReservationSystem.Controllers.MVC_Controllers
{
    public class MenuController : Controller
    {
        private readonly MenuItemService _menuItemService;
        public MenuController(MenuItemService menuItemService)
        {
             _menuItemService = menuItemService;
        }
        public IActionResult Index()
        {
            var menuItems = _menuItemService.GetAsync().Result;


            return View(menuItems);
        }
    }
}
