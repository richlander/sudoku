namespace Sudoku;
using static System.Console;

public static class ConsoleSolver
{
    public static void Solve(Puzzle puzzle, IReadOnlyList<ISolver> solvers, bool quiet = true)
    {
        Counts counts = new();
        foreach (var solution in Solver.Solve(puzzle, solvers))
        {
            PrintSolutionsAndUpdate(puzzle, solution, quiet, ref counts);
            WriteLine(puzzle);

            if (!puzzle.IsValid)
            {
                WriteLine("*****Puzzle is invalid.");
                return;
            }

        }

        bool solved = puzzle.IsSolved;
        bool valid = solved || puzzle.IsValid;
        WriteLine();
        WriteLine($"Cells solved: {counts.CellsSolved}; Solutions: {counts.TotalSolutions}; Complete: {solved}; Valid: {valid}");
        WriteLine(puzzle);
    }

    public static void SolvePuzzles(IEnumerable<string> lines, IReadOnlyList<ISolver> solvers, bool quiet = true)
    {

        int solutions = 0;
        int count = 0;
        foreach (string line in lines)
        {
            if (line.Length is 0)
            {
                continue;
            }

            count++;

            Puzzle puzzle = new(line);
            foreach (var solution in Solver.Solve(puzzle, solvers))
            {
                puzzle.UpdateBoard(solution);
            }

            bool solved = puzzle.IsSolved;
            bool valid = solved || puzzle.IsValid;

            if (solved)
            {
                solutions++;
            }
            else if (valid)
            {
                if (!quiet)
                {
                    WriteLine($"Incomplete: {count}; {line}");
                }
            }
            else
            {
                WriteLine($"Invalid: {count}; {line}");
            }
        }

        WriteLine();
        WriteLine($"Count: {count}; Solved: {solutions}");
    }

    private static Counts PrintSolutionsAndUpdate(Puzzle puzzle, Solution solution, bool quiet, ref Counts counts)
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
                if (!quiet)
                {
                    DrawSolution(puzzle, nextSolution);
                }
                WriteLine($"{cell.Row},{cell.Column} ({cell.Index, 2}); {nextSolution.Value}");
            }
            else if (nextSolution.RemovalCandidates is not null)
            {
                Cell cell = nextSolution.Cell;
                Write($"{cell.Row},{cell.Column} ({cell.Index, 2})");
                
                string demark = "";
                if (nextSolution.AlignedIndices is not null)
                {
                    Console.Write("; Aligned: ");
                    foreach (int index in nextSolution.AlignedIndices)
                    {
                        Write($"{demark}{index}");
                        demark = ", ";
                    }
                }

                demark = "";
                if (nextSolution.AlignedCandidates is not null)
                {
                    Console.Write("; Aligned Candidates: ");
                    foreach (int index in nextSolution.AlignedCandidates)
                    {
                        Write($"{demark}{index}");
                        demark = ", ";
                    }
                }

                Write("; Removals: ");

                demark = "";
                foreach(var c in nextSolution.RemovalCandidates)
                {
                    Write($"{demark}{c}");
                    demark = ", ";
                }

                WriteLine();
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
