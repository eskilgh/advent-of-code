using System.Text.RegularExpressions;
using AdventOfCode.Common;

namespace AdventOfCode.Y2020.D16;

record Rule(string Field, Interval Interval);

record struct Ticket(int X1, int X2, int X3);

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var (rules, _, nearbyTickets) = Parse(input);

        bool IsValidValue(int value) => rules.Values.SelectMany(ranges => ranges).Any(range => range.Contains(value));
        return nearbyTickets.SelectMany(ticket => ticket.Where(IsValidValue)).Sum();
    }

    static (Dictionary<string, IEnumerable<Interval>> rules, int[] ticket, IEnumerable<int[]> nearbyTickets) Parse(
        string input
    )
    {
        var sections = input.Split("\n\n");

        var ticket = sections[1].Split('\n')[^1].Split(',').Select(int.Parse).ToArray();
        var nearbyTickets = sections[2].Split('\n').Skip(1).Select(line => line.Split(',').Select(int.Parse).ToArray());

        var rules = sections[0]
            .Split('\n')
            .Select(line =>
            {
                var field = line.Split(':')[0];
                var ranges = Regex
                    .Matches(line, @"\d+-\d+")
                    .Select(m =>
                    {
                        var nums = m.Value.Split('-').Select(int.Parse);
                        return new Interval(nums.First(), nums.Last());
                    });
                return (field, ranges);
            })
            .ToDictionary(tpl => tpl.field, tpl => tpl.ranges);
        return (rules, ticket, nearbyTickets);
    }

    public object PartTwo(string input)
    {
        return "Not available";
    }
}
