foreach (var numbers in GetNumbers())
{
    Console.WriteLine(Sum(numbers));
}

static IEnumerable<Span<int>> GetNumbers()
{
    yield return [ 1, 2, 3 ];
    yield return [ 4, 5, 6 ];
}

static int Sum(ReadOnlySpan<int> numbers)
{
    int sum = 0;
    foreach (int number in numbers)
    {
        sum += number;
    }

    return sum;
}
