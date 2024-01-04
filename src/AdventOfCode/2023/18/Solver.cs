using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using AdventOfCode.Common;

namespace AdventOfCode.Y2023.D18;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var instructions = input
            .Split('\n')
            .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            .Select(segments => (segments[0], int.Parse(segments[1])));

        return LagoonLavaCapacity(instructions);
    }

    public object PartTwo(string input)
    {
        var instructions = input.Split('\n').Select(line => line.Split("#")[1][0..^1]).Select(ParseHexadecimal);

        return LagoonLavaCapacity(instructions);
    }

    static BigInteger LagoonLavaCapacity(IEnumerable<(string, int)> instructions)
    {
        var pos = new Vector2d(0, 0);
        List<Vector2d> path = [pos];
        foreach (var (dir, length) in instructions)
        {
            var dirVector = dir switch
            {
                "U" => Vector2d.Up,
                "R" => Vector2d.Right,
                "D" => Vector2d.Down,
                "L" => Vector2d.Left,
                _ => throw new ArgumentOutOfRangeException()
            };
            for (var i = 0; i < length; i++)
            {
                pos += dirVector;
                path.Add(pos);
            }
        }
        return CalcArea(path);
    }

    static BigInteger CalcArea(List<Vector2d> polygonPoints)
    {
        var shoelaceArea =
            polygonPoints.Pairwise((a, b) => a.X * b.Y - b.X * a.Y).Aggregate(BigInteger.Zero, (a, b) => a + b) / 2;
        return shoelaceArea + polygonPoints.Count / 2 + 1;
    }

    (string Direction, int Length) ParseHexadecimal(string hexadecimal)
    {
        if (hexadecimal.Length != 6)
            throw new ArgumentOutOfRangeException();
        var length = int.Parse(hexadecimal[0..^1], System.Globalization.NumberStyles.HexNumber);
        var direction = (hexadecimal[^1] - '0') switch
        {
            0 => "R",
            1 => "D",
            2 => "L",
            3 => "U",
            _ => throw new ArgumentOutOfRangeException()
        };
        return (direction, length);
    }

    void Print(IList<Vector2d> points)
    {
        var xMax = points.MaxBy(p => p.X).X;
        var xMin = points.MinBy(p => p.X).X;
        var yMax = points.MaxBy(p => p.Y).Y;
        var yMin = points.MinBy(p => p.Y).Y;

        var sb = new StringBuilder();
        for (var y = yMin; y <= yMax; y++)
        {
            for (var x = xMin; x <= xMax; x++)
            {
                var idx = points.IndexOf(new Vector2d(x: x, y: y));
                sb.Append($"{(idx > -1 ? "#" : ".")}");
            }
            sb.Append('\n');
        }

        Console.WriteLine(sb.ToString());
    }
}
