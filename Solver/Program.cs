using Sudoku;
using static System.Console;

// string p = "000001030231090000065003100678924300103050006000136700009360570006019843300000000";
// args = [p];

if (args.Length is 0)
{
    WriteLine("Provide a puzzle or puzzle file as input.");
    return;
}

List<ISolver> solvers = [
    new HiddenSinglesSolver(),
    new NakedPairsSolver(),
    new NakedTriplesSolver(),
    new HiddenPairsSolver(),
    new HiddenTriplesSolver(),
    new PointingPairsSolver(),
    new BoxLineReductionSolver(),
    new XWingSolver()];

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
