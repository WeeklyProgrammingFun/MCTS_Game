using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P = MCTS_Game.Player;

using static System.Console;

namespace MCTS_Game
{
    enum Player
    {
        None = 0,
        Player1 = 1,
        Player2 = -1
    }

    enum Outcome
    {
        Unknown = int.MaxValue,
        Draw = 0,
        Player1 = 1,
        Player2 = -1
    }

    record Sq(int X, int Y);
    record Move(Player WhoMoved, Sq From, Sq To);

    class GameStateTTT
    {
        // positive good for player1, negative for player 2
        public int ScorePosition()
        {
            var e = Evaluate();
            if (e == Outcome.Player1) return int.MaxValue;
            if (e == Outcome.Player2) return int.MinValue;
            if (e == Outcome.Draw) return 0;

            int sign = ToMove == P.Player1 ? 1 : -1;

            int score = 0;

            for (var i = 0; i < gameSize; ++i)
                for (var j = 0; j < gameSize; ++j)
                {
                    if (grid[i, j] == ToMove)
                        score += sign * SquareScore(i, j);
                    else if (grid[i, j] != Player.None)
                        score -= sign * SquareScore(i, j);
                }

            return score;

            int SquareScore(int i, int j)
            {
                int corner = 10;
                int center = 15;
                int other = 2;
                int s = gameSize - 1;
                if (i == 0 && j == 0) return corner;
                if (i == s && j == 0) return corner;
                if (i == 0 && j == s) return corner;
                if (i == s && j == s) return corner;
                if (i == gameSize / 2 && j == gameSize / 2) return center;
                return other;
            }

        }

        public Outcome Evaluate()
        {
            if (CheckWin(Player.Player1))
                return Outcome.Player1;
            else if (CheckWin(Player.Player2))
                return Outcome.Player2;
            else if (GenMoves().Count == 0)
                return Outcome.Draw;
            else return Outcome.Unknown;
        }

        private int winLength => gameSize;
        bool CheckWin(Player player)
        {
            Trace.Assert(winLength == gameSize); // todo- fix logic
            int d1 = 0;
            int d2 = 0;
            for (var i = 0; i < gameSize; ++i)
            {
                int horiz = 0;
                int vert = 0;
                for (var j = 0; j < gameSize; ++j)
                { // todo
                    horiz += grid[i, j] == grid[i, 0] ? 1 : 0;
                    vert += grid[j, i] == grid[0, i] ? 1 : 0;
                }

                if (horiz == winLength && grid[i, 0] == player)
                    return true;
                if (vert == winLength && grid[0, i] == player)
                    return true;

                d1 += grid[i, i] == grid[0, 0] ? 1 : 0;
                d2 += grid[i, gameSize - 1 - i] == grid[0, gameSize - 1] ? 1 : 0;

            }
            if (d1 == winLength && grid[0, 0] == player)
                return true;
            if (d2 == winLength && grid[0, gameSize - 1] == player)
                return true;

            return false;
        }

        public GameStateTTT(int size)
        {
            gameSize = size;
            grid = new Player[gameSize, gameSize];
        }

        public int gameSize = 3;

        // +1 player 1, -1 player 2, 0 = empty 
        public Player[,] grid;

        public Player ToMove = Player.Player1;

        char PlayerToTxt(Player p)
            => p switch
            {
                P.None => ' ',
                P.Player1 => 'X',
                P.Player2 => 'Y',
                _ => throw new NotImplementedException("BOO")
            };

        public void Dump()
        {

            void A() => WriteLine($"+{new string('-', (gameSize - 1) * 2)}-+");
            A();

            for (var i = 0; i < gameSize; ++i)
            {

                Write("|");
                for (var j = 0; j < gameSize; ++j)
                {
                    var g = PlayerToTxt(grid[i, j]);

                    Write($"{g}|");

                }
                WriteLine();
                A();
            }
        }

        public List<Move> GenMoves()
        {
            var moves = new List<Move>();
            for (var i = 0; i < gameSize; ++i)
                for (var j = 0; j < gameSize; ++j)
                {
                    if (grid[i, j] == Player.None)
                        moves.Add(new(
                            WhoMoved: ToMove,
                            From: new(0, 0),
                            To: new Sq(i, j))
                        );

                }

            return moves;
        }

        public void DoMove(Move move)
        {
            if (!IsLegal(move))
                throw new Exception("DEAD");

            var to = move.To;
            grid[to.X, to.Y] = ToMove;

            NextPlayer();

        }

        void NextPlayer()
        {
            if (ToMove == P.Player1)
                ToMove = P.Player2;
            else
                ToMove = P.Player1;
        }

        public void UndoMove(Move move)
        {
            // todo - check legal somehow?
            var to = move.To;
            grid[to.X, to.Y] = Player.None;
            NextPlayer();
        }

        bool IsLegal(Move move)
        {
            var to = move.To;
            var ok = grid[to.X, to.Y] == Player.None;
            return ok;
        }


    }
}
