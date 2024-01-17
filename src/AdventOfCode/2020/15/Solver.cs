namespace AdventOfCode.Y2020.D15;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var a = input.Split('\n')[0].Split(',').Select(int.Parse).ToList();
        var iterations = a.Count - 1;
        while (iterations < 2019)
        {
            var lastSpoken = a.Last();
            var idx = LastSeen(a, lastSpoken);
            var numToAdd = idx == -1 ? 0 : iterations - idx;
            a.Add(numToAdd);
            iterations++;
        }
        return a.Last();
    }

    public object PartTwo(string input)
    {
        var a = input.Split('\n')[0].Split(',').Select(int.Parse).ToList();
        var iterations = a.Count - 1;
        while (iterations < 2019)
        {
            var lastSpoken = a.Last();
            var idx = LastSeen(a, lastSpoken);
            var numToAdd = idx == -1 ? 0 : iterations - idx;
            a.Add(numToAdd);
            iterations++;
        }
        return a.Last();
    }

    /// <summary>
    /// Searches from right to left, excluding the last element
    /// </summary>
    static (int index, bool isInLoop) LastSeen(List<int> a, int n)
    {
        var i = a.Count - 2;
        var isInLoop = true;
        while (i >= 0)
        {
            if (isInLoop && a[i] == 0)
                isInLoop = false;
            if (a[i] == n)
                return (i, isInLoop);
            i--;
        }
        return (-1, false);
    }
}
