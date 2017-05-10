using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGame
{
    public class PriorityQueue
    {
        public List<Pair> data;
        private int pos; //1 ==> upper, 2 ==> lower, 3 ==> left, 4 ==> right
        public HashSet<Pair> Prev;

        public PriorityQueue(int pos)
        {
            this.data = new List<Pair>();
            this.pos = pos;
            this.Prev = new HashSet<Pair>();
        }
        public void Copy(PriorityQueue cp)
        {
            if (cp.data.Count > 100)
                System.Console.WriteLine("dsjbfb");
            for (int i = 0; i < cp.data.Count; i++)
            {
                Pair x = new Pair(cp.data[i].x, cp.data[i].y);
                this.data.Add(x);
                this.Prev.Add(x);
            }
            
        }
        public void Enqueue(Pair item)
        {
            if (Prev.Contains(item)) return;
            Prev.Add(item);
            data.Add(item);
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (data[ci].CompareTo(data[pi],pos) <= 0) break; // child item is smaller than (or equal) parent so we're done
                Pair tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        public Pair Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = data.Count - 1; // last index (before removal)
            Pair frontItem = data[0];   // fetch the front
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
            Pair frontItem = data[0];
            return frontItem;
        }

        public int Count()
        {
            return data.Count;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < data.Count; ++i)
                s += data[i].ToString() + " ";
            s += "count = " + data.Count;
            return s;
        }

        public bool IsConsistent()
        {
            // is the heap property true for all data?
            if (data.Count == 0) return true;
            int li = data.Count - 1; // last index
            for (int pi = 0; pi < data.Count; ++pi) // each parent index
            {
                int lci = 2 * pi + 1; // left child index
                int rci = 2 * pi + 2; // right child index

                if (lci <= li && data[pi].CompareTo(data[lci], pos) < 0) return false; // if lc exists and it's smaller than parent then bad.
                if (rci <= li && data[pi].CompareTo(data[rci], pos) < 0) return false; // check the right child too.
            }
            return true; // passed all checks
        } // IsConsistent
    } // PriorityQueue
}
