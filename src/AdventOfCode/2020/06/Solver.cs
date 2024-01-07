namespace AdventOfCode.Y2020.D06;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return input.Split("\n\n").Select(section => section.Where(char.IsLetter).ToHashSet().Count).Sum();
    }

    public object PartTwo(string input)
    {
        return input
            .Split("\n\n")
            .Select(
                section =>
                    section
                        .Split('\n')
                        .Select(line => line.Where(char.IsLetter).ToHashSet())
                        .Aggregate((a, b) => a.Intersect(b).ToHashSet())
                        .Count
            )
            .Sum();
    }
}
