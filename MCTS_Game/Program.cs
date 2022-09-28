// Monte Carlo Tree Search
// N-Player game
//
// 5x5 tic-tac toe, 4 in a row wins


/*
GameState: GenMoves, DoMove, UndoMove

BoardScoring:

GameTreeNode: move made, Score, Comment
 
 */

using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using MCTS_Game;
using static System.Console;
using P = Player;



#if true
var b1 = new RandomBot();
var b2 = new SmartBot();
Fight(b1,b2, 3);
return;
#endif

#if true
var se = new Searcher();
var ttt3 = new GameStateTTT(3);
se.Search(ttt3);
WriteLine($"nodes {se.nodes}");
#endif



void Fight(IBot bob, IBot don, int size)
{
    var (bobWins, donWins, draws) = (0, 0, 0);

    for (var game = 0; game < 10; ++game)
    {
        Console.WriteLine(game);
        var odd = (game & 1) == 1;
        var g = new GameStateTTT(size);
        while (true)
        {
            if (odd)
            {
                PlayOne(bob, g);
                if (g.Evaluate() != Outcome.Unknown) break;
                PlayOne(don, g);
            }
            else
            {
                PlayOne(don, g);
                if (g.Evaluate() != Outcome.Unknown) break;
                PlayOne(bob, g);
            }
            if (g.Evaluate() != Outcome.Unknown) break;
        }

        var outcome = g.Evaluate();
        if (!odd && outcome == Outcome.Player1)
            outcome = Outcome.Player2;
        else if (!odd && outcome == Outcome.Player2)
            outcome = Outcome.Player1;

        if (outcome == Outcome.Draw) ++draws; 
        else if (outcome == Outcome.Player1) ++bobWins;
        else if (outcome == Outcome.Player2) ++donWins;
        else throw new Exception("Yo");
    }

    Console.WriteLine($"Bob wins {bobWins}, don wins {donWins}, draws {draws}");

    void PlayOne(IBot b, GameStateTTT g) => g.DoMove(b.FindMove(g));
}


interface IBot
{
    // get the next move for this state
    Move FindMove(GameStateTTT state);
}

class SmartBot : IBot
{
    // get the next move for this state
    public Move FindMove(GameStateTTT state)
    {
        var se = new Searcher();
        se.Search(state, 0);
        return se.bestMoves[0].Move;
    }
}

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

        for ( var i =0; i < gameSize; ++i)
        for (var j = 0; j < gameSize; ++j)
        {
            if (grid[i, j] == ToMove)
                score += sign * SquareScore(i,j);
            else if (grid[i, j] != Player.None)
                score += SquareScore(i, j);
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
            if (i == gameSize/2 && j == gameSize / 2) return center;
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

            if (horiz == winLength && grid[i,0] == player) 
                return true;
            if (vert == winLength && grid[0,i] == player)
                return true;

            d1 += grid[i, i] == grid[0, 0] ? 1 : 0;
            d2 += grid[i, gameSize-1-i] == grid[0, gameSize-1] ? 1 : 0;

        }
        if (d1 == winLength && grid[0, 0] == player)
            return true;
        if (d2 == winLength && grid[0, gameSize-1] == player)
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
            _=> throw  new NotImplementedException("BOO")
        };

    public void Dump()
    {
        
        void A()=>WriteLine($"+{new string('-', (gameSize-1) * 2)}-+");
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
                    From:new(0, 0), 
                    To:new Sq(i, j))
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
        return grid[to.X, to.Y] == Player.None;
    }


}

#if false
class GameStateOthello
{
    public const int gameSize = 5;

    // +1 player 1, -1 player 2, 0 = empty 
    public Player[,] grid = new Player[gameSize, gameSize];

    public Player ToMove = Player.Player1;

    public List<Move> GenMoves()
    {
        return null; // todo;
    }

    public void DoMove(Move move)
    {
        if (!IsLegal(move))
            throw new Exception("DEAD");

        var p = grid[move.fromX, move.fromY];
        grid[move.fromX, move.fromY] = Player.None;
        grid[move.toX, move.toY] = p;

        var (cx,cy)=((move.fromX+move.toX)/2, (move.fromY + move.toY) / 2);
        grid[cx,cy] = Player.None; // jumped

        NextPlayer();

    }

    void NextPlayer()
    {
        if (ToMove == Player.Player1) ToMove = Player.Player2;
        else ToMove = Player.Player1;
    }

    public void UndoMove(Move move)
    {
        // todo - check legal somehow?>
        var p = grid[move.toX, move.toY];
        grid[move.toX, move.toY] = Player.None;
        grid[move.fromX, move.fromY] = p;

        NextPlayer();
        var (cx, cy) = ((move.fromX + move.toX) / 2, (move.fromY + move.toY) / 2);
        grid[cx, cy] = ToMove; // jumped guy restores
    }

    bool IsLegal(Move move)
    {
        return true;
    }


}
#endif

public class Node
{
    public Node Parent;
    public List<Node> Children { get; } = new();

    public string Comment;
}