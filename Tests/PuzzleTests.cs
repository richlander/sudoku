using System.Net;
using Sudoku;

namespace Tests;

public class PuzzleTests
{
    [Fact]
    public void PuzzleIndexIsCorrect()
    {
        for (int i = 0; i < 81; i++)
        {
            // Expected
            int index = i;
            // Actual
            Cell cell = Puzzle.GetCellForIndex(i);

            Assert.True(cell == i, $"Expected: {index}; Observed: {cell.Index}; Input: {i}");
        }
    }

    [Fact]
    public void PuzzleRowIsCorrect()
    {
        for (int i = 0; i < 81; i++)
        {
            // Expected
            int row = i / 9;
            // Actual
            Cell cell = Puzzle.GetCellForIndex(i);

            Assert.True(cell.Row == row, $"Expected: {row}; Observed: {cell.Row}; Input: {i}");
        }
    }

    [Fact]
    public void PuzzleColumnIsCorrect()
    {
        for (int i = 0; i < 81; i++)
        {
            // Expected
            int column = i % 9;
            // Actual
            Cell cell = Puzzle.GetCellForIndex(i);

            Assert.True(cell.Column == column, $"Expected: {column}; Observed: {cell.Column}; Input: {i}");
        }
    }
}
