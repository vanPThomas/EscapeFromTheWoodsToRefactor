using EscapeFromTheWoods.Database;
using EscapeFromTheWoods.Managers;
using EscapeFromTheWoods.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EscapeFromTheWoods
{
    public class Wood
    {
        private const int drawingFactor = 8;
        private string path;
        public MongoDBMonkeyRepo db { get; set; }
        private Random r = new Random(1);
        public int woodID { get; set; }
        public List<Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;
        private GridWood gridWood;
        private Dictionary<int, bool> visited;

        public Wood(int woodID, List<Tree> trees, Map map, string path, MongoDBMonkeyRepo db)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
            this.db = db;
            gridWood = new GridWood(map, 100, trees);
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
                routes.Add(EscapeMonkey(m, 1));
            }
            WritingManager.WriteEscaperoutesToBitmap(
                routes,
                woodID,
                map,
                trees,
                drawingFactor,
                path
            );
        }

        public List<Tree> EscapeMonkey(Monkey monkey, int n)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");

            // create a library of the trees and check if they're visited
            visited = new Dictionary<int, bool>();
            trees.ForEach(tree => visited.Add(tree.treeID, false));

            // create route starting at the monkey location
            List<Tree> route = new List<Tree>() { monkey.tree };
            do
            {
                visited[monkey.tree.treeID] = true;
                SortedList<double, List<Tree>> distanceToMonkey =
                    new SortedList<double, List<Tree>>(); //list of trees and how far they are from the monkey
                (int i, int j) = FindCell(monkey.tree.x, monkey.tree.y);
                //check the trees in the current grid
                ProcessCell(distanceToMonkey, i, j, monkey.tree.x, monkey.tree.y, n);
                int ring = 0;
                // nothing in distance to monkey yet? process the ring around it
                while (distanceToMonkey.Count < n)
                {
                    ring++;
                    ProcessRing(i, j, ring, distanceToMonkey, monkey.tree.x, monkey.tree.y, n);
                }

                int n_ring = 1;
                if (ring > 0)
                    n_ring = (int)Math.Ceiling(Math.Sqrt(2) * ring) - ring;
                for (int extraRings = 1; extraRings <= n_ring; extraRings++)
                {
                    ProcessRing(
                        i,
                        j,
                        ring + extraRings,
                        distanceToMonkey,
                        monkey.tree.x,
                        monkey.tree.y,
                        n
                    );
                }
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
                    WritingManager.WriteRouteToDB(monkey, route, woodID, db, this);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }
                if (distanceToBorder < Math.Sqrt(distanceToMonkey.First().Key))
                {
                    WritingManager.WriteRouteToDB(monkey, route, woodID, db, this);
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
            double y,
            int n
        )
        {
            for (int gx = i - ring; gx <= i + ring; gx++)
            {
                //onderste rij
                int gy = j - ring;
                if (IsValidCell(gx, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n);
                //bovenste rij
                gy = j + ring;
                if (IsValidCell(gx, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n);
            }
            for (int gy = j - ring + 1; gy <= j + ring - 1; gy++)
            {
                //linker kolom
                int gx = i - ring;
                if (IsValidCell(gx, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n);
                // rechter kolom
                gx = i + ring;
                if (IsValidCell(gx, gy))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n);
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
            double y,
            int n
        )
        {
            foreach (Tree tree in gridWood.GridData[i][j])
            {
                if (!visited[tree.treeID])
                {
                    double dsquare = Math.Pow(tree.x - x, 2) + Math.Pow(tree.y - y, 2);
                    if (
                        (distanceToMonkey.Count < n)
                        || (dsquare < distanceToMonkey.Keys[distanceToMonkey.Count - 1])
                    )
                    {
                        if (distanceToMonkey.ContainsKey(dsquare))
                            distanceToMonkey[dsquare].Add(tree);
                        else
                            distanceToMonkey.Add(dsquare, new List<Tree>() { tree });
                    }
                }
            }
        }
    }
}
