namespace Sudoku;

public record Cell(int Index, int Row, int Column, int Box)
{
    public static implicit operator int(Cell c) => c.Index;
};
