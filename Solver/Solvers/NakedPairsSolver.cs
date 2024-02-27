using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class NakedPairsSolver : ISolver
{
    // Naked pairs solver: matching pairs are locked and allow removal of candidates from other cells
    // Targets cells with 2 candidates
    // It then can remove those 2 candidate values in other cells (in the same unit)
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
            if (puzzle.TryFindCellCandidatePairsMatch(cell, line, out int uniqueIndex))
            {
                // Remove values from other cells in line
                foreach (int index in line.Where(x => !(puzzle.IsCellSolved(x) || x == cell || x == uniqueIndex)))
                {
                    IReadOnlyList<int> neighborCandidates = puzzle.GetCellCandidates(index);

                    List<int> removals = neighborCandidates.Intersect(cellCandidates).ToList();
                    if (removals.Count > 0)
                    {
                        Solution s = new(puzzle.GetCell(index), -1, removals, nameof(NakedPairsSolver))
                        {
                            Next = solution
                        };
                        solution = s;
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
}
