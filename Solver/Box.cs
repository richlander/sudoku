using System.Diagnostics.Contracts;

namespace Sudoku;

public partial class Box(int index)
{
    // Navigation values
    public int FirstCell { get; } = Puzzle.BoxFirstCellIndices[index];

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
}
