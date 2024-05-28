using Sudoku;

namespace BacktrackerFour;

public static class Backtracker
{
    public static bool Solve(ReadOnlySpan<int> board, out int[]? solution)
    {
        if (!ValidateBoard(board))
        {
            solution = null;    
            return false;
        }

        Puzzle puzzle = new(board);

        return Solver(puzzle, board, 0, out solution) && ValidateBoard(solution);
    }

    private static bool Solver(Puzzle puzzle, ReadOnlySpan<int> board, int index, out int[]? solution)
    {
        solution = null;

        if (board[index] > 0)
        {
            if (index is 80)
            {
                solution = new int[81];
                solution[80] = board[index];
                return true;
            }

            if(Solver(puzzle, board, index + 1, out solution))
            {
                if (solution is not null)
                {
                    solution[index] = board[index];
                }

                return true;
            }

            return false;
        }

        Cell cell = puzzle.Cells[index];
        int candidates = puzzle.GetCandidates(cell);
        int oldValue = 0;
        int candidateMask = 1;

        for (int i = 1; i < 10; i++)
        {
            bool found = (candidates & candidateMask) > 0;
            candidateMask <<= 1;

            if (found)
            {
                continue;
            }

            puzzle.UpdateCell(cell, oldValue, i);
            oldValue = i;

            if (index is 80)
            {
                solution = new int[81];
                solution[80] = i;
                return true;
            }
            
            if (Solver(puzzle, board, index + 1, out solution))
            {
                if (solution is not null)
                {
                    solution[cell] = i;
                }

                return true;
            }
        }

        puzzle.UpdateCell(cell, oldValue, 0);
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
