using Major_BeanScene_ReservationSystem.Data.API_Models;
using Major_BeanScene_ReservationSystem.Models.Orders;
using Mapster;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbOrdersAPI.Model;
using MongoDbOrdersAPI.Services;
using MongoDbOrdersAPI.Data;

namespace Major_BeanScene_ReservationSystem.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly MenuItemService _menuItemService;
        public OrderService(IOptions<OrderSystemDatabaseSettings> orderSystemDatabaseSettings, MenuItemService menuItemService)
        {
            MongoClient client = new MongoClient(orderSystemDatabaseSettings.Value.ConnectionString);
            IMongoDatabase data = client.GetDatabase(orderSystemDatabaseSettings.Value.DatabaseName);

            _ordersCollection = data.GetCollection<Order>(orderSystemDatabaseSettings.Value.OrderCollectionName);
            _menuItemService = menuItemService;
        }
        public async Task<List<Order>> GetAllOrdersAsync() =>
            await _ordersCollection.Find(_ => true).SortByDescending(s=>s.OrderDate).ToListAsync();

        public async Task<Order?> GetAsync(string id) =>
            await _ordersCollection.Find(x => x.OrderId == id).FirstOrDefaultAsync();

        public async Task<string> CreateAsync(Order model)
        {
            //var newOrder = model.Adapt<Order>();
            //regardless of id being set.. mongo will create a new id.
            //validate productItems in Order and set total
            
            foreach (var productItem in model.ProductItems)
            {
                var menuItem = await _menuItemService.GetByNameAsync(productItem.Name);
                if (menuItem == null)
                {
                    throw new Exception("Menu items do not match within the database.");
                }
                if (menuItem.Price != productItem.Price)
                {
                    throw new Exception("Error");
                }
                productItem.Status = "PENDING";
            }
            await _ordersCollection.InsertOneAsync(model);
            return model.OrderId;
        }

        public async Task<string> ReplaceOneAsync(Order replaced,Order existing)
        {

            foreach (var productItem in replaced.ProductItems)
            {
                var menuItem = await _menuItemService.GetByNameAsync(productItem.Name);
                var existingItem = existing.ProductItems.Where(o=>o.Name==productItem.Name).FirstOrDefault();
                if (menuItem == null )
                {
                    throw new Exception("Menu items do not match within the database.");
                }
                else if (menuItem.Price != productItem.Price)
                {
                    throw new Exception("Error");
                }
                else if (existingItem == null)
                {
                    productItem.Status = "PENDING";
                }
                else if (existingItem.Quantity < productItem.Quantity)
                {
                    productItem.Status = "PENDING";
                }

            }
            await _ordersCollection.ReplaceOneAsync(o => o.OrderId == replaced.OrderId, replaced);
            return replaced.OrderId;
        }
        
        public async Task<string> IncrementItemInOrderAsync(string orderId,string menuItem)
        {
            //UpdateDefinition updateDefinition = new MongoDB.Driver.UpdateDefinition();
            //await _ordersCollection.UpdateOneAsync(o=>o.OrderId == orderId,);

            return "not yet";
        }

        public async Task<string> ApproveProductItemAsync(string orderId, string productItem)
        {
            var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId) 
                & Builders<Order>.Filter.ElemMatch(o => o.ProductItems, Builders<ProductItem>.Filter.Eq(p => p.Name, productItem));

            var update = Builders<Order>.Update.Set(o => o.ProductItems.FirstMatchingElement().Status, "SERVED");
            var action = await _ordersCollection.UpdateOneAsync(filter, update);
            return orderId;
        }

        public async Task<string> CompletedOrderAsync(string orderId)
        {
            var filter = Builders<Order>.Filter.Eq(o=>o.OrderId, orderId);
            var update = Builders<Order>.Update
                //.Set(o => o.ProductItems.AllElements().Status, "COMPLETED")
                .Set(o => o.OrderStatus, "COMPLETED");
            var action = await _ordersCollection.UpdateOneAsync(filter,update);
            return orderId;
        }

        public async Task<SortedSet<Order>> GetOrdersPast24HoursByAsync(string tableNumber,string orderStatus)
        {
            //limited by 24 hours..
            //Just by the tableNumber and orderStatus --PENDING/ALL/COMPLETED
            //ordered by recent
            DateTime now = DateTime.Now;
            DateTime last24Hours = now.AddHours(-24);

            var filter = Builders<Order>.Filter.Eq(o => o.TableNumber, tableNumber)
                & Builders<Order>.Filter.Lte(o => o.OrderDate, now)
                & Builders<Order>.Filter.Gte(o => o.OrderDate, last24Hours);
            if (orderStatus != "ALL")
            {
                filter = filter & Builders<Order>.Filter.Eq(o => o.OrderStatus, orderStatus);
            }

            //var orders = await _ordersCollection.FindAsync(filter).Result.ToListAsync();
            var orders = await (await _ordersCollection.FindAsync(filter)).ToListAsync();
            SortedSet<Order> sortedOrder = new SortedSet<Order>(orders,new OrderDateDescend());

            return sortedOrder;            
        }

        public async Task<List<Table>> GetTablesWithPendingOrdersAsync()
        {
            DateTime now = DateTime.Now;
            DateTime last24Hours = now.AddHours(-24);

            var filter = Builders<Order>.Filter.Eq(o => o.OrderStatus, "PENDING")
                & Builders<Order>.Filter.Lte(o => o.OrderDate, now)
                & Builders<Order>.Filter.Gte(o => o.OrderDate, last24Hours);
            //var table = await _ordersCollection.Find(filter).ToListAsync();
            //var sortBuilder = Builders<Order>.Sort.Descending(o => o.OrderDate);
            var pipeline = new EmptyPipelineDefinition<Order>()
                .Match(filter)
                //.Sort(sortBuilder)
                .Group(o => o.TableNumber, o => new Table
                {
                    Label = o.Key
                });
            var tables = await _ordersCollection.Aggregate(pipeline).ToListAsync();
            //sort does not work?

            return tables;

        }

        public async Task<List<Order>> GetOrdersPast24Hours()
        {
            DateTime now = DateTime.Now;
            DateTime last24Hours = now.AddHours(-24);
            var filter = Builders<Order>.Filter.Lte(o => o.OrderDate, now)
                & Builders<Order>.Filter.Gte(o => o.OrderDate, last24Hours);
            //var orders = await _ordersCollection.Find(filter).SortByDescending(o => o.OrderDate).ToListAsync();
            var orders = await _ordersCollection.Find(filter).ToListAsync();
            orders = orders.OrderByDescending(o => o.OrderDate).ToList();
            return orders;
        }
    }
}
