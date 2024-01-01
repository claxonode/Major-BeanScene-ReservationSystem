using Major_BeanScene_ReservationSystem.Areas.Manager.Models.Employee;
using Major_BeanScene_ReservationSystem.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Controllers
{
    public class EmployeeController : ManagerBaseController
    {
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeController(ApplicationDbContext context,UserManager<IdentityUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("Staff");
            var managers = await _userManager.GetUsersInRoleAsync("Manager");

            var m = new Models.Employee.Index
            {
                Users = usersInRole.Select(u => new Models.Employee.Index.UserInfo
                {
                    Id = u.Id,
                    Username = u.UserName,
                    IsAdmin = managers.Contains(u),
                    FullName = _context.Staffs.Where(s => s.UserId == u.Id).Select(s => s.FullName).FirstOrDefault()
                }).ToList(),
            };

            return View(m);
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,AssignAdminRole")]Models.Employee.Create m)
        {
            if (ModelState.IsValid)
            {
                //Email Confirmed for now...
                var user = new IdentityUser { Email = m.Email, UserName = m.Email ,EmailConfirmed=true};
                var result = await _userManager.CreateAsync(user, m.Password);
                if (result.Succeeded)
                {
                    var staff = m.Adapt<Major_BeanScene_ReservationSystem.Data.Models.Staff>();
                    
                    await _userManager.AddToRoleAsync(user, "Staff");
                    staff.UserId = user.Id;
                    await _context.Staffs.AddAsync(staff);
                    _context.SaveChanges();
                    if (m.AssignAdminRole)
                    {
                        await _userManager.AddToRoleAsync(user, "Manager");
                    }
                    

                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Grant(Grant m)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.Where(x => x.Id == m.Id).FirstAsync();
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "Manager");
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Revoke(Grant m)
        {
            var managerRole = await _context.Roles.Where(x => x.Name == "Manager").Select(x => x.Id).FirstOrDefaultAsync();
            var adminCount = await _context.UserRoles.Where(x => x.RoleId == managerRole).CountAsync();
            if (adminCount <= 1)
            {
                ModelState.AddModelError(string.Empty, "Cannot have zero managers");
                TempData["Error"] = "Cannot have zero managers.";
            }
            if (ModelState.IsValid)
            {
                //if there is two admins

                var user = await _context.Users.Where(x => x.Id == m.Id).FirstAsync();

                if (user != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, "Manager");
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
