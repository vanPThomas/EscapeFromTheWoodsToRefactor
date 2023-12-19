using EscapeFromTheWoods.Database;
using System;
using System.Collections.Generic;
using System.Text;

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
