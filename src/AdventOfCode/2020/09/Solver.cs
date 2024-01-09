using MathNet.Numerics;

namespace AdventOfCode.Y2020.D09;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var ns = input.Split('\n').Select(long.Parse).ToArray();
        return FindInvalid(ns, 25);
    }

    public object PartTwo(string input)
    {
        var ns = input.Split('\n').Select(long.Parse).ToArray();
        var invalid = FindInvalid(ns, 25);
        var (start, stop) = FindContigousSum(ns, invalid);
        return start + stop;
    }

    /// <summary>
    /// Returns the min/max value of the matching contigous range.
    /// Standard sliding window algorithm.
    /// </summary>
    static (long min, long max) FindContigousSum(long[] ns, long target)
    {
        var l = 0;
        var r = 1;
        var sum = ns[l] + ns[r];
        while (sum != target || l == r)
        {
            if (sum > target)
            {
                sum -= ns[l];
                l++;
            }
            else
            {
                r++;
                sum += ns[r];
            }
        }
        return (ns[l..(r + 1)].Min(), ns[l..(r + 1)].Max());
    }

    /// <summary>
    /// Finds the first number that is not the sum of any two of the previous 25 numbers
    /// </summary>
    static long FindInvalid(long[] ns, int preambleLength)
    {
        for (var i = preambleLength; i < ns.Length; i++)
        {
            if (!ContainsPairSum(ns[(i - preambleLength)..i], ns[i]))
                return ns[i];
        }
        return -1;
    }

    static bool ContainsPairSum(IEnumerable<long> preamble, long value)
    {
        var seen = new HashSet<long>();
        foreach (var n in preamble)
        {
            if (seen.Contains(value - n))
                return true;
            seen.Add(n);
        }
        return false;
    }
}
