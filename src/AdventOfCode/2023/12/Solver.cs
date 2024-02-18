using AngleSharp.Html;

namespace AdventOfCode.Y2023.D12;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return "susepended";
        var lines = input.Split('\n');
        var conditions = lines
            .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].ToCharArray())
            .ToArray();
        var groupings = lines
            .Select(
                line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)[1].Split(',').Select(int.Parse).ToArray()
            )
            .ToArray();

        var permutations = conditions.Zip(groupings, FindPermutationsMemoized).ToArray();
        //Console.WriteLine(string.Join(" ", permutations));
        return permutations.Sum();
        //var permutationsPerConditions = conditions.Zip(groupings, FindPerms).ToArray();

        //Console.WriteLine(FindPermutations(conditions[0], groupings[0]));
        /*for (var i = 0; i < permutationsPerConditions.Length; i++)
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

    static List<List<(int pos, int groupSize)>> FindPerms(char[] cs, int[] groups)
    {
        return FindPermsRec(cs, groups, 0, 0);
    }

    static List<List<(int start, int groupSize)>> FindPermsRec(char[] cs, int[] groups, int startPos, int groupIdx)
    {
        if (groupIdx >= groups.Length)
            return new List<List<(int, int)>>();

        var groupSize = groups[groupIdx];
        var possiblePermutations = new List<List<(int, int)>>();
        var pos = startPos;
        var canPlaceGroupAtCurrentPos =
            (pos + groupSize) <= cs.Length
            && cs[pos..(pos + groupSize)].All(c => c is '?' or '#')
            && (pos + groupSize >= cs.Length || cs[pos + groupSize] is '?' or '.');
        if (canPlaceGroupAtCurrentPos)
        {
            FindPermsRec(cs, groups, pos + groupSize + 1, groupIdx + 1)
                .Select(e =>
                {
                    List<(int, int)> placements = [.. e, (pos, groupSize)];
                    return placements;
                });
        }
        return possiblePermutations;
    }

    public static int FindUnfoldedPermutations(char[] cs, int[] groups)
    {
        var basePermutations = FindPermutations(cs, groups);
        var subPermutations = FindPermutations(['?', .. cs], groups);

        var csEndingWithBroken = cs.Take(cs.Length - 1).Append('#').ToArray();
        var basePermutationsEndingWithBroken = FindPermutations(csEndingWithBroken, groups);
        var subPermutationsBeginningWithOperational = FindPermutations(['.', .. cs.Skip(1)], groups);

        var csEndingWithOperational = cs.Take(cs.Length - 1).Append('.').ToArray();
        var basePermutationsEndingWithOperational = FindPermutations(csEndingWithOperational, groups);

        var subPermutationsBeginningWithBroken = FindPermutations(['#', ..cs.Skip(1)], groups);
        return basePermutations * Convert.ToInt32(Math.Pow(subPermutations, 4));
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

    public static long FindPermutationsMemoized(char[] cs, int[] groups)
    {
        var memo = new Dictionary<(char[] cs, int[] groups, int i, int groupIdx), int>();
        return FindPermutationsRecMemoized(cs, groups, 0, 0, memo);
    }

    static int FindPermutationsRecMemoized(char[] cs, int[] groups, int i, int groupIdx, IDictionary<(char[] cs, int[] groups, int i, int groupIdx), int> memo)
    {
        var input = (cs, groups, i, groupIdx);
        if (memo.TryGetValue(input, out var memoed))
            return memoed;
        if (groupIdx >= groups.Length)
        {
            // Not valid if remaining '#'s 
            var ans = i >= cs.Length || cs[i..].All(c => c is not '#') ? 1 : 0;
            memo[input] = ans;
            return ans;
        }
        if (i >= cs.Length)
        {
            var ans = 0;
            memo[input] = ans;
            return ans;
        }

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
                {
                    memo[input] = possiblePermutations;
                    return possiblePermutations;
                }
            }
        
            pos++;
        }
        memo[input] = possiblePermutations;
        return possiblePermutations;
    }

    public object PartTwo(string input)
    {
        var lines = input.Split('\n');
        var conditions = lines
            .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)[0].ToCharArray())
            .ToArray();
        var groupings = lines
            .Select(
                line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)[1].Split(',').Select(int.Parse).ToArray()
            )
            .ToArray();

        var unfoldedConditions = conditions.Select(cs =>
        {
            var ss = string.Join("?", Enumerable.Repeat(new string(cs), 5));
            return ss.ToCharArray();
        }).ToArray();
        var unfoldedGroupings = groupings.Select(groups => Enumerable.Repeat(groups, 5).SelectMany(x => x).ToArray());

        var totalItems = unfoldedConditions.Length;
        var doneItems = 0;
        var permutations = unfoldedConditions
            .Zip(unfoldedGroupings, (condition, grouping) => new { Condition = condition, Grouping = grouping })
            .AsParallel()
            .Select((item, i) =>
            {
                var perms = FindPermutationsMemoized(item.Condition, item.Grouping);
                UpdateProgressBar((++doneItems) * 100 / totalItems);
                return perms;
            }).ToArray();

        Console.WriteLine();
        //var permutations = unfoldedConditions.Zip(unfoldedGroupings, FindPermutationsMemoized).ToArray();
        return permutations.Sum();
    }

    // Add a method to update the progress bar
    void UpdateProgressBar(int percentage)
    {
        //Console.Clear();
        Console.WriteLine($"Progress: {percentage}%");
    }

    
}
