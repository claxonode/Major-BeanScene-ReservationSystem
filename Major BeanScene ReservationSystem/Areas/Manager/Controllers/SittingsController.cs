using Major_BeanScene_ReservationSystem.Areas.Manager.Models.Sittings;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Major_BeanScene_ReservationSystem.Areas.Manager.Models.Sittings.Create;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Controllers
{
    public class SittingsController : ManagerBaseController
    {
        public SittingsController(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> SearchSittings(int? pageNumber, int? pageSize, int? sittingTypeId, string sortOrder, string searchString, DateTime? selectedDate)
        {
            ViewData["CurrentSittingType"] = sittingTypeId;
            ViewData["CurrentPageSize"] = pageSize ?? 20;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;
            ViewData["SelectedDate"] = selectedDate;
            ViewData["SittingStartTimeSortParm"] = String.IsNullOrEmpty(sortOrder) || sortOrder == "sittingStartTime" ? "sittingStartTime_desc" : "sittingStartTime";
            ViewData["SittingTypeSortParm"] = sortOrder == "sittingType" ? "sittingType_desc" : "sittingType";

            IQueryable<Sitting> queryable = _context.Sittings
                .Where(s => s.IsClosed == false || s.IsClosed == true)
                .Include(x => x.SittingType);

            if (sittingTypeId == null)
            {
                sittingTypeId = null;
                ViewData["CurrentSittingType"] = sittingTypeId;
            }

            if (sittingTypeId != null && sittingTypeId != 0)
            {
                queryable = queryable.Where(s => s.SittingTypeId == sittingTypeId);
            }

            if (selectedDate != null)
            {
                queryable = queryable.Where(s => s.StartTime.Date == selectedDate.Value.Date);
            }

            switch (sortOrder)
            {
                case "sittingStartTime_desc":
                    queryable = queryable.OrderByDescending(s => s.StartTime);
                    break;
                case "sittingStartTime":
                    queryable = queryable.OrderBy(s => s.StartTime);
                    break;
                case "sittingType_desc":
                    queryable = queryable.OrderByDescending(s => s.SittingType.Name);
                    break;
                case "sittingType":
                    queryable = queryable.OrderBy(s => s.SittingType.Name);
                    break;
                case "sittingName_desc":
                    queryable = queryable.OrderByDescending(s => s.Name);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                queryable = queryable.Where(s => s.Name.Contains(searchString) || s.SittingType.Name.Contains(searchString));
            }

            SearchSittings model = new SearchSittings()
            {
                SittingType = new SelectList(await _context.SittingTypes.ToListAsync(), "Id", "Name"),
                Sittings = await PaginatedList<Sitting>.CreateAsync(queryable, pageNumber ?? 1, pageSize ?? 20),
                SelectedDate = selectedDate,
                SittingTypeId = sittingTypeId
            };

            return View(model);
        }


        // GET: Manager/Sittings
        public async Task<IActionResult> Index()
        {
            var model = await _context.Sittings
                        .Include(s => s.Restaurant)
                        .Include(s => s.SittingType)
                        .ToListAsync();

            LinkedList<Sitting> sittings = new LinkedList<Sitting>();
            foreach (var m in model)
            {
                sittings.AddLast(m);
            }

            return _context.Sittings != null ?
                        View(sittings) :
                        Problem("Entity set 'ApplicationDbContext.Sittings'  is null.");
        }

        // GET: Manager/Sittings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sittings == null)
            {
                return NotFound();
            }
            var sitting = await _context.Sittings
                .Include(s => s.Restaurant)
                .Include(s => s.SittingType)
                .Include(s => s.Reservations.Where(r => r.SittingId == id))
                    .ThenInclude(r => r.Tables)
                .Include(s => s.Reservations.Where(r => r.SittingId == id))
                    .ThenInclude(r => r.Person)
                .Include(s => s.Reservations.Where(r => r.SittingId == id))
                    .ThenInclude(r => r.Guest)
                .Include(s => s.Reservations.Where(r => r.SittingId == id))
                    .ThenInclude(r => r.ReservationOrigin)
                .Include(s => s.Reservations.Where(r => r.SittingId == id))
                    .ThenInclude(r => r.ReservationStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            var reservationOrigin = await _context.ReservationOrigins.ToListAsync();
            var reservationStatus = await _context.ReservationStatuses.ToListAsync();
            if (sitting == null)
            {
                return NotFound();
            }
            //Mapster notcrashing by changing Name

            var model = sitting.Adapt<Details>();
            model.ReservationsNotAdapted = sitting.Reservations;
            model.RestaurantNotAdapted = sitting.Restaurant;
            model.SittingTypeNotAdapted = sitting.SittingType;
            model.Reservation = new Details.UpdateReservation
            {
                
                ReservationOrigin = new SelectList(reservationOrigin, "Id", "Name"),
                ReservationStatus = new SelectList(reservationStatus, "Id", "Name")
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult Create(DateTime? start,DateTime? end)
        {
            DateTime dateTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,7,0,0);
            ////SelectList xx= new Se
            var model = new Create() {
                Capacity = 40,
                Restaurants = new SelectList(_context.Restaurants, "Id", "Name"),
                StartTime = dateTime,
                EndTime = dateTime.AddHours(2),
                SittingTypes = new SelectList(_context.SittingTypes, "Id", "Name"),
                SelectedDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                RepeatOptions =new List<SelectListItem>()
                {
                    new SelectListItem{Value = "1", Text="Never"},
                    new SelectListItem{Value = "2", Text="Daily"},
                    new SelectListItem{Value = "3", Text="Weekly"},
                }
            };
            if (start != null && end!=null)
            {
                model.StartTime = (DateTime)start;
                model.EndTime = (DateTime)end;
                TempData["Success"] =$"Sitting from {model.StartTime} to {model.EndTime}";
            }

            return View(model);
        }
        // POST: Manager/Sitting/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Capacity,Name,IsClosed,StartTime,EndTime,RestaurantId,SittingTypeId,RepeatOptionId,RepeatWeeksCount,SelectedDaysOfWeek,Occurence")] Create sitting)
        {
            if (ModelState.IsValid)
            {
                //No repeats
                if (sitting.RepeatOptionId == 1)
                {
                    var sit = sitting.Adapt<Sitting>();
                    _context.Add(sit);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Calendar", "Home", "");
                    //return RedirectToAction(nameof(Index));

                }
                //Repeat Daily
                if (sitting.RepeatOptionId == 2)
                {
                    if (sitting.Occurence ==0)
                    {
                        ModelState.AddModelError("Occurence","Must repeat at least one time");
                    }
                    else
                    {
                        //If repeat is 1, results in two sittings
                        
                        var sittings = new List<Sitting>();
                        for (int i = 0; i <= sitting.Occurence; i++)
                        {
                            var sit = sitting.Adapt<Sitting>();
                            sit.StartTime = sit.StartTime.AddDays(i * 7);
                            sit.EndTime = sit.EndTime.AddDays(i * 7);
                            
                            sittings.Add(sit);
                            
                        }
                        await _context.AddRangeAsync(sittings);
                        await _context.SaveChangesAsync();
                        //return RedirectToAction(nameof(Index));
                        return RedirectToAction("Calendar", "Home", "");
                    }
                    
                }
                if (sitting.RepeatOptionId == 3)
                {
                    //Allow for 1 week
                    if (sitting.Occurence == 0)
                    {
                        ModelState.AddModelError("Occurence", "Must repeat at least one time");
                    }
                    if (sitting.SelectedDaysOfWeek != null)
                    {
                        if (!sitting.SelectedDaysOfWeek.Contains(sitting.StartTime.DayOfWeek))
                        {
                            ModelState.AddModelError("RepeatOptionId", $"Must have chosen {sitting.StartTime.DayOfWeek} to repeat");
                        }
                        else
                        {
                            var sittings = new List<Sitting>();
                            //Insertion ording is slighty worse but more efficient
                            //foreach (var day in sitting.SelectedDaysOfWeek) { 
                            //    int dateDif = (int)Math.Abs((int)(sitting.StartTime.DayOfWeek - day));

                            //    for (int i = 0; i <= sitting.Occurence; i++)
                            //    {

                            //        var sit = sitting.Adapt<Sitting>();
                            //        sit.StartTime = sit.StartTime.AddDays(dateDif + i*7);
                            //        sit.EndTime = sit.EndTime.AddDays(dateDif + i*7);
                            //        sittings.Add(sit);
                            //    }
                            //}

                            sitting.SelectedDaysOfWeek.Sort();
                            for (int i = 0; i < sitting.Occurence; i++)
                            {
                                
                                
                                foreach (var day in sitting.SelectedDaysOfWeek)
                                {
                                    //Need to stop create sittings in the past..
                                    if (i==0 && day < sitting.StartTime.DayOfWeek )
                                    {
                                        continue;
                                    }

                                    int dateDif = (day - sitting.StartTime.DayOfWeek);
                                    var sit = sitting.Adapt<Sitting>();
                                    sit.StartTime = sit.StartTime.AddDays(dateDif + i * 7);
                                    sit.EndTime = sit.EndTime.AddDays(dateDif + i * 7);
                                    sittings.Add(sit);
                                }
                                
                            }
                            await _context.AddRangeAsync(sittings);
                            await _context.SaveChangesAsync();
                            //return RedirectToAction("Index");
                            return RedirectToAction("Calendar","Home","");
                        }

                    }
                    ModelState.AddModelError("RepeatOptionId", "Must choose at least one day or the same day you have choosen");
                    
                }
            }
            sitting.Restaurants = new SelectList(_context.Restaurants, "Id", "Name");
            sitting.SittingTypes = new SelectList(_context.SittingTypes, "Id", "Name");
            sitting.SelectedDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
            sitting.RepeatOptions = new List<SelectListItem>()
            {
                new SelectListItem{Value = "1", Text="Never"},
                new SelectListItem{Value = "2", Text="Daily"},
                new SelectListItem{Value = "3", Text="Weekly"},
            };
            
            return View(sitting);
        }

        

        ////Checks that id exists run the edit and create version
        public IActionResult ValidateDateTime(DateTime startTime, DateTime endTime)
        {
            //different validation if id exists
            if (startTime == null || endTime == null)
            {
                return Json(false);
            }
            if (startTime >= endTime)
            {
                return Json("Date is invalid. Check the order of the times.");
            }
            
            return Json(true);
        }

        // GET: Manager/Sitting/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sittings == null)
            {
                return NotFound();
            }

            //var sitting = await _context.Sittings.FindAsync(id);
            var sitting = await _context.Sittings.Include("Reservations")
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sitting == null)
            {
                return NotFound();
            }
            bool sittingHasPast = sitting.EndTime < DateTime.Now;
            if (sittingHasPast)
            {
                TempData["Error"] = "Cannot edit a sitting in the past";
                return RedirectToAction("Details", "Sittings", new { id = id });
            }
            var m = sitting.Adapt<Edit>();
            m.Restaurants = new SelectList(_context.Restaurants, "Id", "Name");
            m.SittingTypes = new SelectList(_context.SittingTypes, "Id", "Name");
            return View(m);
        }

        // POST: Manager/Sitting/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Capacity,Name,IsClosed,StartTime,EndTime,RestaurantId,SittingTypeId")] Edit sitting)
        {
            if (id != sitting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var transaction = _context.Database.BeginTransaction();
                try
                {
                    Sitting current = await _context.Sittings.Where(s => s.Id == id).FirstOrDefaultAsync();
                    _context.Entry(current).State = EntityState.Detached;
                    var toUpdate = sitting.Adapt<Sitting>();
                    _context.Update(toUpdate);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SittingExists(sitting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Calendar", "Home", "");
            }
            sitting.Restaurants = new SelectList(_context.Restaurants, "Id", "Name");
            sitting.SittingTypes = new SelectList(_context.SittingTypes, "Id", "Name");
            return View(sitting);
        }

        // GET: Manager/Sitting/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sittings == null)
            {
                return NotFound();
            }

            var sitting = await _context.Sittings
                .Include(s => s.Restaurant)
                .Include(s => s.SittingType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sitting == null)
            {
                return NotFound();
            }
            bool sittingHasPast = sitting.EndTime < DateTime.Now;
            if (sittingHasPast)
            {
                TempData["Error"] = "Cannot delete a sitting in the past";
                //return RedirectToAction("Index");
                return RedirectToAction("Details", "Sittings",new {id=id});
            }

            return View(sitting);
        }

        // POST: Manager/Sitting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Sittings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Sittings'  is null.");
            }
            var sitting = await _context.Sittings.Include(s => s.Reservations)
                .Include(s => s.Restaurant)
                .Include(s => s.SittingType)
                .FirstOrDefaultAsync(s => s.Id == id);
            ///Prevent delete of sitting that has reservations
            if (sitting != null)
            {
                bool sittingHasPast = sitting.EndTime < DateTime.Now;
                if (sittingHasPast)
                {
                    TempData["Error"] = "Cannot delete a sitting in the past";
                    return RedirectToAction("Index");
                }
                if (sitting.Reservations.Any())
                {
                    ViewData["Error"] = $"Cannot delete this sitting. It contains {sitting.Reservations.Count} reservation";
                    return View(sitting);
                }
                _context.Sittings.Remove(sitting);
                await _context.SaveChangesAsync();
            }

            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Calendar", "Home", "");
        }



        private bool SittingExists(int id)
        {
            return (_context.Sittings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

