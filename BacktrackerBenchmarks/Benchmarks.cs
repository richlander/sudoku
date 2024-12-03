using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;
using Sudoku;

namespace Benchmarks;

public class BacktrackerBenchmarks
{
    [ParamsSource(nameof(Puzzles))]
    public SudokuPuzzle Puzzle { get; set; } = PuzzleSource.Puzzles[0];

    public static List<SudokuPuzzle> Puzzles => PuzzleSource.Puzzles;

    [Benchmark(Baseline = true)]
    public bool BacktrackerBaseline() => BacktrackerOne.Backtracker.Solve(Puzzle.Board, out int[]? solution);
    
    [Benchmark]
    public bool BacktrackerSpanOverData() => BacktrackerTwo.Backtracker.Solve(Puzzle.Board, out int[]? solution);

    [Benchmark]
    public bool BacktrackerQuickBitTwiddler() => BacktrackerThree.Backtracker.Solve(Puzzle.Board, out int[]? solution);

    [Benchmark]
    public bool BacktrackerQuickBitTwiddler16() => BacktrackerFour.Backtracker.Solve(Puzzle.Board, out int[]? solution);

    [Benchmark]
    public bool BacktrackerMDArrayBaseline() => BacktrackerFive.Backtracker.Solve(Puzzle.MultiDimensionalBoard, out int[,]? solution);

    [Benchmark]
    public bool BacktrackerJaggedArrayBaseline() => BacktrackerSix.Backtracker.Solve(Puzzle.JaggedArrayBoard, out int[][]? solution);

    [Benchmark]
    public bool BacktrackerJaggedArrayOptimized() => BacktrackerSeven.Backtracker.Solve(Puzzle.JaggedArrayBoard, out int[][]? solution);


}