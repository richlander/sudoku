using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace Sudoku;

public class PointedPairsSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        Box box = puzzle.GetBox(cell.Box);
        IEnumerable<int> boxLine = Puzzle.GetBoxIndices(cell.Box);

        // We need to determine if there is a candidate in this row that is unique to the box column
        // Only necessary for first cell in each row; answer will repeat
        if (cell.BoxIndex % 3 is 0)
        {
            IEnumerable<int> boxRow = box.GetRowIndices(cell.BoxRow);
            if (TryFindUniqueCandidates(puzzle, boxRow, boxLine, Puzzle.GetRowIndices(cell.Row), out solution))
            {
                return true;
            }
        }

        // We need to determine if there is a candidate in this column that is unique to the box column
        // Only necessary for first/top cell in each column; answer will repeat
        if (cell.BoxIndex < 3)
        {
            IEnumerable<int> boxColumn = box.GetColumnIndices(cell.BoxColumn);
            if (TryFindUniqueCandidates(puzzle, boxColumn, boxLine, Puzzle.GetColumnIndices(cell.Column), out solution))
            {
                return true;
            }
        }
        solution = null;
        return false;
    }

    public bool TryFindUniqueCandidates(Puzzle puzzle, IEnumerable<int> targetLine, IEnumerable<int> testLine, IEnumerable<int> searchLine, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        HashSet<int> targets = new HashSet<int>(targetLine);
        HashSet<int> lineCandidates = new(9);
        
        // Unify the list of candidates
        // Doesn't matter if a candidate shows up in one or alls
        foreach(int index in targets)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            lineCandidates.AddRange(candidates);
        }

        // Clone lineCandidates
        HashSet<int> lineCandidatesSlim = new(lineCandidates);
        // Clean up testLine
        var testCells = testLine.Where(x => !(puzzle.IsCellSolved(x) || targets.Contains(x)));

        // Start removing candidates, per `testLine`
        foreach (int index in testCells)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            lineCandidatesSlim.RemoveRange(candidates);

            if (lineCandidatesSlim.Count is 0)
            {
                break;
            }
        }

        if (lineCandidatesSlim.Count is 0)
        {
            return false;
        }

        // Clean up searchLine
        var SearchCells = searchLine.Where(x => !(targets.Contains(x) || puzzle.IsCellSolved(x)));

        // `lineCandidates` can now be removed from the rest of the `searchLine`
        foreach (int index in SearchCells)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            List<int> removalCandidates = candidates.Intersect(lineCandidatesSlim).ToList();

            if (removalCandidates.Count > 0)
            {
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
