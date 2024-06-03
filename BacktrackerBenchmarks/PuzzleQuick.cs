using Sudoku;
using Microsoft.Diagnostics.Tracing.StackSources;

namespace PuzzleQuick;

public class Puzzle(ReadOnlySpan<int> board)
{
    public Cell[] Cells { get; } = GetCells();

    public int[] BoardRows { get; } = GetBoardRows(board);

    public int[] BoardColumns { get; } = GetBoardColumns(board);

    public int[] BoardBoxes { get; } = GetBoardBoxes(board);

    public static ReadOnlySpan<int> GetBoxIndices(int index) => PuzzleData.IndicesByBox.AsSpan().Slice(index * 9, 9);

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

    private static int[] GetBoardRows(ReadOnlySpan<int> board)
    {
        int[] rows = new int[9];

        for (int i = 0; i < 9; i++)
        {
            int row = 0;
            foreach (int value in board.Slice(i * 9, 9))
            {
                if (value is 0)
                {
                    continue;
                }

                row |= 1 << value;
            }

            rows[i] = row;
        }

        return rows;
    }

    private static int[] GetBoardColumns(ReadOnlySpan<int> board)
    {
        int[] columns = new int[9];
        for (int i = 0; i < 9; i++)
        {
            int column = 0;
            for (int j = 0; j < 9; j++)
            {
                int index = j * 9 + i;
                int value = board[index];
                if (value is 0)
                {
                    continue;
                }

                column |= 1 << value;
            }

            columns[i] = column;
        }

        return columns;
    }

   private static int[] GetBoardBoxes(ReadOnlySpan<int> board)
    {
        int[] boxes = new int[9];
        ReadOnlySpan<int> boxIndices = PuzzleData.IndicesByBox;
        for (int i = 0; i < 9; i++)
        {
            int box = 0;
            foreach (int index in boxIndices.Slice(i * 9, 9))
            {
                int value = board[index];
                if (value is 0)
                {
                    continue;
                }

                box |= 1 << value;
            }

            boxes[i] = box;
        }

        return boxes;
    }

    private static Cell GetCellForIndex(int index) => new(
    index,                      // index
    index / 9,                  // row
    index % 9,                  // column
    PuzzleData.BoxByIndices[index],        // box
    PuzzleData.BoxRowByIndices[index],     // box row
    PuzzleData.BoxColumnByIndices[index],  // box column
    PuzzleData.BoxIndices[index]);         // box index
}
