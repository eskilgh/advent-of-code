using System.Numerics;
using System.Text;

namespace AdventOfCode.Y2022.D09;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var instructions = ParseInstructions(input);

        var knot = Enumerable.Range(0, 2).Select(_ => new Vector2(0, 0)).ToList();
        var visitedPositions = new HashSet<Vector2> { knot[^1] };

        foreach (var (direction, steps) in instructions)
        {
            for (var i = 0; i < steps; i++)
            {
                knot = PerformStep(knot, direction);
                visitedPositions.Add(knot[^1]);
            }
        }
        return visitedPositions.Count;
    }

    public object PartTwo(string input)
    {
        var instructions = ParseInstructions(input);

        var knot = Enumerable.Range(0, 10).Select(_ => new Vector2(0, 0)).ToList();
        var visitedPositions = new HashSet<Vector2> { knot[^1] };

        foreach (var (direction, steps) in instructions)
        {
            for (var i = 0; i < steps; i++)
            {
                knot = PerformStep(knot, direction);
                visitedPositions.Add(knot[^1]);
            }
        }
        return visitedPositions.Count;
    }

    private static IEnumerable<(string Direction, int Steps)> ParseInstructions(string input) =>
        input.Split("\n").Select(line => (Direction: line.Split(" ")[0], Steps: int.Parse(line.Split(" ")[1])));

    private static List<Vector2> PerformStep(List<Vector2> knot, string direction)
    {
        var directionVector = direction.ToVector();

        return knot.TakeLast(knot.Count - 1)
            .Aggregate(
                new List<Vector2> { knot[0] + directionVector },
                (knot, current) => knot.Append(MoveTail(knot[^1], current)).ToList()
            );
    }

    private static Vector2 MoveTail(Vector2 head, Vector2 tail)
    {
        var difference = head - tail;

        return difference.Length() switch
        {
            < 2.0F => tail,
            _ => tail + difference.ToDirectionVector(),
        };
    }
}

file static class Utils
{
    /// <summary>
    /// Returns a vector where each component of the original vector
    /// has been been normalized. I.e. <br/>
    /// (-5, 2) => (-1, 1) <br/>
    /// (0, -3) => (0, -1) <br/>
    /// (1, 2) => (1, 1)
    /// </summary>
    public static Vector2 ToDirectionVector(this Vector2 vector)
    {
        var x = vector.X == 0 ? vector.X : vector.X / Math.Abs(vector.X);
        var y = vector.Y == 0 ? vector.Y : vector.Y / Math.Abs(vector.Y);
        return new Vector2(x, y);
    }
}

file static class Extensions
{
    public static string ToGridString(this (Vector2 Head, Vector2 Tail) vs)
    {
        var xMax = (int)Math.Max(vs.Head.X, vs.Tail.X);
        var yMax = (int)Math.Max(vs.Head.Y, vs.Tail.Y);

        var sb = new StringBuilder();

        var OccupiesPosition = (int row, int col, Vector2 v) => v.X == col && v.Y == yMax - row;
        for (var row = 0; row <= yMax; row++)
        {
            for (var col = 0; col <= xMax; col++)
            {
                if (OccupiesPosition(row, col, vs.Head))
                    sb.Append('H');
                else if (OccupiesPosition(row, col, vs.Tail))
                    sb.Append('T');
                else if (row == yMax && col == 0)
                    sb.Append('s');
                else
                    sb.Append('.');
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }

    public static Vector2 ToVector(this string direction) =>
        direction switch
        {
            "U" => new Vector2(0, 1),
            "L" => new Vector2(-1, 0),
            "D" => new Vector2(0, -1),
            "R" => new Vector2(1, 0),
            _ => throw new ArgumentException("")
        };
}
