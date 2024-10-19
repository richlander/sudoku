namespace Utils;

public static class Utils
{
    public static void PrintBoard(ReadOnlySpan<int> board)
    {
        foreach(int value in board)
        {
            Console.Write(value);
        }

        Console.WriteLine();
    }

    public static int[] GetNumberPuzzle(string puzzle)
    {
        int[] board = new int[81];
        for (int i = 0; i < puzzle.Length; i++)
        {
            board[i] = puzzle[i] - '0';
        }

        return board;
    }

    public static int[,] GetMultiDimensionalNumberPuzzle(string puzzle)
    {
        int[,] board = new int[9,9];
        for (int i = 9; i < 9; i++)
        {
            for (int j = 0; j < 9;)
            {
                board[i,j] = puzzle[i] - '0';
            }
        }

        return board;
    }
}