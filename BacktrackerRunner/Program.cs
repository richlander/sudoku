
using System.ComponentModel;
using System.Diagnostics;
using PuzzleMultiDimensionalArray;

// if (!Console.IsInputRedirected)
// {
//     Console.WriteLine("Puzzle must be passed via stdin.");
// }

// var stream = Console.OpenStandardInput();
// StreamReader reader = new(stream);

string puzzle = "096040001100060004504810390007950043030080000405023018010630059059070830003590007";
Console.WriteLine($"Puzzle: {puzzle}");

var board = Utils.Utils.GetJaggedNumberPuzzle(Utils.Utils.GetNumberPuzzle(puzzle));

Stopwatch watch = Stopwatch.StartNew();
if (BacktrackerSeven.Backtracker.Solve(board, out int[][]? solution))
{
    Console.WriteLine("Puzzle solved");
}
else
{
    Console.WriteLine("Puzzle not solved");
}

watch.Stop();
Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds}");
