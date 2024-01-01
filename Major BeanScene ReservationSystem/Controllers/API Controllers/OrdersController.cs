using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Models.Orders;
using Major_BeanScene_ReservationSystem.Services;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDbOrdersAPI.Model;
using MongoDbOrdersAPI.Services;

namespace Major_BeanScene_ReservationSystem.Controllers.API_Controllers
{
    /// <summary>
    /// Order controller that gets orders
    /// </summary>
    [Authorize(Roles = "Staff")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly MenuItemService _menuItemService;
        private readonly OrderService _orderService;
        private readonly ApplicationContextAPIService _applicationContextService;
        private readonly UserManager<IdentityUser> _userManager;
        public OrdersController(OrderService orderService, MenuItemService menuItemService,ApplicationContextAPIService applicationContextService, UserManager<IdentityUser> userManager)
        {
            _orderService = orderService;
            _menuItemService = menuItemService;
            _applicationContextService = applicationContextService;
            _userManager = userManager;
        }
        /// <summary>
        /// Gets all orders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync() { 
            var orders = await _orderService.GetAllOrdersAsync();
            
            return Ok(orders);
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="id">the id of the order</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(string id)
        {
            var order = await _orderService.GetAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
        /// <summary>
        /// Creates an order
        /// </summary>
        /// <param name="orderData">The order data that will be created</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([Bind("TableNumber,OrderItems,Notes,OrderStatus")] Post orderData)
        {
            var tableId = await _applicationContextService.GetTableByNameAsync(orderData.TableNumber);
            IdentityUser? user = await _userManager.GetUserAsync(User);
            var staff = await _applicationContextService.GetStaffAsync(user.Id);
            if (tableId == null)
            {
                return BadRequest("Could not find matching table.");
            }
            if (user == null ) //staff== null
            {
                return BadRequest("Could not find staff account");
            }

            if (!ModelState.IsValid) //check status PENDING/etc.
            {
                return BadRequest("New order has invalid data");
            }

            try
            {
                //var postedOrder = input.Adapt<Order>();
                var newOrder =  orderData.Adapt<Order>();
                newOrder.OrderDate = DateTime.Now;
                newOrder.LastUpdated = DateTime.Now;
                newOrder.CreatedBy = user.Email;


                //newOrder.StaffEmail = user.Email;
                var id = await _orderService.CreateAsync(newOrder);
                newOrder.OrderId = id;

                return CreatedAtAction(nameof(Get),new {id= id}, newOrder);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not create order");
            }
                    
        }
        /// <summary>
        /// Updates an order
        /// </summary>
        /// <param name="model">the order that will be updated</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(
            [Bind("OrderId,Notes,OrderItems,OrderStatus")] 
        Update model)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest("Body data is invalid");
            }
            var currentOrder = await _orderService.GetAsync(model.OrderId);
            if (currentOrder == null)
            {
                return BadRequest("Could not find this order");
            }
            try
            {
                var replacedOrder = currentOrder.Adapt<Order>();
                replacedOrder.LastUpdated = DateTime.Now;
                replacedOrder.Notes = model.Notes;
                replacedOrder.ProductItems = model.ProductItems;
                var id = await _orderService.ReplaceOneAsync(replacedOrder, currentOrder);
                return CreatedAtAction(nameof(Get), new { id = id }, currentOrder);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not update order");
            }
            
        }
        /// <summary>
        /// Updates the order item's status
        /// </summary>
        /// <param name="orderId">The order</param>
        /// <param name="productName">The menu item's name</param>
        /// <returns></returns>
        [HttpPatch("changeOrderProductStatus/{productName}")]
        public async Task<IActionResult> ChangeProductStatusAsync([FromQuery]string orderId, string productName)
        {
            var order = await _orderService.GetAsync(orderId);
            if (order ==null)
            {
                return BadRequest("Could not find order in database");
            }
            var menuItem = await _menuItemService.GetByNameAsync(productName);
            if (menuItem == null)
            {
                return BadRequest("Could not find menu item in database");
            }
            var findProduct = order.ProductItems.Where(o => o.Name == productName).FirstOrDefault();
            if (findProduct == null)
            {
                return BadRequest("Could not find menu item in order");
            }
            var item = await _orderService.ApproveProductItemAsync(orderId,productName);
            return Ok(item);
        }
        /// <summary>
        /// Completes the order via status
        /// </summary>
        /// <param name="orderId">The order's id</param>
        /// <returns></returns>
        [HttpPatch("completeOrder/{orderId}")]
        public async Task<IActionResult> CompletedOrderAsync(string orderId)
        {
            var order = await _orderService.GetAsync(orderId);
            if (order == null)
            {
                return BadRequest("Could not find order in database");
            }
            var update = await _orderService.CompletedOrderAsync(orderId);
            return Ok(update);
        }
        /// <summary>
        /// Get orders via tables with is status
        /// </summary>
        /// <param name="tableNumber">The table number</param>
        /// <param name="orderStatus">The order status</param>
        /// <returns></returns>
        [HttpGet("getOrdersByTable/{tableNumber}")]
        public async Task<IActionResult> GetOrdersPast24HoursBy(string tableNumber,[FromQuery]string orderStatus)
        {
            var orders = await _orderService.GetOrdersPast24HoursByAsync(tableNumber.ToUpper(),orderStatus.ToUpper());
            return Ok(orders);
        }
        /// <summary>
        /// Get orders past 24 hours
        /// </summary>
        /// <returns></returns>
        [HttpGet("getOrdersPast24Hours")]
        public async Task<IActionResult> GetOrdersPast24Hours()
        {
            var orders = await _orderService.GetOrdersPast24Hours();
            return Ok(orders);
        }
        /// <summary>
        /// Get tables that have pending orders
        /// </summary>
        /// <returns></returns>
        [HttpGet("getTablesWithOrders")]
        public async Task<IActionResult> GetTablesWithPendingOrders()
        {
            var tables = await _orderService.GetTablesWithPendingOrdersAsync();
            return Ok(tables);
        }
    }
}
