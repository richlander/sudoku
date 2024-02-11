namespace Sudoku;

public partial class Box
{
    // Cell indices
    public static int GetFirstCellForBox(int index) => index / 3 * 27 + index % 3 * 3;

    // public static IEnumerable<int> GetRowIndices(int boxIndex, int rowIndex)
    // {
    //     int cell = GetFirstCellForBox(boxIndex) + rowIndex * 9;
    //     yield return cell++;
    //     yield return cell++;
    //     yield return cell;
    // }

    // public static IEnumerable<int> GetColumn(int boxIndex, int columnIndex)
    // {
    //     int cell = GetFirstCellForBox(boxIndex) + columnIndex;
    //     yield return cell;
    //     yield return cell + 9;
    //     yield return cell + 18;
    // }

    // Neighbors
    public static int GetHorizontalNeighbor(int index, int distance) => (index / 3) * 3 + (index + distance) % 3;

    public static int GetVerticalNeighbor(int index, int distance) => (index + distance) % 9;
}