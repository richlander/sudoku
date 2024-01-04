using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Linq.Expressions;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Sudoku;
public class Puzzle
{
    private readonly int[] _solvedForRow = new int[9];
    private readonly int[] _solvedForColumn = new int[9];
    private readonly int[] _solvedForBox = new int[9];
    private readonly Box[] _boxes = new Box[9];
    
    public Puzzle(string puzzle)
    {
        InitializePuzzle(puzzle);
        InitializeBoxes();
        InitializeCandidates();
        CountCells();
    }

    public List<int> Cells = new(81);
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

    public IEnumerable<int> GetCellsForBox(int index) => GetBoxCells(index);

    public Box GetBox(int index) => _boxes[index];

    public IEnumerable<int> GetCellsForNeighboringRow(int box, int cell)
    {
        Box box0 = _boxes[box];
        Box box1 = _boxes[box0.FirstHorizontalNeighbor];
        Box box2 = _boxes[box0.SecondHorizontalNeighbor];

        int offset = cell / 3 * 3;

        for (int i = 0; i < 3; i++)
        {
            yield return this[box1.CellsForCells[offset + i]];
        }

        for (int i = 0; i < 3; i++)
        {
            yield return this[box2.CellsForCells[offset + i]];
        }
    }

    public IEnumerable<int> GetCellsForNeighboringColumn(int box, int cell)
    {
        Box box0 = _boxes[box];
        Box box1 = _boxes[box0.FirstVerticalNeighbor];
        Box box2 = _boxes[box0.SecondVerticalNeighbor];

        int offset = cell % 3;

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

    public IEnumerable<Location> FindDuplicatesCells()
    {
        for (int i = 0; i < 9; i++)
        {
            foreach(var duplicate in FindDuplicates(GetCellsForRow(i), i))
            {
                yield return duplicate;
            }

            foreach(var duplicate in FindDuplicates(GetColumnCells(i), i))
            {
                yield return duplicate;
            }

            foreach(var duplicate in FindDuplicates(GetBoxCells(i), i))
            {
                yield return duplicate;
            }
        }
    }

    public bool Update(Solution solution)
    {
        int row = solution.Row;
        int column = solution.Column;
        int value = solution.Value;
        var index = row * 9 + column;

        if (Cells[index] != 0 || SolvedCells is 81)
        {
            throw new Exception("Something went wrong! Oops.");    
        }

        Cells[index] = value;
        SolvedCells++;
        _solvedForRow[row]++;
        _solvedForColumn[column]++;
        int box = Sudoku.Box.GetIndex(row, column);
        _solvedForBox[box]++;

        return true;
    }

    private IEnumerable<int> GetColumnCells(int index)
    {
        for (int i = 0; i < 9; i++)
        {
            yield return Cells[(i * 9) + index];
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

    private static IEnumerable<Location> FindDuplicates(IEnumerable<int> values, int index)
    {
        bool[] exists = new bool[10];
        int count = 0;

        foreach (var value in values)
        {
            count++;

            if (value is 0)
            {
                continue;
            }
            else if (exists[value])
            {
                // TODO: This value is wrong
                yield return new(index, count, value);
            }

            exists[value] = true;
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

    private void InitializeBoxes()
    {
        for (int i = 0; i < 9; i++)
        {
            _boxes[i] = new Box(i);
        }
    }

    private void InitializeCandidates()
    {
        for (int i = 0; i < 81; i++)
        {
            if (Cells[i] is not 0)
            {
                Candidates[i] = [];
                continue;
            }

            Candidates[i] = Enumerable.Range(1, 9).ToList();
        } 
    }
}