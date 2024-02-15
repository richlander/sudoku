using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class NakedPairsSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        IReadOnlyList<int> candidates = puzzle.GetCellCandidates(cell);

        // This solver solely targets cells with 2 or 3 candidates
        // It then looks for matches in cells for those candidates
        if (candidates.Count > 3)
        {
            solution = null;
            return false;
        }

        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];

        foreach (IEnumerable<int> line in lines)
        {
            if (TryFindMatchesInLine(puzzle, candidates, line, cell, out solution))
            {
                return true;
            }
        }

        return false;
    }

    // This is a two pass algorithm
    // Find matching cells with the same two or three candidates as this cell
    // Those candidates can be removed from all others in the line (except the match)
    // This only ever results in candidate removal, never a solution
    private static bool TryFindMatchesInLine(Puzzle puzzle, IReadOnlyList<int> cellCandidates, IEnumerable<int> line, int thisCell, [NotNullWhen(true)] out Solution? solution)
    {
        int count = cellCandidates.Count;
        solution = null;
        List<int>? matches = null;
        bool found = false;

        // is there a match
        foreach (int index in line)
        {
            if (index == thisCell)
            {
                continue;
            }

            IReadOnlyList<int> otherCandidates = puzzle.GetCellCandidates(index);

            if (otherCandidates.Count == count &&
                cellCandidates.SequenceEqual(otherCandidates))
            {
                matches ??= new(3);
                matches.Add(index);
                found = matches.Count == count - 1;

                if (found)
                {
                    break;
                }
            }
        }

        if (!found || matches is null)
        {
            return false;
        }

        // This match will repeat in a single line with the same result
        // We only need to do the computation once
        if (matches.Min() < thisCell)
        {
            return false;
        }

        matches.Add(thisCell);

        // can we remove these candidates from another cell
        // skip "matches" indices
        foreach (int index in line)
        {
            if (matches.Contains(index))
            {
                continue;
            }

            IReadOnlyList<int> otherCandidates = puzzle.GetCellCandidates(index);

            // are any of cellCandidates in otherCandidates?
            List <int> removals = otherCandidates.Intersect(cellCandidates).ToList();
            if (removals.Count > 0)
            {
                Solution s = new(puzzle.GetCell(index), -1, removals, nameof(NakedPairsSolver))
                {
                    Next = solution
                };
                solution = s;
            }
        }

        if (solution is null)
        {
            return false;
        }

        return true;
    }
}
