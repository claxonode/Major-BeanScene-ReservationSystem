using Microsoft.AspNetCore.Mvc;
using Major_BeanScene_ReservationSystem.Data.Models;
using Major_BeanScene_ReservationSystem.Data;
using MongoDB.Driver.Linq;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace Major_BeanScene_ReservationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SittingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SittingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/sittings
        [HttpGet]
        public ActionResult<IEnumerable<Sitting>> GetSittings()
        {
            var sittings = _context.Sittings.ToList();
            return Ok(sittings);
        }

        [HttpGet("{selectedDate}")]
        public ActionResult<IEnumerable<Sitting>> GetSittingsByDate(DateTime selectedDate)
        {
            // Retrieve sittings based on the specified date
            var availableSittings = _context.Sittings
                .Where(s =>
                selectedDate >= s.StartTime.Date &&
                selectedDate <= s.EndTime.Date
                && !s.IsClosed)
                .ToList();

            var sittingsToKeep = new List<Sitting>();

            foreach (var s in availableSittings)
            {
                if (s.EndTime >= DateTime.Now)
                {
                    sittingsToKeep.Add(s);
                }
            }

            return sittingsToKeep;
        }

        [HttpGet("getCurrentCapacity/{sittingId}")]
        public ActionResult GetCurrentCapacityOfSelectedSitting(int sittingId)
        {

            //Dynamic guest count based of the sitting, in Staff/Create/Reservation 
            //To do .. link it to staff/create/reservation
            var selectedSitting = _context.Sittings.Include("Reservations").FirstOrDefault(s=>s.Id== sittingId);
            if (selectedSitting == null)
            {
                return BadRequest("Could not find selected sitting");
            }
            return Ok(selectedSitting.CurrentCapacity);

        }
        [HttpGet("between")]
        public ActionResult<IEnumerable<Sitting>> GetAllSittingsWithinDateRangeCalander([FromQuery]DateTime start, [FromQuery]DateTime end)
        {
            // Retrieve sittings based on the specified date
            var sittings = _context.Sittings
                .Include(s=>s.Reservations)
                .Where(s =>
                s.StartTime >= start &&
                s.StartTime <= end)
                .Select(s=>
                new {title=s.Name +" Capacity: " +(s.Capacity-s.CurrentCapacity)+"/"+s.Capacity,
                    start=s.StartTime,
                    end=s.EndTime,
                    url=$"/manager/sittings/details/{s.Id}",

                })
                .ToList();

            return Ok(sittings);

        }


        //Geoffrey version
        //if you want to pass the dateTime (date&time) from c# to js and fetch into this api
        //1. In c#, Send this value as "dateTime.toString('s')" into the tag as data-attribute;
        // e.g. <span data-combined = "@u.dateTime.toString('s')" >
        //2. In js, Get the tag's data attribute, and use value for fetch
        [HttpGet("dateTime/{selectedDate}")]
        public ActionResult<IEnumerable<Sitting>> GetSittingsByDateMoreAccurate(DateTime selectedDate)
        {
            // Retrieve sittings based on the specified date
            var availableSittings = _context.Sittings
                .Where(s =>
                selectedDate.Date <= s.EndTime.Date &
                selectedDate.Date >= s.StartTime.Date
                )
                .ToList();

            return availableSittings;

        }

        [HttpGet("GetAvailableTimes/{sittingId}")]
        public IActionResult GetAvailableTimes(int sittingId, DateTime selectedDate)
        {
            try
            {
                var selectedSitting = _context.Sittings.FirstOrDefault(s => s.Id == sittingId);

                if (selectedSitting == null)
                {
                    return NotFound("Sitting not found");
                }

                // Calculate available times for the selected day (selectedDate)
                var midnight = selectedDate.TimeOfDay;
                TimeSpan quarterTo = new TimeSpan (23,59,0);
                var startTime = selectedSitting.StartTime.TimeOfDay;
                var endTime = selectedSitting.EndTime.TimeOfDay;
                var timeInterval = TimeSpan.FromMinutes(15); // Adjust as needed

                var availableTimes = new List<TimeSpan>();
                if (selectedSitting.StartTime.Date != selectedDate && selectedSitting.EndTime.Date > selectedDate)
                {
                    while (midnight <= quarterTo)
                    {
                        availableTimes.Add(midnight);
                        midnight = midnight.Add(timeInterval);
                    }
                }
                else if(selectedSitting.StartTime.Date != selectedDate && selectedSitting.EndTime.Date == selectedDate)
                {
                    while (midnight.Add(timeInterval) <= endTime)
                    {
                        availableTimes.Add(midnight);
                        midnight= midnight.Add(timeInterval);
                    }
                }
                else if(selectedSitting.StartTime.Date != selectedSitting.EndTime.Date)
                {
                    while(startTime <= quarterTo)
                    {
                        availableTimes.Add(startTime);
                        startTime = startTime.Add(timeInterval);
                    }
                }
                else
                {
                    while (startTime.Add(timeInterval) <= endTime)
                    {
                        availableTimes.Add(startTime);
                        startTime = startTime.Add(timeInterval);
                    }
                }

                return Ok(availableTimes);
            }
            catch (Exception ex)
            {
                return BadRequest("Error fetching available times: " + ex.Message);
            }
        }
        // Do we want to create more API endpoints, such as POST, PUT, DELETE for managing sittings?
    }

}
