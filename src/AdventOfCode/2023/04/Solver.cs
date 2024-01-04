namespace AdventOfCode.Y2023.D04;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        return input
            .Split('\n')
            .Select(line =>
            {
                var nums = line.Split(':')[^1]
                    .Split('|')
                    .Select(nums => nums.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToHashSet())
                    .ToList();
                var winningNumbersCount = nums[0].Intersect(nums[1]).Count();
                return winningNumbersCount == 0 ? 0 : Math.Pow(2, winningNumbersCount - 1);
            })
            .Sum();
    }

    public object PartTwo(string input)
    {
        var matchingNumbersCounts = input
            .Split('\n')
            .Select(line =>
            {
                var nums = line.Split(':')[^1]
                    .Split('|')
                    .Select(nums => nums.Split(' ').Where(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToHashSet())
                    .ToList();
                return nums[0].Intersect(nums[1]).Count();
            })
            .ToArray();
        var cardInstances = matchingNumbersCounts.Select(_ => 1).ToArray();
        for (var i = 0; i < matchingNumbersCounts.Length; i++)
        {
            var instancesOfCurrentCard = cardInstances[i];
            var matchingNumbersOfCurrentCard = matchingNumbersCounts[i];
            for (
                var cardInstanceIndex = i + 1;
                cardInstanceIndex <= i + matchingNumbersOfCurrentCard;
                cardInstanceIndex++
            )
            {
                cardInstances[cardInstanceIndex] += instancesOfCurrentCard;
            }
        }
        return cardInstances.Sum();
    }
}
