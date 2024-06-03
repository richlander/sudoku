using NuGet.Frameworks;
using Sudoku;

namespace Tests;

public class BacktrackerTests
{
    [Fact]
    public void BacktrackerOneTest()
    {
        foreach (var puzzle in PuzzleSource.Puzzles)
        {
            if (BacktrackerOne.Backtracker.Solve(puzzle.Board, out int[]? solution))
            {
                var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
                Assert.Equal(expectedSolution, solution);
            }
            else
            {
                Assert.Fail($"Puzzle was not solved: {puzzle.Description}");
            }

        }
    }

    [Fact]
    public void BacktrackerTwoTest()
    {
        foreach (var puzzle in PuzzleSource.Puzzles)
        {
            if (BacktrackerTwo.Backtracker.Solve(puzzle.Board, out int[]? solution))
            {
                var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
                Assert.Equal(expectedSolution, solution);
            }
            else
            {
                Assert.Fail($"Puzzle was not solved: {puzzle.Description}");
            }

        }
    }

    [Fact]
    public void BacktrackerThreeTest()
    {
        foreach (var puzzle in PuzzleSource.Puzzles)
        {
            if (BacktrackerThree.Backtracker.Solve(puzzle.Board, out int[]? solution))
            {
                var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
                Assert.Equal(expectedSolution, solution);
            }
            else
            {
                Assert.Fail($"Puzzle was not solved: {puzzle.Description}");
            }

        }
    }
}