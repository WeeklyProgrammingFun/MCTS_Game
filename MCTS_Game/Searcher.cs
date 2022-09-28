using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTS_Game
{

    internal class Searcher
    {
        public long nodes = 0;

        public Move nullMove = new Move(Player.Player1, new Sq(0, 0), new Sq(0, 0));

        public List<(Move Move, int Score)> bestMoves = new();
        

        // return best score at position
        public int Search(GameStateTTT state, int depth = 0)
        {
            if (depth == 0) bestMoves.Clear();
            if (bestMoves.Count <= depth)
                bestMoves.Add((nullMove, 0));

            nodes++;
            var moves = state.GenMoves();

            if (depth > 2  && false)
            {
                state.Dump();
                Console.WriteLine(state.ScorePosition());
                return 0;
            }


            var e = state.Evaluate();
            if (e == Outcome.Player1 || e == Outcome.Player2)
            {
                //state.Dump();
                //Console.WriteLine();
                return state.ScorePosition();
            }

            if (e == Outcome.Draw)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.ForegroundColor = ConsoleColor.Black;
                //state.Dump();
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;

                //Console.WriteLine();
                return state.ScorePosition();
            }

            var sign = 1;
            if (state.ToMove == Player.Player2)
                sign = -1;

            // depth first search
            foreach (var m in moves)
            {
                state.DoMove(m);

                int score = Search(state, depth + 1);

                if (score*sign > bestMoves[depth].Score*sign)
                    bestMoves[depth] = new(m,score);

                state.UndoMove(m);
            }

            return bestMoves[depth].Score;

        }

    }
}
