using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Major_BeanScene_ReservationSystem.Areas.Manager.Models.Tables;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class TablesController : ManagerBaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        public TablesController(ApplicationDbContext context,UserManager<IdentityUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        // GET: Manager/Tables
        public async Task<IActionResult> Index()
        {
            var tables = _context.Tables;
            return View(await tables.ToListAsync());
        }

        // GET: Manager/Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }
            //Use lazy loader
            var table = await _context.Tables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // GET: Manager/Tables/Create
        public IActionResult Create()
        {
            var model = new Create() { 
                Areas = new SelectList(_context.Areas, "Id", "Name") 
            };
            return View(model);
        }

        // POST: Manager/Tables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,AreaId")] Models.Tables.Create table)
        {
            if (ModelState.IsValid)
            {
                var tab = table.Adapt<Table>();
                _context.Add(tab);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            table.Areas = new SelectList(_context.Areas, "Id", "Name");
            return View(table);
        }

        // GET: Manager/Tables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            var m = table.Adapt<Edit>();
            m.Areas = new SelectList(_context.Areas, "Id", "Name");
            return View(m);
        }

        // POST: Manager/Tables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AreaId")] Models.Tables.Edit table)
        {
            if (id != table.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Table current = await _context.Tables.Where(t => t.Id == id).FirstOrDefaultAsync();
                    _context.Entry(current).State = EntityState.Detached;
                    var toUpdate = table.Adapt<Table>();
                    _context.Update(toUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(table.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            table.Areas = new SelectList(_context.Areas, "Id", "Name");
            return View(table);
        }

        // GET: Manager/Tables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tables == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Manager/Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tables == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tables'  is null.");
            }
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TableExists(int id)
        {
          return (_context.Tables?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
