using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGame
{
    public class DSU
    {
        public char C;
        public List<Connection> Connections;
        public int ConCount;
        public int[] par, size, con;

        public DSU(char C)
        {
            this.C = C;
            
            par = new int[11 * 11];
            size = new int[11 * 11];
            con = new int[11 * 11];

            for (int i = 0; i < 11 * 11; ++i)
            {
                par[i] = i;
                size[i] = 1;
                con[i] = -1;
            }

            ConCount = 0;
            Connections = new List<Connection>();
        }
        public void Copy(DSU ME)
        {
            this.C = ME.C;
            for (int i = 0; i < 11 * 11; ++i)
            {
                par[i] = ME.par[i];
                size[i] = ME.size[i];
                con[i] = ME.con[i];
            }
            ConCount = ME.ConCount;

            this.Connections = new List<Connection>();

            for (int i = 0; i < ConCount; i++)
            {
                Connection add = new Connection(this.C);
                add.Copy(ME.Connections[i]);
                this.Connections.Add(add);
            }
        }
        public void update(Pair ne, State S)
        {

            con[(ne.x * 11) + ne.y] = ConCount++;
            Connections.Add(new Connection(C));
            Connections[con[(ne.x * 11) + ne.y]].Update(ne);

            List<Pair> adj = GetAdjacentOfSameColor(ne, S);
            List<Bridge> brd = PointBridgesOfSameColor(ne, S);
            Board x = new Board();

            for (int i = 0; i < adj.Count; i++)
            {
                int par1 = FindPar((adj[i].x * 11) + adj[i].y);
                int par2 = FindPar((ne.x * 11) + ne.y);
                if (con[par1] == con[par2] && par1 != par2)
                    Console.WriteLine("ya5taaaaay");
                join((adj[i].x * 11) + adj[i].y, (ne.x * 11) + ne.y);
            }
            for (int i = 0; i < brd.Count; i++)
            {
                int par1 = FindPar((brd[i].Pos.x * 11) + brd[i].Pos.y);
                int par2 = FindPar((ne.x * 11) + ne.y);
                if (con[par1] == con[par2] && par1 != par2)
                    Console.WriteLine("ya5taaaaay");
                join((brd[i].Pos.x * 11) + brd[i].Pos.y, (ne.x * 11) + ne.y);
            }
        }
        private void join(int firs, int secs)
        {
            int fir = FindPar(firs);
            int sec = FindPar(secs);
            if (con[sec] >= ConCount || con[fir] >= ConCount)
                System.Console.WriteLine("yade el nela");
            if (fir == sec) return;
            if (size[fir] < size[sec]) { int temp = fir; fir = sec; sec = temp; }
            size[fir] += size[sec];
            par[sec] = fir;
            if (con[fir] == con[sec])
                Console.WriteLine("ya5taaaaay");
            
            Connections[con[fir]].Copy(Connections[con[sec]]);
            
            Connections[con[sec]].Clear();
            
            con[sec] = con[fir];
        }
        private int FindPar(int i)
        {
            if(i == par[i])return i; 
            return par[i] = FindPar(par[i]);
        }
        private List<Pair> GetAdjacentOfSameColor(Pair P, State s)
        {
            int i = P.x;
            int j = P.y;
            List<Pair> mylist = new List<Pair>();
            if (i - 1 >= 0 && s.BoardCell[i - 1, j].OccupiedBy == C) mylist.Add(new Pair(i - 1, j));
            if (i - 1 >= 0 && j + 1 <= 10 && s.BoardCell[i - 1, j + 1].OccupiedBy == C) mylist.Add(new Pair(i - 1, j + 1));
            if (j + 1 <= 10 && s.BoardCell[i, j + 1].OccupiedBy == C) mylist.Add(new Pair(i, j + 1));
            if (i + 1 <= 10 && s.BoardCell[i + 1, j].OccupiedBy == C) mylist.Add(new Pair(i + 1, j));
            if (i + 1 <= 10 && j - 1 >= 0 && s.BoardCell[i + 1, j - 1].OccupiedBy == C) mylist.Add(new Pair(i + 1, j - 1));
            if (j - 1 >= 0 && s.BoardCell[i, j - 1].OccupiedBy == C) mylist.Add(new Pair(i, j - 1));

            return mylist;


        }
        private List<Bridge> PointBridgesOfSameColor(Pair Po, State s)
        {
            List<Bridge> Bridges = new List<Bridge>();
            int x = Po.x;
            int y = Po.y;

            Bridge X1;
            if (x - 1 >= 0 && y - 1 >= 0 && s.BoardCell[x - 1, y - 1].OccupiedBy == C)
            {
                X1 = new Bridge(new Pair(x - 1, y - 1), new Pair(x - 1, y), new Pair(x, y - 1));
                if (s.BoardCell[X1.mids[0].x, X1.mids[0].y].OccupiedBy == 'N' && s.BoardCell[X1.mids[1].x, X1.mids[1].y].OccupiedBy == 'N')
                    Bridges.Add(X1);
            }
            if (x - 2 >= 0 && x - 1 >= 0 && y + 1 <= 10 && s.BoardCell[x - 2, y + 1].OccupiedBy == C)
            {
                X1 = new Bridge(new Pair(x - 2, y + 1), new Pair(x - 1, y), new Pair(x - 1, y + 1));
                if (s.BoardCell[X1.mids[0].x, X1.mids[0].y].OccupiedBy == 'N' && s.BoardCell[X1.mids[1].x, X1.mids[1].y].OccupiedBy == 'N')
                    Bridges.Add(X1);
            }
            if (x - 1 >= 0 && y + 2 <= 10 && y + 1 <= 10 && s.BoardCell[x - 1, y + 2].OccupiedBy == C)
            {
                X1 = new Bridge(new Pair(x - 1, y + 2), new Pair(x - 1, y + 1), new Pair(x, y + 1));
                if (s.BoardCell[X1.mids[0].x, X1.mids[0].y].OccupiedBy == 'N' && s.BoardCell[X1.mids[1].x, X1.mids[1].y].OccupiedBy == 'N')
                    Bridges.Add(X1);
            }
            if (x + 1 <= 10 && y + 1 <= 10 && s.BoardCell[x + 1, y + 1].OccupiedBy == C)
            {
                X1 = new Bridge(new Pair(x + 1, y + 1), new Pair(x, y + 1), new Pair(x + 1, y));
                if (s.BoardCell[X1.mids[0].x, X1.mids[0].y].OccupiedBy == 'N' && s.BoardCell[X1.mids[1].x, X1.mids[1].y].OccupiedBy == 'N')
                    Bridges.Add(X1);
            }
            if (x + 2 <= 10 && x + 1 <= 10 && y - 1 >= 0 && s.BoardCell[x + 2, y - 1].OccupiedBy == C)
            {
                X1 = new Bridge(new Pair(x + 2, y - 1), new Pair(x + 1, y), new Pair(x + 1, y - 1));
                if (s.BoardCell[X1.mids[0].x, X1.mids[0].y].OccupiedBy == 'N' && s.BoardCell[X1.mids[1].x, X1.mids[1].y].OccupiedBy == 'N')
                    Bridges.Add(X1);
            }
            if (x + 1 <= 10 && y - 1 >= 0 && y - 2 >= 0 && s.BoardCell[x + 1, y - 2].OccupiedBy == C)
            {
                X1 = new Bridge(new Pair(x + 1, y - 2), new Pair(x + 1, y - 1), new Pair(x, y - 1));
                if (s.BoardCell[X1.mids[0].x, X1.mids[0].y].OccupiedBy == 'N' && s.BoardCell[X1.mids[1].x, X1.mids[1].y].OccupiedBy == 'N')
                    Bridges.Add(X1);
            }

            return Bridges;

        }
    }
}
