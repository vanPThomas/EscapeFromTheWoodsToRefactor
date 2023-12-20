using EscapeFromTheWoods.Database;
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

        public int RecordID { get; set; }
        public int WoodID { get; set; }
        public List<DBTreeRecord> Trees { get; set; }
    }
}
