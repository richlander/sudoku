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
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        int index = cell.Index;
        IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);

        // Get boxes
        Box box = puzzle.GetBox(cell.Box);
        // get adjacent neighboring boxes
        Box ahnb1 = puzzle.GetBox(box.FirstHorizontalNeighbor);
        Box ahnb2 = puzzle.GetBox(box.SecondHorizontalNeighbor);
        Box avnb1 = puzzle.GetBox(box.FirstVerticalNeighbor);
        Box avnb2 = puzzle.GetBox(box.SecondVerticalNeighbor);

        NeighborBoxes neighbors = new([ ahnb1, ahnb2 ], [ avnb1, avnb2 ]);

        // if (TrySolveRowOneCellUnsolved(puzzle, boxCell, neighbors, out solution))
        // {
        //     return true;
        // }

        // if (TrySolveColumnOneCellUnsolved(puzzle, boxCell, neighbors, out solution))
        // {
        //     return true;
        // }

        if (TrySolveCell(puzzle, cell, neighbors, out solution))
        {
            return true;
        }

        solution = null;
        return false;
    }

    // Determine which candidates are unique in the given cell per each unit (box, column, row)
    // If there are unique candidates, remove the ones that are not unique
    // If there is just one candidate, then that's the solution
    private static bool TrySolveCell(Puzzle puzzle, Cell cell, NeighborBoxes neighbors, [NotNullWhen(true)] out Solution? solution)
    {
        int rowOne = cell.BoxRow;
        int columnOne = cell.BoxColumn;

        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)
        ];
        
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

record NeighborBoxes(Box[] Horizontal, Box[] Vertical);
