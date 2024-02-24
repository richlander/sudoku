using Sudoku;

if (args.Length is 0)
{
    Console.WriteLine("Provide a puzzle or puzzle file as input.");
    return;
}

List<ISolver> solvers = [
    new HiddenSinglesSolver(), 
    new NakedPairsSolver(), 
    new HiddenPairsSolver(),
    new PointedPairsSolver(),
    new BoxLineReductionSolver(),
    new XWingSolver()];

string puzzle = args[0];
if (File.Exists(puzzle))
{
    int solutions = 0;
    int count = 1;
    foreach (string line in File.ReadLines(puzzle))
    {
        if (line.Length is 0)
        {
            continue;
        }
        else if (line.Length != 81)
        {
            Console.WriteLine(line);
            Console.WriteLine($"Invalid (length: {line.Length}): {count}");
            return;
        }

        Console.WriteLine($"{count}: {line}");

        Puzzle p = new(line);
        bool solved = Solver.Solve(p, solvers);

        if (solved)
        {
            solutions++;
        }
        else if (!p.IsValid)
        {
            Console.WriteLine($"Invalid: {count}");
            Console.WriteLine($"Input: {line}");
            Console.WriteLine($"Final: {p}");
            return;
        }
        else
        {
            Console.WriteLine($"Failed: {count}; {line}");
        }
        count++;
    }

    Console.WriteLine($"Count: {count}; Solutions: {solutions}");
}
else if (puzzle.Length is 81)
{
    Puzzle p = new(puzzle);
    Solver.Solve(p, solvers);  
}
else
{
    Console.WriteLine("Puzzle is invalid");
    return;
}
