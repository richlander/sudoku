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
    private readonly int[] _solvedForRow = new int[9];
    private readonly int[] _solvedForColumn = new int[9];
    private readonly int[] _solvedForBox = new int[9];
    private readonly Box[] _boxes = new Box[9];
    private readonly Cell[] _cells = new Cell[81];
    private readonly List<int>[] _candidates = new List<int>[81];
    private readonly List<int> _empty = [];
    
    public Puzzle(string puzzle)
    {
        InitializePuzzle(puzzle);
        IntializeBoxes();
        InitializeCells();
        CountCells();
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
    public IReadOnlyList<int> GetCandidates(int index) => _candidates[index];

    public IEnumerable<List<int>> GetCellCandidates(IEnumerable<int> line)
    {
        foreach (int index in line)
        {
            yield return _candidates[index];
        }
    }

    public IEnumerable<IEnumerable<List<int>>> GetCellCandidates(List<IEnumerable<int>> lines)
    {
        foreach (IEnumerable<int> row in lines)
        {
            yield return GetCellCandidates(row);
        }
    }

    // Unit info
    public Box GetBox(int index) => _boxes[index];

    public Cell GetCell(int index) => _cells[index];

    public (int rowIndex, int columnIndex) GetBoxCellInfo(int index) => (BoxRowByIndices[index], BoxColumnByIndices[index]);

    // Solution data
    public int SolvedCellsInitial {get; private set; }
    
    public int SolvedCells {get; private set; }
    
    public int SolvedForBox(int index) => _solvedForBox[index];
    
    public int SolvedForRow(int index) => _solvedForRow[index];

    public int SolvedForColumn(int index) => _solvedForColumn[index];

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

    private void CountCells()
    {
        for (int i = 0; i < 9; i++)
        {
            _solvedForRow[i]+= Count(GetRowValues(i));
            _solvedForColumn[i]+= Count(GetColumnValues(i));
            _solvedForBox[i]+= Count(GetBoxValues(i));
            SolvedCells += _solvedForRow[i];
        }
    }

    private int Count(IEnumerable<int> group)
    {
        int count = 0;

        foreach(var cell in group)
        {
            if (cell > 0)
            {
                count++;
            }
        }

        return count;
    }

    // Updating puzzle
    public void Update(Solution solution)
    {
        int index = solution.Cell.Index;
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

            return;
        }

        _board[index] = solution.Value;
        SolvedCells++;
        _solvedForRow[solution.Cell.Row]++;
        _solvedForColumn[solution.Cell.Column]++;
        _solvedForBox[solution.Cell.Box]++;
        _candidates[index] = _empty;
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

    // Initialize puzzle
    private void InitializePuzzle(string puzzle)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(puzzle);
        puzzle = puzzle is {Length: 81} ? puzzle : throw new Exception("Puzzle is wrong length");

        for (int i = 0; i < puzzle.Length; i++)
        {
            char c = puzzle[i];
            _board[i] = c is '.' ? 0 : c - '0';
        }
    }

    private void InitializeCells()
    {
        for (int i = 0; i < 81; i++)
        {
            Cell cell = GetCellForIndex(i);
            _cells[i] = cell;

            InitializeCandidates(cell);
        }
    }

    private void IntializeBoxes()
    {
        for (int i = 0; i < 9; i++)
        {
            _boxes[i] = new Box(i);
        }
    }

    private void InitializeCandidates(Cell cell)
    {
        int index = cell.Index;

        if (_board[cell.Index] is not 0)
        {
            _candidates[index] = _empty;
            return;
        }

        var boxValues = GetBoxValues(cell.Box);
        var rowValues = GetRowValues(cell.Row);
        var columnValues = GetColumnValues(cell.Column);
        _candidates[index] = Enumerable.Range(1, 9).Except(boxValues).Except(rowValues).Except(columnValues).ToList();
    } 

    // public IEnumerable<int> GetCellsForRow(int index) => Cells.Skip(index * 9).Take(9);

    // public IEnumerable<int> GetCellsForColumn(int index) => GetColumnCells(index);

    // public IEnumerable<int> GetCellIndicesForColumn(int index)
    // {
    //     for (int i = 0; i < 9; i++)
    //     {
    //         yield return (i * 9) + index;
    //     }
    // }

    // public IEnumerable<int> GetCellsForBox(int index) => GetBoxCells(index);


    // public IEnumerable<int> GetCellsForNeighboringRow(int box, int row)
    // {
    //     Box box0 = _boxes[box];
    //     Box box1 = _boxes[box0.FirstHorizontalNeighbor];
    //     Box box2 = _boxes[box0.SecondHorizontalNeighbor];

    //     int offset = (row % 3) * 3;

    //     for (int i = 0; i < 3; i++)
    //     {
    //         yield return this[box1.CellsForCells[offset + i]];
    //     }

    //     for (int i = 0; i < 3; i++)
    //     {
    //         yield return this[box2.CellsForCells[offset + i]];
    //     }
    // }

    // public IEnumerable<int> GetCellsForNeighboringColumn(int box, int column)
    // {
    //     Box box0 = _boxes[box];
    //     Box box1 = _boxes[box0.FirstVerticalNeighbor];
    //     Box box2 = _boxes[box0.SecondVerticalNeighbor];

    //     // Start of column
    //     int offset = column % 3;

    //     for (int i = 0; i < 3; i++)
    //     {
    //         yield return this[box1.CellsForCells[offset + i * 3]];
    //     }

    //     for (int i = 0; i < 3; i++)
    //     {
    //         yield return this[box2.CellsForCells[offset + i * 3]];
    //     }
    // }



    


    

    // private IEnumerable<int> GetColumnCells(int index)
    // {
    //     foreach(int cell in GetCellIndicesForColumn(index))
    //     {
    //         yield return Cells[cell];
    //     }
    // }


    // private IEnumerable<int> GetBoxCells(int index)
    // {
    //     int offset = _boxes[index].FirstCell;

    //     for (int i = 0; i < 3; i++)
    //     {
    //         int cell = offset + i * 9;
    //         yield return Cells[cell];
    //         yield return Cells[cell + 1];
    //         yield return Cells[cell + 2];
    //     }
    // }

    // public static BoxCell GetBoxCell(Box box, int boxIndex)
    // {
    //     int puzzleIndex = Box.GetPuzzleIndexForBoxCell2(box.Index, boxIndex);
    //     int row = CellFoo.GetRowForCell(puzzleIndex);
    //     int column = CellFoo.GetColumnForCell(puzzleIndex);
    //     Cell cell = new(puzzleIndex, row, column, box.Index);
    //     BoxCell boxCell = new(cell, box, boxIndex, Box.GetRowForCell(boxIndex), Box.GetColumnForCell(boxIndex));
    //     return boxCell;
    // }
}