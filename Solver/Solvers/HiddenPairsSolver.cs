using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

public class HiddenPairsSolver : ISolver
{
    // Hidden singles: One logically absent value in a box, row or column
    // Determine which candidates are unique in the given cell per each unit (box, column, row)
    // If there is just one candidate, then that's the solution
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];
        
        foreach (IEnumerable<int> line in lines)
        {
            if (TryFindMatchingCandidates(puzzle, cell, line, out (int Index, int Value1, int Value2) match))
            {
                if (match.Index < cell)
                {
                    return false;
                }

                List<int> pairs = [match.Value1, match.Value2];
                List<int> indices = [cell, match.Index];
                foreach (int index in indices)
                {
                    IReadOnlyList<int> matchCandidates = puzzle.GetCellCandidates(index);
                    List<int> removals = matchCandidates.Except(pairs).ToList();

                    if (removals.Count is 0)
                    {
                        continue;
                    }

                    Solution s = new(puzzle.GetCell(index), -1, removals, nameof(HiddenPairsSolver))
                    {
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

    private bool TryFindMatchingCandidates(Puzzle puzzle, Cell cell, IEnumerable<int> line, out (int Index, int Value1, int Value2) match)
    {
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        // index, values
        Dictionary<int, List<int>> uniqueValues = [];
        foreach (int candidate in cellCandidates)
        {
            if (puzzle.TryFindValueAppearsOnce(cell, line, candidate, out int uniqueIndex))
            {
                if (!uniqueValues.TryGetValue(uniqueIndex, out List<int>? values))
                {
                    values = [];
                    uniqueValues.Add(uniqueIndex, values);
                }
                
                values.Add(candidate);
            }
        }

        var matches = uniqueValues.Where(x => x.Value.Count is 2).ToList();

        if (matches.Count is 1 && matches[0].Value.Count is 2)
        {
            var f = matches[0];
            match = (f.Key, f.Value[0], f.Value[1]);

            return true;
        }

        match = default;
        return false;
    }
}
