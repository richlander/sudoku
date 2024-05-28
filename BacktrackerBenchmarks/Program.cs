// using System.Diagnostics;

// string puzzle = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
// int[] board = Utils.GetNumberPuzzle(puzzle);

// var watch = Stopwatch.StartNew();
// if (BacktrackerThree.Backtracker.Solve(board, out int[]? solution))
// {
//     watch.Stop();
//     Console.WriteLine($"Puzzle is valid; Time: {watch.Elapsed.TotalMilliseconds}");

// }
// else
// {
//     Console.WriteLine("Puzzle is invalid");
// }

// if (solution is not null)
// {
//     Utils.PrintBoard(solution);
// }

using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<BacktrackerBenchmarks>();
