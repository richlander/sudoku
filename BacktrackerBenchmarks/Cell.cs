namespace Sudoku;

public record Cell(int Index, int Row, int Column, int Box, int BoxRow, int BoxColumn, int BoxIndex)
{
    public static implicit operator int(Cell c) => c.Index;
};
