using AdventOfCode.Common;

namespace AdventOfCode.Y2020.D17;

record struct Cube3d(int X, int Y, int Z);
record struct Cube4d(int X, int Y, int Z, int W);

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var activeCubes = input
            .Split('\n')
            .SelectMany(
                (line, y) =>
                    line.ToCharArray()
                        .WithIndex()
                        .Where(tpl => tpl.Element == '#')
                        .Select(tpl => new Cube3d(X: tpl.Index, Y: y, Z: 0))
            )
            .ToHashSet();

        var cycles = 0;

        while (cycles < 6)
        {
            var numNeighbours = new Dictionary<Cube3d, int>();
            foreach (var cube in activeCubes)
            {
                var neighbours = GetNeighbours(cube);
                foreach (var neighbour in neighbours)
                {
                    if (numNeighbours.ContainsKey(neighbour))
                        numNeighbours[neighbour]++;
                    else
                        numNeighbours[neighbour] = 1;
                }
            }
            activeCubes = numNeighbours
                .Where(kvp => kvp.Value == 3 || (kvp.Value == 2 && activeCubes.Contains(kvp.Key)))
                .Select(kvp => kvp.Key)
                .ToHashSet();
            cycles++;
        }

        return activeCubes.Count();
    }

    public object PartTwo(string input)
    {
        var activeCubes = input
            .Split('\n')
            .SelectMany(
                (line, y) =>
                    line.ToCharArray()
                        .WithIndex()
                        .Where(tpl => tpl.Element == '#')
                        .Select(tpl => new Cube4d(X: tpl.Index, Y: y, Z: 0, W: 0))
            )
            .ToHashSet();

        var cycles = 0;

        while (cycles < 6)
        {
            var numNeighbours = new Dictionary<Cube4d, int>();
            foreach (var cube in activeCubes)
            {
                var neighbours = GetNeighbours(cube);
                foreach (var neighbour in neighbours)
                {
                    if (numNeighbours.ContainsKey(neighbour))
                        numNeighbours[neighbour]++;
                    else
                        numNeighbours[neighbour] = 1;
                }
            }
            activeCubes = numNeighbours
                .Where(kvp => kvp.Value == 3 || (kvp.Value == 2 && activeCubes.Contains(kvp.Key)))
                .Select(kvp => kvp.Key)
                .ToHashSet();
            cycles++;
        }

        return activeCubes.Count();
    }

    static IEnumerable<Cube3d> GetNeighbours(Cube3d cube)
    {
        return from dx in Enumerable.Range(-1, 3)
            from dy in Enumerable.Range(-1, 3)
            from dz in Enumerable.Range(-1, 3)
            where !(dx is 0 && dy is 0 && dz is 0)
            select new Cube3d(X: cube.X + dx, Y: cube.Y + dy, Z: cube.Z + dz);
    }

    static IEnumerable<Cube4d> GetNeighbours(Cube4d cube)
    {
        return from dx in Enumerable.Range(-1, 3)
               from dy in Enumerable.Range(-1, 3)
               from dz in Enumerable.Range(-1, 3)
               from dw in Enumerable.Range(-1, 3)
               where !(dx is 0 && dy is 0 && dz is 0 && dw is 0)
               select new Cube4d(X: cube.X + dx, Y: cube.Y + dy, Z: cube.Z + dz, W: cube.W + dw);
    }
}
