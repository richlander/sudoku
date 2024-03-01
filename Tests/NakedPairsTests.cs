using Sudoku;

namespace Tests;

public class NakedPairsTests
{
    [Fact]
    public void NakedPairsTest()
    {
        // Expected
        string name = "Test Solver";
        List<Solution> solutions = [
            new(GetCell(18), -1, name){RemovalCandidates = [1]},
            new(GetCell( 4), -1, name){RemovalCandidates = [1]},
            new(GetCell( 5), -1, name){RemovalCandidates = [1, 6]},
            new(GetCell( 6), -1, name){RemovalCandidates = [6]}
        ];
        // Actual
        string board = "400000938032094100095300240370609004529001673604703090957008300003900400240030709";
        NakedPairsSolver solver = new();
        Puzzle puzzle = new(board);
        Cell cell = Puzzle.GetCellForIndex(1);
        solver.TrySolve(puzzle, cell, out Solution? solution);
        int count = 0;

        while (solution is not null)
        {
            Assert.True(count < solutions.Count, $"Expected solutions: {solutions.Count}; Observed: {count};");
            Solution expectedSolution = solutions[count];

            if (solution.RemovalCandidates is null || expectedSolution.RemovalCandidates is null)
            {
                Assert.True(false, "Expected `RemovalCandidates` to be non-null");
            }

            int candidateCount = expectedSolution.RemovalCandidates.Count();
            bool matchingCount = solution.RemovalCandidates.Count() == candidateCount;
            bool matchingContent = solution.RemovalCandidates.Intersect(expectedSolution.RemovalCandidates).Count() == candidateCount;
            bool matchingValue = solution.Value == expectedSolution.Value;
            Assert.True(matchingCount && matchingContent && matchingValue, $"Expected candidate count: {candidateCount}; Observed: {solution.RemovalCandidates.Count()}; Cell: {cell}");
            count++;
            solution = solution.Next;
        }

        static Cell GetCell(int index) => Puzzle.GetCellForIndex(index);
    }
}