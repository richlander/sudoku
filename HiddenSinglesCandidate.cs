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
        Box box = puzzle.GetBox(index);
        // get adjacent neighboring boxes
        int ahnb1 = box.FirstHorizontalNeighbor;
        int ahnb2 = box.SecondHorizontalNeighbor;
        int avnb1 = box.FirstVerticalNeighbor;
        int avnb2 = box.SecondVerticalNeighbor;

        solution = default;
        return false;
    }

    private bool TrySolveRowOneCellUnsolved(Puzzle puzzle, Box box, NeighborRows neighbors, [NotNullWhen(true)] out Solution? solution)
    {
        // Try for three rows
        for (int i = 0; i < 3; i++)
        {
            // get row values
            int[] row = box.GetRowValues(i, puzzle).ToArray();

            // test for one cell unsolved in row
            int unsolvedCount = 0;
            int unsolvedIndex = 0;
            foreach(int cell in row)
            {
                if (cell is 0)
                {
                    unsolvedCount++;
                    unsolvedIndex++;
                }
                else if (unsolvedCount is 0)
                {
                    unsolvedIndex++;
                }

            }

            if (unsolvedCount is 1)
            {
                int boxCellIndex = (i * 3) + unsolvedIndex;
                int cellIndex = box.CellsForCells[boxCellIndex];
                List<int> candidates = puzzle.Candidates[cellIndex];

                if (TrySolveRowOneCellUnsolvedForRow(candidates, row, neighbors, out solution))
                {
                    return true;
                }
            }
        }

        solution = default;
        return false;
    }

    // One cell unsolved in row
    private static bool TrySolveRowOneCellUnsolvedForRow(IEnumerable<int> candidates, IEnumerable<int> row, NeighborRows neighbors, out int value)
    {
        // Union each row -- all the values that cannot be in those rows within the box
        IEnumerable<int> neighborRow2Union = neighbors.Box1Row2.Union(neighbors.Box2Row2);
        IEnumerable<int> neighborRow3Union = neighbors.Box1Row3.Union(neighbors.Box2Row3);
        
        // Determine matching candidates
        IEnumerable<int> row2Candidates = neighborRow2Union.Intersect(candidates);
        IEnumerable<int> row3Candidates = neighborRow3Union.Intersect(candidates);

        // If there is one matching candidate, then we have a solution
        IEnumerable<int> row1Candidates = row2Candidates.Intersect(row3Candidates);

        if (row1Candidates.ValueIfCountExact(1, out value))
        {
            return true;
        }

        return false;
    }
}

record NeighborRows(IEnumerable<int> Box1Row2, IEnumerable<int> Box1Row3, IEnumerable<int> Box2Row2, IEnumerable<int> Box2Row3);
