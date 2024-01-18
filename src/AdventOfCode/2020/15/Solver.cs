namespace AdventOfCode.Y2020.D15;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var startingNumbers = input.Split('\n')[0].Split(',').Select(int.Parse).ToArray();
        return GetNthNumber(startingNumbers, 2020);
    }

    public object PartTwo(string input)
    {
        var startingNumbers = input.Split('\n')[0].Split(',').Select(int.Parse).ToArray();
        return GetNthNumber(startingNumbers, 30_000_000);
    }

    static int GetNthNumber(int[] startingNumbers, int N)
    {
        var seen = startingNumbers[0..^1].Select((n, i) => (n, i)).ToDictionary(tpl => tpl.n, tpl => tpl.i + 1);

        var lastSpoken = startingNumbers[^1];
        var iterations = startingNumbers.Length;
        while (iterations < N)
        {
            var next = seen.TryGetValue(lastSpoken, out var lastSeen) ? iterations - lastSeen : 0;
            seen[lastSpoken] = iterations;
            lastSpoken = next;
            iterations++;
        }
        return lastSpoken;
    }
}
