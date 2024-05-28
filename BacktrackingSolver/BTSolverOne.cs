using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Sudoku;

public class BTSolverOne
{
    public static bool SolvePuzzle(ReadOnlySpan<int> puzzle, [NotNullWhen(true)] out int[]? solution)
    {
        int[] board = [..puzzle];

        if (!ValidateBoard(board))
        {
            solution = null;
            return false;
        }

        if (Solver(board, 0))
        {
            solution = board;
            return true;
        }
        else
        {
            solution = null;
            return false;
        }
    }

    private static bool Solver(Span<int> board, int index)
    {
        if (board[index] > 0)
        {
            return index is 80 || Solver(board, index + 1);
        }

        while (board[index] < 9)
        {
            board[index]++;
                    
            if (ValidateBoard(board))
            {
                if (index is 80)
                {
                    return true;
                }

                if (Solver(board, index + 1))
                {
                    return true;
                }
            }
        }

        board[index] = 0;
        return false;
    }

    private static bool ValidateBoard(Span<int> board)
    {
        for (int i = 0; i < 9; i++)
        {
            if (!IsValid(board, i))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValid(Span<int> board, int index) => IsValidRow(board, index) && IsValidColumn(board, index) && IsValidBox(board, index);

    private static bool IsValidRow(Span<int> board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index * 9;
        Span<int> range = board.Slice(offset, 9);
        foreach (int value in range)
        {
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidColumn(Span<int> board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index;
        for (int i = 0; i < 9; i++)
        {
            int value = board[offset];
            if (! (value is 0 || cells.Add(value)))
            {
                return false;
            }
            offset += 9;
        }

        return true;
    }

    private static bool IsValidBox(Span<int> board, int index)
    {
        HashSet<int> cells = new(10);
        Span<int> indices = Puzzle.IndicesByBox.AsSpan(index * 9, 9);
        foreach (int cell in indices)
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