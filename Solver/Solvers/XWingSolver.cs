using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Transactions;

namespace Sudoku;

public class XWingSolver : ISolver
{
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
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        if (cell.Box > 5 || cell.Box % 3 is 2)
        {
            return false;
        }

        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        if (TrySolveColumn(puzzle, cell, cellCandidates, out (int Value, List<int> Indices) winged))
        {
            foreach (int index in winged.Indices)
            {
                Cell solutionCell = puzzle.GetCell(index);
                Solution s = new(solutionCell, -1, [winged.Value], nameof(XWingSolver))
                {
                    Next = solution
                };

                solution = s;
            }

            if (solution is {})
            {
                return true;
            }
        }

        return false;
    }

    public bool TrySolveColumn(Puzzle puzzle, Cell cell, IReadOnlyList<int> cellCandidates, [NotNullWhen(true)] out (int Value, List<int> Indices) winged)
    {
        // "lower" and "higher" are a bit confusing
        // "higher" is intended to mean a higher index value, but will be spatially lower
        int lowerLeftIndex = cell;
        foreach (int candidate in cellCandidates)
        {
            // Find cells in column (in other boxes) with the same candidates; we want just one
            IEnumerable<int> column = Puzzle.GetColumnIndices(cell.Column).Where(x => x != lowerLeftIndex);
            if (TryFindCandidateAppearsOnce(puzzle, cell, column, candidate, out int higherLeftIndex))
            {
                // If the single match is in the same box, reject
                // can safely skip case were higher cells "look up" the column
                if (Puzzle.BoxByIndices[higherLeftIndex] == cell.Box ||
                    higherLeftIndex < lowerLeftIndex)
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
                    if (TryFindCandidateAppearsOnce(puzzle, higherRightCell, higherColumn, candidate, out int lowerRightIndex))
                    {
   
                        // Test of X-Wing
                        if (Puzzle.RowByIndices[lowerLeftIndex] == Puzzle.RowByIndices[lowerRightIndex])
                        {
                            // Still have to find candidates to remove
                            IEnumerable<int> lowRowCandidates = Puzzle.GetRowIndices(cell.Row).Where(x => x != lowerLeftIndex && x != lowerRightIndex && puzzle.GetCellCandidates(x).Contains(candidate));
                            IEnumerable<int> highRowCandidates = Puzzle.GetRowIndices(higherRightCell.Row).Where(x => x != higherLeftIndex && x != higherRightIndex && puzzle.GetCellCandidates(x).Contains(candidate));

                            List<int> finalList = [];
                            finalList.AddRange(lowRowCandidates);
                            finalList.AddRange(highRowCandidates);

                            if (finalList.Count > 0)
                            {
                                winged = (candidate, finalList);
                                return true;
                            }
                        }
                    }
                }
            }
        }

        winged = default;
        return false;
    }

    public bool TryFindLineLockedCandidates(Puzzle puzzle, Cell cell, IEnumerable<int> line, [NotNullWhen(true)] out Dictionary<int, List<int>>? lockCandidates)
    {
        // Candidates for cell
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        // open cells to check
        IEnumerable<int> effectiveLine = line.Where(x => !(puzzle.IsCellSolved(x) || x == cell));
        HashSet<int> lockedLine = new(effectiveLine);
        lockCandidates = null;

        if (lockedLine.Count is 0)
        {
            return false;
        }

        // Determine which cells these candidates match
        Dictionary<int, List<int>> matchbook = new(cellCandidates.Count);     
        foreach(int index in lockedLine)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            IEnumerable<int> matchingCandidates = candidates.Intersect(cellCandidates);

            foreach (int match in matchingCandidates)
            {
                if (matchbook.TryGetValue(match, out List<int>? value))
                {
                    value.Add(index);
                }
                else
                {
                    matchbook.Add(match, new List<int>(9){index});
                }
            }
        }

        // Determine where there is one other cell the candidate appears
        List<int> matches = matchbook.Keys.Where(k => matchbook[k].Count is 1).ToList();

        if (matches.Count is 0)
        {
            return false;
        }

        lockCandidates = matchbook;
        return true;
    }

    public bool TryFindCandidateAppearsOnce(Puzzle puzzle, Cell cell, IEnumerable<int> line, int uniqueValue, out int uniqueIndex)
    {
        uniqueIndex = -1;
        int match = -1;
        foreach (int index in line)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            if (!candidates.Contains(uniqueValue))
            {
                continue;
            }
            
            if (match > -1)
            {
                return false;
            }

            match = index;
        }

        if (match is 0)
        {
            return false;
        }

        uniqueIndex = match;
        return true;
    }
}
