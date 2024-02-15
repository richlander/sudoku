namespace Sudoku;

public record Solution(Cell Cell, int Value, IEnumerable<int> Removed, string Solver, Solution? Next = null);
