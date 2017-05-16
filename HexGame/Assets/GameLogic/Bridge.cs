using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HexGame
{
    public class Bridge
    {
        public Pair Pos;
        public List<Pair> mids;
        public int dir;

        public Bridge(Pair pos, Pair mid1, Pair mid2)
        {
            this.Pos = new Pair(pos.x, pos.y);
            Pair mid12 = new Pair(mid1.x, mid1.y);
            Pair mid22 = new Pair(mid2.x, mid2.y);
            this.mids = new List<Pair>();
            this.mids.Add(mid12);
            this.mids.Add(mid22);
        }
    }

}
