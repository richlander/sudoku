using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku;    
    
public interface ISolver
{
    bool TrySolve(Puzzle puzzle, CellLocation location, [NotNullWhen(true)] out Solution? solution);
}

public record struct CellLocation(int Index, int Row, int Column, int Box);
