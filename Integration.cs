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
            Console.WriteLine("Welcome to Hex Game!!!\n");
            Console.WriteLine("Enter 1-> for Player VS MyAgent mode (Console)");
            Console.WriteLine("Enter 2-> for Player VS MyAgent mode (UnityInterface)");
            Console.WriteLine("Enter 3-> for Player VS OtherAgent mode");
            Console.WriteLine("Enter 4-> for MyAgent VS OtherAgent mode");
            Console.WriteLine("Mode: ");

            string Mode = Console.ReadLine();

            if (Mode == "1") // Player VS MyAgent mode (Console)
            {
                MyBoard = new Board();
                MyAgent = new MonteCarlo(MyBoard);

                int[] MyPlay = new int[2];
                while (true)
                {
                    //Send to MonteCarlo Board state
                    AgentAgain:
                    MyPlay = MyAgent.runalgo(MyBoard.HexBoard);
                    //Update board
                    Cell c = MyBoard.UpdateMyBoard(MyPlay[0], MyPlay[1], MyBoard.P1, ref MyBoard.HexBoard);
                    //Update P1
                    if (c == null)
                    {
                        Console.WriteLine("Cell out of range or Occupied(Agent).");
                        goto AgentAgain;
                    }

                    MyBoard.P1.NumofCellsPlayed++;
                    MyBoard.P1.PlayerCells[MyBoard.P1.NumofCellsPlayed - 1] = c;
                    MyBoard.P1.Buffer.Add(c);
                    //Check if it won

                    MyBoard.PrintBoardConsole(MyBoard.HexBoard);
                    int p1 = MyBoard.Winner(ref MyBoard.P1, ref MyBoard.HexBoard);
                    if (MyBoard.P1.num == p1)
                    {
                        Console.WriteLine("My Agent is ksb hahahahaha !!!!!");
                        return;
                    }

                    //-----------------------------------------------------------------------------------
                    string swap = "";
                    //Check if P2 want to swap
                    if (MyBoard.HexBoard.OccCells == 1) //Check if I want to swap colors
                    {
                        Console.WriteLine("You want to swap? y or n");
                        Console.Write("Input: ");
                        swap = Console.ReadLine();
                        if (swap == "y")
                            MyBoard.SwapAtFirstTurn();
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
                        //Update P2
                        MyBoard.P2.NumofCellsPlayed++;
                        MyBoard.P2.PlayerCells[MyBoard.P2.NumofCellsPlayed - 1] = c2;
                        MyBoard.P2.Buffer.Add(c2);
                        //Print board
                        MyBoard.PrintBoardConsole(MyBoard.HexBoard);

                        int p = MyBoard.Winner(ref MyBoard.P2, ref MyBoard.HexBoard);
                        if (MyBoard.P2.num == p)
                        {
                            Console.WriteLine("enta ksbt :'(");
                            return;
                        }
                        //Check if terminal state
                    }
                }

                string xx;
                xx = Console.ReadLine();
            }
            else if (Mode == "3") // Player VS OtherAgent mode
            {

            }
            else // MyAgent VS OtherAgent mode
            {

            }
        }
    }
}
