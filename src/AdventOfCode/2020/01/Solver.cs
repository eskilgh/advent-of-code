namespace AdventOfCode.Y2020.D01;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var ns = input.Split('\n').Select(int.Parse);
        var (a, b) = FindTwoSum(ns, 2020);
        return a * b;
    }
    public object PartTwo(string input)
    {
        var ns = input.Split('\n').Select(int.Parse);
        var (a, b, c) = FindThreeSum(ns, 2020);
        return a * b * c;
    }

    (int, int) FindTwoSum(IEnumerable<int> ns, int target)
    {
        var seen = new HashSet<int>();
        foreach (var n in ns)
        {
            if (seen.Contains(target - n))
                return (target - n, n);
            seen.Add(n);
        }
        return (-1, -1);
    }
    (int, int, int) FindThreeSum(IEnumerable<int> ns, int target)
    {
        foreach (var n in ns)
        {
            var (a, b) = FindTwoSum(ns, target - n);
            if (a > -1 && b > -1)
                return (n, a, b);
        }
        return (-1, -1, -1);
    }


}
