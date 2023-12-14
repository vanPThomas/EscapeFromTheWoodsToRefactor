using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Objects
{
    public class GridWood
    {
        public double SquareSize { get; private set; } //size of a single square
        public int NX { get; private set; } // number of squares on x-axis
        public int NY { get; private set; } // number of squares on y-axis
        public Map Map { get; private set; }
        public List<Tree>[][] GridData { get; private set; }

        public GridWood(Map map, double squareSize)
        {
            SquareSize = squareSize;
            Map = map;
            NX = (int)(map.xmax / squareSize) + 1;
            NY = (int)(map.ymax / squareSize) + 1;
            GridData = new List<Tree>[NX][];
            for (int i = 0; i < NX; i++)
            {
                GridData[i] = new List<Tree>[NY];
                for (int j = 0; j < NY; j++)
                    GridData[i][j] = new List<Tree>();
            }
        }

        public GridWood(Map map, double squareSize, List<Tree> trees)
            : this(map, squareSize)
        {
            foreach (Tree tree in trees)
                AddTree(tree);
        }

        public void AddTree(Tree tree)
        {
            int i = (int)((tree.x - Map.xmin) / SquareSize);
            int j = (int)((tree.y - Map.ymin) / SquareSize);
            if (i == NX)
                i--;
            if (j == NY)
                j--;
            GridData[i][j].Add(tree);
        }
    }
}
