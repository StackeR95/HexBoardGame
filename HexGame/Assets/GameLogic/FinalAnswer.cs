using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HexGame;
public  class FinalAnswer
{
    public int[] AgentPosition; //{ get; set; }
    public  bool AgentPlayed;//{ get; set; }
    public string[] PlayerPosition; //{ get; set; }
    public bool PlayerPlayed;//{ get; set; }
    public bool EndGame = false;
    public bool InterfaceNotFinished = false;
    public int DidPlayerSwap = -1; // 1 Player Swapped ... 0 Player Refused To Swap -2 Swap Rule Is OFF -1 No Input Yet
    public int WinLose = -1;
    public int CommTurn = -1;
    public bool SyncCommGame = false;
    public FinalAnswer()
    {
        AgentPosition = new int[2];
        
        PlayerPosition = new string[2];
       
    }

}
