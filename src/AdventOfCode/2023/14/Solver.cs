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
        return "Not available";
    }


    void TiltNorth(char[][] m)
    {
        for (var col = 0; col < m[0].Length; col++)
        {
            var l = 0;
            while (l < m.Length)
            {
                while (l < m.Length && m[l][col] == '#') l++;
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
}
