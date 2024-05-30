using System.ComponentModel;

namespace Sudoku;

public class Puzzle(int[] board)
{
    public Cell[] Cells { get; } = GetCells();

    public int[] Board { get; } = board;

    public static ReadOnlySpan<int> GetColumnIndices(int index) => PuzzleData.IndicesByColumn.AsSpan().Slice(index * 9, 9);

    public static ReadOnlySpan<int> GetBoxIndices(int index) => PuzzleData.IndicesByBox.AsSpan().Slice(index * 9, 9);

    public ReadOnlySpan<int> GetRowValues(int index) => Board.AsSpan().Slice(index * 9, 9);

    public ReadOnlySpan<int> GetColumnValues(int index, Span<int> buffer) => GetCellValues(GetColumnIndices(index), buffer);

    public ReadOnlySpan<int> GetBoxValues(int index, Span<int> buffer) => GetCellValues(GetBoxIndices(index), buffer);

    public ReadOnlySpan<int> GetCellValues(ReadOnlySpan<int> line, Span<int> buffer)
    {
        if (buffer.Length < line.Length)
        {
            throw new ArgumentException($"{nameof(buffer)} is too short.");
        }

        for (int i = 0; i < line.Length; i++)
        {
            buffer[i] = Board[line[i]];
        }

        return buffer;
    }

    public List<int> GetCandidates(Cell cell)
    {
        var rowValues = GetRowValues(cell.Row);
        var columnValues = GetColumnValues(cell.Column, stackalloc int[9]);
        var boxValues = GetBoxValues(cell.Box, stackalloc int[9]);
        List<int> cellCandidates = [ 1, 2 , 3 , 4 , 5 , 6 , 7, 8, 9 ];

        RemoveCandidates(cellCandidates, rowValues);
        RemoveCandidates(cellCandidates, columnValues);
        RemoveCandidates(cellCandidates, boxValues);

        static void RemoveCandidates(List<int> candidates, ReadOnlySpan<int> values)
        {
            if (candidates.Count is 0)
            {
                return;
            }

            foreach (int value in values)
            {
                if (value is 0)
                {
                    continue;
                }

                candidates.Remove(value);
            }
        }

        return cellCandidates;
    }

    public int GetValuesInView(Cell cell)
    {
        var rowValues = GetRowValues(cell.Row);
        var columnValues = GetColumnValues(cell.Column, stackalloc int[9]);
        var boxValues = GetBoxValues(cell.Box, stackalloc int[9]);
        int candidates = 0;

        AddValues(ref candidates, rowValues);
        AddValues(ref candidates, columnValues);
        AddValues(ref candidates, boxValues);

        static void AddValues(ref int candidates, ReadOnlySpan<int> values)
        {
            foreach (int value in values)
            {
                if (value is 0)
                {
                    continue;
                }

                candidates |= PuzzleData.Masks[value];
            }
        }

        return candidates;
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

    public static Cell GetCellForIndex(int index) => new(
    index,                      // index
    index / 9,                  // row
    index % 9,                  // column
    PuzzleData.BoxByIndices[index],        // box
    PuzzleData.BoxRowByIndices[index],     // box row
    PuzzleData.BoxColumnByIndices[index],  // box column
    PuzzleData.BoxIndices[index]);         // box index
}
