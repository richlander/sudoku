using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace Sudoku;

public class PointingPairsSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        Box box = puzzle.GetBox(cell.Box);
        IEnumerable<int> boxLine = Puzzle.GetBoxIndices(cell.Box);

        // We need to determine if there is a candidate in this row that is unique to the box column
        // Only necessary for first cell in each row; answer will repeat
        if (puzzle.IsIndexFirstUnsolved(box.GetRowIndices(cell.BoxRow), cell))
        {
            IEnumerable<int> boxRow = box.GetRowIndices(cell.BoxRow);
            if (puzzle.TryFindUniqueCandidates(boxRow, boxLine, Puzzle.GetRowIndices(cell.Row), nameof(PointingPairsSolver), out Solution? s))
            {
                solution = s;
            }
        }

        // We need to determine if there is a candidate in this column that is unique to the box column
        // Only necessary for first/top cell in each column; answer will repeat
        if (puzzle.IsIndexFirstUnsolved(box.GetColumnIndices(cell.BoxColumn), cell))
        {
            IEnumerable<int> boxColumn = box.GetColumnIndices(cell.BoxColumn);
            if (puzzle.TryFindUniqueCandidates(boxColumn, boxLine, Puzzle.GetColumnIndices(cell.Column), nameof(PointingPairsSolver), out Solution? s))
            {
                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
            }
        }

        if (solution is null)
        {
            return false;
        }

        return true;
    }
}
