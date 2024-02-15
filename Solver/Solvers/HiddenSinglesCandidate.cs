using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

// Hidden singles: One logically absent value in a row or column, based on 
// values being present in an adjacent row or column.
// Example:
// Solved cell: r3:c1; 8
// Solved by: HiddenSinglesSolver:RowSolver
//  *
//  0 0 2 | 0 3 0 | 0 0 8
//  0 0 0 | 0 0 8 | 0 0 0
//  8 3 1 | 0 2 0 | 0 0 0*

public class HiddenSinglesSolver : ISolver
{
    // Determine which candidates are unique in the given cell per each unit (box, column, row)
    // If there are unique candidates, remove the ones that are not unique
    // If there is just one candidate, then that's the solution
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];
        
        foreach(IEnumerable<int> line in lines)
        {
            HashSet<int> candidates = new(puzzle.GetCellCandidates(cell));
            // Candidate unique in column
            if (TrySolveCandidateUniqueInLine(puzzle, candidates, line, cell))
            {
                int value = candidates.Single();
                solution = new(cell, value, candidates, nameof(HiddenSinglesSolver));
                return true;
            }
        }

        solution = null;
        return false;
    }

    // Remove all lineCandidates
    // Remaining candidates must be unique
    private static bool TrySolveCandidateUniqueInLine(Puzzle puzzle, HashSet<int> cellCandidates, IEnumerable<int> line, int cellToSkip)
    {
        foreach (int index in line)
        {
            if (index == cellToSkip)
            {
                continue;
            }

            foreach (int candidate in puzzle.GetCellCandidates(index))
            {
                cellCandidates.Remove(candidate);
            }
        }

        return cellCandidates.Count is 1;
    }
}
