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
        
        foreach (IEnumerable<int> line in lines)
        {
            if (puzzle.TryFindMatchingCandidates(cell, line, out (int Index, int Value1, int Value2) match))
            {
                // only need to do this once per unit
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
}
