using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

// Almost identical to pointed pairs, with a sort of 90° translated mirror of the algorithm
public class BoxLineReductionSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        Box box = puzzle.GetBox(cell.Box);

        // We need to determine if there is a candidate in this row that is unique to the box column
        // Only necessary for first cell in each row; answer will repeat
        if (puzzle.IsIndexFirstUnsolved(box.GetRowIndices(cell.BoxRow), cell))
        {
            IEnumerable<int> boxLine = Puzzle.GetBoxIndices(cell.Box);
            IEnumerable<int> boxRow = box.GetRowIndices(cell.BoxRow);
            if (puzzle.TryFindUniqueCandidates(boxRow, Puzzle.GetRowIndices(cell.Row), boxLine, nameof(BoxLineReductionSolver), out Solution? s))
            {
                solution = s;
            }
        }

        // We need to determine if there is a candidate in this column that is unique to the box column
        // Only necessary for first/top cell in each column; answer will repeat
        if (puzzle.IsIndexFirstUnsolved(box.GetColumnIndices(cell.BoxColumn), cell))
        {
            IEnumerable<int> boxLine = Puzzle.GetBoxIndices(cell.Box);
            IEnumerable<int> boxColumn = box.GetColumnIndices(cell.BoxColumn);
            if (puzzle.TryFindUniqueCandidates(boxColumn, Puzzle.GetColumnIndices(cell.Column), boxLine, nameof(BoxLineReductionSolver), out Solution? s))
            {
                if (solution is null)
                {
                    solution = s;
                }
                else
                {
                    Puzzle.AttachToLastSolution(solution, s);
                }
            }
        }

        if (solution is null)
        {
            return false;
        }

        return true;
    }
}