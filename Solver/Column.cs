namespace Sudoku;

public static class Column
{
    public static int GetColumnForCell(int index) => index % 9;
}
