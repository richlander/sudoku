using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class NakedTriplesSolver : ISolver
{
        public string Name => nameof(NakedTriplesSolver);

    /*
        Naked triples: matching tiples are three locked cells that contain (in aggregate) the same three candidates and allow removal of those candidates from other cells in the same unit
        Can be in 3/3/3, 3/3/2, 3/2/2, or 2/2/2 forms (number of candidates in each of the three cells).

        2,0 (18); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 1
        0,3 ( 3); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 1
        0,4 ( 4); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 1, 6
        0,5 ( 5); Aligned: 1, 2; Aligned Candidates: 1, 6; Removals: 6
          * *
        4 0 0 | 0 0 0 | 9 3 8*
        0 3 2 | 0 9 4 | 1 0 0
        0 9 5 | 3 0 0 | 2 4 0
        ------+-------+------
        3 7 0 | 6 0 9 | 0 0 4
        5 2 9 | 0 0 1 | 6 7 3
        6 0 4 | 7 0 3 | 0 9 0
        ------+-------+------
        9 5 7 | 0 0 8 | 3 0 0
        0 0 3 | 9 0 0 | 4 0 0
        2 4 0 | 0 3 0 | 7 0 9

        https://www.sudokuwiki.org/sudoku.htm?bd=400000938032094100095300240370609004529001673604703090957008300003900400240030709

        Task: Find a cell with two candidates where another cell in same unit has the same two candidates.
        Remove those values (either one or both) from other cells in the unit.
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        int candidateCount = cellCandidates.Count;

        if (candidateCount > 3)
        {
            return false;
        }

        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];

        foreach (IEnumerable<int> line in lines)
        {
            // Which candiates are present in other cells in unit?
            if (TryFindCandidatePairsMatchCell(puzzle, cell, line, out Dictionary<int, List<int>>? matches))
            {
                IEnumerable<int> twoOrThreeCount = matches.Keys.Where(x => matches[x].Count <= 3);
                foreach (int candidate in twoOrThreeCount)
                {
                    List<int> indices = matches[candidate];

                    List<int> matchingCells = FindMatches(puzzle, cellCandidates, indices);

                    if (candidateCount is 2 && matches.Count is 2)
                    {
                        // return solution
                    }
                    else if (candidateCount is 3 && matches.Count is 3)
                    {
                        // return solution
                    }
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
    public static bool TryFindCandidatePairsMatchCell(Puzzle puzzle, Cell cell, IEnumerable<int> line, [NotNullWhen(true)] out Dictionary<int, List<int>>? matches)
    {
        Dictionary<int, List<int>> match = [];
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        foreach (int index in line.Where(x => x !=cell && !puzzle.IsCellSolved(x)))
        {
            IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);

            if (candidates.Count > 3)
            {
                continue;
            }

            foreach (int candidate in candidates.Intersect(cellCandidates))
            {
                if (!match.TryGetValue(candidate, out List<int>? indices))
                {
                    indices = [];
                }

                indices.Add(index);
            }
        }

        if (match.Count != 0)
        {
            matches = match;
            return true;
        }

        matches = null;
        return false;
    }

    private static List<int> FindMatches(Puzzle puzzle, IReadOnlyList<int> cellCandidates, List<int> indices)
    {
        int candidateCount = cellCandidates.Count;
        List<int> matches = [];

        foreach (int index in indices)
        {
            IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);
            int intersectionCount = candidates.Intersect(candidates).Count();
            if (intersectionCount == candidateCount ||
                intersectionCount == candidates.Count)
            {
                matches.Add(index);
            }
        }

        return matches;
    }

    private static List<int> FindMatchesPartial(Puzzle puzzle, IReadOnlyList<int> baselineCandidates, List<int> indices)
    {
        foreach (int index in indices)
        {
            IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);
            if (baselineCandidates.Intersect(candidates).Count() != 0)
            {
                matches.Add(index);
            }
        }

    }
}


/*
    Algorithm:
    - Candidate is in <= 3 cells (that's an implication of the point above)
    - Candidate is in two cells, both with two candidates, and they matches (then naked pair)
    - Candidate is in three cells, all with three candidates, and they match (then naked triple)
    - Candidate is in three cells, all with two or three candidates, all with just three candidates in total (then naked triple)
    - Candidate is in two cells, all with two candidates, all with just three candidates in total (then naked triple)
    - The 3 candidate values can be removed from the rest of the cells in the unit

    Data format:
    - Dictionary of candidate, List of indexes
    - If candidate list length is >3, skip
    - If candidate list is 3, do x
    - If candidate list is 2, do y
*/