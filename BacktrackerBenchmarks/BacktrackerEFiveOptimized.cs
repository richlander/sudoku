using System.Diagnostics.CodeAnalysis;
using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Perfolizer.Mathematics.SignificanceTesting;
using PuzzleMultiDimensionalArray;
using Sudoku;
using Puzzle = PuzzleMultiDimensionalArray.Puzzle;
using Cell = PuzzleMultiDimensionalArray.Cell;

namespace BacktrackerFiveOptimized;

/*
    Backtracker, based on array, collection, span, and integer data types.
    Adds use of pre-computed data, spans, bit twiddling, and ref ints relative to baseline.
    Relies on a helper class for some of the more complicated Sudoku logic.

    This implementation is built on the following premises:

    - We can represent the units (rows, columns, boxes) as a set of 9 cell lists, with legal values 0-9.
    - The order of the cells doesn't matter. We just need to know if a given value is present.
    - Using these lists, we can determine which values are in view to produce candidate lists for a given cell.
    - Given the use of recursion, the stack represents the puzzle with all the correct final values.
    - A "solution" array can be created very late, to collect the final puzzle data that the stack contains.
*/

public static class Backtracker
{   
    public static bool Solve(ReadOnlySpan<int> board, [NotNullWhen(true)] out int[,]? solution)
    {
        if (!IsValid(solution))
        {
            solution = null;
            return false;
        }
        
        solution = Utils.Utils.GetMultiDimensionalNumberPuzzle(board);
        Puzzle puzzle = new(solution);
        return Solver(puzzle, solution, new(0, 0, 0), out solution) || IsValid(solution, true);
    }

    private static bool Solver(Puzzle puzzle, int[,] board, Cell cell, out int[,]? solution)
    {
        solution = null;
        var (x, y, _) = cell;

        if (board[x, y] > 0)
        {
            if (IsLastCell(cell))
            {
                solution = GetSolution(board[x, y]);
                return true;
            }

            if (TryNext(puzzle, board, cell, out solution))
            {
                if (solution is not null)
                {
                    solution[x, y] = board[x, y];
                }

                return true;
            }

            return false;
        }

        int inViewValues = puzzle.GetValuesInView(cell);
        int previousValue = 0;
        int valuesMask = 1;

        for (int i = 1; i < 10; i++)
        {
            valuesMask <<= 1;
            bool found = (inViewValues & valuesMask) > 0;

            if (found)
            {
                continue;
            }

            puzzle.UpdateCell(cell, previousValue, i);
            previousValue = i;

            if (IsLastCell(cell))
            {
                solution = GetSolution(i);
                return true;
            }
            
            if (TryNext(puzzle, board, cell, out solution))
            {
                if (solution is not null)
                {
                    solution[x, y] = board[x, y];
                }

                return true;
            }
        }

        puzzle.UpdateCell(cell, previousValue, 0);
        return false;

        static bool TryNext(Puzzle puzzle, int[,] board, Cell cell, out int[,]? solution)
        {
            solution = null;

            if (!MoveIndexNext(cell, out Cell nextCell))
            {
                return false;
            }
            
            return Solver(puzzle, board, nextCell, out solution);
        }
    }

    public static int[,] GetSolution(int value)
    {
        var solution = new int[9, 9];
        solution[8, 8] = value;
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

    private static bool IsLastCell(Cell cell) => cell.Row is 8 && cell.Column is 8;

    private static bool MoveIndexNext(Cell cell, out Cell nextCell)
    {
        var (x, y, _) = cell;

        if (y < 8)
        {
            int index = x * 9 + y +1;
            nextCell = PuzzleDataMD.Cells[index];
            return true;
        }
        else if (x < 8)
        {
            int index = (x + 1) * 9 + y;
            nextCell = PuzzleDataMD.Cells[index];
            return true;
        }

        nextCell = cell;
        return false;
    }
}
