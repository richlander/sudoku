using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class NakedPairsSolver : ISolver
{
    public string Name => nameof(NakedPairsSolver);

    public bool IsTough => false;

    /*
        Naked pairs: matching pairs are locked and allow removal of candidates from other cells in the same unit

        2,0 (18); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 1
        0,3 ( 3); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 1
        0,4 ( 4); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 1, 6
        0,5 ( 5); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 6
          * *
        4 0 0 | 0 0 0 | 9 3 8*
        0 3 2 | 0 9 4 | 1 0 0
        0 9 5 | 3 0 0 | 2 4 0
        ------+-------+------
        3 7 0 | 6 0 9 | 0 0 4
        5 2 9 | 0 0 1 | 6 7 3
        6 0 4 | 7 0 3 | 0 9 0
        ------+-------+------
        9 5 7 | 0 0 8 | 3 0 0
        0 0 3 | 9 0 0 | 4 0 0
        2 4 0 | 0 3 0 | 7 0 9

        https://www.sudokuwiki.org/sudoku.htm?bd=400000938032094100095300240370609004529001673604703090957008300003900400240030709

        Task: Find a cell with two candidates where another cell in same unit has the same two candidates.
        Remove those values (either one or both) from other cells in the unit.
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);

        if (cellCandidates.Count is not 2)
        {
            return false;
        }

        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];

        foreach (IEnumerable<int> line in lines)
        {
            // Which index has a matching pair to cell?
            if (TryFindCandidatePair(puzzle, cell, line, out int uniqueIndex))
            {
                if (uniqueIndex < cell)
                {
                    continue;
                }

                // Remove values from other cells in line
                foreach (int index in line.Where(x => !(puzzle.IsCellSolved(x) || x == cell || x == uniqueIndex)))
                {
                    IReadOnlyList<int> neighborCandidates = puzzle.GetCellCandidates(index);

                    if (neighborCandidates.Intersect(cellCandidates).Any())
                    {
                        // List<int> removals = neighborCandidates.Except(cellCandidates).ToList();
                        Solution s = new(puzzle.GetCell(index), -1, Name)
                        {
                            RemovalCandidates = neighborCandidates.Intersect(cellCandidates).ToList(),
                            AlignedCandidates = cellCandidates,
                            AlignedIndices = [cell, uniqueIndex],
                        };
                        solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                    }
                }
            }
        }

        if (solution is null)
        {
            return false;
        }

        return true;
    }

    // Finds unique index in line that contains a candidate pair (and only that pair)
    public static bool TryFindCandidatePair(Puzzle puzzle, Cell cell, IEnumerable<int> line, out int matchIndex)
    {
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        foreach (int neighborIndex in line.Where(x => x !=cell))
        {
            IReadOnlyList<int> neighborCandidates = puzzle.GetCellCandidates(neighborIndex);
            if (neighborCandidates.Count is 2 &&
                neighborCandidates.Intersect(cellCandidates).Count() is 2)
            {
                matchIndex = neighborIndex;
                return true;
            }
        }

        matchIndex = -1;
        return false;
    }
}
