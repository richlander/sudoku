using Sudoku;
using static System.Console;

if (args.Length is 0)
{
    WriteLine("Provide a puzzle or puzzle file as input.");
    return;
}

List<ISolver> solvers = [
    new HiddenSinglesSolver(), 
    new NakedPairsSolver(), 
    new HiddenPairsSolver(),
    new PointedPairsSolver(),
    new BoxLineReductionSolver(),
    new XWingSolver()];

string input = args[0];
if (File.Exists(input))
{
    ConsoleSolver.Solve(File.ReadLines(input), solvers);
}
else if (input.Length is 81)
{
    Puzzle puzzle = new(input);
    ConsoleSolver.Solve(puzzle, solvers);
}
else
{
    WriteLine("Puzzle string is invalid");
}
