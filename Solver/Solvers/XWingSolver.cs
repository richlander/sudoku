using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class XWingSolver : ISolver
{
    public string Name => nameof(XWingSolver);

    public bool IsTough => true;

    /*
        This is like an elaborate version of (double) pointed pairs
        Multi-step process:
        Validate a locked column (two and only two cells in different boxes with a candidate)
        Two cells have been identified, which gives us two rows
        Find all cells (in other boxes) in row with the same candidate
        Determine if any of those cells are in similarly locked columns
        Do the rows match for the two cells in those columns?
        Are there other cells in those rows with the same candidate in unlocked columns
        Those candidates can be removed
        And we have a "X-Wing"

        The algorithm attempts to create a rectangle (and only works for that shape)
        Find locking match in a cell below in column
        Find another locking match in another column
        If the rows line up, that's a rectangle

        The right-most and bottom-most boxes be skipped entirely
        The rectangles need to be drawn across two boxes and
        the right-most boxes will already have had a chance to participate

        "lower" and "higher" are a bit confusing, as used in the following code
        "higher" is intended to mean a higher index value, but will be spatially lower on the board
        "right" and "left" are naturally intended to orient spatially as well
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        if (cell.Box > 5 || cell.Box % 3 is 2)
        {
            return false;
        }

        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        if (TrySolveColumn(puzzle, cell, cellCandidates, out solution) ||
            TrySolveRow(puzzle, cell, cellCandidates, out solution))
        {
            return true;
        }

        return false;
    }

    // This rectangle gets drawn counterclockwise, down, right, up, and then up left again
    public bool TrySolveColumn(Puzzle puzzle, Cell cell, IReadOnlyList<int> cellCandidates, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        int lowerLeftIndex = cell;
        foreach (int candidate in cellCandidates)
        {
            // Find cells in column (in other boxes) with the same candidates; we want just one
            IEnumerable<int> column = Puzzle.GetColumnIndices(cell.Column).Where(x => x != lowerLeftIndex);
            if (puzzle.TryFindIndexForUniqueValue(cell, column, candidate, out int higherLeftIndex))
            {
                // can skip case were higher cells "look up" the column (will produce same result)
                // If the single match is in the same box, reject
                if (higherLeftIndex < lowerLeftIndex || Puzzle.BoxByIndices[higherLeftIndex] == cell.Box)
                {
                    continue;
                }

                // Find cells in higher row (in other boxes) with the same candidates
                Cell higherLeftCell = puzzle.GetCell(higherLeftIndex);
                IEnumerable<int> higherRow = Puzzle.GetRowIndices(higherLeftCell.Row).Where(x => Puzzle.BoxByIndices[x] != higherLeftCell.Box && puzzle.GetCellCandidates(x).Contains(candidate));
                // Determine if any of those columns are locked for the same candididate
                foreach (int higherRightIndex in higherRow)
                {
                    // only need to try this test once per row
                    if (higherRightIndex < higherLeftIndex)
                    {
                        continue;
                    }

                    Cell higherRightCell = puzzle.GetCell(higherRightIndex);
                    // If column is locked, check if rows match
                    // Find cells in column (in other boxes) with the same candidates; we want just one
                    IEnumerable<int> higherColumn = Puzzle.GetColumnIndices(higherRightCell.Column).Where(x => x != higherRightCell);
                    if (puzzle.TryFindIndexForUniqueValue(higherRightCell, higherColumn, candidate, out int lowerRightIndex))
                    {
   
                        // Test of X-Wing
                        if (Puzzle.RowByIndices[lowerLeftIndex] == Puzzle.RowByIndices[lowerRightIndex])
                        {
                            // Still have to find candidates to remove
                            IEnumerable<int> lowRowCandidates = Puzzle.GetRowIndices(cell.Row).Where(x => !(x == lowerLeftIndex || x == lowerRightIndex) && puzzle.GetCellCandidates(x).Contains(candidate));
                            IEnumerable<int> highRowCandidates = Puzzle.GetRowIndices(higherRightCell.Row).Where(x => !(x == higherLeftIndex || x == higherRightIndex) && puzzle.GetCellCandidates(x).Contains(candidate));

                            List<int> finalList = [];
                            finalList.AddRange(lowRowCandidates);
                            finalList.AddRange(highRowCandidates);

                            foreach (int index in finalList)
                            {
                                Solution s = new(puzzle.GetCell(index), -1, $"{Name}:Column")
                                {
                                    RemovalCandidates = [candidate],
                                    AlignedCandidates = [candidate],
                                    AlignedIndices = [lowerLeftIndex, lowerRightIndex, higherLeftIndex, higherRightIndex],
                                };

                                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                            }

                            if (solution is not null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    // This rectangle gets drawn clockwise, right, down, left, and then up again
    public bool TrySolveRow(Puzzle puzzle, Cell cell, IReadOnlyList<int> cellCandidates, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        int lowerLeftIndex = cell;
        foreach (int candidate in cellCandidates)
        {
            // Find cells in row (in other boxes) with the same candidates; we want just one
            IEnumerable<int> row = Puzzle.GetRowIndices(cell.Row).Where(x => x != lowerLeftIndex);
            if (puzzle.TryFindIndexForUniqueValue(cell, row, candidate, out int lowerRightIndex))
            {
                // can skip case were higher cells "look left" across the row (will produce same result)
                // If the single match is in the same box, reject
                if (lowerRightIndex < lowerLeftIndex || Puzzle.BoxByIndices[lowerRightIndex] == cell.Box)
                {
                    continue;
                }

                // Find cells in right-er column (in other boxes) with the same candidates
                Cell lowerRightCell = puzzle.GetCell(lowerRightIndex);
                IEnumerable<int> rightColumn = Puzzle.GetColumnIndices(lowerRightCell.Column).Where(x => Puzzle.BoxByIndices[x] != lowerRightCell.Box && puzzle.GetCellCandidates(x).Contains(candidate));
                // Determine if any of those columns are locked for the same candididate
                foreach (int higherRightIndex in rightColumn)
                {
                    // only need to try this test once per column
                    if (higherRightIndex < lowerRightIndex)
                    {
                        continue;
                    }

                    Cell higherRightCell = puzzle.GetCell(higherRightIndex);
                    // If row is locked, check if columns match
                    // Find cells in row (in other boxes) with the same candidates; we want just one
                    IEnumerable<int> higherRow = Puzzle.GetRowIndices(higherRightCell.Row).Where(x => x != higherRightIndex);
                    if (puzzle.TryFindIndexForUniqueValue(higherRightCell, higherRow, candidate, out int higherLeftIndex))
                    {
   
                        // Test of X-Wing
                        if (Puzzle.ColumnByIndices[lowerLeftIndex] == Puzzle.ColumnByIndices[higherLeftIndex])
                        {
                            // Still have to find candidates to remove
                            IEnumerable<int> leftColumnCandidates = Puzzle.GetColumnIndices(cell.Column).Where(x => !(x == lowerLeftIndex || x == higherLeftIndex) && puzzle.GetCellCandidates(x).Contains(candidate));
                            IEnumerable<int> rightColumnCandidates = Puzzle.GetColumnIndices(higherRightCell.Column).Where(x => !(x == lowerRightIndex || x == higherRightIndex) && puzzle.GetCellCandidates(x).Contains(candidate));

                            List<int> finalList = [];
                            finalList.AddRange(leftColumnCandidates);
                            finalList.AddRange(rightColumnCandidates);

                            foreach (int index in finalList)
                            {
                                Solution s = new(puzzle.GetCell(index), -1, $"{Name}:Row")
                                {
                                    RemovalCandidates = [candidate],
                                    AlignedCandidates = [candidate],
                                    AlignedIndices = [lowerLeftIndex, lowerRightIndex, higherLeftIndex, higherRightIndex],
                                };

                                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                            }

                            if (solution is not null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
}
