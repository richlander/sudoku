using Sudoku;

namespace Tests;

public class BacktrackerTests
{
    public static TheoryData<SudokuPuzzle> Puzzles => [..PuzzleSource.Puzzles];

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerAOneTest(SudokuPuzzle puzzle)
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

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerBTwoTest(SudokuPuzzle puzzle)
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

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerCThreeTest(SudokuPuzzle puzzle)
    {
        var result = BacktrackerThree.Backtracker.Solve(puzzle.Board, out int[]? solution);
        Assert.True(result, $"Puzzle was not solved: {puzzle.Description}");
        var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
        Assert.Equal(expectedSolution, solution);
    }

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerDFourTest(SudokuPuzzle puzzle)
    {
        var result = BacktrackerFour.Backtracker.Solve(puzzle.Board, out int[]? solution);
        Assert.True(result, $"Puzzle was not solved: {puzzle.Description}");
        var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
        Assert.Equal(expectedSolution, solution);
    }

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerEFiveTest(SudokuPuzzle puzzle)
    {
        var result = BacktrackerFive.Backtracker.Solve(puzzle.Board, out int[,]? solution);
        Assert.True(result, $"Puzzle was not solved: {puzzle.Description}");
        if (result)
        {
            var actualSolution = Utils.Utils.ConvertToSingleDimensionalBoard(solution!);
            Assert.Equal(puzzle.NumberSolution, actualSolution);
        }

    }

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerFSixTest(SudokuPuzzle puzzle)
    {
        if (BacktrackerSix.Backtracker.Solve(puzzle.Board, out int[][]? solution))
        {
            var actualSolution = Utils.Utils.ConvertToSingleDimensionalBoard(solution);
            Assert.Equal(puzzle.NumberSolution, actualSolution);
        }
        else
        {
            Assert.Fail($"Puzzle was not solved: {puzzle.Description}");
        }
    }

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerGSevenTest(SudokuPuzzle puzzle)
    {
        if (BacktrackerSeven.Backtracker.Solve(puzzle.Board, out int[][]? solution))
        {
            var actualSolution = Utils.Utils.ConvertToSingleDimensionalBoard(solution);
            Assert.Equal(puzzle.NumberSolution, actualSolution);
        }
        else
        {
            Assert.Fail($"Puzzle was not solved: {puzzle.Description}");
        }
    }

}