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
        int index = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                board[i, j] = puzzle[index++] - '0';
            }
        }

        return board;
    }

    public static int[,] CloneArray(int[,] originalArray)
    {
        int[,] copiedArray = new int[originalArray.GetLength(0), originalArray.GetLength(1)];
        Array.Copy(originalArray, copiedArray, originalArray.Length);
        return copiedArray;
    }

    public static int[][] GetJaggedNumberPuzzle(string puzzle)
    {
        int[][] board = new int[9][];
        int index = 0;
        for (int i = 0; i < 9; i++)
        {
            board[i] = new int[9];
            for (int j = 0; j < 9; j++)
            {
                board[i][j] = puzzle[index++] - '0';
            }
        }

        return board;
    }

    public static int[] ConvertToSingleDimensionalBoard(int[,] board)
    {
        int[] b = new int[81];
        int index = 0;
        foreach (var value in board)
        {
            b[index++] = value;
        }

        return b;
    }

    public static int[] ConvertToSingleDimensionalBoard(int[][] board)
    {
        int[] b = new int[81];
        int index = 0;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                b[index++] = board[i][j];
            }
        }

        return b;
    }
}
