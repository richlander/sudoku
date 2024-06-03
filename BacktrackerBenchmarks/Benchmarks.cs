using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using Sudoku;

public class BacktrackerBenchmarks
{
    [ParamsSource(nameof(Puzzles))]
    public SudokuPuzzle Puzzle { get; set; } = PuzzleSource.Puzzles[0];

    public static IEnumerable<SudokuPuzzle> Puzzles => PuzzleSource.Puzzles;

    [Benchmark]
    public bool BacktrackerBaseline() => BacktrackerOne.Backtracker.Solve(Puzzle.Board, out int[]? solution);
    
    [Benchmark]
    public bool BacktrackerSpanOverData() => BacktrackerTwo.Backtracker.Solve(Puzzle.Board, out int[]? solution);


    [Benchmark]
    public bool BacktrackerQuickBitTwiddler() => BacktrackerThree.Backtracker.Solve(Puzzle.Board, out int[]? solution);
}