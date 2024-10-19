using System.Runtime.Versioning;

namespace FasterSudoku;

public ref struct Puzzle (int[] board)
{
    private readonly Span<int> _board = board;
    private readonly Span<Cell> _cells = GetCells();
    private readonly Span<int> 





    private static Cell[] GetCells()
    {
        Cell[] cells = new Cell[81];
        for (int i = 0; i < 81; i++)
        {
            cells[i] = GetCellForIndex(i);
        }

        return cells;
    }

    private static Cell GetCellForIndex(int index) => new(
        index,
        index / 9,
        index % 9,
        PuzzleData.BoxByIndices[index],
        PuzzleData.BoxRowByIndices[index],
        PuzzleData.BoxColumnByIndices[index],
        PuzzleData.BoxIndices[index]);

}