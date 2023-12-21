using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EscapeFromTheWoods
{
    public class DBMonkeyRecord
    {
        public DBMonkeyRecord(int monkeyID, string monkeyName)
        {
            MonkeyID = monkeyID;
            MonkeyName = monkeyName;
        }

        public int MonkeyID { get; set; }
        public string MonkeyName { get; set; }
    }
}
