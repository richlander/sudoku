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
        new PointingPairsSolver(),
        new BoxLineReductionSolver(),
        new XWingSolver()];

    public static List<ISolver> Solvers => solvers;

    // Validate solution
    public static void CheckSolutions(Puzzle puzzle, IEnumerable<Solution> solutions, List<Solution> expectedSolutions)
    {
        foreach (Solution solution in solutions)
        {
            int count = CheckSolutions(puzzle, solution, expectedSolutions);

            if (count < expectedSolutions.Count)
            {
                expectedSolutions.RemoveRange(0, count);
            }
            else
            {
                return;
            }
        }
    }

    public static int CheckSolutions(Puzzle puzzle, Solution solutions, List<Solution> expectedSolutions)
    {
        puzzle.UpdateBoard(solutions);
        int count = 0;
        IEnumerator<Solution> expected = expectedSolutions.GetEnumerator();
        foreach (Solution solution in Solution.Enumerate(solutions))
        {
            if (!expected.MoveNext())
            {
                return count;
            }

            Solution expectedSolution = expected.Current;
            if (solution.RemovalCandidates is null || expectedSolution.RemovalCandidates is null)
            {
                Assert.Fail(PuzzleHelpers.ErrorMessage);
                return -1;
            }

            CheckSolution(solution, expectedSolution);
            count++;
        }

        return count;
    }

    public static void CheckSolution(Solution solution, Solution expectedSolution)
    {

        if (solution.RemovalCandidates is null || expectedSolution.RemovalCandidates is null)
        {
            Assert.Fail("Expected `RemovalCandidates` to be non-null");
            return;
        }

        int candidateCount = expectedSolution.RemovalCandidates.Count();
        bool matchingCount = solution.RemovalCandidates.Count() == candidateCount;
        bool matchingContent = solution.RemovalCandidates.Intersect(expectedSolution.RemovalCandidates).Count() == candidateCount;
        bool matchingValue = solution.Value == expectedSolution.Value;
        bool matching = matchingCount && matchingContent && matchingValue;
        Assert.True(matching, $"Expected: {expectedSolution}; Observed: {solution}");
    }


    // Utility
    public static string ErrorMessage => "Something wrong happended.";
}
