using MongoDB.Bson;
using System.Collections.Generic;

namespace EscapeFromTheWoods.Database
{
    public class DBRouteRecord
    {
        public DBRouteRecord(List<DBTreeRecord> route, DBMonkeyRecord monkey, ObjectId woodId)
        {
            Route = route;
            Monkey = monkey;
            WoodId = woodId;
        }

        public ObjectId WoodId { get; set; }

        //public int WoodId { get; set; }
        public List<DBTreeRecord> Route { get; set; }
        public DBMonkeyRecord Monkey { get; set; }
    }
}
