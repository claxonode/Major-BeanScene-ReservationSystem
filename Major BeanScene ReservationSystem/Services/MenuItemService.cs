using Humanizer;
using Major_BeanScene_ReservationSystem.Models.MenuItems;
using Mapster;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbOrdersAPI.Data;
using MongoDbOrdersAPI.Model;

namespace MongoDbOrdersAPI.Services
{
    public class MenuItemService
    {
        private readonly IMongoCollection<MenuItem> _menuItemCollection;
        public MenuItemService(IOptions<OrderSystemDatabaseSettings> orderSystemDatabaseSettings)
        {
            MongoClient client = new MongoClient(orderSystemDatabaseSettings.Value.ConnectionString);
            IMongoDatabase data = client.GetDatabase(orderSystemDatabaseSettings.Value.DatabaseName);

            _menuItemCollection = data.GetCollection<MenuItem>(orderSystemDatabaseSettings.Value.MenuCollectionName);
        }

        public async Task<List<MenuItem>> GetAsync() =>
            await _menuItemCollection.Find(_ => true).ToListAsync();

        public List<SectionData> GetAsSectionData()
        {
            var query = _menuItemCollection.AsQueryable().GroupBy(x => x.Category).ToList();

            List<SectionData> sectionData = new List<SectionData>();
            foreach (var item in query)
            {
                sectionData.Add(new SectionData()
                {
                    Title = item.Select(x => x.Category).First(),
                    Data = item.ToList()
                });
            }

            return sectionData;
        }

        public List<SectionData> GetAsSectionData(string category, string sortBy, string ? menuName)
        {
            var queryableCollection = _menuItemCollection.AsQueryable();
            List<IGrouping<string, MenuItem>> query;
            //1 Find by category, 2. each spaced name search it

            if (category.ToLower() == "all")
            {
                if (menuName == null)
                {
                    query = queryableCollection.GroupBy(x => x.Category).ToList();
                }
                else
                {
                    menuName = menuName.ToLower();
                    query = queryableCollection.ToList()
                        .Where(x => x.Name.Split(' ').Any(word => word.ToLower().StartsWith(menuName)))
                        .GroupBy(x => x.Category)
                        .ToList();
                }
            }
            else
            {
                if (menuName == null)
                {
                    query = queryableCollection.Where(x => x.Category.ToLower() == category.ToLower())
                        .GroupBy(x => x.Category)
                        .ToList();
                }
                else
                {
                    menuName = menuName.ToLower();
                    query = queryableCollection.Where(x => x.Category.ToLower() == category.ToLower())
                    .ToList()
                    .Where(x => x.Name.Split(' ').Any(word => word.ToLower().StartsWith(menuName)))
                    .GroupBy(x => x.Category).ToList();
                }
            }


            List<SectionData> sectionData = new List<SectionData>();
            foreach (var item in query)
            {
                sectionData.Add(new SectionData()
                {
                    Title = item.Select(x => x.Category).First(),
                    Data = item.ToList()
                });
            }
            sortBy = sortBy.ToLower();
            foreach (var section  in sectionData)
            {
                if (sortBy == "nameasc")
                {
                    section.Data = section.Data.OrderBy(x => x.Name).ToList();
                }
                else if (sortBy == "namedes")
                {
                    section.Data = section.Data.OrderByDescending(x => x.Name).ToList();
                }
                else if (sortBy == "priceasc")
                {
                    section.Data = section.Data.OrderBy(x => x.Price).ToList();
                }
                else if (sortBy == "pricedes")
                {
                    section.Data = section.Data.OrderByDescending(x => x.Price).ToList();
                }
                
            }

            sectionData = sectionData.OrderBy(x => x.Title).ToList();
            

            return sectionData;
        }

        public List<MenuItem> FilterItems(List<MenuItem> items,string query) { 
            query = query.ToLower();
            return items.Where(item=>item.Name.Split(' ').Any(word=>word.ToLower().StartsWith(query))).ToList(); 
        }


        public async Task<MenuItem?> GetAsync(string id) => 
            await _menuItemCollection.Find(x=>x.Id == id).FirstOrDefaultAsync();

        public async Task<MenuItem?> GetByNameAsync(string name) =>
            await _menuItemCollection.Find(x => x.Name == name).FirstOrDefaultAsync();

        //public async Task<HashSet<string>> GetAllIdsAsync()
        //{
        //    var indexes = _menuItemCollection.AsQueryable().Select(m=>m.Id).ToHashSet();

        //    return indexes;
        //}
            

        public async Task CreateAsync(MenuItem newMenuItem) =>
            await _menuItemCollection.InsertOneAsync(newMenuItem);

        public async Task UpdateAsync(string id, MenuItem updatedMenuItem) =>
            await _menuItemCollection.ReplaceOneAsync(x => x.Id == id, updatedMenuItem);

        public async Task RemoveAsync(string id) =>
            await _menuItemCollection.DeleteOneAsync(x=> x.Id == id);

        public async Task<LinkedList<MenuCategory>> GetCategoriesAsync()
        {
            var matchBuilder = Builders<MenuItem>.Filter.Empty;
            //var sortBuilder = Builders<MenuItem>.Sort.Ascending(m => m.Category);
            //var pipeline = new EmptyPipelineDefinition<MenuItem>()
            //    .Match(matchBuilder)
            //    .Sort(sortBuilder)
            //    .Group(m => m.Category, cat => new
            //    {
            //        Value = cat.Key,
            //        //cat.Key you can't do string operations for key
            //    });

            //var categoryAnon = await _menuItemCollection.AggregateAsync(pipeline).Result.ToListAsync();
            //var categoryAnon = await (await _menuItemCollection.AggregateAsync(pipeline)).ToListAsync();
            //var categoryAnon = await _menuItemCollection.Aggregate(pipeline).ToListAsync();

            var categoryAnon = await _menuItemCollection.Distinct(s=>s.Category,matchBuilder).ToListAsync();
            LinkedList<MenuCategory> cat = new LinkedList<MenuCategory>();
            foreach (var category in categoryAnon)
            {
                cat.AddLast(new MenuCategory()
                {
                    Label = category[0] + category.Substring(1).ToLower(),
                    Value = category,
                }) ;
            }
            cat.OrderBy(s => s.Label);
            return cat;
        }
    }
}
