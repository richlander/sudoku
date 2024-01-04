using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

// Sole candidate (AKA "naked single")
public class SoleCandidateSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
    {
        for (int i = 0; i < 9; i++)
        {
            if (TrySolveBox(i, puzzle, out solution))
            {
                return true;
            }
        }
        
        solution = default;
        return false;
    }

    private bool TrySolveBox(int index, Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
    {
        Box box0 = puzzle.GetBox(index);
        for (int i = 0; i < 9; i++)
        {
            if (puzzle.IsCellSolved(box0.CellsForCells[i]))
            {
                continue;
            }

            if (RemoveCandidatesForCell(puzzle, index, i))
            {
                var puzzleCell = Box.GetIndexForBoxCell(index, i);
                var candidate = puzzle.Candidates[puzzleCell].Single();
                solution = Box.GetSolutionForBox(index, i, candidate, nameof(SoleCandidateSolver));
                return true;
            }
        }

        solution = default;
        return false;
    }

/*
    Box is the best (and only required) grouping for this solver.
    With a box, the box, rows and columns can be used to remove candidates.

    Info we need for a given cell (within a box):

    - Is this cell unsolved?
    - Which are the 6 cells in the same row in the neighbor horizontal boxes?
    - Which are the 6 cells in the same column in the neighbor horizontal boxes
    - Given a horizontal or vertical "lool", union the other box and appropriate
      6 group cells to remove candidates for celll.
*/

    // Assumptions
    // cell is not 0
    // candidate list is > 1
    private bool RemoveCandidatesForCell(Puzzle puzzle, int box, int cell)
    {
        var row = puzzle.GetCellsForNeighboringRow(box, cell);
        var column = puzzle.GetCellsForNeighboringColumn(box, cell);
        var boxcells = puzzle.GetCellsForBox(box);
        List<IEnumerable<int>> groups = [row, column, boxcells];
        var puzzleCell = Box.GetIndexForBoxCell(box, cell);
        var candidates = puzzle.Candidates[puzzleCell];
        
        foreach(var group in groups)
        {
            if (RemoveCandidates(candidates, group))
            {
                if (candidates.Count is 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool RemoveCandidates(List<int> candidates, IEnumerable<int> values)
    {
        bool removed = false;

        foreach (int value in values)
        {
            if (candidates.Contains(value))
            {
                candidates.Remove(value);
                removed = true;
            }
        }

        return removed;
    }
}