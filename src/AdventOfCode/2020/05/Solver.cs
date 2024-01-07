namespace AdventOfCode.Y2020.D05;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var seats = ParseSeatIds(input);
        return seats.Max();
    }

    public object PartTwo(string input)
    {
        var seatIds = ParseSeatIds(input);
        var allSeatIds = Enumerable.Range(seatIds.Min(), seatIds.Count()).ToHashSet();
        return allSeatIds.Except(seatIds).Single();
    }

    private static IEnumerable<int> ParseSeatIds(string input)
    {
        return input
            .Replace('B', '1')
            .Replace('F', '0')
            .Replace('R', '1')
            .Replace('L', '0')
            .Split('\n')
            .Select(binaryNumber => Convert.ToInt32(binaryNumber, 2));
    }
}
