using System.Collections;
using Sudoku;

public static class IEnumerableExtensions
{
    public static bool ValueIfCountExact(this IEnumerable<int> enumerable, int count, out int value)
    {
        int x = 0;
        value = default;
        IEnumerator<int> enumerator = enumerable.GetEnumerator();

        while (enumerator.MoveNext())
        {
            x++;

            if (x > count)
            {
                return false;
            }
        }

        if (x == count)
        {
            value = enumerator.Current;
            return true;
        }

        return false;
    }
}
