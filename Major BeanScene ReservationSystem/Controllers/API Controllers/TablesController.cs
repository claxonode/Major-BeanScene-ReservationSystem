using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDbOrdersAPI.Model;

namespace Major_BeanScene_ReservationSystem.Controllers
{
    [Route("api/tables")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TablesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableTables()
        {
            try
            {
                //var tables = await  _context.Tables.Include(t => t.Area).ToListAsync();
                var tables = await _context.Tables.ToListAsync();
                var tableData = tables.Select(t => new
                {
                    t.Id,
                    t.Name,
                    Area = t.Area.Name
                }).ToList();
                return Ok(tableData);
                
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving available tables: {ex.Message}");
            }
        }

        

    }
}
