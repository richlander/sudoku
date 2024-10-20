using System.Diagnostics.CodeAnalysis;
using Sudoku;

namespace BacktrackerTwo;

/*
    Backtracker, based on array, collection, and span data types.
    Adds use of pre-computed data, and spans relative to baseline.
    Relies on a helper class for some of the more complicated Sudoku logic.
*/
public static class Backtracker
{
    public static bool Solve(ReadOnlySpan<int> board, [NotNullWhen(true)] out int[]? solution)
    {
        if (!IsValid(board))
        {
            solution = null;
            return false;
        }

        solution = [.. board];
        Puzzle puzzle = new(solution);
        return Solver(puzzle, 0) && IsValid(solution, true);
    }

    private static bool Solver(Puzzle puzzle, int index)
    {
        Span<int> board = puzzle.Board;
        if (board[index] > 0)
        {
            return index is 80 || Solver(puzzle, index + 1);
        }

        Cell cell = puzzle.Cells[index];

        foreach (int candidate in puzzle.GetCandidates(cell))
        {
            board[index] = candidate;
            if (index is 80 || Solver(puzzle, index + 1))
            {
                return true;
            }
        }

        board[index] = 0;
        return false;
    }

    private static bool IsValid(ReadOnlySpan<int> board, bool testForEmpties = false)
    {
        if (board.Length != 81)
        {
            return false;
        }

        if (testForEmpties && board.Contains(0))
        {
            return false;
        }

        for (int i = 0; i < 9; i++)
        {
            if (IsValidRowStride(board, i * 9) && 
                IsValidRowStride(board, i, 9) && 
                IsValidBox(board, i))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool IsValidRowStride(ReadOnlySpan<int> board, int index, int stride = 1)
    {
        HashSet<int> cells = new(10);
        for (int i = 0; i < 9; i++)
        {
            int value = board[index + i * stride];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidBox(ReadOnlySpan<int> board,  int index)
    {
        HashSet<int> cells = new(10);
        foreach (int cell in Puzzle.GetBoxIndices(index))
        {
            int value = board[cell];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }
}
