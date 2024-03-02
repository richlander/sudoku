using Sudoku;

namespace Tests;

public class PointedPairsTests
{
    [Fact]
    public void PointingPairsTest()
    {
        // Expected
        string name = "Test Solver";
        List<Solution> expectedSolutions = [
            new(GetCell(32), -1, name){RemovalCandidates = [5, 8]},
            new(GetCell(35), -1, name){RemovalCandidates = [8]},
            new(GetCell(14), -1, name){RemovalCandidates = [4, 6]},
            new(GetCell(68), -1, name){RemovalCandidates = [4, 8, 9]},
            new(GetCell(66), -1, name){RemovalCandidates = [2, 8]},
            new(GetCell(68), -1, name){RemovalCandidates = [4, 8, 9]},            
            new(GetCell( 9), -1, name){RemovalCandidates = [3]},
            new(GetCell(10), -1, name){RemovalCandidates = [3]},
            new(GetCell(11), -1, name){RemovalCandidates = [3]},
            new(GetCell(55), -1, name){RemovalCandidates = [2]},
        ];
        // Actual
        string board = "017903600000080000900000507072010430000402070064370250701000065000030000005601720";
        PointingPairsSolver pointedSolver = new();
        Puzzle puzzle = new(board);
        // Solvers
        List<ISolver> solvers = [
            new HiddenSinglesSolver(),
            new NakedPairsSolver(),
            new HiddenPairsSolver(),
            new PointingPairsSolver()];

        // Test
        IEnumerable<Solution> solutions = Solver.Solve(puzzle, solvers);
        PuzzleHelpers.CheckSolutions(puzzle, solutions, expectedSolutions);

        static Cell GetCell(int index) => Puzzle.GetCellForIndex(index);
    }
}
