using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexBoardGame
{
    class Board
    {
        int StartGame(int Self) //Returns a representation of the starting state of the game.
        {
            return 0; 
        }

        int Current_Player(int Self,int State) //     # Takes the game state and returns the current player's # number.
        {
            return 0;
        }

        int Next_State(int Self,int State,int Play) //     # Takes the game state, and the move to be applied.  # Returns the new game state.
        {
            return 0;
        }

        int Legal_Play(int self, int state_history) //        # Takes a sequence of game states representing the full  # game history, and returns the full list of moves that  # are legal plays for the current player.     pass
        {
           return 0;
        }
      int WinTieLose(int self, int state_history) //        # Takes a sequence of game states representing the full  # game history.  If the game is now won, return the player # number.  If the game is still ongoing, return zero.  If  # the game is tied, return a different distinct value, e.g. -1.
        {
            return 0;
        }

    }
}
