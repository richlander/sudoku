using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection.Metadata.Ecma335;

namespace Sudoku;

public class HiddenTriplesSolver : ISolver
{
    public string Name => nameof(HiddenTriplesSolver);

    public bool IsTough => false;

    /*
        Hidden triple: Three candidates are hiding among other candidates in three cells.

        https://www.sudokuwiki.org/sudoku.htm?bd=000001030231090000065003100678924300103050006000136700009360570006019843300000000
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        IReadOnlyList<int> cellCandidates = puzzle.GetCellCandidates(cell);
            List<IEnumerable<int>> lines = [
                Puzzle.GetBoxIndices(cell.Box),
                Puzzle.GetRowIndices(cell.Row),
                Puzzle.GetColumnIndices(cell.Column)];

        foreach (IEnumerable<int> line in lines)
        {
            // There is a clue which will help someone in spotting hidden triples. If there is a hidden triple it must be in a unit with two or less solved squares.
            // Source: https://www.sudokuwiki.org/Hidden_Candidates
            if (puzzle.CountSolvedCells(line) > 2)
            {
                continue;
            }

            int[] counts = GetCandidateCounts(puzzle, line);
            List<int> filteredCellCandidates = FilterCandidates(cellCandidates, counts);

            if (filteredCellCandidates.Count < 2)
            {
                continue;
            }

            List<int> searchLine = line.Where(x => x > cell).ToList();
            
            for (int i = 0; i < searchLine.Count; i++)
            {
                int index = searchLine[i];
                IReadOnlyList<int> candidates = puzzle.GetCellCandidates(index);
                List<int> filteredCandidates = FilterCandidates(candidates, counts);

                if (filteredCandidates.Count < 2 ||
                    filteredCellCandidates.Intersect(filteredCandidates).Count() is 0)
                {
                    continue;
                }

                for (int j = i + 1; j < searchLine.Count; j++)
                {
                    int nextIndex = searchLine[j];
                    IReadOnlyList<int> nextCandidates = puzzle.GetCellCandidates(nextIndex);
                    List<int> nextFilteredCandidates = FilterCandidates(nextCandidates, counts);

                    if (nextFilteredCandidates.Count < 2)
                    {
                        continue;
                    }
                    
                    List<int> threeUnion = filteredCellCandidates.Union(filteredCandidates).Union(nextFilteredCandidates).ToList();
                    List<int> alignedIndices = [ cell, index, nextIndex ];

                    // Validate:
                    // - threeUnion.Count is 3
                    // - No instances of threeUnion in other cells
                    if (threeUnion.Count is 3 &&
                        !line.Where(x => 
                            !alignedIndices.Contains(x) &&
                            puzzle.GetCellCandidates(x).Intersect(threeUnion).Any()).Any())
                    {
                        if (TryFindSolution(puzzle, threeUnion, alignedIndices, alignedIndices, out solution))
                        {
                            return true;
                        }
                    }
                }
                
            }
        }

        solution = null;
        return false;
    }

    private static int[] GetCandidateCounts(Puzzle puzzle, IEnumerable<int> line)
    {
        int[] counts = new int[10];
        foreach (int index in line)
        {
            foreach (int candidate in puzzle.GetCellCandidates(index))
            {
                counts[candidate]++;
            }
        }

        return counts;
    }

    private static List<int> FilterCandidates(IReadOnlyList<int> candidates, int[] candidateCounts)
    {
        List<int> filteredCandidates = new(candidates.Count);

        foreach (int candidate in candidates)
        {
            if (candidateCounts[candidate] < 4)
            {
                filteredCandidates.Add(candidate);
            }
        }

        return filteredCandidates;
    }

    private bool TryFindSolution(Puzzle puzzle, IReadOnlyList<int> candidates, IEnumerable<int> line, List<int> alignedIndices, [NotNullWhen(true)] out Solution? solution)
    {
        solution = null;

        foreach (int index in line.Where(x => !(puzzle.IsCellSolved(x))))
        {
            IReadOnlyList<int> neighborCandidates = puzzle.GetCellCandidates(index);

            if (neighborCandidates.Except(candidates).Any())
            {
                // List<int> removals = neighborCandidates.Except(cellCandidates).ToList();
                Solution s = new(puzzle.GetCell(index), -1, Name)
                {
                    RemovalCandidates = neighborCandidates.Except(candidates).ToList(),
                    AlignedCandidates = candidates,
                    AlignedIndices = alignedIndices,
                };
                solution = Puzzle.UpdateSolutionWithNextSolution(solution, s);
            }
        }

        return solution is {};
    }
}
