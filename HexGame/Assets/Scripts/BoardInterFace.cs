using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Timers = System.Timers;
 using HexGame;
public class BoardInterFace : MonoBehaviour {

    public GameObject Circle;
    public GameObject Player1Fab;
    public GameObject Player2Fab;
    public GameObject HexBoardDetect;
    public GameObject WinLosePanel;
    public GameObject SwapMessagePanel;

    private Player Player1;
    private Player Player2;

    public Text GameTurnLabel;
    public Text LeftPlayerName;
    public Text RightPlayerName;
    public Text Congratz;
    public Text TimeText;
    public Text PlayerPositionText;
    public Text AgentPositionText;

    private bool Mode;
    private bool SwapRuleOn;
    private int PlayerSwapped=-1;
    private float StartTime;
    FinalAnswer FinalAns; 
    private Communication CommObj;
    Thread CommThread;
    private int turn = 0;
    GameObject SelectedHexCell ;

    private Transform[] ArrayOfSelectedHexs;
    int Count = 0;
    bool Played = false;
    private Communication CommTest;
    private Integration GameLogic;
    Thread GameIntegration ;

    public int[] MyPlay ;
    public  MonteCarlo MyAgent;
    public Board MyBoard;
	// Use this for initialization
	void Start ()
    {
        StartTime = Time.time;
         
        WinLosePanel.SetActive(false);
        FinalAns = new FinalAnswer();
        ArrayOfSelectedHexs = new Transform[121];
        for (int i = 0; i < 121; i++)
        {
            ArrayOfSelectedHexs[i] = null;
        }
        if (HandleInput.GameMode == "PvsP")
        {
            LeftPlayerName.text = HandleInput.Player1Name;
            RightPlayerName.text = HandleInput.Player2Name;
        }
        else if (HandleInput.GameMode == "PvsC")
        {
            LeftPlayerName.text = HandleInput.PlayerName;
            RightPlayerName.text = "Computer";
            Mode = HandleInput.HumanFirst.isOn; //true ==1 which means human first ,, false ==2 which means agent first
            SwapRuleOn = HandleInput.SwapRulePvsC.isOn;
          //  Debug.Log(SwapRuleOn);
            if (!SwapRuleOn) FinalAns.DidPlayerSwap = -2; // No Swap Rule Applied 
            GameLogic = new Integration();
            GameIntegration = new Thread(() => GameLogic.PlayGame(Mode,ref FinalAns));
            GameIntegration.Priority = System.Threading.ThreadPriority.Highest;
            GameIntegration.Start();
            //UnityEngine.Debug.Log("3mal Start Lel Thread");
            Thread.Sleep(50);
            Player1 = GameLogic.MyBoard.P1; //Agent
            Player2 = GameLogic.MyBoard.P2; //Human
            if(Player1.Color=='R')
            {
                RightPlayerName.color = UnityEngine.Color.red;
            }
            if(Player2.Color=='B')
            {
                LeftPlayerName.color = UnityEngine.Color.blue;
            }
        }
        else if(HandleInput.GameMode=="CommMode")
        {
            TimeText.text = "";
            //MyBoard = new Board();
           // CommObj = new Communication(ref FinalAns);
            CommTest = new Communication(ref FinalAns);

            if (HandleInput.CommunicationMode == 3)
            {
              //Array used to get next play from myagent
              //  CommThread = new Thread(() => CommObj.CommMain(3));
            LeftPlayerName.text = "Our Agent ";
            RightPlayerName.text = "Oponnent Agent";
            MyPlay = new int[2];
            MyBoard = new Board();
            MyAgent = new MonteCarlo(MyBoard);
            //Turn is used to synchronize turns between players


            //1-Human start then agent, 2-Agent starts


                //Myagent start
                turn = 1;
                MyBoard.P1 = new Player('B', 2);
                MyBoard.P2 = new Player('R', 1);
            
           // CommTest.CommMain(3);
            CommTest.StartConnectionAsServer("192.168.0.1");
            }
            else if (HandleInput.CommunicationMode == 4)
            {
                //Array used to get next play from myagent
                //  CommThread = new Thread(() => CommObj.CommMain(3));
                LeftPlayerName.text = "Our Agent ";
                RightPlayerName.text = "Oponnent Agent";
                MyPlay = new int[2];
                MyBoard = new Board();
                MyAgent = new MonteCarlo(MyBoard);
                //Turn is used to synchronize turns between players


                //1-Human start then agent, 2-Agent starts


                //Myagent start
                turn = 0;
                MyBoard.P1 = new Player('R', 1); // The Agent
                MyBoard.P2 = new Player('B', 2); //P2 Means Human
                
                CommTest.StartConnectionAsClient("192.168.0.2");
            }
          //  MyBoard.P1 = Player1;
           // MyBoard.P2 = Player2;
          //  CommThread.Start();
        }

  
	}

