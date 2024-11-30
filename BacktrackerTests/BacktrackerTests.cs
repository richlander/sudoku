using Sudoku;

namespace Tests;

public class BacktrackerTests
{
    public static TheoryData<SudokuPuzzle> Puzzles => [..PuzzleSource.Puzzles];

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerOneTest(SudokuPuzzle puzzle)
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
    public void BacktrackerTwoTest(SudokuPuzzle puzzle)
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
    public void BacktrackerThreeTest(SudokuPuzzle puzzle)
    {
        var result = BacktrackerThree.Backtracker.Solve(puzzle.Board, out int[]? solution);
        Assert.True(result, $"Puzzle was not solved: {puzzle.Description}");
        var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
        Assert.Equal(expectedSolution, solution);
    }

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerFourTest(SudokuPuzzle puzzle)
    {
        var result = BacktrackerFour.Backtracker.Solve(puzzle.Board, out int[]? solution);
        Assert.True(result, $"Puzzle was not solved: {puzzle.Description}");
        var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
        Assert.Equal(expectedSolution, solution);
    }

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerFiveTest(SudokuPuzzle puzzle)
    {
        if (BacktrackerFive.Backtracker.Solve(puzzle.MultiDimensionalBoard))
        {
            var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
            var actualSolution = Utils.Utils.ConvertToSingleDimensionalBoard(puzzle.MultiDimensionalBoard);
            Assert.Equal(expectedSolution, actualSolution);
        }
        else
        {
            Assert.Fail($"Puzzle was not solved: {puzzle.Description}");
        }
    }

    [Theory, MemberData(nameof(Puzzles))]
    public void BacktrackerSixTest(SudokuPuzzle puzzle)
    {
        if (BacktrackerSix.Backtracker.Solve(puzzle.JaggedArrayBoard))
        {
            var expectedSolution = Utils.Utils.GetNumberPuzzle(puzzle.Solution);
            var actualSolution = Utils.Utils.ConvertToSingleDimensionalBoard(puzzle.JaggedArrayBoard);
            Assert.Equal(expectedSolution, actualSolution);
        }
        else
        {
            Assert.Fail($"Puzzle was not solved: {puzzle.Description}");
        }
    }
}