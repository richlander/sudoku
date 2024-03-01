using Sudoku;

namespace Tests;

public class HiddenSingles
{
    [Fact]
    public void HiddenSinglesTest()
    {
        // Expected
        int expectedValue = 4;
        // Actual
        string board = "200070038000006070300040600008020700100000006007030400004080009060400000910060002";
        HiddenSinglesSolver solver = new();
        Puzzle puzzle = new(board);
        Cell cell = Puzzle.GetCellForIndex(1);
        solver.TrySolve(puzzle, cell, out Solution? solution);

        Assert.True(solution?.Value == expectedValue, $"Expected: {expectedValue}; Observed: {solution?.Value};");
    }
}