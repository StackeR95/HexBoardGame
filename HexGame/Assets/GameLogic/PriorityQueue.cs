using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HexGame
{
    public class PriorityQueue
    {
        public List<Pair> data;
        private int pos; //1 ==> upper, 2 ==> lower, 3 ==> left, 4 ==> right
        public bool[] Prev;
        public PriorityQueue(int pos)
        {
            this.data = new List<Pair>();
            this.pos = pos;
            Prev = new bool[11 * 11];
        }
        public void Copy(PriorityQueue cp)
        {
            for (int i = 0; i < cp.data.Count; i++)
            {
                this.Enqueue(cp.data[i]);
            }
        }
        public void Enqueue(Pair item)
        {
            if (Prev[item.x * 11 + item.y]) return;
            
            Prev[item.x * 11 + item.y] = true;
            data.Add(new Pair(item.x,item.y));
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (data[ci].CompareTo(data[pi], pos) <= 0) break; // child item is smaller than (or equal) parent so we're done
                Pair tmp = new Pair(data[ci].x, data[ci].y); data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        public Pair Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = data.Count - 1; // last index (before removal)
            Pair frontItem = new Pair(data[0].x, data[0].y);   // fetch the front
            Prev[data[0].x * 11 + data[0].y] = false;
            data[0] = data[li];
            data.RemoveAt(li);

            --li; // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li) break;  // no children so done
                int rc = ci + 1;     // right child
                if (rc <= li && data[rc].CompareTo(data[ci], pos) > 0) // if there is a rc (ci + 1), and it is larger than left child, use the rc instead
                    ci = rc;
                if (data[pi].CompareTo(data[ci], pos) >= 0) break; // parent is larger than (or equal to) largest child so done
                Pair tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
                pi = ci;
            }
            return frontItem;
        }

        public Pair Peek()
        {
            Pair frontItem = new Pair(data[0].x, data[0].y);
            return frontItem;
        }

        public int Count()
        {
            return data.Count;
        }



    } // PriorityQueue
}
