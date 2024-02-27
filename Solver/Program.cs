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
    int solutions = 0;
    int count = 0;
    foreach (string line in File.ReadLines(input))
    {
        if (line.Length is 0)
        {
            continue;
        }

        count++;

        Puzzle puzzle = new(line);
        ConsoleSolver.SolveQuietly(puzzle, solvers);

        bool solved = puzzle.IsSolved;
        bool valid = solved || puzzle.IsValid;

        if (solved)
        {
            solutions++;
        }
        else if (valid)
        {
            WriteLine($"Incomplete: {count}; {line}");
        }
        else
        {
            WriteLine($"Invalid: {count}; {line}");
        }
    }

    WriteLine();
    WriteLine($"Count: {count}; Solved: {solutions}");
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
