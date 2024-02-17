/*namespace AdventOfCode.Y2023.D12;

internal class AttemptTwo : ISolver
{
    public object PartOne(string input)
    {
        var patterns = input.Split('\n');
        return "Not available";
    }

    public object PartTwo(string input)
    {
        return "Not available";
    }

    public static int FindValidArrangements(string line, int[] groups)
    {
        return FindValidArrangements(line, groups, 0, 0);
    }

    private static int FindValidArrangements(string line, int[] groups, int startIdx, int groupIdx)
    {
        var currentGroup = groups[groupIdx];
        List<int> possibleInsertions = [];

        var i = startIdx;
        while (line.Length - i >= currentGroup)
        {
            var s = line.Substring(i, currentGroup);
            if (line[i..(i + currentGroup)].All(c => c is '#' or '?'))
            {
                if (groupIdx == groups.Length - 1)
                    possibleInsertions.Add(1);
                // the next pos has to be a '.'
                else if ((i + currentGroup + 1) < line.Length && line[i + currentGroup + 1] is '.' or '?')
                {
                    var subArrangements = FindValidArrangements(line, groups, i + currentGroup + 1, ++groupIdx);
                    possibleInsertions.Add(subArrangements);
                }
            }
            i++;
        }
        return possibleInsertions.Sum();
    }
}
*/