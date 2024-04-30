using System.Diagnostics.CodeAnalysis;

namespace Sudoku;

public class SolvedCellsSolver : ISolver
{
    public string Name => nameof(SolvedCellsSolver);

    public bool IsTough => false;


    /*
        Solved cell: This could be called "naked single", because there is only one candidate left.

        The critical aspect of this solver is that it is run after other solvers to find other values,
        both to make the board consistent and to avoid the expense of more exotic solvers.
        Sometimes, this solver can find many solutions in a row.
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        if (puzzle.GetCellCandidates(cell).Count is 1)
        {
            int value = puzzle.GetCellCandidates(cell).Single();
            solution = new(cell, value, Name);
            return true;
        }

        solution = null;
        return false;
    }
}
