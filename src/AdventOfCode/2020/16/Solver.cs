using System.Text.RegularExpressions;
using AdventOfCode.Common;

namespace AdventOfCode.Y2020.D16;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var (rules, _, nearbyTickets) = Parse(input);

        return nearbyTickets.SelectMany(ticket => ticket.Where(value => IsValidValue(rules, value))).Sum();
    }

    public object PartTwo(string input)
    {
        var (rules, myTicket, nearbyTickets) = Parse(input);
        var validTickets = nearbyTickets.Where(ticket => ticket.All(value => IsValidValue(rules, value)));

        var valuesByPosition = validTickets.Aggregate(
            myTicket.Select((_) => Enumerable.Empty<int>()),
            (acc, ticket) => acc.Select((vs, i) => vs.Append(ticket[i]))
        );

        var validRulesForPositions = valuesByPosition.Select(
            vs => rules.Where(kvp => vs.All(v => kvp.Value.Any(range => range.Contains(v)))).ToHashSet()
        );

        // For my input this was sufficient for ending up with one rule per position
        // Other input may require more advanced 'sudoku'-solving
        while (validRulesForPositions.Any(rules => rules.Count > 1))
        {
            var takenRules = validRulesForPositions.Where(rules => rules.Count == 1).SelectMany(r => r).ToHashSet();
            validRulesForPositions = validRulesForPositions.Select(
                rules => rules.Count == 1 ? rules : rules.Except(takenRules).ToHashSet()
            );
        }

        return validRulesForPositions
            .Select((rules, i) => (ruleName: rules.Single().Key, i))
            .Where(tpl => tpl.ruleName.StartsWith("departure"))
            .Aggregate((long)1, (acc, tpl) => acc * myTicket[tpl.i]);
    }

    static bool IsValidValue(Dictionary<string, Interval[]> rules, int value)
    {
        return rules.Values.SelectMany(ranges => ranges).Any(range => range.Contains(value));
    }

    static (Dictionary<string, Interval[]> rules, int[] ticket, IEnumerable<int[]> nearbyTickets) Parse(string input)
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
                    })
                    .ToArray();
                return (field, ranges);
            })
            .ToDictionary(tpl => tpl.field, tpl => tpl.ranges);
        return (rules, ticket, nearbyTickets);
    }
}
