namespace Sudoku;
using static System.Console;

public static class ConsoleSolver
{
    public static void Solve(Puzzle puzzle, IReadOnlyList<ISolver> solvers, bool drawSolution = true)
    {
        Counts counts = new();
        foreach (var solution in Solver.Solve(puzzle, solvers))
        {
            PrintSolutionsAndUpdate(puzzle, solution, drawSolution, ref counts);
        }

        bool solved = puzzle.IsSolved;
        bool valid = solved || puzzle.IsValid;
        WriteLine();
        WriteLine($"Solutions: {counts.TotalSolutions}; Cells solved: {counts.CellsSolved} Complete: {solved}; Valid: {valid}");
        WriteLine(puzzle);
    }

    public static void SolveQuietly(Puzzle puzzle, IReadOnlyList<ISolver> solvers)
    {
        foreach (var solution in Solver.Solve(puzzle, solvers))
        {
            puzzle.UpdateBoard(solution);
        }
    }

    private static Counts PrintSolutionsAndUpdate(Puzzle puzzle, Solution solution, bool drawSolution, ref Counts counts)
    {
        WriteLine();
        WriteLine($"**** {solution.Solver} -- {CountSolutions(solution)} solution(s)**");
        WriteLine();
        Solution? nextSolution = solution;
        // Process solutions
        while (nextSolution is not null)
        {
            if (puzzle.UpdateCell(nextSolution))
            {
                counts.CellsSolved++;
                Cell cell = nextSolution.Cell;
                if (drawSolution)
                {
                    DrawSolution(puzzle, nextSolution);
                }
                WriteLine($"{cell.Row},{cell.Column} ({cell.Index, 2}); {nextSolution.Value}");
            }
            else
            {
                Cell cell = nextSolution.Cell;
                Write($"{cell.Row},{cell.Column} ({cell.Index, 2}); ");
                string demark = "";
                foreach(var c in nextSolution.Removed)
                {
                    Write($"{demark}{c}");
                    demark = ", ";
                }

                WriteLine(" ; Removals");
            }

            counts.TotalSolutions++;
            nextSolution = nextSolution.Next;
        }

        puzzle.UpdateCandidates();
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
