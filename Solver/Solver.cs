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

        while (solutionsFound)
        {
            solutionsFound = false;
            playlist.Add(solvedCellsSolver);

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

                        Solution? next = solution.Next;
                        
                        while (next is not null)
                        {
                            solutionBag.Add(next);
                            next = next.Next;
                        }
                    }
                }
                
                // Found nothing so bail
                if (solutionBag.Count is 0)
                {
                    continue;
                }

                Console.WriteLine("**Stage**");
                // Process solutions
                HashSet<int> solutionCop = new(solutionBag.Count);
                foreach (Solution solution in solutionBag)
                {
                    if (!solutionCop.Add(solution.Cell))
                    {
                        continue;
                    }

                    if (puzzle.Update(solution))
                    {
                        newSolutions++;
                        Cell cell = solution.Cell;
                        Console.WriteLine($"{cell.Row},{cell.Column}: {solution.Value}; {solution.Solver}");
                    }
                    else
                    {
                        newSolutions++;
                        Cell cell = solution.Cell;
                        Console.Write($"{cell.Row},{cell.Column}; Removed: ");

                        foreach(var c in solution.Removed)
                        {
                            Console.Write($"{c}, ");
                        }

                        Console.WriteLine($"; {solution.Solver}");
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
                break;
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
