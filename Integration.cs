using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexGame
{
    class Integration
    {
        Board MyBoard;
        MonteCarlo MyAgent;

        public void PlayGame()
        {
            //Create Objects
            MyBoard = new Board();
            MyAgent = new MonteCarlo(MyBoard);

            //Array used to get next play from myagent

            int[] MyPlay = new int[2];


            Console.WriteLine("Welcome to Hex Game!!!\n");
            Console.WriteLine("Enter 1-> To be P1 or 2-> to be P2");
            Console.WriteLine("Mode: ");

            string Mode = Console.ReadLine();

            //Turn is used to synchronize turns between players
            int turn = 0;

            //1-Human start then agent, 2-Agent starts

            if (Mode == "1")
            {
                //Human start, i.e P1 is blue
                turn = 0;
                MyBoard.P1 = new Player('B', 2);
                MyBoard.P2 = new Player('R', 1);
            }
            else
            {
                //Myagent start
                turn = 1;
                MyBoard.P1 = new Player('R', 1);
                MyBoard.P2 = new Player('B', 2);
            }


            while (true)
            {
                if (turn == 1)
                {
                //Send to MonteCarlo Board state
                AgentAgain:

                    //Run agent
                    MyPlay = MyAgent.runalgo(MyBoard.HexBoard);
                    //Update board
                    //-1,-1 means swapping
                    if (MyPlay[0] == -1 && MyPlay[1] == -1)
                    {
                        MyBoard.SwapAtFirstTurn();
                    }
                    else
                    {
                        Cell c = MyBoard.UpdateMyBoard(MyPlay[0], MyPlay[1], MyBoard.P1, ref MyBoard.HexBoard);

                        if (c == null)
                        {
                            Console.WriteLine("Cell out of range or Occupied(Agent).");
                            goto AgentAgain;
                        }

                        //Update play in main board
                        MyBoard.P1.newplay(c, MyBoard.HexBoard);

                        //Check if it won

                        //MyBoard.PrintBoardConsole(MyBoard.HexBoard);
                        int p1 = MyBoard.Winner(ref MyBoard.P1, ref MyBoard.HexBoard);
                        if (MyBoard.P1.num == p1)
                        {
                            Console.WriteLine("My Agent is ksb hahahahaha !!!!!");
                            return;
                        }
                    }
                    turn = 0;
                }
                else
                {
                    //-----------------------------------------------------------------------------------
                    string swap = "";
                    //Check if P2 want to swap
                    if (MyBoard.HexBoard.OccCells == 1 && MyBoard.swappedflag == 0) //Check if I want to swap colors
                    {
                        Console.WriteLine("You want to swap? y or n");
                        Console.Write("Input: ");
                        swap = Console.ReadLine();
                        if (swap == "y")
                        {
                            MyBoard.swappedflag = 1;
                            MyBoard.SwapAtFirstTurn();
                        }
                    }
                    //-------------------------------------------------------------------------------------      
                    if (swap == "" || swap == "n")
                    {
                    //P2 will play
                    OpponentAgain:
                        Console.WriteLine("P2 is playing");
                        Console.WriteLine("Choose your cell (x & y): ");
                        Console.Write("X: ");
                        string x = Console.ReadLine();
                        Console.Write("Y: ");
                        string y = Console.ReadLine();
                        Cell c2 = MyBoard.UpdateMyBoard(Convert.ToInt32(x), Convert.ToInt32(y), MyBoard.P2, ref MyBoard.HexBoard);
                        if (c2 == null)
                        {
                            Console.WriteLine("Cell out of range(P2).");
                            goto OpponentAgain;
                        }


                        MyBoard.P2.newplay(c2, MyBoard.HexBoard);
                        int p = MyBoard.Winner(ref MyBoard.P2, ref MyBoard.HexBoard);
                        if (MyBoard.P2.num == p)
                        {
                            Console.WriteLine("enta ksbt :'(");
                            return;
                        }
                    }
                    turn = 1;


                }
                MyBoard.PrintBoardConsole(MyBoard.HexBoard);
            }
        }
    }
}
