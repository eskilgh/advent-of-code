using System.Drawing;
using System.Text;
using AdventOfCode.Common;

namespace AdventOfCode.Y2023.D22;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var bricks = ParseBricks(input);
        bricks.Sort((a, b) => Math.Min(a.Z0, a.Z1) - Math.Min(b.Z0, b.Z1));

        var collapsed = Collapse(bricks);
        var notSafeToDisintegrate = FindBricksNotSafeToDisintegrate(collapsed);
        return bricks.Count - notSafeToDisintegrate.Count;
    }

    private static HashSet<Brick> FindBricksNotSafeToDisintegrate(Brick[] collapsed)
    {
        HashSet<Brick> notSafeToDisintegrate = [];
        for (var i = 0; i < collapsed.Length; i++)
        {
            var current = collapsed[i];
            var bricksBelow = collapsed[..i].Where(b => Math.Max(b.Z0, b.Z1) == Math.Min(current.Z0, current.Z1) - 1);
            var supportingBricks = bricksBelow.Where(b => Overlaps(current, b));
            if (supportingBricks.Count() == 1)
                notSafeToDisintegrate.Add(supportingBricks.Single());
        }

        return notSafeToDisintegrate;
    }

    private static List<Brick> ParseBricks(string input)
    {
        return input
            .Split('\n')
            .Select(
                (line, i) =>
                {
                    var coords = line.Split('~').Select(part => part.Split(',').Select(int.Parse).ToArray()).ToArray();
                    var (x0, y0, z0) = (coords[0][0], coords[0][1], coords[0][2]);
                    var (x1, y1, z1) = (coords[1][0], coords[1][1], coords[1][2]);
                    return new Brick(i, x0, y0, z0, x1, y1, z1);
                }
            )
            .ToList();
    }

    /// <summary>
    /// Collapses the provided bricks.
    /// </summary>
    /// <param name="bricks">Bricks to collapse</param>
    /// <param name="startFrom">When provided, will start collapsing from bricks with an index gte. to this</param>
    /// <returns>A new array with the collapsed bricks</returns>
    static Brick[] Collapse(List<Brick> bricks, int startFrom = 0)
    {
        var collapsedBricks = new Brick[bricks.Count];
        for (var i = 0; i < startFrom; i++)
        {
            collapsedBricks[i] = bricks[i];
        }
        for (var i = startFrom; i < bricks.Count; i++)
        {
            var current = bricks[i];
            var newZ0 =
                collapsedBricks
                    .Where(b => Overlaps(current, b))
                    .Select(b => Math.Max(b.Z0, b.Z1))
                    // If no supporting brick, will default to Z of 0
                    .DefaultIfEmpty()
                    .Max() + 1;
            var currentCollapsed = current with { Z0 = newZ0, Z1 = newZ0 + (current.Z1 - current.Z0) };
            collapsedBricks[i] = currentCollapsed;
        }
        return collapsedBricks;
    }

    static bool Overlaps(Brick a, Brick b)
    {
        var xOverlap = Math.Max(0, (Math.Min(a.X1, b.X1) - Math.Max(a.X0, b.X0) + 1));
        var yOverlap = Math.Max(0, (Math.Min(a.Y1, b.Y1) - Math.Max(a.Y0, b.Y0) + 1));
        return xOverlap * yOverlap > 0;
    }

    public object PartTwo(string input)
    {
        var bricks = ParseBricks(input);
        bricks.Sort((a, b) => Math.Min(a.Z0, a.Z1) - Math.Min(b.Z0, b.Z1));
        var collapsed = Collapse(bricks);
        // Just a lazy way to have a fast method to compare against the original collapsed bricks
        var collapsedSet = collapsed.ToHashSet();

        // Only the destruction of 'non-safe' bricks will affect the other bricks.
        // For all of these: Destroy them and Collapse again, then check how many bricks were affected
        return FindBricksNotSafeToDisintegrate(collapsed)
            .Select(brickToDestroy =>
            {
                var i = Array.IndexOf(collapsed, brickToDestroy);
                var numAffected = Collapse([..collapsed[0..i], ..collapsed[(i + 1)..]], startFrom: i)
                    .Count(b => !collapsedSet.Contains(b));
                return numAffected;
            })
            .Sum();
    }

    private void Print(Brick[] bricks)
    {
        var xMin = bricks.Select(brick => Math.Min(brick.X0, brick.X1)).Min();
        var xMax = bricks.Select(brick => Math.Max(brick.X0, brick.X1)).Max();
        var zMin = 0;
        var zMax = bricks.Select(brick => Math.Max(brick.Z0, brick.Z1)).Max();

        var height = zMax - zMin + 1;
        var width = xMax - xMin + 1;
        var m = Enumerable.Range(0, height).Select(_ => Enumerable.Repeat('.', width).ToArray()).ToArray();

        foreach (var brick in bricks)
        {
            for (var z = Math.Min(brick.Z0, brick.Z1); z <= Math.Max(brick.Z0, brick.Z1); z++)
            {
                var row = height - z - 1;
                for (var col = Math.Min(brick.X0, brick.X1); col <= Math.Max(brick.X0, brick.X1); col++)
                {
                    m[row][col] = (char)('A' + brick.Index);
                }
            }
        }
        m.Print();
    }
}

public record struct Brick(int Index, int X0, int Y0, int Z0, int X1, int Y1, int Z1);
