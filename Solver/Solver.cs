using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

public static class Solver
{

    public static void Solve(Puzzle puzzle, List<ISolver> solvers)
    {
        int solutions = 0;
        bool solved = true;

        while (solved)
        {
            solved = false;

            foreach(ISolver solver in solvers)
            {
                int startIndex = 0;
                while (TrySolve(puzzle, solver, startIndex, out int lastIndex, out Solution? solution))
                {
                    Cell cell = solution.Cell;
                    if (solution.Value > 0)
                    {
                        solutions++;
                        Console.WriteLine($"{cell.Row},{cell.Column}: {solution.Value}; {solution.Solver}");
                    }

                    puzzle.Update(solution);
                    solved = true;

                    if (puzzle.IsSolved)
                    {
                        Console.WriteLine($"Puzzle is solved!");
                        return;    
                    }

                    startIndex = lastIndex + 1;

                    if (lastIndex >= 80)
                    {
                        break;
                    }
                }

                if (puzzle.IsSolved)
                {
                    Console.WriteLine($"Puzzle is solved!");
                    return;
                }
            }
        }

        Console.WriteLine($"Final puzzle with {solutions} solutions applied");
        Console.WriteLine(puzzle.ToString());
    }

    public static bool TrySolve(Puzzle puzzle, ISolver solver, int startIndex, out int lastIndex, [NotNullWhen(true)] out Solution? solution)
    {
        lastIndex = 80;
        for (int i = startIndex; i <= lastIndex; i++)
        {
            if (TrySolveCell(puzzle, solver, i, out solution))
            {
                lastIndex = i;
                return true;
            }
        }

        solution = default;
        return false;
    }

    public static bool TrySolveCell(Puzzle puzzle, ISolver solver, int index, [NotNullWhen(true)] out Solution? solution)
    {
        solution = default;

        if (puzzle.IsCellSolved(index))
        {
            return false;
        }

        BoxCell boxCell = puzzle.BoxCells(index);

        return solver.TrySolve(puzzle, boxCell, out solution);
    }
}
