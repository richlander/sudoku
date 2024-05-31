using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BacktrackerOne;

/*
    Backtracker, based on array and collection data types.
    This is the baseline approach.
*/
public static class Backtracker
{
    public static bool Solve(int[] board, [NotNullWhen(true)] out int[]? solution)
    {
        if (!IsValid(board))
        {
            solution = null;
            return false;
        }

        solution = [.. board];
        return Solver(solution, 0) && IsValid(solution, true);
    }

    private static bool Solver(int[] board, int index)
    {
        if (board[index] > 0)
        {
            return index is 80 || Solver(board, index + 1);
        }

        var (row, column, box) = GetCellInfo(index);

        while (board[index] < 9)
        {
            board[index]++;
        
            if (IsValidRow(board, row) &&
                IsValidColumn(board, column) && 
                IsValidBox(board, box))
            {
                if (index is 80 || Solver(board, index + 1))
                {
                    return true;
                }
            }
        }

        board[index] = 0;
        return false;
    }

    private static bool IsValid(int[] board, bool testForEmpties = false)
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
            if (!IsValidCell(board, i))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidCell(int[] board, int index) => 
        IsValidRow(board, index) && 
        IsValidColumn(board, index) && 
        IsValidBox(board, index);

    private static bool IsValidRow(int[] board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index * 9;
        for (int i = offset; i < offset + 9; i++)
        {
            int value = board[i];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsValidColumn(int[] board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index;
        for (int i = 0; i < 9; i++)
        {
            int value = board[offset];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
            offset += 9;
        }

        return true;
    }

    private static bool IsValidBox(int[] board,  int index)
    {
        HashSet<int> cells = new(10);
        foreach (int cell in GetBoxCells(index))
        {
            int value = board[cell];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static int GetRowForCell(int index) => index / 9;

    private static int GetColumnForCell(int index) => index  % 9;

    private static int GetBoxForCell(int index) => index / 27 * 3 + index % 9 / 3;

    private static int GetFirstCellForBox(int index) => index / 3 * 27 + index % 3 * 3;

    private static IEnumerable<int> GetBoxCells(int index)
    {
        int offset = GetFirstCellForBox(index);
        for (int i = 0; i < 3; i++)
        {
            int cell = offset + (9 * i);
            yield return cell;
            yield return cell + 1;
            yield return cell + 2;
        }
    }

    private static Cell GetCellInfo(int index) => new(
        GetRowForCell(index),
        GetColumnForCell(index),
        GetBoxForCell(index)
    );
}

record struct Cell(int Row, int Column, int Box);
