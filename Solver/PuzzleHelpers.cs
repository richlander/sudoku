namespace Sudoku;
public partial class Puzzle
{
    public static Cell GetCellForRowIndex(int row, int index, int value)
    {
        int puzzleIndex = row * 9 + index;
        int box = GetBoxForCell(puzzleIndex);
        Cell cell = new(puzzleIndex, row, index, box);
        return cell;
    }

    public static Cell GetCellForColumnIndex(int column, int index, int value)
    {
        int puzzleIndex = index * 9 + column;
        int box = GetBoxForCell(puzzleIndex);
        Cell cell = new(puzzleIndex, column, index, box);
        return cell;
    }

    public static Cell GetCellForBoxIndex(int box, int index, int value)
    {
        int puzzleIndex = Box.GetPuzzleIndexForBoxCell(box, index);
        int row = Box.GetPuzzleRowForBoxCell(box, index);
        int column = Box.GetPuzzleColumnForBoxCell(box, index);
        Cell cell = new(puzzleIndex, column, index, box);
        return cell;
    }

    public static int GetBoxForCell(int cell) => (cell / 27) * 3 + (cell - ((cell / 9) * 9)) % 3;

    // public static int GetCellForRow(int cell) => cell / 9;

    // public static int GetCellForColumn(int cell) => cell / 9;
}