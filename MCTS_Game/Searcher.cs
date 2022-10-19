using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Formats.Asn1.AsnWriter;

namespace MCTS_Game
{

    internal class Searcher
    {
        public long nodes = 0;

        public bool UseAlphaBeta { get; set; } = false;

        public Move nullMove = new Move(Player.Player1, new Sq(0, 0), new Sq(0, 0));

        // best sequence from position
        public List<(Move Move, int Score)> bestMoves = new();

        public class TreeNode
        {
            public TreeNode Parent;
            public List<TreeNode> Children = new();
            public int Score;
            public Move Move;
            public int Depth;
        }

        public TreeNode Root;

        // return best score at position

        // depth - depth in search tree
        public int Search(
            GameStateTTT state, 
            int depth = 0, int maxDepth = int.MaxValue, 
            TreeNode parent = null,
            int alpha = int.MinValue, // - infinity
            int beta  = int.MaxValue   // + infinity
            )
        {
            if (depth == 0)
            {
                Root = new();
                parent = Root;
                bestMoves.Clear();
            }
            if (bestMoves.Count <= depth)
                bestMoves.Add((nullMove, 0));

            nodes++;

            // exits
            if (depth > 4)
            {
                //state.Dump();
                //Console.WriteLine(state.ScorePosition());
                // todo
                return state.ScorePosition();
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

            var moves = state.GenMoves();

            var sign = 1;  // want most positive scores
            if (state.ToMove == Player.Player2)
                sign = -1; // want most negative scores

            int alphaBetaScore = sign == 1 ? int.MinValue : int.MaxValue;

            // depth first search
            foreach (var m in moves)
            {
                var child = new TreeNode { Move = m, Score = -1, Parent = parent};
                parent.Children.Add(child);

                state.DoMove(m);

                int score = Search(state, depth + 1, maxDepth,child, alpha, beta);

                state.UndoMove(m);

                if (UseAlphaBeta)
                {
                    if (state.ToMove == Player.Player1)
                    {
                        alphaBetaScore = Math.Max(alphaBetaScore, score);

                        if (alphaBetaScore >= beta)
                            break;
                        alpha = Math.Max(alpha, alphaBetaScore);
                    }
                    else
                    {
                        alphaBetaScore = Math.Min(alphaBetaScore, score);

                        if (alphaBetaScore <= alpha)
                            break;
                        beta = Math.Min(beta, alphaBetaScore);
                    }
                }

                child.Score = score;
                child.Depth = depth;

                if (sign == 1)
                { // biggest score
                    if (score > bestMoves[depth].Score)
                    {
                        bestMoves[depth] = new(m, score);
                    }

                }
                else
                { // smallest score
                    if (score < bestMoves[depth].Score)
                    {
                        bestMoves[depth] = new(m, score);
                    }
                }
            }

            // minimax
            if (sign == 1)
                parent.Score = parent.Children.Select(c => c.Score).Max();
            else
                parent.Score = parent.Children.Select(c => c.Score).Min();

            return parent.Score; // bestMoves[depth].Score;
        }

    }
}
