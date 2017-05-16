using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HexGame
{
    public class Pair
    {
        public int x, y;
        public Pair(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int CompareTo(Pair RHS, int pos)
        {
            if (pos == 1) return RHS.x - this.x;
            else if (pos == 2) return this.x - RHS.x;
            else if (pos == 3) return RHS.y - this.y;
            else return this.y - RHS.y;
        }
    }
}
