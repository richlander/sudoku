namespace BacktrackerOne;

/*
    Backtracker, based on array and collection data types.
    It is self-sufficient, not relying on any external helpers.
    This is the baseline approach.
*/
public static class Backtracker
{
    public static void Solve(int[] board) => Solver(board, new(0, 0, 0, 0));

    private static bool Solver(int[] board, Cell cell)
    {
        var (index, row, column, box) = cell;

        if (board[index] > 0)
        {
            return TryNext(board, cell);
        }

        while (board[index] < 9)
        {
            board[index]++;

            if (IsValidRow(board, row) &&
                IsValidColumn(board, column) &&
                IsValidBox(board, box))
            {
                if (TryNext(board, cell))
                {
                    return true;
                }
            }
        }

        board[index] = 0;
        return false;

        static bool TryNext(int[] board, Cell cell) => MoveNext(cell, out Cell nextCell) || Solver(board, nextCell);

    }

    private static bool MoveNext(Cell cell, out Cell nextCell)
    {
        var (i, x, y, z) = cell;

        if (i is 80)
        {
            nextCell = cell;
            return false;
        }

        if (y < 8)
        {
            y++;

            if (y % 3 is 0)
            {
                z++;
            }
        }
        else if (x < 8)
        {
            x++;
            y = 0;
            z = x / 3 * 3;
        }

        nextCell = new(i++, x, y, z);
        return true;
    }

    private static bool IsValidRow(ReadOnlySpan<int> board, int index)
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

    private static bool IsValidColumn(ReadOnlySpan<int> board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index;
        for (int i = 0; i < 9; i++)
        {
            int value = board[index];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
            offset += 9;
        }

        return true;
    }

    private static bool IsValidBox(ReadOnlySpan<int> board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index / 3 * 27 + index % 3 * 3;
        // Three rows
        for (int i = 0; i < 3; i++)
        {
            int cell = offset + (9 * i);
            // Three cells
            for (int j = 0; j < 3; j++)
            {
                int value = board[offset + j];
                if (!(value is 0 || cells.Add(value)))
                {
                    return false;
                }
            }
        }

        return true;
    }

    record Cell(int Index, int Row, int Column, int Box);
}
