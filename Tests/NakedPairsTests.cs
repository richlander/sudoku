using Sudoku;

namespace Tests;

public class NakedPairsTests
{
    [Fact]
    public void NakedPairsTest()
    {
        // Expected
        string name = "Test Solver";
        List<Solution> expectedSolutions = [
            new(GetCell(18), -1, name){RemovalCandidates = [1]},
            new(GetCell( 4), -1, name){RemovalCandidates = [1]},
            new(GetCell( 5), -1, name){RemovalCandidates = [1, 6]},
            new(GetCell( 6), -1, name){RemovalCandidates = [6]}
        ];
        // Actual
        string board = "400000938032094100095300240370609004529001673604703090957008300003900400240030709";
        int index = 1;
        NakedPairsSolver solver = new();
        Puzzle puzzle = new(board);
        Cell cell = Puzzle.GetCellForIndex(index);
        // Test
        IEnumerable<Solution> solutions = Solver.Solve(puzzle, [solver]);
        PuzzleHelpers.CheckSolutions(puzzle, solutions, expectedSolutions);

        static Cell GetCell(int index) => Puzzle.GetCellForIndex(index);
    }
}