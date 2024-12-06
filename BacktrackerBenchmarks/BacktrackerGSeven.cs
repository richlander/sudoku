using System.Diagnostics.CodeAnalysis;
using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Perfolizer.Mathematics.SignificanceTesting;
using Sudoku;
using Puzzle = PuzzleMultiDimensionalArray.Puzzle;

namespace BacktrackerSeven;

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
    public static bool Solve(ReadOnlySpan<int> board, [NotNullWhen(true)] out int[][]? solution)
    {
        solution = Utils.Utils.GetJaggedNumberPuzzle(board);

        if (!IsValid(solution))
        {
            return false;
        }

        return Solver(solution, new(0, 0)) || IsValid(solution, true);
    }

    private static bool Solver(int[][] board, Cell cell)
    {
        var (x, y) = cell;

        if (board[x][y] > 0)
        {
            return TryNext(board, cell);
        }

        if (!CellInfoCells.TryGetValue(cell, out var cellInfo))
        {
            cellInfo = GetCellInfo(cell);
            CellInfoCells.Add(cell, cellInfo);
        }

        var (row, column, box) = cellInfo;

        while (board[x][y] < 9)
        {
            board[x][y]++;
        
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

        board[x][y] = 0;
        return false;

        static bool TryNext(int[][] board, Cell cell) => !MoveIndexNext(cell, out Cell nextCell) || Solver(board, nextCell);
    }

    private static bool IsValid(int[][] board, bool testForEmpties = false)
    {
        if (testForEmpties)
        {
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                if (board[i][j] is 0)
                {
                    return false;
                }
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (IsValidRow(board, i) && 
                IsValidColumn(board, i) && 
                IsValidBox(board, i))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool IsValidRow(int[][] board, int index)
    {
        HashSet<int> cells = new(10);
        for (int i = 0; i < 9; i++)
        {
            int value = board[index][i];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsValidColumn(int[][] board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index;
        for (int i = 0; i < 9; i++)
        {
            int value = board[i][offset];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidBox(int[][] board, int index)
    {
        HashSet<int> cells = new(10);
        foreach (var (x, y) in BoxCells[index])
        {
            int value = board[x][y];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static CellInfo GetCellInfo(Cell index) => new(
        index.X,
        index.Y,
        GetBoxForCell(index)
    );

    private static int GetBoxForCell(Cell index) => index.X / 3 * 3 + index.Y / 3;

    private static IEnumerable<Cell> GetBoxCells(int index)
    {
        int x = index / 3 * 3;
        int y = index % 3 * 3;
        for (int i = 0; i < 3; i++)
        {
            yield return new(x, y);
            yield return new(x, y + 1);
            yield return new(x, y + 2);
            x++;
        }
    }

    private static Dictionary<int, List<Cell>> GetBoxCellsForBoard()
    {
        Dictionary<int, List<Cell>> cells = [];
        
        for (int i = 0; i < 9; i++)
        {
            cells.Add(i, [.. GetBoxCells(i)]);
        }

        return cells;
    }

    private static bool MoveIndexNext(Cell cell, out Cell nextCell)
    {
        var (x, y) = cell;
        if (y < 8)
        {
            nextCell = new(x, y + 1);
            return true;
        }
        else if (x < 8)
        {
            nextCell = new(x + 1, 0);
            return true;
        }

        nextCell = cell;
        return false;
    }

    private static Dictionary<int, List<Cell>> BoxCells { get; } = GetBoxCellsForBoard();
    private static Dictionary<Cell, CellInfo> CellInfoCells { get; } = [];
}

record struct CellInfo(int Row, int Column, int Box);

record struct Cell(int X, int Y);
