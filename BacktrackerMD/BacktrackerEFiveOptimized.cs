using System.Diagnostics.CodeAnalysis;
using PuzzleMultiDimensionalArray;
using Sudoku;

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
    public static bool Solve(int[,] board, [NotNullWhen(true)] out int[,]? solution)
    {
        solution = board;
        if (!IsValid(solution))
        {
            solution = null;
            return false;
        }

        Puzzle puzzle = new(solution);
        return Solver(puzzle, solution, new(0, 0, 0), out solution) && IsValid(solution!, true);
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

    public static bool MoveIndexNext(Cell cell, out Cell nextCell)
    {
        var (x, y, z) = cell;

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
        else
        {
            nextCell = cell;
            return false;
        }

        nextCell = new(x, y, z);
        return true;
    }

    public static int[,] GetSolution(int value)
    {
        var solution = new int[9, 9];
        solution[8, 8] = value;
        return solution;
    }

    private static bool IsValid(int[,] board, bool testForEmpties = false)
    {
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

    private static bool IsValidRow(int[,] board, int index)
    {
        HashSet<int> cells = new(10);
        for (int i = 0; i < 9; i++)
        {
            int value = board[index, i];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsValidColumn(int[,] board, int index)
    {
        HashSet<int> cells = new(10);
        int offset = index;
        for (int i = 0; i < 9; i++)
        {
            int value = board[i, offset];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsValidBox(int[,] board, int index)
    {
        HashSet<int> cells = new(10);
        foreach (Cell cell in BoxCells[index])
        {
            var (x, y, _) = cell;
            int value = board[x, y];
            if (!(value is 0 || cells.Add(value)))
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<Cell> GetBoxCells(int index)
    {
        int x = index / 3 * 3;
        int y = index % 3 * 3;
        for (int i = 0; i < 3; i++)
        {
            yield return new(x, y, index);
            yield return new(x, y + 1, index);
            yield return new(x, y + 2, index);
            x++;
        }
    }

    private static Dictionary<int, List<Cell>> BoxCells { get; } = GetBoxCellsForBoard();

    private static Dictionary<int, List<Cell>> GetBoxCellsForBoard()
    {
        Dictionary<int, List<Cell>> cells = [];

        for (int i = 0; i < 9; i++)
        {
            cells.Add(i, [.. GetBoxCells(i)]);
        }

        return cells;
    }


    private static bool IsLastCell(Cell cell) => cell.Row is 8 && cell.Column is 8;
}
