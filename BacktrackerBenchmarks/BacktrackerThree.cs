using Sudoku;

namespace BacktrackerThree;

public static class Backtracker
{
    public static bool Solve(ReadOnlySpan<int> puzzleInput, out int[]? solution)
    {
        if (!ValidateBoard(puzzleInput))
        {
            solution = null;
            return false;
        }

        solution = puzzleInput.ToArray();
        Puzzle puzzle = new(solution);
        return Solver(puzzle, 0) && ValidateBoard(solution);
    }

    private static bool Solver(Puzzle puzzle, int index)
    {
        Span<int> board = puzzle.Board;
        if (board[index] > 0)
        {
            return index is 80 || Solver(puzzle, index + 1);
        }

        Cell cell = puzzle.Cells[index];
        int candidates = puzzle.GetCandidatesQuick(cell);
        int candidateMask = 1;

        for (int i = 1; i < 10; i++)
        {
            bool found = (candidates & candidateMask) > 0;
            candidateMask <<= 1;

            if (found)
            {
                continue;
            }

            board[index] = i;

            if (index is 80 ||
                Solver(puzzle, index + 1))
            {
                return true;
            }
        }

        board[index] = 0;
        return false;
    }

    private static IEnumerable<int> GetCandidates(int candidates)
    {
        for (int i = 1; i < 10; i++)
        {
            var result = candidates & PuzzleData.Masks[i];
            if (result > 0)
            {
                yield return i;
            }
        }
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
