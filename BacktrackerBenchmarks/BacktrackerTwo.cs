using Sudoku;

namespace BacktrackerTwo;

public static class Backtracker
{
    public static bool Solve(ReadOnlySpan<int> puzzleInput, out int[]? solution)
    {
        Puzzle puzzle = new(puzzleInput.ToArray());
        solution = puzzle.Board;
        if (!ValidateBoard(puzzle.Board))
        {
            return false;
        }

        return Solver(puzzle, 0);
    }

    private static bool Solver(Puzzle puzzle, int index)
    {
        Span<int> board = puzzle.Board;
        if (board[index] > 0)
        {
            return index is 80 && ValidateBoard(board) || Solver(puzzle, index + 1);
        }

        Cell cell = puzzle.Cells[index];

        foreach (int candidate in puzzle.GetCandidates(cell))
        {
            board[index] = candidate;

            if (index is 80)
            {
                return ValidateBoard(board);
            }
            
            if (Solver(puzzle, index + 1))
            {
                return true;
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
