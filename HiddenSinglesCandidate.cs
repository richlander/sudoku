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
    public bool TrySolve(Puzzle puzzle, BoxCell boxCell, [NotNullWhen(true)] out Solution? solution)
    {
        Cell cell = boxCell.Cell;
        int index = cell.Index;

        List<int> candidates = puzzle.Candidates[index];
        Box box = boxCell.Box;
        // get adjacent neighboring boxes
        Box ahnb1 = puzzle.GetBox(box.FirstHorizontalNeighbor);
        Box ahnb2 = puzzle.GetBox(box.SecondHorizontalNeighbor);
        Box avnb1 = puzzle.GetBox(box.FirstVerticalNeighbor);
        Box avnb2 = puzzle.GetBox(box.SecondVerticalNeighbor);

        NeighborBoxes neighbors = new([ ahnb1, ahnb2 ], [ avnb1, avnb2 ]);

        if (TrySolveRowOneCellUnsolved(puzzle, boxCell, neighbors, out solution))
        {
            return true;
        }

        if (TrySolveColumnOneCellUnsolved(puzzle, boxCell, neighbors, out solution))
        {
            return true;
        }

        if (TrySolveCell(puzzle, boxCell, out solution ))
        {
            return true;
        }

        solution = default;
        return false;
    }

    private bool TrySolveRowOneCellUnsolved(Puzzle puzzle, BoxCell boxCell, NeighborBoxes neighbors, [NotNullWhen(true)] out Solution? solution)
    {
        Box box = boxCell.Box;
        bool oneUnsolved = box.GetRowValues(boxCell.Row).Count(num => num is 0) is 1;

        if (oneUnsolved)
        {
            int cellIndex = boxCell.Cell.Index;
            List<int> candidates = puzzle.Candidates[cellIndex];
            int rowIndex =  boxCell.Row;

            // find neighbors
            int two = (rowIndex + 1) % 3;
            int three = (rowIndex + 2) % 3;

            NeighborRows neighborRows = new(neighbors.Horizontal[0].GetRowValues(two),
                                            neighbors.Horizontal[0].GetRowValues(three),
                                            neighbors.Horizontal[1].GetRowValues(two),
                                            neighbors.Horizontal[1].GetRowValues(three));

            if (TrySolveRowOneCellUnsolvedForRow(candidates, box.GetRow(rowIndex), neighborRows, out int value))
            {
                solution = new(boxCell.Cell, value, [], nameof(HiddenSinglesSolver));
                return true;
            }
        }

        solution = default;
        return false;
    }

    private bool TrySolveColumnOneCellUnsolved(Puzzle puzzle, BoxCell boxCell, NeighborBoxes neighbors, [NotNullWhen(true)] out Solution? solution)
    {
        Box box = boxCell.Box;
        bool oneUnsolved = box.GetColumnValues(boxCell.Column).Count(num => num is 0) is 1;

        if (oneUnsolved)
        {
            int cellIndex = boxCell.Cell.Index;
            List<int> candidates = puzzle.Candidates[cellIndex];
            int columnIndex =  boxCell.Column;

            // find neighbors
            int two = (columnIndex + 1) % 3;
            int three = (columnIndex + 2) % 3;

            NeighborRows neighborRows = new(neighbors.Horizontal[0].GetColumnValues(two),
                                            neighbors.Horizontal[0].GetColumnValues(three),
                                            neighbors.Horizontal[1].GetColumnValues(two),
                                            neighbors.Horizontal[1].GetColumnValues(three));

            if (TrySolveRowOneCellUnsolvedForRow(candidates, box.GetColumn(columnIndex), neighborRows, out int value))
            {
                solution = new(boxCell.Cell, value, [], nameof(HiddenSinglesSolver));
                return true;
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

    private static bool TrySolveCell(Puzzle puzzle, BoxCell boxCell, [NotNullWhen(true)] out Solution? solution)
    {
    //     // Cell cell = boxCell.Cell;
    //     IEnumerable<int> columnCells = puzzle.GetCellIndicesForColumn(cell.Column);

    //     if (box.Index is 8 && cell > 6)
    //     {

    //     }

    //     // Candidate unique in column
    //     if (TrySolveCandidateUniqueInLine(puzzle, cellInPuzzle, cellInColumn, columnCells, out int value))
    //     {
    //         solution = Box.GetSolutionForBox(box.Index, cell, value, nameof(HiddenSinglesSolver));
    //         return true;
    //     }
        
    //     solution = default;
    //     return false;
    // }

    // private static bool TrySolveCandidateUniqueInLine(Puzzle puzzle, int index, int indexInLine, IEnumerable<int> cells, out int value)
    // {
    //     List<int> candidates = puzzle.Candidates[index];
    //     IEnumerable<int> candidates2 = candidates;
    //     int count = 0;
    //     value = 0;

    //     foreach (int cell in cells)
    //     {
    //         if (indexInLine == count)
    //         {
    //             count++;
    //             continue;
    //         }

    //         List<int> cellCandidates = puzzle.Candidates[cell];
    //         candidates2 = candidates2.Except(cellCandidates);

    //         if (candidates2.Count() is 0)
    //         {
    //             return false;
    //         }

    //         count++;
    //     }

    //     if (candidates2.Count() is 1)
    //     {
    //         value = candidates2.Single();
    //         return true;
    //     }

        solution = default;
        return false;
    }

}

record NeighborRows(IEnumerable<int> Box1Row2, IEnumerable<int> Box1Row3, IEnumerable<int> Box2Row2, IEnumerable<int> Box2Row3);

record NeighborBoxes(Box[] Horizontal, Box[] Vertical);
