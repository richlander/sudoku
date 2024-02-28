namespace Sudoku;

public record Solution(Cell Cell, int Value, string Solver)
{
    // Candidates to remove
    public IEnumerable<int>? RemovalCandidates { get; set; }

    public IEnumerable<int>? AlignedCandidates { get; set; }

    public IEnumerable<int>? AlignedIndices { get; set; }

    public Solution? Next { get; set; }
};
