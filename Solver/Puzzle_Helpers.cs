namespace Sudoku;
public partial class Puzzle
{

    // Board navigation
    public static IEnumerable<int> IndicesForRow(int index) => IndicesByRow.Skip(index * 9).Take(9);

    public static IEnumerable<int> IndicesForColumn(int index) => IndicesByColumn.Skip(index * 9).Take(9);

    public static IEnumerable<int> IndicesForBox(int index) => IndicesByBox.Skip(index * 9).Take(9);

    // Get cell from unit
    public static Cell CellForRowIndex(int row, int index, int value)
    {
        int puzzleIndex = row * 9 + index;
        int box = BoxByIndices[puzzleIndex];
        Cell cell = new(puzzleIndex, row, index, box);
        return cell;
    }

    public static Cell CellForColumnIndex(int column, int index, int value)
    {
        int puzzleIndex = index * 9 + column;
        int box = BoxByIndices[puzzleIndex];
        Cell cell = new(puzzleIndex, column, index, box);
        return cell;
    }

    public static Cell CellForBoxIndex(int box, int index, int value)
    {
        int puzzleIndex = IndicesByBox[box * 9 + index];
        int row = RowByIndices[puzzleIndex];
        int column = ColumnByIndices[puzzleIndex];
        Cell cell = new(puzzleIndex, column, index, box);
        return cell;
    }       
}