using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class SolvedCellsSolver : ISolver
{
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        if (puzzle.GetCellCandidates(cell).Count is 1)
        {
            int value = puzzle.GetCellCandidates(cell).Single();
            solution = new(cell, value, nameof(SolvedCellsSolver));
            return true;
        }

        solution = null;
        return false;
    }
}
