using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading.Channels;

namespace Sudoku;

public class PointingPairsSolver : ISolver
{
    public string Name => nameof(PointingPairsSolver);

    /*
        Pointing pairs: The same candidate shows up in one line or column within a box and not elswhere.
        This is another "hidden" pattern.
        The candidate can be removed from column or row cells outside the box.

        
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        Box box = puzzle.GetBox(cell.Box);
        List<int> boxLine = Puzzle.GetBoxIndices(cell.Box).ToList();

        // We need to determine if there is a candidate in this row that is unique to the box column
        // Only necessary for first cell in each row; answer will repeat
        if (puzzle.IsIndexFirstUnsolved(box.GetRowIndices(cell.BoxRow), cell))
        {
            IEnumerable<int> boxRow = box.GetRowIndices(cell.BoxRow);
            if (puzzle.TryFindSolutionWithIntersectionRemoval(boxRow, boxLine, Puzzle.GetRowIndices(cell.Row), this, out Solution? s))
            {
                solution = s;
            }
        }

        // We need to determine if there is a candidate in this column that is unique to the box column
        // Only necessary for first/top cell in each column; answer will repeat
        if (puzzle.IsIndexFirstUnsolved(box.GetColumnIndices(cell.BoxColumn), cell))
        {
            IEnumerable<int> boxColumn = box.GetColumnIndices(cell.BoxColumn);
            if (puzzle.TryFindSolutionWithIntersectionRemoval(boxColumn, boxLine, Puzzle.GetColumnIndices(cell.Column), this, out Solution? s))
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
