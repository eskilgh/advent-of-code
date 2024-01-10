using AdventOfCode.Common;
using MathNet.Numerics;

namespace AdventOfCode.Y2020.D10;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var adapters = input.Split('\n').Select(int.Parse).ToArray();
        Array.Sort(adapters);
        adapters = [0, ..adapters, adapters[^1] + 3];
        var diffs = adapters.Pairwise((a, b) => b - a);
        return diffs.Count(d => d == 1) * diffs.Count(d => d == 3);
    }

    public object PartTwo(string input)
    {
        var adapters = input.Split('\n').Select(int.Parse).ToArray();
        Array.Sort(adapters);
        adapters = [0, ..adapters, adapters[^1] + 3];
        return ValidArrangements(adapters);
    }

    static long ValidArrangements(int[] adapters)
    {
        var dp = new long[adapters.Length];
        var i = adapters.Length - 1;
        dp[i] = 1;
        i--;
        while (i >= 0)
        {
            var j = i + 1;
            while (j < adapters.Length && adapters[j] - adapters[i] <= 3)
                dp[i] += dp[j++];
            i--;
        }
        return dp[0];
    }
}
