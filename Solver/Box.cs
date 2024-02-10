using System.Diagnostics.Contracts;

namespace Sudoku;

public class Box(int index)
{
    public int Index { get; } = index;
    public int FirstCell { get; } = GetFirstCellForBox(index);

    public int FirstColumn { get; } = GetPuzzleColumnForBoxCell(index, 0);

    public int FirstRow { get; } = (index / 3) % 3;

    public int FirstHorizontalNeighbor { get; } = GetNextHorizontalBox(index, 1);
    
    public int SecondHorizontalNeighbor { get; } = GetNextHorizontalBox(index, 2);
    
    public int FirstVerticalNeighbor { get; } = GetNextVerticalBox(index, 3);
    
    public int SecondVerticalNeighbor { get; } = GetNextVerticalBox(index, 6);

    public int[] RowsForCells { get; } = GetRowValuesForBoxCells(index).ToArray();

    public int[] ColumnsForCells { get; } = GetColumnValuesForBoxCells(index).ToArray();

    public int[] CellsForCells { get; } = GetCellsForCells(index).ToArray();

    // public BoxCell[] BoxCells { get; } = GetBoxCells(puzzle, index).ToArray();

    public IEnumerable<int> GetRow(int index)
    {
        int cell = FirstCell + index * 9;
        yield return cell++;
        yield return cell++;
        yield return cell;
    }

    public IEnumerable<int> GetColumn(int index)
    {
        int cell = FirstCell + index;
        yield return cell;
        yield return cell + 9;
        yield return cell + 18;
    }

    public static int GetRowForCell(int index) => index / 3;
    public static int GetColumnForCell(int index) => index % 3;
    public static int GetBoxIndexForRowColumn(int row, int column) => (row / 3) * 3 + (column % 3);

    public static int GetFirstCellForBox(int index) => (index / 3) * 27 + (index % 3) * 3;

    public static int GetPuzzleColumnForBoxCell(int box, int cell) => ((box % 3) * 3) + (cell % 3);

    public static int GetPuzzleRowForBoxCell(int box, int cell) => (box / 3) * 3 + (cell / 3);

    public static int GetPuzzleIndexForBoxCell(int box, int cell) => GetPuzzleColumnForBoxCell(box, cell) + (GetPuzzleRowForBoxCell(box, cell) * 9);

    public static int GetPuzzleIndexForBoxCell2(int box, int cell) => (box / 3 * 27) + (box % 3 * 3) + (cell / 3 * 9) + cell % 3;

    public static int GetBoxForCell(int cell) => cell / 27 * 3 + cell % 9 / 3;

    public static int GetNextHorizontalBox(int index, int next) => (index / 3) * 3 + (index + next) % 3;

    public static int GetNextVerticalBox(int index, int next) => (index + next) % 9;

    public static IEnumerable<int> GetRowValuesForBoxCells(int index)
    {
        for (int i = 0; i < 9; i++)
        {
            yield return GetPuzzleRowForBoxCell(index, i);
        }
    }

    public static IEnumerable<int> GetColumnValuesForBoxCells(int index)
    {
        for (int i = 0; i < 9; i++)
        {
            yield return GetPuzzleColumnForBoxCell(index,i);
        }
    }

    public static IEnumerable<int> GetCellsForCells(int box)
    {
        for (int i = 0; i < 9; i++)
        {
            yield return GetPuzzleIndexForBoxCell(box, i);
        }
    }

    // public static Solution GetSolutionForBox(int box, int cell, int value, string solver)
    // {
    //     var puzzleCell = GetIndexForBoxCell(box, cell);
    //     var row = CellFoo.GetRowForCell(puzzleCell);
    //     var column = CellFoo.GetColumnForCell(puzzleCell);
    //     return new Solution(row, column, value, [],  solver);
    // }
}
