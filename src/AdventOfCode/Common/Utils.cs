using System.Numerics;
using System.Text;

namespace AdventOfCode.Common;

public static class Utils
{
    public static void Print(this char[][] m)
    {
        Print(m, new HashSet<Vector2d>());
    }

    public static void Print(this char[][] m, ISet<Vector2d> visited)
    {
        var sb = new StringBuilder();

        for (var row = 0; row < m.Length; row++)
        {
            for (var col = 0; col < m[row].Length; col++)
            {
                sb.Append(visited.Contains(new Vector2d(col, row)) ? '#' : m[row][col]);
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    public static (int, int) IndexOf2d(this char[][] m, char value)
    {
        for (var row = 0; row < m.Length; row++)
        for (var col = 0; col < m[row].Length; col++)
            if (m[row][col] == value)
                return (row, col);

        return (-1, -1);
    }

    public static bool AreCoprime(int a, int b)
    {
        return GCD(a, b) == 1;
    }

    public static int GCD(int x, int y)
    {
        while (y != 0)
        {
            int temp = y;
            y = x % y;
            x = temp;
        }
        return x;
    }
}
