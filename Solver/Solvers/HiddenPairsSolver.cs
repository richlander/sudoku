using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace Sudoku;

public class HiddenPairsSolver : ISolver
{
    public string Name => nameof(HiddenPairsSolver);

    /*
        Hidden pairs: A pair of candidate values that is absent from all other cells in unit (box, row or column).
        Naked pairs are a candidate set of 2, hence immediately visible.
        Hidden pairs are in a candidate set > 2, hence not imemdiately visible.

        3,2 (29); Aligned: 29, 38; Aligned Candidates: 2, 4; Removals: 5, 6
        4,2 (38); Aligned: 29, 38; Aligned Candidates: 2, 4; Removals: 3, 6, 7
            *
        7 2 0 | 4 0 8 | 0 3 0
        0 8 0 | 0 0 0 | 0 4 7
        4 0 1 | 0 7 6 | 8 0 2
        ------+-------+------
        8 1 0 | 7 3 9 | 0 0 0*
        0 0 0 | 8 5 1 | 0 0 0*
        0 0 0 | 2 6 4 | 0 8 0
        ------+-------+------
        2 0 9 | 6 8 0 | 4 1 3
        3 4 0 | 0 0 0 | 0 0 8
        1 6 8 | 9 4 3 | 2 7 5

        https://www.sudokuwiki.org/sudoku.htm?bd=720408030080000047401076802810739000000851000000264080209680413340000008168943275

        Task: Find a candidate pair that is present in one other cell in the unit.
        All other cells can be removed from those two cells.
        Hidden pairs never present a cell solution (since the pairs remain), but only present removals.
        This is sort of the opposite of naked pairs
    */
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
            if (puzzle.TryFindHiddenMatchingCandidates(cell, line, out Dictionary<int, List<int>>? matches))
            {
                // Naked pair: an index that has two unique values (matching cell)
                if (matches.Where(x => x.Value.Count is 2).Count() is not 1)
                {
                    continue;
                }

                KeyValuePair<int, List<int>> match = matches.Where(x => x.Value.Count is 2).Single();
                int uniqueIndex = match.Key;

                // only need to do this once per unit
                if (uniqueIndex < cell || found.Contains(cell))
                {
                    continue;
                }

                List<int> pairs = [match.Value[0], match.Value[1]];
                List<int> indices = [cell, uniqueIndex];
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
                    Solution s = new(puzzle.GetCell(index), -1, Name)
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
