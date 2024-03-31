using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;

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


        Approach:
        - Find all partial/full matches of candidates in others cells
        - Index by both index and candidate
        - For a given index, we can see which matching candidates it has and how many
        - For a given candidate, we can see which indices it is at
        - First, check if another index has the same number of candidates.
        - That's either a pair or a triple, dependending on the details.
        - Otherwise ... not a pair, and maybe a triple
        - Continue the algorithm for 3/3/2 and 2/2/2 triple forms
        
        Approach for a cell with 2 candidates:
        - This is the most interesting scenario
        - Do the same thing, which is to get all the partial matches
        - This will naturally get all the cells which would be involved in 3/3/2, 3/2/2, and 2/2/2 form
        - For each partial match, do a union on all the other matches (one at a time) + the cell
        - If this results in 3 candidates, then we have a naked triple
        - We can determine the form at that point
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        int candidateCount = cellCandidates.Count;

        if (cellCandidates.Count is > 3)
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
            if (TryFindPartialMatchesForCellCandidates(puzzle, cell, line, out Matches matches))
            {
                var (indexMatches, candidateMatches) = matches;
                List<int> matchingIndices = [ ..indexMatches.Keys.Where(x => indexMatches[x].Count == puzzle.GetCellCandidates(x).Count && x != index) ];
                List<int> matchingThreeIndices = [ ..indexMatches.Keys.Where(x => indexMatches[x].Count is 3) ];
                List<int> matchingTwoIndices = [ ..indexMatches.Keys.Where(x => indexMatches[x].Count is 2 && puzzle.GetCellCandidates(x).Count is 2) ];

                foreach (int index in indexMatches.Keys)
                {
                    List<int> indexCandidates = indexMatches[index];
                    int indexCandidateCount = indexCandidates.Count;


                    // Naked pairs
                    if (candidateCount is 2)
                    {
                        foreach (int matchingIndex in matchingIndices)
                        {
                            List<int> union = cellCandidates.Union(indexCandidates).Union(indexMatches[matchingIndex]).ToList();
                            if (union.Count is 3)
                            {
                                List<int> alignedIndices = [ cell, index, matchingIndex ];
                                if (TryFindSolution(puzzle, cellCandidates, line, alignedIndices, out Solution? s))
                                {
                                    solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                                }    
                            }
                        }

                    }
                    else if (candidateCount is 3)
                    {

                        // Naked triple of form 3/3/3
                        if (matchingThreeIndices.Count is 3)
                        {
                            List<int> alignedIndices = matchingThreeIndices;
                            bool isLowestIndex = alignedIndices.Count(x => x < index) is 1;
                            if (isLowestIndex && TryFindSolution(puzzle, cellCandidates, line, alignedIndices, out Solution? s))
                            {
                                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                            }
                        }
                        // Naked triple of form 3/3/2
                        // Naked triple of form 3/2/2
                        else if (matchingThreeIndices.Count + matchingTwoIndices.Count is 3)
                        {
                            List<int> alignedIndices = [ ..matchingThreeIndices, ..matchingTwoIndices ];
                            bool isLowestIndex = alignedIndices.Count(x => x < index) is 1;
                            if (isLowestIndex && TryFindSolution(puzzle, cellCandidates, line, alignedIndices, out Solution? s))
                            {
                                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                            }
                        }
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
    
    private bool TryFindSolution(Puzzle puzzle, IReadOnlyList<int> candidates, IEnumerable<int> line, List<int> alignedIndices, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;

        foreach (int index in line.Where(x => !(puzzle.IsCellSolved(x) || alignedIndices.Contains(x))))
        {
            IReadOnlyList<int> neighborCandidates = puzzle.GetCellCandidates(index);

            if (neighborCandidates.Intersect(candidates).Any())
            {
                // List<int> removals = neighborCandidates.Except(cellCandidates).ToList();
                Solution s = new(puzzle.GetCell(index), -1, Name)
                {
                    RemovalCandidates = neighborCandidates.Intersect(candidates).ToList(),
                    AlignedCandidates = candidates,
                    AlignedIndices = alignedIndices,
                };
                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
            }
        }

        return solution is {};
    }

    // Finds unique index in line that contains a candidate pair (and only that pair)
    private static bool TryFindPartialMatchesForCellCandidates(Puzzle puzzle, Cell cell, IEnumerable<int> line, out Matches matches)
    {
        // key: candidate; value: list of indexes (where that candidate is present)
        Dictionary<int, List<int>> indicesbyCandidate = [];
        // key: index; value: list of candidates (present at that candidate)
        Dictionary<int, List<int>> candidatesbyIndex = [];
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
        foreach (int index in line.Where(x => x != cell))
        {
            IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);

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

    // private static List<int> FindMatches(Puzzle puzzle, IReadOnlyList<int> cellCandidates, List<int> partialMatches)
    // {
    //     int candidateCount = cellCandidates.Count;
    //     List<int> matches = [];

    //     foreach (int index in partialMatches)
    //     {
    //         IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);
    //         int intersectionCount = cellCandidates.Intersect(candidates).Count();
    //         if (candidateCount == intersectionCount)
    //         {
    //             matches.Add(index);
    //         }
    //     }

    //     return matches;
    // }
}

public record struct Matches(Dictionary<int, List<int>> IndexMatches, Dictionary<int, List<int>> CandidateMatches);

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

/*
            if (TryFindPartialMatchesForCellCandidates(puzzle, cell, line, out Matches matches))
            {
                var (candidateMatches, indexMatches) = matches;
                foreach (int candidate in twoOrThreeCount)
                {
                    List<int> indices = matches[candidate];

                    List<int> matches = FindMatches(puzzle, cellCandidates, indices);

                    // Naked pairs
                    if (candidateCount is 2)
                    {
                        if (matches.Count is 1)
                        {
                            List<int> alignedIndices = [ cell, matches[0] ];
                            if (TryFindSolution(puzzle, cellCandidates, line, alignedIndices, out Solution? s))
                            {
                                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                            }
                        }
                    }
                    // candidateCount is 3
                    // Naked triple of form 3/3/3
                    else if (matches.Count is 2 && matches.Where(x => matches[x].Count ))
                    {
                        List<int> alignedIndices = [ cell, matches[0], matches[1] ];
                        if (TryFindSolution(puzzle, cellCandidates, line, alignedIndices, out Solution? s))
                        {
                            solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
                        }
                    }

                }
            }
*/