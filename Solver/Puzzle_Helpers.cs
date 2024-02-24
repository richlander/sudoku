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
        BoxIndices[index]
    );

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
        Solution? s = solution;
        while (s.Next is not null)
        {
            s = s.Next;
        }

        solution.Next = nextSolution;
    }

    // Board solving
    // Finds unique index in line that contains a specific candidate value
    public bool TryFindValueAppearsOnce(Cell cell, IEnumerable<int> line, int uniqueValue, out int uniqueIndex)
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

        uniqueIndex = matchIndex;
        return true;
    }

    // Finds unique index in line that contains a candidate pair (and only that pair)
    // Assumed that cell has only two candidates
    public bool TryFindCellCandidatePairsAppearOnce(Cell cell, IEnumerable<int> line, out int uniqueIndex)
    {
        IReadOnlyList<int> cellCandidates = GetCellCandidates(cell);
        uniqueIndex = -1;
        int matchIndex = -1;
        foreach (int neighborIndex in line.Where(x => x !=cell))
        {
            IReadOnlyList<int> neighborCandidates = GetCellCandidates(neighborIndex);
            if (neighborCandidates.Count is not 2 || 
                neighborCandidates.Intersect(cellCandidates).Count() is not 2)
            {
                continue;
            }
            
            if (matchIndex > -1)
            {
                return false;
            }

            matchIndex = neighborIndex;
        }

        if (matchIndex is -1)
        {
            return false;
        }

        uniqueIndex = matchIndex;
        return true;
    }

    // Finds any cells in line with match of any cell candidates
    public bool TryFindMatchingCandidates(Cell cell, IEnumerable<int> line, out Dictionary<int, List<int>>? matches)
    {
        IReadOnlyList<int> cellCandidates = GetCellCandidates(cell);
        // index, values
        Dictionary<int, List<int>> uniqueValues = [];
        foreach (int candidate in cellCandidates)
        {
            if (TryFindValueAppearsOnce(cell, line.Where(x => x != cell), candidate, out int uniqueIndex))
            {
                if (!uniqueValues.TryGetValue(uniqueIndex, out List<int>? values))
                {
                    values = [];
                    uniqueValues.Add(uniqueIndex, values);
                }
                
                values.Add(candidate);
            }
        }

        if (uniqueValues.Count is 0)
        {
            matches = null;
            return false;
        }

        matches = uniqueValues;
        return true;
    }


    public bool TryFindUniqueCandidates(IEnumerable<int> targetLine, IEnumerable<int> homeLine, IEnumerable<int> searchLine, string solver, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        HashSet<int> targets = new(targetLine);
        HashSet<int> targetCandidates = new(9);
        
        // Unify the list of candidates
        // Doesn't matter if a candidate shows up in one or all cells
        foreach(int index in targets)
        {
            IEnumerable<int> candidates = GetCellCandidates(index);
            targetCandidates.AddRange(candidates);
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

        // targetCandidates can now be removed from the rest of the `searchLine`
        foreach (int index in searchLineFiltered)
        {
            IEnumerable<int> candidates = GetCellCandidates(index);

            if (candidates.Intersect(targetCandidates).Any())
            {
                List<int> removalCandidates = candidates.Intersect(targetCandidates).ToList();
                Solution s = new(GetCell(index), -1, removalCandidates, solver)
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