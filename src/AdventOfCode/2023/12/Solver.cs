using AngleSharp.Html;

namespace AdventOfCode.Y2023.D12;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return Parse(input).Select(tpl => FindPermutations(tpl.conditions, tpl.groupings)).Sum();

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
        var data = Parse(input).ToArray();

        //var permutations = unfoldedConditions.Zip(unfoldedGroupings, FindPermutationsMemoized).ToArray();
        return "not available";
    }

    static IEnumerable<(char[] conditions, int[] groupings)> Parse(string input)
    {

        return input.Split('\n').Select(line =>
        {
            var sections = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var conditions = sections[0].ToCharArray();
            var groupings = sections[1].Split(',').Select(int.Parse).ToArray();
            return (conditions, groupings);
        });
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

    
}
