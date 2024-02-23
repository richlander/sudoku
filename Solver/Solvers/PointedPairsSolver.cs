using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace Sudoku;

public class PointedPairsSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        Box box = puzzle.GetBox(cell.Box);
        IEnumerable<int> boxLine = Puzzle.GetBoxIndices(cell.Box);

        // We need to determine if there is a candidate in this row that is unique to the box column
        // Only necessary for first cell in each row; answer will repeat
        if (cell.BoxIndex % 3 is 0)
        {
            IEnumerable<int> boxRow = box.GetRowIndices(cell.BoxRow);
            if (TryFindUniqueCandidates(puzzle, boxRow, boxLine, Puzzle.GetRowIndices(cell.Row), out Solution? s))
            {
                solution = s;
            }
        }

        // We need to determine if there is a candidate in this column that is unique to the box column
        // Only necessary for first/top cell in each column; answer will repeat
        if (cell.BoxIndex < 3)
        {
            IEnumerable<int> boxColumn = box.GetColumnIndices(cell.BoxColumn);
            if (TryFindUniqueCandidates(puzzle, boxColumn, boxLine, Puzzle.GetColumnIndices(cell.Column), out Solution? s))
            {
                if (solution is null)
                {
                    solution = s;
                }
                else
                {
                    Puzzle.AttachToLastSolution(solution, s);
                }
            }
        }

        if (solution is null)
        {
            return false;
        }

        return true;
    }

    public bool TryFindUniqueCandidates(Puzzle puzzle, IEnumerable<int> targetLine, IEnumerable<int> homeLine, IEnumerable<int> searchLine, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        HashSet<int> targets = new(targetLine);
        HashSet<int> targetCandidates = new(9);
        
        // Unify the list of candidates
        // Doesn't matter if a candidate shows up in one or all cells
        foreach(int index in targets)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            targetCandidates.AddRange(candidates);
        }

        // Filter homeLine
        IEnumerable<int> homeLineFiltered = homeLine.Where(x => !(puzzle.IsCellSolved(x) || targets.Contains(x)));

        // Start removing candidates, from homeLine
        foreach (int index in homeLineFiltered)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            targetCandidates.RemoveRange(candidates);

            if (targetCandidates.Count is 0)
            {
                // There is no unique candidate
                return false;
            }
        }

        // We now know that the candidate is unique to targetLine within the homeLine unit
        // Filter searchLine
        IEnumerable<int> searchLineFiltered = searchLine.Where(x => !(targets.Contains(x) || puzzle.IsCellSolved(x)));

        // targetCandidates can now be removed from the rest of the `searchLine`
        foreach (int index in searchLineFiltered)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);

            if (candidates.Intersect(targetCandidates).Any())
            {
                List<int> removalCandidates = candidates.Intersect(targetCandidates).ToList();
                Solution s = new(puzzle.GetCell(index), -1, removalCandidates, nameof(PointedPairsSolver))
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
