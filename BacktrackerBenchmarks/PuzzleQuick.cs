using Sudoku;
using Microsoft.Diagnostics.Tracing.StackSources;

namespace PuzzleQuick;

public class Puzzle(ReadOnlySpan<int> board)
{
    public Cell[] Cells { get; } = GetCells();

    public int[] BoardRows { get; } = GetInitialValues(board, PuzzleData.IndicesByRow);

    public int[] BoardColumns { get; } = GetInitialValues(board, PuzzleData.IndicesByColumn);

    public int[] BoardBoxes { get; } = GetInitialValues(board, PuzzleData.IndicesByBox);

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

    private static Cell[] GetCells()
    {
        Cell[] cells = new Cell[81];
        for (int i = 0; i < 81; i++)
        {
            cells[i] = GetCellForIndex(i);
        }

        return cells;
    }

   private static int[] GetInitialValues(ReadOnlySpan<int> board, ReadOnlySpan<int> indices)
    {
        int[] values = new int[9];
        for (int i = 0; i < 9; i++)
        {
            int value = 0;
            foreach (int index in indices.Slice(i * 9, 9))
            {
                int boardValue = board[index];
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

    private static Cell GetCellForIndex(int index) => new(
        index,                      // index
        index / 9,                  // row
        index % 9,                  // column
        PuzzleData.BoxByIndices[index]); // box

}
