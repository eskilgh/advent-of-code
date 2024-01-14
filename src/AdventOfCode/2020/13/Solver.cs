using System.Numerics;

namespace AdventOfCode.Y2020.D13;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var lines = input.Split('\n');
        var minTime = int.Parse(lines[0]);
        var buses = lines[1].Split(',').Where(s => s is not "x").Select(int.Parse);

        var disembarkTime = minTime;
        while (true)
        {
            var bus = buses.Cast<int?>().FirstOrDefault(b => disembarkTime % b == 0);
            if (bus.HasValue)
            {
                return bus.Value * (disembarkTime - minTime);
            }
            disembarkTime++;
        }
    }

    public object PartTwo(string input)
    {
        var buses = input
            .Split('\n')[1]
            .Split(',')
            .Select(s => s is "x" ? 0 : long.Parse(s))
            .Select((busId, i) => (id: busId, departureTime: (long)i));

        var listedBuses = buses.Where(b => b.id != 0);

        // We want to find t which satifisfies a set of equations in this form
        // t = b_i mod n_i
        // Where
        // b_i = bus.id - bus.departureTime, n_i = bus.id

        var bs = listedBuses.Select(b => b.id - b.departureTime);
        var ns = listedBuses.Select(b => b.id);
        return ChineseRemainder(bs, ns);
    }

    // "Direct construction" algorithm from https://en.wikipedia.org/wiki/Chinese_remainder_theorem
    static long ChineseRemainder(IEnumerable<long> bs, IEnumerable<long> ns)
    {
        long N = ns.Aggregate((long)1, (a, b) => a * b);
        return bs.Zip(ns)
                .Select(
                    (tpl) =>
                    {
                        var (a, n) = tpl;
                        var p = N / n;
                        return a * InverseModulo(p, n) * p;
                    }
                )
                .Sum() % N;
    }

    static long InverseModulo(long a, long m)
    {
        return (long)BigInteger.ModPow(a, m - 2, m);
    }
}
