using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku;    
    
public interface ISolver
{
    bool TrySolve(Puzzle puzzle, Cell cell, [NotNullWhen(true)] out Solution? solution);
    string Name { get; }
}
