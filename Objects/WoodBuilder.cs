﻿using EscapeFromTheWoods.Database;
using System;
using System.Collections.Generic;

namespace EscapeFromTheWoods
{
    public static class WoodBuilder
    {
        public static Wood GetWood(int size, Map map, string path)
        {
            // create wood object
            Random r = new Random(100);
            List<Tree> trees = new List<Tree>();
            int n = 0;
            while (n < size)
            {
                Tree t = new Tree(
                    IDgenerator.GetTreeID(),
                    r.Next(map.xmin, map.xmax),
                    r.Next(map.ymin, map.ymax)
                );
                if (!trees.Contains(t))
                {
                    trees.Add(t);
                    n++;
                }
            }
            Wood w = new Wood(IDgenerator.GetWoodID(), trees, map, path);
            return w;
        }
    }
}
