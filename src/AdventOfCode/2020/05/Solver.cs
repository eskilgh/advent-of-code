using AdventOfCode.Common;

namespace AdventOfCode.Y2020.D05;

record Seat(int Row, int Column)
{
    public int Id => Row * 8 + Column;
}

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var seats = ParseSeats(input);
        return seats.Max(seat => seat.Id);
    }

    public object PartTwo(string input)
    {
        var seats = ParseSeats(input).ToList();
        seats.Sort((a, b) => a.Id - b.Id);
        var seatIdStart = seats.First().Id;
        var missingSeat = seats.Where((seat, i) => seat.Id != (i + seatIdStart)).First().Id - 1;
        return missingSeat;
    }

    private static IEnumerable<Seat> ParseSeats(string input)
    {
        return input
            .Split('\n')
            .Select(line =>
            {
                var row = Convert.ToInt32(line[0..7].Replace('B', '1').Replace('F', '0'), 2);
                var column = Convert.ToInt32(line[7..].Replace('R', '1').Replace('L', '0'), 2);
                return new Seat(row, column);
            });
    }
}
