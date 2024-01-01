using Major_BeanScene_ReservationSystem.CustomValidation;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Models.Reservation;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Major_BeanScene_ReservationSystem.Controllers
{
    public class ReservationController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly SignInManager<IdentityUser> _signInManager;
        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Create()
        {

            DateTime dateTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            
            var p = _context.People.FirstOrDefault(p => p.Email == User.Identity.Name);
            if(p != null)
            {                
                var m = new CreateWithSelectedDate
                {
                    ForSomeoneElse = null,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    SelectedDate = dateTime.Date
                };

                return View(m);
            }
            else if (_signInManager.IsSignedIn(User))
            {
                var m = new CreateWithSelectedDate
                {
                    ForSomeoneElse = null,
                    Email = User.Identity.Name,
                    SelectedDate = dateTime.Date
                };

                return View(m);
            }
            else
            {
                var m = new CreateWithSelectedDate
                {
                    ForSomeoneElse = null,
                    SelectedDate = dateTime.Date
                    //StartTime = dateTime.AddHours(1)
                };

                return View(m);
            }
        }
        [HttpPost]
        public IActionResult UpdateCreateModel([Bind("SelectedDate")][FromBody]Update m)
        {
            //later on validate the selected date to only future dates
            if (ModelState.IsValid) {
                var model = new Models.Reservation.Create()
                {
                    Sitting = new SelectList(_context.Sittings.Where(x => x.StartTime <= m.SelectedDate
                    & x.EndTime >= m.SelectedDate & x.IsClosed != true), "Id", "Name"),
                };
                return Ok(new { SittingId = model.Sitting });

            }
            else
            {
                return BadRequest(new { message = "Invalid date format"});
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWithSelectedDate m)
        {
            var personExist = await _context.People.FirstOrDefaultAsync(p => p.Email == m.Email);
            var u = await _context.Users.FirstOrDefaultAsync(u => u.Email == m.Email);

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

                    return RedirectToAction("Confirmation", new { id = r.Id });
                }
                else
                {     
                    if (u != null && personExist != null)
                    {
                        personExist.UserId= u.Id;

                        r.Person = personExist;
                        r.ReservationOriginId = 1;
                        r.Guests = m.NumberOfGuests;
                        r.StartTime = m.SelectedDate + m.SelectedTime;
                        _context.Add(r);

                        await _context.SaveChangesAsync();
                    }
                    else if (personExist != null)
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
                        _context.People.Add(p);

                        r.Person = p;
                        r.ReservationOriginId = 1;
                        r.Guests = m.NumberOfGuests;
                        r.StartTime = m.SelectedDate + m.SelectedTime;
                        _context.Add(r);

                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction("Confirmation", new { id = r.Id });
                }
            }
            
            m.Sitting = new SelectList(_context.Sittings, "Id", "Name");
            return View(m);
        }

        [HttpGet]
        public IActionResult Confirmation(int id)
        {
            var reservation = _context.Reservations
            .Include(r => r.Person)
            .Include(r => r.Guest)
            .Include(r => r.ReservationOrigin)
            .Include(r => r.ReservationStatus)
            .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public IActionResult ValidateDate([Bind("SelectedDate")]DateTime selectedDate)
        {

            if (selectedDate == null)
            {
                return Json(false);
            }
            if (selectedDate < DateTime.Now.Date)
            {
                return Json("You cannot make a reservation for a date that has passed. Please select a future date.");
            }

            return Json(true);
        }

        public IActionResult ValidateGuests([Bind("NumberOfGuests")]int numberOfGuests)
         {
            if (numberOfGuests == null)
            {
                return Json(false);
            }
            if (numberOfGuests > 10)
            {
                return Json("Maximum 10 guests allowed. Please contact us for larger groups.");
            }
            else if (numberOfGuests <= 0)
            {
                return (Json("There must be at least 1 guest to make a reservation."));
            }

            return Json(true);
        }

        public IActionResult ValidateTime([Bind("SelectedTime", "SelectedDate")] TimeSpan selectedTime, int sittingId, DateTime selectedDate)
        {
            var selectedSitting = _context.Sittings.FirstOrDefault(s => s.Id == sittingId);

            if (selectedSitting == null || selectedTime == null)
            {
                return Json(false);
            }

            if (selectedSitting.StartTime.Date != selectedSitting.EndTime.Date)
            {
                // Handle overnight reservations
                var midnight = new TimeSpan(0, 0, 0);
                var quarterToMidnight = new TimeSpan(23, 45, 0);
                if (selectedDate.Date == selectedSitting.StartTime.Date && selectedTime < selectedSitting.StartTime.TimeOfDay)
                {
                    return Json("Invalid time selection. Please choose a time within the sitting's duration.");
                }
                if(selectedDate.Date == selectedSitting.EndTime.Date && selectedTime > selectedSitting.EndTime.TimeOfDay)
                {
                    return Json("Invalid time selection. Please choose a time within the sitting's duration.");
                }

                if(selectedDate.Date == DateTime.Now.Date && selectedTime < DateTime.Now.TimeOfDay)
                {
                    return Json("You cannot book reservations for a time that has passed. Please select a future time.");
                }
                if (selectedTime >= midnight && selectedTime <= quarterToMidnight)
                {
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            else
            {
                var selectedDateTime = selectedDate.Date.Add(selectedTime);

                if (selectedDateTime < selectedSitting.StartTime || selectedDateTime > selectedSitting.EndTime)
                {
                    return Json("Invalid time selection. Please choose a time within the sitting's duration.");
                }

                if (selectedDate.Date == DateTime.Today && selectedDateTime <= DateTime.Now)
                {
                    return Json("You cannot book reservations for a time that has passed. Please select a future time.");
                }

                return Json(true);
            }
        }


        public IActionResult ValidateDuration([Bind("Duration")] int duration)
        {
            if (duration == null)
            {
                return Json(false);
            }
            if (duration < 15)
            {
                return Json("You cannot make a reservation of under 15 minutes. Please select an appropriate duration.");
            }
            if (duration > 60)
            {
                return Json("If you would like to book a reservation of more than 60 minutes please contact us via email or phone.");
            }

            return Json(true);
        }
    }
}
