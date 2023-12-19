using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Database
{
    public class DBRouteRecord
    {
        public DBRouteRecord(List<DBTreeRecord> route, DBMonkeyRecord monkey, DBWoodRecord wood)
        {
            Route = route;
            Monkey = monkey;
            Wood = wood;
        }

        public List<DBTreeRecord> Route { get; set; }
        public DBMonkeyRecord Monkey { get; set; }
        public DBWoodRecord Wood { get; set; }
    }
}
