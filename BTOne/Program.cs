using System.Diagnostics;
using System.Text;
using Sudoku;

Stopwatch stopwatch= Stopwatch.StartNew();
string puzzle = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
int[] board = new int[81];

HashSet<int> cells = new(10);

for (int i = 0; i < 81; i++)
{
    board[i] = puzzle[i] - '0';
}

if (!ValidateBoard())
{
    Console.WriteLine("Puzzle is invalid");
    return;
}

if (Solver(0))
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

bool Solver(int index)
{
    if (board[index] > 0)
    {
        return index is 80 || Solver(index + 1);
    }

    var (row, column, box) = GetCellInfo(index);

    while (board[index] < 9)
    {
        board[index]++;
    
        if (IsValidRow(row) &&
            IsValidColumn(column) && 
            IsValidBox(box))
        {
            if (index is 80 && ValidateBoard())
            {
                return true;
            }

            if (Solver(index + 1))
            {
                return true;
            }
        }
    }

    board[index] = 0;
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

void PrintBoard(int[] b)
{
    StringBuilder builder = new(81);

    foreach(int value in b)
    {
        builder.Append(value);
    }

    Console.WriteLine(builder);
}

int GetBoxForCell(int index) => index / 27 * 3 + index % 9 / 3;

int GetRowForCell(int index) => index / 9;

int GetColumnForCell(int index) => index  % 9;

Cell GetCellInfo(int index) => new(
    GetRowForCell(index),
    GetColumnForCell(index),
    GetBoxForCell(index)
);

record struct Cell(int Row, int Column, int Box);
