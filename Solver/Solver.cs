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
        Solution? solution = null;

        do
        {
            playlist.Add(solvedCellsSolver);
            solution = null;

            foreach(ISolver solver in playlist.Play())
            {
                // Search for solutions across board
                for (int i = 0; i < 81; i++)
                {
                    if (TrySolveCell(puzzle, solver, i, out Solution? s))
                    {
                        solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                        if (puzzle.IsSolved)
                        {
                            break;    
                        }
                    }
                }

                // Need to run SolverCellsSolver next
                if (solution is not null)
                {
                    yield return solution;
                    break;
                }
            }
        } while (solution is not null);
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

    public static bool TrySolveCellAndUpdate(Puzzle puzzle, List<ISolver> solvers, int index, [NotNullWhen(true)] out Solution? solution)
    {
        foreach(ISolver solver in solvers)
        {
            if (TrySolveCell(puzzle, solver, index, out solution))
            {
                puzzle.UpdateBoard(solution);
                return true;
            }
        }

        solution = null;
        return false;
    }
}
