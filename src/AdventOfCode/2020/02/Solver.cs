namespace AdventOfCode.Y2020.D02;

record Policy(int Min, int Max, char Letter);

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return Parse(input).Count(IsValidPasswordSledRental);
    }

    public object PartTwo(string input)
    {
        return Parse(input).Count(IsValidPasswordToboggan);
    }

    private static IEnumerable<(Policy Policy, string Password)> Parse(string input)
    {
        return input
            .Split('\n')
            .Select(line =>
            {
                var parts = line.Split(':', StringSplitOptions.TrimEntries);
                var letter = parts[0][^1];
                var nums = parts[0].Split(' ')[0].Split('-').Select(int.Parse).ToArray();
                var min = nums[0];
                var max = nums[1];
                return (Policy: new Policy(min, max, letter), Password: parts[1]);
            });
    }

    static bool IsValidPasswordSledRental((Policy Policy, string Password) entry)
    {
        var occurences = entry.Password.Count(c => c == entry.Policy.Letter);
        return occurences >= entry.Policy.Min && occurences <= entry.Policy.Max;
    }

    static bool IsValidPasswordToboggan((Policy Policy, string Password) entry)
    {
        var (pos1, pos2) = (entry.Password[entry.Policy.Min - 1], entry.Password[entry.Policy.Max - 1]);
        return pos1 == entry.Policy.Letter ^ pos2 == entry.Policy.Letter;
    }
}
