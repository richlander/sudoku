using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

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
            int startIndex = 0;
            
            foreach(ISolver solver in solvers)
            {

                while (TrySolve(puzzle, solver, startIndex, out int nextIndex, out Solution? solution))
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

    public static bool TrySolve(Puzzle puzzle, ISolver solver, int startIndex, out int nextIndex, [NotNullWhen(true)] out Solution? solution)
    {
        nextIndex = 0;
        solution = default;

        for (int i = startIndex; i < 81; i++)
        {
            if (TrySolveCell(puzzle, solver, i, out solution))
            {
                nextIndex = i < 80 ? i + 1 : 0;
                return true;
            }
        }

        return false;
    }

    public static bool TrySolveCell(Puzzle puzzle, ISolver solver, int index, [NotNullWhen(true)] out Solution? solution)
    {
        solution = default;
        int box = Cell.GetBoxForCell(index);
        int row = Cell.GetRowForCell(index);
        int column = Cell.GetColumnForCell(index);
        CellLocation location = new(index, row, column, box);

        return solver.TrySolve(puzzle, location, out solution);
    }
}
