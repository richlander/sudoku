using Sudoku;
using Microsoft.Diagnostics.Tracing.StackSources;

namespace PuzzleMultiDimensionalArray;

public class Puzzle(int[,] board)
{
    public Cell[] Cells { get; } = GetCells();

    public int[] BoardRows { get; } = GetInitialValues(board, PuzzleDataMD.PuzzleRow);

    public int[] BoardColumns { get; } = GetInitialValues(board, PuzzleDataMD.PuzzleColumn);

    public int[] BoardBoxes { get; } = GetInitialValues(board, PuzzleDataMD.PuzzleBox);

    public int GetValuesInView(Cell cell) => 
        BoardRows[cell.Row] |
        BoardColumns[cell.Column] |
        BoardBoxes[cell.Box];

    public void UpdateCell(Cell cell, int oldValue, int value)
    {
        if (oldValue > 0)
        {
            ClearValue(ref BoardRows[cell.Row], oldValue);
            ClearValue(ref BoardColumns[cell.Column], oldValue);
            ClearValue(ref BoardBoxes[cell.Box], oldValue);
        }

        if (value > 0)
        {
            WriteValue(ref BoardRows[cell.Row], value);
            WriteValue(ref BoardColumns[cell.Column], value);
            WriteValue(ref BoardBoxes[cell.Box], value);
        }
    }

    public static void WriteValue(ref int line, int value) => line |= 1 << value;

    public static void ClearValue(ref int line, int value) => line ^= 1 << value;

    private static int[] GetInitialRowValues(int[,] board)
    {
        int[] values = new int[9];

        for (int i = 0; i < 9; i++)
        {
            int value = 0;

            for (int j = 0; j < 9; j++)
            {
                int boardValue = board[i, j];
                if (boardValue is 0)
                {
                    continue;
                }

                value |= 1 << boardValue;
            }

            values[i] = value;
        }

        return values;
    }

    private static int[] GetInitialColumnValues(int[,] board)
    {
        int[] values = new int[9];

        for (int i = 0; i < 9; i++)
        {
            int value = 0;

            for (int j = 0; j < 9; j++)
            {
                int boardValue = board[j, i];
                if (boardValue is 0)
                {
                    continue;
                }

                value |= 1 << boardValue;
            }

            values[i] = value;
        }

        return values;
    }

    private static int[] GetInitialBoxValues(int[,] board)
    {
        int[] values = new int[9];

        for (int i = 0; i < 9; i++)
        {
            int value = 0;
            int firstBoxRow = i / 3;
            int firstBoxColumn = i % 3 * 3;

            for (int j = 0; j < 3; j++)
            {
                int row = firstBoxRow;
                value |= 1 << board[row++, firstBoxColumn + j];
                value |= 1 << board[row++, firstBoxColumn + j];
                value |= 1 << board[row++, firstBoxColumn + j];
            }

            values[i] = value;
        }

        return values;
    }

    private static int[] GetInitialValues(int[,] board, ReadOnlySpan<Point> indices)
    {
        int[] values = new int[9];
        for (int i = 0; i < 9; i++)
        {
            int value = 0;
            foreach (Point point in indices.Slice(i * 16, 9))
            {
                int boardValue = board[point.X, point.Y];
                if (boardValue is 0)
                {
                    continue;
                }

                value |= 1 << boardValue;
            }

            values[i] = value;
        }

        return values;
    }

    private static Cell[] GetCells()
    {
        Cell[] cells = new Cell[81];
        for (int i = 0; i < 81; i++)
        {
            cells[i] = GetCellForIndex(i);
        }

        return cells;
    }

    private static Cell GetCellForIndex(int index) => new(
        PuzzleDataMD.PuzzleRow[index],    // index
        PuzzleData16.PuzzleRow[index],    // row
        PuzzleData16.PuzzleColumn[index], // column
        PuzzleData.BoxByIndices[index]);  // box
}
