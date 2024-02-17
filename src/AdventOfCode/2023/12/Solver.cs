using AngleSharp.Html;

namespace AdventOfCode.Y2023.D12;

internal class Solver : ISolver
{
    public object PartOne(string input)
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

        var permutations = conditions.Zip(groupings, FindPermutations).ToArray();
        Console.WriteLine(string.Join(" ", permutations));
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

    public object PartTwo(string input)
    {
        return "Not available";
    }
}
