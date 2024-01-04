using System.Diagnostics.CodeAnalysis;

namespace Sudoku;    
    
    public interface ISolver
    {
        bool TrySolve(Puzzle puzzle, [NotNullWhen(true)] out Solution? solution);
    }