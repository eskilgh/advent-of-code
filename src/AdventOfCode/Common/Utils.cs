﻿using System.Text;

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
}
