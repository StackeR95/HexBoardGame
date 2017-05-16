using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace HexGame
{
    class Integration
    {
        public Board MyBoard;
        public  MonteCarlo MyAgent;
        public int turn = 0;

        public void PlayGame(bool Mode,ref FinalAnswer FN)
        {
            //Create Objects
            MyBoard = new Board();
            MyAgent = new MonteCarlo(MyBoard);

            //Array used to get next play from myagent

            int[] MyPlay = new int[2];

            //Turn is used to synchronize turns between players
           

            //1-Human start then agent, 2-Agent starts

            if (Mode == true)
            {
                //Human start, i.e P1 is blue
                turn = 0;
                MyBoard.P1 = new Player('R', 1); // The Agent
                MyBoard.P2 = new Player('B', 2); //P2 Means Human
            }
            else
            {
                //Myagent start
                turn = 1;
                MyBoard.P1 = new Player('B', 2);
                MyBoard.P2 = new Player('R', 1);
            }

           // Thread.Sleep(1000);
            while (!FN.EndGame)
            {   
                if (turn == 1)
                {
                    
                  //  Debug.Log("bravo da5al 3and el agent");
                    //Send to MonteCarlo Board state    

                    //Run agent
                    MyPlay = MyAgent.runalgo(MyBoard.HexBoard);
                    
                  //  UnityEngine.Debug.Log("5alas Algorithm");
                    //Update board
                    //-1,-1 means swapping
                    
                    if (MyPlay[0] == -1 && MyPlay[1] == -1)//&& FN.DidPlayerSwap !=-2
                    {
                        MyBoard.SwapAtFirstTurn();
                        turn = 0;
                        FN.AgentPlayed = false;
                        FN.InterfaceNotFinished = false;
                        continue;
                    }
                    else
                    {
                        Cell c = MyBoard.UpdateMyBoard(MyPlay[0], MyPlay[1], MyBoard.P1, ref MyBoard.HexBoard);
                        FN.AgentPosition[0] = MyPlay[0];
                        FN.AgentPosition[1] = MyPlay[1];
                      //  UnityEngine.Debug.Log("3amal update lel agent position");
                      //  UnityEngine.Debug.Log("agent X = " + FN.AgentPosition[0] + " Agent Y =" + FN.AgentPosition[1]);


                        //Update play in main board
                        MyBoard.P1.newplay(c, MyBoard.HexBoard);
                        
                        //Check if it won

                      //  MyBoard.PrintBoardConsole(MyBoard.HexBoard);
                        int p1 = MyBoard.Winner(ref MyBoard.P1, ref MyBoard.HexBoard);
                        if (MyBoard.P1.num == p1)
                        {
                        //    Console.WriteLine("My Agent is ksb hahahahaha !!!!!");
                            //UnityEngine.Debug.Log("Ezay Ya3ney Ya Ghabey");
                            FN.WinLose = 2;
                            Thread.Sleep(3);
                            return;
                        }
                    }

                    FN.AgentPlayed = true;
                    Thread.Sleep(5);
                   // UnityEngine.Debug.Log("Agent Played true");
                   
                        while (!FN.InterfaceNotFinished)
                        {

                        }

                    
 
                //    UnityEngine.Debug.Log("Inter face finished");
                    turn = 0;
                    FN.AgentPlayed = false;
                    FN.InterfaceNotFinished = false;

                }
                else
                {
                    FN.PlayerPlayed = false;
                    //-----------------------------------------------------------------------------------
                    
                    //Check if P2 want to swap
                    if (MyBoard.HexBoard.OccCells == 1) //Check if I want to swap colors //&& FN.DidPlayerSwap != -2
                    {
                        // Should Ask The User If He Need To Swap The Game The Agent Played INTERFACE
                        while (FN.DidPlayerSwap == -1)
                        {
                            
                        }
                        if (FN.DidPlayerSwap == 1)
                        {
                            MyBoard.swappedflag = 1;
                            MyBoard.SwapAtFirstTurn();
                           
                        }
                    }

                    //-------------------------------------------------------------------------------------      
                    if (FN.DidPlayerSwap == 0 || FN.DidPlayerSwap == -1 || FN.DidPlayerSwap == -2)
                    {
                        while (FN.PlayerPlayed == false)
                        {
                        }
                        //P2 will play
                    OpponentAgain:
   
                        Cell c2 = MyBoard.UpdateMyBoard(Convert.ToInt32(FN.PlayerPosition[0]), Convert.ToInt32(FN.PlayerPosition[1]), MyBoard.P2, ref MyBoard.HexBoard);
                       // Thread.Sleep(4);
                        //   UnityEngine.Debug.Log("Player X = " + FN.PlayerPosition[0] + " Player Y =" + FN.PlayerPosition[1]);
                         //   Debug.Log("Coord X = " + MyBoard.HexBoard.cord[0] + "Coord Y = " + MyBoard.HexBoard.cord[1]);
                        if (c2 == null)
                        {
                       //     Console.WriteLine("Cell out of range(P2).");
                            goto OpponentAgain;
                        }
                     

                        MyBoard.P2.newplay(c2, MyBoard.HexBoard);
                        int p = MyBoard.Winner(ref MyBoard.P2, ref MyBoard.HexBoard);
                        if (MyBoard.P2.num == p)
                        {
                         //   Console.WriteLine("enta ksbt :'(");
                            FN.WinLose = 1;
                            Thread.Sleep(3);
                            return;
                        }
                    }
                    turn = 1;

                    FN.DidPlayerSwap = -2;
                    Thread.Sleep(3);
                }
              //  MyBoard.PrintBoardConsole(MyBoard.HexBoard);
            }
          // Debug.Log("END OF FUNCTION");
        }
        public void PlayGameComm(bool Mode, ref FinalAnswer FN)
        {
            //Create Objects
            MyBoard = new Board();
            MyAgent = new MonteCarlo(MyBoard);

            //Array used to get next play from myagent

            int[] MyPlay = new int[2];

            //Turn is used to synchronize turns between players


            //1-Human start then agent, 2-Agent starts

            if (Mode == true)
            {
                //Human start, i.e P1 is blue
                turn = 0;
                MyBoard.P1 = new Player('R', 1); // The Agent
                MyBoard.P2 = new Player('B', 2); //P2 Means Human
            }
            else
            {
                //Myagent start
                turn = 1;
                MyBoard.P1 = new Player('B', 2);
                MyBoard.P2 = new Player('R', 1);
            }

            // Thread.Sleep(1000);
            while (!FN.EndGame)
            {
                if (turn == 1)
                {
                     UnityEngine.Debug.Log("turn == 1");
                    //Send to MonteCarlo Board state    

                    //Run agent
                    MyPlay = MyAgent.runalgo(MyBoard.HexBoard);

                //      UnityEngine.Debug.Log("5alas Algorithm");
                    //Update board
                    //-1,-1 means swapping
                    if (MyPlay[0] == -1 && MyPlay[1] == -1 && FN.DidPlayerSwap != -2)
                    {
                        MyBoard.SwapAtFirstTurn();
                    }
                    else
                    {
                        Cell c = MyBoard.UpdateMyBoard(MyPlay[0], MyPlay[1], MyBoard.P1, ref MyBoard.HexBoard);
                        FN.AgentPosition[0] = MyPlay[0];
                        FN.AgentPosition[1] = MyPlay[1];
                        //  UnityEngine.Debug.Log("3amal update lel agent position");
                        //  UnityEngine.Debug.Log("agent X = " + FN.AgentPosition[0] + " Agent Y =" + FN.AgentPosition[1]);


                        //Update play in main board
                        MyBoard.P1.newplay(c, MyBoard.HexBoard);
                     //   UnityEngine.Debug.Log("5alas newplay");
                        //Check if it won

                        //  MyBoard.PrintBoardConsole(MyBoard.HexBoard);
                        int p1 = MyBoard.Winner(ref MyBoard.P1, ref MyBoard.HexBoard);
                        if (MyBoard.P1.num == p1)
                        {
                            //    Console.WriteLine("My Agent is ksb hahahahaha !!!!!");
                            //UnityEngine.Debug.Log("Ezay Ya3ney Ya Ghabey");
                            FN.WinLose = 2;
                            Thread.Sleep(3);
                            return;
                        }
                    }

                    FN.AgentPlayed = true;
                   // FN.CommTurn = 2;
                    Thread.Sleep(5);
                    // UnityEngine.Debug.Log("Agent Played true");
                    while (!FN.InterfaceNotFinished)
                    {

                    }
                    Thread.Sleep(10);
                    turn = 0;
                    FN.AgentPlayed = false;
                    FN.CommTurn = 3;
                   // FN.InterfaceNotFinished = false;
                    
                }
                else if (FN.PlayerPlayed)
                {

                    if (FN.PlayerPosition[0] == "-1" && FN.PlayerPosition[1] == "-1")
                    {
                        MyBoard.swappedflag = 1;
                        MyBoard.SwapAtFirstTurn();
                    }
                    else
                    {

                        string x = "";
                        string y = "";

                        x = FN.PlayerPosition[0];
                        y = FN.PlayerPosition[1];
                        Cell c2 = MyBoard.UpdateMyBoard(Convert.ToInt32(x), Convert.ToInt32(y), MyBoard.P2, ref MyBoard.HexBoard);
                        MyBoard.P2.newplay(c2, MyBoard.HexBoard);
                        FN.PlayerPlayed =true;
                        //FN.CommTurn = 3;
                        while(FN.PlayerPlayed==true)
                        {

                        }
                        
                        int p = MyBoard.Winner(ref MyBoard.P2, ref MyBoard.HexBoard);
                        if (MyBoard.P2.num == p)
                        {
                            // UnityEngine.Debug.Log("Player Keseb");
                            //   Console.WriteLine("enta ksbt :'(");
                            FN.WinLose = 1;
                            Thread.Sleep(6);
                            return;
                        }
                    }

                  //  UnityEngine.Debug.Log("finish and changing turn");
                 //   Thread.Sleep(3);
                    turn = 1;
                   // FN.CommTurn = 2;
                   
                }   
                


                }
                //  MyBoard.PrintBoardConsole(MyBoard.HexBoard);
            }
            // Debug.Log("END OF FUNCTION");
        }
    }

