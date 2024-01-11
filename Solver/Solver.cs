using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

public static class Solver
{

    public static void Solve(Puzzle puzzle, List<ISolver> solvers)
    {
        int solutions = 0;
        bool solutionsFound = true;
        SolvedCellsSolver solvedCellsSolver = new();

        SolverPlaylist playlist = new(solvers);
        playlist.Add(solvedCellsSolver);

        while (solutionsFound)
        {
            solutionsFound = false;

            foreach(ISolver solver in playlist.Play())
            {
                int newSolutions = 0;
                List<Solution> solutionBag = new(12);

                // Search for solutions across board
                for (int i = 0; i < 81; i++)
                {
                    if (TrySolveCell(puzzle, solver, i, out Solution? solution))
                    {
                        solutionBag.Add(solution);
                    }
                }
                
                // Found nothing so bail
                if (solutionBag.Count is 0)
                {
                    continue;
                }

                Console.WriteLine("**Stage**");
                // Process solutions
                foreach (Solution solution in solutionBag)
                {
                    if (puzzle.Update(solution))
                    {
                        newSolutions++;
                        Cell cell = solution.Cell;
                        Console.WriteLine($"{cell.Row},{cell.Column}: {solution.Value}; {solution.Solver}");
                    }
                }

                solutions += newSolutions;
                
                if (puzzle.IsSolved)
                {
                    Console.WriteLine($"Puzzle is solved!");
                    return;    
                }

                solutionsFound |= newSolutions > 0;
                puzzle.UpdateCandidates();

                playlist.Add(solvedCellsSolver);
            }
        }

        Console.WriteLine($"Final puzzle with {solutions} solutions applied");
        Console.WriteLine(puzzle.ToString());
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
