using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Sudoku;
public partial class Puzzle
{
    private readonly int[] _board = new int[81];
    private readonly Cell[] _cells = new Cell[81];
    private readonly List<int>[] _candidates = new List<int>[81];
    private readonly Box[] _boxes = new Box[9];
    
    public Puzzle(string puzzle)
    {
        InitializePuzzle(puzzle);
    }

    /*  Terms:
        puzzle  -- the 81 cell board and associated functionality
        cell    -- single cell on the board
        unit    -- a 9 cell grouping: box, column, row
        line    -- 9 one-dimentional indices or values in a box, column, or box
    */

    // Board values
    public int this[int index] => _board[index];

    public int GetValue(int index ) => _board[index];

    public IEnumerable<int> GetCellValues(IEnumerable<int> line)
    {
        foreach(int index in line)
        {
            yield return _board[index];
        }
    }

    public IEnumerable<int> GetBoxValues(int index) => GetCellValues(GetBoxIndices(index));

    public IEnumerable<int> GetColumnValues(int index) => GetCellValues(GetColumnIndices(index));

    public IEnumerable<int> GetRowValues(int index) => GetCellValues(GetRowIndices(index));

    // Cell candidates
    public IReadOnlyList<int> GetCellCandidates(int index) => _candidates[index];

    // Unit info
    public Cell GetCell(int index) => _cells[index];
    
    public Box GetBox(int index) => _boxes[index];

    // Solution data  
    public int SolvedCellsInitial {get; private set; }

    public int SolvedCells {get; private set; }
    
    public bool IsSolved => SolvedCells is 81 && IsValid;

    public bool IsCellSolved(int index) => _board[index] is not 0;

    // Validation
    public bool IsValid => FindAnyDuplicateCells();

    private bool FindAnyDuplicateCells()
    {
        foreach (var _ in FindDuplicatesCells())
        {
            return false;
        }

        return true;
    }

    // The intent is that this API provides sufficient information to identify problematic cells
    public IEnumerable<int> FindDuplicatesCells()
    {
        for (int i = 0; i < 9; i++)
        {
            foreach(int index in FindDuplicates(GetRowIndices(i)))
            {
                yield return index;
            }

            foreach(int index in FindDuplicates(GetColumnIndices(i)))
            {
                yield return index;
            }

            foreach(int index in FindDuplicates(GetBoxIndices(i)))
            {
                yield return index;
            }
        }
    }

    private IEnumerable<int> FindDuplicates(IEnumerable<int> line)
    {
        HashSet<int> values = [];

        foreach (var index in line)
        {
            int value = GetValue(index);
            if (value is 0)
            {
                continue;
            }
            
            if (!values.Add(value))
            {
                yield return index;
            }
        }
    }

    // Initialize puzzle
    private void InitializePuzzle(string puzzle)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(puzzle);
        puzzle = puzzle is {Length: 81} ? puzzle : throw new Exception("Puzzle is wrong length");

        // Layout board by itself
        for (int i = 0; i < puzzle.Length; i++)
        {
            // Initialize board
            char c = puzzle[i];
            int value = c is '.' ? 0 : c - '0';
            _board[i] = value;
            SolvedCells += value > 0 ? 1 : 0;
        }

        SolvedCellsInitial = SolvedCells;

        // Things that are cell-specific
        for (int i = 0; i < puzzle.Length; i++)
        {
            // Initialize cell data
            Cell cell = GetCellForIndex(i);
            _cells[i] = cell;

            // Initialize initial candidates
            if (_board[i] is 0)
            {
                _candidates[i] = Enumerable.Range(1, 9).ToList();
                UpdateCandidate(cell);
            }
            else
            {
                _candidates[i] = [];
            }
        }

        // Things that are unit-specific
        for (int i = 0; i < 9; i++)
        {
            _boxes[i] = new Box(i);
        }
    }

    // Updating puzzle
    public bool UpdateCell(Solution solution)
    {
        Cell cell = solution.Cell;
        if (_board[cell] != 0 || SolvedCells is 81)
        {
            throw new Exception("Something went wrong! Oops.");    
        }

        if (solution.Value is -1)
        {
            List<int> candidates = _candidates[cell];
            foreach (int value in solution.Removed)
            {
                candidates.Remove(value);
            }

            return false;
        }

        _board[cell] = solution.Value;
        SolvedCells++;
        _candidates[cell] = [];

        // `true` indicates that a new board value was found
        // This bool is intended to avoid the need for readers to peek into `solution`
        return true;
    }

    public void UpdateBoard(Solution solution)
    {
        Solution? nextSolution = solution;
        while (nextSolution is not null)
        {
            UpdateCell(nextSolution);
            nextSolution = nextSolution.Next;
        }

        UpdateCandidates();
    }

    public void UpdateCandidates()
    {
        for (int i = 0; i < 81; i++)
        {
            UpdateCandidate(GetCell(i));
        }
    }

    public void UpdateCandidate(Cell cell)
    {
        if (_board[cell] is not 0)
        {
            return;
        }

        var boxValues = GetBoxValues(cell.Box);
        var rowValues = GetRowValues(cell.Row);
        var columnValues = GetColumnValues(cell.Column);
        List<int> cellCandidates = _candidates[cell];
        List<IEnumerable<int>> lines = [boxValues, rowValues, columnValues];

        foreach (IEnumerable<int> line in lines)
        {
            foreach (int value in line)
            {
                if (value is 0)
                {
                    continue;
                }

                cellCandidates.Remove(value);
            }
        }
    }

    // Visualizing
    public override string ToString()
    {
        var buffer = new StringBuilder();
        foreach (int num in _board)
        {
            char cell = num is 0 ? '.' : (char)('0' + num);
            buffer.Append(cell);
        }

        return buffer.ToString();
    }
}
