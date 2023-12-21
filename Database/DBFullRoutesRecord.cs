using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EscapeFromTheWoods.Database
{
    public class DBFullRoutesRecord
    {
        public ObjectId woodId { get; set; }

        public List<DBMonkeyRecord> monkeyRecords { get; set; } = new List<DBMonkeyRecord>();
        public List<List<DBTreeRecord>> treeRecords { get; set; } = new List<List<DBTreeRecord>>();
    }
}
