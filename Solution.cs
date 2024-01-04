namespace Sudoku;

public record Solution(int Row, int Column, int Value, string Solver)
{
    public string GetLocation() => $"r{Row+1}:c{Column+1}";

    public static Solution Empty => new(0, 0, 0, ""); 
};

public record Location(int Row, int Column, int Value);
