namespace PuzzleMultiDimensionalArray;

public record Point(int X, int Y);

public record Cell(Point Index, int Row, int Column, int Box);
