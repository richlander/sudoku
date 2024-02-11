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
    private readonly int[] _solvedForRow = new int[9];
    private readonly int[] _solvedForColumn = new int[9];
    private readonly int[] _solvedForBox = new int[9];
    private readonly Box[] _boxes = new Box[9];
    private readonly int[] _cells = new int[81];
    private readonly List<int>[] _candidates = new List<int>[81];
    private readonly BoxCell[] _boxCells = new BoxCell[81];
    private readonly List<int> _empty = [];
    
    public Puzzle(string puzzle)
    {
        InitializePuzzle(puzzle);
        InitializeCellInfo();
        InitializeCandidates();
        CountCells();
    }

    /*  Terms:
        puzzle  -- the 81 cell board
        unit    -- a puzzle concept: cell, box, column, row
        index   -- used primarily to define a puzzle unit
        line    -- 9 indices or values in a box, column, or box
    */

    // Cell values
    public int this[int index] => _cells[index];

    public int GetCell(int index ) => _cells[index];

    public IEnumerable<int> GetCellValues(IEnumerable<int> line)
    {
        foreach(int index in line)
        {
            yield return _cells[index];
        }
    }

    public IEnumerable<IEnumerable<int>> GetCellValues(List<IEnumerable<int>> lines)
    {
        foreach(IEnumerable<int> line in lines)
        {
            yield return GetCellValues(line);
        }
    }
    public IEnumerable<int> GetBoxValues(int index) => GetCellValues(IndicesForBox(index));

    public IEnumerable<int> GetColumnValues(int index) => GetCellValues(IndicesForColumn(index));

    public IEnumerable<int> GetRowValues(int index) => GetCellValues(IndicesForRow(index));

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

    // Box values
    public Box GetBox(int index) => _boxes[index];

    // Solution data
    public int SolvedCellsInitial {get; private set; }
    
    public int SolvedCells {get; private set; }
    
    public int SolvedForBox(int index) => _solvedForBox[index];
    
    public int SolvedForRow(int index) => _solvedForRow[index];

    public int SolvedForColumn(int index) => _solvedForColumn[index];

    public bool IsSolved => SolvedCells is 81 && IsValid();

    public bool IsCellSolved(int index) => _cells[index] is not 0;

    // Validation
    public bool IsValid()
    {
        foreach (Cell duplicate in FindDuplicatesCells())
        {
            return false;
        }

        return true;
    }

    public IEnumerable<Cell> FindDuplicatesCells()
    {
        for (int i = 0; i < 9; i++)
        {
            foreach(var (value, index) in FindDuplicates(GetRowValues(i)))
            {
                yield return CellForRowIndex(i, index, value);
            }

            foreach(var (value, index) in FindDuplicates(GetColumnValues(i)))
            {
                yield return CellForColumnIndex(i, index, value);;
            }

            foreach(var (value, index) in FindDuplicates(GetBoxValues(i)))
            {
                yield return CellForBoxIndex(i, index, value);
            }
        }
    }

    private static IEnumerable<(int Index, int Value)> FindDuplicates(IEnumerable<int> line)
    {
        HashSet<int> values = [];
        int count = 0;

        foreach (var value in line)
        {
            if (!values.Add(value))
            {
                yield return (count, value);
            }

            count++;
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
        if (_cells[index] != 0 || SolvedCells is 81)
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

        _cells[index] = solution.Value;
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
        foreach(int num in _cells)
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
        puzzle = puzzle is  {Length: 81} ? puzzle : throw new Exception("Puzzle is wrong length");

        for (int i = 0; i < puzzle.Length; i++)
        {
            char c = puzzle[i];
            _cells[i] = c is '.' ? 0 : c - '0';
        }
    }

    private void InitializeCellInfo()
    {
        for (int i = 0; i < 9; i++)
        {
            Box box = new(i);
            _boxes[i] = box;

            for (int j = 0; j < 9; j++)
            {
                BoxCell boxCell = GetBoxCell(box, j);
                _boxCells[boxCell.Cell.Index] = boxCell;
            }
        }
    }

    public static BoxCell GetBoxCell(Box box, int boxIndex)
    {
        int puzzleIndex = IndicesByBox[box.Index * 9 + boxIndex];
        int row = RowByIndices[puzzleIndex];
        int column = ColumnByIndices[puzzleIndex];
        Cell cell = new(puzzleIndex, row, column, box.Index);
        BoxCell boxCell = new(cell, box, boxIndex, BoxRowByIndices[puzzleIndex], BoxColumnByIndices[puzzleIndex]);
        return boxCell;
    }

    private void InitializeCandidates()
    {
        for (int i = 0; i < 81; i++)
        {
            if (_cells[i] is not 0)
            {
                _candidates[i] = _empty;
                continue;
            }

            BoxCell boxCell = _boxCells[i];
            Cell cell = boxCell.Cell;
            var boxCells = GetBoxValues(boxCell.Box.Index);
            var rowCells = GetRowValues(cell.Row);
            var columnCells = GetColumnValues(cell.Column);
            _candidates[i] = Enumerable.Range(1, 9).Except(boxCells).Except(rowCells).Except(columnCells).ToList();
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
}