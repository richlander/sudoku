using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku;

Stopwatch stopwatch= Stopwatch.StartNew();
string puzzle = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
long[] board = new long[9];
long[] masks = [15, 240, 3840, 61440, 983040, 15728640, 251658240, 4026531840, 64424509440];

HashSet<int> cells = new(10);

for (int i = 0; i < 9; i++)
{
    ReadOnlySpan<char> segment = puzzle.AsSpan(0 * 9, 9);
    int offset = i * 9;
    long boardValue = board[i];
    
    for (int j = 0; j < 9; j++)
    {
        int value = puzzle[offset] - '0';
        offset++;

        if (value is 0)
        {
            continue;
        }

        long shift = value << j * 4;
        boardValue |= shift;
    }

    board[i] = boardValue;
}

if (!ValidateBoard())
{
    Console.WriteLine("Puzzle is invalid");
    return;
}

if (Solver(0))
{
    Console.WriteLine("Puzzle is valid.");
    // PrintBoard(board);
}
else
{
    Console.WriteLine("Puzzle is invalid");
}

stopwatch.Stop();
Console.WriteLine($"Time Elapsed (ms): {stopwatch.Elapsed.Milliseconds}");

bool Solver(int index)
{
    Cell cell = Puzzle.GetCellForIndex(index);
    long row = board[cell.Row];
    int column = cell % 9;
    long highValue = row & masks[column];
    long value = highValue >> column;

    if (value > 0)
    {
        return index is 80 || Solver(index + 1);
    }

    while (value < 9)
    {
        value++;
        row |= masks[column];
        row ^= masks[column];
        row |= value << column * 4;
        board[cell.Row] |= row;
    
        if (true)
        {
            if (index is 80)
            {
                return true;
            }

            if (Solver(index + 1))
            {
                return true;
            }
        }
    }

    row |= masks[column];
    row ^= masks[column];
    board[column] |= row;
    return false;
}

bool ValidateBoard()
{
    for (int i = 0; i < 9; i++)
    {
        if (!IsValid(i))
        {
            return false;
        }
    }

    return true;
}

bool IsValid(int index) => IsValidRow(index) && IsValidColumn(index) && IsValidBox(index);

bool IsValidRow(int index)
{
    cells.Clear();
    int offset = index * 9;
    Span<int> range = board.AsSpan(offset, 9);
    foreach (int value in range)
    {
        if (!(value is 0 || cells.Add(value)))
        {
            return false;
        }
    }

    return true;
}

bool IsValidColumn(int index)
{
    cells.Clear();
    int offset = index;
    for (int i = 0; i < 9; i++)
    {
        int value = board[offset];
        if (! (value is 0 || cells.Add(value)))
        {
            return false;
        }
        offset += 9;
    }

    return true;
}

bool IsValidBox(int index)
{
    cells.Clear();
    Span<int> indices = Puzzle.IndicesByBox.AsSpan(index * 9, 9);
    foreach (int cell in indices)
    {
        int value = board[cell];
        if (!(value is 0 || cells.Add(value)))
        {
            return false;
        }
    }

    return true;
}

void PrintBoard(long[] b)
{
    StringBuilder builder = new(81);

    foreach(int value in b)
    {
        builder.Append(value);
    }

    Console.WriteLine(builder);
}
