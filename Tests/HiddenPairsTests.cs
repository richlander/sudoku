using Sudoku;
using Xunit.Sdk;

namespace Tests;

public class HiddenPairsTests
{
    [Fact]
    public void HiddenPairsTest()
    {
        // Expected
        string name = "Test Solver";
        List<Solution> solutions = [
            new(GetCell(42), -1, name){RemovalCandidates = [6, 9]},
            new(GetCell(51), -1, name){RemovalCandidates = [1, 5, 9]}
        ];
        // Actual
        string board = "720408030080000047401076802810739000000851000000264080209680413340000008168943275";
        int index = 42;
        HiddenPairsSolver solver = new();
        Puzzle puzzle = new(board);
        Cell cell = Puzzle.GetCellForIndex(index);

        if (solver.TrySolve(puzzle, cell, out Solution? solution))
        {
            PuzzleHelpers.CheckSolutions(puzzle, solution, solutions);
        }
        else
        {
            Assert.Fail(PuzzleHelpers.ErrorMessage);
        }

        static Cell GetCell(int index) => Puzzle.GetCellForIndex(index);
    }

}
