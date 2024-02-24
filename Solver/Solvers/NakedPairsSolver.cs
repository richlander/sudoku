using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class NakedPairsSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);

        // This solver solely targets cells with 2 candidates
        // It then looks for matches in cells for those candidates
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
            if (puzzle.TryFindCellCandidatesAppearOnce(cell, line, out int uniqueIndex))
            {
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
