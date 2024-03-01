using System.Net;
using Sudoku;

namespace Tests;

public class PuzzleHelpers
{
    // Box
    public static int GetBoxForCell(int index) => index / 27 * 3 + index % 9 / 3;
    
    public static int GetPuzzleIndexForBoxCell(int box, int index) => (box / 3 * 27) + (box % 3 * 3) + (index / 3 * 9) + index % 3;
    
    public static int GetFirstCellForBox(int index) => index / 3 * 27 + index % 3 * 3;

    public static int GetBoxIndexForRowColumn(int row, int column) => row % 3 * 3 + (column % 3);

    public static int GetBoxIndexforCell(int index) => GetBoxIndexForRowColumn(index / 9, index % 9);

    // Solvers
    private static List<ISolver> solvers = [
        new HiddenSinglesSolver(),
        new NakedPairsSolver(),
        new HiddenPairsSolver(),
        new PointedPairsSolver(),
        new BoxLineReductionSolver(),
        new XWingSolver()];

    public static List<ISolver> Solvers => solvers;
}
