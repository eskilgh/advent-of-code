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
}
