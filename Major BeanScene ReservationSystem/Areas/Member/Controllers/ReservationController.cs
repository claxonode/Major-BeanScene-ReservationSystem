using Major_BeanScene_ReservationSystem.Areas.Member.Models;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Models.Reservation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Major_BeanScene_ReservationSystem.Areas.Member.Controllers
{
    public class ReservationController : MemberBaseController
    {
        public ReservationController(ApplicationDbContext context) : base(context)
        {

        }

        [HttpGet]
        public IActionResult Create()
        {
            DateTime dateTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            var p = _context.People.FirstOrDefault(p => p.Email == User.Identity.Name);
            if (p != null)
            {
                var m = new Areas.Member.Models.Create
                {
                    ForSomeoneElse = null,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    SelectedDate = dateTime.Date
                };

                return View(m);
            }
            else
            {
                var m = new Areas.Member.Models.Create
                {
                    ForSomeoneElse = null,
                    Email = User.Identity.Name,
                    SelectedDate = dateTime.Date
                };

                return View(m);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Areas.Member.Models.Create m)
        {
            var personExist = await _context.People.FirstOrDefaultAsync(p => p.Email == m.Email);
            var u = await _context.Users.FirstOrDefaultAsync(u => u.Email == m.Email);
            if (personExist != null) {
                personExist.UserId = u.Id;
            }
            if (ModelState.IsValid)
            {
                var r = m.Adapt<Reservation>();

                if (m.ForSomeoneElse == true)
                {
                    if (m.GuestPhoneNumber == null)
                    {
                        ModelState.AddModelError("GuestPhoneNumber", "Guest phone number is required when making a reservation for someone else.");
                    }

                    var g = m.Adapt<Guest>();
                    g.FirstName = m.GuestFirstName;
                    g.LastName = m.GuestLastName;
                    g.Email = m.GuestEmail;
                    g.PhoneNumber = m.GuestPhoneNumber;

                    _context.Add(g);
                    await _context.SaveChangesAsync();
                    if (personExist != null)
                    {
                        r.Guest = g;
                        r.GuestId = g.Id;
                        r.Person = personExist;
                        r.ReservationOriginId = 1;
                        r.Guests = m.NumberOfGuests;
                        r.StartTime = m.SelectedDate + m.SelectedTime;
                        _context.Add(r);

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var p = m.Adapt<Person>();
                        p.UserId = u.Id;
                        _context.People.Add(p);
                        r.GuestId = g.Id;
                        r.Guest = g;
                        r.Person = p;
                        r.ReservationOriginId = 1;
                        r.Guests = m.NumberOfGuests;
                        r.StartTime = m.SelectedDate + m.SelectedTime;
                        _context.Add(r);

                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("Confirmation","Reservation", new {area ="", id = r.Id });
                }
                else
                {
                    if (personExist != null)
                    { 
                        r.Person = personExist;
                        r.ReservationOriginId = 1;
                        r.Guests = m.NumberOfGuests;
                        r.StartTime = m.SelectedDate + m.SelectedTime;
                        _context.Add(r);

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        var p = m.Adapt<Person>();
                        p.UserId = u.Id;
                        _context.People.Add(p);

                        r.Person = p;
                        r.ReservationOriginId = 1;
                        r.Guests = m.NumberOfGuests;
                        r.StartTime = m.SelectedDate + m.SelectedTime;
                        _context.Add(r);

                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("Confirmation", new { area = "", id = r.Id });
                }
            }

            m.Sitting = new SelectList(_context.Sittings, "Id", "Name");
            return View(m);
        }
    }
}
