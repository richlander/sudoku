using System.Diagnostics.CodeAnalysis;
using Sudoku;
using Puzzle = PuzzleQuick.Puzzle;

namespace BacktrackerThree;

/*
    Backtracker, based on array, collection, span, and integer data types.
    Adds use of pre-computed data, spans, bit twiddling, and ref ints relative to baseline.
*/

public static class Backtracker
{
    public static bool Solve(ReadOnlySpan<int> board, [NotNullWhen(true)] out int[]? solution)
    {
        if (!ValidateBoard(board))
        {
            solution = null;    
            return false;
        }

        Puzzle puzzle = new(board);
        return Solver(puzzle, board, 0, out solution) && ValidateBoard(solution, true);
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
        int viewValues = puzzle.GetValuesInView(cell);
        int oldValue = 0;
        int valuesMask = 1;

        for (int i = 1; i < 10; i++)
        {
            bool found = (viewValues & valuesMask) > 0;
            valuesMask <<= 1;

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

    private static bool ValidateBoard(ReadOnlySpan<int> board, bool testForEmpties = false)
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
            if (!IsValid(board, i))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValid(ReadOnlySpan<int> board, int index) => 
        IsValidRowStride(board, index * 9) && 
        IsValidRowStride(board, index, 9) && 
        IsValidBox(board, index);

    private static bool IsValidRowStride(ReadOnlySpan<int> board, int index, int stride = 1)
    {
        int bitMask = 0;

        for (int i = 0; i < 9; i++)
        {
            int value = board[index + i * stride];
            if (value is 0)
            {
                continue;
            }

            int bit = 1 << value;
            bitMask ^= bit;
            if ((bitMask & bit) == 0)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidBox(ReadOnlySpan<int> board,  int index)
    {
        int bitMask = 0;
        foreach (int value in Puzzle.GetBoxIndices(index))
        {
            if (value is 0)
            {
                continue;
            }

            int bit = 1 << value;
            bitMask ^= bit;
            if ((bitMask & bit) == 0)
            {
                return false;
            }
        }

        return true;
    }
}
