using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home;
using Microsoft.Data.SqlClient;
using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using MongoDB.Driver.Linq;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Controllers
{
    public class HomeController : StaffBaseController
    {
        public HomeController(ApplicationDbContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> OldIndex()
        {
            DateTime datetime = DateTime.Now;

            ReservationViewModel viewModel = new ReservationViewModel()
            {
                ReservationsNotAdapted = await _context.Reservations.Where(r => r.StartTime <= datetime || r.StartTime >= datetime)
                .Include(x => x.Person)
                .Include(x => x.ReservationOrigin)
                .Include(x => x.ReservationStatus)
                .Include(x => x.Tables)
                .Include(x => x.Sitting)
                .ToListAsync(),
                TableList = new SelectList(await _context.Tables.ToListAsync(),"Id","Name")
                
            };            
            return viewModel!=null
                ?View(viewModel)
                :Problem("Entity set is null");
        }

        public async Task<IActionResult> SearchReservations(int? pageNumber,int? pageSize, int? statusId, string sortOrder,string searchString)
        {
            DateTime datetime = DateTime.Now;

            ViewData["CurrentStatus"] = statusId;
            ViewData["CurrentPageSize"] = pageSize==null?20:pageSize;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["ReservationStartTimeSortParm"] = String.IsNullOrEmpty(sortOrder) ||sortOrder=="reservationDate" ? "reservationDate_desc" : "reservationDate";
            ViewData["FirstNameSortParm"] = sortOrder == "firstName" ? "firstName_desc" : "firstName";
            ViewData["LastNameSortParm"] = sortOrder == "lastName" ? "lastName_desc" : "lastName";
            

            if (statusId ==null || ViewData["CurrentStatus"]==null)
            {
                ViewData["CurrentStatus"] = 1;
                statusId = 1;
            }
            
            IQueryable<Reservation> queryable = _context.Reservations.Where(r => r.StartTime.Date >= datetime.Date && r.ReservationStatusId == statusId)
                .Include(x=>x.Person)
                .Include(x=>x.Guest)
                .Include(x=>x.ReservationOrigin)
                .Include(x=>x.ReservationStatus)
                .Include(x=>x.Sitting);

            SearchReservations model = new SearchReservations()
            {
                ReservationStatus = _context.ReservationStatuses.Where(r=>r.Id==statusId).Select(r=>r.Name).FirstOrDefault(),
                PendingCount = await _context.Reservations.Where(r => r.StartTime.Date >= datetime.Date && r.ReservationStatusId == 1).CountAsync(),
                ConfirmedCount = await _context.Reservations.Where(r => r.StartTime.Date >= datetime.Date && r.ReservationStatusId == 2).CountAsync(),
                CancelledCount = await _context.Reservations.Where(r => r.StartTime.Date >= datetime.Date && r.ReservationStatusId == 3).CountAsync(),
                SeatedCount = await _context.Reservations.Where(r => r.StartTime.Date >= datetime.Date && r.ReservationStatusId == 4).CountAsync(),
                CompletedCount = await _context.Reservations.Where(r => r.StartTime.Date >= datetime.Date && r.ReservationStatusId == 5).CountAsync(),
            };
            if (statusId == 2 || statusId == 4 || statusId == 5)
            {
                model.TableList = new SelectList(await _context.Tables.ToListAsync(), "Id", "Name");
                //model.StatusList = new SelectList(await _context.ReservationStatuses.ToListAsync(), "Id", "Name");
                queryable = queryable.Include(x => x.Tables);
            }

            switch (sortOrder)
            {
                case "firstName_desc":
                    queryable = queryable.OrderByDescending(s => s.Person.FirstName);
                    break;
                case "firstName":
                    queryable = queryable.OrderBy(s => s.Person.FirstName);
                    break;
                case "reservationDate_desc":
                    queryable = queryable.OrderByDescending(s => s.StartTime);
                    break;
                case "lastName":
                    queryable = queryable.OrderBy(s => s.Person.LastName);
                    break;
                case "lastName_desc":
                    queryable = queryable.OrderByDescending(s => s.Person.LastName);
                    break;
                default:
                    queryable = queryable.OrderBy(s => s.StartTime);
                    break;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                queryable = queryable.Where(s => s.Person.FirstName.Contains(searchString) || s.Person.LastName.Contains(searchString));
            }

            model.Reservations = await PaginatedList<Reservation>.CreateAsync(queryable, pageNumber ?? 1, (int)ViewData["CurrentPageSize"]);



            //var pagList = await PaginatedList<Reservation>.CreateAsync(queryable, pageNumber ?? 1, (int)ViewData["CurrentPageSize"]);
            return model != null
                ? View(model)
                : Problem("Entity set is null");
        }

        [HttpGet]
        public IActionResult Create([FromQuery]DateTime? chosenTime,int sittingId)
        {
            DateTime dateTime = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
            if (chosenTime != null)
            {
                dateTime = (DateTime)chosenTime;
            }
            var items = _context.Sittings.Where(x => x.StartTime <= dateTime
                    & x.EndTime >= dateTime & x.IsClosed != true);
            var m = new StaffCreateReservation
            {
                Sitting = new SelectList(items, "Id", "Name"),
                SelectedDate = dateTime.Date,
                ReservationOrigin = new SelectList(_context.ReservationOrigins, "Id", "Name"),
            };

            
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StaffCreateReservation m)
        {
            var personExist = await _context.People.FirstOrDefaultAsync(p => p.Email == m.Email);
            var u = await _context.Users.FirstOrDefaultAsync(u => u.Email == m.Email);

            if (ModelState.IsValid)
            {
                //if (m.NumberOfGuests > 10)
                //{
                //    ModelState.AddModelError("NumGuests", "Maximum 10 guests allowed. Please contact us for larger groups.");
                //    return View(m);
                //}

                var r = m.Adapt<Reservation>();

                if (personExist != null)
                {
                    r.Person = personExist;
                    r.StartTime = m.SelectedDate + m.SelectedTime;
                    r.Guests = m.NumberOfGuests;
                    _context.Add(r);

                    await _context.SaveChangesAsync();
                    //return RedirectToAction("Index");
                    return RedirectToAction("SearchReservations");
                }
                else
                {
                    var x = m.Adapt<Person>();
                    _context.People.Add(x);

                    r.Person = x;
                    r.StartTime = m.SelectedDate + m.SelectedTime;
                    r.Guests = m.NumberOfGuests;
                    _context.Add(r);

                    await _context.SaveChangesAsync();
                    //return RedirectToAction("Index");
                    return RedirectToAction("SearchReservations");
                }
                
            }

            m.Sitting = new SelectList(_context.Sittings, "Id", "Name");
            m.ReservationOrigin = new SelectList(_context.ReservationOrigins, "Id", "Name");
            return View(m);
        }
        public IActionResult ValidateDate([Bind("SelectedDate")] DateTime selectedDate)
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



        public IActionResult ValidateGuests([Bind("NumberOfGuests")] int numberOfGuests)
        {
            if (numberOfGuests == null)
            {
                return Json(false);
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
                if (selectedDate.Date == selectedSitting.EndTime.Date && selectedTime > selectedSitting.EndTime.TimeOfDay)
                {
                    return Json("Invalid time selection. Please choose a time within the sitting's duration.");
                }

                if (selectedDate.Date == DateTime.Now.Date && selectedTime < DateTime.Now.TimeOfDay)
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


        public IActionResult ValidateDuration([Bind("Duration,SelectedTime,SittingId")] int duration,TimeSpan? selectedTime,int sittingId)
        {
            var sitting = _context.Sittings.FirstOrDefault(s => s.Id == sittingId);
            TimeSpan minutes = new TimeSpan(0, duration, 0);
            if (selectedTime==null)
            {
                return Json("Choose a reservation time");
            }
            if (sitting == null)
            {
                return Json(false);
            }
            if (duration == null)
            {
                return Json(false);
            }
            if (duration % 5 != 0)
            {
                return Json("Please create choose time that is divisible by 5");
            }
            if (duration < 15)
            {
                return Json("You cannot make a reservation of under 15 minutes. Please select an appropriate duration.");
            }
            var chosenDateTime = sitting.StartTime.Date.Add((TimeSpan)selectedTime);
            if (chosenDateTime > sitting.EndTime)
            {
                return Json("Duration cannot exceed this sitting's end time.");
            }

            return Json(true);
        }
        public IActionResult AssignTablesToReservations()
        {

            DateTime now = DateTime.Now;
            var sittings = _context.Sittings.Where(s => now.Date <= s.EndTime.Date & now.Date >= s.StartTime.Date).ToList();
            var reservations = _context.Reservations.
                Where(r =>
                (r.ReservationStatus.Id == 2 || r.ReservationStatus.Id == 4))
                .Include(r => r.Sitting)
                .Include(r => r.Person)
                .Include(r => r.ReservationStatus)
                .Include(r => r.Tables)
                .ToList();

            AssignTablesToReservations model = new AssignTablesToReservations()
            {
                Sittings = sittings,
                Reservations = reservations.Where(r => sittings.Any(s => s.Id == r.SittingId)).ToList(),
                Tables = _context.Tables.ToList()
            };
            return View(model);
        }

        public async Task<IActionResult> Today(string sortOrder, string searchString)
        {
            DateTime today = DateTime.Now.Date;
            DateTime tomorrow = today.AddDays(1);
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            //IQueryable for better performance on server
            IQueryable<Reservation> reservations = _context.Reservations.Where(r => r.StartTime >= today && r.StartTime <= tomorrow)
                .Include(x => x.Person)
                .Include(x => x.ReservationOrigin)
                .Include(x => x.ReservationStatus)
                .Include(x => x.Tables)
                .Include(x => x.Sitting);

            ReservationViewModel viewModel = new ReservationViewModel()
            {
                TableList = new SelectList(await _context.Tables.ToListAsync(), "Id", "Name")

            };
            if (!String.IsNullOrEmpty(searchString))
            {
                reservations = reservations.Where(s => s.Sitting.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    reservations = reservations.OrderByDescending(s => s.Person.FirstName);
                    break;
                case "reservationDate":
                    reservations = reservations.OrderBy(s => s.StartTime);
                    break;
                case "reservationDate_desc":
                    reservations = reservations.OrderByDescending(s => s.StartTime);
                    break;
                default:
                    reservations = reservations.OrderBy(s => s.Person.FirstName);
                    break;
            }

            return viewModel != null
                ? View(viewModel)
                : Problem("Entity set is null");
        }

 
        public async Task<IActionResult> ChangeStatus(ChangeStatus model)
        {

            if (ModelState.IsValid)
            {
                var reservation = await _context.Reservations
                    .Include(x=>x.Tables)
                    .Where(x => x.Id == model.ReservationId).FirstOrDefaultAsync();
                if (reservation!=null)
                {  //Is Canceled 
                    
                    if (reservation.ReservationStatusId == 3 && DateTime.Now > reservation.EndTime )
                    {
                        TempData["Error"] = "Cannot change status of a past cancelled reservation";
                        return RedirectToAction(model.Redirect);
                    }

                    //If you are confirming you need a table for completing/seating a reservation
                    if ((model.ReservationStatus==5 && !reservation.Tables.Any()) || (model.ReservationStatus==4 &&!reservation.Tables.Any()))
                    {
                        TempData["Error"] = "Reservation needs a table";
                        return RedirectToAction(model.Redirect);
                    }

                    reservation.ReservationStatusId = model.ReservationStatus;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                if (!model.Redirect.IsNullOrEmpty())
                {
                    return RedirectToAction(model.Redirect);
                }
            }
            //return RedirectToAction("Index");
            return RedirectToAction("SearchReservations");
        }

 
        public async Task<IActionResult> ChangeOrigin(ChangeOrigin model)
        {
            if (ModelState.IsValid)
            {
                var reservation = await _context.Reservations.Where(x => x.Id == model.ReservationId).FirstOrDefaultAsync();
                if (reservation != null)
                {
                    reservation.ReservationOriginId = model.ReservationOrigin;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AssignTable(AssignTable model)
        {
            if (ModelState.IsValid)
            {
                var reservation = await _context.Reservations.Include(x=>x.Tables).
                    Where(x=>x.Id == model.ReservationId).FirstOrDefaultAsync();
                var table = await _context.Tables.Where(x=>x.Id==model.TableId).FirstOrDefaultAsync();
                //change status

                if (reservation!=null && table!=null)
                {
                    reservation.Tables.Add(table);
                    reservation.ReservationStatusId = 4;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
            }
            //return RedirectToAction("Index");
            return RedirectToAction("AssignTablesToReservations");
        }

        public async Task<IActionResult> UnassignTable(AssignTable model)
        {
            if (ModelState.IsValid)
            {
                var reservation = await _context.Reservations.Include(x => x.Tables).
                    Where(x => x.Id == model.ReservationId).FirstOrDefaultAsync();
                var table = await _context.Tables.Where(x => x.Id == model.TableId).FirstOrDefaultAsync();
                if (reservation != null && table != null)
                {
                    reservation.Tables.Remove(table);
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
            }
            //return RedirectToAction("Index");
            return RedirectToAction("AssignTablesToReservations");
        }
        public async Task<IActionResult> UnassignAllTables(int id,string? redirect)
        {
            if (ModelState.IsValid)
            {
                var reservation = await _context.Reservations
                    .Include(x => x.Tables)
                    .Include(x=>x.ReservationStatus)
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
                if (reservation != null)
                {

                    reservation.Tables.Clear();
                    reservation.ReservationStatusId = 2;
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
            }
            if (!redirect.IsNullOrEmpty())
            {
                return RedirectToAction(redirect);
            }
            //return RedirectToAction("Index");
            return RedirectToAction("AssignTablesToReservations");
        }
    }
}
