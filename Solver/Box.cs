using System.Diagnostics.Contracts;

namespace Sudoku;

public partial class Box(int index)
{
    // Navigation values
    public int Index { get; } = index;
    public int FirstCell { get; } = Puzzle.BoxFirstCellIndices[index];

    // public int FirstColumn { get; } = Puzzle.BoxColumnByIndices[Puzzle.BoxFirstCellIndices[index]];

    // public int FirstRow { get; } = (index / 3) % 3;

    public int FirstHorizontalNeighbor { get; } = GetHorizontalNeighbor(index, 1);
    
    public int SecondHorizontalNeighbor { get; } = GetHorizontalNeighbor(index, 2);
    
    public int FirstVerticalNeighbor { get; } = GetVerticalNeighbor(index, 3);
    
    public int SecondVerticalNeighbor { get; } = GetVerticalNeighbor(index, 6);

    // Cell indices
    public IEnumerable<int> GetRowIndices(int index)
    {
        int cell = FirstCell + index * 9;
        yield return cell++;
        yield return cell++;
        yield return cell;
    }

    public IEnumerable<int> GetColumnIndices(int index)
    {
        int cell = FirstCell + index;
        yield return cell;
        yield return cell + 9;
        yield return cell + 18;
    }

    // public static int GetPuzzleColumnForBoxCell(int box, int cell) => ((box % 3) * 3) + (cell % 3);

    // public static int GetPuzzleRowForBoxCell(int box, int cell) => (box / 3) * 3 + (cell / 3);

    // public static int GetPuzzleIndexForBoxCell(int box, int cell) => GetPuzzleColumnForBoxCell(box, cell) + (GetPuzzleRowForBoxCell(box, cell) * 9);

    // public static int GetPuzzleIndexForBoxCell2(int box, int cell) => (box / 3 * 27) + (box % 3 * 3) + (cell / 3 * 9) + cell % 3;

    // public static int GetBoxForCell(int cell) => cell / 27 * 3 + cell % 9 / 3;

    // public static IEnumerable<int> GetRowValuesForBoxCells(int index)
    // {
    //     for (int i = 0; i < 9; i++)
    //     {
    //         yield return GetPuzzleRowForBoxCell(index, i);
    //     }
    // }

    // public static IEnumerable<int> GetColumnValuesForBoxCells(int index)
    // {
    //     for (int i = 0; i < 9; i++)
    //     {
    //         yield return GetPuzzleColumnForBoxCell(index,i);
    //     }
    // }

    // public static IEnumerable<int> GetCellsForCells(int box)
    // {
    //     for (int i = 0; i < 9; i++)
    //     {
    //         yield return GetPuzzleIndexForBoxCell(box, i);
    //     }
    // }
}
