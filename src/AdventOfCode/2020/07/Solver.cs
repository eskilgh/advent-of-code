using System.Text.RegularExpressions;

namespace AdventOfCode.Y2020.D07;

record Rule(string Color, Dictionary<string, int> Contents);

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var rules = ParseRules(input);

        return rules.Values.Where(rule => Contains(rules, rule, "shiny gold")).Count();
    }

    public object PartTwo(string input)
    {
        var rules = ParseRules(input);
        return NumContainedBags(rules, rules["shiny gold"]);
    }

    static int NumContainedBags(Dictionary<string, Rule> rules, Rule rule)
    {
        if (rule.Contents.Count == 0)
            return 0;

        return rule.Contents.Select(kvp => kvp.Value * (1 + NumContainedBags(rules, rules[kvp.Key]))).Sum();
    }

    static bool Contains(Dictionary<string, Rule> rules, Rule rule, string color)
    {
        if (rule.Contents.Count == 0)
            return false;
        if (rule.Contents.ContainsKey(color))
            return true;
        return rule.Contents.Keys.Any(c => Contains(rules, rules[c], color));
    }

    static Dictionary<string, Rule> ParseRules(string input)
    {
        return input
            .Split('\n')
            .Select(line =>
            {
                var bagDescription = string.Join(' ', line.Split(' ')[0..2]);
                var contents = Regex
                    .Matches(line, @"(\d+)\s+(\w+\s+\w+)\s")
                    .Select(match => match.Value)
                    .Select(s => s.Split(' '))
                    .ToDictionary(s => string.Join(' ', s[1..3]), s => int.Parse(s[0]));
                return new Rule(bagDescription, contents);
            })
            .ToDictionary(rule => rule.Color, rule => rule);
    }
}
