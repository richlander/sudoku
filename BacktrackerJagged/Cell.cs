namespace Sudoku;

public record Cell(int Row, int Column, int Box)
{
    public bool MoveNext(out Cell nextCell)
    {
        var (x, y, z) = this;

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
        else
        {
            nextCell = this;
            return false;
        }

        nextCell = new(x, y, z);
        return true;
    }
}
