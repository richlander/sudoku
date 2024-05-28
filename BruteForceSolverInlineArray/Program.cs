using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Sudoku;


Stopwatch stopwatch= Stopwatch.StartNew();

string puzzle = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
var boardBuffer = new Board();
var board = MemoryMarshal.CreateSpan(ref Unsafe.As<Board, byte>(ref boardBuffer), 81);
Bools boolBuffer= new Bools();
var bools = MemoryMarshal.CreateSpan(ref Unsafe.As<Bools, bool>(ref boolBuffer), 10);
var cellBuffer = new Cells();
var cells = MemoryMarshal.CreateSpan(ref Unsafe.As<Cells, Cell>(ref cellBuffer), 81);

var context = new Context(board, cells, bools);

for (int i = 0; i < 81; i++)
{
    board[i] = (byte)(puzzle[i] - '0');
    cells[i] = Puzzle.GetCellForIndex(i);
}

// if (!ValidateBoard(context))
// {
//     Console.WriteLine("Puzzle is invalid");
//     return;
// }


if (Solver(context, 0))
{
    Console.WriteLine("Puzzle is valid.");
    PrintBoard(board);
}
else
{
    Console.WriteLine("Puzzle is invalid");
}

stopwatch.Stop();
Console.WriteLine($"Time Elapsed (ms): {stopwatch.Elapsed.Milliseconds}");

bool Solver(Context context, int index)
{
    var (board, cells, bools) = context;
    if (board[index] > 0)
    {
        return index is 80 || Solver(context, index + 1);
    }

    Cell cell = cells[index];

    while (board[index] < 9)
    {
        board[index]++;
    
        if (IsValidForCell(context, cell))
        {
            if (index is 80 && ValidateBoard(context))
            {
                return true;
            }

            if (Solver(context, index + 1))
            {
                return true;
            }
        }
    }

    board[index] = 0;
    return false;
}

bool ValidateBoard(Context context)
{
    for (int i = 0; i < 9; i++)
    {
        if (!IsValid(context, i))
        {
            return false;
        }
    }

    return true;
}

bool IsValidForCell(Context context, Cell cell) => IsValidRow(context, cell.Row) && IsValidColumn(context, cell.Column) && IsValidBox(context, cell.Box);

bool IsValid(Context context,  int index) => IsValidRow(context, index) && IsValidColumn(context, index) && IsValidBox(context, index);

bool IsValidRow(Context context, int index)
{
    var (board, cells, bools) = context;
    bools.Clear();
    int offset = index * 9;
    Span<byte> indices = board.Slice(offset, 9);

    for (int i = 0; i < indices.Length; i++)
    {
        int value = indices[i];

        if (value is 0)
        {
            continue;
        }

        if (bools[value])
        {
            return false;
        }

        bools[value] = true;
    }

    return true;
}

bool IsValidColumn(Context context, int index)
{
    var (board, cells, bools) = context;
    bools.Clear();
    int offset = index;
    for (int i = 0; i < 9; i++)
    {
        int value = board[offset];
        offset += 9;

        if (value is 0)
        {
            continue;
        }

        if (bools[value])
        {
            return false;   
        }

        bools[value] = true;
    }

    return true;
}

bool IsValidBox(Context context, int index)
{
    var (board, cells, bools) = context;
    bools.Clear();
    Span<int> indices = Puzzle.IndicesByBox.AsSpan(index * 9, 9);
    for (int i = 0; i < indices.Length; i++)
    {
        int cell = indices[i];
        int value = board[cell];

        if (value is 0)
        {
            continue;
        }

        if (bools[value])
        {
            return false;
        }

        bools[value] = true;
    }

    return true;
}

void PrintBoard(Span<byte> b)
{
    StringBuilder builder = new(81);

    foreach(int value in b)
    {
        builder.Append(value);
    }

    Console.WriteLine(builder)  ;
}

[System.Runtime.CompilerServices.InlineArray(81)]
public struct Board
{
    private object _element0;
}

[System.Runtime.CompilerServices.InlineArray(10)]
public struct Bools
{
    private object _element0;
}

[System.Runtime.CompilerServices.InlineArray(81)]
public struct Cells
{
    private object _element0;
}

public readonly ref struct Context(Span<byte> board, Span<Cell> cells, Span<bool> bools)
{
    public Span<byte> Board { get; } = board;
    public Span<Cell> Cells { get; } = cells;
    public Span<bool> Bools { get; } = bools;

    public void Deconstruct(out Span<byte> board, out Span<Cell> cells, out Span<bool> bools)
    {
        board = Board;
        cells = Cells;
        bools = Bools;
    }
}