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

    // Finds candidate matches in one and only one other cell in line relative to cell
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

    // Finds solutions in searchline using candidates that are inique in targetLine relative to homeLine
    // This is typically candidates unique in a row within a box (or similar), which locks those candidates
    // and allows removing them in the full row (minus the row in the box)
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

    // Finds unique index in line that contains a candidate pair (and only that pair)
    public bool TryFindPartialMatchesForCellCandidates(Cell cell, IEnumerable<int> line, out Matches matches)
    {
        // key: candidate; value: list of indexes (where that candidate is present)
        Dictionary<int, List<int>> indicesbyCandidate = [];
        // key: index; value: list of candidates (present at that candidate)
        Dictionary<int, List<int>> candidatesbyIndex = [];
        IReadOnlyList<int> cellCandidates = GetCellCandidates(cell);
        foreach (int index in line.Where(x => x > cell))
        {
            IReadOnlyList<int> candidates = GetCellCandidates(index);

            if (candidates.Count is 0 or > 3)
            {
                continue;
            }

            // Will find where intersections as low as one candidate
            foreach (int candidate in cellCandidates.Intersect(candidates))
            {
                if (!candidatesbyIndex.TryGetValue(index, out List<int>? matchingCandidates))
                {
                    matchingCandidates = [];
                    candidatesbyIndex.Add(index, matchingCandidates);
                }

                if (!indicesbyCandidate.TryGetValue(candidate, out List<int>? matchingIndices))
                {
                    matchingIndices = [];
                    indicesbyCandidate.Add(candidate, matchingIndices);
                }

                // Records just the index since more analysis needed
                matchingIndices.Add(index);
                matchingCandidates.Add(candidate);
            }
        }

        if (indicesbyCandidate.Count != 0 && candidatesbyIndex.Count != 0)
        {
            matches = new(candidatesbyIndex, indicesbyCandidate);
            return true;
        }

        matches = new();
        return false;
    }

    public int CountSolvedCells(IEnumerable<int> line)
    {
        int count = 0;
        foreach (int index in line)
        {
            if (_board[index] > 0)
            {
                count++;
            }
        }

        return count;
    }
}