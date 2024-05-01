using Sudoku;
using static System.Console;

if (args.Length is 0)
{
    WriteLine("Provide a puzzle or puzzle file as input.");
    return;
}

List<ISolver> solvers = [
    new SolvedCellsSolver(),
    new HiddenSinglesSolver(),
    new NakedPairsSolver(),
    new NakedTriplesSolver(),
    new HiddenPairsSolver(),
    new HiddenTriplesSolver(),
    new PointingPairsSolver(),
    new BoxLineReductionSolver(),
    new XWingSolver(),
    new YWingSolver(),
    new XYZWingSolver(),
    ];

string input = args[0];
if (File.Exists(input))
{
    ConsoleSolver.SolvePuzzles(File.ReadLines(input), solvers);
}
else if (input.Length is 81)
{
    Puzzle puzzle = new(input);
    ConsoleSolver.Solve(puzzle, solvers, false);
}
else
{
    WriteLine("Puzzle string is invalid");
}
