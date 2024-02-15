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
    private readonly Box[] _boxes = new Box[9];
    private readonly Cell[] _cells = new Cell[81];
    private readonly List<int>[] _candidates = new List<int>[81];
    private readonly List<int> _emptyList = [];
    
    public Puzzle(string puzzle)
    {
        InitializePuzzle(puzzle);
    }

    /*  Terms:
        puzzle  -- the 81 cell board
        unit    -- a puzzle concept: cell, box, column, row
        index   -- used primarily to define a puzzle unit
        line    -- 9 indices or values in a box, column, or box
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

    public IEnumerable<IEnumerable<int>> GetCellValues(List<IEnumerable<int>> lines)
    {
        foreach(IEnumerable<int> line in lines)
        {
            yield return GetCellValues(line);
        }
    }
    public IEnumerable<int> GetBoxValues(int index) => GetCellValues(GetBoxIndices(index));

    public IEnumerable<int> GetColumnValues(int index) => GetCellValues(GetColumnIndices(index));

    public IEnumerable<int> GetRowValues(int index) => GetCellValues(GetRowIndices(index));

    // Cell candidates
    public IReadOnlyList<int> GetCellCandidates(int index) => _candidates[index];

    public IEnumerable<List<int>> GetCellCandidates(IEnumerable<int> line)
    {
        foreach (int index in line)
        {
            yield return _candidates[index];
        }
    }

    public IEnumerable<IEnumerable<List<int>>> GetCellCandidates(IEnumerable<IEnumerable<int>> lines)
    {
        foreach (IEnumerable<int> row in lines)
        {
            yield return GetCellCandidates(row);
        }
    }

    // Unit info
    public Cell GetCell(int index) => _cells[index];
    
    public Box GetBox(int index) => _boxes[index];

    public BoxSet GetBoxSet(int index)
    {
        // Get boxes
        Box box = GetBox(index);
        // get adjacent neighboring boxes
        Box ahnb1 = GetBox(box.FirstHorizontalNeighbor);
        Box ahnb2 = GetBox(box.SecondHorizontalNeighbor);
        Box avnb1 = GetBox(box.FirstVerticalNeighbor);
        Box avnb2 = GetBox(box.SecondVerticalNeighbor);

        return new BoxSet(
            box,
            ahnb1,
            ahnb2,
            avnb1,
            avnb2);
    }

    // Solution data  
    public int SolvedCells {get; private set; }
    
    public bool IsSolved => SolvedCells is 81 && IsValid();

    public bool IsCellSolved(int index) => _board[index] is not 0;

    // Validation
    public bool IsValid()
    {
        foreach (var _ in FindDuplicatesCells())
        {
            return false;
        }

        return true;
    }

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
            _board[i] = c is '.' ? 0 : c - '0';
        }

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
                _candidates[i] = _emptyList;
            }
        }

        // Things that are unit-specific
        for (int i = 0; i < 9; i++)
        {
            _boxes[i] = new Box(i);
            SolvedCells = GetRowValues(i).Count(x => x > 0);
        }
    }

    // Updating puzzle
    public bool Update(Solution solution)
    {
        int index = solution.Cell;
        Cell cell = solution.Cell;
        if (_board[index] != 0 || SolvedCells is 81)
        {
            throw new Exception("Something went wrong! Oops.");    
        }

        if (solution.Value is -1)
        {
            List<int> candidates = _candidates[index];
            foreach (int value in solution.Removed)
            {
                candidates.Remove(value);
            }

            return false;
        }

        _board[index] = solution.Value;
        SolvedCells++;
        _candidates[index] = _emptyList;

        // `true` indicators that a new board value was found
        return true;
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
