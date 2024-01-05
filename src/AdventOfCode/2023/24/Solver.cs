using Extreme.Mathematics;

namespace AdventOfCode.Y2023.D24;

internal class Solver : ISolver
{
    public object PartOne(string input)
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
                return new Hail(Position: vectors[0], Velocity: vectors[1]);
            })
            .ToArray();

        decimal min = 200_000_000_000_000;
        decimal max = 400_000_000_000_000;
        bool IsWithinBounds(Vector2 vector) =>
            min <= vector.X0 && vector.X0 <= max && min <= vector.X1 && vector.X1 <= max;

        var futureIntersections = hails[0..^1].SelectMany(
            (hail, i) =>
                hails[(i + 1)..]
                    .Select(secondHail => FutureIntersection(hail, secondHail))
                    .Where(p => p.HasValue && IsWithinBounds(p.Value))
                    .Select(p => p!.Value)
        );

        return futureIntersections.Count();
    }

    static Vector2? FutureIntersection(Hail first, Hail second)
    {
        var vCrossProduct = CrossProduct(first.Velocity, second.Velocity);
        var areParallel = vCrossProduct == 0;
        if (areParallel)
            return null;

        var firstT = CrossProduct(second.Position - first.Position, second.Velocity) / vCrossProduct;
        var secondT = CrossProduct(first.Position - second.Position, first.Velocity) / (vCrossProduct * (-1));
        if (firstT < 0 || secondT < 0)
            return null;

        return first.Position + firstT * first.Velocity;
    }

    static decimal CrossProduct(Vector2 a, Vector2 b) => a.X0 * b.X1 - b.X0 * a.X1;

    /// Blatant ripoff of https://github.com/encse/adventofcode/tree/master/2023/Day24
    public object PartTwo(string input)
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
                        return (x: nums[0], y: nums[1], z: nums[2]);
                    })
                    .ToArray();
                return (pos: vectors[0], vel: vectors[1]);
            })
            .ToArray();

        var hailsXY = hails
            .Select(h => new Hail(new Vector2(h.pos.x, h.pos.y), new Vector2(h.vel.x, h.vel.y)))
            .ToArray();
        var hailsXZ = hails
            .Select(h => new Hail(new Vector2(h.pos.x, h.pos.z), new Vector2(h.vel.x, h.vel.z)))
            .ToArray();

        var stoneXY = Solve2D(hailsXY);
        var stoneXZ = Solve2D(hailsXZ);
        return stoneXY.X0 + stoneXY.X1 + stoneXZ.X1;
    }

    static Vector2 Solve2D(Hail[] hails)
    {
        // Try to guess the velocity of our stone (a for loop), then supposing
        // that it is the right velocity we create a new reference frame that
        // moves with that speed. The stone doesn't move in this frame, it has
        // some fixed unknown coordinate. Now transform each particle into
        // this reference frame as well. Since the stone is not moving, if we
        // properly guessed the speed, we find that each particle meets at the
        // same point. This must be the stone's location.

        Hail TranslateVelocity(Hail hail, Vector2 vel) => new Hail(hail.Position, hail.Velocity - vel);

        var s = 500; //arbitrary +- limit for the brute force
        for (var v1 = -s; v1 < s; v1++)
        {
            for (var v2 = -s; v2 < s; v2++)
            {
                var vel = new Vector2(v1, v2);

                var stone = Intersection(TranslateVelocity(hails[0], vel), TranslateVelocity(hails[1], vel));
                if (!stone.HasValue)
                    continue;

                if (hails.All(h => IntersectsPoint(TranslateVelocity(h, vel), stone.Value)))
                {
                    return stone.Value;
                }
            }
        }
        throw new Exception("No suitable stone found");
    }

    static Vector2? Intersection(Hail first, Hail second)
    {
        var vCrossProduct = CrossProduct(first.Velocity, second.Velocity);
        var areParallel = vCrossProduct == 0;
        if (areParallel)
            return null;

        var firstT = CrossProduct(second.Position - first.Position, second.Velocity) / vCrossProduct;

        return first.Position + firstT * first.Velocity;
    }

    static bool IntersectsPoint(Hail hail, Vector2 point)
    {
        var d = (point.X0 - hail.Position.X0) * hail.Velocity.X1 - (point.X1 - hail.Position.X1) * hail.Velocity.X0;
        return Math.Abs(d) < (decimal)0.0001;
    }
}

public record struct Hail(Vector2 Position, Vector2 Velocity);

public readonly struct Vector2(decimal x0, decimal x1)
{
    public readonly decimal X0 = x0;
    public readonly decimal X1 = x1;

    public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.X0 + right.X0, left.X1 + right.X1);

    public static Vector2 operator -(Vector2 left, Vector2 right) => new(left.X0 - right.X0, left.X1 - right.X1);

    public static Vector2 operator *(decimal scalar, Vector2 vector) => vector * scalar;

    public static Vector2 operator *(Vector2 vector, decimal scalar) => new(vector.X0 * scalar, vector.X1 * scalar);
}
