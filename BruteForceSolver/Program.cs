using System.Diagnostics;
using System.Text;
using Sudoku;

Stopwatch stopwatch= Stopwatch.StartNew();
string puzzle = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
int[] board = new int[81];
Cell[] boardCells = new Cell[81];

HashSet<int> cells = new(10);

for (int i = 0; i < 81; i++)
{
    board[i] = puzzle[i] - '0';
    boardCells[i] = Puzzle.GetCellForIndex(i);
}

if (!ValidateBoard(board))
{
    Console.WriteLine("Puzzle is invalid");
    return;
}

if (Solver(board, boardCells, 0))
{
    Console.WriteLine("Puzzle is valid.");
    PrintBoard(board);
}
else
{
    Console.WriteLine("Puzzle is invalid");
}

stopwatch.Stop();
Console.WriteLine($"Time Elapsed (ms): {stopwatch.Elapsed.TotalMilliseconds}");

bool Solver(Span<int> board, Cell[] boardCells, int index)
{
    if (board[index] > 0)
    {
        return index is 80 || Solver(board, boardCells, index + 1);
    }

    Cell cell = boardCells[index];

    bool[] values = GetLegalValues(board, cell);
    int internalIndex = 0;

    while (internalIndex < 9)
    {
        internalIndex++;

        if (values[internalIndex])
        {
            continue;
        }

        board[index] = internalIndex;
        // Console.WriteLine($"Index: {index}; Value: {board[index]}");
    
        if (IsValidForCell(board, cell))
        {
            if (index is 80 && ValidateBoard(board))
            {
                return true;
            }

            if (Solver(board, boardCells, index + 1))
            {
                return true;
            }
        }
    }

    board[index] = 0;
    return false;
}

bool[] GetLegalValues(Span<int> board, Cell cell)
{
    bool[] values = new bool[10];
    foreach (int value in board.Slice(cell.Row * 9, 9))
    {
        if (value is 0)
        {
            continue;
        }

        if (!values[value])
        {
            values[value] = true;
        }
    }

    int offset = cell.Column;
    for (int i = 0; i < 9; i++)
    {
        int value = board[offset];
        if (!values[value])
        {
            values[value] = true;
        }
        offset += 9;
    }

    Span<int> indices = Puzzle.IndicesByBox.AsSpan(cell.Box * 9, 9);
    foreach (int index in indices)
    {
        int value = board[index];
        if (!values[value])
        {
            values[value] = true;
        }
    }

    return values;
}

bool ValidateBoard(Span<int> board)
{
    for (int i = 0; i < 9; i++)
    {
        if (!IsValid(board, i))
        {
            return false;
        }
    }

    return true;
}

bool IsValidForCell(Span<int> board, Cell cell) => IsValidRow(board, cell.Row) && IsValidColumn(board, cell.Column) && IsValidBox(board, cell.Box);

bool IsValid(Span<int> board, int index) => IsValidRow(board, index) && IsValidColumn(board, index) && IsValidBox(board, index);

bool IsValidRow(Span<int> board, int index)
{
    cells.Clear();
    int offset = index * 9;
    Span<int> range = board.Slice(offset, 9);
    foreach (int value in range)
    {
        if (!(value is 0 || cells.Add(value)))
        {
            return false;
        }
    }

    return true;
}

bool IsValidColumn(Span<int> board, int index)
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

bool IsValidBox(Span<int> board, int index)
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

void PrintBoard(Span<int> b)
{
    StringBuilder builder = new(81);

    foreach(int value in b)
    {
        builder.Append(value);
    }

    Console.WriteLine(builder);
}
