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
        private Random r = new Random(1);
        public int woodID { get; set; }
        public List<Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;
        private GridWood gridWood;
        private Dictionary<int, bool> visited;

        public Wood(int woodID, List<Tree> trees, Map map, string path)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
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

        public Dictionary<Monkey, List<Tree>> Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            Dictionary<Monkey, List<Tree>> monkeyToRoute = new Dictionary<Monkey, List<Tree>>();
            foreach (Monkey m in monkeys)
            {
                routes.Add(EscapeMonkey(m, 1));
                monkeyToRoute.Add(m, EscapeMonkey(m, 2));
            }
            WritingManager.WriteEscaperoutesToBitmap(
                routes,
                woodID,
                map,
                trees,
                drawingFactor,
                path
            );
            return monkeyToRoute;
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
                (int i, int j) = GridManager.FindCell(monkey.tree.x, monkey.tree.y, gridWood);
                //check the trees in the current grid
                GridManager.ProcessCell(
                    distanceToMonkey,
                    i,
                    j,
                    monkey.tree.x,
                    monkey.tree.y,
                    n,
                    gridWood,
                    visited
                );
                int ring = 0;
                // nothing in distance to monkey yet? process the ring around it
                while (distanceToMonkey.Count < n)
                {
                    ring++;
                    GridManager.ProcessRing(
                        i,
                        j,
                        ring,
                        distanceToMonkey,
                        monkey.tree.x,
                        monkey.tree.y,
                        n,
                        gridWood,
                        visited
                    );
                }

                int n_ring = 1;
                if (ring > 0)
                    n_ring = (int)Math.Ceiling(Math.Sqrt(2) * ring) - ring;
                for (int extraRings = 1; extraRings <= n_ring; extraRings++)
                {
                    GridManager.ProcessRing(
                        i,
                        j,
                        ring + extraRings,
                        distanceToMonkey,
                        monkey.tree.x,
                        monkey.tree.y,
                        n,
                        gridWood,
                        visited
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

                    return route;
                }
                if (distanceToBorder < Math.Sqrt(distanceToMonkey.First().Key))
                {
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
            } while (true);
        }
    }
}
