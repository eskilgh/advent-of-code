namespace AdventOfCode.Y2022.D04;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        return input
            .Split("\n")
            .Select(line =>
            {
                var left = line.Split(",")[0];
                var right = line.Split(",")[1];
                var leftParsed = left.Split("-").Select(n => Convert.ToInt32(n)).ToArray();
                var rightParsed = right.Split("-").Select(n => Convert.ToInt32(n)).ToArray();
                return new int[] { leftParsed[0], leftParsed[1], rightParsed[0], rightParsed[1] };
            })
            .Where(ns => IsFullyContained(ns[0], ns[1], ns[2], ns[3]))
            .Count()
            .ToString();
    }

    public string PartTwo(string input)
    {
        return input
            .Split("\n")
            .Select(line =>
            {
                var left = line.Split(",")[0];
                var right = line.Split(",")[1];
                var leftParsed = left.Split("-").Select(n => Convert.ToInt32(n)).ToArray();
                var rightParsed = right.Split("-").Select(n => Convert.ToInt32(n)).ToArray();
                return new int[] { leftParsed[0], leftParsed[1], rightParsed[0], rightParsed[1] };
            })
            .Where(ns => Overlaps(ns[0], ns[1], ns[2], ns[3]))
            .Count()
            .ToString();
    }

    private bool IsFullyContained(int leftMin, int leftMax, int rightMin, int rightMax) =>
        leftMin <= rightMin && leftMax >= rightMax || rightMin <= leftMin && rightMax >= leftMax;

    private bool Overlaps(int leftMin, int leftMax, int rightMin, int rightMax) =>
        leftMin <= rightMin && leftMax >= rightMin || rightMin <= leftMin && rightMax >= leftMin;
}