	// Update is called once per frame
    void OfflineGame()
    { 
        if (Input.GetKeyDown(KeyCode.Escape))
    {
        OnApplicationQuit();
    }
        float F = Time.time - StartTime;
        string Minutes = ((int)F / 60).ToString();
        string Secounds = (F % 60).ToString("f2");
        TimeText.text ="Time = "+ Minutes + ":" + Secounds;
        Player1 = GameLogic.MyBoard.P1; //Agent
        Player2 = GameLogic.MyBoard.P2; //Human
        if (GameLogic.turn == 0)
        {
            if (Count == 1 && PlayerSwapped == -1 && SwapRuleOn == true && Mode==false)
            {
                SwapMessagePanel.SetActive(true);
                HexBoardDetect.SetActive(false);

            }

            FinalAns.PlayerPlayed = CheckInput(Player2.Color);
            if (Player2.Color == 'B')
            {
                GameTurnLabel.color = UnityEngine.Color.blue;
                PlayerPositionText.color = UnityEngine.Color.blue;
                LeftPlayerName.color = UnityEngine.Color.blue;
            }

            else
            {
                PlayerPositionText.color = UnityEngine.Color.red;
                GameTurnLabel.color = UnityEngine.Color.red;
                LeftPlayerName.color = UnityEngine.Color.red;
                PlayerPositionText.text = "Hex " + FinalAns.PlayerPosition[0] + "x " + FinalAns.PlayerPosition[1];
            }

            GameTurnLabel.text = LeftPlayerName.text.ToString() + "'s Turn ..";
        }

        else if (GameLogic.turn == 1 && FinalAns.AgentPlayed)//&& FinalAns.AgentPlayed
        {
            
            AgentPlay();
            Thread.Sleep(5);
            if (Player1.Color == 'B')
            {
                AgentPositionText.color = UnityEngine.Color.blue;
                AgentPositionText.text = "Hex " + FinalAns.AgentPosition[0] + "x " + FinalAns.AgentPosition[1];
            }
            else
            {
                AgentPositionText.color = UnityEngine.Color.red;
                AgentPositionText.text = "Hex " + FinalAns.AgentPosition[0] + "x " + FinalAns.AgentPosition[1];
            }
            //FinalAns.PlayerPlayed = false;
        }
        if(GameLogic.turn==1)
        {
            if (Player1.Color == 'B')
            {
                GameTurnLabel.color = UnityEngine.Color.blue;
                AgentPositionText.color = UnityEngine.Color.blue;
                RightPlayerName.color = UnityEngine.Color.blue;
                AgentPositionText.text = "Hex " + FinalAns.AgentPosition[0] + "x " + FinalAns.AgentPosition[1];
            }
            else
            {
                AgentPositionText.color = UnityEngine.Color.red;
                GameTurnLabel.color = UnityEngine.Color.red;
                RightPlayerName.color = UnityEngine.Color.red;
                AgentPositionText.text = "Hex " + FinalAns.AgentPosition[0] + "x " + FinalAns.AgentPosition[1];
            }
               GameTurnLabel.text = "Computer's Turn..";
        }
        if (FinalAns.WinLose != -1)
        {
            Thread.Sleep(3);
            WinLosePanel.SetActive(true);
            HexBoardDetect.SetActive(false);
            GameTurnLabel.text = "";
            //  HexBoardDetect.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (FinalAns.WinLose == 1)
            {

                if (Player2.Color == 'B') Congratz.color = UnityEngine.Color.blue;
                else Congratz.color = UnityEngine.Color.red;

                Congratz.text = Congratz.text.ToString() + LeftPlayerName.text.ToString() + " \n Win";
            }
            else if(FinalAns.WinLose==2)
            {
                if (Player1.Color == 'B') Congratz.color = UnityEngine.Color.blue;
                else Congratz.color = UnityEngine.Color.red;

                Congratz.text += "Our Agent " + "\n Win";
            }
            FinalAns.WinLose = -2;
            return;
        }
    }
 
