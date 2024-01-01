using Major_BeanScene_ReservationSystem.Areas.Member.Models;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Major_BeanScene_ReservationSystem.Areas.Member.Controllers
{
    public class PersonController : MemberBaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        public PersonController(ApplicationDbContext context, UserManager<IdentityUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var user =  _userManager.GetUserName(User);
            var person =  _context.People.FirstOrDefault(p => p.Email == user);

            if (person == null)
            {
                return View(new Edit());
            }
            var m = new Edit
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                PhoneNumber = person.PhoneNumber
            };
            
            
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Edit m)
        {
            var user = await _userManager.GetUserAsync(User);
            var person = await _context.People.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (person == null)
            {
                Person details = new Person()
                {
                    UserId = user.Id,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    PhoneNumber = m.PhoneNumber,
                    Email = user.Email
                };
                _context.People.Add(details);
                await _context.SaveChangesAsync();
                //will it create another person? or check for another personal with the same details
                //TempData["PersonalDetailsMessage"] = "Please make a reservation first";
                TempData["PersonalDetailsMessage"] = "Your personal details have been updated";
                return RedirectToAction("Index", "Home");
            }

            person.FirstName = m.FirstName;
            person.LastName = m.LastName;
            person.PhoneNumber = m.PhoneNumber;
            await _context.SaveChangesAsync();
            TempData["PersonalDetailsMessage"] = "Your personal details have been updated";

            return RedirectToAction("Index", "Home");
        }
    }
}
