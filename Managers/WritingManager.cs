using EscapeFromTheWoods.Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace EscapeFromTheWoods.Managers
{
    public class WritingManager
    {
        public static void WriteRouteToDB(
            Monkey monkey,
            List<Tree> route,
            int woodID,
            MongoDBMonkeyRepo db,
            Wood wood
        )
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");
            List<DBTreeRecord> routeTreeRecord = new List<DBTreeRecord>();
            foreach (Tree t in route)
            {
                routeTreeRecord.Add(new DBTreeRecord(t.treeID, t.x, t.y));
            }
            List<DBTreeRecord> treesInWoodRecord = new List<DBTreeRecord>();
            foreach (Tree t in wood.trees)
            {
                treesInWoodRecord.Add(new DBTreeRecord(t.treeID, t.x, t.y));
            }
            DBWoodRecord woodRecord = new DBWoodRecord(wood.woodID, treesInWoodRecord);
            DBRouteRecord routeRecord = new DBRouteRecord(
                routeTreeRecord,
                new DBMonkeyRecord(monkey.monkeyID, monkey.name),
                woodRecord
            );
            db.WriteRoute(routeRecord);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }

        public static void WriteEscaperoutesToBitmap(
            List<List<Tree>> routes,
            int woodID,
            Map map,
            List<Tree> trees,
            int drawingFactor,
            string path
        )
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");
            Color[] cvalues = new Color[]
            {
                // Color.Red,
                Color.Yellow,
                //Color.Blue,
                //Color.Cyan,
                //Color.GreenYellow
            };
            Bitmap bm = new Bitmap(
                (map.xmax - map.xmin) * drawingFactor,
                (map.ymax - map.ymin) * drawingFactor
            );
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.White, 1);
            foreach (Tree t in trees)
            {
                g.DrawEllipse(
                    p,
                    t.x * drawingFactor,
                    t.y * drawingFactor,
                    drawingFactor,
                    drawingFactor
                );
            }
            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(
                    new SolidBrush(color),
                    p1x - delta,
                    p1y - delta,
                    drawingFactor,
                    drawingFactor
                );
                for (int i = 1; i < route.Count; i++)
                {
                    g.DrawLine(
                        pen,
                        p1x,
                        p1y,
                        route[i].x * drawingFactor + delta,
                        route[i].y * drawingFactor + delta
                    );
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }
                colorN++;
            }
            bm.Save(Path.Combine(path, woodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }
    }
}
