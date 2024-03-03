using AngleSharp.Html;

namespace AdventOfCode.Y2023.D12;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return Parse(input).Select(tpl => FindPermutations1(new string(tpl.conditions), tpl.groupings)).Sum();

        /*
        for (var i = 0; i < permutationsPerConditions.Length; i++)
        {
            var condition = conditions[i];
            Console.WriteLine("\n" + new string(condition));
            var permutations = permutationsPerConditions[i];
            foreach (var permutation in permutations)
            {
                foreach (var (pos, groupSize) in permutation)
                {
                    for (var offset = 0; offset < groupSize; offset++)
                        condition[pos + offset] = '#';
                }
                Console.WriteLine(new string(condition).Replace('?', '.'));
            }
        }
        return permutationsPerConditions.Select(ps => ps.Count()).Sum();
        */
    }

    public object PartTwo(string input)
    {
        var answers = Parse(input)
            .Select(tpl => Unfold(tpl.conditions, tpl.groupings))
            .Select((tpl) => FindPermutations1(new string(tpl.conditions), tpl.groups)).ToArray();
        return answers.Sum();
        Console.WriteLine("------");
        var parsed = Parse(input).ToArray();
        var i = 0;
        foreach (var num in answers)
        {
            Console.Write(new string(parsed[i].conditions) + " [" + string.Join(" ", parsed[i].groupings) + "] ");
            Console.WriteLine(num);
            i++;
        }

        /*
        for (var i = 0; i < permutationsPerConditions.Length; i++)
        {
            var condition = conditions[i];
            Console.WriteLine("\n" + new string(condition));
            var permutations = permutationsPerConditions[i];
            foreach (var permutation in permutations)
            {
                foreach (var (pos, groupSize) in permutation)
                {
                    for (var offset = 0; offset < groupSize; offset++)
                        condition[pos + offset] = '#';
                }
                Console.WriteLine(new string(condition).Replace('?', '.'));
            }
        }
        return permutationsPerConditions.Select(ps => ps.Count()).Sum();
        */
        return answers.Sum();
        //var permutations = unfoldedConditions.Zip(unfoldedGroupings, FindPermutationsMemoized).ToArray();
        return "not available";
    }

    static IEnumerable<(char[] conditions, int[] groupings)> Parse(string input)
    {
        return input.Split('\n').Select(line =>
        {
            var sections = line.Split(" ",
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries);
            var conditions = sections[0].ToCharArray();
            var groupings = sections[1].Split(',').Select(int.Parse).ToArray();
            return (conditions, groupings);
        });
    }

    public static long FindUnfoldedPermutations(char[] cs, int[] groups)
    {
        var basePermutations = FindPermutations(cs, groups);
        var prefPerms = FindPermutations(['?', ..cs], groups);
        var sufPerms = FindPermutations([..cs, '?'], groups);
        var subPermutations = FindPermutations(['?', .. cs], groups);
        if (subPermutations == basePermutations)
        {
            var suffixedPermutations = FindPermutations([.. cs, '?'], groups);
            return basePermutations * Convert.ToInt64(Math.Pow(suffixedPermutations, 4));
        }

        char[] csEndingWithBroken = [..cs[..^1], '#'];
        var basePermutationsEndingWithBroken = FindPermutations(csEndingWithBroken, groups);
        var subPermutationsBeginningWithOperational = FindPermutations(['.', ..cs[1..]], groups);
        var numPermutationsForBaseEndingWithBroken = cs switch
        {
            [.. , '.'] => 0,
            _ => Convert.ToInt64(Math.Pow(basePermutationsEndingWithBroken, 4)),
        };


        char[] csEndingWithOperational = [..cs[..^1], '.'];
        var basePermutationsEndingWithOperational = FindPermutations(csEndingWithOperational, groups);
        var numPermutationsForBaseEndingWithOperational =
            cs switch
            {
                [.. , '#'] => 0,
                _ => basePermutationsEndingWithOperational * Convert.ToInt64(Math.Pow(subPermutations, 4))
            };

        return numPermutationsForBaseEndingWithBroken + numPermutationsForBaseEndingWithOperational;

        //var subPermutationsBeginningWithBroken = FindPermutations(['#', ..cs[1..]], groups);
        //return basePermutations * Convert.ToInt32(Math.Pow(subPermutations, 4));
    }

    private static Dictionary<(string, int[]), long> memo = new();
    public static long FindPermutations1(string record, int[] groups)
    {
        //var memo = new Dictionary<(string, int[]), long>();
        return FindPermutations1Rec(record, groups, memo);
    }
    static long FindPermutations1Rec(string record, int[] groups, Dictionary<(string, int[]), long> memo)
    {
        if (!memo.ContainsKey((record, groups)))
        {
            memo[(record, groups)] = record.FirstOrDefault() switch
            {
                '.' => PermutationsForOperational(record, groups, memo),
                '#' => PermutationsForBroken(record, groups, memo),
                '?' => PermutationsForWildcard(record, groups, memo),
                _ when groups.Length is 0 => 1,
                _ => 0,
            };

        }
        return memo[(record, groups)];
    }

    static long PermutationsForWildcard(string record, int[] groups, Dictionary<(string, int[]), long> memo)
    {
        return PermutationsForOperational(record, groups, memo) + PermutationsForBroken(record, groups, memo);
    }

    static long PermutationsForOperational(string record, int[] groups, Dictionary<(string, int[]), long> memo)
    {
        return FindPermutations1Rec(record[1..], groups, memo);
    }

    static long PermutationsForBroken(string record, int[] groups, Dictionary<(string, int[]), long> memo)
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


    public static int FindPermutations(char[] cs, int[] groups)
    {
        return FindPermutationsRec(cs, groups, 0, 0);
    }

    static int FindPermutationsRec(char[] cs, int[] groups, int i, int groupIdx)
    {
        if (groupIdx >= groups.Length)
            // Not valid if remaining '#'s 
            return i >= cs.Length || cs[i..].All(c => c is not '#') ? 1 : 0;
        if (i >= cs.Length)
            return 0;

        var groupSize = groups[groupIdx];
        var possiblePermutations = 0;
        var pos = i;
        var nextBrokenSpring = Array.IndexOf(cs, '#', i);
        while ((pos + groupSize) <= cs.Length && (nextBrokenSpring == -1 || pos <= nextBrokenSpring))
        {
            var canPlaceGroupAtCurrentPos =
                cs[pos..(pos + groupSize)].All(c => c is '?' or '#')
                // if not at the end, must have a '.' between '#'
                && (pos + groupSize >= cs.Length || cs[pos + groupSize] is '?' or '.');
            if (canPlaceGroupAtCurrentPos)
            {
                possiblePermutations += FindPermutationsRec(cs, groups, pos + groupSize + 1, groupIdx + 1);
                var isTheOnlyValidPlacementLeft = cs[pos..(pos + groupSize)].All(c => c is '#');
                if (isTheOnlyValidPlacementLeft)
                    return possiblePermutations;
            }

            pos++;
        }

        return possiblePermutations;
    }

    public static (char[] conditions, int[] groups) Unfold(char[] conditions, int[] groups)
    {
        var newConditions = string.Join('?', Enumerable.Range(0, 5).Select(_ => new string(conditions))).ToCharArray();
        var newGroups = Enumerable.Range(0, 5).SelectMany(_ => groups).ToArray();
        return (newConditions, newGroups);
    }
}