namespace AdventOfCode.Y2023.D12;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return Parse(input)
            .Select(tpl => FindPermutations1(tpl.record, tpl.groups))
            .Sum();
    }

    public object PartTwo(string input)
    {
        var answers = Parse(input)
            .Select(tpl => Unfold(tpl.record, tpl.groups))
            .Select(tpl => FindPermutations1(tpl.record, tpl.groups)).ToArray();
        return answers.Sum();
    }

    static IEnumerable<(string record, int[] groups)> Parse(string input)
    {
        return input.Split('\n').Select(line =>
        {
            var sections = line.Split(" ",
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
            var groups = sections[1].Split(',').Select(int.Parse).ToArray();
            return (sections[0], groups);
        });
    }

    public static (string record, int[] groups) Unfold(string record, int[] groups)
    {
        var unfoldedRecord = string.Join('?', Enumerable.Repeat(record, 5));
        var unfoldedGroups = Enumerable.Repeat(groups, 5).SelectMany(x => x).ToArray();
        return (unfoldedRecord, unfoldedGroups);
    }


    public static long FindPermutations1(string record, int[] groups)
    {
        var memo = new Dictionary<(string, string), long>();
        return FindPermutations1Rec(record, groups, memo);
    }

    static long FindPermutations1Rec(string record, int[] groups, Dictionary<(string, string), long> memo)
    {
        // Use a string for comparing groups to get 'value' equality
        var key = (record, string.Join(',', groups));
        if (!memo.ContainsKey(key))
        {
            memo[key] = record.FirstOrDefault() switch
            {
                '.' => PermutationsForOperational(record, groups, memo),
                '#' => PermutationsForBroken(record, groups, memo),
                '?' => PermutationsForWildcard(record, groups, memo),
                _ when groups.Length is 0 => 1,
                _ => 0,
            };
        }

        return memo[key];
    }

    static long PermutationsForWildcard(string record, int[] groups, Dictionary<(string, string), long> memo)
    {
        return PermutationsForOperational(record, groups, memo) + PermutationsForBroken(record, groups, memo);
    }

    static long PermutationsForOperational(string record, int[] groups, Dictionary<(string, string), long> memo)
    {
        return FindPermutations1Rec(record[1..], groups, memo);
    }

    static long PermutationsForBroken(string record, int[] groups, Dictionary<(string, string), long> memo)
    {
        if (groups.Length is 0)
            // '#' left after filling all groups -> not valid
            return 0;

        var targetSize = groups.First();
        var n = record.TakeWhile((c, i) => c is '#' or '?').Count();

        return n switch
        {
            _ when n < targetSize => 0,
            // End of record, but more groups to go
            _ when targetSize == record.Length && groups.Length > 1 => 0,
            // Last group left gets filled
            _ when targetSize == record.Length && groups.Length is 1 => 1,
            // '#' Follows the group -> not valid
            _ when record[targetSize] is '#' => 0,
            // We know that record has min length (targetSize + 1) so the index is safe
            // Make sure a '.' follows the group
            _ => FindPermutations1Rec(record[(targetSize + 1)..], groups[1..], memo)
        };
    }
}