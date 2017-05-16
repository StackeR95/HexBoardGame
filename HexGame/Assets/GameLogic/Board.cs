using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            SetOfConnections = new DSU(C);
        }

        public void CopyPlayer(Player p)
        {
          //  UnityEngine.Debug.Log("Player Num = " + p.num +" Player Color = "+p.Color);
            Color = p.Color;
            num = p.num;
            NumofCellsPlayed = p.NumofCellsPlayed;
            PlayerCells = new Cell[62];

            SetOfConnections.Copy(p.SetOfConnections);

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
            if(MyStat!=null)
            {
                this.Buffer.Add(C);
                this.NumofCellsPlayed++;
                this.PlayerCells[this.NumofCellsPlayed - 1] = C;
                SetOfConnections.update(new Pair(C.CorX, C.CorY), MyStat);
            }

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
            P1 = new Player('R', 1);
            P2 = new Player('B', 2);

        }


        public void SwapAtFirstTurn()
        {
            /////////////////////////////// PRIORITY QUEUES WILL EXCHANGE DATA ////////////////////////////
            int p1num = P1.num;
            int p2num = P2.num;


            Player tB = new Player('B', 0);
            Player tR = new Player('R', 0);

            if (P1.Color == 'R')
            {
                tR.CopyPlayer(P1);
                tB.CopyPlayer(P2);
                P1 = tB;
                P2 = tR;
            }
            else
            {
                tR.CopyPlayer(P2);
                tB.CopyPlayer(P1);
                P1 = tR;
                P2 = tB;
            }

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
            if (x - 2 >= 0 && x - 1 >= 0 && y + 1 <= 10)
            {
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

        public List<Pair> MustConnect(Player P, State Cur)
        {
            char Pcolor = P.Color;
            List<Pair> Must = new List<Pair>();
            for (int i = 0; i < P.SetOfConnections.ConCount; i++)
            {
                if (P.Color == 'R' && P.SetOfConnections.Connections[i].Higher.data.Count != 0 && P.SetOfConnections.Connections[i].Lower.data.Count != 0)
                {
                    Pair up = P.SetOfConnections.Connections[i].Higher.Peek();
                    Pair down = P.SetOfConnections.Connections[i].Lower.Peek();
                    if (up.x == 0 || (up.x == 1 && (Cur.BoardCell[up.x - 1, up.y].OccupiedBy == 'N' || (
                        up.y + 1 <= 10 && Cur.BoardCell[up.x - 1, up.y + 1].OccupiedBy == 'N'))))
                    {
                        if (down.x == 10 || (down.x == 9 && (Cur.BoardCell[down.x + 1, down.y].OccupiedBy == 'N' || (
                            down.y - 1 >= 0 && Cur.BoardCell[down.x + 1, down.y - 1].OccupiedBy == 'N'))))
                        {
                            Connection tmp = new Connection(P.Color);
                            for (int j = 0; j < P.SetOfConnections.Connections[i].Higher.data.Count; j++)
                            {
                                Pair tx = P.SetOfConnections.Connections[i].Higher.Dequeue();
                                tmp.Higher.Enqueue(tx);
                                if (tx.x == 1)
                                {

                                    if (Cur.BoardCell[tx.x - 1, tx.y].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x - 1, tx.y));
                                    else continue;

                                    if (tx.y + 1 <= 10 && Cur.BoardCell[tx.x - 1, tx.y + 1].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x - 1, tx.y + 1));
                                    else continue;
                                }
                                else
                                {
                                    List<Bridge> brd = PointBridges(new Pair(tx.x, tx.y));
                                    for (int k = 0; k < brd.Count; k++)
                                    {
                                        if (Cur.BoardCell[brd[k].Pos.x, brd[k].Pos.y].OccupiedBy == P.Color)
                                        {
                                            if (Cur.BoardCell[brd[k].mids[0].x, brd[k].mids[0].y].OccupiedBy != 'N') continue;
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy != 'N') continue;

                                            if (Cur.BoardCell[brd[k].mids[0].x, brd[k].mids[0].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[0].x, brd[k].mids[0].y));
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[1].x, brd[k].mids[1].y));
                                        }
                                    }
                                }
                            }
                            for (int j = 0; j < P.SetOfConnections.Connections[i].Lower.data.Count; j++)
                            {
                                Pair tx = P.SetOfConnections.Connections[i].Lower.Dequeue();
                                tmp.Lower.Enqueue(tx);
                                if (tx.x == 9)
                                {
                                    if (Cur.BoardCell[tx.x + 1, tx.y].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x + 1, tx.y));
                                    else continue;

                                    if (tx.y - 1 >= 0 && Cur.BoardCell[tx.x + 1, tx.y - 1].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x + 1, tx.y - 1));
                                    else continue;
                                }
                                else
                                {
                                    List<Bridge> brd = PointBridges(new Pair(tx.x, tx.y));
                                    for (int k = 0; k < brd.Count; k++)
                                    {
                                        if (Cur.BoardCell[brd[k].Pos.x, brd[k].Pos.y].OccupiedBy == P.Color)
                                        {
                                            if (Cur.BoardCell[brd[k].mids[0].x, brd[k].mids[0].y].OccupiedBy != 'N') continue;
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy != 'N') continue;

                                            if (Cur.BoardCell[brd[k].mids[0].x, brd[k].mids[0].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[0].x, brd[k].mids[0].y));
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[1].x, brd[k].mids[1].y));
                                        }
                                    }
                                }
                            }
                            // todo enqueue the connection again
                            for (int j = 0; j < tmp.Higher.data.Count; j++)
                            {
                                P.SetOfConnections.Connections[i].Higher.Enqueue(tmp.Higher.Dequeue());
                            }
                            for (int j = 0; j < tmp.Lower.data.Count; j++)
                            {
                                P.SetOfConnections.Connections[i].Lower.Enqueue(tmp.Lower.Dequeue());
                            }
                        }
                    }
                }
                else
                {
                    if (P.SetOfConnections.Connections[i].Higher.data.Count == 0 || P.SetOfConnections.Connections[i].Lower.data.Count == 0)
                        continue;
                    Pair left = P.SetOfConnections.Connections[i].Higher.Peek();
                    Pair right = P.SetOfConnections.Connections[i].Lower.Peek();
                    if (left.y == 0 || (left.y == 1 && (Cur.BoardCell[left.x, left.y - 1].OccupiedBy == 'N' || (
                        left.x + 1 <= 10 && Cur.BoardCell[left.x + 1, left.y - 1].OccupiedBy == 'N'))))
                    {
                        if (right.y == 10 || (right.y == 9 && (Cur.BoardCell[right.x, right.y + 1].OccupiedBy == 'N' || (
                            right.x - 1 >= 0 && Cur.BoardCell[right.x - 1, right.y + 1].OccupiedBy == 'N'))))
                        {
                            Connection tmp = new Connection(P.Color);
                            for (int j = 0; j < P.SetOfConnections.Connections[i].Higher.data.Count; j++)
                            {
                                Pair tx = P.SetOfConnections.Connections[i].Higher.Dequeue();
                                tmp.Higher.Enqueue(tx);
                                if (tx.y == 1)
                                {
                                    if (Cur.BoardCell[tx.x, tx.y - 1].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x, tx.y - 1));
                                    else continue;

                                    if (tx.x + 1 <= 10 && Cur.BoardCell[tx.x + 1, tx.y - 1].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x + 1, tx.y - 1));
                                    else continue;
                                }
                                else
                                {
                                    List<Bridge> brd = PointBridges(new Pair(tx.x, tx.y));
                                    for (int k = 0; k < brd.Count; k++)
                                    {
                                        if (Cur.BoardCell[brd[k].Pos.x, brd[k].Pos.y].OccupiedBy == P.Color)
                                        {
                                            if (Cur.BoardCell[brd[k].mids[0].x, brd[k].mids[0].y].OccupiedBy != 'N') continue;
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy != 'N') continue;

                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[0].x, brd[k].mids[0].y));
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[1].x, brd[k].mids[1].y));
                                        }
                                    }
                                }
                            }
                            for (int j = 0; j < P.SetOfConnections.Connections[i].Lower.data.Count; j++)
                            {
                                Pair tx = P.SetOfConnections.Connections[i].Lower.Dequeue();
                                tmp.Lower.Enqueue(tx);
                                if (tx.y == 9)
                                {
                                    if (Cur.BoardCell[tx.x, tx.y + 1].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x, tx.y + 1));
                                    else continue;

                                    if (tx.x - 1 >= 0 && Cur.BoardCell[tx.x - 1, tx.y + 1].OccupiedBy == 'N')
                                        Must.Add(new Pair(tx.x - 1, tx.y + 1));
                                    else continue;
                                }
                                else
                                {
                                    List<Bridge> brd = PointBridges(new Pair(tx.x, tx.y));
                                    for (int k = 0; k < brd.Count; k++)
                                    {
                                        if (Cur.BoardCell[brd[k].Pos.x, brd[k].Pos.y].OccupiedBy == P.Color)
                                        {
                                            if (Cur.BoardCell[brd[k].mids[0].x, brd[k].mids[0].y].OccupiedBy != 'N') continue;
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy != 'N') continue;

                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[0].x, brd[k].mids[0].y));
                                            if (Cur.BoardCell[brd[k].mids[1].x, brd[k].mids[1].y].OccupiedBy == 'N') Must.Add(new Pair(brd[k].mids[1].x, brd[k].mids[1].y));
                                        }
                                    }
                                }
                            }
                            // todo enqueue the connection again
                            for (int j = 0; j < tmp.Higher.data.Count; j++)
                            {
                                P.SetOfConnections.Connections[i].Higher.Enqueue(tmp.Higher.Dequeue());
                            }
                            for (int j = 0; j < tmp.Lower.data.Count; j++)
                            {
                                P.SetOfConnections.Connections[i].Lower.Enqueue(tmp.Lower.Dequeue());
                            }
                        }
                    }
                }
            }
            return Must;
        }
        public List<Pair> MustPlay(Player P, State Cur)
        {
            char Opp;
            if (P.Color == 'R') Opp = 'B';
            else Opp = 'R';

            List<Pair> Must = new List<Pair>();

            //1- Connect my bridges if opp. played in a midlock
            for (int i = 0; i < P.NumofCellsPlayed; i++)
            {
                List<Bridge> temp = PointBridges(new Pair(P.PlayerCells[i].CorX, P.PlayerCells[i].CorY));
                for (int j = 0; j < temp.Count; j++)
                {
                    if (Cur.BoardCell[temp[j].Pos.x, temp[j].Pos.y].OccupiedBy == P.Color)
                    {
                        if (Cur.BoardCell[temp[j].mids[0].x, temp[j].mids[0].y].OccupiedBy == Opp && Cur.BoardCell[temp[j].mids[1].x, temp[j].mids[1].y].OccupiedBy == 'N')
                            Must.Add(temp[j].mids[1]);
                        else if (Cur.BoardCell[temp[j].mids[1].x, temp[j].mids[1].y].OccupiedBy == Opp && Cur.BoardCell[temp[j].mids[0].x, temp[j].mids[0].y].OccupiedBy == 'N')
                            Must.Add(temp[j].mids[0]);
                    }
                }
            }
            if (Must.Count != 0) return Must;

            ////////////////////////////////////////////////
            //2- Cells adj. to board walls --> assumed as bridges
            ////////////////////////////////////////////////////
            for (int i = 0; i < P.NumofCellsPlayed; i++)
            {
                int X = P.PlayerCells[i].CorX, Y = P.PlayerCells[i].CorY;
                if (P.Color == 'R' && X == 1)
                {
                    if (X - 1 >= 0 && Y + 1 <= 10 && Opp == Cur.BoardCell[X - 1, Y].OccupiedBy && Cur.BoardCell[X - 1, Y + 1].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X - 1, Y + 1));

                    }
                    else if (X - 1 >= 0 && Y + 1 <= 10 && Opp == Cur.BoardCell[X - 1, Y + 1].OccupiedBy && Cur.BoardCell[X - 1, Y].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X - 1, Y));
                    }
                }
                else if (P.Color == 'R' && X == 9)
                {
                    if (X + 1 <= 10 && Y - 1 >= 0 && Opp == Cur.BoardCell[X + 1, Y].OccupiedBy && Cur.BoardCell[X + 1, Y - 1].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X + 1, Y - 1));
                    }

                    else if (X + 1 <= 10 && Y - 1 >= 0 && Opp == Cur.BoardCell[X + 1, Y - 1].OccupiedBy && Cur.BoardCell[X + 1, Y].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X + 1, Y));
                    }
                }
                else if (P.Color == 'B' && Y == 1)
                {
                    if (X + 1 <= 10 && Y - 1 >= 0 && Opp == Cur.BoardCell[X + 1, Y - 1].OccupiedBy && Cur.BoardCell[X, Y - 1].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X, Y - 1));
                    }

                    else if (X + 1 <= 10 && Y - 1 >= 0 && Opp == Cur.BoardCell[X, Y - 1].OccupiedBy && Cur.BoardCell[X + 1, Y - 1].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X + 1, Y - 1));
                    }
                }
                else if (P.Color == 'B' && Y == 9)
                {
                    if (X - 1 >= 0 && Y + 1 <= 10 && Opp == Cur.BoardCell[X - 1, Y + 1].OccupiedBy && Cur.BoardCell[X, Y + 1].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X, Y + 1));
                    }

                    else if (X - 1 >= 0 && Y + 1 <= 10 && Opp == Cur.BoardCell[X, Y + 1].OccupiedBy && Cur.BoardCell[X - 1, Y + 1].OccupiedBy == 'N')
                    {
                        Must.Add(new Pair(X - 1, Y + 1));
                    }
                }
            }

            if (Must.Count != 0) return Must;

            ////////////////////////////////////////////////////////
            //3- Connect non connected bridges, if I finished a line from side to side
            Must = MustConnect(P, Cur);

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
        

        public List<Bridge> GetVCBridges(Player Pme, State MyState)
        {
            List<Bridge> Plays = new List<Bridge>();
            for (int i = 0; i < Pme.SetOfConnections.ConCount; i++)
            {
                Pair tm1;
                List<Bridge> PmeBridges;
                Connection tmp = new Connection(Pme.Color);

                while (Pme.SetOfConnections.Connections[i].Higher.Count() != 0)
                {
                    if (Pme.SetOfConnections.Connections[i].Higher.Count() == 0) break;

                    tm1 = Pme.SetOfConnections.Connections[i].Higher.Dequeue();

                    tmp.Higher.Enqueue(new Pair(tm1.x, tm1.y));
                    PmeBridges = new List<Bridge>();
                    PmeBridges = GetBridges(tm1.x, tm1.y, MyState);
                    for (int k = 0; k < PmeBridges.Count; k++)
                    {
                        Plays.Add(PmeBridges[k]);
                    }
                    if (PmeBridges.Count != 0) break;
                }

                while (Pme.SetOfConnections.Connections[i].Lower.Count() != 0)
                {
                    if (Pme.SetOfConnections.Connections[i].Lower.Count() == 0)
                        break;

                    tm1 = Pme.SetOfConnections.Connections[i].Lower.Dequeue();

                    tmp.Lower.Enqueue(new Pair(tm1.x, tm1.y));
                    PmeBridges = new List<Bridge>();
                    PmeBridges = GetBridges(tm1.x, tm1.y, MyState);
                    for (int k = 0; k < PmeBridges.Count; k++)
                    {
                        Plays.Add(PmeBridges[k]);
                    }

                    if (PmeBridges.Count != 0)
                        break;
                }

                Pme.SetOfConnections.Connections[i].Copy(tmp);
            }
            return Plays;
        }

        public List<Pair> GetVCAdjac(Player Pme, State MyState)
        {
            List<Pair> Plays = new List<Pair>();
            for (int i = 0; i < Pme.SetOfConnections.ConCount; i++)
            {
                Pair tm1;
                List<Pair> adj1;
                Connection tmp = new Connection(Pme.Color);

                while (Pme.SetOfConnections.Connections[i].Higher.Count() != 0)
                {
                    tm1 = Pme.SetOfConnections.Connections[i].Higher.Dequeue();
                    tmp.Higher.Enqueue(new Pair(tm1.x, tm1.y));

                    adj1 = new List<Pair>();
                    adj1 = GetAdjacent(tm1, MyState);
                    for (int k = 0; k < adj1.Count; k++)
                    {
                        Plays.Add(adj1[k]);
                    }
                    if (adj1.Count != 0) break;
                }

                while (Pme.SetOfConnections.Connections[i].Lower.Count() != 0)
                {
                    tm1 = Pme.SetOfConnections.Connections[i].Lower.Dequeue();
                    tmp.Lower.Enqueue(new Pair(tm1.x, tm1.y));

                    adj1 = new List<Pair>();
                    adj1 = GetAdjacent(tm1, MyState);
                    for (int k = 0; k < adj1.Count; k++)
                    {
                        Plays.Add(adj1[k]);
                    }
                    if (adj1.Count != 0) break;
                }
                Pme.SetOfConnections.Connections[i].Copy(tmp);
            }
            return Plays;
        }
        
        public List<Pair> LegalPlays(Player Pme, Player Popp, State MyState) //Used in simulation & node expansion
        {

            HashSet<Pair> Prev = new HashSet<Pair>();
            bool[,] Hash;
            Hash = new bool[11, 11];

            List<Pair> LegalPair = new List<Pair>();

            List<Pair> MustPair = new List<Pair>();

            List<Bridge> PrioBridges = new List<Bridge>();

            List<Pair> PrioAdjac = new List<Pair>();


            //Must Play (3 cases)
            MustPair = MustPlay(Pme, MyState);

            for (int i = 0; i < MustPair.Count; i++)
            {
                if (!Hash[MustPair[i].x, MustPair[i].y])
                {
                    LegalPair.Add(MustPair[i]);
                    Prev.Add(MustPair[i]);
                    Hash[MustPair[i].x, MustPair[i].y] = true;
                }
            }

            if (LegalPair.Count != 0) return LegalPair;

            ////////////////Attack/////////////////////
            //1-Play in Connection end points bridges
            PrioBridges = GetVCBridges(Pme, MyState);

            for (int i = 0; i < PrioBridges.Count; i++)
            {
                if (!Hash[PrioBridges[i].Pos.x, PrioBridges[i].Pos.y])
                {
                    LegalPair.Add(PrioBridges[i].Pos);
                    Prev.Add(PrioBridges[i].Pos);
                    Hash[PrioBridges[i].Pos.x, PrioBridges[i].Pos.y] = true;
                }
            }

            if (LegalPair.Count != 0) goto OppCalc;

            //////////////////////////////////////////////
            //2- Adjacent to endpoints of connections
            PrioAdjac = GetVCAdjac(Pme, MyState);

            for (int i = 0; i < PrioAdjac.Count; i++)
            {
                if (!Hash[PrioAdjac[i].x, PrioAdjac[i].y])
                {
                    LegalPair.Add(PrioAdjac[i]);
                    Prev.Add(PrioAdjac[i]);
                    Hash[PrioAdjac[i].x, PrioAdjac[i].y] = true;
                }
            }

            if (LegalPair.Count != 0) goto OppCalc;
            


            /////////////////Defense/////////////////////
            //1- Play in possible bridge's midlocks
            OppCalc:
            List<Bridge> PoppBridges = new List<Bridge>();
            PoppBridges = GetVCBridges(Popp, MyState);
            for (int i = 0; i < PoppBridges.Count; i++)
            {
                for (int k = 0; k < PoppBridges[i].mids.Count; k++)
                {
                    if (!Hash[PoppBridges[i].mids[k].x, PoppBridges[i].mids[k].y])
                    {
                        LegalPair.Add(PoppBridges[i].mids[k]);
                        Prev.Add(PoppBridges[i].mids[k]);
                        Hash[PoppBridges[i].mids[k].x, PoppBridges[i].mids[k].y] = true;
                    }
                    //if (!Hash[PoppBridges[i].Pos.x, PoppBridges[i].Pos.y])
                    //{
                    //    LegalPair.Add(PoppBridges[i].Pos);
                    //    Hash[PoppBridges[i].Pos.x, PoppBridges[i].Pos.y] = true;
                    //}
                }
            }


            if (PoppBridges.Count != 0)
                goto ret;

            //2-Play in Adjacent to endpoints

            for (int i = 0; i < Popp.NumofCellsPlayed; i++)
            {
                List<Pair> oppadjacent = new List<Pair>();
                oppadjacent = GetVCAdjac(Popp, MyState);
                for (int j = 0; j < oppadjacent.Count; j++)
                {
                    if (!Hash[oppadjacent[j].x, oppadjacent[j].y])
                    {
                        LegalPair.Add(oppadjacent[j]);
                        Prev.Add(oppadjacent[j]);
                        Hash[oppadjacent[j].x, oppadjacent[j].y] = true;
                    }
                }
            }

            ret:
            for (int i = 0; i < LegalPair.Count; i++)
            {
                List<Bridge> temp = PointBridges(LegalPair[i]);
                for (int j = 0; j < temp.Count; j++)
                {
                    if (MyState.BoardCell[temp[j].Pos.x, temp[j].Pos.y].OccupiedBy == Pme.Color)
                    {
                        if (LegalPair.Count > 1 && MyState.BoardCell[temp[j].mids[0].x, temp[j].mids[0].y].OccupiedBy == Popp.Color && MyState.BoardCell[temp[j].mids[1].x, temp[j].mids[1].y].OccupiedBy == 'N')
                        { LegalPair.RemoveAt(i); i--; break; }
                        else if (LegalPair.Count > 1 && MyState.BoardCell[temp[j].mids[1].x, temp[j].mids[1].y].OccupiedBy == Popp.Color && MyState.BoardCell[temp[j].mids[0].x, temp[j].mids[0].y].OccupiedBy == 'N')
                        { LegalPair.RemoveAt(i); i--; break; }
                    }
                }
            }
            return LegalPair;
        }


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
            //   UnityEngine.Debug.Log("X = "+x+" Y = "+y +" Of Player = "+P.num);
                return s.BoardCell[x, y];
            }
            else return null;
        }
    }
}
