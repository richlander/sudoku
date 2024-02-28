using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

public class HiddenPairsSolver : ISolver
{
    // Hidden pairs solver: matching candidate pairs that show up in only two cells enable removal of candidates in those same cells
    // Targets two cells with multiple candidates, with two matching (and unique to those two cells).
    // This is sort of the opposite of naked pairs
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];

        List<int> found = [];
        
        foreach (IEnumerable<int> line in lines)
        {
            if (puzzle.TryFindMatchingCandidatePairs(cell, line, out (int Index, int Value1, int Value2) match))
            {
                // only need to do this once per unit
                if (match.Index < cell || found.Contains(cell))
                {
                    continue;
                }

                List<int> pairs = [match.Value1, match.Value2];
                List<int> indices = [cell, match.Index];
                // look at only cell and match
                // remove all candidates but pairs
                foreach (int index in indices)
                {
                    IReadOnlyList<int> matchCandidates = puzzle.GetCellCandidates(index);
                    List<int> removals = matchCandidates.Except(pairs).ToList();

                    if (removals.Count is 0)
                    {
                        continue;
                    }

                    found.Add(index);
                    Solution s = new(puzzle.GetCell(index), -1, nameof(HiddenPairsSolver))
                    {
                        RemovalCandidates =removals,
                        AlignedCandidates = pairs,
                        AlignedIndices = indices,
                    };
                    solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
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
