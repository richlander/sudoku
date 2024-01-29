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
    private readonly BoxCell[] _boxCells = new BoxCell[81];
    private readonly List<int> _empty = [];
    
    public Puzzle(string puzzle)
    {
        InitializePuzzle(puzzle);
        InitializeCellInfo();
        InitializeCandidates();
        CountCells();
    }

    public List<int> Cells = new(81);

    public BoxCell BoxCells(int index) => _boxCells[index];
    public Dictionary<int, List<int>> Candidates { get; } = new(81);

    public int this[int index] => Cells[index];

    public int SolvedCellsInitial {get; private set; }
    public int SolvedCells {get; private set; }
    public int SolvedForBox(int index) => _solvedForBox[index];
    public int SolvedForRow(int index) => _solvedForRow[index];
    public int SolvedForColumn(int index) => _solvedForColumn[index];

    public bool IsSolved => SolvedCells is 81 && IsValid();

    public IEnumerable<int> GetCellsForRow(int index) => Cells.Skip(index * 9).Take(9);

    public IEnumerable<int> GetCellsForColumn(int index) => GetColumnCells(index);

    public IEnumerable<int> GetCellIndicesForColumn(int index)
    {
        for (int i = 0; i < 9; i++)
        {
            yield return (i * 9) + index;
        }
    }

    public IEnumerable<int> GetCellsForBox(int index) => GetBoxCells(index);

    public Box GetBox(int index) => _boxes[index];

    public IEnumerable<int> GetCellsForNeighboringRow(int box, int row)
    {
        Box box0 = _boxes[box];
        Box box1 = _boxes[box0.FirstHorizontalNeighbor];
        Box box2 = _boxes[box0.SecondHorizontalNeighbor];

        int offset = (row % 3) * 3;

        for (int i = 0; i < 3; i++)
        {
            yield return this[box1.CellsForCells[offset + i]];
        }

        for (int i = 0; i < 3; i++)
        {
            yield return this[box2.CellsForCells[offset + i]];
        }
    }

    public IEnumerable<int> GetCellsForNeighboringColumn(int box, int column)
    {
        Box box0 = _boxes[box];
        Box box1 = _boxes[box0.FirstVerticalNeighbor];
        Box box2 = _boxes[box0.SecondVerticalNeighbor];

        // Start of column
        int offset = column % 3;

        for (int i = 0; i < 3; i++)
        {
            yield return this[box1.CellsForCells[offset + i * 3]];
        }

        for (int i = 0; i < 3; i++)
        {
            yield return this[box2.CellsForCells[offset + i * 3]];
        }
    }

    public bool IsValid()
    {
        foreach (var duplicate in FindDuplicatesCells())
        {
            return false;
        }

        return true;
    }

    public bool IsCellSolved(int index) => Cells[index] is not 0;

    public IEnumerable<Cell> FindDuplicatesCells()
    {
        for (int i = 0; i < 9; i++)
        {
            foreach(var (value, index) in FindDuplicates(GetCellsForRow(i)))
            {
                yield return GetCellForRowIndex(i, index, value);
            }

            foreach(var (value, index) in FindDuplicates(GetColumnCells(i)))
            {
                yield return GetCellForColumnIndex(i, index, value);;
            }

            foreach(var (value, index) in FindDuplicates(GetBoxCells(i)))
            {
                yield return GetCellForBoxIndex(i, index, value);
            }
        }
    }

    public void Update(Solution solution)
    {
        int index = solution.Cell.Index;
        if (Cells[index] != 0 || SolvedCells is 81)
        {
            throw new Exception("Something went wrong! Oops.");    
        }

        if (solution.Value is -1)
        {
            List<int> candidates = Candidates[index];
            foreach (int value in solution.Removed)
            {
                candidates.Remove(value);
            }

            return;
        }

        Cells[index] = solution.Value;
        SolvedCells++;
        _solvedForRow[solution.Cell.Row]++;
        _solvedForColumn[solution.Cell.Column]++;
        _solvedForBox[solution.Cell.Box]++;
        Candidates[index] = _empty;
    }

    private IEnumerable<int> GetColumnCells(int index)
    {
        foreach(int cell in GetCellIndicesForColumn(index))
        {
            yield return Cells[cell];
        }
    }

    private IEnumerable<int> GetBoxCells(int index)
    {
        int offset = _boxes[index].FirstCell;

        for (int i = 0; i < 3; i++)
        {
            int cell = offset + i * 9;
            yield return Cells[cell];
            yield return Cells[cell + 1];
            yield return Cells[cell + 2];
        }
    }

    public static BoxCell GetBoxCell(Box box, int boxIndex)
    {
        int puzzleIndex = Box.GetPuzzleIndexForBoxCell2(box.Index, boxIndex);
        int row = CellFoo.GetRowForCell(puzzleIndex);
        int column = CellFoo.GetColumnForCell(puzzleIndex);
        Cell cell = new(puzzleIndex, row, column, box.Index);
        BoxCell boxCell = new(cell, box, boxIndex, Box.GetRowForCell(boxIndex), Box.GetColumnForCell(boxIndex));
        return boxCell;
    }

    private void CountCells()
    {
        for (int i = 0; i < 9; i++)
        {
            _solvedForRow[i]+= Count(GetCellsForRow(i));
            _solvedForColumn[i]+= Count(GetColumnCells(i));
            _solvedForBox[i]+= Count(GetBoxCells(i));
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

    public override string ToString()
    {
        var buffer = new StringBuilder();
        foreach(int num in Cells)
        {
            char cell = num is 0 ? '.' : (char)('0' + num);
            buffer.Append(cell);
        }
        return buffer.ToString();
    }

    private static IEnumerable<(int Index, int Value)> FindDuplicates(IEnumerable<int> values)
    {
        HashSet<int> ints = [];
        int count = 0;

        foreach (var value in values)
        {
            if (!ints.Add(value))
            {
                yield return (count, value);
            }

            count++;
        }
    }

    private void InitializePuzzle(string puzzle)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(puzzle);
        puzzle = puzzle is  {Length: 81} ? puzzle : throw new Exception("Puzzle is wrong length");

        List<int> cells = [];

        foreach (var c in puzzle)
        {
            int num = c is '.' ? 0 : c - '0';
            Cells.Add(num);
        }
    }

    private void InitializeCellInfo()
    {
        for (int i = 0; i < 9; i++)
        {
            Box box = new Box(this, i);
            _boxes[i] = box;

            for (int j = 0; j < 9; j++)
            {
                BoxCell boxCell = GetBoxCell(box, j);
                _boxCells[boxCell.Cell.Index] = boxCell;
            }
        }
    }

    private void InitializeCandidates()
    {
        for (int i = 0; i < 81; i++)
        {
            if (Cells[i] is not 0)
            {
                Candidates[i] = _empty;
                continue;
            }

            BoxCell boxCell = _boxCells[i];
            Cell cell = boxCell.Cell;
            var boxCells = GetCellsForBox(boxCell.Box.Index);
            var rowCells = GetCellsForRow(cell.Row);
            var columnCells = GetCellsForColumn(cell.Column);
            Candidates[i] = Enumerable.Range(1, 9).Except(boxCells).Except(rowCells).Except(columnCells).ToList();
        } 
    }
}