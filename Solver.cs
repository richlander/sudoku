namespace Sudoku;

public static class Solver
{

    public static void Solve(Puzzle puzzle, List<ISolver> solvers)
    {
        int steps = 0;
        bool solved = true;

        while (solved)
        {
            solved = false;
            foreach(ISolver solver in solvers)
            {

                while (solver.TrySolve(puzzle, out Solution? solution))
                {
                    Console.WriteLine($"{solution.Row},{solution.Column}: {solution.Value}; {solution.Solver}");
                    puzzle.Update(solution);
                    steps++;
                    solved |= true;

                    if (puzzle.IsSolved)
                    {
                        Console.WriteLine($"Puzzle is solved!");
                        return;    
                    }
                }

                if (puzzle.IsSolved)
                {
                    Console.WriteLine($"Puzzle is solved!");
                    return;
                }
            }
        }

        Console.WriteLine($"Final puzzle after {steps} steps");
        Console.WriteLine(puzzle.ToString());
    }
}
