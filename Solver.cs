namespace Sudoku;

public static class Solver
{

    public static void Solve(Puzzle puzzle, ISolver solver)
    {
        int steps = 0;
        bool solved = false;
        while (!(solved = puzzle.IsSolved) && solver.TrySolve(puzzle, out Solution? solution))
        {
            Console.WriteLine($"{solution.Row},{solution.Column}: {solution.Value}; {solution.Solver}");
            puzzle.Update(solution);
            steps++;
        }

        if (solved)
        {
            Console.WriteLine($"Puzzle is solved!");
        }
        else
        {
            Console.WriteLine($"Puzzle has {81 - puzzle.SolvedCells} cells unsolved.");
        }

        Console.WriteLine($"Final puzzle after {steps} steps");
        Console.WriteLine(puzzle.ToString());
    }
}