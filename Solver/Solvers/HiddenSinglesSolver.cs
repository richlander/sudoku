using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

public class HiddenSinglesSolver : ISolver
{
    // Hidden singles: One value logically absent candidate value in a box, row or column
    // Determine which candidates are unique in the given cell per each unit (box, column, row)
    // If there is just one candidate, then that's the solution
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];
        
        foreach(IEnumerable<int> line in lines)
        {
            HashSet<int> cellCandidates = new(puzzle.GetCellCandidates(cell));

            foreach (int neighborIndex in line.Where(x => x != cell))
            {
                IReadOnlyList<int> neighborCandidates = puzzle.GetCellCandidates(neighborIndex);
                cellCandidates.RemoveRange(neighborCandidates);
            }

            // Count may be 0, 1, or another value
            // A solution is present with a single value
            if (cellCandidates.Count is 1)
            {
                int value = cellCandidates.Single();
                solution = new(cell, value, nameof(HiddenSinglesSolver));
                return true;
            }
        }

        return false;
    }
}
