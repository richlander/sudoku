namespace Sudoku;

public static class Cell
{
    public static int GetBoxForCell(int cell) => (cell % 26) * 3 + (cell - ((cell / 9) * 9)) % 3;

    public static int GetRowForCell(int index) => index / 9;

    public static int GetColumnForCell(int index) => index % 9;
}
