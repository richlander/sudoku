namespace Sudoku;
using static System.Console;

public static class ConsoleSolver
{
    public static bool Solve(Puzzle puzzle, IReadOnlyList<ISolver> solvers, bool drawSolution = true)
    {
        Counts counts = new();
        foreach (var solution in Solver.Solve(puzzle, solvers))
        {
            PrintSolutionAndUpdate(puzzle, solution, drawSolution, ref counts);
        }

        bool solved = puzzle.IsSolved;
        bool valid = solved || puzzle.IsValid;
        WriteLine($"Solutions: {counts.TotalSolutions}; Cells solved: {counts.CellsSolved} Complete: {solved}; Valid: {valid}");
        WriteLine(puzzle);
        return solved;
    }

    public static void SolveQuietly(Puzzle puzzle, IReadOnlyList<ISolver> solvers)
    {
        foreach (var solution in Solver.Solve(puzzle, solvers))
        {
            puzzle.UpdateAndUnwrapSolutions(solution);
        }
    }

    private static Counts PrintSolutionAndUpdate(Puzzle puzzle, Solution solution, bool drawSolution, ref Counts counts)
    {
        WriteLine();
        WriteLine($"****Batch -- {CountSolutions(solution)} solution(s)**");
        WriteLine();
        Solution? sol = solution;
        // Process solutions
        while (sol is not null)
        {
            if (puzzle.Update(sol))
            {
                counts.CellsSolved++;
                Cell cell = sol.Cell;
                if (drawSolution)
                {
                    DrawSolution(puzzle, sol);
                }
                WriteLine($"Solution; {cell.Row},{cell.Column} ({cell.Index}); {sol.Value}; {sol.Solver}");
            }
            else
            {
                Cell cell = sol.Cell;
                Write($"Removals; {cell.Row},{cell.Column} ({cell.Index}); ");
                string demark = "";
                foreach(var c in sol.Removed)
                {
                    Write($"{demark}{c}");
                    demark = ", ";
                }

                WriteLine($"; {sol.Solver}");
            }

            counts.TotalSolutions++;
            sol = sol.Next;
        }

        return counts;
    }

    public static void DrawSolution(Puzzle puzzle, Solution solution)
    {
        WriteLine(solution.Solver);
        PrintColumnSolution(solution);

        for (int i = 0; i < 9; i++)
        {
            List<int> row = puzzle.GetRowValues(i).ToList();

            if (i == 3 || i == 6)
            {
                WriteLine("------+-------+------");
            }
            
            for (int j = 0; j < 9; j++)
            {
                if (j == 3 || j == 6)
                {
                    Write($"| {row[j]} ");
                }
                else if (j == 8)
                {
                    Write($"{row[j]}");
                }
                else
                {
                    Write($"{row[j]} ");
                }
            }

            if (i == solution.Cell.Row)
            {
                Write("*");
            }
            
            WriteLine();
        }
        
        WriteLine();

        void PrintColumnSolution(Solution solution)
        {
            for(int i = 0; i < solution.Cell.Column; i++)
            {
                Write("  ");
                if (i == 2 || i == 5)
                {
                    Write("  ");
                }
            }
            Write("*");
            WriteLine();
        }
    }

    public static int CountSolutions(Solution solution)
    {
        int count = 0;
        Solution? sol = solution;
        while (sol is not null)
        {
            count++;
            sol = sol.Next;
        }
        return count;
    }
}

public record struct Counts(int CellsSolved, int TotalSolutions);
