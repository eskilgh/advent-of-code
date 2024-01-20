using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode.Common;

internal static class Extensions
{
    public static char[][] To2dCharArray(this string input) =>
        input.Split('\n').Select(line => line.ToCharArray()).ToArray();

    public static char CharAt(this char[][] m, int row, int col, char fallback = '.')
    {
        if (row < 0 || row >= m.Length || col < 0 || col >= m[0].Length)
            return fallback;
        return m[row][col];
    }

    public static bool IsOutOfBounds(this char[][] m, Complex pos)
    {
        return pos.Real < 0 || pos.Real >= m[0].Length || pos.Imaginary < 0 || pos.Imaginary >= m.Length;
    }

    public static IEnumerable<TResult> Pairwise<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TSource, TResult> resultSelector
    )
    {
        using var it = source.GetEnumerator();

        if (!it.MoveNext())
            yield break;

        var previous = it.Current;

        while (it.MoveNext())
            yield return resultSelector(previous, previous = it.Current);
    }

    public static string ToPrettyString(this char[][] m, ISet<Vector2d>? visited = default, char visitedFill = '#')
    {
        var sb = new StringBuilder();

        for (var row = 0; row < m.Length; row++)
        {
            for (var col = 0; col < m[row].Length; col++)
            {
                var c = visited?.Contains(new Vector2d(col, row)) ?? false ? visitedFill : m[row][col];
                sb.Append(c);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static IEnumerable<(T Element, int Index)> WithIndex<T>(this IEnumerable<T> enumerable) =>
        enumerable.Select((e, i) => (e, i));
}
