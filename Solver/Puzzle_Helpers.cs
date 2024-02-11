namespace Sudoku;
public partial class Puzzle
{

    // Board navigation
    public static IEnumerable<int> GetRowIndices(int index) => IndicesByRow.Skip(index * 9).Take(9);

    public static IEnumerable<int> GetColumnIndices(int index) => IndicesByColumn.Skip(index * 9).Take(9);

    public static IEnumerable<int> GetBoxIndices(int index) => IndicesByBox.Skip(index * 9).Take(9);

    public static Cell GetCellForIndex(int index) => new(
        index,
        index / 9,
        index % 9,
        BoxByIndices[index],
        BoxRowByIndices[index],
        BoxColumnByIndices[index]
    );

    // // Get cell from unit
    // public static Cell GetCellForRowIndex(int row, int index, int value)
    // {
    //     int puzzleIndex = row * 9 + index;
    //     int box = BoxByIndices[puzzleIndex];
    //     Cell cell = new(puzzleIndex, row, index, box);
    //     return cell;
    // }

    // public static Cell GetCellForColumnIndex(int column, int index, int value)
    // {
    //     int puzzleIndex = index * 9 + column;
    //     int box = BoxByIndices[puzzleIndex];
    //     Cell cell = new(puzzleIndex, column, index, box);
    //     return cell;
    // }

    // public static Cell GetCellForBoxIndex(int box, int index, int value)
    // {
    //     int puzzleIndex = IndicesByBox[box * 9 + index];
    //     int row = puzzleIndex / 9;
    //     int column = puzzleIndex % 9;
    //     Cell cell = new(puzzleIndex, column, index, box);
    //     return cell;
    // }       
}