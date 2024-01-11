using Sudoku;

namespace Tests;

public class PuzzleTests
{
    [Fact]
    public void PuzzleIndexIsCorrect()
    {
        int[] values = PuzzleData.Boxes;
        int[] indices = PuzzleData.BoxIndices;
        for (int i = 0; i < 81; i++)
        {
            int index = Box.GetPuzzleIndexForBoxCell2(values[i], indices[i]);
            Assert.True(index == i, $"Expected: {i}; Observed: {index}; Input: {i}");
        }
    }
}