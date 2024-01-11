using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

// Naked singles
// One absent value in a unit (row, column, box) of 9
// Solves a row like this: 023456789
// Expected solution is: 1

// Example:
// Solved cell: r4:c3; 2
// Solved by: NakedSinglesSolver
//     *
// 3 0 5 | 4 2 0 | 8 1 0
// 4 8 7 | 9 0 1 | 5 0 6
// 0 2 9 | 0 5 6 | 3 7 4
// ------+-------+------
// 8 5 2 | 7 9 3 | 0 4 1*
// 6 1 3 | 2 0 8 | 9 5 7
// 0 7 4 | 0 6 5 | 2 8 0
// ------+-------+------
// 2 4 1 | 3 0 9 | 0 6 5
// 5 0 8 | 6 7 0 | 1 9 2
// 0 9 6 | 5 1 2 | 4 0 8

public class NakedSinglesSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, [NotNullWhen(true)] out Solution? solution)
    {
        // iterate over boxes
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
        Box box = puzzle.GetBox(index);
        // iterate over cells
        for (int i = 0; i < 9; i++)
        {
            if (puzzle.IsCellSolved(box.CellsForCells[i]))
            {
                continue;
            }

            var puzzleCell = Box.GetIndexForBoxCell(index, i);
            var candidates = puzzle.Candidates[puzzleCell];

            if (RemoveCandidatesForCell(puzzle, index, i, candidates))
            {
                var candidate = puzzle.Candidates[puzzleCell].Single();
                solution = Box.GetSolutionForBox(index, i, candidate, nameof(NakedSinglesSolver));
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
    - Which are the 6 cells in the same column in the neighbor vertical boxes
    - Given a horizontal or vertical "lool", union the other box and appropriate
      6 group cells to remove candidates for celll.
*/

    // Assumptions
    // cell is not 0
    // candidate list is > 1
    private bool RemoveCandidatesForCell(Puzzle puzzle, int box, int cell, List<int> candidates)
    {
        // Find groupings
        var row = puzzle.GetCellsForNeighboringRow(box, cell);
        var column = puzzle.GetCellsForNeighboringColumn(box, cell);
        var boxcells = puzzle.GetCellsForBox(box);
        List<IEnumerable<int>> groups = [row, column, boxcells];
        
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

    // Removes candidates that are cancelled out by their existence in neighboring areas
    private bool RemoveCandidates(List<int> candidates, IEnumerable<int> values)
    {
        bool removed = false;

        foreach (int value in values)
        {
            removed |= candidates.Remove(value);
        }

        return removed;
    }
}