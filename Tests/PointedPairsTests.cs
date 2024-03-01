using Sudoku;

namespace Tests;

public class PointedPairsTests
{
    [Fact]
    public void PointedPairsTest()
    {
        // Expected
        string name = "Test Solver";
        List<Solution> solutions = [
            new(GetCell(55), -1, name){RemovalCandidates = [2]},
        ];
        // Actual
        string board = "017903600000080000900000507072010430000402070064370250701000065000030000005601720";
        int index = 57;
        Solution? solution = null;
        PointedPairsSolver pointedSolver = new();
        HiddenPairsSolver hiddenSolver = new();
        Puzzle puzzle = new(board);

        // Prep puzzle
        if (hiddenSolver.TrySolve(puzzle, GetCell(66), out solution))
        {
            puzzle.UpdateBoard(solution);
        }
        else
        {
            Assert.Fail(PuzzleHelpers.ErrorMessage);
        }

        // Test
        solution = null;
        Cell cell = Puzzle.GetCellForIndex(index);
        if (pointedSolver.TrySolve(puzzle, cell, out solution))
        {
            PuzzleHelpers.CheckSolutions(cell, solution, solutions);
        }
        else
        {
            Assert.Fail(PuzzleHelpers.ErrorMessage);
        }

        static Cell GetCell(int index) => Puzzle.GetCellForIndex(index);
    }
}
