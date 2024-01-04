namespace AdventOfCode.Y2022.D03;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return input
            .Split("\n")
            .Select(line => FindDuplicate(line[0..(line.Length / 2)], line[(line.Length / 2)..]))
            .Select(ToPriority)
            .Sum();
    }

    public object PartTwo(string input)
    {
        return input.Split("\n").Chunk(3).Select(FindDuplicate).Select(ToPriority).Sum();
    }

    static char FindDuplicate(params string[] ss) =>
        ss.Select(s => s.ToHashSet()).Aggregate((intersection, s) => intersection.Intersect(s).ToHashSet()).Single();

    static int ToPriority(char c)
    {
        var isUpperCase = c < 91;
        var ans = isUpperCase ? c - 'A' + 27 : c - 'a' + 1;
        return ans;
    }
}
