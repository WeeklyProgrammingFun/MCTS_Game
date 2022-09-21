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
using static System.Console;
using P = Player;




long nodes = new();
var ttt3 = new GameStateTTT(3);
Search(ttt3);
WriteLine($"nodes {nodes}");


void Search(GameStateTTT state, int depth = 0)
{
    nodes++;
    var moves = state.GenMoves();

    var e = state.Evaluate();
    if (e == Outcome.Player1 || e == Outcome.Player2)
    {
        state.Dump();
        Console.WriteLine();
        return;
    }

    if (e == Outcome.Draw)
    {
        Console.BackgroundColor = ConsoleColor.Cyan;
        Console.ForegroundColor = ConsoleColor.Black;
        state.Dump();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Gray;

        Console.WriteLine();
        return;

    }

    // depth first search
    foreach (var m in moves)
    {
        state.DoMove(m);
        
        // do some work? scoring?

        Search(state, depth + 1);

        // do some work? scoring?

        state.UndoMove(m);
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
            ToMove = Player.Player2;
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