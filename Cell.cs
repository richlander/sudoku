namespace Sudoku;

public static class Cell
{
    public static int GetRowForCell(int cell) => cell / 9;

    public static int GetColumnForCell(int cell) => cell % 9;

}