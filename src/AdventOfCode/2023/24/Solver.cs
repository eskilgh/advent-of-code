using AdventOfCode.Common;
using AdventOfCode.Y2022.D12;
using Extreme.Mathematics;
using MathNet.Numerics.LinearAlgebra.Complex;
using Microsoft.VisualBasic;

namespace AdventOfCode.Y2023.D24;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var hails = input
            .Split('\n')
            .Select(line =>
            {
                var vectors = line.Split('@', StringSplitOptions.TrimEntries)
                    .Select(section =>
                    {
                        var nums = section
                            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(long.Parse)
                            .ToArray();
                        return new Vector2(nums[0], nums[1]);
                    })
                    .ToArray();
                return (p: vectors[0], v: vectors[1]);
            })
            .ToArray();

        BigFloat min = 2e14;
        BigFloat max = 4e14;
        bool IsWithinBounds(Vector2 vector) => min <= vector.X && vector.X <= max && min <= vector.Y && vector.Y <= max;

        var futureIntersections = hails[0..^1].SelectMany(
            (hail, i) =>
                hails[(i + 1)..]
                    .Select(secondHail => FutureIntersection(hail, secondHail))
                    .Where(p => p.HasValue && IsWithinBounds(p.Value))
                    .Select(p => p!.Value)
        );

        return futureIntersections.Count().ToString();
    }

    static Vector2? FutureIntersection((Vector2 p, Vector2 v) first, (Vector2 p, Vector2 v) second)
    {
        var vCrossProduct = CrossProduct(first.v, second.v);
        var areParallel = vCrossProduct == BigFloat.Zero;
        if (areParallel)
            return null;

        var firstT = CrossProduct(second.p - first.p, second.v) / vCrossProduct;
        var secondT = CrossProduct(first.p - second.p, first.v) / (vCrossProduct * (-1));
        if (firstT < BigFloat.Zero || secondT < BigFloat.Zero)
            return null;

        return first.p + firstT * first.v;
    }

    static BigFloat CrossProduct(Vector2 a, Vector2 b) => a.X * b.Y - b.X * a.Y;

    public string PartTwo(string input)
    {
        var hails = input
            .Split('\n')
            .Select(line =>
            {
                var vectors = line.Split('@', StringSplitOptions.TrimEntries)
                    .Select(section =>
                    {
                        return section
                            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(decimal.Parse)
                            .ToArray();
                    })
                    .ToArray();

                return (p: vectors[0], v: vectors[1]);
            })
            // Only need three lines to setup the equation
            .Take(3)
            .ToArray();

        if (!(hails is [var A, var B, var C])) throw new InvalidOperationException();
        return "Not available";
    }


}

public readonly struct Vector2(BigFloat x, BigFloat y)
{
    public readonly BigFloat X = x;
    public readonly BigFloat Y = y;

    public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator -(Vector2 left, Vector2 right) => new(left.X - right.X, left.Y - right.Y);

    public static Vector2 operator *(BigFloat scalar, Vector2 vector) => vector * scalar;

    public static Vector2 operator *(Vector2 vector, BigFloat scalar) =>
        new(vector.X * scalar, vector.Y * scalar);
}
