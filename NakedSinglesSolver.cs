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
        solution = default;
        int index = cell.Index;
        int box = cell.Box;
        List<int> candidates = puzzle.Candidates[index];

        var row = puzzle.GetCellsForNeighboringRow(box, cell.Row);
        var column = puzzle.GetCellsForNeighboringColumn(box, cell.Column);
        var boxcells = puzzle.GetCellsForBox(box);
        List<IEnumerable<int>> groups = [row, column, boxcells];
        int oneCandidateLeft = candidates.Count - 1;
        HashSet<int> matches = [];

        foreach(var group in groups)
        {
            FindCandidatesToRemove(candidates, group, matches);

            // Solution found
            if (matches.Count == oneCandidateLeft)
            {
                int value = candidates.Except(matches).Single();
                solution = new(cell, value, matches, nameof(NakedSinglesSolver));
                return true;
            }
        }

        // Candidates can be removed
        if (matches.Count > 0)
        {
            solution = new(cell, -1, matches, nameof(NakedSinglesSolver));
            return true;
        }

        return false;
    }

    // Identify candidates that are cancelled out by their existence in neighboring areas
    private static void FindCandidatesToRemove(List<int> candidates, IEnumerable<int> values, HashSet<int> matches)
    {
        foreach (int value in values)
        {
            if (candidates.Contains(value))
            {
                matches.Add(value);
            }
        }
    }
}