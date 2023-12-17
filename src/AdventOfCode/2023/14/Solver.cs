using AdventOfCode.Common;

namespace AdventOfCode.Y2023.D14;

internal class Solver : ISolver
{
    public string PartOne(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();
        TiltNorth(m);
        return m.Select((row, i) => row.Count(c => c == 'O') * (row.Length - i)).Sum().ToString();
    }

    public string PartTwo(string input)
    {
        var m = input.Split('\n').Select(line => line.ToCharArray()).ToArray();

        var seen = new Dictionary<char[][], int>(new Char2dArrayComparer());
        var i = 0;
        int seqStart;
        while (!seen.TryGetValue(m, out seqStart))
        {
            seen[m] = i++;
            Cycle(m);
        }
        var seqLength = seen.Count - seqStart;
        var itersLeft = (1000_000_000 - seqStart) % (seqLength);

        for (var k = 0; k < itersLeft; k++)
            Cycle(m);

        return m.Select((row, i) => row.Count(c => c == 'O') * (row.Length - i)).Sum().ToString();
    }

    static void Cycle(char[][] m)
    {
        for (var j = 0; j < 4; j++)
        {
            TiltNorth(m);
            Rotate90Clockwise(m);
        }
    }

    static void TiltNorth(char[][] m)
    {
        for (var col = 0; col < m[0].Length; col++)
        {
            var l = 0;
            while (l < m.Length)
            {
                while (l < m.Length && m[l][col] == '#')
                    l++;
                var roundRocks = 0;
                var emptySpaces = 0;
                var r = l;
                while (r < m.Length && m[r][col] != '#')
                {
                    if (m[r][col] == 'O')
                        roundRocks++;
                    else if (m[r][col] == '.')
                        emptySpaces++;
                    r++;
                }
                for (var i = 0; i < roundRocks; i++)
                {
                    m[l + i][col] = 'O';
                }
                for (var i = 1; i <= emptySpaces; i++)
                {
                    m[r - i][col] = '.';
                }
                l = r + 1;
            }
        }
    }

    static void Rotate90Clockwise(char[][] m)
    {
        var n = m.Length;
        if (n == 0 || n != m[0].Length)
            throw new ArgumentOutOfRangeException(nameof(m), "Must be a NxN matrix");
        for (var i = 0; i < n / 2; i++)
        {
            for (var j = i; j < n - 1 - i; j++)
            {
                var upper = (i, j);
                var left = (n - 1 - j, i);
                var right = (j, n - 1 - i);
                var lower = (n - 1 - i, n - 1 - j);

                Swap(m, upper, left);
                Swap(m, left, right);
                Swap(m, left, lower);
            }
        }
    }

    static void Swap(char[][] m, (int Row, int Col) a, (int Row, int Col) b)
    {
        var tmp = m[b.Row][b.Col];
        m[b.Row][b.Col] = m[a.Row][a.Col];
        m[a.Row][a.Col] = tmp;
    }
}

public class Char2dArrayComparer : IEqualityComparer<char[][]>
{
    public bool Equals(char[][]? x, char[][]? y)
    {
        if (x is null && y is null)
            return true;
        if (x is null || y is null)
            return false;

        return x.SequenceEqual(y, new CharArrayEqualityComparer());
    }

    public int GetHashCode(char[][] obj)
    {
        int hashCode = 17;
        foreach (var row in obj)
        {
            hashCode = hashCode * 31 + new String(row).GetHashCode();
        }
        return hashCode;
    }
}

public class CharArrayEqualityComparer : IEqualityComparer<char[]>
{
    public bool Equals(char[]? x, char[]? y)
    {
        if (x is null && y is null)
            return true;
        if (x is null || y is null)
            return false;

        return x.SequenceEqual(y);
    }

    public int GetHashCode(char[] obj)
    {
        return new string(obj).GetHashCode();
    }
}
