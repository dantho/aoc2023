// https://adventofcode.com/2023/day/24
// Creating structs as value types for comparison an processing speed:
// https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
// Operator Overloading:
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading

public static class MyExtensions
{
    public static IEnumerable<(T First, T Second)> PairUp<T>(this IEnumerable<T> source)
    {
        using (var iterator = source.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                var first = iterator.Current;
                var second = iterator.MoveNext() ? iterator.Current : default(T);
                yield return (first, second);
            }
        }
    }
} 