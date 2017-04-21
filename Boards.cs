using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MI_Project
{

//--------------------------------------------------------UTILITY CLASSES-------------------------------------------------------------//
     public class State
   {
         public Cell[,] BoardCell = new Cell[11, 11];    
   };

   public class Cell
   {
       public int CorX;
       public int CorY;
       public  char OccupiedBy;
       public int flag;

       public Cell(int x,int y,char P,int f)
       {
           CorX=x;
           CorY=y;
           OccupiedBy=P;
           flag = f;
       } 
   }
  
   public class Player
   {
    public char Color; //R:Red or B:Blue
    public int  num;
    public Cell [] PlayerCells;

    public Player(char C,int N)
    {
        Color=C;
        num=N;
        PlayerCells=new Cell[62];
    }
   }
//------------------------------------------------------------------------------------------------------------------------------------//
public class Board
{
    public static int OccupiedCells;
    public static stack MySt;
    public static Player P1;
    public static Player P2;
    public static State HexBoard;
    public static List<Cell>BufferR;
    public static List<Cell> BufferB;
//------------------------------------------------Utitlity Functions of Board--------------------------------------------------------//
//Only used implicitly in Board functions, aren't used by anything outside the board
    private static int CheckNeighbours(Cell c, Player P)
    {
        int i = c.CorX;
        int j = c.CorY;
        int flag = 0;
        List<Cell> temp = new List<Cell>();
        if (HexBoard.BoardCell[i - 1, j].OccupiedBy == P.Color && HexBoard.BoardCell[i - 1, j].flag == 1)
        {
            temp.Add(HexBoard.BoardCell[i - 1, j]);
            HexBoard.BoardCell[i - 1, j].flag = 0;
        }
        if (HexBoard.BoardCell[i - 1, j + 1].OccupiedBy == P.Color && HexBoard.BoardCell[i - 1, j + 1].flag == 1)
        {
            temp.Add(HexBoard.BoardCell[i - 1, j + 1]);
            HexBoard.BoardCell[i - 1, j + 1].flag = 0;
        }
        if (HexBoard.BoardCell[i, j + 1].OccupiedBy == P.Color && HexBoard.BoardCell[i, j + 1].flag == 1)
        {
            temp.Add(HexBoard.BoardCell[i, j + 1]);
            HexBoard.BoardCell[i, j + 1].flag = 0;
        }
        if (HexBoard.BoardCell[i + 1, j].OccupiedBy == P.Color && HexBoard.BoardCell[i + 1, j].flag == 1)
        {
            temp.Add(HexBoard.BoardCell[i + 1, j]);
            HexBoard.BoardCell[i + 1, j].flag = 0;
        }
        if (HexBoard.BoardCell[i + 1, j - 1].OccupiedBy == P.Color && HexBoard.BoardCell[i + 1, j - 1].flag == 1)
        {
            temp.Add(HexBoard.BoardCell[i + 1, j - 1]);
            HexBoard.BoardCell[i + 1, j - 1].flag = 0;
        }
        if (HexBoard.BoardCell[i, j - 1].OccupiedBy == P.Color && HexBoard.BoardCell[i, j - 1].flag == 1)
        {
            temp.Add(HexBoard.BoardCell[i, j - 1]);
            HexBoard.BoardCell[i, j - 1].flag = 0;
        }

        foreach(Cell x in temp)
        {
            if ((x.CorX == 10 && P.Color=='R') || (x.CorY==10 && P.Color=='B'))
                return 24;
            MySt.Push(x);
            if (CheckNeighbours(MySt.C[MySt.top], P)==0)
                MySt.Pop();
            flag = 1;
        }
        return flag; //found Neighbours or not
    }



   public static int DoesAnyOfTheNeighborsHasAZeroFlag(Cell c,Player P)
    {
        int i = c.CorX;
        int j = c.CorY;
        if ((HexBoard.BoardCell[i - 1, j].flag == 0 && HexBoard.BoardCell[i - 1, j].OccupiedBy == P.Color) || (HexBoard.BoardCell[i - 1, j + 1].flag == 0 && HexBoard.BoardCell[i - 1, j + 1].OccupiedBy == P.Color) || (HexBoard.BoardCell[i, j + 1].flag == 0 && HexBoard.BoardCell[i, j + 1].OccupiedBy == P.Color) || (HexBoard.BoardCell[i + 1, j].flag == 0 && HexBoard.BoardCell[i + 1, j].OccupiedBy == P.Color) || (HexBoard.BoardCell[i + 1, j - 1].flag == 0 && HexBoard.BoardCell[i + 1, j - 1].OccupiedBy == P.Color) || (HexBoard.BoardCell[i, j - 1].flag == 0 && HexBoard.BoardCell[i, j - 1].OccupiedBy == P.Color))
            return 1;
        else
            return 0;
    }
//-----------------------------------------------------Board Main Functions---------------------------------------------------------//
    public Board()
    {
        OccupiedCells=0;
        P1.Color='R';
        P2.Color='B';
        for (int i = 0; i < 11; i++)
            for (int j = 0; j < 11; j++)
            {
                HexBoard.BoardCell[i, j].CorX = i;
                HexBoard.BoardCell[i, j].CorY = j;
                HexBoard.BoardCell[i, j].OccupiedBy = 'N';
                HexBoard.BoardCell[i, j].flag = 1;
            }
        BufferR = new List<Cell>();
        BufferB = new List<Cell>();

    }
 
    public static void SwapAtFirstTurn(Player P1,Player P2,int Swap)
    {
        if(Swap==1)
        {
          P1.Color='B';
          P2.Color='R';
        }
    }
    public static State NextState(State ChosenByMonteCarlo)
    {
        //After performing MonteCarlo Algo, a chosen state will be returned by the algo containing both (The played history, the new play)
        HexBoard = ChosenByMonteCarlo;
        return HexBoard;
    }


    public static List <State> LegalPlays(Player P,State MyState)
    {
        List <State> s=new List<State>(121-OccupiedCells);
        for (int i = 0; i < 11; i++)
            for (int j = 0; j < 11; j++)
            {
                State TempHex= MyState;
                if (HexBoard.BoardCell[i, j].OccupiedBy == 'N')
                    TempHex.BoardCell[i, j].OccupiedBy = P.Color;

                s.Add(TempHex);
            }
        return s;
    }

 
    public static int Winner(Player P,State s)
    {
        int Counter=0;
        List <Cell> temp=new List<Cell>();
        //------------------Check if the game is tied--------------------------------//
        if (OccupiedCells == 121)
            return 10;
        //------------------Check if RED Player Has Won------------------------------//
        if(P.Color=='R') //Edges are up & Down
        {
          foreach(Cell c in P.PlayerCells)
          {
            if(c.CorX==0 || c.CorX==10)
             Counter++;
             
            if(c.CorX==0)
             temp.Add(c);
          }
          if(Counter<2)
           return 0; //OngoingGame
           //If it reached here then there are 2 plays at the 2 edges (Higher probability of wining)
          while (BufferR.Count!=0)
          {
              //1.Check if it is in row #zero 
              if (BufferR[0].CorX == 0)
              {
                  //Stack Method
                  MySt.Push(BufferR[0]);
                  if(CheckNeighbours(MySt.C[MySt.top],P)==24)
                      return P.num;
                  MySt.Pop();
              }
              //2.Check if any of its neighbors has a zero flag
              else if (DoesAnyOfTheNeighborsHasAZeroFlag(BufferR[0],P)==1)
              {
                  MySt.Push(BufferR[0]);
                  if (CheckNeighbours(MySt.C[MySt.top], P) == 24)
                      return P.num;
                  MySt.Pop();
              }
              //3.Neither, thus Neglect
              //Remove from buffer for all
              BufferR.RemoveAt(0);
          }
          return 0; //Ongoing Game 
        }
        //------------------Check if BLUE Player Has Won------------------------------//
        else //edges are left & right
        {
          foreach(Cell c in P.PlayerCells)
          {
            if(c.CorY==0 || c.CorY==10)
             Counter++;
          }
          if(Counter<2)
           return 0;
           //If it reached here then there are 2 plays at the 2 edges (Higher probability of wining)
          while (BufferB.Count != 0)
          {
              //1.Check if it is in row #zero 
              if (BufferB[0].CorX == 0)
              {
                  //Stack Method
                  MySt.Push(BufferB[0]);
                  if (CheckNeighbours(MySt.C[MySt.top], P) == 24)
                      return P.num;
                  MySt.Pop();
              }
              //2.Check if any of its neighbors has a zero flag
              else if (DoesAnyOfTheNeighborsHasAZeroFlag(BufferB[0], P) == 1)
              {
                  MySt.Push(BufferB[0]);
                  if (CheckNeighbours(MySt.C[MySt.top], P) == 24)
                      return P.num;
                  MySt.Pop();
              }

              //3.Neither, thus Neglect
              //Remove from buffer for all
              BufferB.RemoveAt(0);
          }
          return 0; //Ongoing Game 
              
        }
    }
        
    }
}
 
