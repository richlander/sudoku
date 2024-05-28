using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.AspNet;

public class BacktrackerBenchmarks
{
    [ParamsSource(nameof(Boards))]
    public int[]? Board { get; set; }

    public static IEnumerable<int[]> Boards => PuzzleSource.GetPuzzles();

    [Benchmark]
    public bool BacktrackerBaseline() => Backtracker.BacktrackerOne.Solve(Board, out int[]? solution);
    
    [Benchmark]
    public bool BacktrackerDataOriented() => BacktrackerTwo.Backtracker.Solve(Board, out int[]? solution);

    [Benchmark]
    public bool BacktrackerCandidateBytes() => BacktrackerThree.Backtracker.Solve(Board, out int[]? solution);

    [Benchmark]
    public bool BacktrackerQuick() => BacktrackerFour.Backtracker.Solve(Board, out int[]? solution);
}