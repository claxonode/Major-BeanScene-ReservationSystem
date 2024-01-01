using Major_BeanScene_ReservationSystem.Areas.Manager.Models.Reservations;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDbOrdersAPI.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Controllers
{

    public class ReservationController : Controller
    {
        protected readonly ApplicationDbContext _context;
        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Do we delete the person as well connected to reservation
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            if (reservation.StartTime <= DateTime.Now)
            {
                TempData["Error"] = "Cannot delete past record";
                return RedirectToAction("Details", "Sittings", new { Area = "Manager", id = reservation.SittingId });
            }
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();


            return RedirectToAction("Details","Sittings",new {Area="Manager" ,id=reservation.SittingId});
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, int route, 
            [Bind("Id,PersonId,SelectedDate,SelectedTime,SittingId,Duration,Guests,ReservationOriginId,ReservationStatusId,AdditionalNotes")]
             Edit model)
        {

            if (model == null)
            {
                return NotFound();
            }
            //if (id != model.Id)
            //{
            //    return NotFound();
            //}
            var reservation = await _context.Reservations.Include(x=>x.Person).Where(x=>x.Id==id).FirstOrDefaultAsync();
            if (reservation == null)
            {
                return NotFound();
            }
            if (reservation.PersonId != model.PersonId)
            {
                TempData["Error"] = "Person does not match for this reservation.";
                return RedirectToAction("Details", "Sittings", new { Area = "Manager", id = route });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    reservation.SittingId = model.SittingId;
                    reservation.ReservationStatusId = model.ReservationStatusId;
                    reservation.ReservationOriginId = model.ReservationOriginId;
                    reservation.Duration = model.Duration;
                    reservation.AdditionalNotes = model.AdditionalNotes;
                    reservation.Guests = model.Guests;
                    reservation.StartTime = model.SelectedDateTime;
                    _context.Reservations.Update(reservation);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Saved changes";

                }
                catch (DBConcurrencyException) {
                    if (!ReservationExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        
                        ModelState.AddModelError("","Contact system adminstrator.");
                    }
                }
            }
            else
            {
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                TempData["Error"] = JsonConvert.SerializeObject(errorList);
            }
            
            return RedirectToAction("Details", "Sittings",new { Area = "Manager",id = route});
        }

        private bool ReservationExists(int id)
        {
            return (_context.Reservations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
