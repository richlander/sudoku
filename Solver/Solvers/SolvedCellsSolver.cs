using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class SolvedCellsSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        HashSet<int> candidates = new(puzzle.GetCellCandidates(cell));
        if (candidates.Count is 1)
        {
            int value = candidates.Single();
            solution = new(cell, value, [], nameof(SolvedCellsSolver));
            return true;
        }

        solution = null;
        return false;
    }
}