    void AgentVsAgent()
    {
        Debug.Log("Turn = " + turn);
            if (turn == 1)
            {
                if (MyBoard.P1.Color == 'B')
                {
                    GameTurnLabel.color = UnityEngine.Color.blue;
                    LeftPlayerName.color = UnityEngine.Color.blue;
                } 
                else
                {
                    GameTurnLabel.color = UnityEngine.Color.red;
                    LeftPlayerName.color = UnityEngine.Color.red;
                }
                
                GameTurnLabel.text = "Our Agent Played ..";
                MyPlay = MyAgent.runalgo(MyBoard.HexBoard);
                //-1,-1 means swapping
                if (MyPlay[0] == -1 && MyPlay[1] == -1 && FinalAns.DidPlayerSwap != -2)
                {
                    MyBoard.SwapAtFirstTurn();
                }
                else
                {
                   
                    Cell c = MyBoard.UpdateMyBoard(MyPlay[0], MyPlay[1], MyBoard.P1, ref MyBoard.HexBoard);
                    //Debug.Log("Board E7na = " + MyBoard.HexBoard); //"Cx = " + c.CorX + "Cy = " + c.CorY + 
                    FinalAns.AgentPosition[0] = MyPlay[0];
                    FinalAns.AgentPosition[1] = MyPlay[1];
                    MyBoard.P1.newplay(c, MyBoard.HexBoard);
                    //   UnityEngine.Debug.Log("5alas newplay");
                    //Check if it won

                    //  MyBoard.PrintBoardConsole(MyBoard.HexBoard);
                    int p1 = MyBoard.Winner(ref MyBoard.P1, ref MyBoard.HexBoard);
                    if (MyBoard.P1.num == p1)
                    {
                        //    Console.WriteLine("My Agent is ksb hahahahaha !!!!!");
                        //UnityEngine.Debug.Log("Ezay Ya3ney Ya Ghabey");
                        FinalAns.WinLose = 2;
                        Thread.Sleep(100);
                        return;
                    }
                }
                // UnityEngine.Debug.Log("Agent Played true");
                AgentCommPlay(MyBoard.P1, FinalAns.AgentPosition[0].ToString(), FinalAns.AgentPosition[1].ToString());
                Thread.Sleep(10);
                turn = 0;
                FinalAns.AgentPlayed = true;
                CommTest.send(FinalAns.AgentPosition[0], FinalAns.AgentPosition[1]);
                // FN.InterfaceNotFinished = false;

            }
            else
            {
                if (MyBoard.P2.Color == 'B')
                {
                    GameTurnLabel.color = UnityEngine.Color.blue;
                    RightPlayerName.color = UnityEngine.Color.blue;
                }
                else
                {
                    GameTurnLabel.color = UnityEngine.Color.red;
                    RightPlayerName.color = UnityEngine.Color.red;
                }
                
                GameTurnLabel.text = "Oponnent Agent Played ..";
                CommTest.Receive();
                
               // Thread.Sleep(100);
               // Debug.Log("Recieved x= " + FinalAns.PlayerPosition[0] + " y = " + FinalAns.PlayerPosition[1]);
                if (FinalAns.PlayerPosition[0] == "-1" && FinalAns.PlayerPosition[1] == "-1")
                {
                    Debug.Log("3amal swap el 5abees");
                    MyBoard.swappedflag = 1;
                    MyBoard.SwapAtFirstTurn();
                }
                else
                {

                    string x = "";
                    string y = "";

                    x = FinalAns.PlayerPosition[0];
                    y = FinalAns.PlayerPosition[1];
                    Cell c2 = MyBoard.UpdateMyBoard(Convert.ToInt32(x), Convert.ToInt32(y), MyBoard.P2, ref MyBoard.HexBoard);
                    //Debug.Log("Board Ka7la = " + MyBoard.HexBoard.BoardCell); //"C2x = " + c2.CorX +"C2y = "+c2.CorY+
                    MyBoard.P2.newplay(c2, MyBoard.HexBoard);
                    
                    AgentCommPlay(MyBoard.P2, FinalAns.PlayerPosition[0], FinalAns.PlayerPosition[1]);
                    int p = MyBoard.Winner(ref MyBoard.P2, ref MyBoard.HexBoard);
                    if (MyBoard.P2.num == p)
                    {
                        FinalAns.WinLose = 1;
                        Thread.Sleep(100);
                        return;
                    }
                }
                turn = 1;

            }
        
    }
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnApplicationQuit();
        }
        float F = Time.time - StartTime;
        string Minutes = ((int)F / 60).ToString();
        string Secounds = (F % 60).ToString("f2");
        TimeText.text = "Time = " + Minutes + ":" + Secounds;
        if (HandleInput.GameMode =="PvsC")
        {
            OfflineGame();
        }
        if (HandleInput.GameMode == "CommMode" && FinalAns.SyncCommGame)
        {
            AgentVsAgent();
            if (FinalAns.WinLose != -1)
            {
                Thread.Sleep(3);
                WinLosePanel.SetActive(true);
                HexBoardDetect.SetActive(false);
                GameTurnLabel.text = "";
                if (FinalAns.WinLose == 1)
                {

                    if (MyBoard.P2.Color == 'B') Congratz.color = UnityEngine.Color.blue;
                    else Congratz.color = UnityEngine.Color.red;

                    Congratz.text = Congratz.text.ToString() + LeftPlayerName.text.ToString() + " \n Win";
                }
                else if (FinalAns.WinLose == 2)
                {
                    if (MyBoard.P1.Color == 'B') Congratz.color = UnityEngine.Color.blue;
                    else Congratz.color = UnityEngine.Color.red;

                    Congratz.text = Congratz.text.ToString() + RightPlayerName.text.ToString() + "\n Win";
                }
            }
        }
    }

    void AgentPlay()
    {
        Thread.Sleep(3);
        if (FinalAns.AgentPlayed && FinalAns.WinLose != -2 && FinalAns.InterfaceNotFinished==false)
        {
            if (Player1.Color == 'B')
                SelectedHexCell = Instantiate(Player2Fab);
            else
                SelectedHexCell = Instantiate(Player1Fab);
            //  SelectedHexCell.transform.position = new Vector3(hit.transform.position.x, -4.1f, hit.transform.position.z);
            // ArrayOfSelectedHexs[Count] = hit.transform;
            string Place = "Hex" + FinalAns.AgentPosition[0] + "x" + FinalAns.AgentPosition[1];
          //  UnityEngine.Debug.Log("Place = " + Place);
            GameObject HexPlace = new GameObject();
            HexPlace = GameObject.Find(Place);
            SelectedHexCell.transform.position = new Vector3(HexPlace.transform.position.x, -3.1f, HexPlace.transform.position.z);

            if (Count == 0) ArrayOfSelectedHexs[Count] = HexPlace.transform;
            else
            {
                if (ArrayOfSelectedHexs[Count - 1] != HexPlace.transform)
                {
                    ArrayOfSelectedHexs[Count] = HexPlace.transform;
                }

            }

            Count++;
            if (Count > 120)
            {
                Count = 0;
            }
            FinalAns.InterfaceNotFinished = true;
        }
    }

    void AgentCommPlay(Player Playeer,string x,string y)
    {
        //Thread.Sleep(3);
      
            if (Playeer.Color == 'B')
                SelectedHexCell = Instantiate(Player2Fab);
            else
                SelectedHexCell = Instantiate(Player1Fab);
            //  SelectedHexCell.transform.position = new Vector3(hit.transform.position.x, -4.1f, hit.transform.position.z);
            // ArrayOfSelectedHexs[Count] = hit.transform;
            string Place = "Hex" + x + "x" + y;
            //  UnityEngine.Debug.Log("Place = " + Place);
            GameObject HexPlace = new GameObject();
            HexPlace = GameObject.Find(Place);
            SelectedHexCell.transform.position = new Vector3(HexPlace.transform.position.x, -3.1f, HexPlace.transform.position.z);

            if (Count == 0) ArrayOfSelectedHexs[Count] = HexPlace.transform;
            else
            {
                if (ArrayOfSelectedHexs[Count - 1] != HexPlace.transform)
                {
                    ArrayOfSelectedHexs[Count] = HexPlace.transform;
                }

            }

            Count++;
            if (Count > 120)
            {
                Count = 0;
            }
            FinalAns.InterfaceNotFinished = true;
        
    }


    bool CheckInput(char Color) // Num =1 Red ,, Num =2 Blue
    {
        bool InTheBoard = false;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000000.0f))
            {
                int i = 0;
                while (ArrayOfSelectedHexs[i] != null)
                {
                    if (ArrayOfSelectedHexs[i] == hit.transform)
                    {
                      // Debug.Log("Enta Dost 5alas Mat7warsh");
                        return false;
                    }
                    i++;
                }

                if (Color == 'R')
                {
                    SelectedHexCell = Instantiate(Player1Fab); 

                }
                else
                {
                    SelectedHexCell = Instantiate(Player2Fab);
                }
                string OBJName = hit.transform.name;
              //  Debug.Log(OBJName);
                if(OBJName[0]=='H')
                {
                    InTheBoard = true;
                }
                OBJName= OBJName.Remove(0, 3);
                string []h=OBJName.Split('x');
                FinalAns.PlayerPosition[0] = h[0];
                FinalAns.PlayerPosition[1] = h[1];
                SelectedHexCell.transform.position = new Vector3(hit.transform.position.x, -3.1f, hit.transform.position.z);
                ArrayOfSelectedHexs[Count] = hit.transform;
                Count++;
                if (Count > 120)
                {
                    Count = 0;
                }
                
            }
          //  Debug.Log("5arag be true");
            
        }
        return InTheBoard;
    }


 public  void OnApplicationQuit()
    {
        try
        {
            if (HandleInput.GameMode == "PvsC")
            {
                if (GameIntegration.IsAlive)
                {
                    FinalAns.EndGame = true;
                    Thread.Sleep(3);
                    GameIntegration.Abort();
                }
            }

            if (HandleInput.GameMode == "CommMode" && HandleInput.CommunicationMode!=3)
            {
                if (CommThread.IsAlive)
                {
                    CommThread.Abort();
                }
            } 
            Application.Quit();
        }
        catch
        {
            Debug.Log("Failed");
        }
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    public void PlayerResponseSwap(int Res)
    {
        PlayerSwapped = Res;
        if(Res==1)
        {
            Player P = Player1;
            Player1 = Player2;
            Player2 = P;
            Color Temp = LeftPlayerName.color;
            LeftPlayerName.color = RightPlayerName.color;
            RightPlayerName.color = Temp;
            if (Player2.Color == 'B')
            {
                GameTurnLabel.color = UnityEngine.Color.blue;
                PlayerPositionText.color = UnityEngine.Color.blue;
            }

            else
            {
                PlayerPositionText.color = UnityEngine.Color.red;
                GameTurnLabel.color = UnityEngine.Color.red;
            }

        }
        FinalAns.DidPlayerSwap = Res;
        SwapMessagePanel.SetActive(false);
        HexBoardDetect.SetActive(true);
        return;
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void RestartLevel() 
    {
        SceneManager.LoadScene(1);
    
    }

}
    

