using Sudoku;
using static System.Console;

string p = "400000938032094100095300240370609004529001673604703090957008300003900400240030709";

args = [p];

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
