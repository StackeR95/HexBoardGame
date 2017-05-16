using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HexGame
{

    public class Connection
    {
        public PriorityQueue Higher; // Upper or Left
        public PriorityQueue Lower; // Lower or Right
        char C;
        public Connection(char C)
        {
            this.C = C;
            if (C == 'R') { Higher = new PriorityQueue(1); Lower = new PriorityQueue(2); }
            else { Higher = new PriorityQueue(3); Lower = new PriorityQueue(4); }
        }

        public void Copy(Connection ne)
        {
            this.Higher.Copy(ne.Higher);
            this.Lower.Copy(ne.Lower);
        }
        public void Update(Pair ne)
        {
            Lower.Enqueue(ne);
            Higher.Enqueue(ne);
        }
        public void Clear()
        {
            this.Higher.data.Clear();
            this.Lower.data.Clear();
        }
    }
}
