using System.Collections;
using AngleSharp.Dom;

namespace AdventOfCode.Y2023.D13;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        return "";
        /*IEnumerable<char[][]> patterns = ParsePatterns(input);

        return patterns
            .Select(pattern =>
            {
                var horizontalReflectionLine = FindHorizontalReflectionLine(pattern);
                if (horizontalReflectionLine > 0)
                    return horizontalReflectionLine * 100;
                return FindVerticalReflectionLine(pattern);
            })
            .Sum()
            .ToString();*/
    }

    private static IEnumerable<char[][]> ParsePatterns(string input)
    {
        return input.Split("\n\n").Select(x => x.Split('\n').Select(line => line.ToCharArray()).ToArray());
    }

    static int FindVerticalReflectionLine(char[][] pattern)
    {
        return PairwiseEqualCols(pattern).FirstOrDefault(candidate => IsReflectionCol(pattern, candidate), -2) + 1;
    }

    static int FindHorizontalReflectionLine(char[][] pattern)
    {
        return PairwiseEqualRows(pattern).FirstOrDefault(candidate => IsReflectionRow(pattern, candidate), -2) + 1;
    }

    static bool IsReflectionRow(char[][] pattern, int candidate)
    {
        var iters = Math.Min(candidate, pattern.Length - candidate - 2);
        for (var i = 1; i <= iters; i++)
        {
            if (!pattern[candidate - i].SequenceEqual(pattern[candidate + 1 + i]))
                return false;
        }
        return true;
    }

    static bool IsReflectionCol(char[][] pattern, int candidate)
    {
        var iters = Math.Min(candidate, pattern[0].Length - candidate - 2);
        for (var i = 1; i <= iters; i++)
        {
            if (!pattern.All(row => row[candidate - i] == row[candidate + i + 1]))
                return false;
        }
        return true;
    }

    static IEnumerable<int> PairwiseEqualRows(char[][] pattern)
    {
        for (var row = 0; row < pattern.Length - 1; row++)
        {
            if (pattern[row].SequenceEqual(pattern[row + 1]))
                yield return row;
        }
    }

    static IEnumerable<int> PairwiseEqualCols(char[][] pattern)
    {
        for (var col = 0; col < pattern[0].Length - 1; col++)
        {
            if (pattern.All(row => row[col] == row[col + 1]))
                yield return col;
        }
    }

    public string PartTwo(string input)
    {
        var test = ".####.##.\n#####.##.";
        var patterns = ParsePatterns(test);

        foreach (var pattern in patterns)
        {
            foreach (var item in FindHorizontalCandidates(pattern))
            {
                Console.WriteLine(item);
            }
            ;
        }
        return "Not available";
    }

    static IEnumerable<uint> FindHorizontalCandidates(char[][] pattern)
    {
        for (var row = 0; row < pattern.Length - 1; row++)
        {
            var iters = Math.Min(row, pattern.Length - row - 2);
            var height = (1 + iters) * 2;
            var flipped = FindFlippedBit(pattern, row - iters, row + iters + 2);
            if (flipped > 0)
                yield return flipped;
        }
        yield break;
    }

    static uint FindFlippedBit(char[][] pattern, int startRow, int endRow)
    {
        var nums = pattern[startRow..endRow].Select(
            cs =>
                Convert.ToUInt32(string.Join("", cs.Select(c => c == '#' ? '1' : '0')).PadLeft(pattern[0].Length, '0'))
        );
        var xored = nums.Aggregate((a, b) => a ^ b);
        Console.WriteLine(Convert.ToString(xored, 2));
        var isExactlyOneBitFlipped = (xored & (xored - 1)) == 0;
        return isExactlyOneBitFlipped ? xored : 0;
    }
}
