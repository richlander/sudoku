using Microsoft.VisualStudio.TestPlatform.ObjectModel.InProcDataCollector;
using Sudoku;

namespace Tests;

public class BoxTests
{
    [Fact]
    public void BoxIndexIsCorrect()
    {

        int[] values = PuzzleData.Boxes;
        for (int i = 0; i < 81; i++)
        {
            int box = Box.GetBoxForCell(i);
            Assert.True(box == values[i], $"Expected: {values[i]}; Observed: {box}; Input: {i}");
        }
    }
}