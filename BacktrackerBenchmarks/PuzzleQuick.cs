using Sudoku;
using Microsoft.Diagnostics.Tracing.StackSources;

namespace BacktrackerFour;

public class Puzzle(ReadOnlySpan<int> board)
{
    public Cell[] Cells { get; } = GetCells();

    public int[] BoardRows { get; } = GetBoardRows(board);

    public int[] BoardColumns { get; } = GetBoardColumns(board);

    public int[] BoardBoxes { get; } = GetBoardBoxes(board);

    public static ReadOnlySpan<int> GetRowIndices(int index) => PuzzleData.IndicesByRow.AsSpan().Slice(index * 9, 9);

    public static ReadOnlySpan<int> GetColumnIndices(int index) => PuzzleData.IndicesByColumn.AsSpan().Slice(index * 9, 9);

    public static ReadOnlySpan<int> GetBoxIndices(int index) => PuzzleData.IndicesByBox.AsSpan().Slice(index * 9, 9);

    public void UpdateCell(Cell cell, int oldValue, int value)
    {
        if (oldValue > 0)
        {
            UnWriteRowValue(cell.Row, oldValue);
            UnWriteColumnValue(cell.Column, oldValue);
            UnWriteBoxValue(cell.Box, oldValue);
        }

        if (value > 0)
        {
            WriteRowValue(cell.Row, value);
            WriteColumnValue(cell.Column, value);
            WriteBoxValue(cell.Box, value);
        }
    }

    public int ReadRow(int index) => BoardRows[index];

    public void WriteRowValue(int index, int value)
    {
        int row = BoardRows[index];
        row |= PuzzleData.Masks[value];
        BoardRows[index] = row;
    }

    public void UnWriteRowValue(int index, int value)
    {
        int row = BoardRows[index];
        row ^= PuzzleData.Masks[value];
        BoardRows[index] = row;
    }

    public int ReadColumn(int index) => BoardColumns[index];

    public void WriteColumnValue(int index, int value)
    {
        int column = BoardColumns[index];
        column |= PuzzleData.Masks[value];
        BoardColumns[index] = column;
    }

    public void UnWriteColumnValue(int index, int value)
    {
        int column = BoardColumns[index];
        column ^= PuzzleData.Masks[value];
        BoardColumns[index] = column;
    }

    public int ReadBoxes(int index) => BoardBoxes[index];

    public void WriteBoxValue(int index, int value)
    {
        int box = BoardBoxes[index];
        box |= PuzzleData.Masks[value];
        BoardBoxes[index] = box;
    }

    public void UnWriteBoxValue(int index, int value)
    {
        int box = BoardBoxes[index];
        box ^= PuzzleData.Masks[value];
        BoardBoxes[index] = box;
    }

    public int GetRowValues(int index) => BoardRows[index];

    public int GetColumnValues(int index) => BoardColumns[index];

    public int GetBoxValues(int index) => BoardBoxes[index];

    private static IEnumerable<int> GetValues(int values)
    {
        for (int i = 1; i < 10; i++)
        {
            var result = values & PuzzleData.Masks[i];
            if (result > 0)
            {
                yield return i;
            }
        }
    }

    public int GetCandidates(Cell cell)
    {
        var rowValues = GetRowValues(cell.Row);
        var columnValues = GetColumnValues(cell.Column);
        var boxValues = GetBoxValues(cell.Box);
        int inUse = rowValues | columnValues | boxValues;
        return inUse;

        // int candidates = 0b1_11111111;

        // RemoveCandidates(ref candidates, rowValues);
        // RemoveCandidates(ref candidates, columnValues);
        // RemoveCandidates(ref candidates, boxValues);

        // static void RemoveCandidates(ref int candidates, int values)
        // {
        //     foreach (int value in GetValues(values))
        //     {
        //         var result = candidates & PuzzleData.Masks[value];
        //         if (result > 0)
        //         {
        //             candidates ^= PuzzleData.Masks[value];
        //         }
        //     }
        // }

        // return candidates;
    }

    public static Cell[] GetCells()
    {
        Cell[] cells = new Cell[81];
        for (int i = 0; i < 81; i++)
        {
            cells[i] = GetCellForIndex(i);
        }

        return cells;
    }

    public static int[] GetBoardRows(ReadOnlySpan<int> board)
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

                row |= PuzzleData.Masks[value];
            }

            rows[i] = row;
        }

        return rows;
    }

    public static int[] GetBoardColumns(ReadOnlySpan<int> board)
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

                column |= PuzzleData.Masks[value];
            }

            columns[i] = column;
        }

        return columns;
    }

   public static int[] GetBoardBoxes(ReadOnlySpan<int> board)
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

                box |= PuzzleData.Masks[value];
            }

            boxes[i] = box;
        }

        return boxes;
    }

    public static Cell GetCellForIndex(int index) => new(
    index,                      // index
    index / 9,                  // row
    index % 9,                  // column
    PuzzleData.BoxByIndices[index],        // box
    PuzzleData.BoxRowByIndices[index],     // box row
    PuzzleData.BoxColumnByIndices[index],  // box column
    PuzzleData.BoxIndices[index]);         // box index
}
