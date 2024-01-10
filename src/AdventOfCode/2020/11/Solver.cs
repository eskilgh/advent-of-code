using System.Numerics;
using System.Text;
using Extreme.Mathematics.LinearAlgebra;

namespace AdventOfCode.Y2020.D11;

internal class Solver : ISolver
{
    public object PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        while (Iterate(m))
            ;

        return m.SelectMany(row => row.Where(c => c is '#')).Count();
    }

    static bool Iterate(char[][] m)
    {
        var a = MapAdjacent(m);

        var numChanges = 0;
        for (var row = 0; row < m.Length; row++)
        for (var col = 0; col < m[0].Length; col++)
        {
            var current = m[row][col];
            var numAdjacent = a[row][col];
            var newValue = current switch
            {
                'L' when numAdjacent == 0 => '#',
                '#' when numAdjacent >= 4 => 'L',
                _ => current
            };
            if (current != newValue)
                numChanges++;
            m[row][col] = newValue;
        }
        return numChanges > 0;
    }

    private static int[][] MapAdjacent(char[][] m)
    {
        // For each seated char, increase count of all adjacent positions
        var a = m.Select(row => new int[row.Length]).ToArray();

        for (var row = 0; row < m.Length; row++)
        for (var col = 0; col < m[0].Length; col++)
        {
            var current = m[row][col];
            if (current is '#')
            {
                var adjacent = GetAdjacent(new Complex(col, row), m);
                foreach (var c in adjacent)
                    a[(int)c.Imaginary][(int)c.Real] += 1;
            }
        }
        return a;
    }

    static void Print(int[][] a)
    {
        var sb = new StringBuilder();
        for (var row = 0; row < a.Length; row++)
        {
            for (var col = 0; col < a[0].Length; col++)
                sb.Append(a[row][col]);
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    static void Print(char[][] a)
    {
        var sb = new StringBuilder();
        for (var row = 0; row < a.Length; row++)
        {
            for (var col = 0; col < a[0].Length; col++)
                sb.Append(a[row][col]);
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    static IEnumerable<Complex> GetAdjacent(Complex c, char[][] m)
    {
        Complex[] directions =
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
        var iMax = m.Length - 1;
        var rMax = m[0].Length - 1;
        return directions
            .Select(dir => c + dir)
            .Where(z => 0 <= z.Real && z.Real <= rMax && 0 <= z.Imaginary && z.Imaginary <= iMax);
    }

    public object PartTwo(string input)
    {
        return "Not available";
    }
}
