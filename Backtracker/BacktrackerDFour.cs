using System.Diagnostics.CodeAnalysis;
using Sudoku;
using Puzzle = PuzzleQuick16.Puzzle;

namespace BacktrackerFour;

/*
    Backtracker, based on array, collection, span, and integer data types.
    Adds use of pre-computed data, spans, bit twiddling, and ref ints relative to baseline.
    Relies on a helper class for some of the more complicated Sudoku logic.

    This implementation is built on the following premises:

    - Units (rows, columns, boxes) can represented as a set of 9-cell lists, with legal values 0-9.
    - The order of the cells doesn't matter, only whether a given value is present.
    - Using these lists, a candidate list can be produced for a given cell using the values that are in view.
    - Given the use of recursion, the stack represents the puzzle with all the correct final values.
    - A "solution" array can be created very late, to collect the final puzzle data based on stack data.

    This implementation differs from BacktrackerThree by using a data souce oriented around 16 count spans
    instead of 9. This approach is bsed on the idea that algorithms based on powers of 2 are more efficient
    that those based on powers of 3. In this case, the algorithm is based on strides of 16 vs 9.
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

        Puzzle puzzle = new(board);
        return Solver(puzzle, board, 0, out solution) && IsValid(solution, true);
    }

    private static bool Solver(Puzzle puzzle, ReadOnlySpan<int> board, int index, out int[]? solution)
    {
        solution = null;
        if (board[index] > 0)
        {
            if (index is 80)
            {
                solution = GetSolution(board[index]);
                return true;
            }

            if (Solver(puzzle, board, index + 1, out solution))
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
        int previousValue = 0;
        int valuesMask = 1;

        for (int i = 1; i < 10; i++)
        {
            valuesMask <<= 1;
            bool found = (viewValues & valuesMask) > 0;

            if (found)
            {
                continue;
            }

            puzzle.UpdateCell(cell, previousValue, i);
            previousValue = i;

            if (index is 80)
            {
                solution = GetSolution(i);
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

        puzzle.UpdateCell(cell, previousValue, 0);
        return false;
    }

    public static int[] GetSolution(int value)
    {
        var solution = new int[81];
        solution[80] = value;
        return solution;
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

        ReadOnlySpan<int> rows = PuzzleData16.IndicesByRow;
        ReadOnlySpan<int> columns = PuzzleData16.IndicesByColumn;
        ReadOnlySpan<int> boxes = PuzzleData16.IndicesByBox;

        for (int i = 0; i < 9; i++)
        {
            if (IsValidLine(board, rows.Slice(i * 16, 9)) && 
                IsValidLine(board, columns.Slice(i * 16, 9)) && 
                IsValidLine(board, boxes.Slice(i * 16, 9)))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool IsValidLine(ReadOnlySpan<int> board, ReadOnlySpan<int> indices)
    {
        int bitMask = 0;
        foreach (int value in indices)
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
