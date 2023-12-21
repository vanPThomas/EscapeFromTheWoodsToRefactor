using EscapeFromTheWoods.Database;
using MongoDB.Bson;
using System.Collections.Generic;

namespace EscapeFromTheWoods
{
    public class DBWoodRecord
    {
        public DBWoodRecord(int woodID, List<DBTreeRecord> trees)
        {
            WoodID = woodID;
            Trees = trees;
        }

        public int WoodID { get; set; }
        public ObjectId _id { get; set; }
        public List<DBTreeRecord> Trees { get; set; }
    }
}
