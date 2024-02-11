namespace Sudoku;

public record struct Cell(int Index, int Row, int Column, int Box);
public record BoxCell(Cell Cell, Box Box, int Index, int Row, int Column);
