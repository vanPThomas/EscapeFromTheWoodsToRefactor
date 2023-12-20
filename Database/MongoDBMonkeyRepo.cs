using MongoDB.Driver;
using System.Collections.Generic;

namespace EscapeFromTheWoods.Database
{
    public class MongoDBMonkeyRepo
    {
        private IMongoClient dbClient;
        private IMongoDatabase database;
        private string connectionString;

        public MongoDBMonkeyRepo(string connectionString)
        {
            this.connectionString = connectionString;
            dbClient = new MongoClient(connectionString);
            database = dbClient.GetDatabase("MonkeyDB");
        }

        public void WriteMonkeys(List<DBMonkeyRecord> monkeys)
        {
            var collection = database.GetCollection<DBMonkeyRecord>("monkeys");
            collection.InsertMany(monkeys);
        }

        public void WriteWoods(List<DBWoodRecord> woods)
        {
            var collection = database.GetCollection<DBWoodRecord>("woods");
            collection.InsertMany(woods);
        }

        public void WriteRoute(DBRouteRecord route)
        {
            var collection = database.GetCollection<DBRouteRecord>("route");
            collection.InsertOne(route);
        }
    }
}
