using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using EscapeFromTheWoods.Database;
using EscapeFromTheWoods.Objects;
using System.Diagnostics;

namespace EscapeFromTheWoods
{
    public class Wood
    {
        private const int drawingFactor = 8;
        private string path;
        private MongoDBMonkeyRepo db;
        private Random r = new Random(1);
        public int woodID { get; set; }
        public List<Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;
        private GridWood gridWood;

        public Wood(int woodID, List<Tree> trees, Map map, string path, MongoDBMonkeyRepo db)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
            this.db = db;
            gridWood = new GridWood(map, 50, trees);
        }

        public void PlaceMonkey(string monkeyName, int monkeyID)
        {
            int treeNr;
            do
            {
                treeNr = r.Next(0, trees.Count - 1);
            } while (trees[treeNr].hasMonkey);
            Monkey m = new Monkey(monkeyID, monkeyName, trees[treeNr]);
            monkeys.Add(m);
            trees[treeNr].hasMonkey = true;
        }

        public void Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in monkeys)
            {
                routes.Add(EscapeMonkey(m));
            }
            WriteEscaperoutesToBitmap(routes);
        }

        public List<Tree> EscapeMonkey(Monkey monkey)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");
            Dictionary<int, bool> visited = new Dictionary<int, bool>();
            trees.ForEach(tree => visited.Add(tree.treeID, false));
            List<Tree> route = new List<Tree>() { monkey.tree };
            do
            {
                visited[monkey.tree.treeID] = true;
                SortedList<double, List<Tree>> distanceToMonkey =
                    new SortedList<double, List<Tree>>(); //list of trees and how far they are from the monkey
                (int i, int j) = FindCell(monkey.tree.x, monkey.tree.y);
                //Look for the closest unvisited tree
                ProcessCell(distanceToMonkey, i, j, monkey.tree.x, monkey.tree.y);
                //foreach (Tree tree in trees)
                //{
                //    if ((!visited[tree.treeID]) && (!tree.hasMonkey))
                //    {
                //        double distance = Math.Sqrt(
                //            Math.Pow(tree.x - monkey.tree.x, 2)
                //                + Math.Pow(tree.y - monkey.tree.y, 2)
                //        );
                //        if (distanceToMonkey.ContainsKey(distance))
                //            distanceToMonkey[distance].Add(tree);
                //        else
                //            distanceToMonkey.Add(distance, new List<Tree>() { tree });
                //    }
                //}
                //distance to border
                //north east south west
                double distanceToBorder = (
                    new List<double>()
                    {
                        map.ymax - monkey.tree.y,
                        map.xmax - monkey.tree.x,
                        monkey.tree.y - map.ymin,
                        monkey.tree.x - map.xmin
                    }
                ).Min();
                if (distanceToMonkey.Count == 0)
                {
                    WriteRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }
                if (distanceToBorder < distanceToMonkey.First().Key)
                {
                    WriteRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
            } while (true);
        }

        private void ProcessRing(
            int i,
            int j,
            int ring,
            SortedList<double, List<Tree>> distanceToMonkey,
            double x,
            double y
        )
        {
            for (int gx = i - ring; gx <= i + ring; gx++)
            {
                int gy = j - ring;
                if (IsValidCell(gx, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y);
                gy = j + ring;
                if (IsValidCell(gy, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y);
            }
            for (int gy = j - ring + 1; gy <= j + ring; gy++)
            {
                int gx = i - ring;
                if (IsValidCell(gx, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y);
                gx = i + ring;
                if (IsValidCell(gx, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y);
            }
        }

        private bool IsValidCell(int i, int j)
        {
            if ((i < 0) || (i >= gridWood.NX))
                return false;
            if ((j < 0) || (j >= gridWood.NY))
                return false;
            return true;
        }

        private (int, int) FindCell(double x, double y)
        {
            int i = (int)((x - gridWood.Map.xmin) / gridWood.SquareSize);
            int j = (int)((y - gridWood.Map.ymin) / gridWood.SquareSize);
            if (i == gridWood.NX)
                j--;
            if (j == gridWood.NY)
                j--;
            return (i, j);
        }

        private void ProcessCell(
            SortedList<double, List<Tree>> distanceToMonkey,
            int i,
            int j,
            double x,
            double y
        )
        {
            foreach (Tree tree in gridWood.GridData[i][j])
            {
                double dsquare = Math.Pow(tree.x - x, 2) + Math.Pow(tree.y - y, 2);
                if (distanceToMonkey.ContainsKey(dsquare))
                    distanceToMonkey[dsquare].Add(tree);
                else
                    distanceToMonkey.Add(dsquare, new List<Tree>() { tree });
            }
        }

        private void WriteRouteToDB(Monkey monkey, List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");
            List<DBMonkeyRecord> records = new List<DBMonkeyRecord>();
            for (int j = 0; j < route.Count; j++)
            {
                records.Add(
                    new DBMonkeyRecord(
                        monkey.monkeyID,
                        monkey.name,
                        woodID,
                        j,
                        route[j].treeID,
                        route[j].x,
                        route[j].y
                    )
                );
            }
            db.WriteMonkeys(records);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }

        public void WriteWoodToDB()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} start");
            List<DBWoodRecord> records = new List<DBWoodRecord>();
            foreach (Tree t in trees)
            {
                records.Add(new DBWoodRecord(woodID, t.treeID, t.x, t.y));
            }
            db.WriteWoods(records);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} end");
        }

        public void WriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");
            Color[] cvalues = new Color[]
            {
                Color.Red,
                Color.Yellow,
                Color.Blue,
                Color.Cyan,
                Color.GreenYellow
            };
            Bitmap bm = new Bitmap(
                (map.xmax - map.xmin) * drawingFactor,
                (map.ymax - map.ymin) * drawingFactor
            );
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);
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
