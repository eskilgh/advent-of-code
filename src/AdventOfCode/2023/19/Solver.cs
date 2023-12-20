using System;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode.Y2023.D19;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        if ((input.Split("\n\n") is not [string first, string second]))
            throw new ArgumentOutOfRangeException();

        var workflows = ParseWorkflows(first);
        var parts = ParseParts(second);

        var accepted = parts.Aggregate(
            new List<Part>(),
            (accepted, part) =>
            {
                var dest = "in";
                while (dest is not ("R" or "A"))
                    dest = workflows[dest].First(rule => rule.Predicate(part)).Dest;
                return (dest is "A") ? [.. accepted, part] : accepted;
            }
        );

        return accepted.Select(part => part.X + part.A + part.M + part.S).Sum().ToString();
    }

    public string PartTwo(string input)
    {
        var workflows = input
            .Split("\n\n")[0]
            .Split("\n")
            .Select(line =>
            {
                var key = line.Split('{')[0];
                var updateFns = line.Split('{')[1][0..^1].Split(',').Select(ParseRule).ToList();
                return (key, updateFns);
            })
            .ToDictionary(e => e.key, e => e.updateFns);

        var accepted = new List<Combination>();
        var queue = new Queue<(Combination, string dest)>();
        var initialCombination = new Combination(
            new Range(1, 4000),
            new Range(1, 4000),
            new Range(1, 4000),
            new Range(1, 4000)
        );
        queue.Enqueue((initialCombination, "in"));
        while (queue.Count > 0)
        {
            var (combination, dest) = queue.Dequeue();
            foreach (var fn in workflows[dest])
            {
                var (nonConforming, conforming, newDest) = fn(combination);
                if (conforming is null)
                {
                    // Non-Conforming
                    if (newDest is "A" && nonConforming is not null)
                        accepted.Add(nonConforming);
                    else if (newDest is not "R" && nonConforming is not null)
                        queue.Enqueue((nonConforming, newDest));
                    break;
                }
                if (nonConforming is null)
                    break;

                // Conforming
                if (newDest is "A")
                    accepted.Add(conforming);
                else if (newDest is not "R")
                    queue.Enqueue((conforming, newDest));

                combination = nonConforming;
            }
        }

        var ans = BigInteger.Zero;
        for (var i = 0; i < accepted.Count; i++) {
            var combination = accepted[i];
            var distinctCombinations = combination.DistinctCombinations();
            var distinctIntersections = i == 0 ? 0 : accepted.Take(i)
                .Select(seenCombination =>
                    seenCombination.DistinctIntersections(combination)).Aggregate((acc, e) => acc + e);
            ans += distinctCombinations - distinctIntersections;
        }
        return ans.ToString();
    }

    static Func<Combination, (Combination?, Combination?, string)> ParseRule(string rawRule)
    {
        if (!rawRule.Contains(':'))
            return (Combination combination) => (combination, null, rawRule);

        var propKey = rawRule[0].ToString().ToUpper()!;
        var comparator = rawRule[1];
        var comparand = int.Parse(rawRule[2..].Split(':')[0]);
        var dest = rawRule[2..].Split(':')[1];

        (Range? NonConforming, Range? Conforming) Compare(Range range)
        {
            return comparator switch
            {
                '>' and var _ when range.Max <= comparand => (range, null),
                '>' and var _ when range.Min > comparand => (null, range),
                '>' => (range.WithNewMax(comparand), range.WithNewMin(comparand + 1)),
                '<' and var _ when range.Min >= comparand => (range, null),
                '<' and var _ when range.Max < comparand => (null, range),
                '<' => (range.WithNewMin(comparand), range.WithNewMax(comparand - 1)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        (Combination?, Combination?, string) UpdateCombination(Combination combination)
        {
            var property = typeof(Combination).GetProperty(propKey)!;
            var range = combination.GetRange(propKey);
            var newRanges = Compare(combination.GetRange(propKey));
            return newRanges switch
            {
                (null, Range conforming) => (null, combination.WithNewRange(propKey, conforming), dest!),
                (Range nonConforming, null) => (combination.WithNewRange(propKey, nonConforming), null, dest!),
                (Range nonConforming, Range conforming)
                    => (
                        combination.WithNewRange(propKey, nonConforming),
                        combination.WithNewRange(propKey, conforming),
                        dest!
                    ),
                _ => throw new ArgumentException()
            };
        }
        return UpdateCombination;
    }

    static Dictionary<string, List<(Func<Part, bool> Predicate, string Dest)>> ParseWorkflows(string rawWorkflows)
    {
        return rawWorkflows
            .Split('\n')
            .Select(line =>
            {
                var key = line.Split('{')[0];
                var rules = line.Split('{')[1][0..^1].Split(',').Select(ParsePredicateRule).ToList();
                return (key, rules);
            })
            .ToList()
            .ToDictionary(e => e.key, e => e.rules);
    }

    static (Func<Part, bool> Predicate, string Dest) ParsePredicateRule(string rawRule)
    {
        if (!rawRule.Contains(':'))
            return ((Part _) => true, rawRule);

        var propKey = rawRule[0];
        var comparand = int.Parse(rawRule[2..].Split(':')[0]);
        var dest = rawRule[2..].Split(':')[1];
        bool LesserThan(int val) => val < comparand;
        bool GreaterThan(int val) => val > comparand;
        Func<int, bool> operand = rawRule[1] switch
        {
            '>' => GreaterThan,
            '<' => LesserThan,
            _ => throw new ArgumentOutOfRangeException()
        };

        var predicate = (Part part) =>
            propKey switch
            {
                'x' => operand(part.X),
                'm' => operand(part.M),
                'a' => operand(part.A),
                's' => operand(part.S),
                _ => throw new ArgumentOutOfRangeException()
            };

        return (predicate, dest);
    }

    static IEnumerable<Part> ParseParts(string rawParts)
    {
        return rawParts
            .Split('\n')
            .Select(
                line =>
                    line[1..^1]
                        .Split(',')
                        .Aggregate(
                            new Part(0, 0, 0, 0),
                            (part, info) =>
                            {
                                var key = info.Split('=')[0][0];
                                var val = int.Parse(info.Split('=')[1]);
                                return key switch
                                {
                                    'x' => part with { X = val },
                                    'm' => part with { M = val },
                                    'a' => part with { A = val },
                                    's' => part with { S = val },
                                    _ => throw new ArgumentOutOfRangeException()
                                };
                            }
                        )
            );
    }
}

public record Part(int X, int M, int A, int S);

public readonly struct Range(int min, int max)
{
    public int Min { get; } = min;
    public int Max { get; } = max;

    public Range WithNewMax(int newMax)
    {
        return new Range(Min, newMax);
    }

    public Range WithNewMin(int newMin)
    {
        return new Range(newMin, Max);
    }
    
    public BigInteger DistinctCombinations()
    {
        return Max - Min + 1;
    }

    public BigInteger DistinctIntersections(Range other)
    {
        var min = Math.Max(Min, other.Min);
        var max = Math.Min(Max, other.Max);
        if (max < min)
            return 0;
        return max - min + 1;
    }
}

public record Combination(Range X, Range M, Range A, Range S)
{
    public BigInteger DistinctCombinations()
    {
        return X.DistinctCombinations() * M.DistinctCombinations() * A.DistinctCombinations() * S.DistinctCombinations();
    }

    public BigInteger DistinctIntersections(Combination other)
    {
        var x = X.DistinctIntersections(other.X);
        var m = M.DistinctIntersections(other.M);
        var a = A.DistinctIntersections(other.A);
        var s = S.DistinctIntersections(other.S);
        return x * m * a * s;
    }
};

file static class Extensions
{
    public static Range GetRange(this Combination combination, string propertyName)
    {
        var property = typeof(Combination).GetProperty(propertyName)!;
        return (Range)property.GetValue(combination)!;
    }

    public static Combination WithNewRange(this Combination combination, string propertyName, Range newRange)
    {
        var newCombination = combination with { };
        var property = typeof(Combination).GetProperty(propertyName)!;
        property.SetValue(newCombination, newRange);
        return newCombination;
    }
}
