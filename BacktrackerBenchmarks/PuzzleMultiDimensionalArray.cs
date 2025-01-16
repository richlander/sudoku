using Sudoku;

namespace PuzzleMultiDimensionalArray;

public class Puzzle
{
    public Puzzle(int[,] board)
    {
        (BoardRows, BoardColumns, BoardBoxes) = GetInitialValues(board);
    }

    public int[] BoardRows { get; }

    public int[] BoardColumns { get; }

    public int[] BoardBoxes { get; }

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

    private static (int[] rowValue, int[] columnValue, int[] boxValue) GetInitialValues(int[,] board)
    {
        Cell[] cells = [];
        int[] rowValues = new int[9];
        int[] columnValues = new int[9];
        int[] boxValues = new int[9];
        for (int i = 0; i < 81; i++)
        {
            var (row, column, box) = PuzzleDataMD.Cells[i];
            int value = board[row, column];

            if (value is 0)
            {
                continue;
            }

            int shiftedValue = 1 << value;
            rowValues[row] |= shiftedValue;
            columnValues[column] |= shiftedValue;
            boxValues[box] |= shiftedValue;
        }

        return (rowValues, columnValues, boxValues);
    }
}

public record Cell2(int Row, int Column, int Box);
