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

bool solveQuietly = false;

string input = args[0];
if (File.Exists(input))
{
    int solutions = 0;
    int count = 1;
    foreach (string line in File.ReadLines(input))
    {
        if (line.Length is 0)
        {
            continue;
        }

        WriteLine($"{count}: {line}");
        count++;

        SolvePuzzle(line, solvers);
    }

    WriteLine($"Count: {count}; Solutions: {solutions}");
}
else if (input.Length is 81)
{
    SolvePuzzle(input, solvers);
}

void SolvePuzzle(string board, IReadOnlyList<ISolver> solvers)
{
    if (board.Length != 81)
    {
        WriteLine("Puzzle string is invalid");
    }

    Puzzle puzzle = new(board);
    if (solveQuietly)
    {
        ConsoleSolver.SolveQuietly(puzzle, solvers);
        if (!puzzle.IsValid)
        {
            WriteLine("Puzzle is not valid.");
            WriteLine(puzzle);
        }
    }
    else
    {
        ConsoleSolver.Solve(puzzle, solvers);
    }
}