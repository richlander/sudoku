namespace Sudoku;

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> values)
    {
        foreach(T value in values)
        {
            hashSet.Add(value);
        }
    }

    public static void RemoveRange<T>(this HashSet<T> hashSet, IEnumerable<T> values)
    {
        foreach(T value in values)
        {
            hashSet.Remove(value);
        }
    }    
}