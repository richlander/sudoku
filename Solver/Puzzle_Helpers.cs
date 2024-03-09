using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Sudoku;
public partial class Puzzle
{
    // Board navigation
    public static IEnumerable<int> GetRowIndices(int index) => IndicesByRow.Skip(index * 9).Take(9);

    public static IEnumerable<int> GetColumnIndices(int index) => IndicesByColumn.Skip(index * 9).Take(9);

    public static IEnumerable<int> GetBoxIndices(int index) => IndicesByBox.Skip(index * 9).Take(9);

    public static Cell GetCellForIndex(int index) => new(
        index,
        index / 9,
        index % 9,
        BoxByIndices[index],
        BoxRowByIndices[index],
        BoxColumnByIndices[index],
        BoxIndices[index]);

    // Used by solvers that want to run once per box column/row
    public bool IsIndexFirstUnsolved(IEnumerable<int> line, int index)
    {
        foreach (int i in line)
        {
            int value = GetValue(i);
            if (value > 0)
            {
                continue;
            }

            if (i == index)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    // Solution handling
    public static void AttachToLastSolution(Solution solution, Solution nextSolution)
    {
        Solution? sol = solution;
        while (sol.Next is not null)
        {
            sol = sol.Next;
        }

        sol.Next = nextSolution;
    }

    public static Solution UpdateSolutionWithNextSolution(Solution? firstSolution, Solution nextSolution)
    {
        if (firstSolution is null)
        {
            return nextSolution;
        }

        AttachToLastSolution(firstSolution, nextSolution);
        return firstSolution;
    }

    // Board solving
    public HashSet<int> MergeCandidates(IEnumerable<int> line)
    {
        HashSet<int> mergedCandidates = [];

        foreach(int index in line)
        {
            IEnumerable<int> candidates = GetCellCandidates(index);
            mergedCandidates.AddRange(candidates);
        }

        return mergedCandidates;
    }

    // Finds unique index in line that contains a specific candidate value
    // Must visit all elements to validate constraint
    public bool TryFindIndexForUniqueValue(Cell cell, IEnumerable<int> line, int uniqueValue, out int uniqueIndex)
    {
        uniqueIndex = -1;
        int matchIndex = -1;
        foreach (int neighborIndex in line.Where(x => x !=cell))
        {
            IEnumerable<int> neighborCandidates = GetCellCandidates(neighborIndex);
            if (!neighborCandidates.Contains(uniqueValue))
            {
                continue;
            }
            
            if (matchIndex > -1)
            {
                return false;
            }

            matchIndex = neighborIndex;
        }

        if (matchIndex > -1)
        {
            uniqueIndex = matchIndex;
            return true;
        }

        return false;
    }

    // Finds unique candidates in targetLine relative to homeLine
    // May return multiple candidates
    // This is typically candidates unique in a row within a box (or similar)
    public bool TryFindUniqueCandidatesInTargetLine(IEnumerable<int> targetLine, IEnumerable<int> homeLine, [NotNullWhen(true)] out HashSet<int>? uniqueCandidates)
    {
        uniqueCandidates = null;
        HashSet<int> targets = new(targetLine);
        HashSet<int> targetCandidates = MergeCandidates(targetLine);

        if (targetCandidates.Count is 0)
        {
            return false;
        }

        // Filter homeLine
        IEnumerable<int> homeLineFiltered = homeLine.Where(x => !(IsCellSolved(x) || targets.Contains(x)));

        // Start removing candidates, from homeLine
        foreach (int index in homeLineFiltered)
        {
            IEnumerable<int> candidates = GetCellCandidates(index);
            targetCandidates.RemoveRange(candidates);

            if (targetCandidates.Count is 0)
            {
                // There is no unique candidate
                return false;
            }
        }

        uniqueCandidates = targetCandidates;
        return true;
    }

    public bool TryFindSolutionWithIntersectionRemoval(IEnumerable<int> targetLine, IEnumerable<int> homeLine, IEnumerable<int> searchLine, ISolver solver, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        if (TryFindUniqueCandidatesInTargetLine(targetLine, homeLine, out HashSet<int>? uniqueCandidates))
        {
            foreach (int candidate in uniqueCandidates)
            {
                List<int> targets = targetLine.Where(x => GetCellCandidates(x).Contains(candidate)).ToList();
                foreach (int index in searchLine.Where(x => !targetLine.Contains(x) && GetCellCandidates(x).Contains(candidate)))
                {
                    Solution s = new(GetCell(index), -1, solver.Name)
                    {
                        RemovalCandidates = [ candidate ],
                        AlignedIndices = targets,
                        Next = solution
                    };
                    solution = s;
                }
            }
        }

        if (solution is null)
        {
            return false;
        }

        return true;
    }

    public bool TryFindSomething(HashSet<int> targetCandidates, IEnumerable<int> searchLine, out Dictionary<int, List<int>> matches)
    {
        matches = [];
        // targetCandidates can now be removed from the rest of the `searchLine`
        foreach (int target in targetCandidates)
        {
            List<int> hasCandidate = searchLine.Where(x => 
                !IsCellSolved(x) && 
                GetCellCandidates(x).Contains(target))
                .ToList();

            if (hasCandidate.Count > 0)
            {
                matches.Add(target, hasCandidate);
            }
        }

        if (matches.Count > 0)
        {
            return true;
        }

        return false;
    }

    public bool TryFindUniqueCandidates(IEnumerable<int> targetLine, IEnumerable<int> homeLine, IEnumerable<int> searchLine, string solver, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        HashSet<int> targets = new(targetLine);
        HashSet<int> targetCandidates = MergeCandidates(targetLine);

        if (targetCandidates.Count is 0)
        {
            return false;
        }

        // Filter homeLine
        IEnumerable<int> homeLineFiltered = homeLine.Where(x => !(IsCellSolved(x) || targets.Contains(x)));

        // Start removing candidates, from homeLine
        foreach (int index in homeLineFiltered)
        {
            IEnumerable<int> candidates = GetCellCandidates(index);
            targetCandidates.RemoveRange(candidates);

            if (targetCandidates.Count is 0)
            {
                // There is no unique candidate
                return false;
            }
        }

        // We now know that the candidate is unique to targetLine within the homeLine unit
        // Filter searchLine
        IEnumerable<int> searchLineFiltered = searchLine.Where(x => !(targets.Contains(x) || IsCellSolved(x)));

        // target line with matching candidates
        List<int> targetLock = targetLine.Where(x => GetCellCandidates(x).Intersect(targetCandidates).Count() == targetCandidates.Count).ToList();

        // targetCandidates can now be removed from the rest of the `searchLine`
        foreach (int index in searchLineFiltered)
        {
            IEnumerable<int> candidates = GetCellCandidates(index);

            if (candidates.Intersect(targetCandidates).Any())
            {
                List<int> removalCandidates = candidates.Intersect(targetCandidates).ToList();
                Solution s = new(GetCell(index), -1, solver)
                {
                    RemovalCandidates = removalCandidates,
                    AlignedCandidates = targetCandidates,
                    AlignedIndices = targetLock,
                };
                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
            }
        }

        if (solution is null)
        {
            return false;
        }

        return true;
    }

    public bool TryFindHiddenMatchingCandidates(Cell cell, IEnumerable<int> line, [NotNullWhen(true)] out Dictionary<int, List<int>>? matches)
    {
        IReadOnlyList<int> cellCandidates = GetCellCandidates(cell);
        // index, values
        Dictionary<int, List<int>> uniqueValues = [];
        // For each candidate, does it show up again and just once?
        foreach (int candidate in cellCandidates)
        {
            // Add an entry for each value we find
            if (TryFindIndexForUniqueValue(cell, line, candidate, out int uniqueIndex))
            {
                if (!uniqueValues.TryGetValue(uniqueIndex, out List<int>? values))
                {
                    values = [];
                    uniqueValues.Add(uniqueIndex, values);
                }
                
                values.Add(candidate);
            }
        }

        if (uniqueValues.Count > 0)
        {
            matches = uniqueValues;
            return true;
        }

        matches = null;
        return false;
    }
}