using System.Data;
using System.Net;

namespace Sudoku;

public record Solution(Cell Cell, int Value, string Solver)
{
    // Candidates to remove
    public IEnumerable<int>? RemovalCandidates { get; set; }

    public IEnumerable<int>? AlignedCandidates { get; set; }

    public IEnumerable<int>? AlignedIndices { get; set; }

    public Solution? Next { get; set; }

    public static IEnumerable<Solution> Enumerate(Solution solution)
    {
        Solution? nextSolution = solution;
        while (nextSolution is not null)
        {
            yield return nextSolution;
            nextSolution = nextSolution.Next;
        }
    }
}
