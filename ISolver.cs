using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku;    
    
public interface ISolver
{
    bool TrySolve(Puzzle puzzle, BoxCell cell, [NotNullWhen(true)] out Solution? solution);
}
