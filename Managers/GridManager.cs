using EscapeFromTheWoods.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Managers
{
    public class GridManager
    {
        public static void ProcessRing(
            int i,
            int j,
            int ring,
            SortedList<double, List<Tree>> distanceToMonkey,
            double x,
            double y,
            int n,
            GridWood gridWood,
            Dictionary<int, bool> visited
        )
        {
            for (int gx = i - ring; gx <= i + ring; gx++)
            {
                //onderste rij
                int gy = j - ring;
                if (IsValidCell(gx, gy, gridWood))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n, gridWood, visited);
                //bovenste rij
                gy = j + ring;
                if (IsValidCell(gx, gy, gridWood))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n, gridWood, visited);
            }
            for (int gy = j - ring + 1; gy <= j + ring - 1; gy++)
            {
                //linker kolom
                int gx = i - ring;
                if (IsValidCell(gx, gy, gridWood))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n, gridWood, visited);
                // rechter kolom
                gx = i + ring;
                if (IsValidCell(gx, gy, gridWood))
                    ProcessCell(distanceToMonkey, gx, gy, x, y, n, gridWood, visited);
            }
        }

        public static bool IsValidCell(int i, int j, GridWood gridWood)
        {
            if ((i < 0) || (i >= gridWood.NX))
                return false;
            if ((j < 0) || (j >= gridWood.NY))
                return false;
            return true;
        }

        public static (int, int) FindCell(double x, double y, GridWood gridWood)
        {
            int i = (int)((x - gridWood.Map.xmin) / gridWood.SquareSize);
            int j = (int)((y - gridWood.Map.ymin) / gridWood.SquareSize);
            if (i == gridWood.NX)
                j--;
            if (j == gridWood.NY)
                j--;
            return (i, j);
        }

        public static void ProcessCell(
            SortedList<double, List<Tree>> distanceToMonkey,
            int i,
            int j,
            double x,
            double y,
            int n,
            GridWood gridWood,
            Dictionary<int, bool> visited
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
