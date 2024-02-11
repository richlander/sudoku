using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

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
/*
    The box, rows and columns can be used to remove candidates.

    Info we need for a given cell (within a box):

    - The 6 cells in the same row in the neighbor horizontal boxes?
    - The 6 cells in the same column in the neighbor vertical boxes
    - Given a horizontal or vertical line, union the other box and appropriate
      6 group cells to remove candidates for cell.
*/
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        int index = cell.Index;
        IReadOnlyList<int> candidates = puzzle.GetCandidates(index);

        // Get box
        Box box = puzzle.GetBox(cell.Box);
        // Get adjacent neighboring boxes
        Box ahnb1 = puzzle.GetBox(box.FirstHorizontalNeighbor);
        Box ahnb2 = puzzle.GetBox(box.SecondHorizontalNeighbor);
        Box avnb1 = puzzle.GetBox(box.FirstVerticalNeighbor);
        Box avnb2 = puzzle.GetBox(box.SecondVerticalNeighbor);

        // Get row and column indices
        int rowOne = Puzzle.BoxRowByIndices[index];
        int columnOne = Puzzle.BoxColumnByIndices[index];

        // The two rows and two columns each need to be considered together
        List<IEnumerable<int>> lines = [
            ahnb1.GetRowIndices(rowOne).Concat(ahnb2.GetRowIndices(rowOne)),
            avnb1.GetColumnIndices(columnOne).Concat(avnb2.GetColumnIndices(columnOne)),
            ];

        HashSet<int> matches = [];

        foreach (IEnumerable<int> line in lines)
        {
            foreach (int lineIndex in line)
            {
                if (puzzle.IsCellSolved(lineIndex))
                {
                    continue;
                }

                FindCandidatesToRemove(candidates, puzzle.GetCandidates(lineIndex), matches);

                // Solution found
                if (candidates.Count - matches.Count is 1)
                {
                    int value = candidates.Except(matches).Single();
                    solution = new(cell, value, matches, nameof(NakedSinglesSolver));
                    return true;
                }

            }
        }

        solution = null;
        return false;
    }

    // Identify candidates that are cancelled out by their existence in neighboring areas
    private void FindCandidatesToRemove(IReadOnlyList<int> candidates, IEnumerable<int> values, HashSet<int> removalCandidates)
    {
        foreach (int value in values)
        {
            if (candidates.Contains(value))
            {
                removalCandidates.Add(value);
            }
        }
    }
}