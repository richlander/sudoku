namespace Sudoku;

public record Solution(Cell Cell, int Value, HashSet<int> Removed, string Solver);
