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

    // Validate solution
    public static void CheckSolutions(Cell cell, Solution? solution, List<Solution> expectedSolutions)
    {
        int count = 0;
        while (solution is not null)
        {
            Assert.True(count < expectedSolutions.Count, $"Expected solutions: {expectedSolutions.Count}; Observed: {count};");
            Solution expectedSolution = expectedSolutions[count];

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
    }

    // Utility
    public static string ErrorMessage => "Something wrong happended.";
}
