using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sudoku;

public class HiddenSinglesSolver : ISolver
{
    public string Name => nameof(HiddenSinglesSolver);

    /*
        Hidden singles: A candidate value that is absent from all other cells in unit (box, row or column).
        Naked singles are a candidate set of 1 (and can be solved with "SolvedCellSolver").
        Hidden singles are in a candidate set > 1, hence not immediately visible.

        0,1 ( 1); 4
          *
        2 4 0 | 0 7 0 | 0 3 8*
        0 0 0 | 0 0 6 | 0 7 0
        3 0 0 | 0 4 0 | 6 0 0
        ------+-------+------
        0 0 8 | 0 2 0 | 7 0 0
        1 0 0 | 0 0 0 | 0 0 6
        0 0 7 | 0 3 0 | 4 0 0
        ------+-------+------
        0 0 4 | 0 8 0 | 0 0 9
        0 6 0 | 4 0 0 | 0 0 0
        9 1 0 | 0 6 0 | 0 0 2

        https://www.sudokuwiki.org/sudoku.htm?bd=200070038000006070300040600008020700100000006007030400004080009060400000910060002

        Task: Find one and only one candidate that is unique in the given cell relative to the unit.
    */
    public bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution)
    {
        List<IEnumerable<int>> lines = [
            Puzzle.GetBoxIndices(cell.Box),
            Puzzle.GetRowIndices(cell.Row),
            Puzzle.GetColumnIndices(cell.Column)];

        foreach(IEnumerable<int> line in lines)
        {
            HashSet<int> cellCandidates = new(puzzle.GetCellCandidates(cell));

            foreach (int neighborIndex in line.Where(x => x != cell))
            {
                IReadOnlyList<int> neighborCandidates = puzzle.GetCellCandidates(neighborIndex);
                cellCandidates.RemoveRange(neighborCandidates);

                if (cellCandidates.Count is 0)
                {
                    break;
                }
            }

            // A solution is present with a single value
            if (cellCandidates.Count is 1)
            {
                int value = cellCandidates.Single();
                solution = new(cell, value, nameof(HiddenSinglesSolver));
                return true;
            }
        }

        solution = null;
        return false;
    }
}
