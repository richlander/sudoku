namespace Sudoku;

public record struct Cell(int Index, int Row, int Column, int Box);
public record BoxCell(Cell Cell, Box Box, int Index, int Row, int Column);

public static class CellFoo
{
    public static int GetRowForCell(int index) => index / 9;

    public static int GetColumnForCell(int index) => index % 9;
}
