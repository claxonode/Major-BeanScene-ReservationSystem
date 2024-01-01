using Major_BeanScene_ReservationSystem.Models.MenuItems;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using MongoDbOrdersAPI.Model;
using MongoDbOrdersAPI.Services;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Controllers
{
    [Authorize(Roles = "Staff")]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly MenuItemService _menuItemService;
        public MenuItemsController(MenuItemService menuItemService) =>
            _menuItemService = menuItemService;



        [HttpGet]
        public async Task<ActionResult<List<MenuItem>>> Get()
        {
            List<MenuItem> data = await _menuItemService.GetAsync();
            return Ok(data);
        }
            

        [HttpGet("asSectionData")]
        public ActionResult<List<SectionData>> GetAsSectionDataByCategory()
        {
            return Ok(_menuItemService.GetAsSectionData());
        }
        [HttpGet("{category}/{sortBy}")]
        public IActionResult GetAsSectionDataByCategory(string category, string sortBy, string? menuName)
        {
            if (sortBy == "nameasc" || sortBy == "namedes" || sortBy == "priceasc" || sortBy == "pricedes")
            {
                return Ok(_menuItemService.GetAsSectionData(category, sortBy, menuName));
            }
            return BadRequest("SortBy does not match criteria");

        }

        [HttpGet("{id}:length(24)")]
        public async Task<ActionResult<MenuItem>> Get(string id)
        {
            var menuItem = await _menuItemService.GetAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return Ok(menuItem);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var categories = await _menuItemService.GetCategoriesAsync();
            categories.AddFirst(
                new MenuCategory() {Label="All",Value="ALL" }
                );
  
            return Ok(categories);
        }
        [HttpPost]
        public async Task<IActionResult> Post([Bind("Name,Description,Category,Ingredients,Dietary")] MenuItem newMenuItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Does not match the schema");
            }
            Console.WriteLine();
            await _menuItemService.CreateAsync(newMenuItem);

            return CreatedAtAction(nameof(Get), new { id = newMenuItem.Id }, newMenuItem);
        }

        [HttpPut]
        public async Task<IActionResult> Update([Bind("Id,Name,Description,Category,Ingredients,Dietary")] UpdateMenuItem updatedItem)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest("Does not match the schema");
            }
            if (updatedItem.Id.Length != 24)
            {
                return BadRequest("Ids is not 24 chars");

            }
            var menuItem = await _menuItemService.GetAsync(updatedItem.Id);
            if (menuItem == null)
            {
                return NotFound();
            }

            updatedItem.Id = menuItem.Id;
            var update = updatedItem.Adapt<MenuItem>();
            await _menuItemService.UpdateAsync(updatedItem.Id, update);

            return NoContent();
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var menuItem = await _menuItemService.GetAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            await _menuItemService.RemoveAsync(id);
            return NoContent();
        }

        //then test with postman at MongoDbOrdersAPI
    }
}
