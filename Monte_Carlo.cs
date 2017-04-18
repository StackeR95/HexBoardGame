using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;



namespace Hextester
{
    
   public class State
   {
     //hanshof eih el content beta3 el state ba3den     
   };

   public class Node
   {
     public Node Parent; 
     public List <Node>Children;
     public State MyState;
     public int NodeStamp;
     public double vi; //value
     public int ni;
     public int N;

     public Node(Node P,List<Node>c,State s,int stamp,double v,int n, int NN)
     {
       Parent=P;
       Children=c;
       MyState=s;
       NodeStamp=stamp;
       vi=v; //value
       ni=n;
       N=NN;
     }
   };

    public class Program
    {
        //Global Section
        public static List <Node> TreeNode;
        public static Node CurrentNode = TreeNode[0]; //intial state
        public static int NodeStamp = 0;
        /////////////////////
        
     public void Selection() //Selects which Node to undergo the simulation & calls NodeExpansion/Simulation or BOTH
    {
        if(CurrentNode.NodeStamp==0) // It is a parentNode
        {
            CurrentNode=HighestUTB();
        }
        else
        {
            if(CurrentNode.ni!=0) //It has been visited and rollout occured. Thus, Begin Expansion
               NodeExpansion();   
            Simulation();
        }
     }

     public static Node HighestUTB() //credits of a part of this function goes to stackoverflow :D
    {
        double Max = double.NegativeInfinity;
        Node MaxUCTChild=TreeNode[0];
        foreach (Node c in CurrentNode.Children)
        {
            double UCB =  GetUCTB(c.vi,c.N,c.ni);
            if (UCB > Max)
            {
                MaxUCTChild = c;
                Max = UCB;
            }
        }
        return MaxUCTChild;
     }

   public static void NodeExpansion()
   {
    //NOTE: Some Logic depending on our game will be written here
    //Why? Because the expanded Nodes will be generated owing to the possible actions that could be taken
    //The actions will then be applied on the states 
    List <State> States;
    foreach(State NewState in States)
    {
        Node NewBorn=new Node(CurrentNode,null,NewState,NodeStamp++,double.PositiveInfinity,0,CurrentNode.ni);     
        CurrentNode.Children.Add(NewBorn);  
    }
   }

   public static void Simulation() //Roll-out
  {
    bool terminalState=false;
    bool win=false;
    //NOTE: terminal State will be defined upon our terms
    //Like if I(the PC) or the opponent made a win act
    //win will be given a value too based upon the winning 
    while(!terminalState)
    {
    //Logic of Generating random actions of hex game
        
    }    
    
    if(win)
     BackPropagation(1);
     else
     BackPropagation(-1);   
   }

  public static void BackPropagation(int WinOrLose)
  {
  while(CurrentNode.Parent!=null)
  {
    CurrentNode.ni++;
    CurrentNode.vi+=WinOrLose;
    CurrentNode=CurrentNode.Parent;
  }  
 }

 public static double GetUCTB(double vi, int N, int ni)
 {
        return vi+2*Math.Sqrt(Math.Log(N)/ni);    
 }

        public static void Main(string[] args)
        {
            //Your code goes here
            Console.WriteLine("Hello, world!");
        }
    }
}


