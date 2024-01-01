namespace MongoDbOrdersAPI.Data
{
    public class OrderSystemDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string MenuCollectionName { get; set; } = null!;

        public string OrderCollectionName { get; set; } = null!;
    }
}
