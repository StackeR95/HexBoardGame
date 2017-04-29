using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGame
{


    public class Node
    {
        public Player P1;
        public Player P2;
        public Node Parent;
        public List<Node> Children;
        public State MyState;
        public double value; //value
        public int visits;
        public int turn; //0--> expand on my player 1--> other player

        public Node(Node P, State s, double v, int n, int turnn, Player p1, Player p2)
        {
            Children = new List<Node>();
            Parent = P;
            MyState = s;
            value = v; //value
            visits = n;
            turn = turnn;
            P1 = new Player(p1.Color, p1.num);
            P2 = new Player(p2.Color, p2.num);
            P1.CopyPlayer(p1);
            P2.CopyPlayer(p2);
        }
    };

    public class MonteCarlo
    {
        //Global Section
        public Node myTree; //Contains root of tree
        public Node prevmyTree; //My prev. tree, used in optimization
        Board b;
        /////////////////////
        public MonteCarlo(Board bbb) {
            b = bbb;

        }

        public int[] runalgo(State initialboard)
        {
           
            //Node CompStates = CompareLists(initialboard);
            //  if (CompStates != null)
            //  {
            //CompStates.Parent = null;
            //myTree.Add(CompStates);
            //    myTree = prevmyTree;
            //}
            //else
            //{
            myTree = new Node(null, initialboard, 0, 0, 0, b.P1, b.P2);

            //}
            if(initialboard.OccCells == 0)
            {
                int []arr = new int[2];
                arr[0] = 5;
                arr[1] = 5;
                return arr;
            }
            else if(initialboard.OccCells == 1)
            {
                //todo swap
            }

            // iterations += 50;

            int iterations = 5000;
            //iterations = b.HexBoard.OccCells + 200;
            while (true)
            {

                Node selectednode, selectednode2;
                selectednode = Selection(myTree); //Choose highest child which hava best UCB
                NodeExpansion(selectednode); //Expand that child
                selectednode2 = Selection(selectednode); //Select the best child
                Simulation(selectednode2); //Simulate node

                iterations--;
                if (iterations == 0) break;

            }

            Node myNextState = null;
            myNextState = HighestUCB(myTree); //Return best child/move for tree root

            //prevmyTree = new List<Node>();
            //prevmyTree = myTree;
            return myNextState.MyState.cord;
        }
        


        public Node Selection(Node n) //Selects which Node to undergo the simulation & calls NodeExpansion/Simulation or BOTH
        {
            Node highest = n;
            while (highest.Children.Count != 0)
            {
                highest = HighestUCB(highest);
            }

            return highest;

        }



        public void NodeExpansion(Node parent) //Expands all possible states of a parent
        {
            Player P1 = new Player(' ', 0); // bymasili ana (machine)
            Player P2 = new Player(' ', 0); ; // bymasil el player el tani
            P1.CopyPlayer(b.P1);
            P2.CopyPlayer(b.P2);

            if (parent.turn == 0) //p1's turn
            {
                List<Pair> SPairs = b.LegalPlays(P1, P2, parent.MyState);
                foreach (Pair Pairs in SPairs)
                {
                    State NewState = new State();
                    NewState.CopyState(parent.MyState);
                    NewState.BoardCell[Pairs.x, Pairs.y].OccupiedBy = P1.Color;
                    NewState.BoardCell[Pairs.x, Pairs.y].CorX = Pairs.x;
                    NewState.BoardCell[Pairs.x, Pairs.y].CorY = Pairs.y;
                    NewState.BoardCell[Pairs.x, Pairs.y].flag = 1;

                    NewState.OccCells++;
                    NewState.cord[0] = Pairs.x;
                    NewState.cord[1] = Pairs.y;

                    Cell c = new Cell(NewState.cord[0], NewState.cord[1], parent.P1.Color, 1);
                    Player p1 = new Player(' ', 0);
                    Player p2 = new Player(' ', 0);
                    p1.CopyPlayer(parent.P1);
                    p2.CopyPlayer(parent.P2);
                    p1.Buffer.Add(c);
                    p1.NumofCellsPlayed++;
                    p1.PlayerCells[p1.NumofCellsPlayed - 1] = c;
                    Node NewBorn = new Node(parent, NewState, 0, 0, 1, p1, p2);
                    parent.Children.Add(NewBorn);
                }
            }
            else
            {

                List<Pair> SPairs = b.LegalPlays(P2, P1, parent.MyState);
                foreach (Pair Pairs in SPairs)
                {
                    State NewState = new State();
                    NewState.CopyState(parent.MyState);
                    NewState.BoardCell[Pairs.x, Pairs.y].OccupiedBy = P2.Color;
                    NewState.BoardCell[Pairs.x, Pairs.y].CorX = Pairs.x;
                    NewState.BoardCell[Pairs.x, Pairs.y].CorY = Pairs.y;
                    NewState.BoardCell[Pairs.x, Pairs.y].flag = 1;

                    NewState.OccCells++;
                    NewState.cord[0] = Pairs.x;
                    NewState.cord[1] = Pairs.y;

                    Cell c = new Cell(NewState.cord[0], NewState.cord[1], parent.P2.Color, 1);
                    Player p1 = new Player(' ', 0);
                    Player p2 = new Player(' ', 0);
                    p1.CopyPlayer(parent.P1);
                    p2.CopyPlayer(parent.P2);
                    p2.Buffer.Add(c);
                    p2.NumofCellsPlayed++;
                    p2.PlayerCells[p2.NumofCellsPlayed - 1] = c;
                    Node NewBorn = new Node(parent, NewState, 0, 0, 0, p1, p2);
                    parent.Children.Add(NewBorn);
                }
            }
        }

        public void Simulation(Node node) //Run simulation on a node and update it's values
        { 
            State IntState = new State();
            IntState.CopyState(node.MyState);
            Player P1 = new Player(' ', 0); // bymasili ana (machine)
            Player P2 = new Player(' ', 0); ; // bymasil el player el tani
            P1.CopyPlayer(node.P1);
            P2.CopyPlayer(node.P2);

            int turn = node.turn;

            while (true)
            {
                if (turn == 0)
                {

                    //b.PrintBoardConsole(IntState);
                    List<Pair> StatesP1 = b.LegalPlays(P1, P2, IntState);

                    Random rnd = new Random();
                    int index = rnd.Next(0, StatesP1.Count);
                    //try
                    //{
                    IntState.BoardCell[StatesP1[index].x, StatesP1[index].y].OccupiedBy = P1.Color;
                    IntState.BoardCell[StatesP1[index].x, StatesP1[index].y].CorX = StatesP1[index].x;
                    IntState.BoardCell[StatesP1[index].x, StatesP1[index].y].CorY = StatesP1[index].y;
                    IntState.BoardCell[StatesP1[index].x, StatesP1[index].y].flag = 1;

                    IntState.OccCells++;
                    IntState.cord[0] = StatesP1[index].x;
                    IntState.cord[1] = StatesP1[index].y;

                    Cell c = new Cell(IntState.cord[0], IntState.cord[1], P1.Color, 1);
                    P1.NumofCellsPlayed++;
                    P1.PlayerCells[P1.NumofCellsPlayed - 1] = c;
                    P1.Buffer.Add(c);
                    //}
                    //catch (Exception ex)
                    //{
                    //    b.PrintBoardConsole(IntState);
                    //}
                    int p1;
                    p1 = b.Winner(ref P1, ref IntState);
                    if (P1.num == p1)
                    {
                        BackPropagation(10, node);
                        return;
                    }

                    turn = 1;

                }
                else
                { //Hwa eli yl3b el awal

                    List<Pair> StatesP2 = b.LegalPlays(P2, P1, IntState);

                    Random rnd = new Random();
                    int index = rnd.Next(0, StatesP2.Count);
                    //try
                    //{
                    IntState.BoardCell[StatesP2[index].x, StatesP2[index].y].OccupiedBy = P2.Color;
                    IntState.BoardCell[StatesP2[index].x, StatesP2[index].y].CorX = StatesP2[index].x;
                    IntState.BoardCell[StatesP2[index].x, StatesP2[index].y].CorY = StatesP2[index].y;
                    IntState.BoardCell[StatesP2[index].x, StatesP2[index].y].flag = 1;

                    IntState.OccCells++;
                    IntState.cord[0] = StatesP2[index].x;
                    IntState.cord[1] = StatesP2[index].y;

                    Cell c = new Cell(IntState.cord[0], IntState.cord[1], P2.Color, 1);

                    P2.NumofCellsPlayed++;
                    P2.PlayerCells[P2.NumofCellsPlayed - 1] = c;
                    P2.Buffer.Add(c);
                    //}
                    //catch (Exception ex)
                    //{
                    //    b.PrintBoardConsole(IntState);
                    //}
                    int p2 = b.Winner(ref P2, ref IntState);
                    if (P2.num == p2)
                    {
                        BackPropagation(-5, node);
                        return;
                    }

                    turn = 0;
                }

            }
        }

        public void BackPropagation(int WinOrLose, Node CurrentNode)
        {
            while (CurrentNode != null)
            {
                CurrentNode.visits++;
                CurrentNode.value += WinOrLose;
                CurrentNode = CurrentNode.Parent;
            }
        }

        public Node HighestUCB(Node parent)
        {
            double Max = double.NegativeInfinity;
            Node MaxUCTChild = parent;
            foreach (Node c in parent.Children)
            {
                double UCB = GetUCB(c.value, myTree.visits, c.visits);
                if (UCB > Max)
                {
                    MaxUCTChild = c;
                    Max = UCB;
                }
            }
            return MaxUCTChild;
        }

        public double GetUCB(double vi, int N, int ni) //Upper Confidence bound equation
        {
            //vi: the value of node
            //ni: the number of visits of node
            //N: the total number of visits
            if (ni == 0) return double.PositiveInfinity;
            else return (vi / ni) + (0.1 * (Math.Sqrt(Math.Log(N) / ni)));
        }

        //public Node CompareLists(State myState)
        //{
        //    bool found = true;
        //    for (int i = 0; i < prevmyTree.Count; i++)
        //    {
        //        State mys = prevmyTree[i].MyState;
        //        for (int x = 0; x < 11; x++)
        //        {
        //            for (int y = 0; y < 11; y++)
        //            {

        //                Cell c = mys.BoardCell[x, y];
        //                if (c.CorX != myState.BoardCell[x, y].CorX || c.CorY != myState.BoardCell[x, y].CorY || c.flag != myState.BoardCell[x, y].flag || c.OccupiedBy != myState.BoardCell[x, y].OccupiedBy)
        //                {
        //                    found = false;
        //                    break;
        //                }

        //            }
        //            if (found == false) break;

        //        }
        //        if (found == true) return prevmyTree[i];
        //    }
        //    return null;
        //}


    }


}
