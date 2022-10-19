using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Game
{

    interface IBot
    {
        // get the next move for this state
        Move FindMove(GameStateTTT state);
    }

    class SmartBot : IBot
    {
        Searcher se = new Searcher();

        // get the next move for this state
        public Move FindMove(GameStateTTT state)
        {
            se.UseAlphaBeta = true;
            se.Search(state, 0);
            //Console.WriteLine($"{se.bestMoves.Count} best variation found, to play {state.ToMove}");
            //foreach (var m in se.bestMoves)
            //    Console.WriteLine($"  {m}");
            Console.WriteLine($"{se.nodes}"); // 155280
            return se.bestMoves[0].Move;
        }
    }

    /*
    with alpha/beta
    0
    891
    Applying move Move { WhoMoved = Player1, From = Sq { X = 0, Y = 0 }, To = Sq { X = 0, Y = 0 } }
    Applying move Move { WhoMoved = Player2, From = Sq { X = 0, Y = 0 }, To = Sq { X = 1, Y = 1 } }
    1072
    Applying move Move { WhoMoved = Player1, From = Sq { X = 0, Y = 0 }, To = Sq { X = 0, Y = 0 } }
    Unhandled exception. System.Exception: DEAD


    no alpha/beta
    0
    18730
    Applying move Move { WhoMoved = Player1, From = Sq { X = 0, Y = 0 }, To = Sq { X = 1, Y = 1 } }
    Applying move Move { WhoMoved = Player2, From = Sq { X = 0, Y = 0 }, To = Sq { X = 1, Y = 0 } }
    21762
    Applying move Move { WhoMoved = Player1, From = Sq { X = 0, Y = 0 }, To = Sq { X = 0, Y = 0 } }
    Applying move Move { WhoMoved = Player2, From = Sq { X = 0, Y = 0 }, To = Sq { X = 2, Y = 2 } }
    22000
    Applyi
    */

    class RandomBot : IBot
    {
        private Random r = new Random(1234);

        // get the next move for this state
        public Move FindMove(GameStateTTT state)
        {
            var moves = state.GenMoves();
            return moves[r.Next(moves.Count)];
        }
    }

}
