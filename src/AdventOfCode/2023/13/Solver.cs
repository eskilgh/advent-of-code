using System.Collections;
using System.Data;
using AngleSharp.Dom;

namespace AdventOfCode.Y2023.D13;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        return ParsePatterns(input)
            .Select(pattern =>
            {
                var horizontalReflectionLine = FindHorizontalReflectionLine(pattern, targetSmudges: 0);
                if (horizontalReflectionLine > -1)
                    return horizontalReflectionLine * 100;

                return FindVerticalReflectionLine(pattern, targetSmudges: 0);
            })
            .Sum()
            .ToString();
    }

    private static IEnumerable<char[][]> ParsePatterns(string input)
    {
        return input.Split("\n\n").Select(x => x.Split('\n').Select(line => line.ToCharArray()).ToArray());
    }

    public string PartTwo(string input)
    {
        return ParsePatterns(input)
            .Select(pattern =>
            {
                var horizontalReflectionLine = FindHorizontalReflectionLine(pattern, targetSmudges: 1);
                if (horizontalReflectionLine > -1)
                    return horizontalReflectionLine * 100;

                return FindVerticalReflectionLine(pattern, targetSmudges: 1);
            })
            .Sum()
            .ToString();
    }

    static int FindHorizontalReflectionLine(char[][] pattern, int targetSmudges = 0)
    {
        for (var row = 0; row < pattern.Length - 1; row++)
        {
            if (EvaluateHorizontal(pattern, row, targetSmudges: targetSmudges))
                return row + 1;
        }
        return -1;
    }

    static int FindVerticalReflectionLine(char[][] pattern, int targetSmudges = 0)
    {
        for (var col = 0; col < pattern[0].Length - 1; col++)
        {
            if (EvaluateVertical(pattern, col, targetSmudges: targetSmudges))
                return col + 1;
        }
        return -1;
    }

    static bool EvaluateHorizontal(char[][] pattern, int candidate, int targetSmudges)
    {
        var iters = Math.Min(candidate, pattern.Length - candidate - 2) + 1;
        var i = 0;
        var smudges = 0;
        while ((candidate - i) >= 0 && (candidate + i + 1) < pattern.Length)
        {
            smudges += pattern[candidate - i].Zip(pattern[candidate + i + 1]).Count(pair => pair.First != pair.Second);
            if (smudges > targetSmudges)
                return false;
            i++;
        }
        return smudges == targetSmudges;
    }

    static bool EvaluateVertical(char[][] pattern, int candidate, int targetSmudges)
    {
        var iters = Math.Min(candidate, pattern.Length - candidate - 2) + 1;
        var i = 0;
        var smudges = 0;
        while ((candidate - i) >= 0 && (candidate + i + 1) < pattern[0].Length)
        {
            smudges += pattern.Count(row => row[candidate - i] != row[candidate + i + 1]);
            if (smudges > targetSmudges)
                return false;
            i++;
        }
        return smudges == targetSmudges;
    }
}
