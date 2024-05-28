public class PuzzleSource
{
    public static List<string> Puzzles =
    [
        "003020600900305001001806400008102900700000008006708200002609500800203009005010300",
        // "000041000060000200000000000320600000000050041700000000000200300048000000501000000"
    ];

    public static IEnumerable<int[]> GetPuzzles()
    {
        foreach (string puzzle in Puzzles)
        {
            yield return Utils.GetNumberPuzzle(puzzle);
        }
    }
}
