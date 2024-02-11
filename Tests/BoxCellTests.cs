using Microsoft.VisualStudio.TestPlatform.ObjectModel.InProcDataCollector;
using Sudoku;

namespace Tests;

public class BoxCellTests
{
    [Fact]
    public void BoxCellIsCorrect()
    {
        int[] indices = PuzzleData.BoxIndices;

        for (int i = 0; i < 81; i++)
        {
            int boxIndex = Puzzle.BoxByIndices[i];
            Box box = PuzzleData.Puzzle.GetBox(boxIndex);
            Cell cell = Puzzle.GetCellForIndex(i);
            Assert.True(cell.Index == i, $"Expected: {i}; Observed: {cell.Index}; Input: {i}");
        }
    }
}