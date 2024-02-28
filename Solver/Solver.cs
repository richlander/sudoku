using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

public static class Solver
{
    public static IEnumerable<Solution> Solve(Puzzle puzzle, IReadOnlyList<ISolver> solvers)
    {
        SolvedCellsSolver solvedCellsSolver = new();
        SolverPlaylist playlist = new(solvers);
        Solution? solutions = null;

        do
        {
            solutions = null;
            playlist.Add(solvedCellsSolver);

            foreach(ISolver solver in playlist.Play())
            {
                // Search for solutions across board
                for (int i = 0; i < 81; i++)
                {
                    if (TrySolveCell(puzzle, solver, i, out Solution? solution))
                    {
                        solutions = Puzzle.UpdateSolutionWithNextSolution(solutions, solution);
                        if (puzzle.IsSolved)
                        {
                            break;    
                        }
                    }
                }

                // Need to run SolverCellsSolver next
                if (solutions is not null)
                {
                    yield return solutions;
                    break;
                }
            }
        } while (solutions is not null);
    }

    public static bool TrySolveCell(Puzzle puzzle, ISolver solver, int index, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;

        if (puzzle.IsCellSolved(index))
        {
            return false;
        }

        Cell cell = puzzle.GetCell(index);
        return solver.TrySolve(puzzle, cell, out solution);
    }
}
