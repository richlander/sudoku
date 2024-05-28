using System;
using System.Diagnostics;

string puzzle = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
int[] board = new int[81];

if (puzzle.Length != 81)
{
    throw new Exception();
}

for (int i = 0; i < puzzle.Length; i++)
{
    board[i] = puzzle[i] - '0';
}


Stopwatch stopwatch= Stopwatch.StartNew();
if (BTSolverOne.SolvePuzzle(board, out int[]? solution))
{
    stopwatch.Stop();
    Utils.PrintBoard(board);
    Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
}
else
{
    Console.WriteLine("Board solving failed");
}

