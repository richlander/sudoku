using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class XWingSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        // This is sort of like double pointed pairs

    }

    public bool TryFindUniqueCandidatesColumnLocked(Puzzle puzzle, Cell cell)
    {
        IEnumerable<int> lockedLine = Puzzle.ColumnByIndices
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        IEnumerable<int> lineSlim = lockedLine.Where(x => !puzzle.IsCellSolved(x) || x != cell);
        HashSet<int> localLine = new(lineSlim);

        // Determine which cells these candidates match
        Dictionary<int, List<int>> matchbook = new(cellCandidates.Count);     
        foreach(int index in localLine)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            IEnumerable<int> matchess = candidates.Intersect(cellCandidates);

            foreach (int match in matchess)
            {
                if (matchbook.TryGetValue(match, out List<int>? value))
                {
                    value.Add(index);
                }
                else
                {
                    matchbook.Add(index, new List<int>(9){index});
                }
            }
        }

        // Determine where there is one other cell the candidate appears
        List<int> matches = matchbook.Keys.Where(k => matchbook[k].Count is 1).ToList();

        if (matches.Count is 0)
        {
            return false;
        }

        // We potentially have one side of the X-Wing
        foreach (int match in matches)
        {
            var testLine = 
            if (TryFindCandidateAppearsOnce(puzzle, )
        }

        return false;
    }

    

    public bool TryFindCandidateAppearsOnce(Puzzle puzzle, IEnumerable<int> line, int uniqueValue, out int uniqueIndex)
    {
        int match = -1;
        foreach (int index in line)
        {
            IEnumerable<int> candidates = puzzle.GetCellCandidates(index);
            bool hasMatch = candidates.Contains(uniqueValue);

            if (!hasMatch)
            {
                continue;
            }
            
            if (match is -1)
            {
                match = index;
            }
            else
            {
                break;
            }
        }

        return false;
    }
}
