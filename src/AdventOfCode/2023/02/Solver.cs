using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.D02;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        Dictionary<string, int> config =
            new()
            {
                { "red", 12 },
                { "green", 13 },
                { "blue", 14 }
            };
        var ans = input
            .Split('\n')
            .Select(line =>
            {
                var gameId = int.Parse(line.Split(':')[0].Split(' ')[^1]);
                var pattern = @"(\d+)\s+(\w+)";
                var colorCounts = Regex.Matches(line, pattern);
                var isPossibleGame = colorCounts.All(m =>
                {
                    var count = int.Parse(m.Groups[1].Value);
                    var color = m.Groups[2].Value;
                    return count <= config[color];
                });
                return isPossibleGame ? gameId : 0;
            })
            .Sum();

        return ans.ToString();
    }

    public string PartTwo(string input)
    {
        var ans = input
            .Split('\n')
            .Select(line =>
            {
                var pattern = @"(\d+)\s+(\w+)";
                Dictionary<string, int> maxCounts =
                    new()
                    {
                        { "red", 0 },
                        { "green", 0 },
                        { "blue", 0 }
                    };
                foreach (var match in Regex.Matches(line, pattern).Cast<Match>())
                {
                    var count = int.Parse(match.Groups[1].Value);
                    var color = match.Groups[2].Value;
                    maxCounts[color] = Math.Max(count, maxCounts[color]);
                }
                return maxCounts.Values.Aggregate((a, b) => a * b);
            })
            .Sum();
        return ans.ToString();
    }
}
