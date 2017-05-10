using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (C == 'R')
            {
                if (ne.x >= 5) Lower.Enqueue(ne);
                if (ne.x <= 5) Higher.Enqueue(ne);
            }
            else
            {
                if (ne.y >= 5) Lower.Enqueue(ne);
                if (ne.y <= 5) Higher.Enqueue(ne); 
            }
        }
    }
}
