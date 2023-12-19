using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Database
{
    public class DBTreeRecord
    {
        public DBTreeRecord(int treeID, int x, int y)
        {
            this.treeID = treeID;
            this.x = x;
            this.y = y;
        }

        public int treeID { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }
}
