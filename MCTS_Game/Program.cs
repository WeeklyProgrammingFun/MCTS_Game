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
using P = MCTS_Game.Player;
using Lomont.Formats;


#if false
var ttt3 = new GameStateTTT(3);
//var m = ttt3.GenMoves();
//Console.WriteLine($"{m.Count}");
//foreach (var mm in m)
//    Console.WriteLine(mm);

var se = new Searcher();
se.Search(ttt3, 0);
Console.WriteLine($"{se.bestMoves.Count} moves found");
foreach (var m in se.bestMoves)
    Console.WriteLine($"  {m}");
Console.WriteLine(se.bestMoves[0].Move);

//se.Root;

Console.WriteLine("----------------");
TreeTextFormatter.Format(
    Console.Out, se.Root,
    n => n.Children,
    n => $"{n.Score} {n.Move}",// {n.Move.WhoMoved}",// {n?.Move.From}",
    TreeTextFormatter.Style.Unicode
);

return;
#endif



#if true
var b1 = new RandomBot();
//var b1 = new SmartBot();
//var b2 = new RandomBot();
var b2 = new SmartBot();
Fight(b1,b2, 3);
return;
#endif

#if false
var se = new Searcher();
var ttt3 = new GameStateTTT(3);
se.Search(ttt3);
WriteLine($"nodes {se.nodes}");
#endif



void Fight(IBot bob, IBot don, int size)
{
    var (bobWins, donWins, draws) = (0, 0, 0);

    for (var game = 0; game < 100; ++game)
    {
        WriteLine();
        WriteLine("---------------------------------");

        Console.WriteLine(game);
        var odd = (game & 1) == 1;
        var g = new GameStateTTT(size);

        void dm()
        {
            var mv = g.GenMoves();
            //Console.WriteLine($"pre moves {mv.Count}");
        }

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
                dm();
                //g.Dump();
                dm();
                PlayOne(don, g);
                dm();
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

    void PlayOne(IBot b, GameStateTTT g)
    {
        var m = b.FindMove(g);
        Console.WriteLine($"Applying move {m}");
        g.DoMove(m);
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