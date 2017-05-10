using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
namespace HexGame
{
    //--------------------------------------------------------UTILITY CLASSES-------------------------------------------------------------//
    public class State
    {
        public Cell[,] BoardCell; //2d array of cells, instance of board
        public int[] cord; //most-recently occupied cells
        public int OccCells;
     
        
        
        public State()
        {
            OccCells = 0;
            cord = new int[2];
            BoardCell = new Cell[11, 11];
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    BoardCell[i, j] = new Cell(i, j, 'N', 1);
                }
            }
         
        }
        public void CopyState(State s)
        {
            OccCells = s.OccCells;
          
            cord[0] = s.cord[0];
            cord[1] = s.cord[1];

            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    BoardCell[i, j].CorX = s.BoardCell[i, j].CorX;
                    BoardCell[i, j].CorY = s.BoardCell[i, j].CorY;
                    BoardCell[i, j].OccupiedBy = s.BoardCell[i, j].OccupiedBy;
                    BoardCell[i, j].flag = s.BoardCell[i, j].flag;
                }
            }
        }

    };

    public class Cell
    {
        public int CorX;
        public int CorY;
        public char OccupiedBy; //Color of player occuping the cells
        public int flag;

        public Cell(int x, int y, char P, int f)
        {
            CorX = x;
            CorY = y;
            OccupiedBy = P;
            flag = f;
        }
    }

    public class Player
    {
        public char Color; //R:Red or B:Blue
        public int num; //Player number
        public int NumofCellsPlayed; //No. of cells player has played
        public Cell[] PlayerCells; //Cells the player played
        public List<Cell> Buffer;

        public DSU SetOfConnections;

        public Player(char C, int N)
        {
            Color = C;
            num = N;
            NumofCellsPlayed = 0;
            PlayerCells = new Cell[62];
            Buffer = new List<Cell>();
            //SetOfConnections = new DSU(C);
        }

        public void CopyPlayer(Player p)
        {
            Buffer = new List<Cell>();
            Color = p.Color;
            num = p.num;
            NumofCellsPlayed = p.NumofCellsPlayed;
            PlayerCells = new Cell[62];

            //SetOfConnections.Copy(p.SetOfConnections);
            
            for (int i = 0; i < p.Buffer.Count; i++)
            {
                Cell c = new Cell(p.Buffer[i].CorX, p.Buffer[i].CorY, p.Buffer[i].OccupiedBy, p.Buffer[i].flag);
                Buffer.Add(c);
            }
            for (int i = 0; i < p.NumofCellsPlayed; i++)
            {
                Cell c = new Cell(p.PlayerCells[i].CorX, p.PlayerCells[i].CorY, p.PlayerCells[i].OccupiedBy, p.PlayerCells[i].flag);
                PlayerCells[i] = c;
            }

        }
        public void newplay(Cell C, State MyStat)
        {
            this.Buffer.Add(C);
            this.NumofCellsPlayed++;
            this.PlayerCells[this.NumofCellsPlayed - 1] = C;
            //SetOfConnections.update(new Pair(C.CorX, C.CorY), MyStat);
        }
    }
    //------------------------------------------------------------------------------------------------------------------------------------//
    public class Board
    {
        public Player P1; //Player 1 representing me (my agent)
        public Player P2; //Player 2 representing the other player
        public State HexBoard; //My main game board
        public int swappedflag = 0; //If swapped applied

        //------------------------------------------------Utitlity Functions of Board--------------------------------------------------------//
        //Only used implicitly in Board functions, aren't used by anything outside the board
        private int CheckNeighbours(Cell c, ref Player P, ref State s, ref stack MySt)
        {
            int i = c.CorX;
            int j = c.CorY;
            int value = 0;
            int PopCounter = 0;
            if ((P.Color == 'R' && i == 10) || (P.Color == 'B' && j == 10)) return 24;
            List<Cell> temp = new List<Cell>();
            if (i - 1 >= 0 && s.BoardCell[i - 1, j].OccupiedBy == P.Color && s.BoardCell[i - 1, j].flag == 1)
            {
                temp.Add(s.BoardCell[i - 1, j]);
                s.BoardCell[i - 1, j].flag = 0;
                PopCounter++;
            }
            if (i - 1 >= 0 && j + 1 <= 10 && s.BoardCell[i - 1, j + 1].OccupiedBy == P.Color && s.BoardCell[i - 1, j + 1].flag == 1)
            {
                temp.Add(s.BoardCell[i - 1, j + 1]);
                s.BoardCell[i - 1, j + 1].flag = 0;
                PopCounter++;
            }
            if (j + 1 <= 10 && s.BoardCell[i, j + 1].OccupiedBy == P.Color && s.BoardCell[i, j + 1].flag == 1)
            {
                temp.Add(s.BoardCell[i, j + 1]);
                s.BoardCell[i, j + 1].flag = 0;
                PopCounter++;
            }
            if (i + 1 <= 10 && s.BoardCell[i + 1, j].OccupiedBy == P.Color && s.BoardCell[i + 1, j].flag == 1)
            {
                temp.Add(s.BoardCell[i + 1, j]);
                s.BoardCell[i + 1, j].flag = 0;
                PopCounter++;
            }
            if (i + 1 <= 10 && j - 1 >= 0 && s.BoardCell[i + 1, j - 1].OccupiedBy == P.Color && s.BoardCell[i + 1, j - 1].flag == 1)
            {
                temp.Add(s.BoardCell[i + 1, j - 1]);
                s.BoardCell[i + 1, j - 1].flag = 0;
                PopCounter++;
            }
            if (j - 1 >= 0 && s.BoardCell[i, j - 1].OccupiedBy == P.Color && s.BoardCell[i, j - 1].flag == 1)
            {
                temp.Add(s.BoardCell[i, j - 1]);
                s.BoardCell[i, j - 1].flag = 0;
                PopCounter++;
            }

            foreach (Cell x in temp)
            {
                if ((x.CorX == 10 && P.Color == 'R') || (x.CorY == 10 && P.Color == 'B'))
                    return 24;
                MySt.Push(x);
                value = 1;
            }

            while (PopCounter > 0)
            {
                PopCounter--;
                if (CheckNeighbours(MySt.C[MySt.top], ref P, ref s, ref MySt) == 24)
                {
                    while (MySt.top > 0)
                        MySt.Pop();
                    return 24; ;
                }
                MySt.Pop();
            }
            return value; //found Neighbours or not
        }



        public int DoesAnyOfTheNeighborsHasAZeroFlag(Cell c, ref Player P, ref State s)
        {
            int i = c.CorX;
            int j = c.CorY;
            if (i - 1 >= 0 && s.BoardCell[i - 1, j].flag == 0 && s.BoardCell[i - 1, j].OccupiedBy == P.Color) return 1;
            else if (i - 1 >= 0 && j + 1 <= 10 && s.BoardCell[i - 1, j + 1].flag == 0 && s.BoardCell[i - 1, j + 1].OccupiedBy == P.Color) return 1;
            else if (j + 1 <= 10 && s.BoardCell[i, j + 1].flag == 0 && s.BoardCell[i, j + 1].OccupiedBy == P.Color) return 1;
            else if (i + 1 <= 10 && s.BoardCell[i + 1, j].flag == 0 && s.BoardCell[i + 1, j].OccupiedBy == P.Color) return 1;
            else if (i + 1 <= 10 && j - 1 >= 0 && s.BoardCell[i + 1, j - 1].flag == 0 && s.BoardCell[i + 1, j - 1].OccupiedBy == P.Color) return 1;
            else if (j - 1 >= 0 && s.BoardCell[i, j - 1].flag == 0 && s.BoardCell[i, j - 1].OccupiedBy == P.Color) return 1;
            else
                return 0;
        }
        //-----------------------------------------------------Board Main Functions---------------------------------------------------------//
        public Board()
        {
            HexBoard = new State();

            
        }


        public void SwapAtFirstTurn()
        {

            /////////////////////////////// PRIORITY QUEUES WILL EXCHANGE DATA ////////////////////////////
            int p1num = P1.num;
            int p2num = P2.num;


            Player t = new Player(' ', 0);
            t.CopyPlayer(P2);
            P2.CopyPlayer(P1);
            P1.CopyPlayer(t);

            P1.num = p1num;
            P2.num = p2num;




        }

        public List<Bridge> PointBridges(Pair Po)
        {
            List<Bridge> Bridges = new List<Bridge>();
            int x = Po.x;
            int y = Po.y;

            Bridge X1;
            if (x - 1 >= 0 && y - 1 >= 0)
            {
                X1 = new Bridge(new Pair(x - 1, y - 1), new Pair(x - 1, y), new Pair(x, y - 1));
                Bridges.Add(X1);
            }
            if (x - 2 >= 0 && x - 1 >= 0 && y + 1 <= 10) {
                X1 = new Bridge(new Pair(x - 2, y + 1), new Pair(x - 1, y), new Pair(x - 1, y + 1));
                Bridges.Add(X1);
            }
            if (x - 1 >= 0 && y + 2 <= 10 && y + 1 <= 10)
            {
                X1 = new Bridge(new Pair(x - 1, y + 2), new Pair(x - 1, y + 1), new Pair(x, y + 1));
                Bridges.Add(X1);
            }
            if (x + 1 <= 10 && y + 1 <= 10)
            {
                X1 = new Bridge(new Pair(x + 1, y + 1), new Pair(x, y + 1), new Pair(x + 1, y));
                Bridges.Add(X1);
            }
            if (x + 2 <= 10 && x + 1 <= 10 && y - 1 >= 0)
            {
                X1 = new Bridge(new Pair(x + 2, y - 1), new Pair(x + 1, y), new Pair(x + 1, y - 1));
                Bridges.Add(X1);
            }
            if (x + 1 <= 10 && y - 1 >= 0 && y - 2 >= 0)
            {
                X1 = new Bridge(new Pair(x + 1, y - 2), new Pair(x + 1, y - 1), new Pair(x, y - 1));
                Bridges.Add(X1);
            }

            return Bridges;

        }
        public List<Pair> MustPlay(Player P, State Cur)
        {
            char Opp;
            if (P.Color == 'R') Opp = 'B';
            else Opp = 'R';

            List<Pair> Must = new List<Pair>();
            for (int i = 0; i < P.NumofCellsPlayed; i++)
            {
                List<Bridge> temp = PointBridges(new Pair (P.PlayerCells[i].CorX, P.PlayerCells[i].CorY));
                for (int j = 0; j < temp.Count; j++)
                {
                    if(Cur.BoardCell[temp[j].Pos.x, temp[j].Pos.y].OccupiedBy == P.Color)
                    {
                        if (Cur.BoardCell[temp[j].mids[0].x, temp[j].mids[0].y].OccupiedBy == Opp && Cur.BoardCell[temp[j].mids[1].x, temp[j].mids[1].y].OccupiedBy == 'N')
                            Must.Add(temp[j].mids[1]);
                        else if (Cur.BoardCell[temp[j].mids[1].x, temp[j].mids[1].y].OccupiedBy == Opp && Cur.BoardCell[temp[j].mids[0].x, temp[j].mids[0].y].OccupiedBy == 'N')
                            Must.Add(temp[j].mids[0]);
                    }
                }
            }

            return Must;
        } 

        public List<Bridge> GetBridges(int x, int y, State Cur)
        {
            List<Bridge> Must = new List<Bridge>();
            List<Bridge> temp = PointBridges(new Pair(x, y));
            for (int i = 0; i < temp.Count; i++)
            {
                if (Cur.BoardCell[temp[i].Pos.x, temp[i].Pos.y].OccupiedBy == 'N' && Cur.BoardCell[temp[i].mids[0].x, temp[i].mids[0].y].OccupiedBy == 'N' && Cur.BoardCell[temp[i].mids[1].x, temp[i].mids[1].y].OccupiedBy == 'N')
                {
                    Must.Add(temp[i]);
                }
            }
            return Must;
        }

        public List<Pair> GetAdjacent(Pair P, State s)
        {
            int i = P.x;
            int j = P.y;
            List<Pair> mylist = new List<Pair>();
            if (i - 1 >= 0 && s.BoardCell[i - 1, j].OccupiedBy == 'N') mylist.Add(new Pair(i - 1, j));
            if (i - 1 >= 0 && j + 1 <= 10 && s.BoardCell[i - 1, j + 1].OccupiedBy == 'N') mylist.Add(new Pair(i - 1, j + 1));
            if (j + 1 <= 10 && s.BoardCell[i, j + 1].OccupiedBy == 'N') mylist.Add(new Pair(i, j + 1));
            if (i + 1 <= 10 && s.BoardCell[i + 1, j].OccupiedBy == 'N') mylist.Add(new Pair(i + 1, j));
            if (i + 1 <= 10 && j - 1 >= 0 && s.BoardCell[i + 1, j - 1].OccupiedBy == 'N') mylist.Add(new Pair(i + 1, j - 1));
            if (j - 1 >= 0 && s.BoardCell[i, j - 1].OccupiedBy == 'N') mylist.Add(new Pair(i, j - 1));

            return mylist;


        }
        public List<Pair> LegalPlays(Player Pme, Player Popp,State MyState) //Used in simulation & node expansion
        {

            HashSet<Pair> Prev = new HashSet<Pair>();

            List<Pair> LegalPair = new List<Pair>();

            List<Pair> MustPair = new List<Pair>();

            MustPair = MustPlay(Pme, MyState);

            for (int i = 0; i < MustPair.Count; i++)
            {
                if (!Prev.Contains(MustPair[i]))
                {
                    LegalPair.Add(MustPair[i]);
                    Prev.Add(MustPair[i]);
                }
            }

            if (LegalPair.Count != 0) return LegalPair;

            /////////////////////////////////////////////
            for (int i = 0; i < Pme.NumofCellsPlayed; i++)
            {
                List<Bridge> PmeBridges = new List<Bridge>();
                PmeBridges = GetBridges(Pme.PlayerCells[i].CorX, Pme.PlayerCells[i].CorY, MyState);
                for (int j = 0; j < PmeBridges.Count; j++)
                {
                    if (!Prev.Contains(PmeBridges[j].Pos))
                    {
                        LegalPair.Add(PmeBridges[j].Pos);
                        Prev.Add(PmeBridges[j].Pos);
                    }
                }
            }

           //if (LegalPair.Count != 0) return LegalPair;

            /////////////////////////////////////////////

            for (int i = 0; i < Popp.NumofCellsPlayed; i++)
            {
                List<Bridge> PoppBridges = new List<Bridge>();
                PoppBridges = GetBridges(Popp.PlayerCells[i].CorX, Popp.PlayerCells[i].CorY, MyState);
                for (int j = 0; j < PoppBridges.Count; j++)
                {
                    for (int k = 0; k < PoppBridges[j].mids.Count; k++)
                    {
                        if (!Prev.Contains(PoppBridges[j].mids[k]))
                        {
                            LegalPair.Add(PoppBridges[j].mids[k]);
                            Prev.Add(PoppBridges[j].mids[k]);
                        }
                    }
                    
                }
            }
            if (LegalPair.Count != 0) return LegalPair;

            for (int i = 0; i < Pme.NumofCellsPlayed; i++)
            {
                List<Bridge> PmeBridges = new List<Bridge>();
                PmeBridges = PointBridges(new Pair(Pme.PlayerCells[i].CorX, Pme.PlayerCells[i].CorY));
                for (int j = 0; j < PmeBridges.Count; j++)
                {
                    if (MyState.BoardCell[PmeBridges[j].Pos.x, PmeBridges[j].Pos.y].OccupiedBy == Pme.Color)
                    {
                        if (!Prev.Contains(PmeBridges[j].mids[0]) && MyState.BoardCell[PmeBridges[j].mids[0].x, PmeBridges[j].mids[0].y].OccupiedBy == 'N')
                        {
                            LegalPair.Add(PmeBridges[j].mids[0]);
                            Prev.Add(PmeBridges[j].mids[0]);
                        }
                        if (!Prev.Contains(PmeBridges[j].mids[1]) && MyState.BoardCell[PmeBridges[j].mids[1].x, PmeBridges[j].mids[1].y].OccupiedBy == 'N')
                        {
                            LegalPair.Add(PmeBridges[j].mids[1]);
                            Prev.Add(PmeBridges[j].mids[1]);
                        }
                    }
                }
            }
            if (LegalPair.Count != 0) return LegalPair;
            for (int i = 0; i < Pme.NumofCellsPlayed; i++)
            {
                List<Pair> myadjacent= new List<Pair>();
                myadjacent = GetAdjacent(new Pair(Pme.PlayerCells[i].CorX, Pme.PlayerCells[i].CorY), MyState);
                for (int j = 0; j < myadjacent.Count; j++)
                {
                    if (!Prev.Contains(myadjacent[j]))
                        {
                            LegalPair.Add(myadjacent[j]);
                            Prev.Add(myadjacent[j]);
                        }
                    

                }
            }


            //if (LegalPair.Count == 0)
            //    Console.Write("sdjfhlk");

            return LegalPair;
        }


        //public List<State> LegalPlays(Player P, State MyState) //Used in simulation & node expansion
        //{
        //    List<State> s = new List<State>(121 - MyState.OccCells);
        //    //bool flag = false;

        //    for (int i = 0; i < 11; i++)
        //        for (int j = 0; j < 11; j++)
        //        {
        //            if (MyState.bRange[0][0] <= i && MyState.bRange[0][1] <= j && MyState.bRange[1][0] <= i && MyState.bRange[1][1] >= j && MyState.bRange[2][0] >= i && MyState.bRange[2][1] <= j && MyState.bRange[3][0] >= i && MyState.bRange[3][1] >= j)
        //            {


        //                if ((CheckNeighboursOf2circles(i, j, MyState) == true && MyState.BoardCell[i, j].OccupiedBy == 'N') || MyState.OccCells == 0)
        //                //if (MyState.BoardCell[i, j].OccupiedBy == 'N')
        //                {
        //                    State TempHex = new State();
        //                    TempHex.CopyState(MyState);
        //                    TempHex.BoardCell[i, j].OccupiedBy = P.Color;
        //                    TempHex.BoardCell[i, j].CorX = i;
        //                    TempHex.BoardCell[i, j].CorY = j;
        //                    TempHex.BoardCell[i, j].flag = 1;

        //                    TempHex.OccCells++;
        //                    TempHex.cord[0] = i;
        //                    TempHex.cord[1] = j;


        //                    TempHex = UpdateRange(TempHex, i, j);

        //                    State ss = new State();
        //                    ss.CopyState(TempHex);
        //                    s.Add(ss);
        //                }

        //            }
        //        }
        //    return s;
        //}




        //public State UpdateRange(State mys, int i, int j)
        //{
        //    int[][] Range = new int[4][];
        //    for (int l = 0; l < 4; l++) Range[l] = new int[2];
        //    int[][] cRange = mys.bRange;

        //    Range[0][0] = Math.Max((Math.Min(i - 2, cRange[0][0])), 0);
        //    Range[0][1] = Math.Max((Math.Min(j - 2, cRange[0][1])), 0);

        //    Range[1][0] = Math.Max((Math.Min(i - 2, cRange[1][0])), 0);
        //    Range[1][1] = Math.Min((Math.Max(j + 2, cRange[1][1])), 10);

        //    Range[2][0] = Math.Min((Math.Max(i + 2, cRange[2][0])), 10);
        //    Range[2][1] = Math.Max((Math.Min(j - 2, cRange[2][1])), 0);

        //    Range[3][0] = Math.Min((Math.Max(i + 2, cRange[3][0])), 10);
        //    Range[3][1] = Math.Min((Math.Max(j + 2, cRange[3][1])), 10);

        //    mys.bRange = Range;
        //    return mys;
        //}

        private bool CheckNeighboursOf2circles(int i, int j, State mys)  //Check if here exist any neighbours of cells that is occupied 
        {

            bool flag = false;

            if (i - 1 >= 0 && mys.BoardCell[i - 1, j].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i - 1 >= 0 && j + 1 <= 10 && mys.BoardCell[i - 1, j + 1].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (j + 1 <= 10 && mys.BoardCell[i, j + 1].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i + 1 <= 10 && mys.BoardCell[i + 1, j].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }

            if (i + 1 <= 10 && j - 1 >= 0 && mys.BoardCell[i + 1, j - 1].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (j - 1 >= 0 && mys.BoardCell[i, j - 1].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (j + 2 <= 10 && mys.BoardCell[i, j + 2].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (j - 2 >= 0 && mys.BoardCell[i, j - 2].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i + 2 <= 10 && mys.BoardCell[i + 2, j].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i - 2 >= 0 && mys.BoardCell[i - 2, j].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i - 2 >= 0 && j + 2 <= 10 && mys.BoardCell[i - 2, j + 2].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i + 2 <= 10 && j - 2 >= 0 && mys.BoardCell[i + 2, j - 2].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i + 1 <= 10 && j - 2 >= 0 && mys.BoardCell[i + 1, j - 2].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i - 1 >= 0 && j - 1 >= 0 && mys.BoardCell[i - 1, j - 1].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i + 1 <= 10 && j + 1 <= 10 && mys.BoardCell[i + 1, j + 1].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            if (i + 1 <= 10 && j + 2 <= 10 && mys.BoardCell[i + 1, j + 2].OccupiedBy != 'N')
            {

                flag = true;
                return flag;
            }
            return flag;
        }


        public int Winner(ref Player P, ref State s)
        {
            stack MySt = new stack();
            int Counter0 = 0;
            int Counter10 = 0;
            List<Cell> temp = new List<Cell>();

            //------------------Check if RED Player Has Won------------------------------//
            if (P.Color == 'R') //Edges are up & Down
            {
                for (int i = 0; i < P.NumofCellsPlayed; i++)
                {
                    Cell c = P.PlayerCells[i];
                    if (c.CorX == 0)
                        Counter0++;

                    if (c.CorX == 10)
                        Counter10++;
                }
                if (Counter0 == 0 || Counter10 == 0)
                    return 0; //OngoingGame
                //If it reached here then there are 2 plays at the 2 edges (Higher probability of wining)
                while (P.Buffer.Count != 0)
                {
                    //1.Check if it is in row #zero 
                    if (P.Buffer[0].CorX == 0)
                    {
                        //Stack Method
                        s.BoardCell[P.Buffer[0].CorX, P.Buffer[0].CorY].flag = 0;

                        MySt.Push(P.Buffer[0]);
                        if (CheckNeighbours(MySt.C[MySt.top], ref P, ref s, ref MySt) == 24)
                            return P.num;
                        MySt.Pop();
                    }
                    //2.Check if any of its neighbors has a zero flag
                    else if (DoesAnyOfTheNeighborsHasAZeroFlag(P.Buffer[0], ref P, ref s) == 1)
                    {
                        if (s.BoardCell[P.Buffer[0].CorX, P.Buffer[0].CorY].flag != 0)
                        {
                            s.BoardCell[P.Buffer[0].CorX, P.Buffer[0].CorY].flag = 0;
                            MySt.Push(P.Buffer[0]);
                            if (CheckNeighbours(MySt.C[MySt.top], ref P, ref s, ref MySt) == 24)
                                return P.num;
                            MySt.Pop();
                        }
                    }
                    //3.Neither, thus Neglect
                    //Remove from buffer for all
                    P.Buffer.RemoveAt(0);
                }
                return 0; //Ongoing Game 
            }
            //------------------Check if BLUE Player Has Won------------------------------//
            else //edges are left & right
            {
                for (int i = 0; i < P.NumofCellsPlayed; i++)
                {
                    Cell c = P.PlayerCells[i];
                    if (c.CorY == 0)
                        Counter0++;

                    if (c.CorY == 10)
                        Counter10++;
                }
                if (Counter0 == 0 || Counter10 == 0)
                    return 0;
                //If it reached here then there are 2 plays at the 2 edges (Higher probability of wining)
                while (P.Buffer.Count != 0)
                {
                    //1.Check if it is in row #zero 
                    if (P.Buffer[0].CorY == 0)
                    {
                        //Console.WriteLine("Checking if it won");
                        //Stack Method
                        s.BoardCell[P.Buffer[0].CorX, P.Buffer[0].CorY].flag = 0;

                        MySt.Push(P.Buffer[0]);
                        if (CheckNeighbours(MySt.C[MySt.top], ref P, ref s, ref MySt) == 24)
                            return P.num;
                        MySt.Pop();
                    }
                    //2.Check if any of its neighbors has a zero flag
                    else if (DoesAnyOfTheNeighborsHasAZeroFlag(P.Buffer[0], ref P, ref s) == 1)
                    {
                        if (s.BoardCell[P.Buffer[0].CorX, P.Buffer[0].CorY].flag != 0)
                        {
                            s.BoardCell[P.Buffer[0].CorX, P.Buffer[0].CorY].flag = 0;
                            MySt.Push(P.Buffer[0]);
                            if (CheckNeighbours(MySt.C[MySt.top], ref P, ref s, ref MySt) == 24)
                                return P.num;
                            MySt.Pop();
                        }
                    }
                    //3.Neither, thus Neglect
                    //Remove from buffer for all
                    P.Buffer.RemoveAt(0);
                }
                return 0; //Ongoing Game 

            }
        }
        public void PrintBoardConsole(State Board)
        {
            string Indent = "";
            Console.Write(" ");
            for (int i = 0; i < 11; i++)
            {
                Console.Write(i.ToString() + "  ");
            }
            Console.WriteLine();

            for (int i = 0; i < 11; i++)
            {
                Console.Write(i.ToString());
                Console.Write(Indent);
                for (int j = 0; j < 11; j++)
                {
                    int x;
                    if (Board.BoardCell[i, j].OccupiedBy == 'R')
                    {
                        x = 1;
                    }
                    else if (Board.BoardCell[i, j].OccupiedBy == 'B')
                    {
                        x = 2;
                    }
                    else x = 0;
                    Console.Write(x + "  ");
                }
                Console.WriteLine();
                if (i != 9) Indent += " ";
            }
        }

        public Cell UpdateMyBoard(int x, int y, Player P, ref State s)
        {
            if (x >= 0 && x <= 10 && y >= 0 && y <= 10 && s.BoardCell[x, y].OccupiedBy == 'N')
            {
                s.BoardCell[x, y].OccupiedBy = P.Color;
                s.BoardCell[x, y].CorX = x;
                s.BoardCell[x, y].CorY = y;

                s.OccCells++;

                s.cord[0] = x;
                s.cord[1] = y;

                //HexBoard = UpdateRange(HexBoard, x, y);
                return s.BoardCell[x, y];
            }
            else return null;
        }
    }
}
