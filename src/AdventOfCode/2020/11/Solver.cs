using System.Numerics;
using System.Text;
using AdventOfCode.Common;
using Extreme.Mathematics.LinearAlgebra;

namespace AdventOfCode.Y2020.D11;

internal class Solver : ISolver
{
    static Complex[] Directions =
    [
        -Complex.ImaginaryOne,
        new(1, -1),
        Complex.One,
        new(1, 1),
        Complex.ImaginaryOne,
        new(-1, 1),
        -Complex.One,
        new(-1, -1)
    ];

    public object PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        var GetAdjacent = (char[][] m, Complex pos) =>
            Directions.Select(dir => pos + dir).Where(z => !m.IsOutOfBounds(z));

        while (Iterate(m, MapNearbyOccupiedSeats(m, GetAdjacent), threshold: 4))
            ;

        return m.SelectMany(row => row.Where(c => c is '#')).Count();
    }

    public object PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        while (Iterate(m, MapNearbyOccupiedSeats(m, GetVisible), threshold: 5))
            ;
        return m.SelectMany(row => row.Where(c => c is '#')).Count();
    }

    static IEnumerable<Complex> GetVisible(char[][] m, Complex pos)
    {
        foreach (var dir in Directions)
        {
            var nextPos = pos + dir;
            while (!m.IsOutOfBounds(nextPos))
            {
                yield return nextPos;
                if (m[(int)nextPos.Imaginary][(int)nextPos.Real] is '#' or 'L')
                    break;
                nextPos += dir;
            }
        }
        yield break;
    }

    private static int[][] MapNearbyOccupiedSeats(
        char[][] m,
        Func<char[][], Complex, IEnumerable<Complex>> nearbyPositions
    )
    {
        // matrix to hold nearby seated chairs for all positions
        var a = m.Select(row => new int[row.Length]).ToArray();

        for (var row = 0; row < m.Length; row++)
        for (var col = 0; col < m[0].Length; col++)
        {
            var current = m[row][col];
            // For each seated chair, increase count of all nearby (adjacent or visible) positions
            if (current is '#')
            {
                var adjacent = nearbyPositions(m, new Complex(col, row));
                foreach (var c in adjacent)
                    a[(int)c.Imaginary][(int)c.Real] += 1;
            }
        }
        return a;
    }

    static bool Iterate(char[][] m, int[][] a, int threshold)
    {
        var numChanges = 0;
        for (var row = 0; row < m.Length; row++)
        for (var col = 0; col < m[0].Length; col++)
        {
            var current = m[row][col];
            var numAdjacent = a[row][col];
            var newValue = current switch
            {
                'L' when numAdjacent == 0 => '#',
                '#' when numAdjacent >= threshold => 'L',
                _ => current
            };
            if (current != newValue)
                numChanges++;
            m[row][col] = newValue;
        }
        return numChanges > 0;
    }
}
