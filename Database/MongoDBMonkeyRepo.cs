using MongoDB.Bson;
using MongoDB.Driver;
using System;
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

        public ObjectId WriteWoods(DBWoodRecord wood)
        {
            var collection = database.GetCollection<DBWoodRecord>("woods");
            collection.InsertOne(wood);
            ObjectId generatedId = wood._id;
            return generatedId;
        }

        public void WriteRoute(DBRouteRecord route)
        {
            var collection = database.GetCollection<DBRouteRecord>("route");
            collection.InsertOne(route);
        }

        public void WriteFullRoute(DBFullRoutesRecord fullRouteRecord)
        {
            var collection = database.GetCollection<DBFullRoutesRecord>("route");
            collection.InsertOne(fullRouteRecord);
        }
    }
}
