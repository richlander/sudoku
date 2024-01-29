namespace Sudoku;

public static class Row
{
    public static int GetRowForCell(int index) => index / 9;
}