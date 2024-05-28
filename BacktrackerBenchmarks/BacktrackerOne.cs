using System.Text;

namespace Backtracker;

public static class BacktrackerOne
{
    public static bool Solve(ReadOnlySpan<int> puzzle, out int[]? solution)
    {
        int[] board = puzzle.ToArray();
        solution = board;
        if (!ValidateBoard(board))
        {
            return false;
        }

        return Solver(board, 0);
    }

    private static bool Solver(Span<int> board, int index)
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
                if (index is 80 && ValidateBoard(board))
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

    private static bool ValidateBoard(ReadOnlySpan<int> board)
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

    private static bool IsValid(ReadOnlySpan<int> board, int index) => IsValidRow(board, index) && IsValidColumn(board, index) && IsValidBox(board, index);

    private static bool IsValidRow(ReadOnlySpan<int> board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index * 9;
        ReadOnlySpan<int> range = board.Slice(offset, 9);
        foreach (int value in range)
        {
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidColumn(ReadOnlySpan<int> board, int index)
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

    private static bool IsValidBox(ReadOnlySpan<int> board,  int index)
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

    private static int GetBoxForCell(int index) => index / 27 * 3 + index % 9 / 3;

    private static int GetRowForCell(int index) => index / 9;

    private static int GetColumnForCell(int index) => index  % 9;

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
