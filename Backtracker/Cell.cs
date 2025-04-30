namespace Sudoku;

public record Cell(int Index, int Row, int Column, int Box)
{
    public static implicit operator int(Cell c) => c.Index;

    public bool MoveNext(out Cell nextCell)
    {
        var (i, x, y, z) = this;

        if (i is 80)
        {
            nextCell = this;
            return false;
        }

        if (y < 8)
        {
            y++;

            if (y % 3 is 0)
            {
                z++;
            }
        }
        else if (x < 8)
        {
            x++;
            y = 0;
            z = x / 3 * 3;
        }

        nextCell = new(i++, x, y, z);
        return true;
    }
};
