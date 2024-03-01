using Microsoft.VisualStudio.TestPlatform.ObjectModel.InProcDataCollector;
using Sudoku;

namespace Tests;

public class BoxTests
{
    [Fact]
    public void BoxIndexIsCorrect()
    {
        for (int i = 0; i < 81; i++)
        {
            // Expected
            int box = PuzzleHelpers.GetBoxForCell(i);
            // Actual
            Cell cell = Puzzle.GetCellForIndex(i);

            Assert.True(cell.Box == box, $"Expected: {box}; Observed: {cell.Box}; Input: {i}");
        }
    }

    [Fact]
    public void BoxCellIndexIsCorrect()
    {
        for (int i = 0; i < 81; i++)
        {           
            // Expected
            int box = PuzzleHelpers.GetBoxForCell(i);
            // Actual
            Cell cell = Puzzle.GetCellForIndex(i);

            int boxIndex = PuzzleHelpers.GetBoxIndexforCell(i);
            Assert.True(cell.Box == box && cell.BoxIndex == boxIndex, $"Expected: {boxIndex}; Observed: {cell.BoxIndex}; Input: {i}");
        }
    }

    [Fact]
    public void BoxRowIsCorrect()
    {
        for (int i = 0; i < 81; i++)
        {           
            // Synthetic
            int box = PuzzleHelpers.GetBoxForCell(i);
            int boxIndex = PuzzleHelpers.GetBoxIndexforCell(i);
            int boxRow = boxIndex / 3;
            // Actual
            Cell cell = Puzzle.GetCellForIndex(i);

            Assert.True(cell.Box == box && cell.BoxRow == boxRow, $"Expected: {boxRow}; Observed: {cell.BoxRow}; Input: {i}");
        }
    }

    [Fact]
    public void BoxColumnIsCorrect()
    {
        for (int i = 0; i < 81; i++)
        {           
            // Expected
            int box = PuzzleHelpers.GetBoxForCell(i);
            int boxIndex = PuzzleHelpers.GetBoxIndexforCell(i);
            int boxColumn = boxIndex % 3;
            // Actual
            Cell cell = Puzzle.GetCellForIndex(i);

            Assert.True(cell.Box == box && cell.BoxColumn == boxColumn, $"Expected: {boxColumn}; Observed: {cell.BoxColumn}; Input: {i}");
        }
    }          
}
